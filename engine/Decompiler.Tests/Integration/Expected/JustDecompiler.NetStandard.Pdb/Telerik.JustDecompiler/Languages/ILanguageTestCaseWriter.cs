using Mono.Cecil;
using System;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.External.Interfaces;

namespace Telerik.JustDecompiler.Languages
{
	internal interface ILanguageTestCaseWriter : ILanguageWriter, IExceptionThrownNotifier
	{
		void ResetContext();

		void SetContext(DecompilationContext context);

		void SetContext(IMemberDefinition context);

		void Write(Statement statement);

		void Write(Expression expression);

		void WriteMethodDeclaration(MethodDefinition method, bool writeDocumentation = false);
	}
}