using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CDCGameKit
{
    public class ObjectPool
    {
        //--------------------------------------------------------------------
        // Usage Notes
        //--------------------------------------------------------------------
        // Assumptions:
        // - Constructor assumes managed object is a prefab outside of the scene
        //--------------------------------------------------------------------
        // Operations:
        // - GetOne() and MakeAvailable(int) only instantiate
        // - Deactivation is automatic and happens on the updateCycle
        // - SetActive() and other state changes are NOT HANDLED by default
        //--------------------------------------------------------------------

        private readonly GameObject _object;
        public List<GameObject> activePool { get; private set; }
        private List<GameObject> inactivePool { get; set; }

        public ObjectPool(GameObject managedObject)
        {
            _object = managedObject;

            activePool = new List<GameObject>();
            inactivePool = new List<GameObject>();
        }

        public delegate bool IsReadyToDeactivate(GameObject toTest);
        public delegate void CreateHandler(GameObject justCreated);
        public delegate void OnActivateHandler(GameObject toActivate);
        public delegate void OnDeactivateHandler(GameObject toDeactivate);

        private IsReadyToDeactivate isReadyToDeactivate;
        private CreateHandler createHandler;
        private OnActivateHandler onActivateHandlers;
        private OnDeactivateHandler onDeactivateHandlers;

        public bool isReady
        {
            get
            {
                return (isReadyToDeactivate != null && createHandler != null && onActivateHandlers != null && onDeactivateHandlers != null);
            }
        }

        public bool isManaged(GameObject toTest)
        {
            bool inActive = activePool.Contains(toTest);
            bool inInactive = inactivePool.Contains(toTest);

            return (inActive || inInactive);
        }

        public void Initialize(IsReadyToDeactivate readyToDeactivate, CreateHandler createHandler, OnActivateHandler onActivate, OnDeactivateHandler onDeactivate)
        {
            isReadyToDeactivate = readyToDeactivate;
            this.createHandler = createHandler;
            if (onActivate != null) onActivateHandlers = onActivate;
            if (onDeactivate != null) onDeactivateHandlers = onDeactivate;
        }

        public void ChangeDeactivateTest(IsReadyToDeactivate readyToDeactivate)
        {
            isReadyToDeactivate = readyToDeactivate;
        }

        public void ChangeCreateHandler(CreateHandler createHandler)
        {
            this.createHandler = createHandler;
        }

        public void AddHandlers(OnActivateHandler onActivate, OnDeactivateHandler onDeactivate)
        {
            if (onActivate != null) onActivateHandlers += onActivate;
            if (onDeactivate != null) onDeactivateHandlers += onDeactivate;
        }

        public void RemoveHandlers(OnActivateHandler onActivate, OnDeactivateHandler onDeactivate)
        {
            if (onActivate != null) onActivateHandlers -= onActivate;
            if (onDeactivate != null) onDeactivateHandlers -= onDeactivate;
        }

        public GameObject GetOne()
        {
            GameObject toReturn;
            if (inactivePool.Count > 0)
            {
                toReturn = inactivePool[0];
                inactivePool.Remove(toReturn);
            }
            else
            {
                toReturn = GameObject.Instantiate(_object);
                createHandler(toReturn);
            }
            activePool.Add(toReturn);
            onActivateHandlers(toReturn);

            return toReturn;
        }
        private void DeactivateLast()
        {
            int lastIndex = activePool.Count - 1;
            var objectToDeactivate = activePool[lastIndex];
            onDeactivateHandlers(objectToDeactivate);
            activePool.Remove(objectToDeactivate);
            inactivePool.Add(objectToDeactivate);
        }

        public void MakeAvailable(int count)
        {
            if (activePool.Count == count) return;
            else if (activePool.Count < count)
            {
                int delta = count - activePool.Count;
                for (int i = 0; i < delta; i++) GetOne();
            }
            else if (activePool.Count > count)
            {
                for (int i = activePool.Count - 1; i >= count; i--) DeactivateLast();
            }
        }

        public void Update()
        {
            if (activePool == null) return;
            if (activePool.Count == 0) return;

            for (int i = activePool.Count - 1; i >= 0; i--)
            {
                var o = activePool[i];
                if (isReadyToDeactivate(o))
                {
                    onDeactivateHandlers(o);
                    activePool.Remove(o);
                    inactivePool.Add(o);
                }
            }
        }

        public void Cleanup()
        {
            if (activePool.Count > 0)
                for (int i = activePool.Count - 1; i >= 0; i--)
                {
                    var toDestroy = activePool[i];
                    activePool.Remove(toDestroy);
                    GameObject.DestroyImmediate(toDestroy);
                }

            if (inactivePool.Count > 0)
                for (int i = inactivePool.Count - 1; i >= 0; i--)
                {
                    var toDestroy = inactivePool[i];
                    activePool.Remove(toDestroy);
                    GameObject.DestroyImmediate(toDestroy);
                }
        }
    }
}