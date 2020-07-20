using JustDecompile.SmartAssembly.Attributes;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

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
				return this.writer.GetStringBuilder().get_Length();
			}
		}

		public virtual string NewLine
		{
			get
			{
				return this.writer.get_NewLine();
			}
		}

		public PlainTextFormatter(StringWriter writer)
		{
			base();
			this.writer = writer;
			this.writeCommentsOnly = false;
			this.preservedIndents = new Dictionary<IMemberDefinition, int>();
			return;
		}

		public void EndWritingComment()
		{
			this.writeCommentsOnly = false;
			return;
		}

		public virtual void Indent()
		{
			this.indent = this.indent + 1;
			return;
		}

		protected void OnFirstNonWhiteSpaceCharacterOnLineWritten()
		{
			V_0 = this.FirstNonWhiteSpaceCharacterOnLineWritten;
			if (V_0 != null)
			{
				V_0.Invoke(this, this.get_CurrentPosition());
			}
			return;
		}

		protected void OnNewLineWritten()
		{
			V_0 = this.NewLineWritten;
			if (V_0 != null)
			{
				V_0.Invoke(this, EventArgs.Empty);
			}
			return;
		}

		public virtual void Outdent()
		{
			this.indent = this.indent - 1;
			return;
		}

		public void PreserveIndent(IMemberDefinition member)
		{
			this.preservedIndents.Add(member, this.indent);
			return;
		}

		public void RemovePreservedIndent(IMemberDefinition member)
		{
			if (!this.preservedIndents.ContainsKey(member))
			{
				throw new Exception("MemberName not found in preserved indents cache.");
			}
			dummyVar0 = this.preservedIndents.Remove(member);
			return;
		}

		public void RestoreIndent(IMemberDefinition member)
		{
			if (!this.preservedIndents.TryGetValue(member, out V_0))
			{
				throw new Exception("Member not found in preserved indents cache.");
			}
			this.indent = V_0;
			return;
		}

		public void StartWritingComment()
		{
			this.writeCommentsOnly = true;
			return;
		}

		public override string ToString()
		{
			return this.writer.ToString();
		}

		public virtual void Write(string str)
		{
			this.WriteIndent();
			this.writer.Write(str);
			return;
		}

		public virtual void WriteComment(string comment)
		{
			this.Write(comment);
			return;
		}

		public virtual void WriteDefinition(string value, object definition)
		{
			this.Write(value);
			return;
		}

		public virtual void WriteDocumentationStartBlock()
		{
			return;
		}

		public virtual void WriteDocumentationTag(string tag)
		{
			this.Write(tag);
			return;
		}

		public virtual void WriteEndBlock()
		{
			return;
		}

		public virtual void WriteEndUsagesBlock()
		{
			return;
		}

		public virtual void WriteException(string[] exceptionLines)
		{
			V_0 = exceptionLines;
			V_1 = 0;
			while (V_1 < (int)V_0.Length)
			{
				this.Write(V_0[V_1]);
				this.WriteLine();
				V_1 = V_1 + 1;
			}
			return;
		}

		public virtual void WriteExceptionMailToLink(string mailToMessage, string[] exceptionLines)
		{
			this.Write(mailToMessage);
			this.WriteLine();
			return;
		}

		public virtual void WriteIdentifier(string value, object identifier)
		{
			this.Write(value);
			return;
		}

		protected virtual void WriteIndent()
		{
			if (!this.write_indent)
			{
				return;
			}
			V_0 = 0;
			while (V_0 < this.indent)
			{
				this.WriteTab();
				V_0 = V_0 + 1;
			}
			this.write_indent = false;
			this.OnFirstNonWhiteSpaceCharacterOnLineWritten();
			return;
		}

		public virtual void WriteKeyword(string keyword)
		{
			this.Write(keyword);
			return;
		}

		public virtual void WriteLine()
		{
			this.writer.WriteLine();
			this.write_indent = true;
			this.OnNewLineWritten();
			return;
		}

		public virtual void WriteLiteral(string literal)
		{
			this.Write(literal);
			return;
		}

		public virtual void WriteMemberDeclaration(IMemberDefinition member)
		{
			return;
		}

		public virtual void WriteNamespaceEndBlock()
		{
			return;
		}

		public virtual void WriteNamespaceStartBlock()
		{
			return;
		}

		public virtual void WriteNotResolvedReference(string value, MemberReference memberReference, string errorMessage)
		{
			this.Write(value);
			return;
		}

		public virtual void WriteReference(string value, object reference)
		{
			this.Write(value);
			return;
		}

		public virtual void WriteSpace()
		{
			this.Write(" ");
			return;
		}

		public virtual void WriteStartBlock()
		{
			return;
		}

		public virtual void WriteStartUsagesBlock()
		{
			return;
		}

		protected virtual void WriteTab()
		{
			this.writer.Write("\t");
			return;
		}

		public virtual void WriteToken(string token)
		{
			this.Write(token);
			return;
		}

		public event EventHandler<int> FirstNonWhiteSpaceCharacterOnLineWritten;

		public event EventHandler NewLineWritten;
	}
}