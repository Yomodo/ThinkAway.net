using System;
using ThinkAway.Net.Mail.POP3;

namespace ThinkAway.Net.Mail.Exceptions
{
	/// <summary>
	/// This is the base exception for all <see cref="PopClient"/> exceptions.
	/// </summary>
    internal abstract class PopClientException : Exception
	{
		///<summary>
		/// Creates a PopClientException with the given message and InnerException
		///</summary>
		///<param name="message">The message to include in the exception</param>
		///<param name="innerException">The exception that is the cause of this exception</param>
		protected PopClientException(string message, Exception innerException)
			: base(message, innerException)
		{
			if(message == null)
				throw new ArgumentNullException("message");

			if(innerException == null)
				throw new ArgumentNullException("innerException");
		}

		///<summary>
		/// Creates a PopClientException with the given message
		///</summary>
		///<param name="message">The message to include in the exception</param>
		protected PopClientException(string message)
			: base(message)
		{
			if (message == null)
				throw new ArgumentNullException("message");
		}
	}
}