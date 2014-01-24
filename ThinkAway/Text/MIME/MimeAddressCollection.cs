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

using System;

namespace ThinkAway.Text.MIME
{
    internal class MimeAddressCollection : System.Collections.IEnumerable
    {
        protected System.Collections.ArrayList list = new System.Collections.ArrayList();

        public MimeAddressCollection(System.String text)
        {
            System.String[] tokens = ABNF.address_regex.Split(text);
            foreach (System.String token in tokens)
            {
                if (ABNF.address_regex.IsMatch(token))
                    this.Add(new MimeAddress(token));
            }
        }
        public MimeAddress this[int index]
        {
            get
            {
                return this.Get(index);
            }
        }
        public System.Collections.IEnumerator GetEnumerator()
        {
            return list.GetEnumerator();
        }
        public void Add(MimeAddress address)
        {
            list.Add(address);
        }
        public MimeAddress Get(int index)
        {
            return (MimeAddress)list[index];
        }
        public static MimeAddressCollection Parse(System.String text)
        {
            if (text == null)
                throw new ArgumentNullException();
            return new MimeAddressCollection(text);
        }
        public int Count
        {
            get
            {
                return list.Count;
            }
        }
        public override string ToString()
        {
            System.Text.StringBuilder text = new System.Text.StringBuilder();
            foreach (MimeAddress token in list)
            {
                text.Append(token.ToString());
                if (token.Length > 0)
                    text.Append("; ");
            }
            return text.ToString();
        }
    }
}
