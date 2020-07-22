using Mono.Cecil.Cil;
using System;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Decompiler.Inlining
{
	internal interface IVariableInliner
	{
		bool TryInlineVariable(VariableDefinition variableDef, Expression value, ICodeNode target, bool aggressive, out ICodeNode result);
	}
}