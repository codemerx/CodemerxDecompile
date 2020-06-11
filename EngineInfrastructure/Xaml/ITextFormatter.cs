namespace JustDecompile.EngineInfrastructure
{
	public interface ITextFormatter
	{
		void WriteKeyword(string keyword);

		void WriteLiteral(string literal);

		void Write(string str);

		void WritePlainText(string text);

		void WriteReference(string value, object reference);

		void WriteLine(bool indent = true);

		void IncrementIndent();

		void DecrementIndent();

		void WriteComment(string comment);

		void WriteXmlAttribute(string attribute);

		void WriteStartBlock();

		void WriteEndBlock();
	}
}
