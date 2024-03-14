using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using Telerik.JustDecompiler;
using Telerik.JustDecompiler.External.Interfaces;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.External
{
	public static class ExternallyVisibleDecompilationUtilities
	{
		public static Dictionary<TypeDefinition, string> ConvertMapping(Dictionary<MemberIdentifier, string> originalMapping)
		{
			Dictionary<TypeDefinition, string> typeDefinitions = new Dictionary<TypeDefinition, string>();
			if (originalMapping == null)
			{
				return typeDefinitions;
			}
			foreach (KeyValuePair<MemberIdentifier, string> keyValuePair in originalMapping)
			{
				MemberIdentifier key = keyValuePair.Key;
				TypeDefinition typeDefinition = ExternallyVisibleDecompilationUtilities.ResolveTypeInAssembly(ExternallyVisibleDecompilationUtilities.ResolveAssembly(key.Assembly), key.UniqueMemberIdentifier);
				typeDefinitions.Add(typeDefinition, keyValuePair.Value);
			}
			return typeDefinitions;
		}

		public static Dictionary<MemberIdentifier, CodeSpan> GenerateMemberMapping(string assemblyFilePath, StringWriter writer, List<WritingInfo> writingInfos)
		{
			string str = writer.ToString();
			Dictionary<MemberIdentifier, CodeSpan> memberIdentifiers = new Dictionary<MemberIdentifier, CodeSpan>();
			TwoDimensionalString twoDimensionalString = new TwoDimensionalString(str, writer.NewLine, false);
			foreach (WritingInfo writingInfo in writingInfos)
			{
				foreach (KeyValuePair<IMemberDefinition, OffsetSpan> memberDeclarationToCodePostionMap in writingInfo.MemberDeclarationToCodePostionMap)
				{
					IMemberDefinition key = memberDeclarationToCodePostionMap.Key;
					OffsetSpan value = memberDeclarationToCodePostionMap.Value;
					CodeSpan span = ExternallyVisibleDecompilationUtilities.GetSpan(twoDimensionalString, twoDimensionalString.TrimStart(value));
					memberIdentifiers.Add(ExternallyVisibleDecompilationUtilities.GetIdentifier(assemblyFilePath, key), span);
				}
			}
			return memberIdentifiers;
		}

		private static ModuleDefinition GetAssemblyModule(AssemblyDefinition assembly, string moduleFilePath)
		{
			ModuleDefinition moduleDefinition;
			Collection<ModuleDefinition>.Enumerator enumerator = assembly.get_Modules().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ModuleDefinition current = enumerator.get_Current();
					if (!String.Equals(current.get_FilePath(), moduleFilePath, StringComparison.OrdinalIgnoreCase))
					{
						continue;
					}
					moduleDefinition = current;
					return moduleDefinition;
				}
				throw new Exception("Module not found in assembly.");
			}
			finally
			{
				enumerator.Dispose();
			}
			return moduleDefinition;
		}

		public static MemberIdentifier GetIdentifier(string assemblyFilePath, IMemberDefinition member)
		{
			TypeDefinition typeDefinition = member as TypeDefinition ?? member.get_DeclaringType();
			AssemblyIdentifier assemblyIdentifier = new AssemblyIdentifier(assemblyFilePath);
			string filePath = typeDefinition.get_Module().get_FilePath();
			MetadataToken metadataToken = member.get_MetadataToken();
			return new MemberIdentifier(assemblyIdentifier, new UniqueMemberIdentifier(filePath, metadataToken.ToInt32()));
		}

		private static CodeSpan GetSpan(TwoDimensionalString twoDString, OffsetSpan position)
		{
			return new CodeSpan(twoDString.GetTwoDimensionalCordinatesFor(position.StartOffset), twoDString.GetTwoDimensionalCordinatesFor(position.EndOffset));
		}

		public static AssemblyDefinition ResolveAssembly(AssemblyIdentifier assembly)
		{
			IAssemblyResolver instance = GlobalAssemblyResolver.Instance;
			return instance.LoadAssemblyDefinition(assembly.AssemblyPath, new ReaderParameters(instance, 2), true);
		}

		public static IMemberDefinition ResolveMemberByToken(AssemblyDefinition assembly, IUniqueMemberIdentifier uniqueMemberIdentifier)
		{
			return ExternallyVisibleDecompilationUtilities.GetAssemblyModule(assembly, uniqueMemberIdentifier.ModuleFilePath).LookupToken(uniqueMemberIdentifier.MetadataToken) as IMemberDefinition;
		}

		public static TypeDefinition ResolveTypeInAssembly(AssemblyDefinition assembly, IUniqueMemberIdentifier uniqueMemberIdentifier)
		{
			return ExternallyVisibleDecompilationUtilities.ResolveMemberByToken(assembly, uniqueMemberIdentifier) as TypeDefinition;
		}
	}
}