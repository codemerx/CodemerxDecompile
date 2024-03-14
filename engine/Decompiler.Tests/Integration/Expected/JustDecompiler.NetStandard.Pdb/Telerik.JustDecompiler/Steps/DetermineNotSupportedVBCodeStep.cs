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
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			DetermineNotSupportedVBCodeStep.NotSupportedUnaryOperatorFinder notSupportedUnaryOperatorFinder = new DetermineNotSupportedVBCodeStep.NotSupportedUnaryOperatorFinder();
			notSupportedUnaryOperatorFinder.Visit(body);
			if (notSupportedUnaryOperatorFinder.IsAddressUnaryOperatorFound)
			{
				throw new ArgumentException(String.Format("The unary opperator {0} is not supported in VisualBasic", notSupportedUnaryOperatorFinder.FoundUnaryOperator));
			}
			(new DetermineNotSupportedVBCodeStep.NotSupportedFeatureUsageFinder()).Visit(body);
			return body;
		}

		private class NotSupportedFeatureUsageFinder : BaseCodeVisitor
		{
			public NotSupportedFeatureUsageFinder()
			{
			}

			public override void VisitBinaryExpression(BinaryExpression node)
			{
				if (node.Left.CodeNodeType != CodeNodeType.EventReferenceExpression || node.Operator != BinaryOperator.AddAssign && node.Operator != BinaryOperator.SubtractAssign)
				{
					base.VisitBinaryExpression(node);
					return;
				}
				this.Visit(node.Right);
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
				this.IsAddressUnaryOperatorFound = false;
				this.FoundUnaryOperator = UnaryOperator.None;
				this.methodInvocationsStackCount = 0;
			}

			public override void Visit(ICodeNode node)
			{
				if (this.IsAddressUnaryOperatorFound)
				{
					return;
				}
				base.Visit(node);
			}

			public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
			{
				this.Visit(node.MethodExpression);
				if (node.MethodExpression != null)
				{
					this.methodInvocationsStackCount++;
				}
				this.Visit(node.Arguments);
				if (node.MethodExpression != null)
				{
					this.methodInvocationsStackCount--;
				}
			}

			public override void VisitUnaryExpression(UnaryExpression node)
			{
				if (this.methodInvocationsStackCount == 0 && !(node.Operand is ArgumentReferenceExpression) && (node.Operator == UnaryOperator.AddressDereference || node.Operator == UnaryOperator.AddressReference || node.Operator == UnaryOperator.AddressOf))
				{
					this.IsAddressUnaryOperatorFound = true;
					this.FoundUnaryOperator = node.Operator;
				}
				base.VisitUnaryExpression(node);
			}
		}
	}
}