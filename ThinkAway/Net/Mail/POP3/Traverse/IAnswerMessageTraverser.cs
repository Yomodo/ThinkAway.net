﻿namespace ThinkAway.Net.Mail.POP3.Traverse
{
	/// <summary>
	/// This interface describes a MessageTraverser which is able to traverse a Message hierarchy structure
	/// and deliver some answer.
	/// </summary>
	/// <typeparam name="TAnswer">This is the type of the answer you want to have delivered.</typeparam>
	internal interface IAnswerMessageTraverser<TAnswer>
	{
		/// <summary>
		/// Call this when you want to apply this traverser on a <see cref="MailMessage"/>.
		/// </summary>
		/// <param name="message">The <see cref="MailMessage"/> which you want to traverse. Must not be <see langword="null"/>.</param>
		/// <returns>An answer</returns>
		TAnswer VisitMessage(MailMessage message);

		/// <summary>
		/// Call this when you want to apply this traverser on a <see cref="MessagePart"/>.
		/// </summary>
		/// <param name="messagePart">The <see cref="MessagePart"/> which you want to traverse. Must not be <see langword="null"/>.</param>
		/// <returns>An answer</returns>
		TAnswer VisitMessagePart(MessagePart messagePart);
	}
}