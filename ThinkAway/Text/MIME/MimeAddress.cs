using System;

namespace ThinkAway.Text.MIME
{
    /// <summary>
    /// rfc 2822 email address
    /// </summary>
    public class MimeAddress {
        private readonly System.String _name;
        private readonly System.String _address;
        /// <summary>
        /// Initializes a new address from a RFC 2822 name-addr specification string
        /// </summary>
        /// <param name="dir">RFC 2822 name-addr address</param>
        /// 
        public MimeAddress ( System.String dir ) {
            _name = MimeTools.parseFrom ( dir, 1 );
            _address = MimeTools.parseFrom ( dir, 2 );
        }
        /// <summary>
        /// Gets the decoded address or name contained in the name-addr
        /// </summary>
        public System.String this [System.Object key] {
            get {
                if ( key == null ) throw new System.ArgumentNullException();
                switch (key.ToString()) {
                    case "0":
                    case "name":
                        return this._name;
                    case "1":
                    case "address":
                        return this._address;
                }
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override System.String ToString() {
            if ( _name.Equals (System.String.Empty ) && _address.Equals (System.String.Empty ) )
                return "";
            return Equals(System.String.Empty, _name) ? String.Format("<{0}>", _address) : String.Format("\"{0}\" <{1}>", _name, _address);
        }
        /// <summary>
        /// Gets the length of the decoded address
        /// </summary>
        public int Length {
            get {
                return this._name.Length + this._address.Length;
            }
        }
    }
}