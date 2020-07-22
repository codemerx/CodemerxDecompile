using OrchardCore.Environment.Extensions.Features;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace OrchardCore.Environment.Extensions
{
	public class ExtensionInfo : IExtensionInfo
	{
		public bool Exists
		{
			get
			{
				return this.get_Manifest().get_Exists();
			}
		}

		public IEnumerable<IFeatureInfo> Features
		{
			get
			{
				return this.u003cFeaturesu003ek__BackingField;
			}
		}

		public string Id
		{
			get
			{
				return this.get_Manifest().get_ModuleInfo().get_Id();
			}
		}

		public IManifestInfo Manifest
		{
			get
			{
				return this.u003cManifestu003ek__BackingField;
			}
		}

		public string SubPath
		{
			get
			{
				return this.u003cSubPathu003ek__BackingField;
			}
		}

		public ExtensionInfo(string subPath, IManifestInfo manifestInfo, Func<IManifestInfo, IExtensionInfo, IEnumerable<IFeatureInfo>> features)
		{
			base();
			this.u003cSubPathu003ek__BackingField = subPath;
			this.u003cManifestu003ek__BackingField = manifestInfo;
			this.u003cFeaturesu003ek__BackingField = features.Invoke(manifestInfo, this);
			return;
		}
	}
}