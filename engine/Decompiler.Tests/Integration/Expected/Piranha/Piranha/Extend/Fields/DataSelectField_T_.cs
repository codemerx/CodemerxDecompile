using Microsoft.Extensions.DependencyInjection;
using Piranha.Extend;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Piranha.Extend.Fields
{
	[FieldType(Name="DataSelect", Shorthand="DataSelect", Component="data-select-field")]
	public class DataSelectField<T> : DataSelectFieldBase
	where T : class
	{
		public T Value
		{
			get;
			set;
		}

		public DataSelectField()
		{
		}

		public override string GetTitle()
		{
			if (this.Value == null)
			{
				return "Not item selected";
			}
			return this.Value.ToString();
		}

		public async Task Init(IServiceProvider services)
		{
			if (!String.IsNullOrWhiteSpace(base.Id))
			{
				MethodInfo method = typeof(T).GetMethod("GetById", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				if (method != null)
				{
					using (IServiceScope serviceScope = ServiceProviderServiceExtensions.CreateScope(services))
					{
						List<object> objs = new List<object>()
						{
							base.Id
						};
						foreach (ParameterInfo parameterInfo in method.GetParameters().Skip<ParameterInfo>(1))
						{
							objs.Add(serviceScope.get_ServiceProvider().GetService(parameterInfo.ParameterType));
						}
						if (!typeof(Task<T>).IsAssignableFrom(method.ReturnType))
						{
							await Task.Run(() => this.Value = (T)method.Invoke(null, objs.ToArray()));
						}
						else
						{
							ConfiguredTaskAwaitable<T> configuredTaskAwaitable = ((Task<T>)method.Invoke(null, objs.ToArray())).ConfigureAwait(false);
							this.Value = await configuredTaskAwaitable;
						}
					}
					serviceScope = null;
				}
			}
		}

		public async Task InitManager(IServiceProvider services)
		{
			MethodInfo method = typeof(T).GetMethod("GetList", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (method != null)
			{
				using (IServiceScope serviceScope = ServiceProviderServiceExtensions.CreateScope(services))
				{
					List<object> objs = new List<object>();
					ParameterInfo[] parameters = method.GetParameters();
					for (int i = 0; i < (int)parameters.Length; i++)
					{
						ParameterInfo parameterInfo = parameters[i];
						objs.Add(serviceScope.get_ServiceProvider().GetService(parameterInfo.ParameterType));
					}
					if (!typeof(Task<IEnumerable<DataSelectFieldItem>>).IsAssignableFrom(method.ReturnType))
					{
						await Task.Run(() => this.Items = ((IEnumerable<DataSelectFieldItem>)method.Invoke(null, objs.ToArray())).ToArray<DataSelectFieldItem>());
					}
					else
					{
						ConfiguredTaskAwaitable<IEnumerable<DataSelectFieldItem>> configuredTaskAwaitable = ((Task<IEnumerable<DataSelectFieldItem>>)method.Invoke(null, objs.ToArray())).ConfigureAwait(false);
						base.Items = await configuredTaskAwaitable.ToArray<DataSelectFieldItem>();
					}
				}
				serviceScope = null;
			}
		}
	}
}