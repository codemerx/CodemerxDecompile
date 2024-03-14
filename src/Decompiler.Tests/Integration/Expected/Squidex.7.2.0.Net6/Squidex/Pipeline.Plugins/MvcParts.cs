using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Squidex.Pipeline.Plugins
{
	[Nullable(0)]
	[NullableContext(1)]
	public static class MvcParts
	{
		public static void AddParts(this Assembly assembly, IMvcBuilder mvcBuilder)
		{
			MvcCoreMvcBuilderExtensions.ConfigureApplicationPartManager(mvcBuilder, (ApplicationPartManager manager) => {
				IList<ApplicationPart> applicationParts = manager.get_ApplicationParts();
				MvcParts.AddParts(applicationParts, assembly);
				foreach (Assembly relatedAssembly in RelatedAssemblyAttribute.GetRelatedAssemblies(assembly, false))
				{
					MvcParts.AddParts(applicationParts, relatedAssembly);
				}
			});
		}

		private static void AddParts(IList<ApplicationPart> applicationParts, Assembly assembly)
		{
			foreach (ApplicationPart applicationPart in ApplicationPartFactory.GetApplicationPartFactory(assembly).GetApplicationParts(assembly))
			{
				List<ApplicationPart> list = (
					from x in applicationParts
					where x.get_Name() == applicationPart.get_Name()
					select x).ToList<ApplicationPart>();
				foreach (ApplicationPart applicationPart1 in list)
				{
					applicationParts.Remove(applicationPart1);
				}
				applicationParts.Add(applicationPart);
			}
		}
	}
}