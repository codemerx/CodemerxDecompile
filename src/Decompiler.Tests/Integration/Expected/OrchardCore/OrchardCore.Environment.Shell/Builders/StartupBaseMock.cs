using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace OrchardCore.Environment.Shell.Builders
{
	internal class StartupBaseMock : StartupBase
	{
		private readonly object _startup;

		private readonly MethodInfo _configureService;

		private readonly MethodInfo _configure;

		public override int ConfigureOrder
		{
			get;
		}

		public override int Order
		{
			get;
		}

		public StartupBaseMock(object startup, MethodInfo configureService, MethodInfo configure, PropertyInfo order, PropertyInfo configureOrder)
		{
			this._startup = startup;
			this._configureService = configureService;
			this._configure = configure;
			object obj = (order != null ? order.GetValue(this._startup) : null);
			object obj1 = (configureOrder != null ? configureOrder.GetValue(this._startup) : null);
			this.Order = (obj != null ? (int)obj : 0);
			this.ConfigureOrder = (obj1 != null ? (int)obj1 : this.get_Order());
		}

		public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
		{
			if (this._configure == null)
			{
				return;
			}
			IEnumerable<object> objs = this._configure.GetParameters().Select<ParameterInfo, object>((ParameterInfo x) => {
				if (x.ParameterType == typeof(IServiceProvider))
				{
					return serviceProvider;
				}
				if (x.ParameterType == typeof(IApplicationBuilder))
				{
					return app;
				}
				if (x.ParameterType == typeof(IEndpointRouteBuilder))
				{
					return routes;
				}
				return serviceProvider.GetService(x.ParameterType);
			});
			this._configure.Invoke(this._startup, objs.ToArray<object>());
		}

		public override void ConfigureServices(IServiceCollection services)
		{
			if (this._configureService == null)
			{
				return;
			}
			this._configureService.Invoke(this._startup, new IServiceCollection[] { services });
		}
	}
}