using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrchardCore.Environment.Extensions;
using OrchardCore.Environment.Extensions.Features;
using OrchardCore.Environment.Shell;
using OrchardCore.Environment.Shell.Builders.Models;
using OrchardCore.Environment.Shell.Configuration;
using OrchardCore.Modules;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace OrchardCore.Environment.Shell.Builders
{
	public class ShellContainerFactory : IShellContainerFactory
	{
		private IFeatureInfo _applicationFeature;

		private readonly IHostEnvironment _hostingEnvironment;

		private readonly IExtensionManager _extensionManager;

		private readonly IServiceProvider _serviceProvider;

		private readonly IServiceCollection _applicationServices;

		public ShellContainerFactory(IHostEnvironment hostingEnvironment, IExtensionManager extensionManager, IServiceProvider serviceProvider, IServiceCollection applicationServices)
		{
			base();
			this._hostingEnvironment = hostingEnvironment;
			this._extensionManager = extensionManager;
			this._applicationServices = applicationServices;
			this._serviceProvider = serviceProvider;
			return;
		}

		public void AddCoreServices(IServiceCollection services)
		{
			ServiceCollectionDescriptorExtensions.TryAddScoped<IShellStateUpdater, ShellStateUpdater>(services);
			ServiceCollectionDescriptorExtensions.TryAddScoped<IShellStateManager, NullShellStateManager>(services);
			dummyVar0 = ServiceCollectionServiceExtensions.AddScoped<IShellDescriptorManagerEventHandler, ShellStateCoordinator>(services);
			return;
		}

		public IServiceProvider CreateContainer(ShellSettings settings, ShellBlueprint blueprint)
		{
			V_0 = this._serviceProvider.CreateChildContainer(this._applicationServices);
			dummyVar0 = ServiceCollectionServiceExtensions.AddSingleton<ShellSettings>(V_0, settings);
			stackVariable8 = V_0;
			stackVariable9 = ShellContainerFactory.u003cu003ec.u003cu003e9__7_0;
			if (stackVariable9 == null)
			{
				dummyVar1 = stackVariable9;
				stackVariable9 = new Func<IServiceProvider, IShellConfiguration>(ShellContainerFactory.u003cu003ec.u003cu003e9.u003cCreateContaineru003eb__7_0);
				ShellContainerFactory.u003cu003ec.u003cu003e9__7_0 = stackVariable9;
			}
			dummyVar2 = ServiceCollectionServiceExtensions.AddSingleton<IShellConfiguration>(stackVariable8, stackVariable9);
			dummyVar3 = ServiceCollectionServiceExtensions.AddSingleton<ShellDescriptor>(V_0, blueprint.get_Descriptor());
			dummyVar4 = ServiceCollectionServiceExtensions.AddSingleton<ShellBlueprint>(V_0, blueprint);
			this.AddCoreServices(V_0);
			V_1 = this._serviceProvider.CreateChildContainer(this._applicationServices);
			stackVariable26 = blueprint.get_Dependencies();
			stackVariable27 = ShellContainerFactory.u003cu003ec.u003cu003e9__7_3;
			if (stackVariable27 == null)
			{
				dummyVar5 = stackVariable27;
				stackVariable27 = new Func<KeyValuePair<Type, FeatureEntry>, bool>(ShellContainerFactory.u003cu003ec.u003cu003e9.u003cCreateContaineru003eb__7_3);
				ShellContainerFactory.u003cu003ec.u003cu003e9__7_3 = stackVariable27;
			}
			V_6 = stackVariable26.Where<KeyValuePair<Type, FeatureEntry>>(stackVariable27).GetEnumerator();
			try
			{
				while (V_6.MoveNext())
				{
					V_7 = V_6.get_Current();
					ServiceCollectionDescriptorExtensions.TryAddEnumerable(V_1, ServiceDescriptor.Singleton(Type.GetTypeFromHandle(// 
					// Current member / type: System.IServiceProvider OrchardCore.Environment.Shell.Builders.ShellContainerFactory::CreateContainer(OrchardCore.Environment.Shell.ShellSettings,OrchardCore.Environment.Shell.Builders.Models.ShellBlueprint)
					// Exception in: System.IServiceProvider CreateContainer(OrchardCore.Environment.Shell.ShellSettings,OrchardCore.Environment.Shell.Builders.Models.ShellBlueprint)
					// Specified method is not supported.
					// 
					// mailto: JustDecompilePublicFeedback@telerik.com


		private void EnsureApplicationFeature()
		{
			if (this._applicationFeature == null)
			{
				V_0 = this;
				V_1 = false;
				try
				{
					Monitor.Enter(V_0, ref V_1);
					if (this._applicationFeature == null)
					{
						this._applicationFeature = this._extensionManager.GetFeatures().FirstOrDefault<IFeatureInfo>(new Func<IFeatureInfo, bool>(this.u003cEnsureApplicationFeatureu003eb__8_0));
					}
				}
				finally
				{
					if (V_1)
					{
						Monitor.Exit(V_0);
					}
				}
			}
			return;
		}
	}
}