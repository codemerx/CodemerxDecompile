using System;
using System.IO;
using System.Linq;
using Mono.Cecil;

namespace Telerik.JustDecompiler
{
    /// <summary>
    /// Defines member to load the target type to decompile.
    /// </summary>
    public static class AssemblyLoader
    {
        public static AssemblyDefinition LoadAssembly(string fileName, ReaderParameters parameters)
        {
            try
            {
                SetSymbolStore(fileName, parameters);
                if (File.Exists(fileName))
                {
                    return AssemblyDefinition.ReadAssembly(fileName, parameters);
                }
                return null;
            }
            finally
            {
                if (parameters.SymbolStream != null)
                    parameters.SymbolStream.Dispose();
            }
        }

        static void SetSymbolStore(string fileName, ReaderParameters p)
        {
            // search for pdb in same directory as dll
            string pdbName = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + ".pdb");
            if (File.Exists(pdbName))
            {
                p.ReadSymbols = true;
                p.SymbolStream = File.OpenRead(pdbName);
            }
            // TODO : include microsoft symbol store.
        }
    }
}
