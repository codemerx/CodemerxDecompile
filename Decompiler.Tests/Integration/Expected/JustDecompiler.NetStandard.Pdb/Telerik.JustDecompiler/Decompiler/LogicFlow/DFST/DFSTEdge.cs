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
			base();
			this.set_Start(start);
			this.set_End(end);
			return;
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
			if (this.get_Start() != other.get_Start())
			{
				return false;
			}
			return this.get_End() == other.get_End();
		}

		public override int GetHashCode()
		{
			V_0 = this.get_Start().GetHashCode();
			V_1 = this.get_End().GetHashCode();
			V_2 = 32;
			V_3 = V_0 | V_1 & V_2 - 1;
			return V_0 << V_3 & 31 | V_0 >> V_2 - V_3 & 31 ^ V_1;
		}
	}
}