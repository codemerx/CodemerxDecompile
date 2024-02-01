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
		}

		private IEnumerable<IEnumerable<Instruction>> GetInstructionEnumerables()
		{
			BaseCodeNode baseCodeNode = null;
			Queue<ICodeNode> codeNodes = new Queue<ICodeNode>();
			codeNodes.Enqueue(baseCodeNode);
			while (codeNodes.Count > 0)
			{
				BaseCodeNode baseCodeNode1 = codeNodes.Dequeue() as BaseCodeNode;
				yield return baseCodeNode1.GetOwnInstructions();
				foreach (ICodeNode child in baseCodeNode1.Children)
				{
					codeNodes.Enqueue(child);
				}
				baseCodeNode1 = null;
			}
		}

		protected abstract IEnumerable<Instruction> GetOwnInstructions();

		private ICollection<int> MergeSearchableEnumerables(IEnumerable<IEnumerable<Instruction>> enumerables)
		{
			HashSet<int> nums = new HashSet<int>();
			foreach (IEnumerable<Instruction> enumerable in enumerables)
			{
				foreach (Instruction instruction in enumerable)
				{
					nums.Add(instruction.get_Offset());
				}
			}
			return nums;
		}

		protected IEnumerable<Instruction> MergeSortedEnumerables(IEnumerable<IEnumerable<Instruction>> enumerables)
		{
			List<Instruction> instructions = new List<Instruction>();
			foreach (IEnumerable<Instruction> enumerable in enumerables)
			{
				instructions.AddRange(enumerable);
			}
			instructions.Sort((Instruction x, Instruction y) => x.get_Offset().CompareTo(y.get_Offset()));
			return instructions;
		}
	}
}