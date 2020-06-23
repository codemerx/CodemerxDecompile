using Mono.Cecil;
using System;
using System.Runtime.CompilerServices;

namespace Telerik.JustDecompiler.Languages
{
	public interface IFormatter
	{
		int CurrentPosition
		{
			get;
		}

		void EndWritingComment();

		void Indent();

		void Outdent();

		void PreserveIndent(IMemberDefinition member);

		void RemovePreservedIndent(IMemberDefinition member);

		void RestoreIndent(IMemberDefinition member);

		void StartWritingComment();

		void Write(string str);

		void WriteComment(string comment);

		void WriteDefinition(string value, object definition);

		void WriteDocumentationStartBlock();

		void WriteDocumentationTag(string documentationTag);

		void WriteEndBlock();

		void WriteEndUsagesBlock();

		void WriteException(string[] exceptionLines);

		void WriteExceptionMailToLink(string mailToMessage, string[] exceptionLines);

		void WriteIdentifier(string value, object identifier);

		void WriteKeyword(string keyword);

		void WriteLine();

		void WriteLiteral(string literal);

		void WriteMemberDeclaration(IMemberDefinition member);

		void WriteNamespaceEndBlock();

		void WriteNamespaceStartBlock();

		void WriteNotResolvedReference(string value, MemberReference memberReference, string errorMessage);

		void WriteReference(string value, object reference);

		void WriteSpace();

		void WriteStartBlock();

		void WriteStartUsagesBlock();

		void WriteToken(string token);

		event EventHandler<int> FirstNonWhiteSpaceCharacterOnLineWritten;

		event EventHandler NewLineWritten;
	}
}