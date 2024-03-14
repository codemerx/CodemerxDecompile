using Piranha.Models;
using System;
using System.Runtime.CompilerServices;

namespace Piranha.Extend
{
	[AttributeUsage(AttributeTargets.Class)]
	public class BlockGroupTypeAttribute : BlockTypeAttribute
	{
		public BlockDisplayMode Display
		{
			get;
			set;
		}

		public bool UseCustomView
		{
			get;
			set;
		}

		public BlockGroupTypeAttribute()
		{
		}
	}
}