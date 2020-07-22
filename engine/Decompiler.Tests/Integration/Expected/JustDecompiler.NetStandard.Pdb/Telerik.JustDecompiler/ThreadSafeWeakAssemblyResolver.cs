using Mono.Cecil;
using Mono.Cecil.AssemblyResolver;
using System;
using System.Collections.Generic;

namespace Telerik.JustDecompiler
{
	public class ThreadSafeWeakAssemblyResolver : WeakAssemblyResolver
	{
		private object resolvedAssembliesAccessLock;

		public ThreadSafeWeakAssemblyResolver(AssemblyPathResolverCache cache)
		{
			this.resolvedAssembliesAccessLock = new Object();
			base(cache);
			return;
		}

		protected override void AddToResolvedAssembliesInternal(AssemblyStrongNameExtended assemblyKey, List<AssemblyDefinition> assemblyList)
		{
			V_0 = this.resolvedAssembliesAccessLock;
			V_1 = false;
			try
			{
				Monitor.Enter(V_0, ref V_1);
				this.AddToResolvedAssembliesInternal(assemblyKey, assemblyList);
			}
			finally
			{
				if (V_1)
				{
					Monitor.Exit(V_0);
				}
			}
			return;
		}

		protected override void ClearResolvedAssembliesCache()
		{
			V_0 = this.resolvedAssembliesAccessLock;
			V_1 = false;
			try
			{
				Monitor.Enter(V_0, ref V_1);
				this.ClearResolvedAssembliesCache();
			}
			finally
			{
				if (V_1)
				{
					Monitor.Exit(V_0);
				}
			}
			return;
		}

		protected override void RemoveFromResolvedAssemblies(AssemblyStrongNameExtended assemblyKey)
		{
			V_0 = this.resolvedAssembliesAccessLock;
			V_1 = false;
			try
			{
				Monitor.Enter(V_0, ref V_1);
				this.RemoveFromResolvedAssemblies(assemblyKey);
			}
			finally
			{
				if (V_1)
				{
					Monitor.Exit(V_0);
				}
			}
			return;
		}

		protected override bool TryGetResolvedAssembly(AssemblyStrongNameExtended assemblyKey, out List<AssemblyDefinition> assemblyList)
		{
			V_0 = this.resolvedAssembliesAccessLock;
			V_1 = false;
			try
			{
				Monitor.Enter(V_0, ref V_1);
				V_2 = this.TryGetResolvedAssembly(assemblyKey, ref assemblyList);
			}
			finally
			{
				if (V_1)
				{
					Monitor.Exit(V_0);
				}
			}
			return V_2;
		}
	}
}