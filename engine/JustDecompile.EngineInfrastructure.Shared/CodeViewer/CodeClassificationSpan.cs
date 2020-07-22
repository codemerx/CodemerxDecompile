#if !JUSTASSEMBLY && !ENGINEONLYBUILD
using Telerik.Windows.Documents.Code.Tagging;
using Telerik.Windows.Documents.Code.Text;
#endif

namespace JustDecompile.EngineInfrastructure
{
	public class CodeClassificationSpan : PositionToken, ICodeClassificationSpan
	{
#if !JUSTASSEMBLY && !ENGINEONLYBUILD
		public ClassificationType ClassificationType { get; set; }

		public CodeClassificationSpan(Token token, Span span)
		{
			CopyProperties(token, span);
		}

		private void CopyProperties(Token token, Span span)
		{
			this.AccessType = token.AccessType;

			this.IsUsage = token.IsUsage;

			this.Kind = token.Kind;

			this.Text = token.Text;

			this.TypeReference = token.TypeReference;

			this.Span = span;

			if (token is MessageToken)
			{
				Message = ((MessageToken)token).Message;
			}
		}

		protected override void OnTokenKindChanged()
		{
			switch (this.Kind)
			{
				case TokenKind.WhiteSpace:
					ClassificationType = ClassificationTypes.WhiteSpace;
					break;

				case TokenKind.KeyWord:
					ClassificationType = ClassificationTypes.Keyword;
					break;

				case TokenKind.Literal:
					ClassificationType = ClassificationTypes.Literal;
					break;

				case TokenKind.Reference:
					ClassificationType = ClassificationTypes.Identifier;
					break;

				case TokenKind.Error:
					ClassificationType = ClassificationTypes.Literal;
					break;

				case TokenKind.MailSenderLink:
					ClassificationType = JDClassificationTypes.MailLink;
					break;

				case TokenKind.JDUpdateLinkButton:
					ClassificationType = JDClassificationTypes.UpdateJdLink;
					break;

				case TokenKind.JDUpdateText:
					ClassificationType = JDClassificationTypes.UpdateJdText;
					break;

				case TokenKind.Text:
					ClassificationType = JDClassificationTypes.Text;
					break;

				case TokenKind.NotResolvedReference:
					ClassificationType = JDClassificationTypes.NotResolvedReference;
					break;

				case TokenKind.DocumentationTag:
					ClassificationType = JDClassificationTypes.DocumentationTag;
					break;

				case TokenKind.Comment:
					ClassificationType = ClassificationTypes.Comment;
					break;

				case TokenKind.XmlAttribute:
					ClassificationType = JDClassificationTypes.XmlAttribute;
					break;

				case TokenKind.FakeReference:
					ClassificationType = JDClassificationTypes.FakeIdentifier;
					break;

				case TokenKind.NewLine:
					ClassificationType = JDClassificationTypes.Text;
					break;

				default:
					ClassificationType = ClassificationTypes.StringLiteral;
					break;
			}
		}
#endif
	}
}
