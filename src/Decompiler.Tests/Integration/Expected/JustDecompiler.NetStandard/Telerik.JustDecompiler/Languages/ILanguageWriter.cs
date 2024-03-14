using Mono.Cecil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Decompiler.WriterContextServices;
using Telerik.JustDecompiler.External.Interfaces;

namespace Telerik.JustDecompiler.Languages
{
	public interface ILanguageWriter : IExceptionThrownNotifier
	{
		void Stop();

		List<WritingInfo> Write(IMemberDefinition member, IWriterContextService writerContextService);

		void WriteBody(IMemberDefinition member, IWriterContextService writerContextService);

		void WriteEscapeLiteral(string literal);

		void WriteMemberEscapedOnlyName(object memberDefinition);

		void WriteMemberNavigationName(object memberReference);

		void WriteMemberNavigationPathFullName(object member);

		void WriteNamespaceNavigationName(string memberDefinition);

		void WriteTypeDeclaration(TypeDefinition type, IWriterContextService writerContextService);

		void WriteTypeDeclarationsOnly(TypeDefinition member, IWriterContextService writerContextService);
	}
}