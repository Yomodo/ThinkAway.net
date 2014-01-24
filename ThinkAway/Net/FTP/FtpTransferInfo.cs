﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ThinkAway.Net.FTP {
	/// <summary>
	/// Provides upload / download progress information
	/// </summary>
	public class FtpTransferInfo : EventArgs {
		FtpTransferType _xferType;
		/// <summary>
		/// Indicates if the transfer is an upload or download
		/// </summary>
		public FtpTransferType TransferType {
			get { return _xferType; }
			private set { _xferType = value; }
		}

		string _file = null;
		/// <summary>
		/// The full path to remote file
		/// </summary>
		public string FileName {
			get { return _file; }
			private set { _file = value; }
		}

		long _length = 0;
		/// <summary>
		/// The total number of bytes to be transferred
		/// </summary>
		public long Length {
			get { return _length; }
			private set { _length = value; }
		}

		long _resume = 0;
		/// <summary>
		/// Gets the location the download was resumed at
		/// </summary>
		public long Resume {
			get { return _resume; }
			private set { _resume = value; }
		}

		long _transferred = 0;
		/// <summary>
		/// The number of bytes transferred
		/// </summary>
		public long Transferred {
			get { return _transferred; }
			private set { _transferred = value; }
		}

		/// <summary>
		/// Percentage of the transfer that has been completed
		/// </summary>
		public double Percentage {
			get {
				if (this.Length > 0 && this.Transferred > 0) {
					return Math.Round(((double)this.Transferred / (double)this.Length) * 100, 1);
				}

				return 0;
			}
		}

		DateTime _start = DateTime.MinValue;
		/// <summary>
		/// The start time of the transfer
		/// </summary>
		public DateTime Start {
			get { return _start; }
			private set { _start = value; }
		}

		DateTime _now = DateTime.Now;
		/// <summary>
		/// The current time used for calculating bps
		/// </summary>
		public DateTime Now {
			get { return _now; }
			private set { _now = value; }
		}

		/// <summary>
		/// Transfer average
		/// </summary>
		public long BytesPerSecond {
			get {
				TimeSpan t = this.Now.Subtract(this.Start);

				if ((this.Transferred - this.Resume) > 0 && t.TotalSeconds > 0) {
					return (long)Math.Round((this.Transferred - this.Resume) / t.TotalSeconds, 0);
				}

				return 0;
			}
		}

		bool _complete = false;
		/// <summary>
		/// Gets a value indicating if the transfer is complete
		/// </summary>
		public bool Complete {
			get { return _complete; }
			private set { _complete = value; }
		}

		bool _cancel = false;
		/// <summary>
		/// Cancels the transfer
		/// </summary>
		public bool Cancel {
			get { return _cancel; }
			set { _cancel = value; }
		}

		/// <summary>
		/// Iniatlize the FtpTransferInfo object
		/// </summary>
		/// <param name="type">Upload or download</param>
		/// <param name="file">Remote object path</param>
		/// <param name="length">Size of the object in bytes</param>
		/// <param name="resume">Bytes resumed (if this was a resume)</param>
		/// <param name="transferred">Bytes transfered</param>
		/// <param name="start">The time the transfer started</param>
		/// <param name="complete">Value indicating if the transfer is complete</param>
		public FtpTransferInfo(FtpTransferType type, string file, long length, long resume, long transferred, DateTime start, bool complete) {
			this.TransferType = type;
			this.FileName = file;
			this.Length = length;
			this.Resume = resume;
			this.Transferred = transferred;
			this.Start = start;
			this.Complete = complete;
		}
	}
}
