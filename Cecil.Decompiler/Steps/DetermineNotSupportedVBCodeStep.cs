using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Steps
{
	class DetermineNotSupportedVBCodeStep : BaseCodeTransformer, IDecompilationStep
	{
		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			NotSupportedUnaryOperatorFinder notSupportedUnaryOperatorFinder = new NotSupportedUnaryOperatorFinder();

			notSupportedUnaryOperatorFinder.Visit(body);

			if (notSupportedUnaryOperatorFinder.IsAddressUnaryOperatorFound)
			{
				throw new ArgumentException(string.Format("The unary opperator {0} is not supported in VisualBasic", notSupportedUnaryOperatorFinder.FoundUnaryOperator));
			}
            
            (new NotSupportedFeatureUsageFinder()).Visit(body);

			return body;
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
				Visit(node.MethodExpression);

				if (node.MethodExpression is MethodReferenceExpression)
				{
					this.methodInvocationsStackCount++;
				}

				Visit(node.Arguments);

				if (node.MethodExpression is MethodReferenceExpression)
				{
					this.methodInvocationsStackCount--;
				}
			}

			public override void  VisitUnaryExpression(UnaryExpression node)
			{
				if ((this.methodInvocationsStackCount == 0 && !(node.Operand is ArgumentReferenceExpression)) && 
					(node.Operator == UnaryOperator.AddressDereference || node.Operator == UnaryOperator.AddressReference || node.Operator == UnaryOperator.AddressOf))
				{
					this.IsAddressUnaryOperatorFound = true;
					this.FoundUnaryOperator = node.Operator;
				}

				base.VisitUnaryExpression(node);
			}
		}

        private class NotSupportedFeatureUsageFinder : BaseCodeVisitor
        {
            public override void VisitBinaryExpression(BinaryExpression node)
            {
                // We skip the left part of binary expressions which add or remove event handlers to event, because this is valid syntax.
                if (node.Left.CodeNodeType == CodeNodeType.EventReferenceExpression &&
                    (node.Operator == BinaryOperator.AddAssign || node.Operator == BinaryOperator.SubtractAssign))
                {
                    Visit(node.Right);

                    return;
                }

                base.VisitBinaryExpression(node);
            }

            public override void VisitEventReferenceExpression(EventReferenceExpression node)
            {
                throw new Exception("Visual Basic does not support this type of event usage. Please, try using other language.");
            }

            public override void VisitRefVariableDeclarationExpression(RefVariableDeclarationExpression node)
            {
                throw new Exception("Visual Basic does not support reference variables (a.k.a. ref locals). Please, try using other language.");
            }

            public override void VisitRefReturnExpression(RefReturnExpression node)
            {
                throw new Exception("Visual Basic does not support returning references (a.k.a. ref returns). Please, try using other language.");
            }
        }
    }
}
