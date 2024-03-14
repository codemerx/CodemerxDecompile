using System;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	public class ArrayVariablesStep : BaseCodeVisitor, IDecompilationStep
	{
		public ArrayVariablesStep()
		{
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.Visit(body);
			return body;
		}

		public override void VisitBinaryExpression(BinaryExpression node)
		{
			base.VisitBinaryExpression(node);
			if (node.Operator == BinaryOperator.Assign && node.Right.CodeNodeType == CodeNodeType.ArrayCreationExpression)
			{
				ArrayCreationExpression right = node.Right as ArrayCreationExpression;
				bool flag = Utilities.IsInitializerPresent(right.Initializer);
				if (node.Left.CodeNodeType == CodeNodeType.VariableDeclarationExpression)
				{
					VariableDeclarationExpression left = node.Left as VariableDeclarationExpression;
					node.Left = new ArrayVariableDeclarationExpression(left, right.ElementType, right.Dimensions.CloneExpressionsOnly(), flag, null);
				}
				if (node.Left.CodeNodeType == CodeNodeType.VariableReferenceExpression)
				{
					VariableReferenceExpression variableReferenceExpression = node.Left as VariableReferenceExpression;
					node.Left = new ArrayVariableReferenceExpression(variableReferenceExpression, right.ElementType, right.Dimensions.CloneExpressionsOnly(), flag, null);
				}
				if (node.Left.CodeNodeType == CodeNodeType.FieldReferenceExpression)
				{
					FieldReferenceExpression fieldReferenceExpression = node.Left as FieldReferenceExpression;
					node.Left = new ArrayAssignmentFieldReferenceExpression(fieldReferenceExpression, right.ElementType, right.Dimensions.CloneExpressionsOnly(), flag, null);
				}
			}
		}
	}
}