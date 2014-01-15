#if !NET20
using System;
using System.IO;

namespace ThinkAway.Core.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class StreamExtensions
    {


    /// <summary>
    /// 
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
        public static Byte[] ToByteArray(this Stream stream)
        {
            var mm = new MemoryStream();
            stream.CopyTo(mm);
            return mm.ToArray();
        }

        public static Stream CopyTo(this Stream stream,Stream to)
        {
            return stream;
        }
    

    }
}
#endif