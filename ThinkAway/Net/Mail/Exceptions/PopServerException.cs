namespace ThinkAway.Net.Mail.Exceptions
{
	/// <summary>
	/// Thrown when the server does not return "+" to a command.<br/>
	/// The server response is then placed inside.
	/// </summary>
    internal class PopServerException : PopClientException
	{
		///<summary>
		/// Creates a PopServerException with the given message
		///</summary>
		///<param name="message">The message to include in the exception</param>
		public PopServerException(string message)
			: base(message)
		{ }
	}
}