using System;

namespace Telerik.JustDecompiler.Cil
{
	public class BlockRange : IEquatable<BlockRange>
	{
		public readonly InstructionBlock Start;

		public readonly InstructionBlock End;

		public BlockRange(InstructionBlock start, InstructionBlock end)
		{
			this.Start = start;
			this.End = end;
		}

		public bool Equals(BlockRange other)
		{
			if (this.Start != other.Start)
			{
				return false;
			}
			return this.End == other.End;
		}

		public override bool Equals(object obj)
		{
			BlockRange blockRange = obj as BlockRange;
			if (blockRange == null)
			{
				return false;
			}
			return this.Equals(blockRange);
		}

		public override int GetHashCode()
		{
			return this.Start.Index * this.End.Index;
		}
	}
}