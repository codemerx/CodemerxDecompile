using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Telerik.JustDecompiler.Ast
{
	public abstract class BaseCodeNode : ICodeNode
	{
		public abstract IEnumerable<ICodeNode> Children
		{
			get;
		}

		public abstract Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get;
		}

		public ICollection<int> SearchableUnderlyingSameMethodInstructionOffsets
		{
			get
			{
				return this.MergeSearchableEnumerables(this.GetInstructionEnumerables());
			}
		}

		public IEnumerable<Instruction> UnderlyingSameMethodInstructions
		{
			get
			{
				return this.MergeSortedEnumerables(this.GetInstructionEnumerables());
			}
		}

		protected BaseCodeNode()
		{
			base();
			return;
		}

		private IEnumerable<IEnumerable<Instruction>> GetInstructionEnumerables()
		{
			stackVariable1 = new BaseCodeNode.u003cGetInstructionEnumerablesu003ed__8(-2);
			stackVariable1.u003cu003e4__this = this;
			return stackVariable1;
		}

		protected abstract IEnumerable<Instruction> GetOwnInstructions();

		private ICollection<int> MergeSearchableEnumerables(IEnumerable<IEnumerable<Instruction>> enumerables)
		{
			V_0 = new HashSet<int>();
			V_1 = enumerables.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current().GetEnumerator();
					try
					{
						while (V_2.MoveNext())
						{
							V_3 = V_2.get_Current();
							dummyVar0 = V_0.Add(V_3.get_Offset());
						}
					}
					finally
					{
						if (V_2 != null)
						{
							V_2.Dispose();
						}
					}
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			return V_0;
		}

		protected IEnumerable<Instruction> MergeSortedEnumerables(IEnumerable<IEnumerable<Instruction>> enumerables)
		{
			V_0 = new List<Instruction>();
			V_1 = enumerables.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_0.AddRange(V_2);
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			stackVariable9 = V_0;
			stackVariable10 = BaseCodeNode.u003cu003ec.u003cu003e9__6_0;
			if (stackVariable10 == null)
			{
				dummyVar0 = stackVariable10;
				stackVariable10 = new Comparison<Instruction>(BaseCodeNode.u003cu003ec.u003cu003e9.u003cMergeSortedEnumerablesu003eb__6_0);
				BaseCodeNode.u003cu003ec.u003cu003e9__6_0 = stackVariable10;
			}
			stackVariable9.Sort(stackVariable10);
			return V_0;
		}
	}
}