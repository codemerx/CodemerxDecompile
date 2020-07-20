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
			base();
			return;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.Visit(body);
			return body;
		}

		public override void VisitBinaryExpression(BinaryExpression node)
		{
			this.VisitBinaryExpression(node);
			if (node.get_Operator() == 26 && node.get_Right().get_CodeNodeType() == 38)
			{
				V_0 = node.get_Right() as ArrayCreationExpression;
				V_1 = Utilities.IsInitializerPresent(V_0.get_Initializer());
				if (node.get_Left().get_CodeNodeType() == 27)
				{
					V_2 = node.get_Left() as VariableDeclarationExpression;
					node.set_Left(new ArrayVariableDeclarationExpression(V_2, V_0.get_ElementType(), V_0.get_Dimensions().CloneExpressionsOnly(), V_1, null));
				}
				if (node.get_Left().get_CodeNodeType() == 26)
				{
					V_3 = node.get_Left() as VariableReferenceExpression;
					node.set_Left(new ArrayVariableReferenceExpression(V_3, V_0.get_ElementType(), V_0.get_Dimensions().CloneExpressionsOnly(), V_1, null));
				}
				if (node.get_Left().get_CodeNodeType() == 30)
				{
					V_4 = node.get_Left() as FieldReferenceExpression;
					node.set_Left(new ArrayAssignmentFieldReferenceExpression(V_4, V_0.get_ElementType(), V_0.get_Dimensions().CloneExpressionsOnly(), V_1, null));
				}
			}
			return;
		}
	}
}