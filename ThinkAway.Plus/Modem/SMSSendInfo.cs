namespace ThinkAway.Plus.Modem
{
    /// <summary>
    /// SMS Send Info
    /// </summary>
    public class SMSSendInfo
    {
        public int Id { get; set; }

        public int State { get; set; }

        public string Com { get; set; }

        public long DateTime { get; set; }


        string _phone;
        public string Phone
        {
            get { return _phone; }
            set 
            {
                if (value.StartsWith("+86"))
                    value = value.Substring(3, value.Length - 3);
                _phone = value;
            }

        }

        public string Message { get; set; }

        public int Reply { get; set; }

        public override string ToString()
        {
            return string.Format("COM:{0};Phone:{1};Message:{2}",Com,Phone,Message);
        }
    }
}
