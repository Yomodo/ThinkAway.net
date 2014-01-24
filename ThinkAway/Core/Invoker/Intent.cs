using System;
using System.Collections.Generic;

namespace ThinkAway.Core.Invoker
{
    /// <summary>
    /// Intent 提供了一种基于事件消息的可传输模型
    /// </summary>
    [Serializable]
    public class Intent : IEventMessage
    {
        private Dictionary<string, object> _dictionary;
        /// <summary>
        /// Intent
        /// </summary>
        /// <param name="eventMessage">eventMessage</param>
        public Intent(string eventMessage)
        {
            EventMessage = eventMessage;
            _dictionary = new Dictionary<string, object>();
        }
        /// <summary>
        /// Intent
        /// </summary>
        /// <param name="receiver">receiver</param>
        /// <param name="eventMessage">eventMessage</param>
        public Intent(string receiver, string eventMessage)
        {
            Receiver = receiver;
            EventMessage = eventMessage;
            _dictionary = new Dictionary<string, object>();
        }

        private string _receiver;

        /// <summary>
        /// Receiver
        /// </summary>
        public string Receiver
        {
            get { return _receiver; }
            set { _receiver = value; }
        }

        private string _eventMessage;

        /// <summary>
        /// EventMessage
        /// </summary>
        public string EventMessage
        {
            get { return _eventMessage; }
            set { _eventMessage = value; }
        }

        /// <summary>
        /// Data
        /// </summary>
        public Dictionary<string, object> Data 
        {
            get
            {
                return _dictionary;
            }
            set
            {
                _dictionary = value;
            }
        }
        /// <summary>
        /// SetValue
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="value">value</param>
        public void SetValue(string name, object value)
        {
            if (!_dictionary.ContainsKey(name))
            {
                _dictionary.Add(name, value);
            }
        }
        /// <summary>
        /// GetValue
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>value</returns>
        public object GetValue(string name)
        {
            object result = null;
            if(_dictionary.ContainsKey(name))
            {
                result = _dictionary[name];
            }
            return result;
        }
    }
}
