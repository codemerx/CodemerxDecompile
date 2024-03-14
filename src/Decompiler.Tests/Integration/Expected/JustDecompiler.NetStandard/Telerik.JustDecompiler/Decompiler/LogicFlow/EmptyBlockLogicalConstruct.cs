using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
	internal class EmptyBlockLogicalConstruct : CFGBlockLogicalConstruct
	{
		private readonly int index;

		public override int Index
		{
			get
			{
				return this.index;
			}
		}

		public EmptyBlockLogicalConstruct(int index) : base(null, new List<Expression>())
		{
			this.index = index;
		}
	}
}