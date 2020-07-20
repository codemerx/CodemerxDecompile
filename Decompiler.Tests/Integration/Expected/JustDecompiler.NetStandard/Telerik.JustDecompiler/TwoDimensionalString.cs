using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler
{
	public class TwoDimensionalString
	{
		protected List<int> newLineOffsets;

		private readonly bool isZeroIndexed;

		public string NewLine
		{
			get;
			private set;
		}

		public string TheString
		{
			get;
			private set;
		}

		public TwoDimensionalString(string theString, string newLine, bool isZeroIndexed)
		{
			base();
			this.set_TheString(theString);
			this.set_NewLine(newLine);
			this.isZeroIndexed = isZeroIndexed;
			this.FindNewLineOffsets();
			return;
		}

		private void FindNewLineOffsets()
		{
			this.newLineOffsets = new List<int>();
			V_0 = this.get_TheString().IndexOf(this.get_NewLine());
			while (V_0 != -1)
			{
				this.newLineOffsets.Add(V_0);
				V_0 = this.get_TheString().IndexOf(this.get_NewLine(), V_0 + this.get_NewLine().get_Length());
			}
			return;
		}

		public CodePosition GetTwoDimensionalCordinatesFor(int stringOffset)
		{
			V_1 = this.newLineOffsets.BinarySearch(stringOffset);
			if (V_1 >= 0)
			{
				if (!this.isZeroIndexed)
				{
					V_0 = new CodePosition(V_1 + 1, -1);
				}
				else
				{
					V_0 = new CodePosition(V_1, -1);
				}
			}
			else
			{
				V_2 = ~V_1;
				V_3 = 0;
				if (V_2 != 0)
				{
					V_3 = this.newLineOffsets.get_Item(V_2 - 1) + this.get_NewLine().get_Length();
				}
				V_4 = stringOffset - V_3;
				if (!this.isZeroIndexed)
				{
					V_0 = new CodePosition(V_2 + 1, V_4 + 1);
				}
				else
				{
					V_0 = new CodePosition(V_2, V_4);
				}
			}
			return V_0;
		}

		public OffsetSpan TrimStart(OffsetSpan position)
		{
			if (position.EndOffset < this.get_TheString().get_Length())
			{
				stackVariable6 = position.EndOffset;
			}
			else
			{
				stackVariable6 = this.get_TheString().get_Length() - 1;
			}
			V_0 = stackVariable6;
			V_1 = position.StartOffset;
			while (V_1 <= V_0)
			{
				if (!Char.IsWhiteSpace(this.get_TheString().get_Chars(V_1)))
				{
					return new OffsetSpan(V_1, position.EndOffset);
				}
				V_1 = V_1 + 1;
			}
			throw new Exception("The span contains only whitespaces.");
		}
	}
}