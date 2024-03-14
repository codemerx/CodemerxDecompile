using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Telerik.JustDecompiler.Languages.IL
{
	internal sealed class MemberMapping
	{
		private IEnumerable<ILRange> invertedList;

		public int CodeSize
		{
			get;
			internal set;
		}

		public IEnumerable<ILRange> InvertedList
		{
			get
			{
				if (this.invertedList == null)
				{
					List<ILRange> lRanges = this.MemberCodeMappings.ConvertAll<ILRange>((SourceCodeMapping s) => new ILRange()
					{
						From = s.ILInstructionOffset.From,
						To = s.ILInstructionOffset.To
					});
					this.invertedList = ILRange.OrderAndJoint(ILRange.Invert(lRanges, this.CodeSize));
				}
				return this.invertedList;
			}
		}

		internal List<SourceCodeMapping> MemberCodeMappings
		{
			get;
			set;
		}

		public Mono.Cecil.MemberReference MemberReference
		{
			get;
			internal set;
		}

		public uint MetadataToken
		{
			get;
			internal set;
		}

		public MemberMapping()
		{
		}
	}
}