using System;
using System.Collections.Generic;

namespace SocialSimulation
{
    public class Logger
    {
        private readonly List<Action<string>> _listeners;
        private readonly object _listenersLock = new object();

        public Logger()
        {
            _listeners = new List<Action<string>>();
        }

        public void Log(string message)
        {
            lock (_listenersLock)
            {
                foreach (var listener in _listeners)
                {
                    listener(message);
                }
            }
        }

        public void RegisterListener(Action<string> listener)
        {
            lock (_listenersLock)
            {
                _listeners.Add(listener);
            }
        }
    }
}