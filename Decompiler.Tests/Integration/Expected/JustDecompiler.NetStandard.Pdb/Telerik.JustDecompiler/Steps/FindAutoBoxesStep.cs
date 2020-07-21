using Mono.Cecil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class FindAutoBoxesStep : BaseCodeVisitor, IDecompilationStep
	{
		private MethodSpecificContext context;

		private readonly Stack<Expression> parentExpressions;

		public FindAutoBoxesStep()
		{
			this.parentExpressions = new Stack<Expression>();
			base();
			return;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context.get_MethodContext();
			V_0 = context.get_MethodContext().get_Expressions().get_BlockExpressions().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					this.parentExpressions.Clear();
					V_2 = V_1.get_Value().GetEnumerator();
					try
					{
						while (V_2.MoveNext())
						{
							V_3 = V_2.get_Current();
							this.Visit(V_3);
						}
					}
					finally
					{
						if (V_2 != null)
						{
							V_2.Dispose();
						}
					}
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return body;
		}

		public override void Visit(ICodeNode node)
		{
			if (node as Expression != null)
			{
				this.parentExpressions.Push(node as Expression);
			}
			this.Visit(node);
			if (node as Expression != null)
			{
				dummyVar0 = this.parentExpressions.Pop();
			}
			return;
		}

		public override void VisitBoxExpression(BoxExpression node)
		{
			V_0 = new Stack<Expression>();
			V_0.Push(this.parentExpressions.Pop());
			if (this.parentExpressions.get_Count() > 0)
			{
				V_1 = this.parentExpressions.Peek();
				if (V_1 as MemberReferenceExpresion == null)
				{
					if (V_1 as MethodInvocationExpression != null || V_1 as FieldReferenceExpression != null || V_1 as PropertyReferenceExpression != null)
					{
						node.set_IsAutoBox(true);
					}
					else
					{
						if (V_1 as BinaryExpression == null)
						{
							if (V_1 as ReturnExpression == null)
							{
								if (V_1 as YieldReturnExpression != null && (V_1 as YieldReturnExpression).get_Expression().Equals(node))
								{
									node.set_IsAutoBox(true);
								}
							}
							else
							{
								if (!String.op_Equality(this.context.get_Method().get_ReturnType().get_FullName(), "System.Object"))
								{
									V_4 = node.get_BoxedExpression().get_ExpressionType().Resolve();
									if (V_4 != null && V_4.get_IsValueType())
									{
										node.set_IsAutoBox(true);
									}
								}
								else
								{
									node.set_IsAutoBox(true);
								}
							}
						}
						else
						{
							V_3 = V_1 as BinaryExpression;
							if (V_3.get_IsAssignmentExpression() && V_3.get_Right().Equals(node))
							{
								node.set_IsAutoBox(true);
							}
							if (V_3.get_IsComparisonExpression() && node.get_BoxedAs().get_IsGenericParameter())
							{
								if (V_3.get_Left() == node && V_3.get_Right() as LiteralExpression != null && (V_3.get_Right() as LiteralExpression).get_Value() == null)
								{
									node.set_IsAutoBox(true);
								}
								if (V_3.get_Right() == node && V_3.get_Left() as LiteralExpression != null && (V_3.get_Left() as LiteralExpression).get_Value() == null)
								{
									node.set_IsAutoBox(true);
								}
							}
						}
					}
				}
				else
				{
					V_2 = this.parentExpressions.Pop() as MemberReferenceExpresion;
					V_0.Push(V_2);
					if (V_2.get_Member() as MethodReference != null)
					{
						node.set_IsAutoBox(true);
					}
				}
			}
			while (V_0.get_Count() > 0)
			{
				this.parentExpressions.Push(V_0.Pop());
			}
			this.VisitBoxExpression(node);
			return;
		}
	}
}