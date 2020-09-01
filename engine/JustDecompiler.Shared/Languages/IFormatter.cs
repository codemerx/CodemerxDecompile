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
using Mono.Cecil;

namespace Telerik.JustDecompiler.Languages {

	public interface IFormatter {
		int CurrentLineNumber { get; }
		int CurrentColumnIndex { get; }
		void Write (string str);
		void WriteLine ();
		void WriteSpace ();
		void WriteToken (string token);
		void WriteComment (string comment);
		void WriteDocumentationTag(string documentationTag);
		void WriteKeyword (string keyword);
		void WriteLiteral (string literal);
		void WriteDefinition (string value, object definition);
		void WriteReference (string value, object reference);
		void WriteIdentifier (string value, object identifier);
        void WriteException(string[] exceptionLines);
		void WriteExceptionMailToLink(string mailToMessage, string[] exceptionLines);
        void WriteNotResolvedReference(string value, MemberReference memberReference, string errorMessage);
		void Indent ();
		void Outdent ();
		void PreserveIndent(IMemberDefinition member);
		void RestoreIndent(IMemberDefinition member);
		void RemovePreservedIndent(IMemberDefinition member);
        int CurrentPosition { get; }

		/// <summary>
		/// Instructs the formatter, that all the text it recieves from now on should be handled as comment.
		/// </summary>
		void StartWritingComment();
		/// <summary>
		/// Instructs the formatter to stop handling all the text it recieves as comment and return to its default behavior.
		/// </summary>
		void EndWritingComment();
        void WriteStartBlock();
		void WriteDocumentationStartBlock();
        void WriteEndBlock();
        void WriteNamespaceStartBlock();
        void WriteNamespaceEndBlock();
        void WriteStartUsagesBlock();
        void WriteEndUsagesBlock();

        void WriteMemberDeclaration(IMemberDefinition member);

        event EventHandler<int> FirstNonWhiteSpaceCharacterOnLineWritten;
        event EventHandler NewLineWritten;
    }
}
