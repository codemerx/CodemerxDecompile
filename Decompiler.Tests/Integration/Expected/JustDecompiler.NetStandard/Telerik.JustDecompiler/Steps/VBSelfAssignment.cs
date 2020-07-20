using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Steps
{
	public class VBSelfAssignment : SelfAssignment
	{
		public VBSelfAssignment()
		{
			base();
			return;
		}

		protected override Dictionary<BinaryOperator, BinaryOperator> InitializeNormalToAssignOperatorMap()
		{
			stackVariable0 = new Dictionary<BinaryOperator, BinaryOperator>();
			stackVariable0.Add(1, 2);
			stackVariable0.Add(3, 4);
			stackVariable0.Add(5, 6);
			stackVariable0.Add(7, 8);
			stackVariable0.Add(17, 18);
			stackVariable0.Add(19, 20);
			return stackVariable0;
		}
	}
}