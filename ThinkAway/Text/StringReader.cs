﻿using System;
using System.Text;

namespace ThinkAway.Text
{
    /// <summary>
    /// PDU 编码读取操作类
    /// </summary>
    public class StringReader
    {
        private string _pduString;

        /// <summary>
        /// 
        /// </summary>
        public string PduString
        {
            private set { _pduString = value; }
            get { return _pduString; }
        }

        private int _offset;

        /// <summary>
        /// 
        /// </summary>
        public int Offset
        {
            set { _offset = value; }
            get { return _offset; }
        }

        /// <summary>
        /// PDU 读取器
        /// </summary>
        /// <param name="source">PDU 编码字符串</param>
        /// <param name="offset">偏移量</param>
        public StringReader(string source, int offset)
        {
            PduString = source;
            Offset = offset;
        }
        /// <summary>
        /// 可否偏移指针
        /// </summary>
        /// <returns></returns>
        public bool HasNext()
        {
            return Offset < PduString.Length;
        }
        /// <summary>
        /// 下一个 Unicode 字符
        /// </summary>
        /// <returns>Unicode 字符</returns>
        public char NextChar()
        {
            return PduString[Offset++];
        }
        /// <summary>
        /// 下一组 Unicode 字符
        /// </summary>
        /// <param name="length">读取长度</param>
        /// <returns>Unicode 字符串</returns>
        public string NextString(int length)
        {
            StringBuilder retValue = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                if (!HasNext())
                {
                    break;
                }
                retValue.Append(NextChar());
            }
            return retValue.ToString();
        }



        //===============================================


        private readonly string _originalString = "";
        private string _sourceString = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="source">Source string.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>source</b> is null.</exception>
        public StringReader(string source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            _originalString = source;
            _sourceString = source;
        }


        #region method AppenString

        /// <summary>
        /// Appends specified string to SourceString.
        /// </summary>
        /// <param name="str">String value to append.</param>
        public void AppenString(string str)
        {
            _sourceString += str;
        }

        #endregion


        #region method ReadToFirstChar

        /// <summary>
        /// Reads to first char, skips white-space(SP,VTAB,HTAB,CR,LF) from the beginning of source string.
        /// </summary>
        /// <returns>Returns white-space chars which was readed.</returns>
        public string ReadToFirstChar()
        {
            int whiteSpaces = 0;
            foreach (char t in _sourceString)
            {
                if (char.IsWhiteSpace(t))
                {
                    whiteSpaces++;
                }
                else
                {
                    break;
                }
            }

            string whiteSpaceChars = _sourceString.Substring(0, whiteSpaces);
            _sourceString = _sourceString.Substring(whiteSpaces);

            return whiteSpaceChars;
        }

        #endregion

        //	public string ReadToDelimiter(char delimiter)
        //	{
        //	}

        #region method ReadSpecifiedLength

        /// <summary>
        /// Reads string with specified length. Throws exception if read length is bigger than source string length.
        /// </summary>
        /// <param name="length">Number of chars to read.</param>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public string ReadSpecifiedLength(int length)
        {
            if (_sourceString.Length >= length)
            {
                string retVal = _sourceString.Substring(0, length);
                _sourceString = _sourceString.Substring(length);

                return retVal;
            }
            throw new Exception("Read length can't be bigger than source string !");
        }

        #endregion

        #region method QuotedReadToDelimiter

        /// <summary>
        /// Reads string to specified delimiter or to end of underlying string. Notes: Delimiter in quoted string is skipped.
        /// Delimiter is removed by default.
        /// For example: delimiter = ',', text = '"aaaa,eee",qqqq' - then result is '"aaaa,eee"'.
        /// </summary>
        /// <param name="delimiter">Data delimiter.</param>
        /// <returns></returns>
        public string QuotedReadToDelimiter(char delimiter)
        {
            return QuotedReadToDelimiter(new char[] { delimiter });
        }

        /// <summary>
        /// Reads string to specified delimiter or to end of underlying string. Notes: Delimiters in quoted string is skipped.
        /// Delimiter is removed by default.
        /// For example: delimiter = ',', text = '"aaaa,eee",qqqq' - then result is '"aaaa,eee"'.
        /// </summary>
        /// <param name="delimiters">Data delimiters.</param>
        /// <returns></returns>
        public string QuotedReadToDelimiter(char[] delimiters)
        {
            return QuotedReadToDelimiter(delimiters, true);
        }

        /// <summary>
        /// Reads string to specified delimiter or to end of underlying string. Notes: Delimiters in quoted string is skipped. 
        /// For example: delimiter = ',', text = '"aaaa,eee",qqqq' - then result is '"aaaa,eee"'.
        /// </summary>
        /// <param name="delimiters">Data delimiters.</param>
        /// <param name="removeDelimiter">Specifies if delimiter is removed from underlying string.</param>
        /// <returns></returns>
        public string QuotedReadToDelimiter(char[] delimiters, bool removeDelimiter)
        {
            StringBuilder currentSplitBuffer = new StringBuilder(); // Holds active
            bool inQuotedString = false;               // Holds flag if position is quoted string or not
            bool doEscape = false;

            for (int i = 0; i < _sourceString.Length; i++)
            {
                char c = _sourceString[i];

                if (doEscape)
                {
                    currentSplitBuffer.Append(c);
                    doEscape = false;
                }
                else if (c == '\\')
                {
                    currentSplitBuffer.Append(c);
                    doEscape = true;
                }
                else
                {
                    // Start/end quoted string area
                    if (c == '\"')
                    {
                        inQuotedString = !inQuotedString;
                    }

                    // See if char is delimiter
                    bool isDelimiter = false;
                    foreach (char delimiter in delimiters)
                    {
                        if (c == delimiter)
                        {
                            isDelimiter = true;
                            break;
                        }
                    }

                    // Current char is split char and it isn't in quoted string, do split
                    if (!inQuotedString && isDelimiter)
                    {
                        string retVal = currentSplitBuffer.ToString();

                        // Remove readed string + delimiter from source string
                        if (removeDelimiter)
                        {
                            _sourceString = _sourceString.Substring(i + 1);
                        }
                        // Remove readed string
                        else
                        {
                            _sourceString = _sourceString.Substring(i);
                        }

                        return retVal;
                    }
                    currentSplitBuffer.Append(c);
                }
            }

            // If we reached so far then we are end of string, return it
            _sourceString = "";
            return currentSplitBuffer.ToString();
        }

        #endregion


        #region method ReadWord

        /// <summary>
        /// Reads word from string. Returns null if no word is available.
        /// Word reading begins from first char, for example if SP"text", then space is trimmed.
        /// </summary>
        /// <returns></returns>
        public string ReadWord()
        {
            return ReadWord(true);
        }

        /// <summary>
        /// Reads word from string. Returns null if no word is available.
        /// Word reading begins from first char, for example if SP"text", then space is trimmed.
        /// </summary>
        /// <param name="unQuote">Specifies if quoted string word is unquoted.</param>
        /// <returns></returns>
        public string ReadWord(bool unQuote)
        {
            return ReadWord(unQuote, new char[] { ' ', ',', ';', '{', '}', '(', ')', '[', ']', '<', '>', '\r', '\n' }, false);
        }

        /// <summary>
        /// Reads word from string. Returns null if no word is available.
        /// Word reading begins from first char, for example if SP"text", then space is trimmed.
        /// </summary>
        /// <param name="unQuote">Specifies if quoted string word is unquoted.</param>
        /// <param name="wordTerminatorChars">Specifies chars what terminate word.</param>
        /// <param name="removeWordTerminator">Specifies if work terminator is removed.</param>
        /// <returns></returns>
        public string ReadWord(bool unQuote, char[] wordTerminatorChars, bool removeWordTerminator)
        {
            // Always start word reading from first char.
            this.ReadToFirstChar();

            if (this.Available == 0)
            {
                return null;
            }

            // quoted word can contain any char, " must be escaped with \
            // unqouted word can conatin any char except: SP VTAB HTAB,{}()[]<>

            if (_sourceString.StartsWith("\""))
            {
                if (unQuote)
                {
                    return StringHelper.UnQuoteString(QuotedReadToDelimiter(wordTerminatorChars, removeWordTerminator));
                }
                return QuotedReadToDelimiter(wordTerminatorChars, removeWordTerminator);
            }
            int wordLength = 0;
            foreach (char c in _sourceString)
            {
                bool isTerminator = false;
                foreach (char terminator in wordTerminatorChars)
                {
                    if (c == terminator)
                    {
                        isTerminator = true;
                        break;
                    }
                }
                if (isTerminator)
                {
                    break;
                }

                wordLength++;
            }

            string retVal = _sourceString.Substring(0, wordLength);
            if (removeWordTerminator)
            {
                if (_sourceString.Length >= wordLength + 1)
                {
                    _sourceString = _sourceString.Substring(wordLength + 1);
                }
            }
            else
            {
                _sourceString = _sourceString.Substring(wordLength);
            }

            return retVal;
        }

        #endregion

        #region method ReadParenthesized

        /// <summary>
        /// Reads parenthesized value. Supports {},(),[],&lt;&gt; parenthesis. 
        /// Throws exception if there isn't parenthesized value or closing parenthesize is missing.
        /// </summary>
        /// <returns></returns>
        public string ReadParenthesized()
        {
            ReadToFirstChar();

            char startingChar = ' ';
            char closingChar = ' ';

            if (_sourceString.StartsWith("{"))
            {
                startingChar = '{';
                closingChar = '}';
            }
            else if (_sourceString.StartsWith("("))
            {
                startingChar = '(';
                closingChar = ')';
            }
            else if (_sourceString.StartsWith("["))
            {
                startingChar = '[';
                closingChar = ']';
            }
            else if (_sourceString.StartsWith("<"))
            {
                startingChar = '<';
                closingChar = '>';
            }
            else
            {
                throw new Exception("No parenthesized value '" + _sourceString + "' !");
            }

            bool inQuotedString = false; // Holds flag if position is quoted string or not
            char lastChar = (char)0;

            int closingCharIndex = -1;
            int nestedStartingCharCounter = 0;
            for (int i = 1; i < _sourceString.Length; i++)
            {
                // Skip escaped(\) "
                if (lastChar != '\\' && _sourceString[i] == '\"')
                {
                    // Start/end quoted string area
                    inQuotedString = !inQuotedString;
                }
                // We need to skip parenthesis in quoted string
                else if (!inQuotedString)
                {
                    // There is nested parenthesis
                    if (_sourceString[i] == startingChar)
                    {
                        nestedStartingCharCounter++;
                    }
                    // Closing char
                    else if (_sourceString[i] == closingChar)
                    {
                        // There isn't nested parenthesis closing chars left, this is closing char what we want
                        if (nestedStartingCharCounter == 0)
                        {
                            closingCharIndex = i;
                            break;
                        }
                        // This is nested parenthesis closing char
                        nestedStartingCharCounter--;
                    }
                }

                lastChar = _sourceString[i];
            }

            if (closingCharIndex == -1)
            {
                throw new Exception("There is no closing parenthesize for '" + _sourceString + "' !");
            }
            string retVal = _sourceString.Substring(1, closingCharIndex - 1);
            _sourceString = _sourceString.Substring(closingCharIndex + 1);

            return retVal;
        }

        #endregion


        #region method ReadToEnd

        /// <summary>
        /// Reads all remaining string, returns null if no chars left to read.
        /// </summary>
        /// <returns></returns>
        public string ReadToEnd()
        {
            if (this.Available == 0)
            {
                return null;
            }

            string retVal = _sourceString;
            _sourceString = "";

            return retVal;
        }

        #endregion


        #region method StartsWith

        /// <summary>
        /// Gets if source string starts with specified value. Compare is case-sensitive.
        /// </summary>
        /// <param name="value">Start string value.</param>
        /// <returns>Returns true if source string starts with specified value.</returns>
        public bool StartsWith(string value)
        {
            return _sourceString.StartsWith(value);
        }

        /// <summary>
        /// Gets if source string starts with specified value.
        /// </summary>
        /// <param name="value">Start string value.</param>
        /// <param name="case_sensitive">Specifies if compare is case-sensitive.</param>
        /// <returns>Returns true if source string starts with specified value.</returns>
        public bool StartsWith(string value, bool case_sensitive)
        {
            if (case_sensitive)
            {
                return _sourceString.StartsWith(value);
            }
            return _sourceString.ToLower().StartsWith(value.ToLower());
        }

        #endregion

        #region method EndsWith

        /// <summary>
        /// Gets if source string ends with specified value. Compare is case-sensitive.
        /// </summary>
        /// <param name="value">Start string value.</param>
        /// <returns>Returns true if source string ends with specified value.</returns>
        public bool EndsWith(string value)
        {
            return _sourceString.EndsWith(value);
        }

        /// <summary>
        /// Gets if source string ends with specified value.
        /// </summary>
        /// <param name="value">Start string value.</param>
        /// <param name="case_sensitive">Specifies if compare is case-sensitive.</param>
        /// <returns>Returns true if source string ends with specified value.</returns>
        public bool EndsWith(string value, bool case_sensitive)
        {
            if (case_sensitive)
            {
                return _sourceString.EndsWith(value);
            }
            return _sourceString.EndsWith(value, StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion

        #region method StartsWithWord

        /// <summary>
        /// Gets if current source string starts with word. For example if source string starts with
        /// whiter space or parenthesize, this method returns false.
        /// </summary>
        /// <returns></returns>
        public bool StartsWithWord()
        {
            if (_sourceString.Length == 0)
            {
                return false;
            }

            if (char.IsWhiteSpace(_sourceString[0]))
            {
                return false;
            }
            if (char.IsSeparator(_sourceString[0]))
            {
                return false;
            }
            char[] wordTerminators = new char[] { ' ', ',', ';', '{', '}', '(', ')', '[', ']', '<', '>', '\r', '\n' };
            foreach (char c in wordTerminators)
            {
                if (c == _sourceString[0])
                {
                    return false;
                }
            }

            return true;
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets how many chars are available for reading.
        /// </summary>
        public long Available
        {
            get { return _sourceString.Length; }
        }

        /// <summary>
        /// Gets original string passed to class constructor.
        /// </summary>
        public string OriginalString
        {
            get { return _originalString; }
        }

        /// <summary>
        /// Gets currently remaining string.
        /// </summary>
        public string SourceString
        {
            get { return _sourceString; }
        }

        /// <summary>
        /// Gets position in original string.
        /// </summary>
        public int Position
        {
            get { return _originalString.Length - _sourceString.Length; }
        }

        #endregion

    }
}