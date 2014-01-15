/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.Net.Mail.SMTP {

    /// <summary>
	/// This Type stores different priority values used for a MailMessage
	/// <seealso cref="MailMessage"/>
	/// </summary>
	/// <example>
	/// <code>
	///	MailAddress from = new MailAddress("Lsong","song940@163.com");
	///	MailAddress to	 = new MailAddress("admin@lsong.org");
	///	MailMessage mailMessage = new MailMessage(from,to);
	///	mailMessage.Priority = MailPriority.High;
	/// </code>
	/// </example>
    public struct MailPriority
	{

		public const string Highest = 	"1";
		public const string High	= 	"2";
		public const string Normal	= 	"3";
		public const string Low   	=	"4";
		public const string Lowest	=	"5";
	
	
	}
}
	
