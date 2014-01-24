using System;
using System.IO;
using System.Drawing;
using System.Text;
using ThinkAway.Core;

/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.IO
{
    public class FileHelper
    {

        public FileHelper(string fileName)
        {
            this.FileName = fileName;
        }

        public class FileEventArgs : EventArgs
        {

            public long Current { get; set; }

            public long FileSize { get; set; }

            public int Status { get; set; }

            public string FileName { get; set; }
        }

        public event EventHandler<FileEventArgs> ProgressChange;

        protected virtual void OnProgressChange(FileEventArgs e)
        {
            EventHandler<FileEventArgs> handler = ProgressChange;
            if (handler != null) handler(this, e);
        }


        /// <summary>
        /// 从文件扩展名得到文件关联图标
        /// </summary>
        /// <param name="smallIcon">是否是获取小图标，否则是大图标</param>
        /// <returns>图标</returns>
        public Icon GetIcon(bool smallIcon)
        {
            SHFILEINFO fi = new SHFILEINFO();
            Icon ic = null;
            //SHGFI_ICON + SHGFI_USEFILEATTRIBUTES + SmallIcon   
            int iTotal = Win32API.SHGetFileInfo(FileName, 100, ref fi, 0, (uint)(smallIcon ? 273 : 272));
            if (iTotal > 0)
            {
                ic = Icon.FromHandle(fi.HIcon);
            }
            return ic;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool Delete()
        {
            File.Delete(FileName);
            return !File.Exists(FileName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool MoveTo(string p)
        {
           File.Move(FileName,p);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="bs"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool Copy(string path,int bs = 1024)
        {
            if (!File.Exists(this.FileName)) return false;

            if (Directory.Exists(path)) path = Path.Combine(path, Path.GetFileName(FileName));

            FileStream formStream = new FileStream(FileName, FileMode.Open, FileAccess.Read);
            FileStream toStream = new FileStream(path, FileMode.Create, FileAccess.Write);

            byte[] buffer = new byte[bs];


            int length;
            long count = 0;
            FileEventArgs args = new FileEventArgs();
            args.Status = 1;//ing..
            args.FileName = path;
            args.FileSize = formStream.Length;
            while ((length = formStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                toStream.Write(buffer, 0, length);

                count += length;

                args.Current = count;

                OnProgressChange(args);
            }

            toStream.Flush();
            toStream.Close();
            toStream.Dispose();

            formStream.Close();
            formStream.Dispose();

            args.Status = 0;//comp
            OnProgressChange(args);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public string GetHash()
        {
            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public string GetHash(string p)
        {
            return p;
        }
        /// <summary>
        /// Read a line from the stream.
        /// A line is interpreted as all the bytes read until a CRLF or LF is encountered.<br/>
        /// CRLF pair or LF is not included in the string.
        /// </summary>
        /// <param name="stream">The stream from which the line is to be read</param>
        /// <returns>A line read from the stream returned as a byte array or <see langword="null"/> if no bytes were readable from the stream</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="stream"/> is <see langword="null"/></exception>
        public static byte[] ReadLineAsBytes(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (MemoryStream memoryStream = new MemoryStream())
            {
                while (true)
                {
                    int justRead = stream.ReadByte();
                    if (justRead == -1 && memoryStream.Length > 0)
                        break;

                    // Check if we started at the end of the stream we read from
                    // and we have not read anything from it yet
                    if (justRead == -1 && memoryStream.Length == 0)
                        return null;

                    char readChar = (char)justRead;

                    // Do not write \r or \n
                    if (readChar != '\r' && readChar != '\n')
                        memoryStream.WriteByte((byte)justRead);

                    // Last point in CRLF pair
                    if (readChar == '\n')
                        break;
                }

                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Read a line from the stream. <see cref="ReadLineAsBytes"/> for more documentation.
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        /// <returns>A line read from the stream or <see langword="null"/> if nothing could be read from the stream</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="stream"/> is <see langword="null"/></exception>
        public static string ReadLineAsAscii(Stream stream)
        {
            byte[] readFromStream = ReadLineAsBytes(stream);
            return readFromStream != null ? Encoding.ASCII.GetString(readFromStream) : null;
        }


        /// <summary>
        /// 检查通配符
        /// </summary>
        /// <param name="wildString"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static bool CompareWildcard(string wildString, string mask)
        {
            return CompareWildcard(wildString, mask, true);
        }

        /// <summary>
        /// Compares wildcard to string
        /// </summary>
        /// <param name="wildString">String to compare</param>
        /// <param name="mask">Wildcard mask (ex: *.jpg)</param>
        /// <param name="ignoreCase"></param>
        /// <returns>True if match found</returns>
        public static bool CompareWildcard(string wildString, string mask, bool ignoreCase)
        {
            int i = 0, k = 0;
            while (k != wildString.Length)
            {
                if(i >= mask.Length)
                   break; 
                
                switch (mask[i])
                {
                    case '*':
                        if ((i + 1) == mask.Length)
                            return true;
                        while (k != wildString.Length)
                        {
                            if (CompareWildcard(wildString.Substring(k + 1), mask.Substring(i + 1), ignoreCase))
                                return true;
                            k += 1;
                        }
                        return false;
                    case '?':
                        break;
                    default:
                        if (ignoreCase == false && wildString[k] != mask[i])
                            return false;
                        if (ignoreCase && Char.ToLower(wildString[k]) != Char.ToLower(mask[i]))
                            return false;
                        break;
                }
                i += 1;
                k += 1;
                
            }
            if (k == wildString.Length)
            {
                if (i == mask.Length || mask[i] == ';' || mask[i] == '*')
                    return true;
            }
            return false;
        }


        /// <summary>
        /// Compare multiple wildcards to string
        /// </summary>
        /// <param name="wildString">String to compare</param>
        /// <param name="mask">Wildcard masks seperated by a semicolon (;)</param>
        /// <returns>True if match found</returns>
        public static bool CompareWildcards(string wildString, string mask)
        {
            return CompareWildcards(wildString, mask, false);
        }

        /// <summary>
        /// Compare multiple wildcards to string
        /// </summary>
        /// <param name="wildString">String to compare</param>
        /// <param name="mask">Wildcard masks seperated by a semicolon (;)</param>
        /// <param name="ignoreCase"></param>
        /// <returns>True if match found</returns>
        public static bool CompareWildcards(string wildString, string mask, bool ignoreCase)
        {
            int i = 0;

            if (String.IsNullOrEmpty(mask))
                return false;
            if (mask == "*")
                return true;

            while (i != mask.Length)
            {
                if (CompareWildcard(wildString, mask.Substring(i), ignoreCase))
                    return true;

                while (i != mask.Length && mask[i] != ';')
                    i += 1;

                if (i != mask.Length && mask[i] == ';')
                {
                    i += 1;

                    while (i != mask.Length && mask[i] == ' ')
                        i += 1;
                }
            }

            return false;
        }

        public string FileName { get; set; }
    }
}
