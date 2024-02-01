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
			DataSelectField<T>.u003cInitu003ed__4 variable = new DataSelectField<T>.u003cInitu003ed__4();
			variable.u003cu003e4__this = this;
			variable.services = services;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<DataSelectField<T>.u003cInitu003ed__4>(ref variable);
			return variable.u003cu003et__builder.Task;
		}

		public async Task InitManager(IServiceProvider services)
		{
			DataSelectField<T>.u003cInitManageru003ed__5 variable = new DataSelectField<T>.u003cInitManageru003ed__5();
			variable.u003cu003e4__this = this;
			variable.services = services;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<DataSelectField<T>.u003cInitManageru003ed__5>(ref variable);
			return variable.u003cu003et__builder.Task;
		}
	}
}