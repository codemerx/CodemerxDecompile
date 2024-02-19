using System;
using System.IO;
using System.Text.RegularExpressions;
using Mono.Cecil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using System.Resources;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using Mono.Cecil.AssemblyResolver;
using Telerik.JustDecompiler.External.Interfaces;
using Telerik.JustDecompiler.External;
using Telerik.JustDecompiler.Common;
using JustDecompile.EngineInfrastructure;

namespace JustDecompile.Tools.MSBuildProjectBuilder
{
	public static class Utilities
	{
		public const int MaxPathLength = 259; // 259 + NULL == 260
		public static readonly char[] InvalidChars = new char[41] // Hardcoding invalid chars to get same result in different platforms
		{
			'"', '<', '>', '|', '\0', '\u0001', '\u0002', '\u0003', '\u0004', '\u0005',
			'\u0006', '\a', '\b', '\t', '\n', '\v', '\f', '\r', '\u000e', '\u000f',
			'\u0010', '\u0011', '\u0012', '\u0013', '\u0014', '\u0015', '\u0016', '\u0017', '\u0018', '\u0019',
			'\u001a', '\u001b', '\u001c', '\u001d', '\u001e', '\u001f', ':', '*', '?', '\\',
			'/'
		};
		public static string GetLegalFileName(string legalName)
		{
			if (legalName.Length == 0)
			{
				return string.Empty;
			}
			string regexSearch = new string(InvalidChars);

			Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));

			legalName = r.Replace(legalName, "_");

			return legalName.Length == 0 ? string.Empty : legalName;
		}

		public static string GetLegalFolderName(string nameSpace)
		{
			if (nameSpace.Length == 0)
			{
				return string.Empty;
			}
			string regexSearch = new string(InvalidChars);

			Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));

			nameSpace = r.Replace(nameSpace, string.Empty);

			return nameSpace.Length == 0 ? string.Empty : nameSpace;
		}

		// http://msdn.microsoft.com/en-us/library/ms379610(v=vs.80).aspx
		public static bool IsVbInternalTypeWithoutRootNamespace(TypeDefinition type)
		{
			return type.FullName == "My.MyApplication" ||
				   type.FullName == "My.MyComputer" ||
				   type.FullName == "My.MyProject";
		}

		public static bool IsVbInternalTypeWithRootNamespace(TypeDefinition type)
		{
			return type.FullName.EndsWith(".My.MyApplication") ||
				   type.FullName.EndsWith(".My.MyComputer") ||
				   type.FullName.EndsWith(".My.MyProject");
		}

		public static bool IsVBProjectSettingsType(TypeDefinition type)
		{
			return type.FullName.EndsWith("My.MySettings") ||
				   type.FullName.EndsWith("My.MySettingsProperty");
		}

		public static bool IsVBResourceType(TypeDefinition type)
		{
			return type.Namespace.EndsWith(".My.Resources");
		}

		public static bool IsVBResourceType(TypeDefinition type, out string resourceName)
		{
			resourceName = null;
			if (!IsVBResourceType(type))
			{
				return false;
			}

			resourceName = string.Format("{0}.{1}", type.Namespace.Substring(0, type.Namespace.Length - 13 /*".My.Resources".Length*/), type.Name);
			return true;
		}

		public static int GetResourcesCount(Dictionary<ModuleDefinition, Collection<Resource>> resources, bool decompileDangerousResources)
		{
			int result = 0;

			foreach (Collection<Resource> moduleResources in resources.Values)
			{
				foreach (Resource resource in moduleResources)
                {
                    if (!decompileDangerousResources && DangerousResourceIdentifier.IsDangerousResource(resource))
                    {
                        continue;
                    }

                    if (resource.ResourceType != ResourceType.Embedded)
					{
						continue;
					}

					EmbeddedResource embeddedResource = (EmbeddedResource)resource;
					if (resource.Name.EndsWith(".g.resources", StringComparison.OrdinalIgnoreCase))
					{
						using (ResourceReader resourceReader = new ResourceReader(embeddedResource.GetResourceStream()))
						{
                            result += resourceReader.OfType<System.Collections.DictionaryEntry>().Count();
						}
					}
					else
					{
						result++;
					}
				}
			}

			return result;
		}

		public static Dictionary<ModuleDefinition, Collection<TypeDefinition>> GetUserDefinedTypes(AssemblyDefinition assembly, bool decompileDangerousResources)
		{
			Dictionary<ModuleDefinition, Collection<TypeDefinition>> result =
				new Dictionary<ModuleDefinition, Collection<TypeDefinition>>();

			foreach (ModuleDefinition module in assembly.Modules)
			{
				Collection<TypeDefinition> allTypes = module.Types;
				Collection<TypeDefinition> moduleTypes = new Collection<TypeDefinition>(allTypes.Count);

				foreach (TypeDefinition typeDef in allTypes)
				{
					if (typeDef.Name == "<Module>" && typeDef.Namespace == string.Empty)
					{
						continue;
					}

                    if (assembly.Name.IsWindowsRuntime && IsWinRTXamlTypeInfoNamespace(typeDef.Namespace))
                    {
                        continue;
                    }

					//TODO: Refactor if there are other known skippable classes
					if (typeDef.FullName == "XamlGeneratedNamespace.GeneratedInternalTypeHelper" ||
						IsVbInternalTypeWithoutRootNamespace(typeDef)) // Namespace collision with compile generated classes when no RootNamespace is used
					{
						continue;
					}

					if (typeDef.HasCustomAttribute(new[] { "System.Runtime.CompilerServices.CompilerGeneratedAttribute" }))
					{
						if (!(IsSettingsType(typeDef) ||
							  IsVBProjectSettingsType(typeDef) ||
							  ExistsResourceForType(typeDef.FullName, assembly, decompileDangerousResources) ||
							  IsVBResourceType(typeDef)))
						{
							continue;
						}
					}
					moduleTypes.Add(typeDef);
				}

				result.Add(module, moduleTypes);
			}

			return result;
		}

        private static bool IsWinRTXamlTypeInfoNamespace(string @namespace)
        {
            string[] parts = @namespace.Split('.');
            if (parts.Length != 2)
            {
                return false;
            }

            return parts.Length == 2 && parts[0] + "_XamlTypeInfo" == parts[1];
        }

		public static Dictionary<ModuleDefinition, Collection<Resource>> GetResources(AssemblyDefinition assembly)
		{
			Dictionary<ModuleDefinition, Collection<Resource>> result =
				new Dictionary<ModuleDefinition, Collection<Resource>>();

			foreach (ModuleDefinition module in assembly.Modules)
			{
				result.Add(module, module.Resources);
			}

			return result;
		}

		private static bool IsSettingsType(TypeDefinition type)
		{
			foreach (CustomAttribute attribute in type.CustomAttributes)
			{
				if (attribute.AttributeType.FullName == "System.CodeDom.Compiler.GeneratedCodeAttribute")
				{
					attribute.Resolve();

					foreach (CustomAttributeArgument argument in attribute.ConstructorArguments)
					{
						if (argument.Type.FullName == "System.String" && (string)argument.Value == "Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator")
						{
							return true;
						}
					}
				}
			}

			return false;
		}


		private static bool ExistsResourceForType(string typeFullName, AssemblyDefinition assembly, bool decompileDangerousResources)
		{
			foreach (ModuleDefinition module in assembly.Modules)
			{
				foreach (Resource resource in module.Resources)
				{
                    if (!decompileDangerousResources && DangerousResourceIdentifier.IsDangerousResource(resource))
                    {
                        continue;
                    }

					if (resource.ResourceType != ResourceType.Embedded)
					{
						continue;
					}

					EmbeddedResource embeddedResource = (EmbeddedResource)resource;
					if (resource.Name.EndsWith(".resources", StringComparison.OrdinalIgnoreCase))
					{
						if (!embeddedResource.Name.EndsWith(".g.resources", StringComparison.OrdinalIgnoreCase))
						{
							string resourceName = embeddedResource.Name.Substring(0, embeddedResource.Name.Length - 10); //".resources".Length == 10
							if (resourceName == typeFullName)
							{
								return true;
							}
						}
					}
				}
			}

			return false;
		}

		public static int GetMaxRelativePathLength(string targetPath)
		{
			if (targetPath == "")
			{
				return MaxPathLength - 3; // "C:\"
			}

            string targetDirectoryName = Path.GetDirectoryName(targetPath);
			return MaxPathLength - (targetDirectoryName.EndsWith(@"\") ? targetDirectoryName.Length : targetDirectoryName.Length + 1);
		}

		public static string GetNetmoduleName(ModuleReference module)
		{
			if (module.Name.EndsWith(".netmodule"))
			{
				return module.Name.Substring(0, module.Name.Length - ".netmodule".Length);
			}
			else
			{
				return module.Name;
			}
		}

		public static string GetXamlResourceKey(System.Collections.DictionaryEntry xamlResource, ModuleDefinition module)
		{
			return module.Name + @"\" + (string)xamlResource.Key;
		}

		public static bool IsMainModule(ModuleDefinition module)
		{
			return module.Kind != ModuleKind.NetModule;
		}

		public static string GetModuleArchitecturePropertyValue(ModuleDefinition module, bool separateAnyCPU = false)
		{
			switch (module.GetModuleArchitecture())
			{
				case TargetArchitecture.AMD64:
					return "x64";
				case TargetArchitecture.I386:
					return "x86";
				case TargetArchitecture.IA64:
					return "Itanium";
                case TargetArchitecture.ARMv7:
                    return "ARM";
				default:
                    if (separateAnyCPU)
                    {
                        return "Any CPU";
                    }
                    else
                    {
					    return "AnyCPU";
                    }
			}
		}

		public static string GetXamlTypeFullName(XDocument xamlDoc)
		{
			XAttribute classAttribute = xamlDoc.Root.Attribute(XName.Get("Class", "http://schemas.microsoft.com/winfx/2006/xaml"));
			if (classAttribute != null && classAttribute.Name != null)
			{
				return classAttribute.Value;
			}
			return null;
		}

		public static ICollection<string> GetXamlGeneratedFields(XDocument xamlDoc)
		{
			HashSet<string> fieldsToRemove = new HashSet<string>();
			foreach (XElement element in xamlDoc.Descendants())
			{
				XAttribute xNameAttribute = element.Attribute(XName.Get("Name", "http://schemas.microsoft.com/winfx/2006/xaml"));
				if (xNameAttribute != null)
				{
					fieldsToRemove.Add(xNameAttribute.Value);
				}

				XAttribute nameAttribute = element.Attribute(XName.Get("Name", ""));
				if (nameAttribute != null)
				{
					fieldsToRemove.Add(nameAttribute.Value);
				}
			}
			return fieldsToRemove;
		}

		/// <summary>
		/// Decides if the given type should be decompiled as partial or not.
		/// </summary>
		/// <param name="type">The type in question.</param>
		/// <returns> Returns true, if some of the members of the type should be skipped, and the type must be declared partial.</returns>
		public static bool ShouldBePartial(TypeDefinition type)
		{
			AssemblyDefinition assembly = type.Module.Assembly;
			if (assembly.EntryPoint != null && assembly.EntryPoint.DeclaringType == type)
			{
				return false;
			}

			// The pattern is observed in wpf applications.
			// The type is marked with the GeneratedCode attribute
			// and all its generated members are marked with DebuggerNonUserCode
			if (!type.HasCustomAttribute(new string[1] { "System.CodeDom.Compiler.GeneratedCodeAttribute" }))
			{
				return false;
			}

			CustomAttribute generatedCodeCustomAttribute = type.CustomAttributes.First(x => x.AttributeType.FullName == "System.CodeDom.Compiler.GeneratedCodeAttribute");
			generatedCodeCustomAttribute.Resolve();
			if (generatedCodeCustomAttribute.ConstructorArguments[0].Type.FullName != "System.String" ||
				generatedCodeCustomAttribute.ConstructorArguments[0].Value.ToString() != "PresentationBuildTasks")
			{
				return false;
			}


			if (type.HasMethods)
			{
				foreach (MethodDefinition method in type.Methods)
				{
					if (method.HasCustomAttributes)
					{
						if (method.HasCustomAttribute(new string[1] { "System.Diagnostics.DebuggerNonUserCodeAttribute" }))
						{
							return true;
						}
					}
				}
			}

			if (type.HasProperties)
			{
				foreach (PropertyDefinition property in type.Properties)
				{
					if (property.HasCustomAttributes)
					{
						if (property.HasCustomAttribute(new string[1] { "System.Diagnostics.DebuggerNonUserCodeAttribute" }))
						{
							return true;
						}
					}
				}
			}

			if (type.HasFields)
			{
				foreach (FieldDefinition field in type.Fields)
				{
					if (field.HasCustomAttributes)
					{
						if (field.HasCustomAttribute(new string[1] { "System.Diagnostics.DebuggerNonUserCodeAttribute" }))
						{
							return true;
						}
					}

				}
			}

			return false;
		}
	}
}
