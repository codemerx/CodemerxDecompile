using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
	internal abstract class CommonPatterns
	{
		protected readonly TypeSystem typeSystem;

		protected readonly CodePatternsContext patternsContext;

		public CommonPatterns(CodePatternsContext patternsContext, TypeSystem typeSystem)
		{
			base();
			this.typeSystem = typeSystem;
			this.patternsContext = patternsContext;
			return;
		}

		protected void FixContext(VariableDefinition variable, int removedDefinitions, int removedUsages, ExpressionStatement newDefinition)
		{
			if (this.patternsContext.get_VariableToDefineUseCountContext().TryGetValue(variable, out V_0))
			{
				stackVariable6 = V_0;
				stackVariable6.DefineCount = stackVariable6.DefineCount - removedDefinitions;
				stackVariable10 = V_0;
				stackVariable10.UseCount = stackVariable10.UseCount - removedUsages;
				if (V_0.DefineCount == 1 && newDefinition != null)
				{
					this.patternsContext.get_VariableToSingleAssignmentMap().set_Item(variable, newDefinition);
				}
			}
			return;
		}

		protected VariableReference GetVariableReferenceFromExpression(Expression theVariableExpression)
		{
			if (theVariableExpression.get_CodeNodeType() == 26)
			{
				return (theVariableExpression as VariableReferenceExpression).get_Variable();
			}
			if (theVariableExpression.get_CodeNodeType() != 27)
			{
				return null;
			}
			return (theVariableExpression as VariableDeclarationExpression).get_Variable();
		}

		protected bool IsAssignToVariableExpression(BinaryExpression theAssignExpression, out VariableReference theVariable)
		{
			theVariable = null;
			if (theAssignExpression == null || !theAssignExpression.get_IsAssignmentExpression())
			{
				stackVariable3 = false;
			}
			else
			{
				if (theAssignExpression.get_Left().get_CodeNodeType() == 26)
				{
					stackVariable3 = true;
				}
				else
				{
					stackVariable3 = theAssignExpression.get_Left().get_CodeNodeType() == 27;
				}
			}
			if (stackVariable3)
			{
				theVariable = this.GetVariableReferenceFromExpression(theAssignExpression.get_Left());
			}
			return stackVariable3;
		}

		protected class VariableUsageFinder : BaseCodeVisitor
		{
			private readonly VariableReference theVariable;

			public bool Used
			{
				get;
				set;
			}

			public VariableUsageFinder(VariableReference theVariable)
			{
				base();
				this.theVariable = theVariable;
				this.set_Used(false);
				return;
			}

			public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
			{
				if ((object)node.get_Variable() == (object)this.theVariable)
				{
					this.set_Used(true);
					return;
				}
				this.VisitVariableReferenceExpression(node);
				return;
			}
		}
	}
}