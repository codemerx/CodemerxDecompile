using System;
using System.Runtime.CompilerServices;

namespace Telerik.JustDecompiler.Languages.IL
{
	internal sealed class SourceCodeMapping
	{
		internal ILRange ILInstructionOffset
		{
			get;
			set;
		}

		internal Telerik.JustDecompiler.Languages.IL.MemberMapping MemberMapping
		{
			get;
			set;
		}

		public SourceCodeMapping()
		{
			base();
			return;
		}
	}
}