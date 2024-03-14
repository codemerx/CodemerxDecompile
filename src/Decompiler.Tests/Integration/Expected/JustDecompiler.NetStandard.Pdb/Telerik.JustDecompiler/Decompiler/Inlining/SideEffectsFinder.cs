using System;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Decompiler.Inlining
{
	internal class SideEffectsFinder : BaseCodeVisitor
	{
		private bool hasSideEffects;

		public SideEffectsFinder()
		{
		}

		public static bool HasSideEffects(ICodeNode node)
		{
			if (node == null)
			{
				return false;
			}
			CodeNodeType codeNodeType = node.CodeNodeType;
			if (codeNodeType > CodeNodeType.DelegateCreationExpression)
			{
				if (codeNodeType == CodeNodeType.BinaryExpression)
				{
					BinaryExpression binaryExpression = node as BinaryExpression;
					if (binaryExpression.IsChecked || binaryExpression.Operator == BinaryOperator.Divide)
					{
						return true;
					}
					return binaryExpression.Operator == BinaryOperator.Modulo;
				}
				switch (codeNodeType)
				{
					case CodeNodeType.FieldReferenceExpression:
					{
						return !(node as FieldReferenceExpression).IsSimpleStore;
					}
					case CodeNodeType.ExplicitCastExpression:
					{
						return !(node as ExplicitCastExpression).IsExplicitInterfaceCast;
					}
					case CodeNodeType.ImplicitCastExpression:
					case CodeNodeType.SafeCastExpression:
					case CodeNodeType.CanCastExpression:
					case CodeNodeType.ConditionExpression:
					case CodeNodeType.FixedStatement:
					case CodeNodeType.DefaultObjectExpression:
					case CodeNodeType.TypeReferenceExpression:
					case CodeNodeType.UsingStatement:
					case CodeNodeType.SizeOfExpression:
					case CodeNodeType.MakeRefExpression:
					case CodeNodeType.EnumExpression:
					case CodeNodeType.YieldReturnExpression:
					case CodeNodeType.YieldBreakExpression:
					case CodeNodeType.LockStatement:
					case CodeNodeType.ReturnExpression:
					case CodeNodeType.EmptyStatement:
					case CodeNodeType.AnonymousPropertyInitializerExpression:
					case CodeNodeType.LambdaParameterExpression:
					{
						return false;
					}
					case CodeNodeType.TypeOfExpression:
					case CodeNodeType.ArrayCreationExpression:
					case CodeNodeType.ObjectCreationExpression:
					case CodeNodeType.StackAllocExpression:
					case CodeNodeType.EventReferenceExpression:
					case CodeNodeType.LambdaExpression:
					case CodeNodeType.DelegateInvokeExpression:
					case CodeNodeType.BaseCtorExpression:
					case CodeNodeType.ThisCtorExpression:
					case CodeNodeType.DynamicMemberReferenceExpression:
					case CodeNodeType.DynamicConstructorInvocationExpression:
					case CodeNodeType.DynamicIndexerExpression:
					case CodeNodeType.BoxExpression:
					case CodeNodeType.AwaitExpression:
					case CodeNodeType.ArrayLengthExpression:
					{
						break;
					}
					case CodeNodeType.ArrayIndexerExpression:
					{
						return !(node as ArrayIndexerExpression).IsSimpleStore;
					}
					case CodeNodeType.PropertyReferenceExpression:
					{
						return !(node as PropertyReferenceExpression).IsSetter;
					}
					default:
					{
						if (codeNodeType != CodeNodeType.AnonymousObjectCreationExpression)
						{
							return false;
						}
						else
						{
							break;
						}
					}
				}
			}
			else
			{
				if (codeNodeType == CodeNodeType.MethodInvocationExpression || codeNodeType == CodeNodeType.DelegateCreationExpression)
				{
					return true;
				}
				return false;
			}
			return true;
		}

		public bool HasSideEffectsRecursive(ICodeNode node)
		{
			this.hasSideEffects = false;
			this.Visit(node);
			return this.hasSideEffects;
		}

		public override void Visit(ICodeNode node)
		{
			this.hasSideEffects = SideEffectsFinder.HasSideEffects(node);
			if (!this.hasSideEffects)
			{
				base.Visit(node);
			}
		}
	}
}