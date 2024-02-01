using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Telerik.JustDecompiler.Languages.IL
{
	internal class CodeMappings
	{
		public string FullName
		{
			get;
			set;
		}

		public List<MemberMapping> Mapping
		{
			get;
			set;
		}

		public CodeMappings()
		{
		}
	}
}