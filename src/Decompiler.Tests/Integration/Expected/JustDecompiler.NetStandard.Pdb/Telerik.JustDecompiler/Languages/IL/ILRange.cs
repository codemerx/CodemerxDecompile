using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Languages.IL
{
	internal class ILRange
	{
		public int From;

		public int To;

		public ILRange()
		{
		}

		public static IEnumerable<ILRange> Invert(IEnumerable<ILRange> input, int codeSize)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (codeSize <= 0)
			{
				throw new ArgumentException("Code size must be grater than 0");
			}
			List<ILRange> lRanges = ILRange.OrderAndJoint(input);
			if (lRanges.Count != 0)
			{
				if (lRanges.First<ILRange>().From != 0)
				{
					ILRange lRange = new ILRange()
					{
						From = 0,
						To = lRanges.First<ILRange>().From
					};
					yield return lRange;
				}
				for (int i = 0; i < lRanges.Count - 1; i++)
				{
					ILRange lRange1 = new ILRange()
					{
						From = lRanges[i].To,
						To = lRanges[i + 1].From
					};
					yield return lRange1;
				}
				if (lRanges.Last<ILRange>().To != codeSize)
				{
					ILRange lRange2 = new ILRange()
					{
						From = lRanges.Last<ILRange>().To,
						To = codeSize
					};
					yield return lRange2;
				}
			}
			else
			{
				yield return new ILRange()
				{
					From = 0,
					To = codeSize
				};
			}
		}

		public static List<ILRange> OrderAndJoint(IEnumerable<ILRange> input)
		{
			if (input == null)
			{
				throw new ArgumentNullException("Input is null!");
			}
			List<ILRange> list = (
				from r in input
				where r != null
				orderby r.From
				select r).ToList<ILRange>();
			int num = 0;
			while (num < list.Count - 1)
			{
				ILRange item = list[num];
				ILRange lRange = list[num + 1];
				if (item.From > lRange.From || lRange.From > item.To)
				{
					num++;
				}
				else
				{
					item.To = Math.Max(item.To, lRange.To);
					list.RemoveAt(num + 1);
				}
			}
			return list;
		}

		public override string ToString()
		{
			return String.Format("{0}-{1}", (object)this.From.ToString("X"), this.To.ToString("X"));
		}
	}
}