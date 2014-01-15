using System;

namespace ThinkAway.Plus.Update
{
    public class UpdateErrorArgs : EventArgs
    {
        public UpdateErrorArgs(Exception exception)
        {
            this.ErrorCode = exception.GetHashCode();
            this.Message = exception.Message;
        }

        public UpdateErrorArgs(string message, int code)
        {
            this.ErrorCode = code;
            this.Message = message;
        }


        public string Message { get; set; }

        public int ErrorCode { get; set; }
    }
}