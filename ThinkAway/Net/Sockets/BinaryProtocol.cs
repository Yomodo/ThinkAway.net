namespace ThinkAway.Net.Sockets
{
    public class BinaryProtocol : ICustomProtocol
    {
        #region Implementation of ICustomProtocol

        private readonly byte[] _data;

        private BinaryProtocol(byte[] bytes)
        {
            this._data = bytes;
        }

        public BinaryProtocol()
        {
        }


        public object Data
        {
            get { return _data; }
        }

        public ICustomProtocol FromBytes(byte[] data)
        {
            return new BinaryProtocol(data);
        }

        public byte[] ToBytes()
        {
            return _data;
        }

        #endregion
    }
}