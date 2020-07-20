using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public abstract class Expression : BaseCodeNode, IEquatable<Expression>
	{
		protected readonly List<Instruction> instructions;

		public virtual TypeReference ExpressionType
		{
			get;
			set;
		}

		public virtual bool HasType
		{
			get
			{
				return (object)this.get_ExpressionType() != (object)null;
			}
		}

		public IEnumerable<Instruction> MappedInstructions
		{
			get
			{
				return new List<Instruction>(this.instructions);
			}
		}

		protected Expression(IEnumerable<Instruction> instructions)
		{
			this.instructions = new List<Instruction>();
			base();
			if (instructions != null)
			{
				this.instructions.AddRange(instructions);
			}
			return;
		}

		public abstract Expression Clone();

		public Expression CloneAndAttachInstructions(IEnumerable<Instruction> instructions)
		{
			stackVariable1 = this.Clone();
			stackVariable1.instructions.AddRange(instructions);
			return stackVariable1;
		}

		public abstract Expression CloneExpressionOnly();

		public Expression CloneExpressionOnlyAndAttachInstructions(IEnumerable<Instruction> instructions)
		{
			stackVariable1 = this.CloneExpressionOnly();
			stackVariable1.instructions.AddRange(instructions);
			return stackVariable1;
		}

		public abstract bool Equals(Expression other);

		protected override IEnumerable<Instruction> GetOwnInstructions()
		{
			return this.instructions;
		}

		public void MapBranchInstructions(IEnumerable<Instruction> branchInstructions)
		{
			V_0 = branchInstructions.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (V_1.get_OpCode().get_Code() == 55 || V_1.get_OpCode().get_Code() == 42)
					{
						continue;
					}
					throw new InvalidOperationException("Only unconditional branch instructions are allowed");
				}
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
			this.instructions.AddRange(branchInstructions);
			return;
		}

		public void MapDupInstructions(IEnumerable<Instruction> dupInstructions)
		{
			V_0 = dupInstructions.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					if (V_0.get_Current().get_OpCode().get_Code() == 36)
					{
						continue;
					}
					throw new InvalidOperationException("Only dup instructions are allowed");
				}
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
			this.instructions.AddRange(dupInstructions);
			return;
		}
	}
}