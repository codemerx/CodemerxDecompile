using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.CompilerServices;

namespace Piranha
{
	public class PiranhaServiceBuilder : PiranhaRouteConfig
	{
		public readonly IServiceCollection Services;

		public bool AddRazorRuntimeCompilation
		{
			get;
			set;
		}

		public PiranhaServiceBuilder(IServiceCollection services)
		{
			this.Services = services;
		}
	}
}