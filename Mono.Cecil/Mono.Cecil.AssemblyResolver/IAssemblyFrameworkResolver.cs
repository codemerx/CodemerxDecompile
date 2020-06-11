using System;
using System.Collections.Generic;

namespace Mono.Cecil.AssemblyResolver
{
    public interface IAssemblyFrameworkResolver
    {
        FrameworkVersion GetFrameworkVersionForModule(ModuleDefinition moduleDef);

        bool IsCLR4Assembly(ModuleDefinition module);
    }
}
