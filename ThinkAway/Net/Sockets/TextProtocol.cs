using System;

namespace ThinkAway.Net.Sockets
{
    public class TextProtocol : ICustomProtocol
    {
        private readonly string _text;

        public object Data
        {
            get { return _text; }
        }

        public TextProtocol(string text)
        {
            this._text = text;
        }

        private TextProtocol(byte[] data)
        {
            _text = System.Text.Encoding.UTF8.GetString(data);
        }

        public TextProtocol()
        {

        }

        public ICustomProtocol FromBytes(byte[] data)
        {
            return new TextProtocol(data);
        }

        public byte[] ToBytes()
        {
            return System.Text.Encoding.UTF8.GetBytes(_text);
        }
    }

}

