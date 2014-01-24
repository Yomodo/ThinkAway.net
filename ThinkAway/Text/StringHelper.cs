/*
 * 由SharpDevelop创建。
 * 用户： Lsong
 * 日期: 2012-2-1
 * 时间: 10:13
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ThinkAway.Text
{
	/// <summary>
	/// Description of StringHelper.
	/// </summary>
	public class StringHelper
	{
	    /// <summary>
		/// IsNullOrWhiteSpace
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool IsNullOrWhiteSpace(string str)
		{
			if(IsNullOrEmpty(str)){
				return true;
			}
			if(string.Copy(str).Trim() == String.Empty)
			{
				return true;
			}
			return false;
		}
		/// <summary>
		/// IsNullOrEmpty
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool IsNullOrEmpty(string str)
		{			
			if(str == null)
			{
				return true;
			}
			if(str == String.Empty)
			{
				return true;
			}
			if(Equals(str,String.Empty))
			{
				return true;
			}
			return false;
		}

        #region static method QuoteString

        /// <summary>
        /// Qoutes string and escapes fishy('\',"') chars.
        /// </summary>
        /// <param name="text">Text to quote.</param>
        /// <returns></returns>
        public static string QuoteString(string text)
        {
            if(text == null)
                throw new ArgumentNullException("text");
            // String is already quoted-string.
            if (text.StartsWith("\"") && text.EndsWith("\""))
            {
                return text;
            }

            StringBuilder retVal = new StringBuilder();

            foreach (char c in text)
            {
                switch (c)
                {
                    case '\\':
                        retVal.Append("\\\\");
                        break;
                    case '\"':
                        retVal.Append("\\\"");
                        break;
                    default:
                        retVal.Append(c);
                        break;
                }
            }

            return string.Format("\"{0}\"", retVal);
        }

        #endregion

        #region static method UnQuoteString

        /// <summary>
        /// Unquotes and unescapes escaped chars specified text. For example "xxx" will become to 'xxx', "escaped quote \"", will become to escaped 'quote "'.
        /// </summary>
        /// <param name="text">Text to unquote.</param>
        /// <returns></returns>
        public static string UnQuoteString(string text)
        {
            int startPosInText = 0;
            int endPosInText = text.Length;

            //--- Trim. We can't use standard string.Trim(), it's slow. ----//
            for (int i = 0; i < endPosInText; i++)
            {
                char c = text[i];
                if (c == ' ' || c == '\t')
                {
                    startPosInText++;
                }
                else
                {
                    break;
                }
            }
            for (int i = endPosInText - 1; i > 0; i--)
            {
                char c = text[i];
                if (c == ' ' || c == '\t')
                {
                    endPosInText--;
                }
                else
                {
                    break;
                }
            }
            //--------------------------------------------------------------//

            // All text trimmed
            if ((endPosInText - startPosInText) <= 0)
            {
                return "";
            }

            // Remove starting and ending quotes.         
            if (text[startPosInText] == '\"')
            {
                startPosInText++;
            }
            if (text[endPosInText - 1] == '\"')
            {
                endPosInText--;
            }

            // Just '"'
            if (endPosInText == startPosInText - 1)
            {
                return "";
            }

            char[] chars = new char[endPosInText - startPosInText];

            int posInChars = 0;
            bool charIsEscaped = false;
            for (int i = startPosInText; i < endPosInText; i++)
            {
                char c = text[i];

                // Escaping char
                if (!charIsEscaped && c == '\\')
                {
                    charIsEscaped = true;
                }
                // Escaped char
                else if (charIsEscaped)
                {
                    // TODO: replace \n,\r,\t,\v ???
                    chars[posInChars] = c;
                    posInChars++;
                    charIsEscaped = false;
                }
                // Normal char
                else
                {
                    chars[posInChars] = c;
                    posInChars++;
                }
            }

            return new string(chars, 0, posInChars);
        }

        #endregion

        #region static method EscapeString

        /// <summary>
        /// Escapes specified chars in the specified string.
        /// </summary>
        /// <param name="text">Text to escape.</param>
        /// <param name="charsToEscape">Chars to escape.</param>
        public static string EscapeString(string text, char[] charsToEscape)
        {
            // Create worst scenario buffer, assume all chars must be escaped
            char[] buffer = new char[text.Length * 2];
            int nChars = 0;
            foreach (char c in text)
            {
                foreach (char escapeChar in charsToEscape)
                {
                    if (c == escapeChar)
                    {
                        buffer[nChars] = '\\';
                        nChars++;
                        break;
                    }
                }

                buffer[nChars] = c;
                nChars++;
            }

            return new string(buffer, 0, nChars);
        }

        #endregion

        #region static method UnEscapeString

        /// <summary>
        /// Unescapes all escaped chars.
        /// </summary>
        /// <param name="text">Text to unescape.</param>
        /// <returns></returns>
        public static string UnEscapeString(string text)
        {
            // Create worst scenarion buffer, non of the chars escaped.
            char[] buffer = new char[text.Length];
            int nChars = 0;
            bool escapedCahr = false;
            foreach (char c in text)
            {
                if (!escapedCahr && c == '\\')
                {
                    escapedCahr = true;
                }
                else
                {
                    buffer[nChars] = c;
                    nChars++;
                    escapedCahr = false;
                }
            }

            return new string(buffer, 0, nChars);
        }

        #endregion


        #region static method SplitQuotedString

        /// <summary>
        /// Splits string into string arrays. This split method won't split qouted strings, but only text outside of qouted string.
        /// For example: '"text1, text2",text3' will be 2 parts: "text1, text2" and text3.
        /// </summary>
        /// <param name="text">Text to split.</param>
        /// <param name="splitChar">Char that splits text.</param>
        /// <returns></returns>
        public static string[] SplitQuotedString(string text, char splitChar)
        {
            return SplitQuotedString(text, splitChar, false);
        }

        /// <summary>
        /// Splits string into string arrays. This split method won't split qouted strings, but only text outside of qouted string.
        /// For example: '"text1, text2",text3' will be 2 parts: "text1, text2" and text3.
        /// </summary>
        /// <param name="text">Text to split.</param>
        /// <param name="splitChar">Char that splits text.</param>
        /// <param name="unquote">If true, splitted parst will be unqouted if they are qouted.</param>
        /// <returns></returns>
        public static string[] SplitQuotedString(string text, char splitChar, bool unquote)
        {
            return SplitQuotedString(text, splitChar, unquote, int.MaxValue);
        }

        /// <summary>
        /// Splits string into string arrays. This split method won't split qouted strings, but only text outside of qouted string.
        /// For example: '"text1, text2",text3' will be 2 parts: "text1, text2" and text3.
        /// </summary>
        /// <param name="text">Text to split.</param>
        /// <param name="splitChar">Char that splits text.</param>
        /// <param name="unquote">If true, splitted parst will be unqouted if they are qouted.</param>
        /// <param name="count">Maximum number of substrings to return.</param>
        /// <returns>Returns splitted string.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>text</b> is null reference.</exception>
        public static string[] SplitQuotedString(string text, char splitChar, bool unquote, int count)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            List<string> splitParts = new List<string>();  // Holds splitted parts
            int startPos = 0;
            bool inQuotedString = false;               // Holds flag if position is quoted string or not
            char lastChar = '0';

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                // We have exceeded maximum allowed splitted parts.
                if ((splitParts.Count + 1) >= count)
                {
                    break;
                }

                // We have quoted string start/end.
                if (lastChar != '\\' && c == '\"')
                {
                    inQuotedString = !inQuotedString;
                }
                // We have escaped or normal char.
                //else{

                // We igonre split char in quoted-string.
                if (!inQuotedString)
                {
                    // We have split char, do split.
                    if (c == splitChar)
                    {
                        splitParts.Add(unquote
                                           ? UnQuoteString(text.Substring(startPos, i - startPos))
                                           : text.Substring(startPos, i - startPos));

                        // Store new split part start position.
                        startPos = i + 1;
                    }
                }
                //else{

                lastChar = c;
            }

            // Add last split part to splitted parts list
            splitParts.Add(unquote
                               ? UnQuoteString(text.Substring(startPos, text.Length - startPos))
                               : text.Substring(startPos, text.Length - startPos));

            return splitParts.ToArray();
        }

        #endregion


        #region method QuotedIndexOf

        /// <summary>
        /// Gets first index of specified char. The specified char in quoted string is skipped.
        /// Returns -1 if specified char doesn't exist.
        /// </summary>
        /// <param name="text">Text in what to check.</param>
        /// <param name="indexChar">Char what index to get.</param>
        /// <returns></returns>
        public static int QuotedIndexOf(string text, char indexChar)
        {
            int retVal = -1;
            bool inQuotedString = false; // Holds flag if position is quoted string or not			
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                if (c == '\"')
                {
                    // Start/end quoted string area
                    inQuotedString = !inQuotedString;
                }

                // Current char is what index we want and it isn't in quoted string, return it's index
                if (!inQuotedString && c == indexChar)
                {
                    retVal =  i;
                    break;
                }
            }

            return retVal;
        }

        #endregion


        #region static method SplitString

        /// <summary>
        /// Splits string into string arrays.
        /// </summary>
        /// <param name="text">Text to split.</param>
        /// <param name="splitChar">Char Char that splits text.</param>
        /// <returns></returns>
        public static string[] Split(string text, char splitChar)
        {
            ArrayList splitParts = new ArrayList();  // Holds splitted parts

            int lastSplitPoint = 0;
            int textLength = text.Length;
            for (int i = 0; i < textLength; i++)
            {
                if (text[i] == splitChar)
                {
                    // Add current currentSplitBuffer value to splitted parts list
                    splitParts.Add(text.Substring(lastSplitPoint, i - lastSplitPoint));

                    lastSplitPoint = i + 1;
                }
            }
            // Add last split part to splitted parts list
            if (lastSplitPoint <= textLength)
            {
                splitParts.Add(text.Substring(lastSplitPoint));
            }

            string[] retVal = new string[splitParts.Count];
            splitParts.CopyTo(retVal, 0);

            return retVal;
        }

        #endregion


        #region static method IsToken

        /// <summary>
        /// Gets if specified string is valid "token" value.
        /// </summary>
        /// <param name="value">String value to check.</param>
        /// <returns>Returns true if specified string value is valid "token" value.</returns>
        /// <exception cref="ArgumentNullException">Is raised if <b>value</b> is null.</exception>
        public static bool IsToken(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            /* This syntax is taken from rfc 3261, but token must be universal so ... .
                token    =  1*(alphanum / "-" / "." / "!" / "%" / "*" / "_" / "+" / "`" / "'" / "~" )
                alphanum = ALPHA / DIGIT
                ALPHA    =  %x41-5A / %x61-7A   ; A-Z / a-z
                DIGIT    =  %x30-39             ; 0-9
            */

            char[] tokenChars = new char[] { '-', '.', '!', '%', '*', '_', '+', '`', '\'', '~' };
            foreach (char c in value)
            {
                // We don't have letter or digit, so we only may have token char.
                if (!((c >= 0x41 && c <= 0x5A) || (c >= 0x61 && c <= 0x7A) || (c >= 0x30 && c <= 0x39)))
                {
                    bool validTokenChar = false;
                    foreach (char tokenChar in tokenChars)
                    {
                        if (c == tokenChar)
                        {
                            validTokenChar = true;
                            break;
                        }
                    }
                    if (!validTokenChar)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion

        /// <summary>
        /// ToHex
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ToHex(byte[] data)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in data)
            {
                stringBuilder.AppendFormat("{0:X2}", b);
            }
            return stringBuilder.ToString();
        }
        /// <summary>
        /// FormHex
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] FormHex(string str)
        {
            byte[] data = new byte[str.Length / 2];
            for (int i = 0; i < str.Length / 2; i++)
            {
                string substring = str.Substring(i*2, 2);
                int x = (Convert.ToInt32(substring, 16));
                data[i] = (byte)x;
            }
            return data;
        }
        //==============================================================

        /// <summary>
        /// Separate header name and header value.
        /// </summary>
        /// <exception cref="ArgumentNullException">If <paramref name="rawHeader"/> is <see langword="null"/></exception>
        public static string[] GetHeadersValue(string rawHeader)
        {
            if (rawHeader == null)
                throw new ArgumentNullException("rawHeader");

            string[] array = new string[] { string.Empty, string.Empty };
            int indexOfColon = rawHeader.IndexOf(':');

            // Check if it is allowed to make substring calls
            if (indexOfColon >= 0 && rawHeader.Length >= indexOfColon + 1)
            {
                array[0] = rawHeader.Substring(0, indexOfColon).Trim();
                array[1] = rawHeader.Substring(indexOfColon + 1).Trim();
            }

            return array;
        }

        /// <summary>
        /// Remove quotes, if found, around the string.
        /// </summary>
        /// <param name="text">Text with quotes or without quotes</param>
        /// <returns>Text without quotes</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> is <see langword="null"/></exception>
        public static string RemoveQuotesIfAny(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            string returner = text;

            if (returner.StartsWith("\"", StringComparison.OrdinalIgnoreCase))
                returner = returner.Substring(1);
            if (returner.EndsWith("\"", StringComparison.OrdinalIgnoreCase))
                returner = returner.Substring(0, returner.Length - 1);

            return returner;
        }
	}
}
