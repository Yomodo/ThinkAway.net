using System;

namespace ThinkAway.Net
{
    /// <summary>
    /// This class holds UDP or TCP port range.
    /// </summary>
    public class PortRange
    {
        private readonly int _start = 1000;
        private readonly int _end   = 1100;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="start">Start port.</param>
        /// <param name="end">End port.</param>
        /// <exception cref="ArgumentOutOfRangeException">Is raised when any of the aruments value is out of range.</exception>
        public PortRange(int start,int end)
        {
            if(start < 1 || start > 0xFFFF){
                throw new ArgumentOutOfRangeException("start","Argument 'start' value must be > 0 and << 65 535.");
            }
            if(end < 1 || end > 0xFFFF){
                throw new ArgumentOutOfRangeException("end","Argument 'end' value must be > 0 and << 65 535.");
            }
            if(start > end){
                throw new ArgumentOutOfRangeException("start","Argumnet 'start' value must be >= argument 'end' value.");
            }

            _start = start;
            _end   = end;
        }


        #region Properties Implementation

        /// <summary>
        /// Gets start port.
        /// </summary>
        public int Start
        {
            get{ return _start; }
        }

        /// <summary>
        /// Gets end port.
        /// </summary>
        public int End
        {
            get{ return _end; }
        }

        #endregion

    }
}
