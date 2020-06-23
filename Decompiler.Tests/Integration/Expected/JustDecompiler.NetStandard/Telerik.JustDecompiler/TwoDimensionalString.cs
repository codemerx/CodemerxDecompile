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
			this.TheString = theString;
			this.NewLine = newLine;
			this.isZeroIndexed = isZeroIndexed;
			this.FindNewLineOffsets();
		}

		private void FindNewLineOffsets()
		{
			this.newLineOffsets = new List<int>();
			for (int i = this.TheString.IndexOf(this.NewLine); i != -1; i = this.TheString.IndexOf(this.NewLine, i + this.NewLine.Length))
			{
				this.newLineOffsets.Add(i);
			}
		}

		public CodePosition GetTwoDimensionalCordinatesFor(int stringOffset)
		{
			CodePosition codePosition;
			int num = this.newLineOffsets.BinarySearch(stringOffset);
			if (num >= 0)
			{
				codePosition = (!this.isZeroIndexed ? new CodePosition(num + 1, -1) : new CodePosition(num, -1));
			}
			else
			{
				int num1 = ~num;
				int item = 0;
				if (num1 != 0)
				{
					item = this.newLineOffsets[num1 - 1] + this.NewLine.Length;
				}
				int num2 = stringOffset - item;
				codePosition = (!this.isZeroIndexed ? new CodePosition(num1 + 1, num2 + 1) : new CodePosition(num1, num2));
			}
			return codePosition;
		}

		public OffsetSpan TrimStart(OffsetSpan position)
		{
			int num = (position.EndOffset < this.TheString.Length ? position.EndOffset : this.TheString.Length - 1);
			for (int i = position.StartOffset; i <= num; i++)
			{
				if (!Char.IsWhiteSpace(this.TheString[i]))
				{
					return new OffsetSpan(i, position.EndOffset);
				}
			}
			throw new Exception("The span contains only whitespaces.");
		}
	}
}