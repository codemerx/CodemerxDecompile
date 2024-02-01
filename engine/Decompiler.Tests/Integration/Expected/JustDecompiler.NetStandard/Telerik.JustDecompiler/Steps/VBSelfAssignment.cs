using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Steps
{
	public class VBSelfAssignment : SelfAssignment
	{
		public VBSelfAssignment()
		{
		}

		protected override Dictionary<BinaryOperator, BinaryOperator> InitializeNormalToAssignOperatorMap()
		{
			return new Dictionary<BinaryOperator, BinaryOperator>()
			{
				{ BinaryOperator.Add, BinaryOperator.AddAssign },
				{ BinaryOperator.Subtract, BinaryOperator.SubtractAssign },
				{ BinaryOperator.Multiply, BinaryOperator.MultiplyAssign },
				{ BinaryOperator.Divide, BinaryOperator.DivideAssign },
				{ BinaryOperator.LeftShift, BinaryOperator.LeftShiftAssign },
				{ BinaryOperator.RightShift, BinaryOperator.RightShiftAssign }
			};
		}
	}
}