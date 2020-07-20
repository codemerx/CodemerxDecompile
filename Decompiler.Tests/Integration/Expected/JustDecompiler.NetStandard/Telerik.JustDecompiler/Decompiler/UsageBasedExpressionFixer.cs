using Mono.Cecil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Decompiler
{
	internal class UsageBasedExpressionFixer : BaseCodeTransformer
	{
		private readonly MethodSpecificContext methodContext;

		private readonly TypeSystem currentTypeSystem;

		private readonly bool isBoolReturnType;

		private readonly bool isCharReturnType;

		public UsageBasedExpressionFixer(MethodSpecificContext methodContext)
		{
			base();
			this.methodContext = methodContext;
			V_0 = methodContext.get_Method().get_ReturnType();
			this.currentTypeSystem = methodContext.get_Method().get_Module().get_TypeSystem();
			this.isBoolReturnType = String.op_Equality(V_0.get_FullName(), this.currentTypeSystem.get_Boolean().get_FullName());
			this.isCharReturnType = String.op_Equality(V_0.get_FullName(), this.currentTypeSystem.get_Char().get_FullName());
			return;
		}

		private void FixArguments(MethodReference methodRef, ExpressionCollection arguments)
		{
			if (methodRef == null)
			{
				return;
			}
			V_0 = 0;
			while (V_0 < arguments.get_Count())
			{
				V_1 = methodRef.get_Parameters().get_Item(V_0).ResolveParameterType(methodRef);
				V_2 = arguments.get_Item(V_0) as LiteralExpression;
				if (V_2 != null)
				{
					this.HandleLiteralArgument(V_1, V_2);
				}
				V_3 = arguments.get_Item(V_0) as ExplicitCastExpression;
				if (V_3 != null)
				{
					this.HandleCastArgument(V_1, V_3);
				}
				V_0 = V_0 + 1;
			}
			return;
		}

		private void FixBooleanLiteral(LiteralExpression literal)
		{
			literal.set_Value(Convert.ToBoolean(literal.get_Value()));
			return;
		}

		private void FixCharLiteral(LiteralExpression literal)
		{
			literal.set_Value(Convert.ToChar(literal.get_Value()));
			return;
		}

		public void FixLiterals()
		{
			V_0 = this.methodContext.get_Expressions().get_BlockExpressions().get_Values().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current().GetEnumerator();
					try
					{
						while (V_1.MoveNext())
						{
							V_2 = V_1.get_Current();
							dummyVar0 = this.Visit(V_2);
						}
					}
					finally
					{
						if (V_1 != null)
						{
							V_1.Dispose();
						}
					}
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		private void HandleCastArgument(TypeReference parameterType, ExplicitCastExpression castArgument)
		{
			if (String.op_Equality(parameterType.get_FullName(), this.currentTypeSystem.get_Char().get_FullName()) && String.op_Equality(castArgument.get_ExpressionType().get_FullName(), this.currentTypeSystem.get_UInt16().get_FullName()))
			{
				castArgument.set_TargetType(this.currentTypeSystem.get_Char());
			}
			return;
		}

		private void HandleLiteralArgument(TypeReference parameterType, LiteralExpression literalArgument)
		{
			if (String.op_Equality(parameterType.get_FullName(), this.currentTypeSystem.get_Boolean().get_FullName()))
			{
				this.FixBooleanLiteral(literalArgument);
				return;
			}
			if (String.op_Equality(parameterType.get_FullName(), this.currentTypeSystem.get_Char().get_FullName()))
			{
				this.FixCharLiteral(literalArgument);
			}
			return;
		}

		public override ICodeNode VisitBaseCtorExpression(BaseCtorExpression node)
		{
			dummyVar0 = this.VisitBaseCtorExpression(node);
			this.FixArguments(node.get_MethodExpression().get_Method(), node.get_Arguments());
			return node;
		}

		public override ICodeNode VisitBoxExpression(BoxExpression node)
		{
			dummyVar0 = this.VisitBoxExpression(node);
			if (node.get_BoxedExpression().get_CodeNodeType() == 22 && String.op_Equality(node.get_BoxedAs().get_FullName(), this.currentTypeSystem.get_Boolean().get_FullName()))
			{
				this.FixBooleanLiteral(node.get_BoxedExpression() as LiteralExpression);
				return node.get_BoxedExpression().CloneAndAttachInstructions(node.get_MappedInstructions());
			}
			if (node.get_BoxedExpression().get_CodeNodeType() == 31 && ((ExplicitCastExpression)node.get_BoxedExpression()).get_Expression().get_CodeNodeType() == 31)
			{
				stackVariable20 = node.get_BoxedExpression() as ExplicitCastExpression;
				V_0 = stackVariable20.get_Expression() as ExplicitCastExpression;
				if (String.op_Equality(stackVariable20.get_TargetType().get_FullName(), this.currentTypeSystem.get_Char().get_FullName()) && String.op_Equality(V_0.get_TargetType().get_FullName(), this.currentTypeSystem.get_UInt16().get_FullName()))
				{
					V_0.set_TargetType(this.currentTypeSystem.get_Char());
					node.set_BoxedExpression(V_0);
				}
			}
			return node;
		}

		public override ICodeNode VisitExplicitCastExpression(ExplicitCastExpression node)
		{
			if (node.get_Expression().get_CodeNodeType() == 22 && String.op_Equality(node.get_TargetType().get_FullName(), this.currentTypeSystem.get_Boolean().get_FullName()))
			{
				this.FixBooleanLiteral(node.get_Expression() as LiteralExpression);
				return node.get_Expression().CloneAndAttachInstructions(node.get_MappedInstructions());
			}
			if (node.get_Expression().get_CodeNodeType() != 22 || !String.op_Equality(node.get_TargetType().get_FullName(), this.currentTypeSystem.get_Char().get_FullName()))
			{
				return this.VisitExplicitCastExpression(node);
			}
			this.FixCharLiteral(node.get_Expression() as LiteralExpression);
			return node.get_Expression().CloneAndAttachInstructions(node.get_MappedInstructions());
		}

		public override ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			dummyVar0 = this.VisitMethodInvocationExpression(node);
			this.FixArguments(node.get_MethodExpression().get_Method(), node.get_Arguments());
			return node;
		}

		public override ICodeNode VisitObjectCreationExpression(ObjectCreationExpression node)
		{
			dummyVar0 = this.VisitObjectCreationExpression(node);
			this.FixArguments(node.get_Constructor(), node.get_Arguments());
			return node;
		}

		public override ICodeNode VisitReturnExpression(ReturnExpression node)
		{
			dummyVar0 = this.VisitReturnExpression(node);
			V_0 = node.get_Value() as LiteralExpression;
			if (V_0 != null)
			{
				if (!this.isBoolReturnType)
				{
					if (this.isCharReturnType)
					{
						this.FixCharLiteral(V_0);
					}
				}
				else
				{
					this.FixBooleanLiteral(V_0);
				}
			}
			V_1 = node.get_Value() as ExplicitCastExpression;
			if (V_1 != null && String.op_Inequality(V_1.get_ExpressionType().get_FullName(), this.methodContext.get_Method().get_ReturnType().get_FullName()) && this.isCharReturnType && String.op_Equality(V_1.get_ExpressionType().get_FullName(), this.currentTypeSystem.get_UInt16().get_FullName()))
			{
				V_1.set_TargetType(this.currentTypeSystem.get_Char());
			}
			return node;
		}

		public override ICodeNode VisitThisCtorExpression(ThisCtorExpression node)
		{
			dummyVar0 = this.VisitThisCtorExpression(node);
			this.FixArguments(node.get_MethodExpression().get_Method(), node.get_Arguments());
			return node;
		}
	}
}