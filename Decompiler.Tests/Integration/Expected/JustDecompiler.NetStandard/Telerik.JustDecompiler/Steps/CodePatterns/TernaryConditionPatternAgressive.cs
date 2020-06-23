using Mono.Cecil;
using System;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
	internal class TernaryConditionPatternAgressive : TernaryConditionPattern
	{
		public TernaryConditionPatternAgressive(CodePatternsContext patternsContext, TypeSystem typeSystem) : base(patternsContext, typeSystem)
		{
		}

		protected override bool ShouldInlineExpressions(BinaryExpression thenAssignExpression, BinaryExpression elseAssignExpression)
		{
			return true;
		}
	}
}