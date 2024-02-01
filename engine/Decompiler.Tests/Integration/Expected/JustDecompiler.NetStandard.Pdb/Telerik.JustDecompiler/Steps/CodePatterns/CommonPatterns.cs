using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
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
			this.typeSystem = typeSystem;
			this.patternsContext = patternsContext;
		}

		protected void FixContext(VariableDefinition variable, int removedDefinitions, int removedUsages, ExpressionStatement newDefinition)
		{
			DefineUseCount defineUseCount;
			if (this.patternsContext.VariableToDefineUseCountContext.TryGetValue(variable, out defineUseCount))
			{
				defineUseCount.DefineCount -= removedDefinitions;
				defineUseCount.UseCount -= removedUsages;
				if (defineUseCount.DefineCount == 1 && newDefinition != null)
				{
					this.patternsContext.VariableToSingleAssignmentMap[variable] = newDefinition;
				}
			}
		}

		protected VariableReference GetVariableReferenceFromExpression(Expression theVariableExpression)
		{
			if (theVariableExpression.CodeNodeType == CodeNodeType.VariableReferenceExpression)
			{
				return (theVariableExpression as VariableReferenceExpression).Variable;
			}
			if (theVariableExpression.CodeNodeType != CodeNodeType.VariableDeclarationExpression)
			{
				return null;
			}
			return (theVariableExpression as VariableDeclarationExpression).Variable;
		}

		protected bool IsAssignToVariableExpression(BinaryExpression theAssignExpression, out VariableReference theVariable)
		{
			bool flag;
			theVariable = null;
			if (theAssignExpression == null || !theAssignExpression.IsAssignmentExpression)
			{
				flag = false;
			}
			else
			{
				flag = (theAssignExpression.Left.CodeNodeType == CodeNodeType.VariableReferenceExpression ? true : theAssignExpression.Left.CodeNodeType == CodeNodeType.VariableDeclarationExpression);
			}
			if (flag)
			{
				theVariable = this.GetVariableReferenceFromExpression(theAssignExpression.Left);
			}
			return flag;
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
				this.theVariable = theVariable;
				this.Used = false;
			}

			public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
			{
				if ((object)node.Variable == (object)this.theVariable)
				{
					this.Used = true;
					return;
				}
				base.VisitVariableReferenceExpression(node);
			}
		}
	}
}