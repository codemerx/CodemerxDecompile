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
			base();
			return;
		}

		public static bool HasSideEffects(ICodeNode node)
		{
			if (node == null)
			{
				return false;
			}
			V_1 = node.get_CodeNodeType();
			if (V_1 > 21)
			{
				if (V_1 == 24)
				{
					V_0 = node as BinaryExpression;
					if (V_0.get_IsChecked() || V_0.get_Operator() == 7)
					{
						return true;
					}
					return V_0.get_Operator() == 24;
				}
				switch (V_1 - 30)
				{
					case 0:
					{
						return !(node as FieldReferenceExpression).get_IsSimpleStore();
					}
					case 1:
					{
						return !(node as ExplicitCastExpression).get_IsExplicitInterfaceCast();
					}
					case 2:
					case 3:
					case 4:
					case 6:
					case 7:
					case 11:
					case 13:
					case 14:
					case 16:
					case 17:
					case 19:
					case 24:
					case 25:
					case 26:
					case 27:
					case 28:
					case 33:
					case 34:
					{
						goto Label0;
					}
					case 5:
					case 8:
					case 10:
					case 15:
					case 18:
					case 20:
					case 21:
					case 22:
					case 23:
					case 29:
					case 30:
					case 31:
					case 32:
					case 35:
					case 36:
					{
						break;
					}
					case 9:
					{
						return !(node as ArrayIndexerExpression).get_IsSimpleStore();
					}
					case 12:
					{
						return !(node as PropertyReferenceExpression).get_IsSetter();
					}
					default:
					{
						if (V_1 != 72)
						{
							goto Label0;
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
				if (V_1 == 19 || V_1 == 21)
				{
					goto Label1;
				}
				goto Label0;
			}
		Label1:
			return true;
		Label0:
			return false;
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
				this.Visit(node);
			}
			return;
		}
	}
}