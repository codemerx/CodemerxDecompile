//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// Copyright (c) 2008 - 2015 Jb Evain
// Copyright (c) 2008 - 2011 Novell, Inc.
//
// Licensed under the MIT/X11 license.
//

using System;
/*Telerik Authorship*/
using Mono.Cecil.AssemblyResolver;

namespace Mono.Cecil
{
    /*Telerik Authroship*/
    public static class GlobalAssemblyResolver
    {
        public static readonly AssemblyPathResolverCache CurrentAssemblyPathCache = new AssemblyPathResolverCache();

        public static readonly IAssemblyResolver Instance = new DefaultAssemblyResolver(CurrentAssemblyPathCache);
    }

    public class DefaultAssemblyResolver : BaseAssemblyResolver
    {
        public DefaultAssemblyResolver(AssemblyPathResolverCache pathRespository)
            : base(pathRespository, /*Telerik Authroship*/ TargetPlatformResolver.Instance) { }
    }
}
