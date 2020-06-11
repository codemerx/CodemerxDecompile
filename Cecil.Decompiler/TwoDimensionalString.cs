using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler
{
	public class TwoDimensionalString
	{
		protected List<int> newLineOffsets;
		/// <summary>
		/// Determines if the indexing should start form [0,0] or [1,1].
		/// </summary>
		private readonly bool isZeroIndexed;

        public TwoDimensionalString(string theString, string newLine, bool isZeroIndexed)
        {
            TheString = theString;
            NewLine = newLine;
			this.isZeroIndexed = isZeroIndexed;
            FindNewLineOffsets();
        }

        public string TheString { get; private set; }
        public string NewLine { get; private set; }

        private void FindNewLineOffsets()
        {
            newLineOffsets = new List<int>();
            int lastOffset = TheString.IndexOf(NewLine);

            while(lastOffset != -1)
            {
                newLineOffsets.Add(lastOffset);
                lastOffset = TheString.IndexOf(NewLine, lastOffset + NewLine.Length);
            }
        }

        public CodePosition GetTwoDimensionalCordinatesFor(int stringOffset)
        {
            CodePosition result;

            int line = newLineOffsets.BinarySearch(stringOffset);

            if(line < 0) // there is no newline string at this offset
            {
                int lineNumber = ~line;
                int lineStartOffset = 0;

                if(lineNumber != 0)
                {
                    lineStartOffset = newLineOffsets[lineNumber - 1] + NewLine.Length;
                }

                int column = stringOffset - lineStartOffset;
				if (isZeroIndexed)
				{
					result = new CodePosition(lineNumber, column);
				}
				else
				{
					result = new CodePosition(lineNumber + 1, column + 1); //VS editor uses [1,1] as origin as opposed to [0,0]
				}
            }
            else
            {
				if (isZeroIndexed)
				{
					result = new CodePosition(line, -1);
				}
				else
				{
					result = new CodePosition(line + 1, -1);
				}
            }

            return result;
        }

		public OffsetSpan TrimStart(OffsetSpan position) 
		{
			int limit = position.EndOffset < TheString.Length ? position.EndOffset : TheString.Length - 1;
			int offset;
			for (offset = position.StartOffset; offset <= limit; offset++)
			{
				if (!char.IsWhiteSpace(TheString[offset]))
				{
					return new OffsetSpan(offset, position.EndOffset);
				}
			}
			throw new Exception("The span contains only whitespaces.");
		}
	}
}
