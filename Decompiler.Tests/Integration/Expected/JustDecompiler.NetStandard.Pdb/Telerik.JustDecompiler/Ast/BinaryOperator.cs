using System;

namespace Telerik.JustDecompiler.Ast
{
	public enum BinaryOperator
	{
		None,
		Add,
		AddAssign,
		Subtract,
		SubtractAssign,
		Multiply,
		MultiplyAssign,
		Divide,
		DivideAssign,
		ValueEquality,
		ValueInequality,
		LogicalOr,
		LogicalAnd,
		LessThan,
		LessThanOrEqual,
		GreaterThan,
		GreaterThanOrEqual,
		LeftShift,
		LeftShiftAssign,
		RightShift,
		RightShiftAssign,
		BitwiseOr,
		BitwiseAnd,
		BitwiseXor,
		Modulo,
		ModuloAssign,
		Assign,
		NullCoalesce,
		AndAssign,
		OrAssign,
		XorAssign
	}
}