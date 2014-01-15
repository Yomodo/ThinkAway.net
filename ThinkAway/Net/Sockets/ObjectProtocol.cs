using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace ThinkAway.Net.Sockets
{
    [Serializable]
    public class ObjectProtocol : ICustomProtocol
    {
        public ObjectProtocol()
        {
            //_protocol = new ObjectProtocol();
        }

        #region Implementation of ICustomProtocol

        public object Data
        {
            get { return this; }
        }

        public ICustomProtocol FromBytes(byte[] data)
        {
            MemoryStream memoryStream = new MemoryStream(data);
            IFormatter formatter = new BinaryFormatter();
            object deserialize = formatter.Deserialize(memoryStream);
            return (ICustomProtocol)deserialize;
        }

        public byte[] ToBytes()
        {
            MemoryStream memoryStream = new MemoryStream();
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(memoryStream, this);
            byte[] array = memoryStream.ToArray();
            return array;
        }

        #endregion
    }

}
