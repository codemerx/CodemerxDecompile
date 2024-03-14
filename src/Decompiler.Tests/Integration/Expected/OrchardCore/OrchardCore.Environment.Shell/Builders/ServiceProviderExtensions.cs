using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace OrchardCore.Environment.Shell.Builders
{
	public static class ServiceProviderExtensions
	{
		public static IServiceCollection CreateChildContainer(this IServiceProvider serviceProvider, IServiceCollection serviceCollection)
		{
			IServiceCollection serviceCollection1 = new ServiceCollection();
			foreach (IGrouping<Type, ServiceDescriptor> types in 
				from s in serviceCollection
				group s by s.get_ServiceType())
			{
				if (types.Key == typeof(IStartupFilter))
				{
					continue;
				}
				if (types.Key.IsGenericTypeDefinition)
				{
					foreach (ServiceDescriptor serviceDescriptor in types)
					{
						serviceCollection1.Add(serviceDescriptor);
					}
				}
				else if (types.Count<ServiceDescriptor>() == 1)
				{
					ServiceDescriptor serviceDescriptor1 = types.First<ServiceDescriptor>();
					if (serviceDescriptor1.get_Lifetime() != null)
					{
						serviceCollection1.Add(serviceDescriptor1);
					}
					else if (typeof(IDisposable).IsAssignableFrom(ServiceDescriptorExtensions.GetImplementationType(serviceDescriptor1)) || serviceDescriptor1.get_ImplementationFactory() != null)
					{
						serviceCollection1.CloneSingleton(serviceDescriptor1, serviceProvider.GetService(serviceDescriptor1.get_ServiceType()));
					}
					else
					{
						serviceCollection1.CloneSingleton(serviceDescriptor1, (IServiceProvider sp) => serviceProvider.GetService(serviceDescriptor1.get_ServiceType()));
					}
				}
				else if (types.All<ServiceDescriptor>((ServiceDescriptor s) => s.get_Lifetime() != 0))
				{
					foreach (ServiceDescriptor serviceDescriptor2 in types)
					{
						serviceCollection1.Add(serviceDescriptor2);
					}
				}
				else if (!types.All<ServiceDescriptor>((ServiceDescriptor s) => s.get_Lifetime() == 0))
				{
					using (IServiceScope serviceScope = ServiceProviderServiceExtensions.CreateScope(serviceProvider))
					{
						IEnumerable<object> services = ServiceProviderServiceExtensions.GetServices(serviceScope.get_ServiceProvider(), types.Key);
						for (int i = 0; i < types.Count<ServiceDescriptor>(); i++)
						{
							if (types.ElementAt<ServiceDescriptor>(i).get_Lifetime() != null)
							{
								serviceCollection1.Add(types.ElementAt<ServiceDescriptor>(i));
							}
							else
							{
								serviceCollection1.CloneSingleton(types.ElementAt<ServiceDescriptor>(i), services.ElementAt<object>(i));
							}
						}
					}
				}
				else
				{
					IEnumerable<object> objs = ServiceProviderServiceExtensions.GetServices(serviceProvider, types.Key);
					for (int j = 0; j < types.Count<ServiceDescriptor>(); j++)
					{
						serviceCollection1.CloneSingleton(types.ElementAt<ServiceDescriptor>(j), objs.ElementAt<object>(j));
					}
				}
			}
			return serviceCollection1;
		}
	}
}