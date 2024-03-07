using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Extensions.DependencyInjection;
using Piranha;
using Piranha.Extend;
using Piranha.Models;
using Piranha.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Piranha.Services
{
	public class ContentFactory : IContentFactory
	{
		private readonly IServiceProvider _services;

		public ContentFactory(IServiceProvider services)
		{
			this._services = services;
		}

		public Task<T> CreateAsync<T>(ContentTypeBase type)
		where T : ContentBase
		{
			if (typeof(IDynamicContent).IsAssignableFrom(typeof(T)))
			{
				return this.CreateDynamicModelAsync<T>(type);
			}
			return this.CreateModelAsync<T>(type);
		}

		public async Task<object> CreateBlockAsync(string typeName)
		{
			object obj;
			AppBlock byType = App.Blocks.GetByType(typeName);
			if (byType == null)
			{
				obj = null;
			}
			else
			{
				using (IServiceScope serviceScope = ServiceProviderServiceExtensions.CreateScope(this._services))
				{
					Block block = (Block)Activator.CreateInstance(byType.Type);
					block.Type = typeName;
					PropertyInfo[] properties = byType.Type.GetProperties(App.PropertyBindings);
					for (int i = 0; i < (int)properties.Length; i++)
					{
						PropertyInfo propertyInfo = properties[i];
						if (typeof(IField).IsAssignableFrom(propertyInfo.PropertyType))
						{
							object obj1 = Activator.CreateInstance(propertyInfo.PropertyType);
							ConfiguredTaskAwaitable<object> configuredTaskAwaitable = this.InitFieldAsync(serviceScope, obj1, false).ConfigureAwait(false);
							await configuredTaskAwaitable;
							propertyInfo.SetValue(block, obj1);
							obj1 = null;
						}
						propertyInfo = null;
					}
					properties = null;
					obj = block;
				}
			}
			return obj;
		}

		private async Task<T> CreateDynamicModelAsync<T>(ContentTypeBase type)
		where T : ContentBase
		{
			T t;
			ConfiguredTaskAwaitable<object> configuredTaskAwaitable;
			using (IServiceScope serviceScope = ServiceProviderServiceExtensions.CreateScope(this._services))
			{
				T id = Activator.CreateInstance<T>();
				id.TypeId = type.Id;
				foreach (RegionType region in type.Regions)
				{
					object obj = null;
					if (region.Collection)
					{
						configuredTaskAwaitable = this.CreateDynamicRegionAsync(serviceScope, region, false, false).ConfigureAwait(false);
						object obj1 = await configuredTaskAwaitable;
						if (obj1 != null)
						{
							Type type1 = typeof(RegionList<>);
							Type[] typeArray = new Type[] { obj1.GetType() };
							obj = Activator.CreateInstance(type1.MakeGenericType(typeArray));
							((IRegionList)obj).Model = (IDynamicContent)(object)id;
							((IRegionList)obj).TypeId = type.Id;
							((IRegionList)obj).RegionId = region.Id;
						}
					}
					else
					{
						configuredTaskAwaitable = this.CreateDynamicRegionAsync(serviceScope, region, true, false).ConfigureAwait(false);
						obj = await configuredTaskAwaitable;
					}
					if (obj != null)
					{
						((IDictionary<string, object>)((dynamic)((IDynamicContent)(object)id).Regions)).Add(region.Id, obj);
					}
					obj = null;
				}
				t = id;
			}
			return t;
		}

		public Task<object> CreateDynamicRegionAsync(ContentTypeBase type, string regionId, bool managerInit = false)
		{
			Task<object> task;
			using (IServiceScope serviceScope = ServiceProviderServiceExtensions.CreateScope(this._services))
			{
				RegionType regionType = type.Regions.FirstOrDefault<RegionType>((RegionType r) => r.Id == regionId);
				if (regionType == null)
				{
					task = null;
				}
				else
				{
					task = this.CreateDynamicRegionAsync(serviceScope, regionType, true, managerInit);
				}
			}
			return task;
		}

		private async Task<object> CreateDynamicRegionAsync(IServiceScope scope, RegionType regionType, bool initFields = true, bool managerInit = false)
		{
			object obj;
			ConfiguredTaskAwaitable<object> configuredTaskAwaitable;
			object obj1;
			if (regionType.Fields.Count != 1)
			{
				ExpandoObject expandoObjects = new ExpandoObject();
				foreach (FieldType field in regionType.Fields)
				{
					obj1 = this.CreateField(field);
					if (obj1 != null)
					{
						if (initFields)
						{
							configuredTaskAwaitable = this.InitFieldAsync(scope, obj1, managerInit).ConfigureAwait(false);
							await configuredTaskAwaitable;
						}
						((IDictionary<string, object>)expandoObjects).Add(field.Id, obj1);
					}
					obj1 = null;
				}
				obj = expandoObjects;
			}
			else
			{
				obj1 = this.CreateField(regionType.Fields[0]);
				if (obj1 == null)
				{
					obj1 = null;
					obj = null;
				}
				else
				{
					if (initFields)
					{
						configuredTaskAwaitable = this.InitFieldAsync(scope, obj1, managerInit).ConfigureAwait(false);
						await configuredTaskAwaitable;
					}
					obj = obj1;
				}
			}
			return obj;
		}

		private object CreateField(FieldType fieldType)
		{
			AppField byType = App.Fields.GetByType(fieldType.Type);
			if (byType == null)
			{
				return null;
			}
			return Activator.CreateInstance(byType.Type);
		}

		private async Task<T> CreateModelAsync<T>(ContentTypeBase type)
		where T : ContentBase
		{
			T t;
			using (IServiceScope serviceScope = ServiceProviderServiceExtensions.CreateScope(this._services))
			{
				Type type1 = typeof(T);
				if (!typeof(IContentInfo).IsAssignableFrom(type1))
				{
					type1 = Type.GetType(type.CLRType);
					if (type1 != typeof(T) && !typeof(T).IsAssignableFrom(type1))
					{
						t = default(T);
						return t;
					}
				}
				T id = (T)Activator.CreateInstance(type1);
				id.TypeId = type.Id;
				foreach (RegionType region in type.Regions)
				{
					object obj = null;
					if (region.Collection)
					{
						PropertyInfo property = type1.GetProperty(region.Id, App.PropertyBindings);
						if (property != null)
						{
							Type type2 = typeof(List<>);
							Type[] genericArguments = new Type[] { property.PropertyType.GetGenericArguments()[0] };
							obj = Activator.CreateInstance(type2.MakeGenericType(genericArguments));
						}
					}
					else
					{
						ConfiguredTaskAwaitable<object> configuredTaskAwaitable = this.CreateRegionAsync(serviceScope, id, type1, region, true).ConfigureAwait(false);
						obj = await configuredTaskAwaitable;
					}
					if (obj != null)
					{
						type1.SetPropertyValue(region.Id, id, obj);
					}
				}
				t = id;
			}
			return t;
		}

		private async Task<object> CreateRegionAsync(IServiceScope scope, object model, Type modelType, RegionType regionType, bool initFields = true)
		{
			object obj;
			ConfiguredTaskAwaitable<object> configuredTaskAwaitable;
			object obj1;
			if (regionType.Fields.Count != 1)
			{
				PropertyInfo property = modelType.GetProperty(regionType.Id, App.PropertyBindings);
				if (property != null)
				{
					obj1 = Activator.CreateInstance(property.PropertyType);
					foreach (FieldType field in regionType.Fields)
					{
						object obj2 = this.CreateField(field);
						if (obj2 != null)
						{
							if (initFields)
							{
								configuredTaskAwaitable = this.InitFieldAsync(scope, obj2, false).ConfigureAwait(false);
								await configuredTaskAwaitable;
							}
							obj1.GetType().SetPropertyValue(field.Id, obj1, obj2);
						}
						obj2 = null;
					}
					obj = obj1;
					return obj;
				}
			}
			else
			{
				obj1 = this.CreateField(regionType.Fields[0]);
				if (obj1 == null)
				{
					obj1 = null;
				}
				else
				{
					if (initFields)
					{
						configuredTaskAwaitable = this.InitFieldAsync(scope, obj1, false).ConfigureAwait(false);
						await configuredTaskAwaitable;
					}
					obj = obj1;
					return obj;
				}
			}
			obj = null;
			return obj;
		}

		public Task<T> InitAsync<T>(T model, ContentTypeBase type)
		where T : ContentBase
		{
			return this.InitAsync<T>(model, type, false);
		}

		private async Task<T> InitAsync<T>(T model, ContentTypeBase type, bool managerInit)
		where T : ContentBase
		{
			ConfiguredTaskAwaitable configuredTaskAwaitable;
			if ((object)model is IDynamicContent)
			{
				throw new ArgumentException("For dynamic models InitDynamic should be used.");
			}
			using (IServiceScope serviceScope = ServiceProviderServiceExtensions.CreateScope(this._services))
			{
				foreach (RegionType region in type.Regions)
				{
					object propertyValue = model.GetType().GetPropertyValue(region.Id, model);
					if (propertyValue != null)
					{
						if (region.Collection)
						{
							foreach (object obj in (IList)propertyValue)
							{
								configuredTaskAwaitable = this.InitRegionAsync(serviceScope, obj, region, managerInit).ConfigureAwait(false);
								await configuredTaskAwaitable;
							}
						}
						else
						{
							configuredTaskAwaitable = this.InitRegionAsync(serviceScope, propertyValue, region, managerInit).ConfigureAwait(false);
							await configuredTaskAwaitable;
						}
					}
				}
				if (!((object)model is IContentInfo))
				{
					IBlockContent blockContent = (object)model as IBlockContent;
					if (blockContent != null)
					{
						foreach (Block block in blockContent.Blocks)
						{
							configuredTaskAwaitable = this.InitBlockAsync(serviceScope, block, managerInit).ConfigureAwait(false);
							await configuredTaskAwaitable;
							if (block is BlockGroup)
							{
								foreach (Block item in ((BlockGroup)block).Items)
								{
									configuredTaskAwaitable = this.InitBlockAsync(serviceScope, item, managerInit).ConfigureAwait(false);
									await configuredTaskAwaitable;
								}
							}
						}
					}
				}
			}
			serviceScope = null;
			return model;
		}

		private async Task InitBlockAsync(IServiceScope scope, Block block, bool managerInit)
		{
			if (block != null)
			{
				block.GetType();
				PropertyInfo[] properties = block.GetType().GetProperties(App.PropertyBindings);
				for (int i = 0; i < (int)properties.Length; i++)
				{
					PropertyInfo propertyInfo = properties[i];
					if (typeof(IField).IsAssignableFrom(propertyInfo.PropertyType))
					{
						object value = propertyInfo.GetValue(block);
						if (value != null)
						{
							ConfiguredTaskAwaitable<object> configuredTaskAwaitable = this.InitFieldAsync(scope, value, managerInit).ConfigureAwait(false);
							await configuredTaskAwaitable;
						}
					}
				}
				properties = null;
			}
		}

		public Task<T> InitDynamicAsync<T>(T model, ContentTypeBase type)
		where T : IDynamicContent
		{
			return this.InitDynamicAsync<T>(model, type, false);
		}

		private async Task<T> InitDynamicAsync<T>(T model, ContentTypeBase type, bool managerInit)
		where T : IDynamicContent
		{
			object obj;
			ConfiguredTaskAwaitable configuredTaskAwaitable;
			using (IServiceScope serviceScope = ServiceProviderServiceExtensions.CreateScope(this._services))
			{
				foreach (RegionType region in type.Regions)
				{
					if (((IDictionary<string, object>)((dynamic)model.Regions)).TryGetValue(region.Id, out obj))
					{
						if (region.Collection)
						{
							foreach (object obj1 in (IList)obj)
							{
								configuredTaskAwaitable = this.InitDynamicRegionAsync(serviceScope, obj1, region, managerInit).ConfigureAwait(false);
								await configuredTaskAwaitable;
							}
						}
						else
						{
							configuredTaskAwaitable = this.InitDynamicRegionAsync(serviceScope, obj, region, managerInit).ConfigureAwait(false);
							await configuredTaskAwaitable;
						}
					}
				}
				IBlockContent blockContent = (object)model as IBlockContent;
				if (blockContent != null)
				{
					foreach (Block block in blockContent.Blocks)
					{
						configuredTaskAwaitable = this.InitBlockAsync(serviceScope, block, managerInit).ConfigureAwait(false);
						await configuredTaskAwaitable;
						BlockGroup blockGroup = block as BlockGroup;
						if (blockGroup != null)
						{
							foreach (Block item in blockGroup.Items)
							{
								configuredTaskAwaitable = this.InitBlockAsync(serviceScope, item, managerInit).ConfigureAwait(false);
								await configuredTaskAwaitable;
							}
						}
					}
				}
			}
			serviceScope = null;
			return model;
		}

		public Task<T> InitDynamicManagerAsync<T>(T model, ContentTypeBase type)
		where T : IDynamicContent
		{
			return this.InitDynamicAsync<T>(model, type, true);
		}

		private async Task InitDynamicRegionAsync(IServiceScope scope, object region, RegionType regionType, bool managerInit)
		{
			ConfiguredTaskAwaitable<object> configuredTaskAwaitable;
			object obj;
			if (region != null)
			{
				if (regionType.Fields.Count != 1)
				{
					foreach (FieldType field in regionType.Fields)
					{
						if (!((IDictionary<string, object>)region).TryGetValue(field.Id, out obj))
						{
							continue;
						}
						configuredTaskAwaitable = this.InitFieldAsync(scope, obj, managerInit).ConfigureAwait(false);
						await configuredTaskAwaitable;
					}
				}
				else
				{
					configuredTaskAwaitable = this.InitFieldAsync(scope, region, managerInit).ConfigureAwait(false);
					await configuredTaskAwaitable;
				}
			}
		}

		public async Task<object> InitFieldAsync(object field, bool managerInit = false)
		{
			object obj;
			using (IServiceScope serviceScope = ServiceProviderServiceExtensions.CreateScope(this._services))
			{
				obj = await this.InitFieldAsync(serviceScope, field, managerInit);
			}
			return obj;
		}

		private async Task<object> InitFieldAsync(IServiceScope scope, object field, bool managerInit)
		{
			MethodInfo methodInfo = null;
			methodInfo = (managerInit ? field.GetType().GetMethod("InitManager") : field.GetType().GetMethod("Init"));
			if (methodInfo != null)
			{
				List<object> objs = new List<object>();
				ParameterInfo[] parameters = methodInfo.GetParameters();
				for (int i = 0; i < (int)parameters.Length; i++)
				{
					ParameterInfo parameterInfo = parameters[i];
					objs.Add(scope.get_ServiceProvider().GetService(parameterInfo.ParameterType));
				}
				if (!typeof(Task).IsAssignableFrom(methodInfo.ReturnType))
				{
					methodInfo.Invoke(field, objs.ToArray());
				}
				else
				{
					ConfiguredTaskAwaitable configuredTaskAwaitable = ((Task)methodInfo.Invoke(field, objs.ToArray())).ConfigureAwait(false);
					await configuredTaskAwaitable;
				}
			}
			return field;
		}

		public Task<T> InitManagerAsync<T>(T model, ContentTypeBase type)
		where T : ContentBase
		{
			return this.InitAsync<T>(model, type, true);
		}

		private async Task InitRegionAsync(IServiceScope scope, object region, RegionType regionType, bool managerInit)
		{
			ConfiguredTaskAwaitable<object> configuredTaskAwaitable;
			if (region != null)
			{
				if (regionType.Fields.Count != 1)
				{
					Type type = region.GetType();
					foreach (FieldType field in regionType.Fields)
					{
						object propertyValue = type.GetPropertyValue(field.Id, region);
						if (propertyValue == null)
						{
							continue;
						}
						configuredTaskAwaitable = this.InitFieldAsync(scope, propertyValue, managerInit).ConfigureAwait(false);
						await configuredTaskAwaitable;
					}
					type = null;
				}
				else
				{
					configuredTaskAwaitable = this.InitFieldAsync(scope, region, managerInit).ConfigureAwait(false);
					await configuredTaskAwaitable;
				}
			}
		}
	}
}