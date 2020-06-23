using JustDecompile.SmartAssembly.Attributes;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Telerik.JustDecompiler.Languages
{
	[DoNotSealType]
	public class PlainTextFormatter : IFormatter
	{
		protected StringWriter writer;

		protected bool write_indent;

		protected int indent;

		protected Dictionary<IMemberDefinition, int> preservedIndents;

		protected bool writeCommentsOnly;

		public virtual int CurrentPosition
		{
			get
			{
				this.writer.Flush();
				return this.writer.GetStringBuilder().Length;
			}
		}

		public virtual string NewLine
		{
			get
			{
				return this.writer.NewLine;
			}
		}

		public PlainTextFormatter(StringWriter writer)
		{
			this.writer = writer;
			this.writeCommentsOnly = false;
			this.preservedIndents = new Dictionary<IMemberDefinition, int>();
		}

		public void EndWritingComment()
		{
			this.writeCommentsOnly = false;
		}

		public virtual void Indent()
		{
			this.indent++;
		}

		protected void OnFirstNonWhiteSpaceCharacterOnLineWritten()
		{
			EventHandler<int> eventHandler = this.FirstNonWhiteSpaceCharacterOnLineWritten;
			if (eventHandler != null)
			{
				eventHandler(this, this.CurrentPosition);
			}
		}

		protected void OnNewLineWritten()
		{
			EventHandler eventHandler = this.NewLineWritten;
			if (eventHandler != null)
			{
				eventHandler(this, EventArgs.Empty);
			}
		}

		public virtual void Outdent()
		{
			this.indent--;
		}

		public void PreserveIndent(IMemberDefinition member)
		{
			this.preservedIndents.Add(member, this.indent);
		}

		public void RemovePreservedIndent(IMemberDefinition member)
		{
			if (!this.preservedIndents.ContainsKey(member))
			{
				throw new Exception("MemberName not found in preserved indents cache.");
			}
			this.preservedIndents.Remove(member);
		}

		public void RestoreIndent(IMemberDefinition member)
		{
			int num;
			if (!this.preservedIndents.TryGetValue(member, out num))
			{
				throw new Exception("Member not found in preserved indents cache.");
			}
			this.indent = num;
		}

		public void StartWritingComment()
		{
			this.writeCommentsOnly = true;
		}

		public override string ToString()
		{
			return this.writer.ToString();
		}

		public virtual void Write(string str)
		{
			this.WriteIndent();
			this.writer.Write(str);
		}

		public virtual void WriteComment(string comment)
		{
			this.Write(comment);
		}

		public virtual void WriteDefinition(string value, object definition)
		{
			this.Write(value);
		}

		public virtual void WriteDocumentationStartBlock()
		{
		}

		public virtual void WriteDocumentationTag(string tag)
		{
			this.Write(tag);
		}

		public virtual void WriteEndBlock()
		{
		}

		public virtual void WriteEndUsagesBlock()
		{
		}

		public virtual void WriteException(string[] exceptionLines)
		{
			string[] strArray = exceptionLines;
			for (int i = 0; i < (int)strArray.Length; i++)
			{
				this.Write(strArray[i]);
				this.WriteLine();
			}
		}

		public virtual void WriteExceptionMailToLink(string mailToMessage, string[] exceptionLines)
		{
			this.Write(mailToMessage);
			this.WriteLine();
		}

		public virtual void WriteIdentifier(string value, object identifier)
		{
			this.Write(value);
		}

		protected virtual void WriteIndent()
		{
			if (!this.write_indent)
			{
				return;
			}
			for (int i = 0; i < this.indent; i++)
			{
				this.WriteTab();
			}
			this.write_indent = false;
			this.OnFirstNonWhiteSpaceCharacterOnLineWritten();
		}

		public virtual void WriteKeyword(string keyword)
		{
			this.Write(keyword);
		}

		public virtual void WriteLine()
		{
			this.writer.WriteLine();
			this.write_indent = true;
			this.OnNewLineWritten();
		}

		public virtual void WriteLiteral(string literal)
		{
			this.Write(literal);
		}

		public virtual void WriteMemberDeclaration(IMemberDefinition member)
		{
		}

		public virtual void WriteNamespaceEndBlock()
		{
		}

		public virtual void WriteNamespaceStartBlock()
		{
		}

		public virtual void WriteNotResolvedReference(string value, MemberReference memberReference, string errorMessage)
		{
			this.Write(value);
		}

		public virtual void WriteReference(string value, object reference)
		{
			this.Write(value);
		}

		public virtual void WriteSpace()
		{
			this.Write(" ");
		}

		public virtual void WriteStartBlock()
		{
		}

		public virtual void WriteStartUsagesBlock()
		{
		}

		protected virtual void WriteTab()
		{
			this.writer.Write("\t");
		}

		public virtual void WriteToken(string token)
		{
			this.Write(token);
		}

		public event EventHandler<int> FirstNonWhiteSpaceCharacterOnLineWritten;

		public event EventHandler NewLineWritten;
	}
}