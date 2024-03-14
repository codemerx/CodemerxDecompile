using Piranha;
using Piranha.Runtime;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Piranha.Extend
{
	public abstract class Block
	{
		public Guid Id
		{
			get;
			set;
		}

		public string Type
		{
			get;
			set;
		}

		protected Block()
		{
		}

		public virtual string GetTitle()
		{
			AppBlock byType = App.Blocks.GetByType(this.GetType());
			string title = "[Not Implemented]";
			if (!String.IsNullOrEmpty(byType.ListTitleField))
			{
				PropertyInfo property = this.GetType().GetProperty(byType.ListTitleField, App.PropertyBindings);
				if (property != null && typeof(IField).IsAssignableFrom(property.PropertyType))
				{
					title = ((IField)property.GetValue(this)).GetTitle();
				}
			}
			return title;
		}
	}
}