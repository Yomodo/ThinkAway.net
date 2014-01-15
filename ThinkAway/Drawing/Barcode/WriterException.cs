using System;

namespace ThinkAway.Drawing.Barcode
{
    /// <summary> A base class which covers the range of exceptions which may occur when encoding a barcode using
    /// the Writer framework.
    /// 
    /// </summary>
    /// <author>  dswitkin@google.com (Daniel Switkin)
    /// </author>
    /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source 
    /// </author>
    [Serializable]
    public sealed class WriterException : System.Exception
    {

        public WriterException()
            : base()
        {
        }

        public WriterException(System.String message)
            : base(message)
        {
        }
    }
}