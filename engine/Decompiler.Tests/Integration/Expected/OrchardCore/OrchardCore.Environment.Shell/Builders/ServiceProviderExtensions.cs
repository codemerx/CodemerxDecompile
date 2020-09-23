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
			V_0 = new ServiceProviderExtensions.u003cu003ec__DisplayClass0_0();
			V_0.serviceProvider = serviceProvider;
			V_1 = new ServiceCollection();
			stackVariable4 = serviceCollection;
			stackVariable5 = ServiceProviderExtensions.u003cu003ec.u003cu003e9__0_0;
			if (stackVariable5 == null)
			{
				dummyVar0 = stackVariable5;
				stackVariable5 = new Func<ServiceDescriptor, Type>(ServiceProviderExtensions.u003cu003ec.u003cu003e9.u003cCreateChildContaineru003eb__0_0);
				ServiceProviderExtensions.u003cu003ec.u003cu003e9__0_0 = stackVariable5;
			}
			V_2 = stackVariable4.GroupBy<ServiceDescriptor, Type>(stackVariable5).GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					if (Type.op_Equality(V_3.get_Key(), Type.GetTypeFromHandle(// 
					// Current member / type: Microsoft.Extensions.DependencyInjection.IServiceCollection OrchardCore.Environment.Shell.Builders.ServiceProviderExtensions::CreateChildContainer(System.IServiceProvider,Microsoft.Extensions.DependencyInjection.IServiceCollection)
					// Exception in: Microsoft.Extensions.DependencyInjection.IServiceCollection CreateChildContainer(System.IServiceProvider,Microsoft.Extensions.DependencyInjection.IServiceCollection)
					// Specified method is not supported.
					// 
					// mailto: JustDecompilePublicFeedback@telerik.com

	}
}