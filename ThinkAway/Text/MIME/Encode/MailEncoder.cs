using System;
using System.Security.Cryptography;
using System.Globalization;
using System.IO;
using System.Text;
using ThinkAway.Net.Mail.Exceptions;
/******************************************************************************
    Copyright 2001-2005 Ian Stallings
    OpenSmtp.Net is free software; you can redistribute it and/or modify
    it under the terms of the Lesser GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    OpenSmtp.Net is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    Lesser GNU General Public License for more details.

    You should have received a copy of the Lesser GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
/*******************************************************************************/
namespace ThinkAway.Text.MIME.Encode
{
    /// <summary>
    /// This Type is used to encode and decode files and strings using 
    /// MIME compatible encoding methods such as Base64 and quoted-printable
    /// </summary>
    internal class MailEncoder
    {

        private MailEncoder()
        { }
        /// <summary>Encodes a String using Base64 (see RFC 1521)</summary>
        /// <param name="s">string to be encoded</param>
        /// <example>
        /// <code>
        /// string base64EncodedText = MailEncoder.ConvertQP("À«≈√");
        /// </code>
        /// </example>
        /// <returns>Base64 encoded string</returns>
        internal static string ConvertToBase64(String s)
        {
            byte[] from = Encoding.ASCII.GetBytes(s.ToCharArray());
            string returnMsg = Core.ConvertEx.ToBase64(from);

            return returnMsg;
        }


        /// <summary> Encodes a FileStream using Base64 (see RFC 1521)</summary>
        /// <param name="inputFilePath">UNC path to file that needs to be encoded</param>
        /// <param name="outputFilePath">UNC path to file will store Base64 encoded ASCII text</param>
        /// <example>
        /// <code>
        /// MailEncoder.ConvertBase64("file.jpg", "file.txt");
        /// </code>
        /// </example>
        internal static void ConvertToBase64(string inputFilePath, string outputFilePath)
        {
            //Create the file streams to handle the input and output files.
            FileStream fin = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read);
            ConvertToBase64(fin, outputFilePath);
            fin.Close();
        }

        /// <summary> Encodes a FileStream using Base64 (see RFC 1521)</summary>
        /// <param name="inputStream">The stream that needs to be encoded</param>
        /// <param name="outputFilePath">UNC path to file will store Base64 encoded ASCII text</param>
        /// <example>
        /// <code>
        /// MailEncoder.ConvertBase64(Stream, "file.txt");
        /// </code>
        /// </example>
        internal static void ConvertToBase64(Stream inputStream, string outputFilePath)
        {
            //Create the file streams to handle the input and output files.
            FileStream fout = new FileStream(outputFilePath, FileMode.OpenOrCreate, FileAccess.Write);
            fout.SetLength(0);

            ToBase64Transform transformer = new ToBase64Transform();

            //Create variables to help with read and write below.
            //This is intermediate storage for the encryption:
            byte[] bin = new byte[inputStream.Length / transformer.InputBlockSize * transformer.OutputBlockSize];
            long rdlen = 0;              //This is the total number of bytes written.
            long totlen = inputStream.Length;    //This is the total length of the input file.

            CryptoStream encStream = new CryptoStream(fout, transformer, CryptoStreamMode.Write);

            //Read from the input file, then encrypt and write to the output file.
            while (rdlen < totlen)
            {
                int len = inputStream.Read(bin, 0, (int)inputStream.Length);                     //This is the number of bytes to be written at a time.
                encStream.Write(bin, 0, len);
                //inputBlock size(3)
                rdlen = (rdlen + ((len / transformer.InputBlockSize) * transformer.OutputBlockSize));
            }

            encStream.Close();
            fout.Close();
        }

        internal static string ConvertFromBase64(string s)
        {
            byte[] ret = Core.ConvertEx.FromBase64(s);
            return Encoding.ASCII.GetString(ret, 0, ret.Length);
        }

        internal static void ConvertFromBase64(string inputFilePath, string outputFilePath)
        {
            //Create the file streams to handle the input and output files.
            FileStream fin = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read);
            FileStream fout = new FileStream(outputFilePath, FileMode.OpenOrCreate, FileAccess.Write);
            fout.SetLength(0);

            FromBase64Transform transformer = new FromBase64Transform();

            //Create variables to help with read and write below.
            //This is intermediate storage for the decryption:
            byte[] bin = new byte[fin.Length / transformer.InputBlockSize * transformer.OutputBlockSize];
            long rdlen = 0;              //This is the total number of bytes written.
            long totlen = fin.Length;    //This is the total length of the input file.

            CryptoStream encStream = new CryptoStream(fout, transformer, CryptoStreamMode.Write);

            //Read from the input file, then decrypt and write to the output file.
            while (rdlen < totlen)
            {
                int len = fin.Read(bin, 0, (int)fin.Length);                     //This is the number of bytes to be written at a time.
                encStream.Write(bin, 0, len);
                rdlen = (rdlen + ((len / transformer.InputBlockSize) * transformer.OutputBlockSize));
            }

            encStream.Close();
            fout.Close();
            fin.Close();
        }

        /// <summary> Encodes a string using Quoted-Printable encoding (see RFC 1521)</summary>
        /// <param name="str">string that needs to be encoded</param>
        /// <example>
        /// <code>
        /// string qpEncodedText = MailEncoder.ConvertQP("À«≈√");
        /// </code>
        /// </example>
        /// <returns>Quoted-Printable encoded string</returns>
        internal static string ConvertToQP(string str)
        {
            return ConvertToQP(str, "ISO-8859-1");
        }

        /// <summary> Encodes a string using Quoted-Printable encoding (see RFC 1521)</summary>
        /// <param name="str">string that needs to be encoded</param>
        /// <param name="charset">charset of string that needs to be encoded</param>
        /// <example>
        /// <code>
        /// string qpEncodedText = MailEncoder.ConvertQP("À«≈√");
        /// </code>
        /// </example>
        /// <returns>Quoted-Printable encoded string</returns>
        internal static string ConvertToQP(string str, string charset)
        {
            // TURNER.BSD MODIFIED 9/10/2004 GKW to fix NUMEROUS bugs
            // Conforms to rules described in http://www.fourmilab.ch/webtools/qprint/rfc1521.html

            if(str == null)
                throw new ArgumentNullException("str");
            if(charset == null)
                throw new ArgumentNullException("charset");

            // environment newline char
            char[] nl = Environment.NewLine.ToCharArray();

            // source char array
            char[] source = str.ToCharArray();

            StringBuilder sb = new StringBuilder();
            StringBuilder currLine = new StringBuilder();
            bool bNewline = false;

            Encoding oEncoding = Encoding.GetEncoding(charset);

            for (int sidx = 0; sidx < str.Length; sidx++)
            {
                char ch = source[sidx];

                // RULE # 4: LINE BREAKS
                if (ch == nl[0] && sidx <= (str.Length - nl.Length))
                {
                    // peek ahead make sure the "whole" newline is present
                    if (str.Substring(sidx, nl.Length) == Environment.NewLine)
                    {
                        // RULE #3: ENCODE WHITESPACE PRECEEDING A HARD BREAK
                        if (currLine.Length > 0)
                        {
                            if (source[sidx - 1] == ' ')
                            {   // if char is preceded by space char add =20
                                currLine.Remove(currLine.Length - 1, 1);
                                currLine.Append("=20");
                            }
                            else if (source[sidx - 1] == '\t')
                            {   // if char is preceded by tab char add =09
                                currLine.Remove(currLine.Length - 1, 1);
                                currLine.Append("=09");
                            }
                        }

                        // flag for new line
                        bNewline = true;
                        sidx += nl.Length - 1;  // jump ahead 

                    }
                    else
                    {	// not actually a newline.  Encode per Rule #1
                        currLine.AppendFormat("=0{0}", System.Convert.ToString((byte)ch, 16).ToUpper());
                    }
                }
                // RULE #1 and #2
                // Optional characters are: !"#$@[\]^`{|}~
                else if (ch > 126 || (ch < 32 && ch != '\t') || ch == '=')
                {
                    byte[] outByte = new byte[10];
                    int iCount = oEncoding.GetBytes("" + ch, 0, 1, outByte, 0);

                    for (int i = 0; i < iCount; i++)
                    {
                        currLine.AppendFormat(outByte[i] < 16 ? "=0{0}" : "={0}", System.Convert.ToString(outByte[i], 16).ToUpper());
                    }
                }
                else
                {
                    currLine.Append(ch);
                }

                // Rule #5: MAXIMUM length 76 characters per line
                if (currLine.Length >= 76)
                {
                    // just make sure not to split an encoded char
                    string cLine = currLine.ToString();
                    int breakAt = cLine.LastIndexOf("=", System.StringComparison.Ordinal);
                    if (breakAt < 70) breakAt = 74;
                    sb.AppendFormat("{0}=\r\n", cLine.Substring(0, breakAt));
                    currLine = new StringBuilder(cLine.Substring(breakAt));
                }

                if (bNewline)
                {
                    // RFC 822 linebreak = CRLF
                    sb.AppendFormat("{0}\r\n", currLine);
                    currLine = new StringBuilder();
                    bNewline = false;
                }
            }

            // add last line
            sb.Append(currLine.ToString());

            return sb.ToString();
        }

        /// <summary>
        /// Convert to Quoted Printable if necessary
        /// </summary>
        /// <param name="s"></param>
        /// <param name="charset"></param>
        /// <param name="forceconversion">force a conversion</param>
        /// <returns></returns>
        internal static string ConvertHeaderToQP(string s, string charset, bool forceconversion)
        {
            if (!forceconversion)
            {
                foreach (char t in s)
                {
                    if (t <= 126 && t >= 32)
                    {
                        return s;
                    }
                }
            }
            return string.Format("=?{0}?Q?{1}?=", charset, ConvertToQP(s, charset));
        }

        /// <summary>
        /// Convert to Quoted printable if necessary.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        internal static string ConvertHeaderToQP(string s, string charset)
        {
            return ConvertHeaderToQP(s, charset, false);
        }

        internal static string ConvertFromQP(string s)
        {
            if (s == null) return null;

            // source char array:
            char[] source = s.ToCharArray();
            // result char array:
            char[] result = new char[(int)(s.Length * 1.1)];
            // environment newline char:
            char[] nl = Environment.NewLine.ToCharArray();

            int didx = 0,
                slen = s.Length;

            for (int sidx = 0; sidx < slen; )
            {
                char ch = source[sidx++];

                if (ch == '=')
                {

                    // Premature EOF
                    if (sidx >= slen - 1)
                    { throw new ParseException("Premature EOF"); }

                    // RULE # 5
                    if (source[sidx] == '\n' || source[sidx] == '\r')
                    {
                        sidx++;
                        if (source[sidx - 1] == '\r' && source[sidx] == '\n')
                        {
                            sidx++;
                        }
                    }
                    // RULE # 1
                    else
                    {
                        int hi = Int32.Parse(System.Convert.ToString(source[sidx]), NumberStyles.HexNumber);
                        int lo = Int32.Parse(System.Convert.ToString(source[sidx + 1]), NumberStyles.HexNumber);

                        if ((hi | lo) < 0)
                        {
                            throw new ParseException(new String(source, sidx - 1, 3) + " is an invalid code");
                        }
                        char repl = (char)(hi << 4 | lo);
                        sidx += 2;

                        result[didx++] = repl;
                    }

                    int last = didx;

                    // RULE # 4
                    if (ch == '\n' || ch == '\r')
                    {
                        if (ch == '\r' && sidx < slen && source[sidx] == '\n')
                        {
                            sidx++;

                            foreach (char t in nl)
                            {
                                result[last++] = t;
                                didx = last;
                            }
                        }
                        else
                        {
                            result[didx++] = ch;
                            // RULE # 3
                            if (ch != ' ' && ch != '\t')
                            {
                                last = didx;
                            }
                        }

                        if (didx > result.Length - nl.Length - 2)
                        {
                            char[] newCharArray = new char[result.Length + 500];
                            Array.Copy(result, newCharArray, result.Length);
                            result = newCharArray;
                        }
                    }

                }
            }

            return new string(result, 0, didx);
        }

        internal static bool IsAscii(string s)
        {
            // source char array
            char[] source = s.ToCharArray();
            bool all = true;
            foreach (char ch in source)
            {
                if (ch > 127)
                {
                    all = false;
                    break;
                }
            }
            //return false;
            return all;
        }

    }
}

