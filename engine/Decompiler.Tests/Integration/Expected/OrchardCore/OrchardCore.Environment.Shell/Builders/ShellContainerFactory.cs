using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using OrchardCore.Environment.Extensions;
using OrchardCore.Environment.Extensions.Features;
using OrchardCore.Environment.Shell;
using OrchardCore.Environment.Shell.Builders.Models;
using OrchardCore.Environment.Shell.Configuration;
using OrchardCore.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
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
			this._hostingEnvironment = hostingEnvironment;
			this._extensionManager = extensionManager;
			this._applicationServices = applicationServices;
			this._serviceProvider = serviceProvider;
		}

		public void AddCoreServices(IServiceCollection services)
		{
			ServiceCollectionDescriptorExtensions.TryAddScoped<IShellStateUpdater, ShellStateUpdater>(services);
			ServiceCollectionDescriptorExtensions.TryAddScoped<IShellStateManager, NullShellStateManager>(services);
			ServiceCollectionServiceExtensions.AddScoped<IShellDescriptorManagerEventHandler, ShellStateCoordinator>(services);
		}

		public IServiceProvider CreateContainer(ShellSettings settings, ShellBlueprint blueprint)
		{
			FeatureEntry featureEntry;
			IFeatureInfo featureInfo;
			IServiceCollection serviceCollection = this._serviceProvider.CreateChildContainer(this._applicationServices);
			ServiceCollectionServiceExtensions.AddSingleton<ShellSettings>(serviceCollection, settings);
			ServiceCollectionServiceExtensions.AddSingleton<IShellConfiguration>(serviceCollection, (IServiceProvider sp) => ServiceProviderServiceExtensions.GetRequiredService<ShellSettings>(sp).get_ShellConfiguration());
			ServiceCollectionServiceExtensions.AddSingleton<ShellDescriptor>(serviceCollection, blueprint.get_Descriptor());
			ServiceCollectionServiceExtensions.AddSingleton<ShellBlueprint>(serviceCollection, blueprint);
			this.AddCoreServices(serviceCollection);
			IServiceCollection serviceCollection1 = this._serviceProvider.CreateChildContainer(this._applicationServices);
			foreach (KeyValuePair<Type, FeatureEntry> keyValuePair in 
				from t in blueprint.get_Dependencies()
				where typeof(IStartup).IsAssignableFrom(t.Key)
				select t)
			{
				ServiceCollectionDescriptorExtensions.TryAddEnumerable(serviceCollection1, ServiceDescriptor.Singleton(typeof(IStartup), keyValuePair.Key));
				ServiceCollectionDescriptorExtensions.TryAddEnumerable(serviceCollection, ServiceDescriptor.Singleton(typeof(IStartup), keyValuePair.Key));
			}
			this.EnsureApplicationFeature();
			foreach (Type type in 
				from t in blueprint.get_Dependencies().Keys
				where t.Name == "Startup"
				select t)
			{
				if (typeof(IStartup).IsAssignableFrom(type) || blueprint.get_Dependencies().TryGetValue(type, out featureEntry) && featureEntry.get_FeatureInfo().get_Id() == this._applicationFeature.get_Id())
				{
					continue;
				}
				MethodInfo method = type.GetMethod("ConfigureServices", BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.Any, new Type[] { typeof(IServiceCollection) }, null);
				MethodInfo methodInfo = type.GetMethod("Configure", BindingFlags.Instance | BindingFlags.Public);
				PropertyInfo property = type.GetProperty("Order", BindingFlags.Instance | BindingFlags.Public);
				PropertyInfo propertyInfo = type.GetProperty("ConfigureOrder", BindingFlags.Instance | BindingFlags.Public);
				ServiceCollectionServiceExtensions.AddSingleton(serviceCollection1, type);
				ServiceCollectionServiceExtensions.AddSingleton(serviceCollection, type);
				ServiceCollectionServiceExtensions.AddSingleton<IStartup>(serviceCollection1, (IServiceProvider sp) => new StartupBaseMock(sp.GetService(type), method, methodInfo, property, propertyInfo));
				ServiceCollectionServiceExtensions.AddSingleton<IStartup>(serviceCollection, (IServiceProvider sp) => new StartupBaseMock(sp.GetService(type), method, methodInfo, property, propertyInfo));
			}
			ServiceCollectionServiceExtensions.AddSingleton<ShellSettings>(serviceCollection1, settings);
			ServiceCollectionServiceExtensions.AddSingleton<IShellConfiguration>(serviceCollection1, (IServiceProvider sp) => ServiceProviderServiceExtensions.GetRequiredService<ShellSettings>(sp).get_ShellConfiguration());
			ServiceProvider serviceProvider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(serviceCollection1, true);
			FeatureAwareServiceCollection featureAwareServiceCollections = new FeatureAwareServiceCollection(serviceCollection);
			foreach (IStartup startup in 
				from s in ServiceProviderServiceExtensions.GetServices<IStartup>(serviceProvider)
				orderby s.get_Order()
				select s)
			{
				KeyValuePair<Type, FeatureEntry> keyValuePair1 = blueprint.get_Dependencies().FirstOrDefault<KeyValuePair<Type, FeatureEntry>>((KeyValuePair<Type, FeatureEntry> x) => x.Key == startup.GetType());
				FeatureEntry value = keyValuePair1.Value;
				if (value != null)
				{
					featureInfo = value.get_FeatureInfo();
				}
				else
				{
					featureInfo = null;
				}
				featureAwareServiceCollections.SetCurrentFeature(featureInfo ?? this._applicationFeature);
				startup.ConfigureServices(featureAwareServiceCollections);
			}
			serviceProvider.Dispose();
			ServiceProvider serviceProvider1 = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(serviceCollection, true);
			ITypeFeatureProvider requiredService = ServiceProviderServiceExtensions.GetRequiredService<ITypeFeatureProvider>(serviceProvider1);
			foreach (KeyValuePair<IFeatureInfo, ServiceCollection> featureCollection in featureAwareServiceCollections.FeatureCollections)
			{
				foreach (ServiceDescriptor serviceDescriptor in featureCollection.Value)
				{
					Type implementationType = ServiceDescriptorExtensions.GetImplementationType(serviceDescriptor);
					if (implementationType == null)
					{
						continue;
					}
					IFeatureInfo key = featureCollection.Key;
					if ((object)key == (object)this._applicationFeature)
					{
						FeatureAttribute featureAttribute = implementationType.GetCustomAttributes<FeatureAttribute>(false).FirstOrDefault<FeatureAttribute>();
						if (featureAttribute != null)
						{
							key = featureCollection.Key.get_Extension().get_Features().FirstOrDefault<IFeatureInfo>((IFeatureInfo f) => f.get_Id() == featureAttribute.get_FeatureName()) ?? key;
						}
					}
					requiredService.TryAdd(implementationType, key);
				}
			}
			return serviceProvider1;
		}

		private void EnsureApplicationFeature()
		{
			if (this._applicationFeature == null)
			{
				lock (this)
				{
					if (this._applicationFeature == null)
					{
						this._applicationFeature = this._extensionManager.GetFeatures().FirstOrDefault<IFeatureInfo>((IFeatureInfo f) => f.get_Id() == this._hostingEnvironment.get_ApplicationName());
					}
				}
			}
		}
	}
}