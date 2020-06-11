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
using Mono.Cecil;
using Telerik.JustDecompiler.Decompiler.WriterContextServices;
using System.Collections.Generic;
using Telerik.JustDecompiler.External.Interfaces;

namespace Telerik.JustDecompiler.Languages
{
	public interface ILanguageWriter : IExceptionThrownNotifier
    {
		List<WritingInfo> Write(IMemberDefinition member, IWriterContextService writerContextService);
        void WriteTypeDeclarationsOnly(TypeDefinition member, IWriterContextService writerContextService);
		void WriteTypeDeclaration(TypeDefinition type, IWriterContextService writerContextService);
		void WriteBody(IMemberDefinition member, IWriterContextService writerContextService); // used by JustCode
        void Stop();
        void WriteMemberNavigationName(object memberReference);
        void WriteMemberEscapedOnlyName(object memberDefinition);
		void WriteMemberNavigationPathFullName(object member);
        void WriteNamespaceNavigationName(string memberDefinition);
		void WriteEscapeLiteral(string literal);
	}
}