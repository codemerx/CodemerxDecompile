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
			base();
			return;
		}

		public BlockItemTypeAttribute(System.Type type)
		{
			base();
			this.set_Type(type);
			return;
		}
	}
}