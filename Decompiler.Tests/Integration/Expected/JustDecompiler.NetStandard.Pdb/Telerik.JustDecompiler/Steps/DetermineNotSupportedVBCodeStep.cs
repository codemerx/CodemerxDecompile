using System;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class DetermineNotSupportedVBCodeStep : BaseCodeTransformer, IDecompilationStep
	{
		public DetermineNotSupportedVBCodeStep()
		{
			base();
			return;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			V_0 = new DetermineNotSupportedVBCodeStep.NotSupportedUnaryOperatorFinder();
			V_0.Visit(body);
			if (V_0.IsAddressUnaryOperatorFound)
			{
				throw new ArgumentException(String.Format("The unary opperator {0} is not supported in VisualBasic", V_0.FoundUnaryOperator));
			}
			(new DetermineNotSupportedVBCodeStep.NotSupportedFeatureUsageFinder()).Visit(body);
			return body;
		}

		private class NotSupportedFeatureUsageFinder : BaseCodeVisitor
		{
			public NotSupportedFeatureUsageFinder()
			{
				base();
				return;
			}

			public override void VisitBinaryExpression(BinaryExpression node)
			{
				if (node.get_Left().get_CodeNodeType() != 48 || node.get_Operator() != 2 && node.get_Operator() != 4)
				{
					this.VisitBinaryExpression(node);
					return;
				}
				this.Visit(node.get_Right());
				return;
			}

			public override void VisitEventReferenceExpression(EventReferenceExpression node)
			{
				throw new Exception("Visual Basic does not support this type of event usage. Please, try using other language.");
			}

			public override void VisitRefReturnExpression(RefReturnExpression node)
			{
				throw new Exception("Visual Basic does not support returning references (a.k.a. ref returns). Please, try using other language.");
			}

			public override void VisitRefVariableDeclarationExpression(RefVariableDeclarationExpression node)
			{
				throw new Exception("Visual Basic does not support reference variables (a.k.a. ref locals). Please, try using other language.");
			}
		}

		private class NotSupportedUnaryOperatorFinder : BaseCodeVisitor
		{
			public bool IsAddressUnaryOperatorFound;

			public UnaryOperator FoundUnaryOperator;

			private int methodInvocationsStackCount;

			public NotSupportedUnaryOperatorFinder()
			{
				base();
				this.IsAddressUnaryOperatorFound = false;
				this.FoundUnaryOperator = 11;
				this.methodInvocationsStackCount = 0;
				return;
			}

			public override void Visit(ICodeNode node)
			{
				if (this.IsAddressUnaryOperatorFound)
				{
					return;
				}
				this.Visit(node);
				return;
			}

			public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
			{
				this.Visit(node.get_MethodExpression());
				if (node.get_MethodExpression() != null)
				{
					this.methodInvocationsStackCount = this.methodInvocationsStackCount + 1;
				}
				this.Visit(node.get_Arguments());
				if (node.get_MethodExpression() != null)
				{
					this.methodInvocationsStackCount = this.methodInvocationsStackCount - 1;
				}
				return;
			}

			public override void VisitUnaryExpression(UnaryExpression node)
			{
				if (this.methodInvocationsStackCount == 0 && node.get_Operand() as ArgumentReferenceExpression == null && node.get_Operator() == 8 || node.get_Operator() == 7 || node.get_Operator() == 9)
				{
					this.IsAddressUnaryOperatorFound = true;
					this.FoundUnaryOperator = node.get_Operator();
				}
				this.VisitUnaryExpression(node);
				return;
			}
		}
	}
}