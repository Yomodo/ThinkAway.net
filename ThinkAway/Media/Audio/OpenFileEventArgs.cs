

using System;

namespace ThinkAway.Media.Player
{
    #region Event Argumenst for the events implemented by the wrapper class
    /// <summary>
    /// 
    /// </summary>
    public class OpenFileEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        public OpenFileEventArgs(string filename)
        {
            this.FileName = filename;
        }
        /// <summary>
        /// 
        /// </summary>
        public readonly string FileName;
    }

    #endregion
}
