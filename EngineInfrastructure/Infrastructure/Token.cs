namespace JustDecompile.EngineInfrastructure
{
	public class Token
	{
		private TokenKind kind;

		public string Text { get; set; }

		public bool IsUsage { get; set; }

		public AccessTypes AccessType { get; set; }

		public object TypeReference { get; set; }

		public Token() { }

		public Token(string text, TokenKind kind)
		{
			this.Text = text;
			this.Kind = kind;
		}

		public Token(string text, TokenKind kind, bool isUsage)
			: this(text, kind)
		{
			this.IsUsage = isUsage;
		}

		public TokenKind Kind
		{
			get
			{
				return kind;
			}
			set
			{
				kind = value;

				this.OnTokenKindChanged();
			}
		}

		protected virtual void OnTokenKindChanged() { }

		public T GetResource<T>()
		{
			return (T)this.TypeReference;
		}
	}
}
