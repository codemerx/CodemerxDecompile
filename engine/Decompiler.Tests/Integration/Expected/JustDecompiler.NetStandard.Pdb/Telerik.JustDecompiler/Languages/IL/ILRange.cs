using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Telerik.JustDecompiler.Languages.IL
{
	internal class ILRange
	{
		public int From;

		public int To;

		public ILRange()
		{
			base();
			return;
		}

		public static IEnumerable<ILRange> Invert(IEnumerable<ILRange> input, int codeSize)
		{
			stackVariable1 = new ILRange.u003cInvertu003ed__4(-2);
			stackVariable1.u003cu003e3__input = input;
			stackVariable1.u003cu003e3__codeSize = codeSize;
			return stackVariable1;
		}

		public static List<ILRange> OrderAndJoint(IEnumerable<ILRange> input)
		{
			if (input == null)
			{
				throw new ArgumentNullException("Input is null!");
			}
			stackVariable1 = input;
			stackVariable2 = ILRange.u003cu003ec.u003cu003e9__3_0;
			if (stackVariable2 == null)
			{
				dummyVar0 = stackVariable2;
				stackVariable2 = new Func<ILRange, bool>(ILRange.u003cu003ec.u003cu003e9.u003cOrderAndJointu003eb__3_0);
				ILRange.u003cu003ec.u003cu003e9__3_0 = stackVariable2;
			}
			stackVariable3 = stackVariable1.Where<ILRange>(stackVariable2);
			stackVariable4 = ILRange.u003cu003ec.u003cu003e9__3_1;
			if (stackVariable4 == null)
			{
				dummyVar1 = stackVariable4;
				stackVariable4 = new Func<ILRange, int>(ILRange.u003cu003ec.u003cu003e9.u003cOrderAndJointu003eb__3_1);
				ILRange.u003cu003ec.u003cu003e9__3_1 = stackVariable4;
			}
			V_0 = stackVariable3.OrderBy<ILRange, int>(stackVariable4).ToList<ILRange>();
			V_1 = 0;
			while (V_1 < V_0.get_Count() - 1)
			{
				V_2 = V_0.get_Item(V_1);
				V_3 = V_0.get_Item(V_1 + 1);
				if (V_2.From > V_3.From || V_3.From > V_2.To)
				{
					V_1 = V_1 + 1;
				}
				else
				{
					V_2.To = Math.Max(V_2.To, V_3.To);
					V_0.RemoveAt(V_1 + 1);
				}
			}
			return V_0;
		}

		public override string ToString()
		{
			return String.Format("{0}-{1}", this.From.ToString("X"), this.To.ToString("X"));
		}
	}
}