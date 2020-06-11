using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Ast.Expressions;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Steps
{
	public class ArrayVariablesStep : BaseCodeVisitor, IDecompilationStep
	{
		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			Visit(body);
			return body;
		}

		public override void VisitBinaryExpression(BinaryExpression node)
		{
			base.VisitBinaryExpression(node);

			if (node.Operator == BinaryOperator.Assign && node.Right.CodeNodeType == CodeNodeType.ArrayCreationExpression)
			{
				ArrayCreationExpression arrayCreation = node.Right as ArrayCreationExpression;
				bool isInitializerPresent = Utilities.IsInitializerPresent(arrayCreation.Initializer);

				if (node.Left.CodeNodeType == CodeNodeType.VariableDeclarationExpression)
				{
					VariableDeclarationExpression variableDeclaration = node.Left as VariableDeclarationExpression;
					node.Left = new ArrayVariableDeclarationExpression(variableDeclaration, arrayCreation.ElementType, 
						arrayCreation.Dimensions.CloneExpressionsOnly(), isInitializerPresent, null);
				}

				if (node.Left.CodeNodeType == CodeNodeType.VariableReferenceExpression)
				{
					VariableReferenceExpression variableReference = node.Left as VariableReferenceExpression;
					node.Left = new ArrayVariableReferenceExpression(variableReference, arrayCreation.ElementType,
						arrayCreation.Dimensions.CloneExpressionsOnly(), isInitializerPresent, null);
				}

				if (node.Left.CodeNodeType == CodeNodeType.FieldReferenceExpression)
				{
					FieldReferenceExpression fieldReference = node.Left as FieldReferenceExpression;
					node.Left = new ArrayAssignmentFieldReferenceExpression(fieldReference, arrayCreation.ElementType,
						arrayCreation.Dimensions.CloneExpressionsOnly(), isInitializerPresent, null);
				}
			}
		}
	}
}
