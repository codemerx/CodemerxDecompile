using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
	abstract class CommonPatterns
	{
		protected readonly TypeSystem typeSystem;
        protected readonly CodePatternsContext patternsContext;

		public CommonPatterns(CodePatternsContext patternsContext, TypeSystem typeSystem)
		{
			this.typeSystem = typeSystem;
            this.patternsContext = patternsContext;
		}

		protected bool IsAssignToVariableExpression(BinaryExpression theAssignExpression, out VariableReference theVariable)
		{
			theVariable = null;
			bool result = theAssignExpression != null && theAssignExpression.IsAssignmentExpression &&
					 (theAssignExpression.Left.CodeNodeType == CodeNodeType.VariableReferenceExpression ||
					  theAssignExpression.Left.CodeNodeType == CodeNodeType.VariableDeclarationExpression);
			if (result)
			{
				theVariable = GetVariableReferenceFromExpression(theAssignExpression.Left);
			}
			return result;
		}

		protected VariableReference GetVariableReferenceFromExpression(Expression theVariableExpression)
		{
			if (theVariableExpression.CodeNodeType == CodeNodeType.VariableReferenceExpression)
			{
				return (theVariableExpression as VariableReferenceExpression).Variable;
			}
			else if (theVariableExpression.CodeNodeType == CodeNodeType.VariableDeclarationExpression)
			{
				return (theVariableExpression as VariableDeclarationExpression).Variable;
			}
			return null;
		}

        protected void FixContext(VariableDefinition variable, int removedDefinitions, int removedUsages, ExpressionStatement newDefinition)
        {
            DefineUseCount defineUseCount;
            if (patternsContext.VariableToDefineUseCountContext.TryGetValue(variable, out defineUseCount))
            {
                defineUseCount.DefineCount -= removedDefinitions;
                defineUseCount.UseCount -= removedUsages;

                if (defineUseCount.DefineCount == 1 && newDefinition != null)
                {
                    patternsContext.VariableToSingleAssignmentMap[variable] = newDefinition;
                }
            }
        }

		protected class VariableUsageFinder : BaseCodeVisitor
		{
			private readonly VariableReference theVariable;

			public VariableUsageFinder(VariableReference theVariable)
			{
				this.theVariable = theVariable;
				this.Used = false;
			}

			public bool Used { get; set; }

			public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
			{
				if (node.Variable == theVariable)
				{
					this.Used = true;
					return;
				}
				base.VisitVariableReferenceExpression(node);
			}
		}
	}
}
