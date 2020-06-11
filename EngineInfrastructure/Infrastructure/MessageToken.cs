namespace JustDecompile.EngineInfrastructure
{
	public class MessageToken : Token
	{
		public string Message { get; set; }

		public MessageToken() : base() { }

		public MessageToken(string text, TokenKind kind, string message)
			: base(text, kind)
		{
			this.Message = message;
		}

		public MessageToken(string text, TokenKind kind, bool isUsage, string message)
			: base(text, kind, isUsage)
		{
			this.Message = message;
		}
	}
}
