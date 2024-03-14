using Piranha;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Extend.Serializers;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Piranha.Runtime
{
	public sealed class AppFieldList : AppDataList<IField, AppField>
	{
		public AppFieldList()
		{
		}

		public AppField GetByShorthand(string shorthand)
		{
			return this._items.FirstOrDefault<AppField>((AppField i) => i.Shorthand == shorthand);
		}

		protected override AppField OnRegister<TValue>(AppField item)
		where TValue : IField
		{
			FieldTypeAttribute customAttribute = typeof(TValue).GetTypeInfo().GetCustomAttribute<FieldTypeAttribute>();
			if (customAttribute != null)
			{
				item.Name = customAttribute.Name;
				item.Shorthand = customAttribute.Shorthand;
				item.Component = (!String.IsNullOrWhiteSpace(customAttribute.Component) ? customAttribute.Component : "missing-field");
			}
			return item;
		}

		public void RegisterDataSelect<TValue>()
		where TValue : class
		{
			this.Register<DataSelectField<TValue>>();
			App.Serializers.Register<DataSelectField<TValue>>(new DataSelectFieldSerializer<DataSelectField<TValue>>());
		}

		public void RegisterSelect<TValue>()
		where TValue : struct
		{
			this.Register<SelectField<TValue>>();
			App.Serializers.Register<SelectField<TValue>>(new SelectFieldSerializer<SelectField<TValue>>());
		}
	}
}