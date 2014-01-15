/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.Net.Mail.SMTP {

    /// <summary>
	/// This Type is used to store known SMTP responses specified by RFC 821 and 2821
	/// </summary>
	internal struct ReplyConstants
	{
		public const  string SYSTEM_STATUS					= "211";
		public const  string HELP_MSG						= "214";
		public const  string HELO_REPLY 					= "220";
		public const  string QUIT 							= "221";
		public const  string AUTH_SUCCESSFUL 				= "235";
		public const  string OK 							= "250";
		public const  string NOT_LOCAL_WILL_FORWARD			= "251";
		public const  string SERVER_CHALLENGE				= "334";
		public const  string START_INPUT 					= "354";
		public const  string SERVICE_NOT_AVAILABLE			= "421";
		public const  string MAILBOX_BUSY 					= "450";
		public const  string ERROR_PROCESSING				= "451";
		public const  string INSUFFICIENT_STORAGE			= "452";
		public const  string UNKNOWN 						= "500";
		public const  string SYNTAX_ERROR					= "501";
		public const  string CMD_NOT_IMPLEMENTED			= "502";
		public const  string BAD_SEQUENCE					= "503";
		public const  string NOT_IMPLEMENTED				= "504";
		public const  string SECURITY_ERROR 				= "505";
		public const  string ACTION_NOT_TAKEN 				= "550";
		public const  string NOT_LOCAL_PLEASE_FORWARD 		= "551";
		public const  string EXCEEDED_STORAGE_ALLOWANCE		= "552";
		public const  string MAILBOX_NAME_NOT_ALLOWED		= "553";
		public const  string TRANSACTION_FAILED				= "554";

		public const  string PIPELINING						= "PIPELINING";

	}
}