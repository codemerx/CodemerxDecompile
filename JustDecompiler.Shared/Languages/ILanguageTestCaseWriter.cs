using System;
using System.Linq;
using Mono.Cecil;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Languages
{
	interface ILanguageTestCaseWriter : ILanguageWriter
	{
		void WriteMethodDeclaration(MethodDefinition method, bool writeDocumentation = false);
		void Write(Statement statement);
		void Write(Expression expression);
		void SetContext(DecompilationContext context);
		void ResetContext();
		void SetContext(IMemberDefinition context);
	}
}
