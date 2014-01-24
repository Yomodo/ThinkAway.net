using System;
using System.Collections.Generic;
using ThinkAway.Text.MIME.Decode;
/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.Net.Mail
{
    /// <summary>This Type stores a rfc822 email address and a name for that
    /// particular address (Example: "John Smith, jsmith@nowhere.com")
    /// </summary>
    /// <example>
    /// <code>
    /// EmailAddress from = new EmailAddress("user@url.com", "John Smith");
    /// EmailAddress to = new EmailAddress("support@OpenSmtp.com");
    /// MailMessage msg = new MailMessage(from, to);
    /// </code>
    /// </example>
    public class MailAddress
    {
        #region Properties

        private string _address;

        ///<summary>
        /// The email address of this <see cref="MailAddress"/><br/>
        /// It is possibly string.Empty since RFC mail addresses does not require an email address specified.
        ///</summary>
        ///<example>
        /// Example header with email address:<br/>
        /// To: <c>Test test@mail.com</c><br/>
        /// Address will be <c>test@mail.com</c><br/>
        ///</example>
        ///<example>
        /// Example header without email address:<br/>
        /// To: <c>Test</c><br/>
        /// Address will be <see cref="string.Empty"/>.
        ///</example>
        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }

        private string _displayName;

        ///<summary>
        /// The display name of this <see cref="MailAddress"/><br/>
        /// It is possibly <see cref="string.Empty"/> since RFC mail addresses does not require a display name to be specified.
        ///</summary>
        ///<example>
        /// Example header with display name:<br/>
        /// To: <c>Test test@mail.com</c><br/>
        /// DisplayName will be <c>Test</c>
        ///</example>
        ///<example>
        /// Example header without display name:<br/>
        /// To: <c>test@test.com</c><br/>
        /// DisplayName will be <see cref="string.Empty"/>
        ///</example>
        public string DisplayName
        {
            get { return _displayName; }
            private set { _displayName = value; }
        }

        #endregion

        #region Constructors


        /// <summary>
        /// Mail address string  
        /// <example>
        ///  Examples:
        /// <br />
        /// eg: <c>>test@lsong.org</c>
        ///     <c>>Test<test@lsong.org></test></c>
        /// <code> 
        /// MailAddress mailAddress = new MailAddress("Test<test@lsong.org/>");
        /// MailAddress mailAddress = new MailAddress("test@lsong.org"); 
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="address"></param>
        public MailAddress(string address)
        {
            if (address == null)
                throw new ArgumentNullException("address");
            MailAddress mailAddress =  ParseMailAddress(address);
            this.DisplayName = mailAddress.DisplayName;
            this.Address = mailAddress.Address;
        }
        /// <summary>
        /// Mail address string 
        /// <example>
        /// Examples:
        /// <br />
        /// <code>
        /// MailAddress mailAddress = new MailAddress("Lsong","i@lsong.org");
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="displayName">displayName</param>
        /// <param name="address">address</param>
        public MailAddress(string displayName,string address)
        {
            if (displayName == null)
                throw new ArgumentNullException("displayName");
             if (address == null)
                throw new ArgumentNullException("address");
             DisplayName = displayName;
             Address = address;            
        }

        #endregion


        #region Parsing
        /// <summary>
        /// Parses an email address from a MIME header<br/>
        /// <br/>
        /// Examples of input:
        /// <c>Eksperten mailrobot &lt;noreply@mail.eksperten.dk&gt;</c><br/>
        /// <c>"Eksperten mailrobot" &lt;noreply@mail.eksperten.dk&gt;</c><br/>
        /// <c>&lt;noreply@mail.eksperten.dk&gt;</c><br/>
        /// <c>noreply@mail.eksperten.dk</c><br/>
        /// <br/>
        /// It might also contain encoded text, which will then be decoded.
        /// </summary>
        /// <param name="input">The value to parse out and email and/or a username</param>
        /// <returns>A <see cref="MailAddress"/></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="input"/> is <see langword="null"/></exception>
        /// <remarks>
        /// <see href="http://tools.ietf.org/html/rfc5322#section-3.4">RFC 5322 section 3.4</see> for more details on email syntax.<br/>
        /// <see cref="EncodedWord.Decode">For more information about encoded text</see>.
        /// </remarks>
        internal static MailAddress ParseMailAddress(string input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Decode the value, if it was encoded
            input = EncodedWord.Decode(input.Trim());

            // Find the location of the email address
            int indexStartEmail = input.LastIndexOf('<');
            int indexEndEmail = input.LastIndexOf('>');
            int indexAtEmail = input.LastIndexOf('@');
            //Display Name
            string displayName = "",address = "";
            //Find it
            if(indexStartEmail == -1 || indexEndEmail == -1)
            {
            	if(indexAtEmail != -1)
            	{           			
            		displayName = input.Substring(0,indexAtEmail);
            		address = input;
            	}
            }
            else
            {
                    // Check if there is a username in front of the email address
                    if (indexStartEmail > 0)
                    {
                        // Parse out the user
                        displayName = input.Substring(0, indexStartEmail).Trim();
                        displayName = displayName.Trim('"');
                    }
                    else
                    {
                        // There was no user
                        displayName = string.Empty;
                    }

                    // Parse out the email address without the "<"  and ">"
                    indexStartEmail = indexStartEmail + 1;
                    int emailLength = indexEndEmail - indexStartEmail;
                    address = input.Substring(indexStartEmail, emailLength).Trim();
            }
            //Regex mailRegex = new Regex(@"^\w+@\w+(\.\w+)+(\,\w+@\w+(\.\w+)+)*$"); 
            //严格验证
            //mailRegex.IsMatch(address)
            //宽松验证 
            if(!address.Contains("@"))
            {
                throw new FormatException(string.Format("无效的 Mail 地址格式:{0}", address));
            }
            
            MailAddress mailAddress = new MailAddress(displayName,address);
            // It could be that the format used was simply a name
            // which is indeed valid according to the RFC
            // Example:
            // Eksperten mailrobot
            return mailAddress;
        }

        /// <summary>
        /// Parses input of the form<br/>
        /// <c>Eksperten mailrobot &lt;noreply@mail.eksperten.dk&gt;, ...</c><br/>
        /// to a list of MailAddresses
        /// </summary>
        /// <param name="input">The input that is a comma-separated list of EmailAddresses to parse</param>
        /// <returns>A List of <seealso cref="MailAddress"/> objects extracted from the <paramref name="input"/> parameter.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="input"/> is <see langword="null"/></exception>
        internal static List<MailAddress> ParseMailAddresses(string input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // MailAddresses are split by commas
            IEnumerable<string> mailAddresses = SplitMailAddresses(input);

            // Parse each of these

            List<MailAddress> list = new List<MailAddress>();
            foreach (string s in mailAddresses)
                list.Add(ParseMailAddress(s));
            return list;
        }

        /// <summary>
        /// Split a list of addresses in one string into a elements of addresses.<br/>
        /// Basically a split is needed at each comma, except if the comma is inside a quote.
        /// </summary>
        /// <param name="input">The list of addresses to be split</param>
        /// <returns>Each address as an element</returns>
        private static IEnumerable<string> SplitMailAddresses(string input)
        {
            List<string> addresses = new List<string>();

            int lastSplitLocation = 0;
            bool insideQuote = false;

            char[] characters = input.ToCharArray();

            for (int i = 0; i < characters.Length; i++)
            {
                char character = characters[i];
                if (character == '\"')
                    insideQuote = !insideQuote;

                // Only split if a comma is met, but we are not inside quotes
                if (character == ',' && !insideQuote)
                {
                    // We need to split
                    int length = i - lastSplitLocation;
                    addresses.Add(input.Substring(lastSplitLocation, length));

                    // Update last split location
                    // + 1 so that we do not include comma next time
                    lastSplitLocation = i + 1;
                }
            }

            // Add the last part
            addresses.Add(input.Substring(lastSplitLocation, input.Length - lastSplitLocation));

            return addresses;
        }
        #endregion       

		public System.Net.Mail.MailAddress ToMailAddress()
		{
		    System.Net.Mail.MailAddress mailAddress = new System.Net.Mail.MailAddress(Address,DisplayName);
		    return mailAddress;
		}

        public override string ToString()
		{		
			return  string.Format("{0}<{1}>",DisplayName,Address);
		}

    }
}
