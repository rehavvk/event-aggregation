using System;
using System.Collections.Generic;

namespace Rehawk.EventAggregation
{
    public static class EventAggregator
    {
        public delegate void EventListener();
        public delegate void EventListener<in TPayload>(TPayload payload);
        
        private static readonly Dictionary<Type, List<Delegate>> listeners = new Dictionary<Type, List<Delegate>>();
        private static readonly Dictionary<Type, List<Delegate>> payloadListeners = new Dictionary<Type, List<Delegate>>();

        static EventAggregator()
        {
            listeners.Clear();
            payloadListeners.Clear();
        }
        
        public static void Publish<T>()
        {
            Publish<T>(Activator.CreateInstance<T>());
        }
        
        public static void Publish<T>(T payload)
        {
            if (listeners.TryGetValue(typeof(T), out List<Delegate> resultListenersA))
            {
                foreach (Delegate listener in resultListenersA)
                {
                    ((EventListener) listener).Invoke();
                }
            }
            
            if (payloadListeners.TryGetValue(typeof(T), out List<Delegate> resultListenersB))
            {
                foreach (Delegate listener in resultListenersB)
                {
                    ((EventListener<T>) listener).Invoke(payload);
                }
            }
        }
        
        public static bool IsSubscribed<T>(EventListener callback)
        {
            if (listeners.TryGetValue(typeof(T), out List<Delegate> resultListeners))
            {
                return resultListeners.Contains(callback);
            }

            return false;
        }
        
        public static bool IsSubscribed<T>(EventListener<T> callback)
        {
            if (payloadListeners.TryGetValue(typeof(T), out List<Delegate> resultListeners))
            {
                return resultListeners.Contains(callback);
            }

            return false;
        }
        
        public static void Subscribe<T>(EventListener callback)
        {
            if (!listeners.ContainsKey(typeof(T)))
            {
                listeners.Add(typeof(T), new List<Delegate>
                {
                    callback
                });   
            }
            else
            {
                listeners[typeof(T)].Add(callback);
            }
        }

        public static void Subscribe<T>(EventListener<T> callback)
        {
            if (!payloadListeners.ContainsKey(typeof(T)))
            {
                payloadListeners.Add(typeof(T), new List<Delegate>
                {
                    callback
                });   
            }
            else
            {
                payloadListeners[typeof(T)].Add(callback);
            }
        }

        public static void Unsubscribe<T>(EventListener callback)
        {
            if (listeners.TryGetValue(typeof(T), out List<Delegate> resultListeners))
            {
                resultListeners.Remove(callback);
            }
        }

        public static void Unsubscribe<T>(EventListener<T> callback)
        {
            if (payloadListeners.TryGetValue(typeof(T), out List<Delegate> resultListeners))
            {
                resultListeners.Remove(callback);
            }
        }

        public static void UnsubscribeAll<T>()
        {
            if (listeners.ContainsKey(typeof(T)))
            {
                listeners.Remove(typeof(T));
            }
    
            if (payloadListeners.ContainsKey(typeof(T)))
            {
                listeners.Remove(typeof(T));
            }
        }
        
        public static void UnsubscribeAll()
        {
            listeners.Clear();
            payloadListeners.Clear();
        }
    }
}