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
			ContentFactory.u003cCreateBlockAsyncu003ed__4 variable = new ContentFactory.u003cCreateBlockAsyncu003ed__4();
			variable.u003cu003e4__this = this;
			variable.typeName = typeName;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<object>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<ContentFactory.u003cCreateBlockAsyncu003ed__4>(ref variable);
			return variable.u003cu003et__builder.Task;
		}

		private async Task<T> CreateDynamicModelAsync<T>(ContentTypeBase type)
		where T : ContentBase
		{
			ContentFactory.u003cCreateDynamicModelAsyncu003ed__5<T> variable = new ContentFactory.u003cCreateDynamicModelAsyncu003ed__5<T>();
			variable.u003cu003e4__this = this;
			variable.type = type;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<T>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<ContentFactory.u003cCreateDynamicModelAsyncu003ed__5<T>>(ref variable);
			return variable.u003cu003et__builder.Task;
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
			ContentFactory.u003cCreateModelAsyncu003ed__6<T> variable = new ContentFactory.u003cCreateModelAsyncu003ed__6<T>();
			variable.u003cu003e4__this = this;
			variable.type = type;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<T>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<ContentFactory.u003cCreateModelAsyncu003ed__6<T>>(ref variable);
			return variable.u003cu003et__builder.Task;
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
			ContentFactory.u003cInitAsyncu003ed__13<T> variable = new ContentFactory.u003cInitAsyncu003ed__13<T>();
			variable.u003cu003e4__this = this;
			variable.model = model;
			variable.type = type;
			variable.managerInit = managerInit;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<T>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<ContentFactory.u003cInitAsyncu003ed__13<T>>(ref variable);
			return variable.u003cu003et__builder.Task;
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
			ContentFactory.u003cInitDynamicAsyncu003ed__9<T> variable = new ContentFactory.u003cInitDynamicAsyncu003ed__9<T>();
			variable.u003cu003e4__this = this;
			variable.model = model;
			variable.type = type;
			variable.managerInit = managerInit;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<T>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<ContentFactory.u003cInitDynamicAsyncu003ed__9<T>>(ref variable);
			return variable.u003cu003et__builder.Task;
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
			ContentFactory.u003cInitFieldAsyncu003ed__12 variable = new ContentFactory.u003cInitFieldAsyncu003ed__12();
			variable.u003cu003e4__this = this;
			variable.field = field;
			variable.managerInit = managerInit;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<object>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<ContentFactory.u003cInitFieldAsyncu003ed__12>(ref variable);
			return variable.u003cu003et__builder.Task;
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