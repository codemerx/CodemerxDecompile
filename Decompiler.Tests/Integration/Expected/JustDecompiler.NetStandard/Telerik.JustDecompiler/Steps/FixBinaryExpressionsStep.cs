using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Common;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class FixBinaryExpressionsStep : BaseCodeTransformer, IDecompilationStep
	{
		private readonly TypeSystem typeSystem;

		public FixBinaryExpressionsStep(TypeSystem typeSystem)
		{
			this.typeSystem = typeSystem;
		}

		private bool CheckForOverloadedEqualityOperators(Expression expression, out TypeReference unresolvedReference)
		{
			TypeReference typeReference;
			unresolvedReference = null;
			bool? overloadedEqualityOperator = Telerik.JustDecompiler.Common.Extensions.ResolveToOverloadedEqualityOperator(expression.ExpressionType, out typeReference);
			if (overloadedEqualityOperator.HasValue)
			{
				return overloadedEqualityOperator.Value;
			}
			unresolvedReference = typeReference;
			return true;
		}

		private ICodeNode FixBranchingExpression(Expression expression, Instruction branch)
		{
			BinaryExpression binaryExpression;
			Expression expression1;
			Expression unaryExpression;
			bool flag = (branch.OpCode.Code == Code.Brtrue ? true : branch.OpCode.Code == Code.Brtrue_S);
			TypeReference expressionType = expression.ExpressionType;
			BinaryOperator binaryOperator = BinaryOperator.ValueEquality;
			Instruction[] instructionArray = new Instruction[] { branch };
			if (flag)
			{
				binaryOperator = BinaryOperator.ValueInequality;
			}
			if (expressionType.Name == "Boolean" || expressionType.Name.Contains("Boolean "))
			{
				expression1 = (flag ? expression : Negator.Negate(expression, this.typeSystem));
				if (!(expression is SafeCastExpression))
				{
					unaryExpression = new UnaryExpression(UnaryOperator.None, expression1, instructionArray);
				}
				else
				{
					unaryExpression = new BinaryExpression(binaryOperator, expression, this.GetLiteralExpression(false, null), this.typeSystem, instructionArray, false);
				}
				return unaryExpression;
			}
			if (expressionType.Name == "Char")
			{
				binaryExpression = new BinaryExpression(binaryOperator, expression, this.GetLiteralExpression('\0', null), this.typeSystem, instructionArray, false)
				{
					ExpressionType = this.typeSystem.Boolean
				};
			}
			if (expressionType.IsPrimitive)
			{
				binaryExpression = new BinaryExpression(binaryOperator, expression, this.GetLiteralExpression(0, null), this.typeSystem, instructionArray, false)
				{
					ExpressionType = this.typeSystem.Boolean
				};
			}
			else
			{
				TypeDefinition typeDefinition = expressionType.Resolve();
				if (typeDefinition == null || !typeDefinition.IsEnum || expressionType.IsArray)
				{
					binaryExpression = new BinaryExpression(binaryOperator, expression, this.GetLiteralExpression(null, null), this.typeSystem, instructionArray, false)
					{
						ExpressionType = this.typeSystem.Boolean
					};
				}
				else
				{
					FieldDefinition fieldDefinition = null;
					foreach (FieldDefinition field in typeDefinition.Fields)
					{
						if (field.Constant == null || field.Constant.Value == null || !field.Constant.Value.Equals(0))
						{
							continue;
						}
						fieldDefinition = field;
						goto Label0;
					}
				Label0:
					binaryExpression = (fieldDefinition != null ? new BinaryExpression(binaryOperator, expression, new EnumExpression(fieldDefinition, null), this.typeSystem, instructionArray, false)
					{
						ExpressionType = this.typeSystem.Boolean
					} : new BinaryExpression(binaryOperator, expression, this.GetLiteralExpression(0, null), this.typeSystem, instructionArray, false)
					{
						ExpressionType = this.typeSystem.Boolean
					});
				}
			}
			return binaryExpression;
		}

		private ICodeNode FixEqualityComparisonExpression(BinaryExpression expression)
		{
			if (!(expression.Right is LiteralExpression))
			{
				return expression;
			}
			TypeReference elementType = this.GetElementType(expression.Left.ExpressionType);
			if (elementType.FullName != this.typeSystem.Boolean.FullName)
			{
				TypeDefinition typeDefinition = elementType.Resolve();
				if (elementType != null && !elementType.IsPrimitive && typeDefinition != null && !typeDefinition.IsEnum)
				{
					expression.Right = this.GetLiteralExpression(null, null);
				}
				return expression;
			}
			LiteralExpression right = expression.Right as LiteralExpression;
			if (!right.Value.Equals(0) && !right.Value.Equals(null))
			{
				return expression.Left;
			}
			return new UnaryExpression(UnaryOperator.LogicalNot, expression.Left, null);
		}

		private TypeReference GetElementType(TypeReference type)
		{
			if (type is IModifierType)
			{
				return (type as IModifierType).ElementType;
			}
			if (!(type is ByReferenceType))
			{
				return type;
			}
			return (type as ByReferenceType).ElementType;
		}

		private LiteralExpression GetLiteralExpression(object value, IEnumerable<Instruction> instructions)
		{
			return new LiteralExpression(value, this.typeSystem, instructions);
		}

		private bool IsArithmeticOperator(BinaryOperator @operator)
		{
			if (@operator == BinaryOperator.Add || @operator == BinaryOperator.Subtract || @operator == BinaryOperator.Multiply)
			{
				return true;
			}
			return @operator == BinaryOperator.Divide;
		}

		private bool IsBooleanAssignmentOperator(BinaryOperator @operator)
		{
			if (@operator == BinaryOperator.Assign || @operator == BinaryOperator.AndAssign || @operator == BinaryOperator.OrAssign)
			{
				return true;
			}
			return @operator == BinaryOperator.XorAssign;
		}

		private bool IsFalse(Expression expression)
		{
			LiteralExpression literalExpression = expression as LiteralExpression;
			if (literalExpression == null)
			{
				return false;
			}
			if (!literalExpression.Value.Equals(false) && !literalExpression.Value.Equals(0))
			{
				return false;
			}
			return true;
		}

		private bool NeedsPointerCast(BinaryExpression expression)
		{
			if (!expression.Left.ExpressionType.IsPointer && !expression.Left.ExpressionType.IsByReference)
			{
				return false;
			}
			return expression.Left.ExpressionType.GetElementType().FullName != expression.Right.ExpressionType.GetElementType().FullName;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			MethodSpecificContext methodContext = context.MethodContext;
			foreach (int key in methodContext.Expressions.BlockExpressions.Keys)
			{
				IList<Expression> item = methodContext.Expressions.BlockExpressions[key];
				OpCode opCode = methodContext.ControlFlowGraph.InstructionToBlockMapping[key].Last.OpCode;
				Code code = opCode.Code;
				bool flag = (code == Code.Brtrue || code == Code.Brtrue_S || code == Code.Brfalse ? true : code == Code.Brfalse_S);
				for (int i = 0; i < item.Count; i++)
				{
					item[i] = (Expression)this.Visit(item[i]);
				}
				if (!flag)
				{
					continue;
				}
				item[item.Count - 1] = (Expression)this.FixBranchingExpression(item[item.Count - 1], methodContext.ControlFlowGraph.InstructionToBlockMapping[key].Last);
			}
			return body;
		}

		private Expression SimplifyBooleanComparison(BinaryExpression expression)
		{
			bool flag = this.IsFalse(expression.Right);
			if ((!flag || expression.Operator != BinaryOperator.ValueEquality) && (flag || expression.Operator != BinaryOperator.ValueInequality))
			{
				return expression;
			}
			List<Instruction> instructions = new List<Instruction>(expression.MappedInstructions);
			instructions.AddRange(expression.Right.UnderlyingSameMethodInstructions);
			return Negator.Negate(expression.Left, this.typeSystem).CloneAndAttachInstructions(instructions);
		}

		public override ICodeNode VisitBinaryExpression(BinaryExpression expression)
		{
			TypeReference typeReference;
			TypeReference typeReference1;
			expression.Left = (Expression)this.Visit(expression.Left);
			expression.Right = (Expression)this.Visit(expression.Right);
			TypeReference expressionType = expression.Left.ExpressionType;
			expressionType = this.GetElementType(expressionType);
			if (expressionType != null)
			{
				if (expressionType.FullName == this.typeSystem.Char.FullName)
				{
					if (expression.Right.CodeNodeType == CodeNodeType.LiteralExpression)
					{
						if (this.IsArithmeticOperator(expression.Operator))
						{
							expression.ExpressionType = this.typeSystem.Char;
							return expression;
						}
						if (expression.Right.HasType)
						{
							TypeReference elementType = this.GetElementType(expression.Right.ExpressionType);
							if (expressionType.FullName == elementType.FullName)
							{
								return expression;
							}
						}
						LiteralExpression right = expression.Right as LiteralExpression;
						expression.Right = this.GetLiteralExpression((char)((Int32)right.Value), right.MappedInstructions);
					}
					if (this.GetElementType(expression.Right.ExpressionType).FullName != this.typeSystem.Char.FullName)
					{
						if (expression.Right.CodeNodeType != CodeNodeType.ExplicitCastExpression || !(expression.Right.ExpressionType.FullName == this.typeSystem.UInt16.FullName))
						{
							expression.Right = new ExplicitCastExpression(expression.Right, this.typeSystem.Char, null);
						}
						else
						{
							((ExplicitCastExpression)expression.Right).TargetType = this.typeSystem.Char;
						}
					}
				}
				if (expressionType.FullName == this.typeSystem.Boolean.FullName && expression.Right.CodeNodeType == CodeNodeType.LiteralExpression)
				{
					if (expression.Operator == BinaryOperator.ValueEquality || expression.Operator == BinaryOperator.ValueInequality || this.IsBooleanAssignmentOperator(expression.Operator))
					{
						LiteralExpression literalExpression = expression.Right as LiteralExpression;
						bool flag = true;
						if (literalExpression.Value == null || literalExpression.Value.Equals(0) || literalExpression.Value.Equals(false) || literalExpression.Value.Equals(null))
						{
							flag = false;
						}
						expression.Right = this.GetLiteralExpression(flag, literalExpression.MappedInstructions);
					}
					if (expression.Operator == BinaryOperator.ValueEquality || expression.Operator == BinaryOperator.ValueInequality)
					{
						return this.SimplifyBooleanComparison(expression);
					}
				}
			}
			if (expression.Operator == BinaryOperator.ValueEquality || expression.Operator == BinaryOperator.ValueInequality)
			{
				TypeReference elementType1 = this.GetElementType(expression.Right.ExpressionType);
				if (elementType1 != null && expressionType != null && elementType1.FullName != expressionType.FullName)
				{
					return this.FixEqualityComparisonExpression(expression);
				}
			}
			if (expression.IsAssignmentExpression)
			{
				if (this.NeedsPointerCast(expression))
				{
					if (expression.Right.CodeNodeType != CodeNodeType.StackAllocExpression)
					{
						expression.Right = new ExplicitCastExpression(expression.Right, expression.Left.ExpressionType, null);
					}
					else
					{
						expression.Right.ExpressionType = expression.Left.ExpressionType;
					}
				}
				else if (expression.Left.HasType && (expression.Left.ExpressionType.IsByReference || expression.Left.ExpressionType.IsPointer || expression.Left.ExpressionType.IsArray || !expression.Left.ExpressionType.IsPrimitive))
				{
					TypeDefinition typeDefinition = expression.Left.ExpressionType.Resolve();
					if (typeDefinition != null && !typeDefinition.IsEnum && expression.Right is LiteralExpression)
					{
						LiteralExpression right1 = expression.Right as LiteralExpression;
						if (right1.Value != null && right1.Value.Equals(0))
						{
							expression.Right = new LiteralExpression(null, this.typeSystem, expression.Right.UnderlyingSameMethodInstructions);
						}
					}
				}
			}
			if (expression.Operator == BinaryOperator.GreaterThan && expression.MappedInstructions.Count<Instruction>() == 1 && expression.MappedInstructions.First<Instruction>().OpCode.Code == Code.Cgt_Un)
			{
				LiteralExpression literalExpression1 = null;
				if (expression.Right.CodeNodeType == CodeNodeType.LiteralExpression)
				{
					literalExpression1 = expression.Right as LiteralExpression;
				}
				else if (expression.Right.CodeNodeType == CodeNodeType.ExplicitCastExpression)
				{
					ExplicitCastExpression explicitCastExpression = expression.Right as ExplicitCastExpression;
					if (explicitCastExpression.Expression.CodeNodeType == CodeNodeType.LiteralExpression)
					{
						literalExpression1 = explicitCastExpression.Expression as LiteralExpression;
					}
				}
				if (literalExpression1 != null && (literalExpression1.Value == null || literalExpression1.Value.Equals(0)))
				{
					expression.Operator = BinaryOperator.ValueInequality;
				}
			}
			if (expression.IsObjectComparison)
			{
				Expression left = expression.Left;
				Expression expression1 = expression.Right;
				if (this.CheckForOverloadedEqualityOperators(expression.Left, out typeReference) && this.CheckForOverloadedEqualityOperators(expression.Right, out typeReference1))
				{
					expression.Left = new ExplicitCastExpression(left, left.ExpressionType.Module.TypeSystem.Object, null, typeReference);
					expression.Right = new ExplicitCastExpression(expression1, expression1.ExpressionType.Module.TypeSystem.Object, null, typeReference1);
				}
			}
			return expression;
		}
	}
}