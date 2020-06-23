using Mono.Cecil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
			this.methodContext = methodContext;
			TypeReference returnType = methodContext.Method.ReturnType;
			this.currentTypeSystem = methodContext.Method.Module.TypeSystem;
			this.isBoolReturnType = returnType.FullName == this.currentTypeSystem.Boolean.FullName;
			this.isCharReturnType = returnType.FullName == this.currentTypeSystem.Char.FullName;
		}

		private void FixArguments(MethodReference methodRef, ExpressionCollection arguments)
		{
			if (methodRef == null)
			{
				return;
			}
			for (int i = 0; i < arguments.Count; i++)
			{
				TypeReference typeReference = methodRef.Parameters[i].ResolveParameterType(methodRef);
				LiteralExpression item = arguments[i] as LiteralExpression;
				if (item != null)
				{
					this.HandleLiteralArgument(typeReference, item);
				}
				ExplicitCastExpression explicitCastExpression = arguments[i] as ExplicitCastExpression;
				if (explicitCastExpression != null)
				{
					this.HandleCastArgument(typeReference, explicitCastExpression);
				}
			}
		}

		private void FixBooleanLiteral(LiteralExpression literal)
		{
			literal.Value = Convert.ToBoolean(literal.Value);
		}

		private void FixCharLiteral(LiteralExpression literal)
		{
			literal.Value = Convert.ToChar(literal.Value);
		}

		public void FixLiterals()
		{
			foreach (IList<Expression> value in this.methodContext.Expressions.BlockExpressions.Values)
			{
				foreach (Expression expression in value)
				{
					this.Visit(expression);
				}
			}
		}

		private void HandleCastArgument(TypeReference parameterType, ExplicitCastExpression castArgument)
		{
			if (parameterType.FullName == this.currentTypeSystem.Char.FullName && castArgument.ExpressionType.FullName == this.currentTypeSystem.UInt16.FullName)
			{
				castArgument.TargetType = this.currentTypeSystem.Char;
			}
		}

		private void HandleLiteralArgument(TypeReference parameterType, LiteralExpression literalArgument)
		{
			if (parameterType.FullName == this.currentTypeSystem.Boolean.FullName)
			{
				this.FixBooleanLiteral(literalArgument);
				return;
			}
			if (parameterType.FullName == this.currentTypeSystem.Char.FullName)
			{
				this.FixCharLiteral(literalArgument);
			}
		}

		public override ICodeNode VisitBaseCtorExpression(BaseCtorExpression node)
		{
			base.VisitBaseCtorExpression(node);
			this.FixArguments(node.MethodExpression.Method, node.Arguments);
			return node;
		}

		public override ICodeNode VisitBoxExpression(BoxExpression node)
		{
			base.VisitBoxExpression(node);
			if (node.BoxedExpression.CodeNodeType == CodeNodeType.LiteralExpression && node.BoxedAs.FullName == this.currentTypeSystem.Boolean.FullName)
			{
				this.FixBooleanLiteral(node.BoxedExpression as LiteralExpression);
				return node.BoxedExpression.CloneAndAttachInstructions(node.MappedInstructions);
			}
			if (node.BoxedExpression.CodeNodeType == CodeNodeType.ExplicitCastExpression && ((ExplicitCastExpression)node.BoxedExpression).Expression.CodeNodeType == CodeNodeType.ExplicitCastExpression)
			{
				ExplicitCastExpression boxedExpression = node.BoxedExpression as ExplicitCastExpression;
				ExplicitCastExpression expression = boxedExpression.Expression as ExplicitCastExpression;
				if (boxedExpression.TargetType.FullName == this.currentTypeSystem.Char.FullName && expression.TargetType.FullName == this.currentTypeSystem.UInt16.FullName)
				{
					expression.TargetType = this.currentTypeSystem.Char;
					node.BoxedExpression = expression;
				}
			}
			return node;
		}

		public override ICodeNode VisitExplicitCastExpression(ExplicitCastExpression node)
		{
			if (node.Expression.CodeNodeType == CodeNodeType.LiteralExpression && node.TargetType.FullName == this.currentTypeSystem.Boolean.FullName)
			{
				this.FixBooleanLiteral(node.Expression as LiteralExpression);
				return node.Expression.CloneAndAttachInstructions(node.MappedInstructions);
			}
			if (node.Expression.CodeNodeType != CodeNodeType.LiteralExpression || !(node.TargetType.FullName == this.currentTypeSystem.Char.FullName))
			{
				return base.VisitExplicitCastExpression(node);
			}
			this.FixCharLiteral(node.Expression as LiteralExpression);
			return node.Expression.CloneAndAttachInstructions(node.MappedInstructions);
		}

		public override ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			base.VisitMethodInvocationExpression(node);
			this.FixArguments(node.MethodExpression.Method, node.Arguments);
			return node;
		}

		public override ICodeNode VisitObjectCreationExpression(ObjectCreationExpression node)
		{
			base.VisitObjectCreationExpression(node);
			this.FixArguments(node.Constructor, node.Arguments);
			return node;
		}

		public override ICodeNode VisitReturnExpression(ReturnExpression node)
		{
			base.VisitReturnExpression(node);
			LiteralExpression value = node.Value as LiteralExpression;
			if (value != null)
			{
				if (this.isBoolReturnType)
				{
					this.FixBooleanLiteral(value);
				}
				else if (this.isCharReturnType)
				{
					this.FixCharLiteral(value);
				}
			}
			ExplicitCastExpression chr = node.Value as ExplicitCastExpression;
			if (chr != null && chr.ExpressionType.FullName != this.methodContext.Method.ReturnType.FullName && this.isCharReturnType && chr.ExpressionType.FullName == this.currentTypeSystem.UInt16.FullName)
			{
				chr.TargetType = this.currentTypeSystem.Char;
			}
			return node;
		}

		public override ICodeNode VisitThisCtorExpression(ThisCtorExpression node)
		{
			base.VisitThisCtorExpression(node);
			this.FixArguments(node.MethodExpression.Method, node.Arguments);
			return node;
		}
	}
}