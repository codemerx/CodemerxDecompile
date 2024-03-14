using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Decompiler.Inlining
{
	internal class EmptyVariablesToNotInlineFinder : IVariablesToNotInlineFinder
	{
		public EmptyVariablesToNotInlineFinder()
		{
		}

		public HashSet<VariableDefinition> Find(StatementCollection statements)
		{
			return new HashSet<VariableDefinition>();
		}

		public HashSet<VariableDefinition> Find(Dictionary<int, IList<Expression>> blockExpressions)
		{
			return new HashSet<VariableDefinition>();
		}
	}
}