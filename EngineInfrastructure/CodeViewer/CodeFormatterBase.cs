using System;
using Mono.Cecil;
using System.Collections.Generic;
using Telerik.JustDecompiler.Languages;
using System.IO;

namespace JustDecompile.EngineInfrastructure
{
	public class CodeFormatterBase : PlainTextFormatter
	{
		protected const int SpaceCountRepresentingTab = 4;

		private readonly IList<Tuple<int, IMemberDefinition>> lineToMemberMap;

		public CodeFormatterBase(StringWriter writer)
			: base(writer)
		{
			this.lineToMemberMap = new List<Tuple<int, IMemberDefinition>>();
			this.CurrentLineNumber = 0;
		}

		public override string NewLine
		{
			get
			{
				return "\n";
			}
		}

		protected IList<Tuple<int, IMemberDefinition>> LineToMemberMap
		{
			get
			{
				return this.lineToMemberMap;
			}
		}

		protected int CurrentLineNumber { get; set; }

		public override void WriteMemberDeclaration(IMemberDefinition member)
		{
			this.lineToMemberMap.Add(Tuple.Create(this.CurrentLineNumber, member));
		}

		public override void Write(string str)
		{
			base.Write(str);

			int newlineIndex = str.IndexOf(Environment.NewLine);
			if (newlineIndex != -1)
			{
				this.CurrentLineNumber++;
			}
		}

		public override void WriteLine()
		{
			this.writer.Write(this.NewLine);
			this.write_indent = true;

			this.CurrentLineNumber++;
            this.OnNewLineWritten();
		}

		protected string CleanLastNewLineIfAny(string sourceCodeResult)
		{
			int lastIndex = sourceCodeResult.LastIndexOf('\n');
			if (lastIndex > 0 && lastIndex == sourceCodeResult.Length - 1)
			{
				sourceCodeResult = sourceCodeResult.Remove(lastIndex);
			}
			return sourceCodeResult;
		}
	}
}
