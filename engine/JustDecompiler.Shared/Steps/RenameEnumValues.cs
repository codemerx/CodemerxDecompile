#region license
//
//	(C) 2007 - 2008 Novell, Inc. http://www.novell.com
//	(C) 2007 - 2008 Jb Evain http://evain.net
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion
using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Mono.Cecil;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Ast.Expressions;
using Mono.Cecil.Extensions;

namespace Telerik.JustDecompiler.Steps
{
    public class RenameEnumValues : BaseCodeTransformer, IDecompilationStep
    {
        private TypeSystem typeSystem;
		private DecompilationContext context;

		public BlockStatement Process(DecompilationContext context, BlockStatement block)
		{
            this.typeSystem = context.MethodContext.Method.Module.TypeSystem;
			this.context = context;
            BlockStatement newBlock = (BlockStatement)VisitBlockStatement(block);
            return newBlock;
		}

		public override ICodeNode VisitBaseCtorExpression(BaseCtorExpression node)
		{
			node = (BaseCtorExpression)base.VisitBaseCtorExpression(node);
			VisitInvocationArguments(node.Arguments, node.MethodExpression.Method);
			return node;
		}

		public override ICodeNode VisitThisCtorExpression(ThisCtorExpression node)
		{
			node = (ThisCtorExpression)base.VisitThisCtorExpression(node);
			VisitInvocationArguments(node.Arguments, node.MethodExpression.Method);
			return node;
		}

        public override ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
        {
            node = (MethodInvocationExpression)base.VisitMethodInvocationExpression(node);

            VisitInvocationArguments(node.Arguments, node.MethodExpression.Method);

            if (node.IsConstrained)
            {
                if (node.MethodExpression.Target.CodeNodeType == CodeNodeType.LiteralExpression)
                {
                    TypeDefinition constraintTypeDef = node.ConstraintType.Resolve();
                    if (constraintTypeDef.IsEnum)
                    {
                        node.MethodExpression.Target = EnumHelper.GetEnumExpression(constraintTypeDef, node.MethodExpression.Target as LiteralExpression, typeSystem);
                    }
                }
            }

            return node;
        }

        private void VisitInvocationArguments(ExpressionCollection arguments, MethodReference method)
        {
			Collection<ParameterDefinition> parameters = method.Parameters;
            for (int index = 0; index < arguments.Count; index++)
            {
				TypeReference paramType = parameters[index].ResolveParameterType(method);
                if (NeedsCast(arguments[index].ExpressionType, paramType))
                {
                    if (arguments[index].CodeNodeType == CodeNodeType.LiteralExpression)
                    {
						arguments[index] = EnumHelper.GetEnumExpression(paramType.Resolve(), (arguments[index] as LiteralExpression), typeSystem);
                    }
					else
					{
						arguments[index] = new ExplicitCastExpression(arguments[index], paramType, null);
					}
                }
            }
        }

        /// <summary>
        /// Only works when <paramref name="to"/> is enum type. Returns false otherwise.
        /// </summary>
        /// <param name="from">Original type.</param>
        /// <param name="to">Target type.</param>
        /// <returns>Returns true, if <paramref name="from"/> doesn't match <paramref name="to"/>.</returns>
        private bool NeedsCast(TypeReference from, TypeReference to)
        {
            TypeDefinition toDef = to.Resolve();
            if (toDef == null || from == null || !toDef.IsEnum || to.IsArray)
            {
                return false;
            }
            return from.FullName != to.FullName;
        }

        public override ICodeNode VisitSwitchStatement(SwitchStatement node)
        {
            node.Condition = (Expression)Visit(node.Condition);
            foreach (SwitchCase @case in node.Cases)
            {
                if (@case is ConditionCase)
                {
                    ConditionCase conditionCase = @case as ConditionCase;
                    if (NeedsCast(conditionCase.Condition.ExpressionType, node.Condition.ExpressionType))
                    {
                        if (conditionCase.Condition is LiteralExpression)
                        {
							conditionCase.Condition = EnumHelper.GetEnumExpression(node.Condition.ExpressionType.Resolve(),
													(conditionCase.Condition as LiteralExpression), typeSystem);
                        }
                        else
                        {
                            conditionCase.Condition = new ExplicitCastExpression(conditionCase.Condition, node.Condition.ExpressionType, null);
                        }
                    }
                }
            }
            node = (SwitchStatement)base.VisitSwitchStatement(node);
            return node;
        }

        public override ICodeNode VisitBinaryExpression(BinaryExpression node)
        {
            node = (BinaryExpression)base.VisitBinaryExpression(node);

            if (!node.Left.HasType)
            {
                /// Tests, where the left part of the expression is event fail.
                return node;
            }
            TypeDefinition leftType = node.Left.ExpressionType.Resolve();
            if (leftType == null)
            {
                return node;
            }
			if (leftType.IsEnum && !node.Left.ExpressionType.IsArray)
			{
				if (node.Right is LiteralExpression)
				{
					node.Right = EnumHelper.GetEnumExpression(leftType, (node.Right as LiteralExpression), typeSystem);
				}
				else if (node.Right is ExplicitCastExpression && (node.Right as ExplicitCastExpression).Expression is LiteralExpression)
				{
					node.Right = EnumHelper.GetEnumExpression(leftType, (node.Right as ExplicitCastExpression).Expression as LiteralExpression, typeSystem);
				}
			}
			else
			{
				if (!node.Right.HasType)
				{
					// Tests, where the right part of the expression is delegate fail.
					return node;
				}
				TypeDefinition rightType = node.Right.ExpressionType.Resolve();
				if (rightType == null)
				{
					return node;
				}
				if (rightType.IsEnum && !node.Right.ExpressionType.IsArray)
				{
					if (node.Left is LiteralExpression)
					{
						node.Left = EnumHelper.GetEnumExpression(rightType, (node.Left as LiteralExpression), typeSystem);
					}
					else if (node.Left is ExplicitCastExpression && (node.Left as ExplicitCastExpression).Expression is LiteralExpression)
					{
						node.Left = EnumHelper.GetEnumExpression(rightType, (node.Left as ExplicitCastExpression).Expression as LiteralExpression, typeSystem);
					}
				}
			}

            return node;
        }

        public override ICodeNode VisitObjectCreationExpression(ObjectCreationExpression node)
        {
            node = (ObjectCreationExpression)base.VisitObjectCreationExpression(node);
            if (node.Arguments.Count != 0)
            {
                VisitInvocationArguments(node.Arguments, node.Constructor);
            }
            return node;
        }

        public override ICodeNode VisitPropertyReferenceExpression(PropertyReferenceExpression node)
        {
            PropertyReferenceExpression result =(PropertyReferenceExpression) base.VisitPropertyReferenceExpression(node);
            if (node.Arguments.Count > 0)
            {
				VisitInvocationArguments(result.Arguments, result.MethodExpression.Method);
            }
            return result;
        }

        public override ICodeNode VisitExplicitCastExpression(ExplicitCastExpression node)
        {
            if (node.Expression.CodeNodeType == CodeNodeType.LiteralExpression)
            {
                TypeDefinition typeDef = node.ExpressionType.Resolve();
                if (typeDef != null && typeDef.IsEnum && !node.ExpressionType.IsArray)
                {
					return EnumHelper.GetEnumExpression(node.ExpressionType.Resolve(), node.Expression as LiteralExpression, typeSystem);
                }
                return node;
            }
            else
            {
                return base.VisitExplicitCastExpression(node);
            }
        }

		public override ICodeNode VisitReturnExpression(ReturnExpression node)
		{
			node = (ReturnExpression)base.VisitReturnExpression(node);

			TypeDefinition methodReturnType = context.MethodContext.Method.ReturnType.Resolve();
			if (methodReturnType == null)
			{
				return node;
			}
			if (node.Value == null)
			{
				// Covers methods returning void.
				return node;
			}
			if (methodReturnType.IsEnum && !context.MethodContext.Method.ReturnType.IsArray)
			{
				if (node.Value.ExpressionType == null || node.Value.ExpressionType.FullName != methodReturnType.FullName)
				{
					if (node.Value is LiteralExpression)
					{
						node.Value = EnumHelper.GetEnumExpression(methodReturnType, node.Value as LiteralExpression, typeSystem);
					}
					else if (node.Value is ExplicitCastExpression && (node.Value as ExplicitCastExpression).Expression is LiteralExpression)
					{
						node.Value = EnumHelper.GetEnumExpression(methodReturnType, (node.Value as ExplicitCastExpression).Expression as LiteralExpression, typeSystem);
					}
                    else if (node.Value.HasType && NeedsCast(node.Value.ExpressionType, methodReturnType))
					{
						node.Value = new ExplicitCastExpression(node.Value, methodReturnType, null);
					}
				}
			}

			return node;
		}

		public override ICodeNode VisitBoxExpression(BoxExpression node)
		{
			node = (BoxExpression)base.VisitBoxExpression(node);

			TypeDefinition boxedAs = node.BoxedAs.Resolve();

			if (boxedAs != null && boxedAs.IsEnum && !node.BoxedAs.IsArray)
			{
				if (node.BoxedExpression is LiteralExpression)
				{
					node.BoxedExpression = EnumHelper.GetEnumExpression(boxedAs, node.BoxedExpression as LiteralExpression, typeSystem);
				}
			}			

			return node;
		}
    }
}