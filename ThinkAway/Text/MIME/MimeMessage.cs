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
	/// rfc 2045 entity
	/// </summary>
	public class MimeMessage : System.Collections.IEnumerable {
		private struct MessageInfo {
		    private readonly long start;
			internal readonly long start_body;
			internal long end;
			internal readonly MimeHeader header;
			internal readonly MimeMessageCollection parts;

			internal MessageInfo (MimeMessageStream m, long start ) {
				this.start = start;
				this.header = new MimeHeader ( m, this.start );
				this.start_body = this.header.BodyPosition;
				this.end = -1;
				parts = new MimeMessageCollection();
			}
		}

		private readonly MimeMessageStream message;
		private MessageInfo _mi;

		/// <summary>
		/// Initializes a new instance of the <see cref="MimeMessage"/> class from a <see cref="System.IO.Stream"/>
		/// </summary>
		/// <param name="message"><see cref="System.IO.Stream" /> to read the message from</param>
		public MimeMessage( System.IO.Stream message ) {
			this.message = new MimeMessageStream (message);
			this._mi = new MessageInfo ( this.message, this.message.Stream.Position );
		}
		private MimeMessage(MimeMessageStream message, long startpoint ) {
			this.message = message;
			this._mi = new MessageInfo ( this.message, startpoint );
		}
		private MimeMessage(MimeMessageStream message, long startpoint, long endpoint ) {
			this.message = message;
			this._mi = new MessageInfo ( this.message, startpoint );
			this._mi.end = endpoint;
		}
		/// <summary>
		/// Clears the parts references contained in this instance and calls the <b>Close</b> method in those parts.
		/// </summary>
		/// <remarks>This method does not close the underling <see cref="System.IO.Stream" /> used to create this instance.</remarks>
		public void Close() {
			foreach ( MimeMessage part in this._mi.parts )
				part.Close();
			this._mi.parts.Clear();
		}
		/// <summary>
		/// Dumps the body of this entity into a <see cref="System.IO.Stream"/>
		/// </summary>
		/// <param name="stream"><see cref="System.IO.Stream" /> where we want to write the body</param>
		/// <returns><b>true</b> OK;<b>false</b> if write operation fails</returns>
        public void DumpBody(System.IO.Stream stream)
		{
		    if (stream == null)
		        return;
		    if (!stream.CanWrite)
		        return;
		    System.Byte[] buffer;
		    switch (this.Header.ContentTransferEncoding)
		    {
		        case "quoted-printable":
		            buffer = _mi.header.Encoding.GetBytes(this.BodyDecoded);
		            break;
		        case "base64":
		            try
		            {
		                buffer = Convert.FromBase64String(this.Body);
		            }
		            catch (System.Exception e)
		            {
		                throw new Exception("Error Converting base64 string", e);
		            }
		            break;
		        case "7bit":
		        case "8bit":
		        case "binary":
		        case null:
		            buffer = System.Text.Encoding.ASCII.GetBytes(this.Body);
		            break;
		        default:
                    throw new Exception(System.String.Concat("Unsuported Content-Transfer-Encoding [", this.Header.ContentTransferEncoding, "]"));
		    }
		    try
		    {
                stream.Write(buffer, 0, buffer.Length);
		    }
		    catch (System.Exception exception)
		    {
		        throw new Exception("Error dumping body", exception);
		    }
		}

	    /// <summary>
		/// Dumps the body of this entity into a file
		/// </summary>
		/// <param name="path">path of the destination folder</param>
		/// <returns><see cref="System.IO.FileInfo" /> that represents the file where the body has been saved</returns>
		public System.IO.FileInfo DumpBody ( System.String path ) {
			return this.DumpBody ( path, this.Name );
		}
		/// <summary>
		/// Dumps the body of this entity into a file
		/// </summary>
		/// <param name="path">path of the destination folder</param>
		/// <param name="generatename">true if the filename must be generated incase we can't find a valid one in the headers</param>
		/// <returns><see cref="System.IO.FileInfo" /> that represents the file where the body has been saved</returns>
		public System.IO.FileInfo DumpBody ( System.String path, bool generatename ) {
			System.String name = this.Name;
			if ( name==null && generatename )
				name = System.String.Format ( "generated_{0}.{1}", this.GetHashCode(), this.Header.SubType );
			return this.DumpBody ( path, name );
		}
		/// <summary>
		/// Dumps the body of this entity into a file
		/// </summary>
		/// <param name="path">path of the destination folder</param>
		/// <param name="name">name of the file</param>
		/// <returns><see cref="System.IO.FileInfo" /> that represents the file where the body has been saved</returns>
        public System.IO.FileInfo DumpBody(System.String path, System.String name)
		{
		    System.IO.FileInfo file = null;
		    if (name != null)
		    {

		        name = System.IO.Path.GetFileName(name);
		        // Dump file contents
		        try
		        {
		            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(path);
		            dir.Create();
		            try
		            {
		                file = new System.IO.FileInfo(System.IO.Path.Combine(path, name));
		            }
		            catch (System.ArgumentException exception)
		            {
		                file = null;
		                throw new Exception(
		                    System.String.Concat("Filename [", System.IO.Path.Combine(path, name),"] is not allowed by the filesystem"), exception);
		            }
		            if (dir.Exists)
		            {
		                if (dir.FullName.Equals(new System.IO.DirectoryInfo(file.Directory.FullName).FullName))
		                {
		                    if (!file.Exists)
		                    {
		                        System.IO.Stream stream;
		                        try
		                        {
		                            stream = file.Create();

		                        }
		                        catch (System.Exception e)
		                        {
		                            throw new Exception(System.String.Concat("Error creating file [", file.FullName, "]"), e);

		                        }
		                        try
		                        {
		                            this.DumpBody(stream);
		                        }
		                        catch (Exception)
		                        {
                                    throw new Exception(System.String.Concat("Error writting file [", file.FullName, "] to disk"));
		                        }
		                        stream.Close();
	
		                        // The file should be there
		                        file.Refresh();
		                        // Set file dates
		                        if (this.Header.ContentDispositionParameters.ContainsKey("creation-date"))
		                            file.CreationTime =
		                                MimeTools.parseDate(this.Header.ContentDispositionParameters["creation-date"]);
		                        if (this.Header.ContentDispositionParameters.ContainsKey("modification-date"))
		                            file.LastWriteTime =
		                                MimeTools.parseDate(this.Header.ContentDispositionParameters["modification-date"]);
		                        if (this.Header.ContentDispositionParameters.ContainsKey("read-date"))
		                            file.LastAccessTime =
		                                MimeTools.parseDate(this.Header.ContentDispositionParameters["read-date"]);

		                    }
		                    else
		                    {
		                        System.Console.WriteLine("File already exists, skipping.");
		                    }

		                }

		            }
		        }
		        catch (System.Exception)
		        {
                    if (file != null)
                    {
                        file.Refresh();
                        if (file.Exists)
                            file.Delete();
                    }
		            file = null;
		        }
		    }
		    return file;
		}

	    /// <summary>
		/// Returns an enumerator that can iterate through the parts of a multipart entity
		/// </summary>
		/// <returns>A <see cref="System.Collections.IEnumerator" /> for the parts of a multipart entity</returns>
		public System.Collections.IEnumerator GetEnumerator() {
			this.Parse();
			return this._mi.parts.GetEnumerator();
		}
		/// <summary>
		/// Returns the requested part of a multipart entity
		/// </summary>
		/// <param name="index">index of the requested part</param>
		/// <returns>A <see cref="MimeMessage" /> for the requested part</returns>
		public MimeMessage GetPart ( int index ) {
			return this.Parts.Get ( index );
		}
		private void Parse () {
			if ( !this.IsMultipart || this.Equals(this._mi.parts.Parent) ) {
			    return;
			}
			switch (this._mi.header.TopLevelMediaType) {
				case MimeTopLevelMediaType.message:
					this._mi.parts.Parent = this;
					MimeMessage mimeMessage = new MimeMessage (this.message, this._mi.start_body, this._mi.end );
					this._mi.parts.Add (mimeMessage);
					break;
				case MimeTopLevelMediaType.multipart:
					this.message.SeekPoint ( this._mi.start_body );
					System.String line;
					this._mi.parts.Parent = this;
					System.String boundary_start = System.String.Concat("--", this._mi.header.ContentTypeParameters["boundary"]);
					System.String boundary_end = System.String.Concat("--", this._mi.header.ContentTypeParameters["boundary"], "--");
					for ( line=this.message.ReadLine(); line!=null ; line=this.message.ReadLine() ) {
						// It can't be a boundary line
						if ( line.Length<3 )
							continue;
						// Match start boundary line
						if ( line.Length==boundary_start.Length && line==boundary_start ) {
							if ( this._mi.parts.Count>0 ) {
								this._mi.parts.Get( this._mi.parts.Count-1 )._mi.end = this.message.Position_preRead;

							}

							MimeMessage msg = new MimeMessage (this.message, this.message.Position );
							this._mi.parts.Add (msg);
						// Match end boundary line
						} else if ( line.Length==boundary_end.Length && line==boundary_end ) {
							this._mi.end = this.message.Position_preRead;
							if ( this._mi.parts.Count>0 ) {
								this._mi.parts.Get( this._mi.parts.Count-1 )._mi.end = this.message.Position_preRead;
							}
							break;
						}
					}
					break;
			}
		    return;
		}
		/// <summary>
		/// Gets header fields for this entity
		/// </summary>
		/// <param name="name">field name</param>
		/// <remarks>Field names is case insentitive</remarks>
		public System.String this[ System.Object name ] {
			get { return this._mi.header[ name.ToString()]; }
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public System.String Body {
			get {
				this.Parse();
				if ( this._mi.parts.Count == 0 ) {
					this.message.Encoding = this._mi.header.Encoding;
					if ( this._mi.end ==-1 ) {
						return this.message.ReadAll(this._mi.start_body);
					}
				    return this.message.ReadLines(this._mi.start_body, this._mi.end);
				}
			    return null;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public System.String BodyDecoded {
			get {
				switch (this.Header.ContentTransferEncoding) {
					case "quoted-printable":
						System.String body = this.Body;
						MimeTools.QuotedPrintable2Unicode ( this._mi.header.Encoding, ref body );
						return body;
					case "base64":
						System.Byte[] tmp;
						try {
							tmp = System.Convert.FromBase64String(this.Body);

						} catch (Exception exception)
						{
                            throw new Exception("Error dumping body",exception);
						}
						return _mi.header.Encoding.GetString(tmp);
				}
				return this.Body;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public System.String Disposition {
			get {
				return this.Header.ContentDispositionParameters["Content-Disposition"];
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public MimeHeader Header {
			get {
				return this._mi.header;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool IsBrowserDisplay {
			get {
				switch (this._mi.header.TopLevelMediaType) {
					case MimeTopLevelMediaType.audio:
					case MimeTopLevelMediaType.image:
					case MimeTopLevelMediaType.text:
					case MimeTopLevelMediaType.video:
						return true;
					default:
						return false;
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool IsMultipart {
			get {
				switch (this._mi.header.TopLevelMediaType) {
					case MimeTopLevelMediaType.multipart:
					case MimeTopLevelMediaType.message:
						return true;
					default:
						return false;
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool IsTextBrowserDisplay {
			get
			{
			    return _mi.header.TopLevelMediaType.Equals(MimeTopLevelMediaType.text) && this._mi.header.SubType.Equals("plain");
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public System.String Name {
			get {
				this.Parse();
			    System.String param = (this.Header.ContentDispositionParameters["filename"] ??
			                           this.Header.ContentTypeParameters["name"]) ??
			                          this.Header.ContentLocationParameters["Content-Location"];

			    return MimeTools.GetFileName(param);
			}
		}
		internal MimeMessageCollection  Parts {
			get {
				this.Parse();
				return this._mi.parts;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public int PartsCount {
			get {
				this.Parse();
				return this._mi.parts.Count;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public long Size {
			get {
				this.Parse();
				return this._mi.end - this._mi.start_body;
			}
		}
	}
}
