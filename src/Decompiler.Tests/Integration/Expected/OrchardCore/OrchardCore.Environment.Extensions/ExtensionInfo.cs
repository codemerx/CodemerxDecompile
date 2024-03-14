using OrchardCore.Environment.Extensions.Features;
using OrchardCore.Modules.Manifest;
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
				return this.Manifest.get_Exists();
			}
		}

		public IEnumerable<IFeatureInfo> Features
		{
			get;
		}

		public string Id
		{
			get
			{
				return this.Manifest.get_ModuleInfo().get_Id();
			}
		}

		public IManifestInfo Manifest
		{
			get;
		}

		public string SubPath
		{
			get;
		}

		public ExtensionInfo(string subPath, IManifestInfo manifestInfo, Func<IManifestInfo, IExtensionInfo, IEnumerable<IFeatureInfo>> features)
		{
			this.SubPath = subPath;
			this.Manifest = manifestInfo;
			this.Features = features(manifestInfo, this);
		}
	}
}