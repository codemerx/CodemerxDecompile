using Mono.Cecil;
using Mono.Cecil.AssemblyResolver;
using System;
using System.Collections.Generic;

namespace Telerik.JustDecompiler
{
	public class ThreadSafeWeakAssemblyResolver : WeakAssemblyResolver
	{
		private object resolvedAssembliesAccessLock = new Object();

		public ThreadSafeWeakAssemblyResolver(AssemblyPathResolverCache cache) : base(cache)
		{
		}

		protected override void AddToResolvedAssembliesInternal(AssemblyStrongNameExtended assemblyKey, List<AssemblyDefinition> assemblyList)
		{
			lock (this.resolvedAssembliesAccessLock)
			{
				base.AddToResolvedAssembliesInternal(assemblyKey, assemblyList);
			}
		}

		protected override void ClearResolvedAssembliesCache()
		{
			lock (this.resolvedAssembliesAccessLock)
			{
				base.ClearResolvedAssembliesCache();
			}
		}

		protected override void RemoveFromResolvedAssemblies(AssemblyStrongNameExtended assemblyKey)
		{
			lock (this.resolvedAssembliesAccessLock)
			{
				base.RemoveFromResolvedAssemblies(assemblyKey);
			}
		}

		protected override bool TryGetResolvedAssembly(AssemblyStrongNameExtended assemblyKey, out List<AssemblyDefinition> assemblyList)
		{
			bool flag;
			lock (this.resolvedAssembliesAccessLock)
			{
				flag = base.TryGetResolvedAssembly(assemblyKey, out assemblyList);
			}
			return flag;
		}
	}
}