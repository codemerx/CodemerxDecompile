using Newtonsoft.Json;
using Piranha;
using Piranha.Extend;
using System;
using System.Collections.Generic;

namespace Piranha.Extend.Fields
{
	public abstract class SelectFieldBase : IField
	{
		[JsonIgnore]
		public abstract Type EnumType
		{
			get;
		}

		[JsonIgnore]
		public abstract string EnumValue
		{
			get;
			set;
		}

		[JsonIgnore]
		public abstract List<SelectFieldItem> Items
		{
			get;
		}

		protected SelectFieldBase()
		{
		}

		public abstract string GetTitle();

		public abstract void Init(IApi api);
	}
}