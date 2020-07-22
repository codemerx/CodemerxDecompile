namespace JustDecompile.EngineInfrastructure
{
	public enum TokenKind : byte
	{
		Text = 0,
		WhiteSpace,
		KeyWord,
		Reference,
		Literal,
		Comment,
		NewLine,
		XmlAttribute,
		Tab,
		Error,
		Xaml,
		EmbeddedResources,
		EmbeddedImage,
		Linked,
		AssemblyLinked,
		MailSenderLink,
		JDUpdateLinkButton,
		JDUpdateText,
		NotResolvedReference,
		FakeReference,
		DocumentationTag
	}
}
