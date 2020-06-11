using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Mono.Cecil;
using Mono.Cecil.Extensions;

namespace Telerik.JustDecompiler.Decompiler
{
	/// <summary>
	/// This fixer is responsible for changing expressions, based on their usage. For instance, literal expression '1' should be changed to 'true', if it's used as boolean.
	/// This also handles casts to ushort, that should be casts to char (in IL both types are unsigned 2-byte integer).
	/// </summary>
    class UsageBasedExpressionFixer : BaseCodeTransformer
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
            this.isBoolReturnType = returnType.FullName == currentTypeSystem.Boolean.FullName;
            this.isCharReturnType = returnType.FullName == currentTypeSystem.Char.FullName;
        }

        public void FixLiterals()
        {
            foreach (IList<Expression> expressions in methodContext.Expressions.BlockExpressions.Values)
            {
                foreach (Expression expression in expressions)
                {
                    Visit(expression);
                }
            }
        }

        public override ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
        {
            base.VisitMethodInvocationExpression(node);

            FixArguments(node.MethodExpression.Method, node.Arguments);

            return node;
        }

        public override ICodeNode VisitObjectCreationExpression(ObjectCreationExpression node)
        {
            base.VisitObjectCreationExpression(node);

            FixArguments(node.Constructor, node.Arguments);

            return node;
        }

        public override ICodeNode VisitThisCtorExpression(ThisCtorExpression node)
        {
            base.VisitThisCtorExpression(node);

            FixArguments(node.MethodExpression.Method, node.Arguments);

            return node;
        }

        public override ICodeNode VisitBaseCtorExpression(BaseCtorExpression node)
        {
            base.VisitBaseCtorExpression(node);

            FixArguments(node.MethodExpression.Method, node.Arguments);

            return node;
        }

		private void FixArguments(MethodReference methodRef, ExpressionCollection arguments)
		{
            if (methodRef == null)
            {
                return;
            }

			for (int i = 0; i < arguments.Count; i++)
			{
				TypeReference parameterType = methodRef.Parameters[i].ResolveParameterType(methodRef); 
				LiteralExpression literalArgument = arguments[i] as LiteralExpression;
				if (literalArgument != null)
				{
					HandleLiteralArgument(parameterType, literalArgument);
				}

				ExplicitCastExpression castArgument = arguments[i] as ExplicitCastExpression;
				if (castArgument != null)
				{
					HandleCastArgument(parameterType, castArgument);
				}
			}
		}
  
		private void HandleCastArgument(TypeReference parameterType, ExplicitCastExpression castArgument)
		{
			if (parameterType.FullName == currentTypeSystem.Char.FullName && castArgument.ExpressionType.FullName == currentTypeSystem.UInt16.FullName)
			{
				castArgument.TargetType = currentTypeSystem.Char;
			}
		}

		private void HandleLiteralArgument(TypeReference parameterType, LiteralExpression literalArgument)
		{
			if (parameterType.FullName == currentTypeSystem.Boolean.FullName)
			{
				FixBooleanLiteral(literalArgument);
			}
			else if (parameterType.FullName == currentTypeSystem.Char.FullName)
			{
				FixCharLiteral(literalArgument);
			}
		}

        public override ICodeNode VisitReturnExpression(ReturnExpression node)
        {
            base.VisitReturnExpression(node);

            LiteralExpression literalValue = node.Value as LiteralExpression;
            if (literalValue != null)
            {
                if (isBoolReturnType)
                {
                    FixBooleanLiteral(literalValue);
                }
                else if (isCharReturnType)
                {
                    FixCharLiteral(literalValue);
                }
            }

			ExplicitCastExpression castValue = node.Value as ExplicitCastExpression;
			if (castValue != null)
			{
				if (castValue.ExpressionType.FullName != methodContext.Method.ReturnType.FullName)
				{
					if (isCharReturnType && castValue.ExpressionType.FullName == currentTypeSystem.UInt16.FullName)
					{
						castValue.TargetType = currentTypeSystem.Char;
					}
				}
			}

            return node;
        }

        public override ICodeNode VisitBoxExpression(BoxExpression node)
        {
            base.VisitBoxExpression(node);
            if (node.BoxedExpression.CodeNodeType == CodeNodeType.LiteralExpression && node.BoxedAs.FullName == currentTypeSystem.Boolean.FullName)
            {
                FixBooleanLiteral(node.BoxedExpression as LiteralExpression);
                return node.BoxedExpression.CloneAndAttachInstructions(node.MappedInstructions);
            }

			if (node.BoxedExpression.CodeNodeType == CodeNodeType.ExplicitCastExpression && ((ExplicitCastExpression)node.BoxedExpression).Expression.CodeNodeType == CodeNodeType.ExplicitCastExpression)
			{ 
				// double cast in a boxed expression;
				ExplicitCastExpression outerCast = node.BoxedExpression as ExplicitCastExpression;
				ExplicitCastExpression innerCast = outerCast.Expression as ExplicitCastExpression;
				if (outerCast.TargetType.FullName == currentTypeSystem.Char.FullName &&
					innerCast.TargetType.FullName == currentTypeSystem.UInt16.FullName)
				{
					// Remove the outer cast, as it is produced by the box expression and doesn't have any instructions mapped.
					// The inner cast contains the instruction, converting the stack value to 2-byte integer (which at this point is known to be char, not ushort).
					innerCast.TargetType = currentTypeSystem.Char;
					node.BoxedExpression = innerCast;
				}
			}
            return node;
        }

        public override ICodeNode VisitExplicitCastExpression(ExplicitCastExpression node)
        {
			if (node.Expression.CodeNodeType == CodeNodeType.LiteralExpression && node.TargetType.FullName == currentTypeSystem.Boolean.FullName)
            {
                FixBooleanLiteral(node.Expression as LiteralExpression);
                return node.Expression.CloneAndAttachInstructions(node.MappedInstructions);
            }

            if (node.Expression.CodeNodeType == CodeNodeType.LiteralExpression && node.TargetType.FullName == currentTypeSystem.Char.FullName)
            {
                FixCharLiteral(node.Expression as LiteralExpression);
                return node.Expression.CloneAndAttachInstructions(node.MappedInstructions);
            }

            return base.VisitExplicitCastExpression(node);
        }

        private void FixBooleanLiteral(LiteralExpression literal)
        {
            literal.Value = Convert.ToBoolean(literal.Value);
        }

        private void FixCharLiteral(LiteralExpression literal)
        {
            literal.Value = Convert.ToChar(literal.Value);
        }
    }
}
