using Piranha.Extend;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Piranha.Extend.Fields
{
	public abstract class DataSelectFieldBase : IField
	{
		public string Id
		{
			get;
			set;
		}

		public IEnumerable<DataSelectFieldItem> Items
		{
			get;
			set;
		}

		protected DataSelectFieldBase()
		{
		}

		public abstract string GetTitle();
	}
}