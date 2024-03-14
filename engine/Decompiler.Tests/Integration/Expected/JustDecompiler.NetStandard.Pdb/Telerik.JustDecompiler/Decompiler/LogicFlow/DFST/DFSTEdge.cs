using System;
using System.Runtime.CompilerServices;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.DFST
{
	internal class DFSTEdge : IEquatable<DFSTEdge>
	{
		public DFSTNode End
		{
			get;
			private set;
		}

		public DFSTNode Start
		{
			get;
			private set;
		}

		public DFSTEdge(DFSTNode start, DFSTNode end)
		{
			this.Start = start;
			this.End = end;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as DFSTEdge);
		}

		public bool Equals(DFSTEdge other)
		{
			if (other == null)
			{
				return false;
			}
			if (this.Start != other.Start)
			{
				return false;
			}
			return this.End == other.End;
		}

		public override int GetHashCode()
		{
			uint hashCode = (uint)this.Start.GetHashCode();
			uint num = (uint)this.End.GetHashCode();
			int num1 = 32;
			int num2 = (hashCode | num) & num1 - 1;
			return (hashCode << (num2 & 31) | hashCode >> (num1 - num2 & 31)) ^ num;
		}
	}
}