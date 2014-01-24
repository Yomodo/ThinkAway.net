using System;

namespace ThinkAway.Text.MIME
{
    /// <summary>
    /// rfc 2822 header of a rfc 2045 entity
    /// </summary>
    public class MimeHeader : System.Collections.IEnumerable {

        private static System.Text.Encoding default_encoding = System.Text.Encoding.ASCII;
        private readonly MimeMessageStream _message;
        private readonly System.Collections.Specialized.HybridDictionary _headers;
        private System.String _cachedHeaders;
        private readonly long _startpoint;
        private long _endpoint;

        private struct HeaderInfo {
            public readonly System.Collections.Specialized.StringDictionary ContentType;
            public readonly System.Collections.Specialized.StringDictionary ContentdisPosition;
            public readonly System.Collections.Specialized.StringDictionary ContentLocation;
            public readonly MimeTopLevelMediaType TopLevelMediaType;
            public readonly System.Text.Encoding Enc;
            public readonly System.String Subtype;

            public HeaderInfo ( System.Collections.Specialized.HybridDictionary headers ) {
                this.TopLevelMediaType = new MimeTopLevelMediaType();
                this.Enc = null;
                try {
                    this.ContentType = MimeTools.parseHeaderFieldBody ( "Content-Type", headers["Content-Type"].ToString() );
                    this.TopLevelMediaType = (MimeTopLevelMediaType)System.Enum.Parse(TopLevelMediaType.GetType(), this.ContentType["Content-Type"].Split('/')[0], true);
                    this.Subtype = this.ContentType["Content-Type"].Split('/')[1];
                    this.Enc = MimeTools.parseCharSet ( this.ContentType["charset"] );
                } catch (System.Exception) {
                    this.Enc = default_encoding;
                    this.ContentType = MimeTools.parseHeaderFieldBody ( "Content-Type", System.String.Concat("text/plain; charset=", this.Enc.BodyName) );
                    this.TopLevelMediaType = MimeTopLevelMediaType.text;
                    this.Subtype = "plain";
                }
                if ( this.Enc==null ) {
                    this.Enc = default_encoding;
                }
                // TODO: rework this
                try {
                    this.ContentdisPosition = MimeTools.parseHeaderFieldBody ( "Content-Disposition", headers["Content-Disposition"].ToString() );
                } catch ( System.Exception ) {
                    this.ContentdisPosition = new System.Collections.Specialized.StringDictionary();
                }
                try {
                    this.ContentLocation = MimeTools.parseHeaderFieldBody ( "Content-Location", headers["Content-Location"].ToString() );
                } catch ( System.Exception ) {
                    this.ContentLocation = new System.Collections.Specialized.StringDictionary();
                }
            }
        }
        private HeaderInfo mt;

        internal MimeHeader( MimeMessageStream message ) : this ( message, 0 ){}
        internal MimeHeader(MimeMessageStream message, long startpoint) {
            this._startpoint = startpoint;
            this._message = message;
            if ( this._startpoint==0 ) {
                System.String line = this._message.ReadLine();
                // Perhaps there is part of the POP3 response
                if ( line!=null && line.Length>3 && line[0]=='+' && line[1]=='O' && line[2]=='K' ) {
#if LOG
					if ( log.IsDebugEnabled ) log.Debug ("+OK present at top of the message");
#endif
                    this._startpoint = this._message.Position;
                } else this._message.ReadLine_Undo(line);
            }
            this._headers = new System.Collections.Specialized.HybridDictionary(2, true);
            this.Parse();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MimeHeader"/> class from a <see cref="System.IO.Stream"/>
        /// </summary>
        /// <param name="message"><see cref="System.IO.Stream"/> to read headers from</param>
        public MimeHeader( System.IO.Stream message ) : this( new MimeMessageStream (message), 0 ) {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public MimeHeader( System.Byte[] message ) : this( new MimeMessageStream (message), 0 ) {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MimeHeader"/> class from a <see cref="System.IO.Stream"/> starting from the specified point
        /// </summary>
        /// <param name="message">the <see cref="System.IO.Stream" /> to read headers from</param>
        /// <param name="startpoint">initial point of the <see cref="System.IO.Stream"/> where the headers start</param>
        public MimeHeader( System.IO.Stream message, long startpoint ) : this( new MimeMessageStream (message), startpoint ) {
        }
        /// <summary>
        /// Gets header fields
        /// </summary>
        /// <param name="name">field name</param>
        /// <remarks>Field names is case insentitive</remarks>
        public System.String this[ System.Object name ] {
            get {
                return this.GetProperty( name.ToString() );
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void Close(){
            this._cachedHeaders = this._message.ReadLines( this._startpoint, this._endpoint );
            this._message.Close();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Contains ( System.String name ) {
            if ( this._headers==null )
                this.Parse();
            return this._headers.Contains(name);
        }
        /// <summary>
        /// Returns an enumerator that can iterate through the header fields
        /// </summary>
        /// <returns>A <see cref="System.Collections.IEnumerator" /> for the header fields</returns>
        public System.Collections.IEnumerator GetEnumerator() {
            return _headers.GetEnumerator();
        }
        /// <summary>
        /// Returns the requested header field body.
        /// </summary>
        /// <param name="name">Header field name</param>
        /// <param name="defaultvalue">Value to return when the requested field is not present</param>
        /// <param name="uncomment"><b>true</b> to uncomment using <see cref="MimeTools.uncommentString" />; <b>false</b> to return the value unchanged.</param>
        /// <param name="rfc2047decode"><b>true</b> to decode <see cref="MimeTools.rfc2047decode" />; <b>false</b> to return the value unchanged.</param>
        /// <returns>Header field body</returns>
        public System.String GetHeaderField ( System.String name, System.String defaultvalue, bool uncomment, bool rfc2047decode ) {
            System.String tmp = this.GetProperty(name);
            if ( tmp==null )
                tmp = defaultvalue;
            else {
                if ( uncomment )
                    tmp = MimeTools.uncommentString(tmp);
                if ( rfc2047decode )
                    tmp = MimeTools.rfc2047decode(tmp);
            }
            return tmp;
        }
        private System.String GetProperty (  System.String name ) {
            System.String Value=null;
            name = name.ToLower();
            this.Parse();
            if (this._headers == null || this._headers.Count <= 0 || name.Length <= 0 ||
                !this._headers.Contains(name))
                Value = _headers[name].ToString();
            return Value;
        }
        private bool Parse () {
            bool error = false;
            if ( this._headers.Count>0 ) {
                return !error;
            }
            System.String line = System.String.Empty;
            this._message.SeekPoint ( this._startpoint );
            this._message.Encoding = default_encoding;
            for ( line=_message.ReadUnfoldedLine(); line!=null ; line=_message.ReadUnfoldedLine() ) {
                if ( line.Length == 0 ) {
                    this._endpoint = this._message.Position_preRead;
                    this.BodyPosition = this._message.Position;
                    this._message.ReadLine_Undo(line);
                    break;
                }
                String [] headerline = line.Split ( new Char[] {':'}, 2);
                if ( headerline.Length == 2 ) {
                    headerline[1] = headerline[1].TrimStart(new Char[] {' '});
                    if ( this._headers.Contains ( headerline[0]) ) {
                        this._headers[headerline[0]] = System.String.Concat(this._headers[headerline[0]], "\r\n", headerline[1]);
                    } else {
                        this._headers.Add (headerline[0].ToLower(), headerline[1]);
                    }
                }
            }
            this.mt = new HeaderInfo ( this._headers );
            return !error;
        }

        private long _bodyPosition;

        /// <summary>
        /// Gets the point where the headers end
        /// </summary>
        /// <value>Point where the headers end</value>
        public long BodyPosition
        {
            get { return _bodyPosition; }
            private set { _bodyPosition = value; }
        }

        /// <summary>
        /// Gets CC header field
        /// </summary>
        /// <value>CC header body</value>
        public System.String Cc {
            get { return this.GetHeaderField("Cc", System.String.Empty, true, false); }
        }
        /// <summary>
        /// Gets the number of header fields found
        /// </summary>
        public int Count {
            get {
                return this._headers.Count;
            }
        }
        /// <summary>
        /// Gets Content-Disposition header field
        /// </summary>
        /// <value>Content-Disposition header body</value>
        public System.String ContentDisposition {
            get { return this.GetHeaderField("Content-Disposition", System.String.Empty, true, false); }
        }
        /// <summary>
        /// Gets the elements found in the Content-Disposition header body
        /// </summary>
        /// <value><see cref="System.Collections.Specialized.StringDictionary"/> with the elements found in the header body</value>
        public System.Collections.Specialized.StringDictionary ContentDispositionParameters {
            get {
                return this.mt.ContentdisPosition;
            }
        }
        /// <summary>
        /// Gets Content-ID header field
        /// </summary>
        /// <value>Content-ID header body</value>
        public System.String ContentID {
            get { return this.GetHeaderField("Content-ID", System.String.Empty, true, false); }
        }
        /// <summary>
        /// Gets Content-Location header field
        /// </summary>
        /// <value>Content-Location header body</value>
        public System.String ContentLocation {
            get { return this.GetHeaderField("Content-Location", System.String.Empty, true, false); }
        }
        /// <summary>
        /// Gets the elements found in the Content-Location header body
        /// </summary>
        /// <value><see cref="System.Collections.Specialized.StringDictionary"/> with the elements found in the header body</value>
        public System.Collections.Specialized.StringDictionary ContentLocationParameters {
            get {
                return this.mt.ContentLocation;
            }
        }
        /// <summary>
        /// Gets Content-Transfer-Encoding header field
        /// </summary>
        /// <value>Content-Transfer-Encoding header body</value>
        public System.String ContentTransferEncoding {
            get {
                System.String tmp = this.GetHeaderField("Content-Transfer-Encoding", null, false, false);
                if ( tmp!=null ) {
                    tmp = tmp.ToLower();
                }
                return tmp;
            }
        }
        /// <summary>
        /// Gets Content-Type header field
        /// </summary>
        /// <value>Content-Type header body</value>
        public System.String ContentType {
            get { return this.GetHeaderField("Content-Type", System.String.Concat("text/plain; charset=", this.mt.Enc.BodyName), false, false); }
        }
        /// <summary>
        /// Gets the elements found in the Content-Type header body
        /// </summary>
        /// <value><see cref="System.Collections.Specialized.StringDictionary"/> with the elements found in the header body</value>
        public System.Collections.Specialized.StringDictionary ContentTypeParameters {
            get {
                return this.mt.ContentType;
            }
        }
        /// <summary>
        /// Gets Date header field
        /// </summary>
        /// <value>Date header body</value>
        public System.String Date {
            get { return this.GetHeaderField("Date", System.String.Empty, true, false); }
        }
        /// <summary>
        /// Gets <see cref="System.Text.Encoding"/> found on the headers and applies to the body
        /// </summary>
        /// <value><see cref="System.Text.Encoding"/> for the body</value>
        public System.Text.Encoding Encoding {
            get {
                this.Parse();
                return this.mt.Enc;
            }
        }
        /// <summary>
        /// Gets or sets the default <see cref="System.Text.Encoding" /> used when it isn't defined otherwise.
        /// </summary>
        /// <value>The <see cref="System.Text.Encoding" /> used when it isn't defined otherwise</value>
        /// <remarks>The default value is <see cref="System.Text.ASCIIEncoding" /> as defined in RFC 2045 section 5.2.<br />
        /// If you change this value you'll be violating this rfc section.</remarks>
        public static System.Text.Encoding EncodingDefault {
            get {return default_encoding; }
            set {
                if ( value!=null && !value.BodyName.Equals(System.String.Empty) )
                    default_encoding=value;
                else
                    default_encoding=System.Text.Encoding.ASCII;
            }
        }
        /// <summary>
        /// Gets From header field
        /// </summary>
        /// <value>From header body</value>
        public System.String From {
            get { return this.GetHeaderField("From", System.String.Empty, true, false); }
        }
        /// <summary>
        /// Gets Raw headers
        /// </summary>
        /// <value>From header body</value>
        public System.String RawHeaders {
            get
            {
                return _cachedHeaders ?? _message.ReadLines( this._startpoint, this._endpoint );
            }
        }
        /// <summary>
        /// Gets Message-ID header field
        /// </summary>
        /// <value>Message-ID header body</value>
        public System.String MessageID {
            get { return this.GetHeaderField("Message-ID", System.String.Empty, true, false); }
        }
        /// <summary>
        /// Gets reply address as defined by <c>rfc 2822</c>
        /// </summary>
        /// <value>Reply address</value>
        public System.String Reply {
            get
            {
                return !ReplyTo.Equals(System.String.Empty) ? ReplyTo : From;
            }
        }
        /// <summary>
        /// Gets Reply-To header field
        /// </summary>
        /// <value>Reply-To header body</value>
        public System.String ReplyTo {
            get { return this.GetHeaderField("Reply-To", System.String.Empty, true, false); }
        }
        /// <summary>
        /// Gets Return-Path header field
        /// </summary>
        /// <value>Return-Path header body</value>
        public System.String ReturnPath {
            get { return this.GetHeaderField("Return-Path", System.String.Empty, true, false); }
        }
        /// <summary>
        /// Gets Sender header field
        /// </summary>
        /// <value>Sender header body</value>
        public System.String Sender {
            get { return this.GetHeaderField("Sender", System.String.Empty, true, false); }
        }
        /// <summary>
        /// Gets Subject header field
        /// </summary>
        /// <value>Subject header body</value>
        public System.String Subject {
            get { return this.GetHeaderField("Subject", System.String.Empty, false, false); }
        }
        /// <summary>
        /// Gets SubType from Content-Type header field
        /// </summary>
        /// <value>SubType from Content-Type header field</value>
        public System.String SubType {
            get {
                this.Parse();
                return this.mt.Subtype;
            }
        }
        /// <summary>
        /// Gets To header field
        /// </summary>
        /// <value>To header body</value>
        public System.String To {
            get { return this.GetHeaderField("To", System.String.Empty, true, false); }
        }
        /// <summary>
        /// Gets top-level media type from Content-Type header field
        /// </summary>
        /// <value>Top-level media type from Content-Type header field</value>
        public MimeTopLevelMediaType TopLevelMediaType {
            get {
                this.Parse();
                return this.mt.TopLevelMediaType;
            }
        }
        /// <summary>
        /// Gets Version header field
        /// </summary>
        /// <value>Version header body</value>
        public System.String Version {
            get { return this.GetHeaderField("Version", "1.0", true, false); }
        }
    }
}