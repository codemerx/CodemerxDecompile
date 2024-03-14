using Mono.Cecil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Decompiler.WriterContextServices;
using Telerik.JustDecompiler.External.Interfaces;

namespace Telerik.JustDecompiler.Languages
{
	public interface INamespaceLanguageWriter : ILanguageWriter, IExceptionThrownNotifier
	{
		List<WritingInfo> WritePartialTypeAndNamespaces(TypeDefinition type, IWriterContextService writerContextService, Dictionary<string, ICollection<string>> fieldsToSkip = null);

		List<WritingInfo> WriteType(TypeDefinition type, IWriterContextService writerContextService);

		List<WritingInfo> WriteTypeAndNamespaces(TypeDefinition type, IWriterContextService writerContextService);
	}
}