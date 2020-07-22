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
					stackVariable5 = this.get_MemberCodeMappings();
					stackVariable6 = MemberMapping.u003cu003ec.u003cu003e9__18_0;
					if (stackVariable6 == null)
					{
						dummyVar0 = stackVariable6;
						stackVariable6 = new Converter<SourceCodeMapping, ILRange>(MemberMapping.u003cu003ec.u003cu003e9.u003cget_InvertedListu003eb__18_0);
						MemberMapping.u003cu003ec.u003cu003e9__18_0 = stackVariable6;
					}
					V_0 = stackVariable5.ConvertAll<ILRange>(stackVariable6);
					this.invertedList = ILRange.OrderAndJoint(ILRange.Invert(V_0, this.get_CodeSize()));
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
			base();
			return;
		}
	}
}