#region license
//
//	(C) 2007 - 2008 Novell, Inc. http://www.novell.com
//	(C) 2007 - 2008 Jb Evain http://evain.net
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion

using System;
using System.IO;
using Mono.Cecil;
using System.Collections.Generic;
using JustDecompile.SmartAssembly.Attributes;

namespace Telerik.JustDecompiler.Languages
{
    [DoNotSealType]
    public class PlainTextFormatter : IFormatter //, IPositionMaintainingFormatter
    {
        protected StringWriter writer;
        protected bool write_indent;
        protected int indent;
        protected Dictionary<IMemberDefinition, int> preservedIndents;

        /// <summary>
        /// Determines if the formatter is writing only comments now. If this field is set true, all tokens that are created in the formatter will have their
        /// TokenKind changed to Comment. Tokens passed to the formatter will have their kind changed to "comment" as well. The value of this field should 
        /// be changed only via StartWritingComment and EndWritingComment methods. This will ensure that all text between the calls of StartWritingComment.
        /// and EndWritingComment will be formated as comment.
        /// </summary>
        protected bool writeCommentsOnly;

        public event EventHandler<int> FirstNonWhiteSpaceCharacterOnLineWritten;
        public event EventHandler NewLineWritten;

        public PlainTextFormatter(StringWriter writer)
        {
            this.writer = writer;
            writeCommentsOnly = false;
            this.preservedIndents = new Dictionary<IMemberDefinition, int>();
            this.CurrentLineNumber = 0;
            this.CurrentColumnIndex = 0;
        }

        /// <summary>
        /// Gets the offset from the beginning at which this formatter will write the next character
        /// </summary>
        public virtual int CurrentPosition
        {
            get
            {
                writer.Flush();
                return writer.GetStringBuilder().Length;
            }
        }

        public int CurrentLineNumber { get; set; }

        public int CurrentColumnIndex { get; set; }

        /// <summary>
        /// Gets the new line delimiter used by this formatter instance.
        /// </summary>
        public virtual string NewLine
        {
            get
            {
                return writer.NewLine;
            }
        }

        /// <summary>
        /// Switches the state of the formatter. From after calling this method, until the call of "EndWritingComment" all tokens will be changed to
        /// Comment tokens. Their text will remain the same.
        /// </summary>
        public void StartWritingComment()
        {
            writeCommentsOnly = true;
        }

        /// <summary>
        /// Switches the state of the formatter. Negates the effect of "StartWritingComment", effectively putting the formatter in its default state.
        /// </summary>
        public void EndWritingComment()
        {
            writeCommentsOnly = false;
        }

        protected virtual void WriteIndent()
        {
            if (!write_indent)
                return;

            for (int i = 0; i < indent; i++)
                WriteTab();

            write_indent = false;

            this.OnFirstNonWhiteSpaceCharacterOnLineWritten();
        }

        protected virtual void WriteTab()
        {
            writer.Write("\t");
            this.CurrentColumnIndex += "\t".Length;
        }

        public virtual void Write(string str)
        {
            WriteIndent();
            writer.Write(str);

            int newlineIndex = str.IndexOf(Environment.NewLine);
            if (newlineIndex != -1)
            {
                this.CurrentLineNumber++;
                this.CurrentColumnIndex = 0;
            }
            else
            {
                this.CurrentColumnIndex += str.Length;
            }
        }

        public virtual void WriteLine()
        {
            writer.WriteLine();
            write_indent = true;

            this.CurrentLineNumber++;
            this.CurrentColumnIndex = 0;

            OnNewLineWritten();
        }

        public virtual void WriteSpace()
        {
            Write(" ");
        }

        public virtual void WriteToken(string token)
        {
            Write(token);
        }

        public virtual void WriteComment(string comment)
        {
            Write(comment);
        }

        public virtual void WriteDocumentationTag(string tag)
        {
            Write(tag);
        }

        public virtual void WriteKeyword(string keyword)
        {
            Write(keyword);
        }

        public virtual void WriteLiteral(string literal)
        {
            Write(literal);
        }

        public virtual void WriteDefinition(string value, object definition)
        {
            Write(value);
        }

        public virtual void WriteReference(string value, object reference)
        {
            Write(value);
        }

        public virtual void WriteNotResolvedReference(string value, MemberReference memberReference, string errorMessage)
        {
            Write(value);
        }

        public virtual void WriteIdentifier(string value, object identifier)
        {
            Write(value);
        }

        public virtual void Indent()
        {
            indent++;
        }

        public virtual void Outdent()
        {
            indent--;
        }

        public void PreserveIndent(IMemberDefinition member)
        {
            preservedIndents.Add(member, indent);
        }

        public void RestoreIndent(IMemberDefinition member)
        {
            int memberIndent;

            if (!preservedIndents.TryGetValue(member, out memberIndent))
            {
                throw new Exception("Member not found in preserved indents cache.");
            }

            indent = memberIndent;
        }

        public void RemovePreservedIndent(IMemberDefinition member)
        {
            if (!preservedIndents.ContainsKey(member))
            {
                throw new Exception("MemberName not found in preserved indents cache.");
            }

            preservedIndents.Remove(member);
        }

        public virtual void WriteException(string[] exceptionLines)
        {
            foreach (string exceptionLine in exceptionLines)
            {
                Write(exceptionLine);
                WriteLine();
            }
        }

        public virtual void WriteExceptionMailToLink(string mailToMessage, string[] exceptionLines)
        {
            Write(mailToMessage);
            WriteLine();
        }

        public virtual void WriteStartBlock() { }

        public virtual void WriteDocumentationStartBlock() { }

        public virtual void WriteEndBlock() { }

        public override string ToString()
        {
            return this.writer.ToString();
        }

        public virtual void WriteNamespaceStartBlock() { }

        public virtual void WriteNamespaceEndBlock() { }

        public virtual void WriteStartUsagesBlock() { }

        public virtual void WriteEndUsagesBlock() { }

        public virtual void WriteMemberDeclaration(IMemberDefinition member){}

        protected void OnFirstNonWhiteSpaceCharacterOnLineWritten()
        {
            EventHandler<int> handler = this.FirstNonWhiteSpaceCharacterOnLineWritten;
            if (handler != null)
            {
                handler(this, this.CurrentPosition);
            }
        }

        protected void OnNewLineWritten()
        {
            EventHandler handler = this.NewLineWritten;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
