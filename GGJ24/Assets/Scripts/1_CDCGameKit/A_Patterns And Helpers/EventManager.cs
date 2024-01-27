using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CDCGameKit
{
    public class EventManager
    {
        //---------------------
        // Creates singleton for ease of access
        //---------------------

        static private EventManager _instance;
        static public EventManager instance
        {
            get
            {
                if (_instance == null)
                    return _instance = new EventManager();
                else
                    return _instance;
            }
        }

        //---------------------
        // Storage of Events
        //---------------------

        private Dictionary<Type, EventMsg.Handler> registeredHandlers = new Dictionary<Type, EventMsg.Handler>();

        //---------------------
        // Register and Unregister
        //---------------------

        public void Register<T>(EventMsg.Handler handler) where T : EventMsg
        {
            Type type = typeof(T);
            if (registeredHandlers.ContainsKey(type))
            {
                registeredHandlers[type] += handler;
            }
            else
            {
                registeredHandlers[type] = handler;
            }
        }

        public void Unregister<T>(EventMsg.Handler handler) where T : EventMsg
        {
            Type type = typeof(T);
            EventMsg.Handler handlers;
            if (registeredHandlers.TryGetValue(type, out handlers))
            {
                handlers -= handler;
                if (handlers == null)
                {
                    registeredHandlers.Remove(type);
                }
                else
                {
                    registeredHandlers[type] = handlers;
                }
            }
        }

        //---------------------
        // Call event
        //---------------------

        public void Fire(EventMsg e)
        {
            Type type = e.GetType();
            EventMsg.Handler handlers;
            if (registeredHandlers.TryGetValue(type, out handlers))
            {
                handlers(e);
            }
        }

        //---------------------
        // Clear out
        //---------------------

        public void Clear()
        {
            registeredHandlers.Clear();
        }
    }
}