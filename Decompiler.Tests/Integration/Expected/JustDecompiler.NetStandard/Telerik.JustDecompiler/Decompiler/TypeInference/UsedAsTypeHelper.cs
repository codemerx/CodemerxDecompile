using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Decompiler.TypeInference
{
	internal class UsedAsTypeHelper
	{
		private readonly TypeSystem typeSystem;

		private readonly MethodSpecificContext methodContext;

		public UsedAsTypeHelper(MethodSpecificContext methodContext)
		{
			this.methodContext = methodContext;
			this.typeSystem = methodContext.Method.get_Module().get_TypeSystem();
		}

		public TypeReference GetUseExpressionTypeNode(Instruction instruction, Expression instructionExpression, VariableReference variable)
		{
			Code code = instruction.get_OpCode().get_Code();
			if (code == 112)
			{
				return instruction.get_Operand() as TypeReference;
			}
			if (UsedAsTypeHelper.IsConditionalBranch(code))
			{
				return this.typeSystem.get_Boolean();
			}
			if (code == 37)
			{
				return null;
			}
			return this.GetUseExpressionTypeNode(instructionExpression, variable);
		}

		private TypeReference GetUseExpressionTypeNode(Expression expression, VariableReference variable)
		{
			CodeNodeType codeNodeType = expression.CodeNodeType;
			if (codeNodeType > CodeNodeType.PropertyReferenceExpression)
			{
				if ((int)codeNodeType - (int)CodeNodeType.BaseCtorExpression <= (int)CodeNodeType.UnsafeBlock)
				{
					return this.GetUseInMethodInvocation(expression as MethodInvocationExpression, variable);
				}
				if (codeNodeType == CodeNodeType.ReturnExpression)
				{
					return this.methodContext.Method.get_FixedReturnType();
				}
				if (codeNodeType == CodeNodeType.BoxExpression)
				{
					return (expression as BoxExpression).BoxedAs;
				}
			}
			else
			{
				if (codeNodeType == CodeNodeType.ThrowExpression)
				{
					return null;
				}
				switch (codeNodeType)
				{
					case CodeNodeType.MethodInvocationExpression:
					{
						return this.GetUseInMethodInvocation(expression as MethodInvocationExpression, variable);
					}
					case CodeNodeType.MethodReferenceExpression:
					case CodeNodeType.DelegateCreationExpression:
					case CodeNodeType.LiteralExpression:
					case CodeNodeType.ArgumentReferenceExpression:
					case CodeNodeType.ThisReferenceExpression:
					case CodeNodeType.BaseReferenceExpression:
					case CodeNodeType.ImplicitCastExpression:
					{
						break;
					}
					case CodeNodeType.UnaryExpression:
					{
						return this.GetUseExpressionTypeNode((expression as UnaryExpression).Operand, variable);
					}
					case CodeNodeType.BinaryExpression:
					{
						return this.GetUseInBinaryExpression(expression as BinaryExpression, variable);
					}
					case CodeNodeType.VariableReferenceExpression:
					{
						VariableReferenceExpression variableReferenceExpression = expression as VariableReferenceExpression;
						if ((object)variableReferenceExpression.Variable != (object)variable)
						{
							return null;
						}
						return variableReferenceExpression.Variable.get_VariableType();
					}
					case CodeNodeType.VariableDeclarationExpression:
					{
						return (expression as VariableDeclarationExpression).Variable.get_VariableType();
					}
					case CodeNodeType.FieldReferenceExpression:
					{
						return (expression as FieldReferenceExpression).Field.get_DeclaringType();
					}
					case CodeNodeType.ExplicitCastExpression:
					case CodeNodeType.SafeCastExpression:
					{
						return this.typeSystem.get_Object();
					}
					default:
					{
						switch (codeNodeType)
						{
							case CodeNodeType.ArrayCreationExpression:
							{
								return this.GetUseInArrayCreation(expression as ArrayCreationExpression, variable);
							}
							case CodeNodeType.ArrayIndexerExpression:
							{
								return this.GetUseInArrayIndexer(expression as ArrayIndexerExpression, variable);
							}
							case CodeNodeType.ObjectCreationExpression:
							{
								return this.GetUseInObjectCreation(expression as ObjectCreationExpression, variable);
							}
							case CodeNodeType.PropertyReferenceExpression:
							{
								return this.GetUseInMethodInvocation(expression as MethodInvocationExpression, variable);
							}
						}
						break;
					}
				}
			}
			throw new ArgumentOutOfRangeException("Expression is not evaluated to any type.");
		}

		private TypeReference GetUseInArrayCreation(ArrayCreationExpression arrayCreationExpression, VariableReference variable)
		{
			TypeReference num;
			foreach (Expression dimension in arrayCreationExpression.Dimensions)
			{
				if (!(dimension is VariableReferenceExpression) || (object)(dimension as VariableReferenceExpression).Variable != (object)variable)
				{
					continue;
				}
				num = this.typeSystem.get_Int32();
				return num;
			}
			using (IEnumerator<Expression> enumerator = arrayCreationExpression.Initializer.Expressions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Expression current = enumerator.Current;
					if (!(current is VariableReferenceExpression) || (object)(current as VariableReferenceExpression).Variable != (object)variable)
					{
						continue;
					}
					num = arrayCreationExpression.ElementType;
					return num;
				}
				throw new ArgumentOutOfRangeException("Expression is not evaluated to any type.");
			}
			return num;
		}

		private TypeReference GetUseInArrayIndexer(ArrayIndexerExpression arrayIndexerExpression, VariableReference variable)
		{
			TypeReference num;
			using (IEnumerator<Expression> enumerator = arrayIndexerExpression.Indices.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Expression current = enumerator.Current;
					if (!(current is VariableReferenceExpression) || (object)(current as VariableReferenceExpression).Variable != (object)variable)
					{
						continue;
					}
					num = this.typeSystem.get_Int32();
					return num;
				}
				return new TypeReference("System", "Array", this.typeSystem.get_Object().get_Module(), this.typeSystem.get_Object().get_Scope());
			}
			return num;
		}

		private TypeReference GetUseInBinaryExpression(BinaryExpression binaryExpression, VariableReference variable)
		{
			if (binaryExpression.Right.CodeNodeType == CodeNodeType.VariableReferenceExpression && (object)(binaryExpression.Right as VariableReferenceExpression).Variable == (object)variable)
			{
				return binaryExpression.Left.ExpressionType;
			}
			if (binaryExpression.Left is VariableReferenceExpression && (object)(binaryExpression.Left as VariableReferenceExpression).Variable == (object)variable)
			{
				return binaryExpression.Right.ExpressionType;
			}
			return this.GetUseExpressionTypeNode(binaryExpression.Left, variable) ?? this.GetUseExpressionTypeNode(binaryExpression.Right, variable);
		}

		private TypeReference GetUseInMethodInvocation(MethodInvocationExpression methodInvocationExpression, VariableReference variable)
		{
			Expression expression = null;
			foreach (Expression argument in methodInvocationExpression.Arguments)
			{
				if (!(argument is VariableReferenceExpression) || (object)(argument as VariableReferenceExpression).Variable != (object)variable)
				{
					continue;
				}
				expression = argument;
			}
			if (expression == null)
			{
				if ((object)(methodInvocationExpression.MethodExpression.Target as VariableReferenceExpression).Variable != (object)variable)
				{
					return null;
				}
				return methodInvocationExpression.MethodExpression.Member.get_DeclaringType();
			}
			int num = methodInvocationExpression.Arguments.IndexOf(expression);
			MethodReference method = methodInvocationExpression.MethodExpression.Method;
			return method.get_Parameters().get_Item(num).ResolveParameterType(method);
		}

		private TypeReference GetUseInObjectCreation(ObjectCreationExpression objectCreationExpression, VariableReference variable)
		{
			Expression expression = null;
			foreach (Expression argument in objectCreationExpression.Arguments)
			{
				if (!(argument is VariableReferenceExpression) || (object)(argument as VariableReferenceExpression).Variable != (object)variable)
				{
					continue;
				}
				expression = argument;
			}
			return objectCreationExpression.Constructor.get_Parameters().get_Item(objectCreationExpression.Arguments.IndexOf(expression)).ResolveParameterType(objectCreationExpression.Constructor);
		}

		private static bool IsConditionalBranch(Code instructionOpCode)
		{
			if (instructionOpCode == 57 || instructionOpCode == 44 || instructionOpCode == 56)
			{
				return true;
			}
			return instructionOpCode == 43;
		}
	}
}