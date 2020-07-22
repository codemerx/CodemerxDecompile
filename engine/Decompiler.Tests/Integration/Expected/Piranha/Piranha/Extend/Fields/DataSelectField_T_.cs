using Microsoft.Extensions.DependencyInjection;
using Piranha.Extend;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
			base();
			return;
		}

		public override string GetTitle()
		{
			if (this.get_Value() == null)
			{
				return "Not item selected";
			}
			return this.get_Value().ToString();
		}

		public async Task Init(IServiceProvider services)
		{
			V_0.u003cu003e4__this = this;
			V_0.services = services;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<DataSelectField<T>.u003cInitu003ed__4>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task InitManager(IServiceProvider services)
		{
			V_0.u003cu003e4__this = this;
			V_0.services = services;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<DataSelectField<T>.u003cInitManageru003ed__5>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}