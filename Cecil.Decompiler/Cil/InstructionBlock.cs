#region license
//
//	(C) 2005 - 2007 db4objects Inc. http://www.db4o.com
//	(C) 2007 - 2008 Novell, Inc. http://www.novell.com
//	(C) 2007 - 2008 Jb Evain http://evain.net
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion

using System;
using System.Collections;
using System.Collections.Generic;

using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Cil
 {
	public class InstructionBlock : IEnumerable<Instruction>, IComparable<InstructionBlock>, IEqualityComparer<InstructionBlock>
	{
		static readonly InstructionBlock [] NoSuccessors = new InstructionBlock [0];

		int index;
		Instruction first;
		Instruction last;
		InstructionBlock [] successors = NoSuccessors;
		HashSet<InstructionBlock> parents = new HashSet<InstructionBlock>();
		bool invalidated = false;
		List<InstructionBlock> childTree = null;


        public Stack<Telerik.JustDecompiler.Ast.Expressions.Expression> StackContentAfterLeavingBlock { get;private set; }

		public bool Equals(InstructionBlock x, InstructionBlock y)
		{
			if ((object)x == null)
			{
				return (object)y == null;
			}

			if ((object)y == null)
				return false;

			return x.first.Offset == y.First.Offset && x.Last.Offset == y.Last.Offset;
		}

		public int GetHashCode(InstructionBlock obj)
		{
			return first.Offset.GetHashCode();
		}

		public int Index
		{
			get
			{
				return index;
			}
			internal set { index = value; }
		}

		public Instruction First {
			get { return first; }
			internal set { first = value; }
		}

		public Instruction Last {
			get { return last; }
			internal set { last = value; }

		}

		public List<InstructionBlock> GetChildTree()
		{
			if ((childTree == null) || invalidated)
			{
				childTree = new List<InstructionBlock>(GetChildTree(this, new HashSet<int>()));
				if (invalidated)
				{
					invalidated = false;
				}
				return childTree;
			}
			else
			{
				return childTree;
			}
		}

		private void Invalidate()
		{
			if (invalidated)
				return;

			invalidated = true;

			foreach (var parent in parents)
			{
				parent.Invalidate();
			}
		}

		public InstructionBlock[] Successors
		{
			get
			{
				return successors;
			}
			internal set
			{
				UnsetOldParent();
				successors = value;
				SetNewParent();
				Invalidate();
			}
		}

		public HashSet<InstructionBlock> Predecessors
		{
			get { return parents; }
		}

        // The AddToSuccessors and RemoveFromSuccessors are left this way, because they are used in only 1 place. They are written
        // this way so the whole Successors-SetParent thing going on in the Successors property. This implementation is very slow
        // O(n) on add. This should be refactored if planned to be used elsewhere. The refactoring should be changing the successors
        // collection to some kind of list and implementing observed list which does what it needs to be done on add and remove.
        public void AddToSuccessors(InstructionBlock toBeAdded)
        {
            InstructionBlock[] newSuccessors = new InstructionBlock[this.Successors.Length + 1];
            this.Successors.CopyTo(newSuccessors, 0);
            newSuccessors[newSuccessors.Length - 1] = toBeAdded;
            this.Successors = newSuccessors;
        }

        public void RemoveFromSuccessors(InstructionBlock toBeRemoved)
        {
            InstructionBlock[] newSuccessors = new InstructionBlock[this.Successors.Length - 1];
            for (int old = 0, @new = 0; old < this.Successors.Length; old++)
            {
                if (this.Successors[old] != toBeRemoved)
                {
                    newSuccessors[@new++] = this.Successors[old];
                }
            }

            this.Successors = newSuccessors;
        }

		private void SetNewParent()
		{
			for (int i = 0; i < successors.Length; i++)
			{
				successors[i].parents.Add(this);
			}

		}

		private void UnsetOldParent()
		{
			if (successors != null)
			{
				for (int i = 0; i < successors.Length; i++)
				{
					successors[i].parents.Remove(this);
				}
			}
		}

		protected internal static IEnumerable<InstructionBlock> GetChildTree(InstructionBlock block, HashSet<int> visited)
		{
			visited.Add(block.Index);
			yield return block;

			var successors = block.Successors;

			if (successors.Length == 0)
				yield break;

			for (int i = 0; i < successors.Length; i++)
			{
				if ((successors[i].Index > block.Index) && (!visited.Contains(successors[i].Index)))
					foreach (var nested in GetChildTree(successors[i], visited))
						yield return nested;
			}
		}

        internal InstructionBlock (Instruction first)
        {
			if (first == null)
				throw new ArgumentNullException ("first");

			this.first = first;
            StackContentAfterLeavingBlock = new Stack<Telerik.JustDecompiler.Ast.Expressions.Expression>();
        }

		public int CompareTo (InstructionBlock block)
		{
			return first.Offset - block.First.Offset;
		}

		public IEnumerator<Instruction> GetEnumerator ()
		{
			var instruction = first;
			while (true) {
				yield return instruction;

				if (instruction == last)
					yield break;

                instruction = instruction.Next;
			}
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}

		public override bool Equals(object obj)
		{
			if (obj == null || (obj as InstructionBlock) == null)
			{
				return false;
			}

			return Equals(this, (InstructionBlock)obj);
		}

		public bool Equals(InstructionBlock block)
		{
			if ((object)block == null)
			{
				return false;
			}

			return Equals(this, block);
		}

		public static bool operator ==(InstructionBlock a, InstructionBlock b)
		{
			if ((object)a == null)
			{
				return (object)b == null;
			}

			return a.Equals(b);
		}

		public static bool operator != (InstructionBlock a, InstructionBlock b)
		{
			if ((object)a == null)
			{
				return !((object)b == null);
			}

			return !a.Equals(b);
		}
	}
}
