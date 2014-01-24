// -----------------------------------------------------------------------
//
//   Copyright (C) 2003-2006 Angel Marin
// 
//   This file is part of SharpMimeTools
//
//   SharpMimeTools is free software; you can redistribute it and/or
//   modify it under the terms of the GNU Lesser General Public
//   License as published by the Free Software Foundation; either
//   version 2.1 of the License, or (at your option) any later version.
//
//   SharpMimeTools is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//   Lesser General Public License for more details.
//
//   You should have received a copy of the GNU Lesser General Public
//   License along with SharpMimeTools; if not, write to the Free Software
//   Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
//
// -----------------------------------------------------------------------

namespace ThinkAway.Text.MIME
{
	internal class MimeMessageCollection : System.Collections.IEnumerable {
		protected MimeMessage parent;
		protected System.Collections.ArrayList messages = new System.Collections.ArrayList();
	
		public MimeMessage this[ int index ] {
			get { return this.Get( index ); }
		}
		public void Add ( MimeMessage msg ) {
			messages.Add( msg );
		}
		public MimeMessage Get( int index ) {
			return (MimeMessage)messages[index];
		}
		public System.Collections.IEnumerator GetEnumerator() {
			return messages.GetEnumerator();
		}
		public void Clear () {
			messages.Clear();
		}
		public int Count {
			get {
				return messages.Count;
			}
		}
		public MimeMessage Parent {
			get {
				return this.parent;
			}
			set {
				this.parent = value;
			}
		}
	}
}
