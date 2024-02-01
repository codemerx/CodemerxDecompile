using System;
using System.Runtime.CompilerServices;

namespace Piranha.Extend
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
	public class BlockItemTypeAttribute : Attribute
	{
		public System.Type Type
		{
			get;
			set;
		}

		public BlockItemTypeAttribute()
		{
		}

		public BlockItemTypeAttribute(System.Type type)
		{
			this.Type = type;
		}
	}
}