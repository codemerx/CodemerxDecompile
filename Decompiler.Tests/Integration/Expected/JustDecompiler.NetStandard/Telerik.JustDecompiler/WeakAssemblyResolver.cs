using Mono.Cecil;
using Mono.Cecil.AssemblyResolver;
using System;

namespace Telerik.JustDecompiler
{
	public class WeakAssemblyResolver : DefaultAssemblyResolver
	{
		public WeakAssemblyResolver(AssemblyPathResolverCache cache)
		{
			base(new WeakAssemblyResolver.WeakAssemblyCache(cache));
			return;
		}

		public override string FindAssemblyPath(AssemblyName assemblyName, string fallbackDir, AssemblyStrongNameExtended assemblyKey, bool bubbleToUserIfFailed = true)
		{
			return this.FindAssemblyPath(assemblyName, fallbackDir, assemblyKey, false);
		}

		public override AssemblyDefinition Resolve(AssemblyNameReference name, string path, TargetArchitecture architecture, SpecialTypeAssembly special, bool bubbleToUserIfFailed = true)
		{
			return this.Resolve(name, path, architecture, special, false);
		}

		public override AssemblyDefinition Resolve(string fullName, ReaderParameters parameters, TargetArchitecture platform, SpecialTypeAssembly special, bool bubbleToUserIfFailed = true)
		{
			return this.Resolve(fullName, parameters, platform, special, false);
		}

		public override AssemblyDefinition Resolve(AssemblyNameReference name, string path, TargetArchitecture architecture, SpecialTypeAssembly special, bool addToFailedCache, bool bubbleToUserIfFailed = true)
		{
			return this.Resolve(name, path, architecture, special, addToFailedCache, false);
		}

		private class WeakAssemblyCache : AssemblyPathResolverCache
		{
			public WeakAssemblyCache(AssemblyPathResolverCache cache)
			{
				base();
				this.assemblyFaildedResolver = cache.get_AssemblyFaildedResolverCache().Clone();
				this.assemblyNameDefinition = new Dictionary<string, AssemblyName>(cache.get_AssemblyNameDefinition());
				this.assemblyParts = new Dictionary<string, TargetPlatform>(cache.get_AssemblyParts());
				this.assemblyPathArchitecture = new Dictionary<string, TargetArchitecture>(cache.get_AssemblyPathArchitecture());
				this.assemblyPathName = new List<KeyValuePair<AssemblyStrongNameExtended, string>>(cache.get_AssemblyPathName());
				return;
			}
		}
	}
}