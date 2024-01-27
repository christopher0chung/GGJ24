using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CDCGameKit
{
	public class Task
	{
		public enum TaskStatus : byte
		{
			Detached, // Task has not been attached to a TaskManager     
			Pending, // Task has not been initialized     
			Working, // Task has been initialized     
			Success, // Task completed successfully     
			Fail, // Task completed unsuccessfully     
			Aborted // Task was aborted 
		}

		public TaskStatus Status { get; private set; }

		public bool IsDetached { get { return Status == TaskStatus.Detached; } }
		public bool IsAttached { get { return Status != TaskStatus.Detached; } }
		public bool IsPending { get { return Status == TaskStatus.Pending; } }
		public bool IsWorking { get { return Status == TaskStatus.Working; } }
		public bool IsSuccessful { get { return Status == TaskStatus.Success; } }
		public bool IsFailed { get { return Status == TaskStatus.Fail; } }
		public bool IsAborted { get { return Status == TaskStatus.Aborted; } }
		public bool IsFinished { get { return (Status == TaskStatus.Fail || Status == TaskStatus.Success || Status == TaskStatus.Aborted); } }

		public void Abort()
		{
			SetStatus(TaskStatus.Aborted);
		}

		internal void SetStatus(TaskStatus newStatus)
		{
			if (Status == newStatus) return;
			Status = newStatus;
			switch (newStatus)
			{
				case TaskStatus.Working:
					Initialize();
					break;
				case TaskStatus.Success:
					OnSuccess();
					CleanUp();
					break;
				case TaskStatus.Aborted:
					OnAbort();
					CleanUp();
					break;
				case TaskStatus.Fail:
					OnFail();
					CleanUp();
					break;
				// These are "internal" states that are relevant for         
				// the task manager         
				case TaskStatus.Detached:
				case TaskStatus.Pending:
					break;
				default:
					throw new System.ArgumentOutOfRangeException(newStatus.ToString(), newStatus, null);
			}
		}

		protected virtual void OnAbort() { }
		protected virtual void OnSuccess() { }
		protected virtual void OnFail() { }

		// Override this to handle initialization of the task. 
		// This is called when the task enters the Working state 

		protected virtual void Initialize() { }

		// Called whenever the TaskManager updates. Your tasks' work 
		// generally goes here 

		internal virtual void Update() { }

		// This is called when the tasks completes (i.e. is aborted, 
		// fails, or succeeds). It is called after the status change 
		// handlers are called 

		protected virtual void CleanUp() { }

		// Assign a task to be run if this task runs successfully 
		public Task NextTask { get; private set; }

		// Sets a task to be automatically attached when this one completes successfully 
		// NOTE: if a task is aborted or fails, its next task will not be queued 
		// NOTE: **DO NOT** assign attached tasks with this method. 
		public Task Then(Task task)
		{
			Debug.Assert(!task.IsAttached);
			NextTask = task;
			return task;
		}
	}
}