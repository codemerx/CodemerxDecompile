using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Common
{
	internal static class DynamicHelper
	{
		public static BinaryOperator GetBinaryOperator(ExpressionType @operator)
		{
			switch (@operator)
			{
				case ExpressionType.Add:
				case ExpressionType.AddChecked:
				{
					return BinaryOperator.Add;
				}
				case ExpressionType.And:
				{
					return BinaryOperator.BitwiseAnd;
				}
				case ExpressionType.AndAlso:
				{
					return BinaryOperator.LogicalAnd;
				}
				case ExpressionType.ArrayLength:
				case ExpressionType.ArrayIndex:
				case ExpressionType.Call:
				case ExpressionType.Conditional:
				case ExpressionType.Constant:
				case ExpressionType.Convert:
				case ExpressionType.ConvertChecked:
				case ExpressionType.Invoke:
				case ExpressionType.Lambda:
				case ExpressionType.ListInit:
				case ExpressionType.MemberAccess:
				case ExpressionType.MemberInit:
				{
					throw new Exception("Operator is not supported.");
				}
				case ExpressionType.Coalesce:
				{
					return BinaryOperator.NullCoalesce;
				}
				case ExpressionType.Divide:
				{
					return BinaryOperator.Divide;
				}
				case ExpressionType.Equal:
				{
					return BinaryOperator.ValueEquality;
				}
				case ExpressionType.ExclusiveOr:
				{
					return BinaryOperator.BitwiseXor;
				}
				case ExpressionType.GreaterThan:
				{
					return BinaryOperator.GreaterThan;
				}
				case ExpressionType.GreaterThanOrEqual:
				{
					return BinaryOperator.GreaterThanOrEqual;
				}
				case ExpressionType.LeftShift:
				{
					return BinaryOperator.LeftShift;
				}
				case ExpressionType.LessThan:
				{
					return BinaryOperator.LessThan;
				}
				case ExpressionType.LessThanOrEqual:
				{
					return BinaryOperator.LessThanOrEqual;
				}
				case ExpressionType.Modulo:
				{
					return BinaryOperator.Modulo;
				}
				case ExpressionType.Multiply:
				case ExpressionType.MultiplyChecked:
				{
					return BinaryOperator.Multiply;
				}
				default:
				{
					switch (@operator)
					{
						case ExpressionType.NotEqual:
						{
							return BinaryOperator.ValueInequality;
						}
						case ExpressionType.Or:
						{
							return BinaryOperator.BitwiseOr;
						}
						case ExpressionType.OrElse:
						{
							return BinaryOperator.LogicalOr;
						}
						case ExpressionType.Parameter:
						case ExpressionType.Power:
						case ExpressionType.Quote:
						case ExpressionType.TypeAs:
						case ExpressionType.TypeIs:
						{
							throw new Exception("Operator is not supported.");
						}
						case ExpressionType.RightShift:
						{
							return BinaryOperator.RightShift;
						}
						case ExpressionType.Subtract:
						case ExpressionType.SubtractChecked:
						{
							return BinaryOperator.Subtract;
						}
						case ExpressionType.Assign:
						{
							return BinaryOperator.Assign;
						}
						default:
						{
							switch (@operator)
							{
								case ExpressionType.AddAssign:
								case ExpressionType.AddAssignChecked:
								{
									return BinaryOperator.Add;
								}
								case ExpressionType.AndAssign:
								{
									return BinaryOperator.BitwiseAnd;
								}
								case ExpressionType.DivideAssign:
								{
									return BinaryOperator.Divide;
								}
								case ExpressionType.ExclusiveOrAssign:
								{
									return BinaryOperator.BitwiseXor;
								}
								case ExpressionType.LeftShiftAssign:
								{
									return BinaryOperator.LeftShift;
								}
								case ExpressionType.ModuloAssign:
								{
									return BinaryOperator.Modulo;
								}
								case ExpressionType.MultiplyAssign:
								case ExpressionType.MultiplyAssignChecked:
								{
									return BinaryOperator.Multiply;
								}
								case ExpressionType.OrAssign:
								{
									return BinaryOperator.BitwiseOr;
								}
								case ExpressionType.PowerAssign:
								{
									throw new Exception("Operator is not supported.");
								}
								case ExpressionType.RightShiftAssign:
								{
									return BinaryOperator.RightShift;
								}
								case ExpressionType.SubtractAssign:
								case ExpressionType.SubtractAssignChecked:
								{
									return BinaryOperator.Subtract;
								}
								default:
								{
									throw new Exception("Operator is not supported.");
								}
							}
							break;
						}
					}
					break;
				}
			}
		}

		public static bool[] GetDynamicPositioningFlags(CustomAttribute dynamicAttribute)
		{
			dynamicAttribute.Resolve();
			if (!dynamicAttribute.IsResolved)
			{
				throw new Exception("Could not resolve DynamicAttribute");
			}
			if (dynamicAttribute.ConstructorArguments.Count == 0)
			{
				return new Boolean[] { true };
			}
			if (dynamicAttribute.ConstructorArguments[0].Type.FullName != "System.Boolean[]")
			{
				throw new Exception("Invalid argument type for DynamicAttribute");
			}
			CustomAttributeArgument[] value = (CustomAttributeArgument[])dynamicAttribute.ConstructorArguments[0].Value;
			bool[] flagArray = new Boolean[(int)value.Length];
			for (int i = 0; i < (int)value.Length; i++)
			{
				flagArray[i] = (Boolean)value[i].Value;
			}
			return flagArray;
		}

		public static UnaryOperator GetUnaryOperator(ExpressionType @operator)
		{
			if (@operator > ExpressionType.Decrement)
			{
				if (@operator != ExpressionType.Increment)
				{
					switch (@operator)
					{
						case ExpressionType.PreIncrementAssign:
						{
							break;
						}
						case ExpressionType.PreDecrementAssign:
						{
							return UnaryOperator.PreDecrement;
						}
						case ExpressionType.PostIncrementAssign:
						{
							return UnaryOperator.PostIncrement;
						}
						case ExpressionType.PostDecrementAssign:
						{
							return UnaryOperator.PostDecrement;
						}
						case ExpressionType.TypeEqual:
						case ExpressionType.IsTrue:
						{
							throw new Exception("Operator is not supported.");
						}
						case ExpressionType.OnesComplement:
						{
							return UnaryOperator.BitwiseNot;
						}
						case ExpressionType.IsFalse:
						{
							return UnaryOperator.LogicalNot;
						}
						default:
						{
							throw new Exception("Operator is not supported.");
						}
					}
				}
				return UnaryOperator.PreIncrement;
			}
			else
			{
				switch (@operator)
				{
					case ExpressionType.Negate:
					case ExpressionType.NegateChecked:
					{
						return UnaryOperator.Negate;
					}
					case ExpressionType.UnaryPlus:
					{
						return UnaryOperator.UnaryPlus;
					}
					case ExpressionType.New:
					case ExpressionType.NewArrayInit:
					case ExpressionType.NewArrayBounds:
					{
						break;
					}
					case ExpressionType.Not:
					{
						return UnaryOperator.LogicalNot;
					}
					default:
					{
						if (@operator == ExpressionType.Decrement)
						{
							return UnaryOperator.PreDecrement;
						}
						break;
					}
				}
			}
			throw new Exception("Operator is not supported.");
		}
	}
}