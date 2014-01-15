using System;

namespace ThinkAway.Net.Mail.Exceptions
{
	/// <summary>
	/// Thrown when the supplied username or password is not accepted by the POP3 server.
	/// </summary>
	internal class InvalidLoginException : PopClientException
	{
		///<summary>
		/// Creates a InvalidLoginException with the given message and InnerException
		///</summary>
		///<param name="innerException">The exception that is the cause of this exception</param>
		public InvalidLoginException(Exception innerException)
			: base("Server did not accept user credentials", innerException)
		{ }

        public InvalidLoginException(string message, PopServerException innerException)
            : base(message, innerException)
        {

        }
	}
}