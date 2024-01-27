using System.Collections.Generic;
using UnityEngine;

namespace CDCGameKit
{
	public class TaskManager
	{

		// *************************** EXPLANATION *************************** 
		// How to reference:
		// This is a definition of a task manager - you can make instances
		// of a task manager in your own code, but you probably don't need to
		// change anything in here.
		// ******************************************************************* 

		private readonly List<Task> _tasks = new List<Task>();
        public int tasksRunning { get { return _tasks == null ? 0 : _tasks.Count; } }

		// Add a task
		public void Do(Task task)
		{
			Debug.Assert(task != null);
			Debug.Assert(!task.IsAttached);
			_tasks.Add(task);
			task.SetStatus(Task.TaskStatus.Pending);
		}

		public void Update()
		{
			// iterate through all the tasks
			for (int i = _tasks.Count - 1; i >= 0; --i)
			{
				Task task = _tasks[i];
				// Initialize tasks that have just been added                 
				if (task.IsPending)
				{
					task.SetStatus(Task.TaskStatus.Working);
				}

				// A task can finish during initialization               
				// so you need to check before the update 
				if (task.IsFinished)
				{
					HandleCompletion(task, i);
				}
				else
				{
					task.Update();
				}
			}
		}

		private void HandleCompletion(Task task, int taskIndex)
		{
			// If the finished task has a "next" task
			// queue it up - but only if the original task was     
			// successful     
			if (task.NextTask != null && task.IsSuccessful)
			{
				Do(task.NextTask);
			}
			// clear the task from the manager and let it know     
			// it's no longer being managed     
			_tasks.RemoveAt(taskIndex);
			task.SetStatus(Task.TaskStatus.Detached);
		}

		public bool HasTasks()
		{
			if (_tasks.Count == 0)
				return false;
			return true;
		}
	}
}
