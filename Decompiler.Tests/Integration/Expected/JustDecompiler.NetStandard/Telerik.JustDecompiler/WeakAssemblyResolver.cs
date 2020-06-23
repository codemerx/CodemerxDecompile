using Mono.Cecil;
using Mono.Cecil.AssemblyResolver;
using System;
using System.Collections.Generic;

namespace Telerik.JustDecompiler
{
	public class WeakAssemblyResolver : DefaultAssemblyResolver
	{
		public WeakAssemblyResolver(AssemblyPathResolverCache cache) : base(new WeakAssemblyResolver.WeakAssemblyCache(cache))
		{
		}

		public override string FindAssemblyPath(AssemblyName assemblyName, string fallbackDir, AssemblyStrongNameExtended assemblyKey, bool bubbleToUserIfFailed = true)
		{
			return base.FindAssemblyPath(assemblyName, fallbackDir, assemblyKey, false);
		}

		public override AssemblyDefinition Resolve(AssemblyNameReference name, string path, TargetArchitecture architecture, SpecialTypeAssembly special, bool bubbleToUserIfFailed = true)
		{
			return base.Resolve(name, path, architecture, special, false);
		}

		public override AssemblyDefinition Resolve(string fullName, ReaderParameters parameters, TargetArchitecture platform, SpecialTypeAssembly special, bool bubbleToUserIfFailed = true)
		{
			return base.Resolve(fullName, parameters, platform, special, false);
		}

		public override AssemblyDefinition Resolve(AssemblyNameReference name, string path, TargetArchitecture architecture, SpecialTypeAssembly special, bool addToFailedCache, bool bubbleToUserIfFailed = true)
		{
			return base.Resolve(name, path, architecture, special, addToFailedCache, false);
		}

		private class WeakAssemblyCache : AssemblyPathResolverCache
		{
			public WeakAssemblyCache(AssemblyPathResolverCache cache)
			{
				this.assemblyFaildedResolver = cache.AssemblyFaildedResolverCache.Clone();
				this.assemblyNameDefinition = new Dictionary<string, AssemblyName>(cache.AssemblyNameDefinition);
				this.assemblyParts = new Dictionary<string, TargetPlatform>(cache.AssemblyParts);
				this.assemblyPathArchitecture = new Dictionary<string, TargetArchitecture>(cache.AssemblyPathArchitecture);
				this.assemblyPathName = new List<KeyValuePair<AssemblyStrongNameExtended, string>>(cache.AssemblyPathName);
			}
		}
	}
}