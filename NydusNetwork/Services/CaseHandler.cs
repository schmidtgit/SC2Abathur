using System;
using System.Collections.Generic;

namespace NydusNetwork.Services {
    public class CaseHandler<T1, T2> {
        private IDictionary<T1,ICollection<Action<T2>>> _events;
        public CaseHandler() => _events = new Dictionary<T1, ICollection<Action<T2>>>();

        public void Handle(T1 action, T2 type) {
            lock(_events)
                if(_events.TryGetValue(action,out var handlers))
                    foreach(var h in handlers)
                        h(type);
        }

        public void RegisterHandler(T1 action,Action<T2> handler) {
            lock(_events)
                if(_events.ContainsKey(action))
                    _events[action].Add(handler);
                else
                    _events.Add(action,new List<Action<T2>> {handler});
        }
        public void DeregisterHandler(Action<T2> handler) {
            lock(_events)
                foreach(var e in _events)
                    if(e.Value.Remove(handler))
                        return;
        }
    }
}
