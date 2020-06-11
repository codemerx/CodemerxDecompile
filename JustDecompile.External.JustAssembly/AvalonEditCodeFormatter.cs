using JustDecompile.EngineInfrastructure;
using System.IO;

namespace JustDecompile.External.JustAssembly
{
	public class AvalonEditCodeFormatter : CodeFormatterBase, ICodeFormatter
	{
		public AvalonEditCodeFormatter(StringWriter writer)
			: base(writer)
		{
		}

		public DecompiledSourceCode GetSourceCode()
		{
			string sourceCode = this.CleanLastNewLineIfAny(writer.ToString());

			return new DecompiledSourceCode(sourceCode, LineToMemberMap);
		}

		public void WritePlainText(string text)
		{
			this.Write(text);
		}

		public void WriteLine(bool indent = true)
		{
			this.WriteLine();
			this.write_indent = indent;
		}

		public void IncrementIndent()
		{
			this.indent++;
		}

		public void DecrementIndent()
		{
			this.indent--;
		}

		public void WriteXmlAttribute(string attribute)
		{
			this.Write(attribute);
		}

		protected override void WriteTab()
		{
			this.writer.Write(new string(' ', SpaceCountRepresentingTab));
		}
	}
}
