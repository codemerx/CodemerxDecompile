using System.Collections.Generic;
using Mono.Cecil;
using Telerik.JustDecompiler.Decompiler.WriterContextServices;

namespace Telerik.JustDecompiler.Languages
{
	public interface INamespaceLanguageWriter : ILanguageWriter
	{
		List<WritingInfo> WritePartialTypeAndNamespaces(TypeDefinition type, IWriterContextService writerContextService, Dictionary<string, ICollection<string>> fieldsToSkip = null);

		List<WritingInfo> WriteTypeAndNamespaces(TypeDefinition type, IWriterContextService writerContextService);

		List<WritingInfo> WriteType(TypeDefinition type, IWriterContextService writerContextService);
	}
}
