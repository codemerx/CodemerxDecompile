using Piranha.Models;
using System;
using System.Runtime.CompilerServices;

namespace Piranha.Extend
{
	[AttributeUsage(AttributeTargets.Property)]
	public class FieldAttribute : Attribute
	{
		public FieldOption Options
		{
			get;
			set;
		}

		public string Placeholder
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public FieldAttribute()
		{
			base();
			return;
		}
	}
}