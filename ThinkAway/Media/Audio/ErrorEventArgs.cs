using System;

namespace ThinkAway.Media.Player
{
    public class ErrorEventArgs : EventArgs
    {
        public ErrorEventArgs(long err)
        {
            this.ErrNum = err;
        }

        public readonly long ErrNum;
    }
}