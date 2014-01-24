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

using System.IO;

namespace ThinkAway.Text.MIME
{
    /// <summary>
    /// This class provides a simplified way of parsing messages. 
    /// </summary>
    /// <remarks> All the mime complexity is handled internally and all the content is exposed
    /// parsed and decoded. The code takes care of comments, RFC 2047, encodings, etc.</remarks>
    /// <example>Parse a message read from a file enabling the uuencode and ms-tnef decoding and saving attachments to disk.
    /// <code>
    /// System.IO.FileStream msg = new System.IO.FileStream("message_file.txt", System.IO.FileMode.Open);
    /// anmar.SharpMimeTools.SharpMessage message = new anmar.SharpMimeTools.SharpMessage(msg, SharpDecodeOptions.Default|SharpDecodeOptions.DecodeTnef|SharpDecodeOptions.UuDecode);
    /// msg.Close();
    /// Console.WriteLine(System.String.Concat("From:    [", message.From, "][", message.FromAddress, "]"));
    /// Console.WriteLine(System.String.Concat("To:      [", message.To, "]"));
    /// Console.WriteLine(System.String.Concat("Subject: [", message.Subject, "]"));
    /// Console.WriteLine(System.String.Concat("Date:    [", message.Date, "]"));
    /// Console.WriteLine(System.String.Concat("Body:    [", message.Body, "]"));
    /// if ( message.Attachments!=null ) {
    /// 	foreach ( anmar.SharpMimeTools.SharpAttachment attachment in message.Attachments ) {
    /// 		attachment.Save(System.Environment.CurrentDirectory, false);
    /// 		Console.WriteLine(System.String.Concat("Attachment: [", attachment.SavedFile.FullName, "]"));
    /// 	}
    /// }
    /// </code>
    /// </example>
    /// <example>This sample shows how simple is to parse an e-mail message read from a file.
    /// <code>
    /// System.IO.FileStream msg = new System.IO.FileStream("message_file.txt", System.IO.FileMode.Open);
    /// anmar.SharpMimeTools.SharpMessage message = new anmar.SharpMimeTools.SharpMessage(msg);
    /// msg.Close();
    /// Console.WriteLine(System.String.Concat("From:    [", message.From, "][", message.FromAddress, "]"));
    /// Console.WriteLine(System.String.Concat("To:      [", message.To, "]"));
    /// Console.WriteLine(System.String.Concat("Subject: [", message.Subject, "]"));
    /// Console.WriteLine(System.String.Concat("Date:    [", message.Date, "]"));
    /// Console.WriteLine(System.String.Concat("Body:    [", message.Body, "]"));
    /// </code>
    /// </example>
    public sealed class SharpMessage
    {

        private System.Collections.ArrayList _attachments;
        private System.String _body = System.String.Empty;
        private bool _bodyHtml;
        private System.DateTime _date;
        private System.String _from_addr = System.String.Empty;
        private System.String _from_name = System.String.Empty;
        private MimeHeader _headers;
        private System.String _subject = System.String.Empty;
        private MimeAddressCollection _to;

        /// <summary>
        /// Initializes a new instance of the <see cref="SharpMessage" /> class based on the supplied <see cref="System.IO.Stream" />.
        /// </summary>
        /// <param name="message"><see cref="System.IO.Stream" /> that contains the message content</param>
        /// <remarks>The message content is automatically parsed.</remarks>
        public SharpMessage(System.IO.Stream message)
            : this(message, true, true, null, null)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SharpMessage" /> class based on the supplied <see cref="System.IO.Stream" />.
        /// </summary>
        /// <param name="message"><see cref="System.IO.Stream" /> that contains the message content</param>
        /// <param name="attachments"><b>true</b> to allow attachments; <b>false</b> to skip them.</param>
        /// <param name="html"><b>true</b> to allow HTML content; <b>false</b> to ignore the html content.</param>
        /// <remarks>When the <b>attachments</b> parameter is true it's equivalent to adding <b>anmar.SharpMimeTools.MimeTopLevelMediaType.application</b>, <b>anmar.SharpMimeTools.MimeTopLevelMediaType.audio</b>, <b>anmar.SharpMimeTools.MimeTopLevelMediaType.image</b>, <b>anmar.SharpMimeTools.MimeTopLevelMediaType.video</b> to the allowed <see cref="MimeTopLevelMediaType" />.<br />
        /// <b>anmar.SharpMimeTools.MimeTopLevelMediaType.text</b>, <b>anmar.SharpMimeTools.MimeTopLevelMediaType.multipart</b> and <b>anmar.SharpMimeTools.MimeTopLevelMediaType.message</b> are allowed in any case.<br /><br />
        /// In order to have better control over what is parsed, see the other contructors.
        /// </remarks>
        public SharpMessage(System.IO.Stream message, bool attachments, bool html)
            : this(message, attachments, html, null, null)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SharpMessage" /> class based on the supplied <see cref="System.IO.Stream" />.
        /// </summary>
        /// <param name="message"><see cref="System.IO.Stream" /> that contains the message content</param>
        /// <param name="attachments"><b>true</b> to allow attachments; <b>false</b> to skip them.</param>
        /// <param name="html"><b>true</b> to allow HTML content; <b>false</b> to ignore the html content.</param>
        /// <param name="path">A <see cref="System.String" /> specifying the path on which to save the attachments found.</param>
        /// <remarks>When the <b>attachments</b> parameter is true it's equivalent to adding <b>anmar.SharpMimeTools.MimeTopLevelMediaType.application</b>, <b>anmar.SharpMimeTools.MimeTopLevelMediaType.audio</b>, <b>anmar.SharpMimeTools.MimeTopLevelMediaType.image</b>, <b>anmar.SharpMimeTools.MimeTopLevelMediaType.video</b> to the allowed <see cref="MimeTopLevelMediaType" />.<br />
        /// <b>anmar.SharpMimeTools.MimeTopLevelMediaType.text</b>, <b>anmar.SharpMimeTools.MimeTopLevelMediaType.multipart</b> and <b>anmar.SharpMimeTools.MimeTopLevelMediaType.message</b> are allowed in any case.<br /><br />
        /// In order to have better control over what is parsed, see the other contructors.
        /// </remarks>
        public SharpMessage(System.IO.Stream message, bool attachments, bool html, System.String path)
            : this(message, attachments, html, path, null)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SharpMessage" /> class based on the supplied <see cref="System.IO.Stream" />.
        /// </summary>
        /// <param name="message"><see cref="System.IO.Stream" /> that contains the message content</param>
        /// <param name="attachments"><b>true</b> to allow attachments; <b>false</b> to skip them.</param>
        /// <param name="html"><b>true</b> to allow HTML content; <b>false</b> to ignore the html content.</param>
        /// <param name="path">A <see cref="System.String" /> specifying the path on which to save the attachments found.</param>
        /// <param name="preferredtextsubtype">A <see cref="System.String" /> specifying the subtype to select for text parts that contain aternative content (plain, html, ...). Specify the <b>null</b> reference to maintain the default behavior (prefer whatever the originator sent as the preferred part). If there is not a text part with this subtype, the default one is used.</param>
        /// <remarks>When the <b>attachments</b> parameter is true it's equivalent to adding <b>anmar.SharpMimeTools.MimeTopLevelMediaType.application</b>, <b>anmar.SharpMimeTools.MimeTopLevelMediaType.audio</b>, <b>anmar.SharpMimeTools.MimeTopLevelMediaType.image</b>, <b>anmar.SharpMimeTools.MimeTopLevelMediaType.video</b> to the allowed <see cref="MimeTopLevelMediaType" />.<br />
        /// <b>anmar.SharpMimeTools.MimeTopLevelMediaType.text</b>, <b>anmar.SharpMimeTools.MimeTopLevelMediaType.multipart</b> and <b>anmar.SharpMimeTools.MimeTopLevelMediaType.message</b> are allowed in any case.<br /><br />
        /// In order to have better control over what is parsed, see the other contructors.
        /// </remarks>
        public SharpMessage(System.IO.Stream message, bool attachments, bool html, System.String path, System.String preferredtextsubtype)
        {
            MimeTopLevelMediaType types = MimeTopLevelMediaType.text | MimeTopLevelMediaType.multipart | MimeTopLevelMediaType.message;
            DecodeOptions options = DecodeOptions.None;
            if (attachments)
            {
                types = types | MimeTopLevelMediaType.application | MimeTopLevelMediaType.audio | MimeTopLevelMediaType.image | MimeTopLevelMediaType.video;
                options = options | DecodeOptions.AllowAttachments;
            }
            if (html)
                options = options | DecodeOptions.AllowHtml;
            if (path == null || !System.IO.Directory.Exists(path))
                this.ParseMessage(message, types, options, preferredtextsubtype, null);
            else
                this.ParseMessage(message, types, options, preferredtextsubtype, path);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SharpMessage" /> class based on the supplied <see cref="System.IO.Stream" />.
        /// </summary>
        /// <param name="message"><see cref="System.IO.Stream" /> that contains the message content</param>
        /// <param name="types">A <see cref="MimeTopLevelMediaType" /> value that specifies the allowed Mime-Types to being decoded.</param>
        /// <param name="html"><b>true</b> to allow HTML content; <b>false</b> to ignore the html content.</param>
        public SharpMessage(System.IO.Stream message, MimeTopLevelMediaType types, bool html)
            : this(message, types, html, null, null)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SharpMessage" /> class based on the supplied <see cref="System.IO.Stream" />.
        /// </summary>
        /// <param name="message"><see cref="System.IO.Stream" /> that contains the message content</param>
        /// <param name="types">A <see cref="MimeTopLevelMediaType" /> value that specifies the allowed Mime-Types to being decoded.</param>
        /// <param name="html"><b>true</b> to allow HTML content; <b>false</b> to ignore the html content.</param>
        /// <param name="path">A <see cref="System.String" /> specifying the path on which to save the attachments found.</param>
        public SharpMessage(System.IO.Stream message, MimeTopLevelMediaType types, bool html, System.String path)
            : this(message, types, html, path, null)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SharpMessage" /> class based on the supplied <see cref="System.IO.Stream" />.
        /// </summary>
        /// <param name="message"><see cref="System.IO.Stream" /> that contains the message content</param>
        /// <param name="types">A <see cref="MimeTopLevelMediaType" /> value that specifies the allowed Mime-Types to being decoded.</param>
        /// <param name="html"><b>true</b> to allow HTML content; <b>false</b> to ignore the html content.</param>
        /// <param name="path">A <see cref="System.String" /> specifying the path on which to save the attachments found.</param>
        /// <param name="preferredtextsubtype">A <see cref="System.String" /> specifying the subtype to select for text parts that contain aternative content (plain, html, ...). Specify the <b>null</b> reference to maintain the default behavior (prefer whatever the originator sent as the preferred part). If there is not a text part with this subtype, the default one is used.</param>
        public SharpMessage(System.IO.Stream message, MimeTopLevelMediaType types, bool html, System.String path, System.String preferredtextsubtype)
            : this(message, types, ((html) ? DecodeOptions.Default : DecodeOptions.AllowAttachments), path, preferredtextsubtype)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SharpMessage" /> class based on the supplied <see cref="System.IO.Stream" />.
        /// </summary>
        /// <param name="message"><see cref="System.IO.Stream" /> that contains the message content</param>
        /// <param name="options"><see cref="DecodeOptions" /> to determine how to do the decoding (must be combined as a bit map).</param>
        public SharpMessage(System.IO.Stream message, DecodeOptions options)
            : this(message, options, null, null)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SharpMessage" /> class based on the supplied <see cref="System.IO.Stream" />.
        /// </summary>
        /// <param name="message"><see cref="System.IO.Stream" /> that contains the message content</param>
        /// <param name="options"><see cref="DecodeOptions" /> to determine how to do the decoding (must be combined as a bit map).</param>
        /// <param name="path">A <see cref="System.String" /> specifying the path on which to save the attachments found.</param>
        public SharpMessage(System.IO.Stream message, DecodeOptions options, System.String path)
            : this(message, options, path, null)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SharpMessage" /> class based on the supplied <see cref="System.IO.Stream" />.
        /// </summary>
        /// <param name="message"><see cref="System.IO.Stream" /> that contains the message content</param>
        /// <param name="options"><see cref="DecodeOptions" /> to determine how to do the decoding (must be combined as a bit map).</param>
        /// <param name="path">A <see cref="System.String" /> specifying the path on which to save the attachments found.</param>
        /// <param name="preferredtextsubtype">A <see cref="System.String" /> specifying the subtype to select for text parts that contain aternative content (plain, html, ...). Specify the <b>null</b> reference to maintain the default behavior (prefer whatever the originator sent as the preferred part). If there is not a text part with this subtype, the default one is used.</param>
        public SharpMessage(System.IO.Stream message, DecodeOptions options, System.String path, System.String preferredtextsubtype)
        {
            MimeTopLevelMediaType types = MimeTopLevelMediaType.text | MimeTopLevelMediaType.multipart | MimeTopLevelMediaType.message;
            if ((options & DecodeOptions.AllowAttachments) == DecodeOptions.AllowAttachments)
            {
                types = types | MimeTopLevelMediaType.application | MimeTopLevelMediaType.audio | MimeTopLevelMediaType.image | MimeTopLevelMediaType.video;
            }
            if (path != null && (options & DecodeOptions.CreateFolder) != DecodeOptions.CreateFolder && !System.IO.Directory.Exists(path))
            {
                path = null;
            }
            this.ParseMessage(message, types, options, preferredtextsubtype, path);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SharpMessage" /> class based on the supplied <see cref="System.IO.Stream" />.
        /// </summary>
        /// <param name="message"><see cref="System.IO.Stream" /> that contains the message content</param>
        /// <param name="types">A <see cref="MimeTopLevelMediaType" /> value that specifies the allowed Mime-Types to being decoded.</param>
        /// <param name="options"><see cref="DecodeOptions" /> to determine how to do the decoding (must be combined as a bit map).</param>
        /// <param name="path">A <see cref="System.String" /> specifying the path on which to save the attachments found.</param>
        /// <param name="preferredtextsubtype">A <see cref="System.String" /> specifying the subtype to select for text parts that contain aternative content (plain, html, ...). Specify the <b>null</b> reference to maintain the default behavior (prefer whatever the originator sent as the preferred part). If there is not a text part with this subtype, the default one is used.</param>
        public SharpMessage(System.IO.Stream message, MimeTopLevelMediaType types, DecodeOptions options, System.String path, System.String preferredtextsubtype)
        {
            if (path != null && (options & DecodeOptions.CreateFolder) != DecodeOptions.CreateFolder && !System.IO.Directory.Exists(path))
            {
                path = null;
            }
            this.ParseMessage(message, types, options, preferredtextsubtype, path);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SharpMessage" /> class based on the supplied <see cref="System.String" />.
        /// </summary>
        /// <param name="message"><see cref="System.String" /> with the message content</param>
        public SharpMessage(System.String message)
            : this(new System.IO.MemoryStream(System.Text.Encoding.ASCII.GetBytes(message)))
        {
        }

        /// <summary>
        /// <see cref="System.Collections.ICollection" /> that contains the attachments found in this message.
        /// </summary>
        /// <remarks>Each attachment is a <see cref="SharpAttachment" /> instance.</remarks>
        public System.Collections.ICollection Attachments
        {
            get { return this._attachments; }
        }
        /// <summary>
        /// Text body
        /// </summary>
        /// <remarks>If there are more than one text part in the message, they are concatenated.</remarks>
        public System.String Body
        {
            get { return this._body; }
        }
        /// <summary>
        /// Collection of <see cref="MimeAddress" /> instances found in the <b>Cc</b> header field.
        /// </summary>
        public System.Collections.IEnumerable Cc
        {
            get { return MimeAddressCollection.Parse(this._headers.Cc); }
        }
        /// <summary>
        /// Date
        /// </summary>
        /// <remarks>If there is not a <b>Date</b> field present in the headers (or it has an invalid format) then
        /// the date is extrated from the last <b>Received</b> field. If neither of them are found,
        /// <b>System.Date.MinValue</b> is returned.</remarks>
        public System.DateTime Date
        {
            get { return this._date; }
        }
        /// <summary>
        /// From's name
        /// </summary>
        public System.String From
        {
            get { return this._from_name; }
        }
        /// <summary>
        /// From's e-mail address
        /// </summary>
        public System.String FromAddress
        {
            get { return this._from_addr; }
        }
        /// <summary>
        /// Gets a value indicating whether the body contains html content
        /// </summary>
        /// <value><b>true</b> if the body contains html content; otherwise, <b>false</b></value>
        public bool HasHtmlBody
        {
            get { return this._bodyHtml; }
        }
        /// <summary>
        /// <see cref="MimeHeader" /> instance for this message that contains the raw content of the headers.
        /// </summary>
        public MimeHeader Headers
        {
            get { return this._headers; }
        }
        /// <summary>
        /// <b>Message-ID</b> header
        /// </summary>
        public System.String MessageID
        {
            get { return this._headers.MessageID; }
        }
        /// <summary>
        /// <b>Subject</b> field
        /// </summary>
        /// <remarks>The field body is automatically RFC 2047 decoded if it's necessary</remarks>
        public System.String Subject
        {
            get { return this._subject; }
        }
        /// <summary>
        /// Collection of <see cref="MimeAddress" /> found in the <b>To</b> header field.
        /// </summary>
        public System.Collections.IEnumerable To
        {
            get { return this._to; }
        }
        /// <summary>
        /// Returns the requested header field body.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <remarks>The value if present is uncommented and decoded (RFC 2047).<br />
        /// If the requested field is not present in this instance, <see cref="System.String.Empty" /> is returned instead.</remarks>
        public System.String GetHeaderField(System.String name)
        {
            if (this._headers == null)
                return System.String.Empty;
            return this._headers.GetHeaderField(name, System.String.Empty, true, true);
        }

        private void ParseMessage(System.IO.Stream stream, MimeTopLevelMediaType types, DecodeOptions options, System.String preferredtextsubtype, System.String path)
        {
            this._attachments = new System.Collections.ArrayList();
            MimeMessage message = new MimeMessage(stream);
            this.ParseMessage(message, types, (options & DecodeOptions.AllowHtml) == DecodeOptions.AllowHtml, options, preferredtextsubtype, path);
            this._headers = message.Header;
            message.Close();
            // find and decode uuencoded content if configured to do so (and attachments a allowed)
            if ((options & DecodeOptions.UuDecode) == DecodeOptions.UuDecode
                   && (options & DecodeOptions.AllowAttachments) == DecodeOptions.AllowAttachments)
                this.UuDecode(path);
            // Date
            this._date = MimeTools.parseDate(this._headers.Date);
            if (this._date.Equals(System.DateTime.MinValue))
            {
                System.String date = this._headers["Received"] ?? System.String.Empty;
                if (date.IndexOf("\r\n", System.StringComparison.Ordinal) > 0)
                    date = date.Substring(0, date.IndexOf("\r\n", System.StringComparison.Ordinal));
                date = date.LastIndexOf(';') > 0 ? date.Substring(date.LastIndexOf(';') + 1).Trim() : System.String.Empty;
                this._date = MimeTools.parseDate(date);
            }
            // Subject
            this._subject = MimeTools.parserfc2047Header(this._headers.Subject);
            // To
            this._to = MimeAddressCollection.Parse(this._headers.To);
            // From
            MimeAddressCollection from = MimeAddressCollection.Parse(this._headers.From);
            foreach (MimeAddress item in from)
            {
                this._from_name = item["name"];
                this._from_addr = item["address"];
                if (this._from_name == null || this._from_name.Equals(System.String.Empty))
                    this._from_name = item["address"];
            }
        }
        private void ParseMessage(MimeMessage part, MimeTopLevelMediaType types, bool html, DecodeOptions options, System.String preferredtextsubtype, System.String path)
        {
            if ((types & part.Header.TopLevelMediaType) != part.Header.TopLevelMediaType)
            {
                return;
            }
            switch (part.Header.TopLevelMediaType)
            {
                case MimeTopLevelMediaType.multipart:
                case MimeTopLevelMediaType.message:
                    // TODO: allow other subtypes of "message"
                    // Only message/rfc822 allowed, other subtypes ignored
                    if (part.Header.TopLevelMediaType.Equals(MimeTopLevelMediaType.message)
                         && !part.Header.SubType.Equals("rfc822"))
                        break;
                    if (part.Header.SubType.Equals("alternative"))
                    {
                        if (part.PartsCount > 0)
                        {
                            MimeMessage altenative = null;
                            // Get the first mime part of the alternatives that has a accepted Mime-Type
                            for (int i = part.PartsCount; i > 0; i--)
                            {
                                MimeMessage item = part.GetPart(i - 1);
                                if ((types & part.Header.TopLevelMediaType) != part.Header.TopLevelMediaType
                                    || (!html && item.Header.TopLevelMediaType.Equals(MimeTopLevelMediaType.text) && item.Header.SubType.Equals("html"))
                                   )
                                {
                                    continue;
                                }
                                // First allowed one.
                                if (altenative == null)
                                {
                                    altenative = item;
                                    // We don't have to select body part based on subtype if not asked for, or not a text one
                                    // or it's already the preferred one
                                    if (preferredtextsubtype == null || item.Header.TopLevelMediaType != MimeTopLevelMediaType.text || (item.Header.SubType == preferredtextsubtype))
                                    {
                                        break;
                                    }
                                    // This one is preferred over the last part
                                }
                                else if (item.Header.TopLevelMediaType == MimeTopLevelMediaType.text && item.Header.SubType == preferredtextsubtype)
                                {
                                    altenative = item;
                                    break;
                                }
                            }
                            if (altenative != null)
                            {
                                // If message body as html is allowed and part has a Content-ID field
                                // add an anchor to mark this body part
                                if (html && part.Header.Contains("Content-ID") && (options & DecodeOptions.NamedAnchors) == DecodeOptions.NamedAnchors)
                                {
                                    // There is a previous text body, so enclose it in <pre>
                                    if (!this._bodyHtml && this._body.Length > 0)
                                    {
                                        this._body = System.String.Concat("<pre>", System.Web.HttpUtility.HtmlEncode(this._body), "</pre>");
                                        this._bodyHtml = true;
                                    }
                                    // Add the anchor
                                    this._body = System.String.Concat(this._body, "<a name=\"", MimeTools.Rfc2392Url(this.MessageID), "_", MimeTools.Rfc2392Url(part.Header.ContentID), "\"></a>");
                                }
                                this.ParseMessage(altenative, types, html, options, preferredtextsubtype, path);
                            }
                        }
                        // TODO: Take into account each subtype of "multipart"
                    }
                    else if (part.PartsCount > 0)
                    {
                        foreach (MimeMessage item in part)
                        {
                            this.ParseMessage(item, types, html, options, preferredtextsubtype, path);
                        }
                    }
                    break;
                case MimeTopLevelMediaType.text:
                    if ((part.Disposition == null || !part.Disposition.Equals("attachment"))
                        && (part.Header.SubType.Equals("plain") || part.Header.SubType.Equals("html")))
                    {
                        bool body_was_html = this._bodyHtml;
                        // HTML content not allowed
                        if (part.Header.SubType.Equals("html"))
                        {
                            if (!html)
                                break;
                            this._bodyHtml = true;
                        }
                        if (html && part.Header.Contains("Content-ID") && (options & DecodeOptions.NamedAnchors) == DecodeOptions.NamedAnchors)
                        {
                            this._bodyHtml = true;
                        }
                        if (this._bodyHtml && !body_was_html && this._body.Length > 0)
                        {
                            this._body = System.String.Concat("<pre>", System.Web.HttpUtility.HtmlEncode(this._body), "</pre>");
                        }
                        // If message body is html and this part has a Content-ID field
                        // add an anchor to mark this body part
                        if (this._bodyHtml && part.Header.Contains("Content-ID") && (options & DecodeOptions.NamedAnchors) == DecodeOptions.NamedAnchors)
                        {
                            this._body = System.String.Concat(this._body, "<a name=\"", MimeTools.Rfc2392Url(this.MessageID), "_", MimeTools.Rfc2392Url(part.Header.ContentID), "\"></a>");
                        }
                        if (this._bodyHtml && part.Header.SubType.Equals("plain"))
                        {
                            this._body = System.String.Concat(this._body, "<pre>", System.Web.HttpUtility.HtmlEncode(part.BodyDecoded), "</pre>");
                        }
                        else
                            this._body = System.String.Concat(this._body, part.BodyDecoded);
                    }
                    else
                    {
                        if ((types & MimeTopLevelMediaType.application) != MimeTopLevelMediaType.application)
                        {
                            return;
                        }
                        goto case MimeTopLevelMediaType.application;
                    }
                    break;
                case MimeTopLevelMediaType.application:
                case MimeTopLevelMediaType.audio:
                case MimeTopLevelMediaType.image:
                case MimeTopLevelMediaType.video:
                    // Attachments not allowed.
                    if ((options & DecodeOptions.AllowAttachments) != DecodeOptions.AllowAttachments)
                        break;
                    SharpAttachment attachment = null;
                    // Save to a file
                    if (path != null)
                    {
                        System.IO.FileInfo file = part.DumpBody(path, true);
                        if (file != null)
                        {
                            attachment = new SharpAttachment(file);
                            attachment.Name = file.Name;
                            attachment.Size = file.Length;
                        }
                        // Save to a stream
                    }
                    else
                    {
                        System.IO.MemoryStream stream = new System.IO.MemoryStream();
                        part.DumpBody(stream);
                        attachment = new SharpAttachment(stream);
                        attachment.Name = part.Name ?? System.String.Concat("generated_", part.GetHashCode(), ".", part.Header.SubType);
                        attachment.Size = stream.Length;
                    }
                    if (attachment != null && part.Header.SubType == "ms-tnef" && (options & DecodeOptions.DecodeTnef) == DecodeOptions.DecodeTnef)
                    {
                        // Try getting attachments form a tnef stream

                        System.IO.Stream stream = attachment.Stream;
                        if (stream != null && stream.CanSeek)
                            stream.Seek(0, System.IO.SeekOrigin.Begin);
                        TnefMessage tnef = new TnefMessage(stream);
                        if (tnef.Parse(path))
                        {
                            if (tnef.Attachments != null)
                            {
                                this._attachments.AddRange(tnef.Attachments);
                            }
                            attachment.Close();
                            // Delete the raw tnef file
                            if (attachment.SavedFile != null)
                            {
                                if (stream != null && stream.CanRead)
                                    stream.Close();
                                attachment.SavedFile.Delete();
                            }
                            attachment = null;
                            tnef.Close();

                        }
                        else
                        {
                            // The read-only stream is no longer needed and locks the file
                            if (attachment.SavedFile != null && stream != null && stream.CanRead)
                                stream.Close();
                        }
                    }
                    if (attachment != null)
                    {
                        if (part.Disposition != null && part.Disposition == "inline")
                        {
                            attachment.Inline = true;
                        }
                        attachment.MimeTopLevelMediaType = part.Header.TopLevelMediaType;
                        attachment.MimeMediaSubType = part.Header.SubType;
                        // Store attachment's CreationTime
                        if (part.Header.ContentDispositionParameters.ContainsKey("creation-date"))
                            attachment.CreationTime = MimeTools.parseDate(part.Header.ContentDispositionParameters["creation-date"]);
                        // Store attachment's LastWriteTime
                        if (part.Header.ContentDispositionParameters.ContainsKey("modification-date"))
                            attachment.LastWriteTime = MimeTools.parseDate(part.Header.ContentDispositionParameters["modification-date"]);
                        if (part.Header.Contains("Content-ID"))
                            attachment.ContentID = part.Header.ContentID;
                        this._attachments.Add(attachment);
                    }
                    break;
            }
        }
        private System.String ReplaceUrlTokens(System.String url, SharpAttachment attachment)
        {
            if (string.IsNullOrEmpty(url) || url.IndexOf('[') == -1 || url.IndexOf(']') == -1)
                return url;
            if (url.IndexOf("[MessageID]", System.StringComparison.Ordinal) != -1)
            {
                url = url.Replace("[MessageID]", System.Web.HttpUtility.UrlEncode(MimeTools.Rfc2392Url(this.MessageID)));
            }
            if (attachment != null && attachment.ContentID != null)
            {
                if (url.IndexOf("[ContentID]", System.StringComparison.Ordinal) != -1)
                {
                    url = url.Replace("[ContentID]", System.Web.HttpUtility.UrlEncode(MimeTools.Rfc2392Url(attachment.ContentID)));
                }
                if (url.IndexOf("[Name]", System.StringComparison.Ordinal) != -1)
                {
                    url = url.Replace("[Name]", attachment.SavedFile != null ? System.Web.HttpUtility.UrlEncode(attachment.SavedFile.Name) : System.Web.HttpUtility.UrlEncode(attachment.Name));
                }
            }
            return url;
        }
        /// <summary>
        /// Set the URL used to reference embedded parts from the HTML body (as specified on RFC2392)
        /// </summary>
        /// <param name="attachmentsurl">URL used to reference embedded parts from the HTML body.</param>
        /// <remarks>The supplied URL will be replaced with the following tokens for each attachment:<br />
        /// <ul>
        ///  <li><b>[MessageID]</b>: Will be replaced with the <see cref="MessageID" /> of the current instance.</li>
        ///  <li><b>[ContentID]</b>: Will be replaced with the <see cref="SharpAttachment.ContentID" /> of the attachment.</li>
        ///  <li><b>[Name]</b>: Will be replaced with the <see cref="SharpAttachment.Name" /> of the attachment (or with <see cref="System.IO.FileInfo.Name" /> from <see cref="SharpAttachment.SavedFile" /> if the instance has been already saved to disk).</li>
        /// </ul>
        ///</remarks>
        public void SetUrlBase(System.String attachmentsurl)
        {
            // Not a html boy or not body at all
            if (!this._bodyHtml || this._body.Length == 0)
                return;
            // No references found, so nothing to do
            if (this._body.IndexOf("cid:", System.StringComparison.Ordinal) == -1 && this._body.IndexOf("mid:", System.StringComparison.Ordinal) == -1)
                return;
            System.String msgid = MimeTools.Rfc2392Url(this.MessageID);
            // There is a base url and attachments, so try refererencing them properly
            if (attachmentsurl != null && this._attachments != null && this._attachments.Count > 0)
            {
                for (int i = 0, count = this._attachments.Count; i < count; i++)
                {
                    SharpAttachment attachment = (SharpAttachment)this._attachments[i];
                    if (attachment.ContentID != null)
                    {
                        System.String conid = MimeTools.Rfc2392Url(attachment.ContentID);
                        if (conid.Length > 0)
                        {
                            if (this._body.IndexOf("cid:" + conid, System.StringComparison.Ordinal) != -1)
                                this._body = this._body.Replace("cid:" + conid, this.ReplaceUrlTokens(attachmentsurl, attachment));
                            if (msgid.Length > 0 && this._body.IndexOf("mid:" + msgid + "/" + conid, System.StringComparison.Ordinal) != -1)
                                this._body = this._body.Replace("mid:" + msgid + "/" + conid, this.ReplaceUrlTokens(attachmentsurl, attachment));
                            // No more references found, so nothing to do
                            if (this._body.IndexOf("cid:", System.StringComparison.Ordinal) == -1 && this._body.IndexOf("mid:", System.StringComparison.Ordinal) == -1)
                                break;
                        }
                    }
                }
            }
            // The rest references must be to text parts
            // so rewrite them to refer to the named anchors added by ParseMessage
            if (this._body.IndexOf("cid:", System.StringComparison.Ordinal) != -1)
            {
                this._body = this._body.Replace("cid:", "#" + msgid + "_");
            }
            if (msgid.Length > 0 && this._body.IndexOf("mid:", System.StringComparison.Ordinal) != -1)
            {
                this._body = this._body.Replace("mid:" + msgid + "/", "#" + msgid + "_");
                this._body = this._body.Replace("mid:" + msgid, "#" + msgid);
            }
        }
        private void UuDecode(System.String path)
        {
            if (this._body.Length == 0 || this._body.IndexOf("begin ", System.StringComparison.Ordinal) == -1 || this._body.IndexOf("end", System.StringComparison.Ordinal) == -1)
                return;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            System.IO.StringReader reader = new System.IO.StringReader(this._body);
            System.IO.Stream stream = null;
            SharpAttachment attachment = null;
            for (System.String line = reader.ReadLine(); line != null; line = reader.ReadLine())
            {
                if (stream == null)
                {
                    // Found the start point of uuencoded content
                    if (line.Length > 10 && line[0] == 'b' && line[1] == 'e' && line[2] == 'g' && line[3] == 'i' && line[4] == 'n' && line[5] == ' ' && line[9] == ' ')
                    {
                        System.String name = System.IO.Path.GetFileName(line.Substring(10));

                        System.Console.WriteLine(System.String.Concat("uuencoded content found. name[", name, "]"));

                        // In-Memory decoding
                        if (path == null)
                        {
                            attachment = new SharpAttachment(new System.IO.MemoryStream());
                            stream = attachment.Stream;
                            // Filesystem decoding
                        }
                        else
                        {
                            attachment = new SharpAttachment(new System.IO.FileInfo(System.IO.Path.Combine(path, name)));
                            stream = attachment.SavedFile.OpenWrite();
                        }
                        attachment.Name = name;
                        // Not uuencoded line, so add it to new body
                    }
                    else
                    {
                        sb.Append(line);
                        sb.Append(System.Environment.NewLine);
                    }
                }
                else
                {
                    // Content finished
                    if (line.Length == 3 && line == "end")
                    {
                        stream.Flush();
                        if (stream.Length > 0)
                        {
                            System.Console.WriteLine(System.String.Concat("uuencoded content finished. name[", attachment.Name, "] size[", stream.Length, "]"));
                            attachment.Size = stream.Length;
                            this._attachments.Add(attachment);
                        }
                        // When decoding to a file, close the stream.
                        if (attachment.SavedFile != null || stream.Length == 0)
                        {
                            stream.Close();
                        }
                        attachment = null;
                        stream = null;
                        // Decode and write uuencoded line
                    }
                    else
                    {
                        MimeTools.UuDecodeLine(line, stream);
                    }
                }
            }
            if (stream != null && stream.CanRead)
            {
                stream.Close();
            }
            reader.Close();
            this._body = sb.ToString();
        }
    }
    /// <summary>
    /// This class provides the basic functionality for handling attachments
    /// </summary>
    public class SharpAttachment
    {
        private System.DateTime _ctime = System.DateTime.MinValue;
        private System.DateTime _mtime = System.DateTime.MinValue;
        private System.String _name;
        private System.IO.MemoryStream _stream;
        private System.IO.FileInfo _saved_file;

        /// <summary>
        /// Initializes a new instance of the <see cref="SharpAttachment" /> class based on the supplied <see cref="System.IO.MemoryStream" />.
        /// </summary>
        /// <param name="stream"><see cref="System.IO.MemoryStream" /> instance that contains the attachment content.</param>
        public SharpAttachment(System.IO.MemoryStream stream)
        {
            this._stream = stream;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SharpAttachment" /> class based on the supplied <see cref="System.IO.FileInfo" />.
        /// </summary>
        /// <param name="file"><see cref="System.IO.MemoryStream" /> instance that contains the info about the attachment.</param>
        public SharpAttachment(System.IO.FileInfo file)
        {
            this._saved_file = file;
            this._ctime = file.CreationTime;
            this._mtime = file.LastWriteTime;
        }
        /// <summary>
        /// Closes the underling stream if it's open.
        /// </summary>
        public void Close()
        {
            if (this._stream != null && this._stream.CanRead)
                this._stream.Close();
            this._stream = null;
        }
        /// <summary>
        /// Saves of the attachment to a file in the given path.
        /// </summary>
        /// <param name="path">A <see cref="System.String" /> specifying the path on which to save the attachment.</param>
        /// <param name="overwrite"><b>true</b> if the destination file can be overwritten; otherwise, <b>false</b>.</param>
        /// <returns><see cref="System.IO.FileInfo" /> of the saved file. <b>null</b> when it fails to save.</returns>
        /// <remarks>If the file was already saved, the previous <see cref="System.IO.FileInfo" /> is returned.<br />
        /// Once the file is successfully saved, the stream is closed and <see cref="SavedFile" /> property is updated.</remarks>
        public System.IO.FileInfo Save(System.String path, bool overwrite)
        {
            if (path == null || this._name == null)
                return null;
            if (this._stream == null)
            {
                if (this._saved_file != null)
                    return _saved_file;
                return null;
            }
            if (!this._stream.CanRead)
            {

                return null;
            }
            System.IO.FileInfo file = new System.IO.FileInfo(System.IO.Path.Combine(path, this._name));
            if (file.Directory != null && !file.Directory.Exists)
            {
                throw new DirectoryNotFoundException(System.String.Concat("Destination folder [", file.Directory.FullName, "] does not exist"));
            }
            if (file.Exists)
            {
                if (overwrite)
                {
                    try
                    {
                        file.Delete();

                    }
                    catch (System.Exception)
                    {

                        return null;
                    }
                }
                else
                {
                    // Though the file already exists, we set the times
                    if (this._mtime != System.DateTime.MinValue && file.LastWriteTime != this._mtime)
                        file.LastWriteTime = this._mtime;
                    if (this._ctime != System.DateTime.MinValue && file.CreationTime != this._ctime)
                        file.CreationTime = this._ctime;
                    return null;
                }
            }
            try
            {
                System.IO.FileStream stream = file.OpenWrite();
                this._stream.WriteTo(stream);
                stream.Flush();
                stream.Close();
                this.Close();
                if (this._mtime != System.DateTime.MinValue)
                    file.LastWriteTime = this._mtime;
                if (this._ctime != System.DateTime.MinValue)
                    file.CreationTime = this._ctime;
                this._saved_file = file;

            }
            catch (System.Exception)
            {

                return null;
            }
            return file;
        }

        private string _contentId;

        /// <summary>
        /// Gets or sets the Content-ID of this attachment.
        /// </summary>
        /// <value>Content-ID header of this instance. Or the <b>null</b> reference.</value>
        public string ContentID
        {
            get { return _contentId; }
            set { _contentId = value; }
        }

        /// <summary>
        /// Gets or sets the time when the file associated with this attachment was created.
        /// </summary>
        /// <value>The time this attachment was last written to.</value>
        public System.DateTime CreationTime
        {
            get { return this._ctime; }
            set { this._ctime = value; }
        }

        private bool _inline;

        /// <summary>
        /// Gets or sets value indicating whether the current instance is an inline attachment.
        /// </summary>
        /// <value><b>true</b> is it's an inline attachment; <b>false</b> otherwise.</value>
        public bool Inline
        {
            get { return _inline; }
            set { _inline = value; }
        }

        /// <summary>
        /// Gets or sets the time when the file associated with this attachment was last written to.
        /// </summary>
        /// <value>The time this attachment was last written to.</value>
        public System.DateTime LastWriteTime
        {
            get { return this._mtime; }
            set { this._mtime = value; }
        }
        /// <summary>
        /// Gets or sets the name of the attachment.
        /// </summary>
        /// <value>The name of the attachment.</value>
        public System.String Name
        {
            get { return this._name; }
            set
            {
                System.String name = MimeTools.GetFileName(value);
                if (value != null && name == null && this._name != null && System.IO.Path.HasExtension(value))
                {
                    name = System.IO.Path.ChangeExtension(this._name, System.IO.Path.GetExtension(value));
                }
                this._name = name;
            }
        }

        private string _mimeMediaSubType;

        /// <summary>
        /// Gets or sets top-level media type for this <see cref="SharpAttachment" /> instance
        /// </summary>
        /// <value>Top-level media type from Content-Type header field of this <see cref="SharpAttachment" /> instance</value>
        public string MimeMediaSubType
        {
            get { return _mimeMediaSubType; }
            set { _mimeMediaSubType = value; }
        }

        private MimeTopLevelMediaType _mimeTopLevelMediaType;

        /// <summary>
        /// Gets or sets SubType for this <see cref="SharpAttachment" /> instance
        /// </summary>
        /// <value>SubType from Content-Type header field of this <see cref="SharpAttachment" /> instance</value>
        public MimeTopLevelMediaType MimeTopLevelMediaType
        {
            get { return _mimeTopLevelMediaType; }
            set { _mimeTopLevelMediaType = value; }
        }

        private long _size;

        /// <summary>
        /// Gets or sets size (in bytes) for this <see cref="SharpAttachment" /> instance.
        /// </summary>
        /// <value>Size of this <see cref="SharpAttachment" /> instance</value>
        public long Size
        {
            get { return _size; }
            set { _size = value; }
        }

        /// <summary>
        /// Gets the <see cref="System.IO.FileInfo" /> of the saved file.
        /// </summary>
        /// <value>The <see cref="System.IO.FileInfo" /> of the saved file.</value>
        public System.IO.FileInfo SavedFile
        {
            get { return this._saved_file; }
        }
        /// <summary>
        /// Gets the <see cref="System.IO.Stream " /> of the attachment.
        /// </summary>
        /// <value>The <see cref="System.IO.Stream " /> of the attachment.</value>
        /// <remarks>If the underling stream exists, it's returned. If the file has been saved, it opens <see cref="SavedFile" /> for reading.</remarks>
        public System.IO.Stream Stream
        {
            get
            {
                if (this._stream != null)
                    return this._stream;
                if (this._saved_file != null)
                    return this._saved_file.OpenRead();
                return null;
            }
        }
    }
}
