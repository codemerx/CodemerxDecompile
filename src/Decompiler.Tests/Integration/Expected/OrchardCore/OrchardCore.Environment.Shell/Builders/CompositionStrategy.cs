using Microsoft.Extensions.Logging;
using OrchardCore.Environment.Extensions;
using OrchardCore.Environment.Extensions.Features;
using OrchardCore.Environment.Shell;
using OrchardCore.Environment.Shell.Builders.Models;
using OrchardCore.Environment.Shell.Descriptor.Models;
using OrchardCore.Modules;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace OrchardCore.Environment.Shell.Builders
{
	public class CompositionStrategy : ICompositionStrategy
	{
		private readonly IExtensionManager _extensionManager;

		private readonly ILogger _logger;

		public CompositionStrategy(IExtensionManager extensionManager, ILogger<CompositionStrategy> logger)
		{
			this._extensionManager = extensionManager;
			this._logger = logger;
		}

		public async Task<ShellBlueprint> ComposeAsync(ShellSettings settings, ShellDescriptor descriptor)
		{
			Func<string, bool> func = null;
			if (this._logger.IsEnabled(1))
			{
				LoggerExtensions.LogDebug(this._logger, "Composing blueprint", Array.Empty<object>());
			}
			IList<ShellFeature> features = descriptor.get_Features();
			string[] array = (
				from x in features
				select x.get_Id()).ToArray<string>();
			IEnumerable<FeatureEntry> featureEntries = await this._extensionManager.LoadFeaturesAsync(array);
			Dictionary<Type, FeatureEntry> types = new Dictionary<Type, FeatureEntry>();
			foreach (FeatureEntry featureEntry in featureEntries)
			{
				foreach (Type exportedType in featureEntry.get_ExportedTypes())
				{
					IList<string> requiredFeatureNamesForType = RequireFeaturesAttribute.GetRequiredFeatureNamesForType(exportedType);
					Func<string, bool> func1 = func;
					if (func1 == null)
					{
						Func<string, bool> func2 = (string x) => array.Contains<string>(x);
						Func<string, bool> func3 = func2;
						func = func2;
						func1 = func3;
					}
					if (!requiredFeatureNamesForType.All<string>(func1))
					{
						continue;
					}
					types.Add(exportedType, featureEntry);
				}
			}
			ShellBlueprint shellBlueprint = new ShellBlueprint();
			shellBlueprint.set_Settings(settings);
			shellBlueprint.set_Descriptor(descriptor);
			shellBlueprint.set_Dependencies(types);
			if (this._logger.IsEnabled(1))
			{
				LoggerExtensions.LogDebug(this._logger, "Done composing blueprint", Array.Empty<object>());
			}
			return shellBlueprint;
		}
	}
}