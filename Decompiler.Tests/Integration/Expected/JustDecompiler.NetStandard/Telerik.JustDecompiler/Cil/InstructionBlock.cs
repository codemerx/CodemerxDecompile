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

		private InstructionBlock[] successors = InstructionBlock.NoSuccessors;

		private HashSet<InstructionBlock> parents = new HashSet<InstructionBlock>();

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
			}
		}

		static InstructionBlock()
		{
			InstructionBlock.NoSuccessors = new InstructionBlock[0];
		}

		internal InstructionBlock(Instruction first)
		{
			if (first == null)
			{
				throw new ArgumentNullException("first");
			}
			this.first = first;
			this.StackContentAfterLeavingBlock = new Stack<Expression>();
		}

		public void AddToSuccessors(InstructionBlock toBeAdded)
		{
			InstructionBlock[] instructionBlockArrays = new InstructionBlock[(int)this.Successors.Length + 1];
			this.Successors.CopyTo(instructionBlockArrays, 0);
			instructionBlockArrays[(int)instructionBlockArrays.Length - 1] = toBeAdded;
			this.Successors = instructionBlockArrays;
		}

		public int CompareTo(InstructionBlock block)
		{
			return this.first.Offset - block.First.Offset;
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
			if (x.first.Offset != y.First.Offset)
			{
				return false;
			}
			return x.Last.Offset == y.Last.Offset;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is InstructionBlock))
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
			visited.Add(block.Index);
			yield return block;
			InstructionBlock[] instructionBlockArrays = block.Successors;
			if (instructionBlockArrays.Length != 0)
			{
				for (int i = 0; i < (int)instructionBlockArrays.Length; i++)
				{
					if (instructionBlockArrays[i].Index > block.Index && !visited.Contains(instructionBlockArrays[i].Index))
					{
						foreach (InstructionBlock childTree in InstructionBlock.GetChildTree(instructionBlockArrays[i], visited))
						{
							yield return childTree;
						}
					}
				}
			}
			else
			{
			}
		}

		public IEnumerator<Instruction> GetEnumerator()
		{
			InstructionBlock instructionBlocks = null;
			Instruction next = instructionBlocks.first;
			while (true)
			{
				yield return next;
				if (next == instructionBlocks.last)
				{
					break;
				}
				next = next.Next;
			}
		}

		public int GetHashCode(InstructionBlock obj)
		{
			return this.first.Offset.GetHashCode();
		}

		private void Invalidate()
		{
			if (this.invalidated)
			{
				return;
			}
			this.invalidated = true;
			foreach (InstructionBlock parent in this.parents)
			{
				parent.Invalidate();
			}
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
			InstructionBlock[] successors = new InstructionBlock[(int)this.Successors.Length - 1];
			int num = 0;
			int num1 = 0;
			while (num < (int)this.Successors.Length)
			{
				if (this.Successors[num] != toBeRemoved)
				{
					int num2 = num1;
					num1 = num2 + 1;
					successors[num2] = this.Successors[num];
				}
				num++;
			}
			this.Successors = successors;
		}

		private void SetNewParent()
		{
			for (int i = 0; i < (int)this.successors.Length; i++)
			{
				this.successors[i].parents.Add(this);
			}
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private void UnsetOldParent()
		{
			if (this.successors != null)
			{
				for (int i = 0; i < (int)this.successors.Length; i++)
				{
					this.successors[i].parents.Remove(this);
				}
			}
		}
	}
}