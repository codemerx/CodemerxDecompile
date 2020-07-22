using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Cil
{
	public class InstructionBlock : IEnumerable<Instruction>, IEnumerable, IComparable<InstructionBlock>, IEqualityComparer<InstructionBlock>
	{
		private readonly static InstructionBlock[] NoSuccessors;

		private int index;

		private Instruction first;

		private Instruction last;

		private InstructionBlock[] successors;

		private HashSet<InstructionBlock> parents;

		private bool invalidated;

		private List<InstructionBlock> childTree;

		public Instruction First
		{
			get
			{
				return this.first;
			}
			internal set
			{
				this.first = value;
				return;
			}
		}

		public int Index
		{
			get
			{
				return this.index;
			}
			internal set
			{
				this.index = value;
				return;
			}
		}

		public Instruction Last
		{
			get
			{
				return this.last;
			}
			internal set
			{
				this.last = value;
				return;
			}
		}

		public HashSet<InstructionBlock> Predecessors
		{
			get
			{
				return this.parents;
			}
		}

		public Stack<Expression> StackContentAfterLeavingBlock
		{
			get;
			private set;
		}

		public InstructionBlock[] Successors
		{
			get
			{
				return this.successors;
			}
			internal set
			{
				this.UnsetOldParent();
				this.successors = value;
				this.SetNewParent();
				this.Invalidate();
				return;
			}
		}

		static InstructionBlock()
		{
			InstructionBlock.NoSuccessors = new InstructionBlock[0];
			return;
		}

		internal InstructionBlock(Instruction first)
		{
			this.successors = InstructionBlock.NoSuccessors;
			this.parents = new HashSet<InstructionBlock>();
			base();
			if (first == null)
			{
				throw new ArgumentNullException("first");
			}
			this.first = first;
			this.set_StackContentAfterLeavingBlock(new Stack<Expression>());
			return;
		}

		public void AddToSuccessors(InstructionBlock toBeAdded)
		{
			V_0 = new InstructionBlock[(int)this.get_Successors().Length + 1];
			this.get_Successors().CopyTo(V_0, 0);
			V_0[(int)V_0.Length - 1] = toBeAdded;
			this.set_Successors(V_0);
			return;
		}

		public int CompareTo(InstructionBlock block)
		{
			return this.first.get_Offset() - block.get_First().get_Offset();
		}

		public bool Equals(InstructionBlock x, InstructionBlock y)
		{
			if (x == null)
			{
				return y == null;
			}
			if (y == null)
			{
				return false;
			}
			if (x.first.get_Offset() != y.get_First().get_Offset())
			{
				return false;
			}
			return x.get_Last().get_Offset() == y.get_Last().get_Offset();
		}

		public override bool Equals(object obj)
		{
			if (obj == null || InstructionBlock.op_Equality(obj as InstructionBlock, null))
			{
				return false;
			}
			return this.Equals(this, (InstructionBlock)obj);
		}

		public bool Equals(InstructionBlock block)
		{
			if (block == null)
			{
				return false;
			}
			return this.Equals(this, block);
		}

		public List<InstructionBlock> GetChildTree()
		{
			if (this.childTree != null && !this.invalidated)
			{
				return this.childTree;
			}
			this.childTree = new List<InstructionBlock>(InstructionBlock.GetChildTree(this, new HashSet<int>()));
			if (this.invalidated)
			{
				this.invalidated = false;
			}
			return this.childTree;
		}

		protected internal static IEnumerable<InstructionBlock> GetChildTree(InstructionBlock block, HashSet<int> visited)
		{
			stackVariable1 = new InstructionBlock.u003cGetChildTreeu003ed__34(-2);
			stackVariable1.u003cu003e3__block = block;
			stackVariable1.u003cu003e3__visited = visited;
			return stackVariable1;
		}

		public IEnumerator<Instruction> GetEnumerator()
		{
			stackVariable1 = new InstructionBlock.u003cGetEnumeratoru003ed__37(0);
			stackVariable1.u003cu003e4__this = this;
			return stackVariable1;
		}

		public int GetHashCode(InstructionBlock obj)
		{
			return this.first.get_Offset().GetHashCode();
		}

		private void Invalidate()
		{
			if (this.invalidated)
			{
				return;
			}
			this.invalidated = true;
			V_0 = this.parents.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_0.get_Current().Invalidate();
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		public static bool operator ==(InstructionBlock a, InstructionBlock b)
		{
			if (a == null)
			{
				return b == null;
			}
			return a.Equals(b);
		}

		public static bool operator !=(InstructionBlock a, InstructionBlock b)
		{
			if (a == null)
			{
				return (object)b != (object)null;
			}
			return !a.Equals(b);
		}

		public void RemoveFromSuccessors(InstructionBlock toBeRemoved)
		{
			V_0 = new InstructionBlock[(int)this.get_Successors().Length - 1];
			V_1 = 0;
			V_2 = 0;
			while (V_1 < (int)this.get_Successors().Length)
			{
				if (InstructionBlock.op_Inequality(this.get_Successors()[V_1], toBeRemoved))
				{
					stackVariable24 = V_2;
					V_2 = stackVariable24 + 1;
					V_0[stackVariable24] = this.get_Successors()[V_1];
				}
				V_1 = V_1 + 1;
			}
			this.set_Successors(V_0);
			return;
		}

		private void SetNewParent()
		{
			V_0 = 0;
			while (V_0 < (int)this.successors.Length)
			{
				dummyVar0 = this.successors[V_0].parents.Add(this);
				V_0 = V_0 + 1;
			}
			return;
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private void UnsetOldParent()
		{
			if (this.successors != null)
			{
				V_0 = 0;
				while (V_0 < (int)this.successors.Length)
				{
					dummyVar0 = this.successors[V_0].parents.Remove(this);
					V_0 = V_0 + 1;
				}
			}
			return;
		}
	}
}