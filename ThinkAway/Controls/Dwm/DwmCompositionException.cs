using System;
using System.Runtime.Serialization;

namespace ThinkAway.Controls.Dwm
{
    [Serializable]
    internal class DwmCompositionException : Exception
    {
        public DwmCompositionException(string m) : base(m)
        {
        }

        public DwmCompositionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public DwmCompositionException(string m, Exception innerException) : base(m, innerException)
        {
        }
    }
}

