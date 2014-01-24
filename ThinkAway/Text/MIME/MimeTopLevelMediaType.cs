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
    /// <summary>
	/// RFC 2046 Initial top-level media types
	/// </summary>
	[Flags]
	public enum MimeTopLevelMediaType {
		/// <summary>
		/// RFC 2046 section 4.1
		/// </summary>
		text = 1,
		/// <summary>
		/// RFC 2046 section 4.2
		/// </summary>
		image = 2,
		/// <summary>
		/// RFC 2046 section 4.3
		/// </summary>
		audio = 4,
		/// <summary>
		/// RFC 2046 section 4.4
		/// </summary>
		video = 8,
		/// <summary>
		/// RFC 2046 section 4.5
		/// </summary>
		application = 16,
		/// <summary>
		/// RFC 2046 section 5.1
		/// </summary>
		multipart = 32,
		/// <summary>
		/// RFC 2046 section 5.2
		/// </summary>
		message = 64
	}
}
