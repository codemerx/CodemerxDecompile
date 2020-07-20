using System;

namespace Telerik.JustDecompiler.Cil
{
	public class BlockRange : IEquatable<BlockRange>
	{
		public readonly InstructionBlock Start;

		public readonly InstructionBlock End;

		public BlockRange(InstructionBlock start, InstructionBlock end)
		{
			base();
			this.Start = start;
			this.End = end;
			return;
		}

		public bool Equals(BlockRange other)
		{
			if (!InstructionBlock.op_Equality(this.Start, other.Start))
			{
				return false;
			}
			return InstructionBlock.op_Equality(this.End, other.End);
		}

		public override bool Equals(object obj)
		{
			V_0 = obj as BlockRange;
			if (V_0 == null)
			{
				return false;
			}
			return this.Equals(V_0);
		}

		public override int GetHashCode()
		{
			return this.Start.get_Index() * this.End.get_Index();
		}
	}
}