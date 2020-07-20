using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	public class RenameEnumValues : BaseCodeTransformer, IDecompilationStep
	{
		private TypeSystem typeSystem;

		private DecompilationContext context;

		public RenameEnumValues()
		{
			base();
			return;
		}

		private bool NeedsCast(TypeReference from, TypeReference to)
		{
			V_0 = to.Resolve();
			if (V_0 == null || from == null || !V_0.get_IsEnum() || to.get_IsArray())
			{
				return false;
			}
			return String.op_Inequality(from.get_FullName(), to.get_FullName());
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement block)
		{
			this.typeSystem = context.get_MethodContext().get_Method().get_Module().get_TypeSystem();
			this.context = context;
			return (BlockStatement)this.VisitBlockStatement(block);
		}

		public override ICodeNode VisitBaseCtorExpression(BaseCtorExpression node)
		{
			node = (BaseCtorExpression)this.VisitBaseCtorExpression(node);
			this.VisitInvocationArguments(node.get_Arguments(), node.get_MethodExpression().get_Method());
			return node;
		}

		public override ICodeNode VisitBinaryExpression(BinaryExpression node)
		{
			node = (BinaryExpression)this.VisitBinaryExpression(node);
			if (!node.get_Left().get_HasType())
			{
				return node;
			}
			V_0 = node.get_Left().get_ExpressionType().Resolve();
			if (V_0 == null)
			{
				return node;
			}
			if (!V_0.get_IsEnum() || node.get_Left().get_ExpressionType().get_IsArray())
			{
				if (!node.get_Right().get_HasType())
				{
					return node;
				}
				V_1 = node.get_Right().get_ExpressionType().Resolve();
				if (V_1 == null)
				{
					return node;
				}
				if (V_1.get_IsEnum() && !node.get_Right().get_ExpressionType().get_IsArray())
				{
					if (node.get_Left() as LiteralExpression == null)
					{
						if (node.get_Left() as ExplicitCastExpression != null && (node.get_Left() as ExplicitCastExpression).get_Expression() as LiteralExpression != null)
						{
							node.set_Left(EnumHelper.GetEnumExpression(V_1, (node.get_Left() as ExplicitCastExpression).get_Expression() as LiteralExpression, this.typeSystem));
						}
					}
					else
					{
						node.set_Left(EnumHelper.GetEnumExpression(V_1, node.get_Left() as LiteralExpression, this.typeSystem));
					}
				}
			}
			else
			{
				if (node.get_Right() as LiteralExpression == null)
				{
					if (node.get_Right() as ExplicitCastExpression != null && (node.get_Right() as ExplicitCastExpression).get_Expression() as LiteralExpression != null)
					{
						node.set_Right(EnumHelper.GetEnumExpression(V_0, (node.get_Right() as ExplicitCastExpression).get_Expression() as LiteralExpression, this.typeSystem));
					}
				}
				else
				{
					node.set_Right(EnumHelper.GetEnumExpression(V_0, node.get_Right() as LiteralExpression, this.typeSystem));
				}
			}
			return node;
		}

		public override ICodeNode VisitBoxExpression(BoxExpression node)
		{
			node = (BoxExpression)this.VisitBoxExpression(node);
			V_0 = node.get_BoxedAs().Resolve();
			if (V_0 != null && V_0.get_IsEnum() && !node.get_BoxedAs().get_IsArray() && node.get_BoxedExpression() as LiteralExpression != null)
			{
				node.set_BoxedExpression(EnumHelper.GetEnumExpression(V_0, node.get_BoxedExpression() as LiteralExpression, this.typeSystem));
			}
			return node;
		}

		public override ICodeNode VisitExplicitCastExpression(ExplicitCastExpression node)
		{
			if (node.get_Expression().get_CodeNodeType() != 22)
			{
				return this.VisitExplicitCastExpression(node);
			}
			V_0 = node.get_ExpressionType().Resolve();
			if (V_0 == null || !V_0.get_IsEnum() || node.get_ExpressionType().get_IsArray())
			{
				return node;
			}
			return EnumHelper.GetEnumExpression(node.get_ExpressionType().Resolve(), node.get_Expression() as LiteralExpression, this.typeSystem);
		}

		private void VisitInvocationArguments(ExpressionCollection arguments, MethodReference method)
		{
			V_0 = method.get_Parameters();
			V_1 = 0;
			while (V_1 < arguments.get_Count())
			{
				V_2 = V_0.get_Item(V_1).ResolveParameterType(method);
				if (this.NeedsCast(arguments.get_Item(V_1).get_ExpressionType(), V_2))
				{
					if (arguments.get_Item(V_1).get_CodeNodeType() != 22)
					{
						arguments.set_Item(V_1, new ExplicitCastExpression(arguments.get_Item(V_1), V_2, null));
					}
					else
					{
						arguments.set_Item(V_1, EnumHelper.GetEnumExpression(V_2.Resolve(), arguments.get_Item(V_1) as LiteralExpression, this.typeSystem));
					}
				}
				V_1 = V_1 + 1;
			}
			return;
		}

		public override ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			node = (MethodInvocationExpression)this.VisitMethodInvocationExpression(node);
			this.VisitInvocationArguments(node.get_Arguments(), node.get_MethodExpression().get_Method());
			if (node.get_IsConstrained() && node.get_MethodExpression().get_Target().get_CodeNodeType() == 22)
			{
				V_0 = node.get_ConstraintType().Resolve();
				if (V_0.get_IsEnum())
				{
					node.get_MethodExpression().set_Target(EnumHelper.GetEnumExpression(V_0, node.get_MethodExpression().get_Target() as LiteralExpression, this.typeSystem));
				}
			}
			return node;
		}

		public override ICodeNode VisitObjectCreationExpression(ObjectCreationExpression node)
		{
			node = (ObjectCreationExpression)this.VisitObjectCreationExpression(node);
			if (node.get_Arguments().get_Count() != 0)
			{
				this.VisitInvocationArguments(node.get_Arguments(), node.get_Constructor());
			}
			return node;
		}

		public override ICodeNode VisitPropertyReferenceExpression(PropertyReferenceExpression node)
		{
			V_0 = (PropertyReferenceExpression)this.VisitPropertyReferenceExpression(node);
			if (node.get_Arguments().get_Count() > 0)
			{
				this.VisitInvocationArguments(V_0.get_Arguments(), V_0.get_MethodExpression().get_Method());
			}
			return V_0;
		}

		public override ICodeNode VisitReturnExpression(ReturnExpression node)
		{
			node = (ReturnExpression)this.VisitReturnExpression(node);
			V_0 = this.context.get_MethodContext().get_Method().get_ReturnType().Resolve();
			if (V_0 == null)
			{
				return node;
			}
			if (node.get_Value() == null)
			{
				return node;
			}
			if (V_0.get_IsEnum() && !this.context.get_MethodContext().get_Method().get_ReturnType().get_IsArray() && node.get_Value().get_ExpressionType() == null || String.op_Inequality(node.get_Value().get_ExpressionType().get_FullName(), V_0.get_FullName()))
			{
				if (node.get_Value() as LiteralExpression == null)
				{
					if (node.get_Value() as ExplicitCastExpression == null || (node.get_Value() as ExplicitCastExpression).get_Expression() as LiteralExpression == null)
					{
						if (node.get_Value().get_HasType() && this.NeedsCast(node.get_Value().get_ExpressionType(), V_0))
						{
							node.set_Value(new ExplicitCastExpression(node.get_Value(), V_0, null));
						}
					}
					else
					{
						node.set_Value(EnumHelper.GetEnumExpression(V_0, (node.get_Value() as ExplicitCastExpression).get_Expression() as LiteralExpression, this.typeSystem));
					}
				}
				else
				{
					node.set_Value(EnumHelper.GetEnumExpression(V_0, node.get_Value() as LiteralExpression, this.typeSystem));
				}
			}
			return node;
		}

		public override ICodeNode VisitSwitchStatement(SwitchStatement node)
		{
			node.set_Condition((Expression)this.Visit(node.get_Condition()));
			V_0 = node.get_Cases().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (V_1 as ConditionCase == null)
					{
						continue;
					}
					V_2 = V_1 as ConditionCase;
					if (!this.NeedsCast(V_2.get_Condition().get_ExpressionType(), node.get_Condition().get_ExpressionType()))
					{
						continue;
					}
					if (V_2.get_Condition() as LiteralExpression == null)
					{
						V_2.set_Condition(new ExplicitCastExpression(V_2.get_Condition(), node.get_Condition().get_ExpressionType(), null));
					}
					else
					{
						V_2.set_Condition(EnumHelper.GetEnumExpression(node.get_Condition().get_ExpressionType().Resolve(), V_2.get_Condition() as LiteralExpression, this.typeSystem));
					}
				}
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
			node = (SwitchStatement)this.VisitSwitchStatement(node);
			return node;
		}

		public override ICodeNode VisitThisCtorExpression(ThisCtorExpression node)
		{
			node = (ThisCtorExpression)this.VisitThisCtorExpression(node);
			this.VisitInvocationArguments(node.get_Arguments(), node.get_MethodExpression().get_Method());
			return node;
		}
	}
}