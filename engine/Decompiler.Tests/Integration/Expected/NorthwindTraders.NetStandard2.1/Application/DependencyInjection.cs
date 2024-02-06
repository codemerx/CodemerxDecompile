using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Application.Common.Behaviours;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Northwind.Application
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddApplication(this IServiceCollection services)
		{
			AutoMapper.ServiceCollectionExtensions.AddAutoMapper(services, new Assembly[] { Assembly.GetExecutingAssembly() });
			MediatR.ServiceCollectionExtensions.AddMediatR(services, new Assembly[] { Assembly.GetExecutingAssembly() });
			ServiceCollectionServiceExtensions.AddTransient(services, typeof(IPipelineBehavior), typeof(RequestPerformanceBehaviour<,>));
			ServiceCollectionServiceExtensions.AddTransient(services, typeof(IPipelineBehavior), typeof(RequestValidationBehavior<,>));
			return services;
		}
	}
}