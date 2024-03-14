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
		protected readonly List<Instruction> instructions = new List<Instruction>();

		public virtual TypeReference ExpressionType
		{
			get;
			set;
		}

		public virtual bool HasType
		{
			get
			{
				return (object)this.ExpressionType != (object)null;
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
			if (instructions != null)
			{
				this.instructions.AddRange(instructions);
			}
		}

		public abstract Expression Clone();

		public Expression CloneAndAttachInstructions(IEnumerable<Instruction> instructions)
		{
			Expression expression = this.Clone();
			expression.instructions.AddRange(instructions);
			return expression;
		}

		public abstract Expression CloneExpressionOnly();

		public Expression CloneExpressionOnlyAndAttachInstructions(IEnumerable<Instruction> instructions)
		{
			Expression expression = this.CloneExpressionOnly();
			expression.instructions.AddRange(instructions);
			return expression;
		}

		public abstract bool Equals(Expression other);

		protected override IEnumerable<Instruction> GetOwnInstructions()
		{
			return this.instructions;
		}

		public void MapBranchInstructions(IEnumerable<Instruction> branchInstructions)
		{
			foreach (Instruction branchInstruction in branchInstructions)
			{
				if (branchInstruction.get_OpCode().get_Code() == 55 || branchInstruction.get_OpCode().get_Code() == 42)
				{
					continue;
				}
				throw new InvalidOperationException("Only unconditional branch instructions are allowed");
			}
			this.instructions.AddRange(branchInstructions);
		}

		public void MapDupInstructions(IEnumerable<Instruction> dupInstructions)
		{
			foreach (Instruction dupInstruction in dupInstructions)
			{
				if (dupInstruction.get_OpCode().get_Code() == 36)
				{
					continue;
				}
				throw new InvalidOperationException("Only dup instructions are allowed");
			}
			this.instructions.AddRange(dupInstructions);
		}
	}
}