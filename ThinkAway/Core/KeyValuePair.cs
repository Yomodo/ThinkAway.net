namespace ThinkAway.Core
{
    public class KeyValue<TKey,TValue> 
    {
        public TKey Key { get; set; }

        public TValue Value { get; set; }
        
        public KeyValue()
        {
            
        }
        public KeyValue(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public string ToString(string formater)
        {
            return string.Format("{1}{0}{2}",formater, Key, Value);
        }
        public override string ToString()
        {
            return ToString(":");
        }
    }
}
