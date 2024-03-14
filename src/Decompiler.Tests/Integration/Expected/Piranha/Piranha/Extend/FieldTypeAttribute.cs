using System;
using System.Runtime.CompilerServices;

namespace Piranha.Extend
{
	[AttributeUsage(AttributeTargets.Class)]
	public class FieldTypeAttribute : Attribute
	{
		public string Component
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public string Shorthand
		{
			get;
			set;
		}

		public FieldTypeAttribute()
		{
		}
	}
}