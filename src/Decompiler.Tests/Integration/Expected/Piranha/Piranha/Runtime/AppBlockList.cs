using Piranha;
using Piranha.Extend;
using Piranha.Extend.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Piranha.Runtime
{
	public sealed class AppBlockList : AppDataList<Block, AppBlock>
	{
		public AppBlockList()
		{
		}

		public IEnumerable<AppBlock> GetByCategory(string category, bool includeGroups = true)
		{
			IEnumerable<AppBlock> appBlocks = 
				from i in this._items
				where i.Category == category
				select i;
			if (!includeGroups)
			{
				appBlocks = 
					from i in appBlocks
					where !typeof(BlockGroup).IsAssignableFrom(i.Type)
					select i;
			}
			return appBlocks.ToArray<AppBlock>();
		}

		public IEnumerable<string> GetCategories()
		{
			return (
				from c in (
					from i in this._items
					select i.Category).Distinct<string>()
				orderby c
				select c).ToArray<string>();
		}

		protected override AppBlock OnRegister<TValue>(AppBlock item)
		where TValue : Block
		{
			BlockTypeAttribute customAttribute = typeof(TValue).GetTypeInfo().GetCustomAttribute<BlockTypeAttribute>();
			if (customAttribute != null)
			{
				item.Name = customAttribute.Name;
				item.Category = customAttribute.Category;
				item.Icon = customAttribute.Icon;
				item.ListTitleField = customAttribute.ListTitle;
				item.IsUnlisted = customAttribute.IsUnlisted;
				item.IsGeneric = customAttribute.IsGeneric;
				item.Component = (!String.IsNullOrWhiteSpace(customAttribute.Component) ? customAttribute.Component : "missing-block");
				BlockGroupTypeAttribute blockGroupTypeAttribute = customAttribute as BlockGroupTypeAttribute;
				if (blockGroupTypeAttribute != null)
				{
					item.UseCustomView = blockGroupTypeAttribute.UseCustomView;
					item.Display = blockGroupTypeAttribute.Display;
				}
			}
			foreach (Attribute attribute in typeof(TValue).GetTypeInfo().GetCustomAttributes(typeof(BlockItemTypeAttribute)))
			{
				Type type = ((BlockItemTypeAttribute)attribute).Type;
				if (typeof(BlockGroup).IsAssignableFrom(type))
				{
					continue;
				}
				item.ItemTypes.Add(type);
			}
			PropertyInfo[] properties = typeof(TValue).GetProperties(App.PropertyBindings);
			for (int i = 0; i < (int)properties.Length; i++)
			{
				PropertyInfo propertyInfo = properties[i];
				if (typeof(IField).IsAssignableFrom(propertyInfo.PropertyType))
				{
					MethodInfo methodInfo = null;
					methodInfo = (!typeof(SelectFieldBase).IsAssignableFrom(propertyInfo.PropertyType) ? typeof(AppFieldList).GetMethod("Register").MakeGenericMethod(new Type[] { propertyInfo.PropertyType }) : typeof(AppFieldList).GetMethod("RegisterSelect").MakeGenericMethod(new Type[] { propertyInfo.PropertyType.GenericTypeArguments.First<Type>() }));
					methodInfo.Invoke(App.Fields, null);
				}
			}
			return item;
		}
	}
}