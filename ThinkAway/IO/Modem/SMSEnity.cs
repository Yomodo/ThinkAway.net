using System;

namespace ThinkAway.IO.Modem
{
    /// <summary>
    /// SMS Info
    /// </summary>
    [Serializable]
    public class SMSEnity
    {
        private readonly string _center;
        private readonly string _number;
        private readonly string _message;
        private readonly DateTime _dateTime;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="number"></param>
        /// <param name="dateTime"></param>
        /// <param name="message"></param>
        public SMSEnity (string center, string number, DateTime dateTime, string message)
        {
            _center = center;
            _number = number;
            _message = message;
            _dateTime = dateTime;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public SMSEnity(SMSEventArgs args)
        {
            //TODO:
            _center = args.SmsInfo.Center;
            _number = args.SmsInfo.Number;
            _dateTime = args.SmsInfo.TimeStamp;
            _message = args.SmsInfo.Message;
        }
        /// <summary>
        /// 
        /// </summary>
        public SMSEnity()
        {
            //TODO:
        }
        /// <summary>
        /// short message content.
        /// </summary>
        public string Message
        {
            get { return _message; }
        }
        /// <summary>
        /// message timestamp.
        /// </summary>
        public DateTime TimeStamp
        {
            get { return _dateTime; }
        }
        /// <summary>
        /// message phone number.
        /// </summary>
        public string Number
        {
            get { return _number; }
        }
        /// <summary>
        /// short message server number.
        /// </summary>
        public string Center
        {
            get { return _center; }
        }
        /// <summary>
        /// Convert SMSEnity to String type.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string result = string.Format("Center:{0};Number:{1};DateTime:{2};Message:{3};",
                _center, _number, _dateTime,  _message);
            return result;
        }
    }
}