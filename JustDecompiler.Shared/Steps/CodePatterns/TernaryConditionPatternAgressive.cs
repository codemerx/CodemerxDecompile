using Mono.Cecil;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
	class TernaryConditionPatternAgressive : TernaryConditionPattern
	{
		public TernaryConditionPatternAgressive(CodePatternsContext patternsContext, TypeSystem typeSystem) : base(patternsContext, typeSystem)
		{
		}

		protected override bool ShouldInlineExpressions(BinaryExpression thenAssignExpression, BinaryExpression elseAssignExpression)
		{
			/// Inline any expression. This is needed only in cases, where nesting ternaries is the only way
			/// to call this/base constructor legaly.
			return true;
		}
	}
}
