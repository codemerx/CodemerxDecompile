using System;
using System.Runtime.CompilerServices;

namespace Piranha.Extend.Fields
{
	public class SelectFieldItem
	{
		public string Title
		{
			get;
			set;
		}

		public Enum Value
		{
			get;
			set;
		}

		public SelectFieldItem()
		{
			base();
			return;
		}
	}
}