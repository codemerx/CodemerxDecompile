using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Modules;
using System;
using System.Collections.Generic;
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
			get
			{
				return this.u003cConfigureOrderu003ek__BackingField;
			}
		}

		public override int Order
		{
			get
			{
				return this.u003cOrderu003ek__BackingField;
			}
		}

		public StartupBaseMock(object startup, MethodInfo configureService, MethodInfo configure, PropertyInfo order, PropertyInfo configureOrder)
		{
			base();
			this._startup = startup;
			this._configureService = configureService;
			this._configure = configure;
			if (order != null)
			{
				stackVariable11 = order.GetValue(this._startup);
			}
			else
			{
				stackVariable11 = null;
			}
			V_0 = stackVariable11;
			if (configureOrder != null)
			{
				stackVariable16 = configureOrder.GetValue(this._startup);
			}
			else
			{
				stackVariable16 = null;
			}
			V_1 = stackVariable16;
			if (V_0 != null)
			{
				stackVariable20 = (int)V_0;
			}
			else
			{
				stackVariable20 = 0;
			}
			this.u003cOrderu003ek__BackingField = stackVariable20;
			if (V_1 != null)
			{
				stackVariable24 = (int)V_1;
			}
			else
			{
				stackVariable24 = this.get_Order();
			}
			this.u003cConfigureOrderu003ek__BackingField = stackVariable24;
			return;
		}

		public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
		{
			V_0 = new StartupBaseMock.u003cu003ec__DisplayClass11_0();
			V_0.serviceProvider = serviceProvider;
			V_0.app = app;
			V_0.routes = routes;
			if (MethodInfo.op_Equality(this._configure, null))
			{
				return;
			}
			V_1 = this._configure.GetParameters().Select<ParameterInfo, object>(new Func<ParameterInfo, object>(V_0.u003cConfigureu003eb__0));
			dummyVar0 = this._configure.Invoke(this._startup, V_1.ToArray<object>());
			return;
		}

		public override void ConfigureServices(IServiceCollection services)
		{
			if (MethodInfo.op_Equality(this._configureService, null))
			{
				return;
			}
			stackVariable5 = this._configureService;
			stackVariable7 = this._startup;
			stackVariable9 = new IServiceCollection[1];
			stackVariable9[0] = services;
			dummyVar0 = stackVariable5.Invoke(stackVariable7, stackVariable9);
			return;
		}
	}
}