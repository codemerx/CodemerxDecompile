using System;
using System.IO;
using Mono.Cecil.PE;

namespace Mono.Cecil.Extensions
{
    public static class ModuleDefinitionExtensions
    {
        public static TargetArchitecture? GetNullableModuleArchitecture(this ModuleDefinition module)
        {
            if (module == null)
            {
                return null;
            }

            return module.GetModuleArchitecture();
        }

        public static TargetArchitecture GetModuleArchitecture(this ModuleDefinition module)
        {
            if (module == null)
            {
                throw new ArgumentNullException("module");
            }

            bool is32Bit = (module.Attributes & ModuleAttributes.Required32Bit) == ModuleAttributes.Required32Bit;
            if (module.Architecture == TargetArchitecture.I386 && !is32Bit)
            {
                return TargetArchitecture.AnyCPU;
            }
            else
            {
                return module.Architecture;
            }
        }

        public static bool CanReference(this TargetArchitecture self, TargetArchitecture other)
        {
            return  self == other || self == TargetArchitecture.AnyCPU || other == TargetArchitecture.AnyCPU;
        }

        public static byte[] GetCleanImageData(this ModuleDefinition moduleDefinition)
        {
            byte[] imageBytes = File.ReadAllBytes(moduleDefinition.Image.FileName);

            Section guidSection = moduleDefinition.Image.GuidHeap.Section;

            byte[] emptyBytes = new byte[guidSection.SizeOfRawData];

            Buffer.BlockCopy(emptyBytes, 0, imageBytes, moduleDefinition.Image.TimeDateStampPosition, 4);

            Buffer.BlockCopy(emptyBytes, 0, imageBytes, moduleDefinition.Image.FileChecksumPosition, 4);

            //MVID 
            Buffer.BlockCopy(emptyBytes, 0, imageBytes, (int)guidSection.PointerToRawData, (int)guidSection.SizeOfRawData);

            return imageBytes;
        }
    }
}
