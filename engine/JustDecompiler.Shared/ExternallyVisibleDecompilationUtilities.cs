using System;
using System.Collections.Generic;
using System.IO;
using Mono.Cecil;
using Telerik.JustDecompiler.External.Interfaces;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.External
{
	/// <summary>
	/// Class contains utility methods dealing with the interactions between the outer API and the internal representation.
	/// </summary>
	public static class ExternallyVisibleDecompilationUtilities
	{
		public static AssemblyDefinition ResolveAssembly(AssemblyIdentifier assembly)
		{
			IAssemblyResolver resolver = GlobalAssemblyResolver.Instance;
			return resolver.LoadAssemblyDefinition(assembly.AssemblyPath, new ReaderParameters(resolver, ReadingMode.Deferred), true);
		}

		public static Dictionary<TypeDefinition, string> ConvertMapping(Dictionary<MemberIdentifier, string> originalMapping)
		{
			Dictionary<TypeDefinition, string> result = new Dictionary<TypeDefinition, string>();
			if (originalMapping == null)
			{
				return result;
			}
			foreach (KeyValuePair<MemberIdentifier, string> memberPair in originalMapping)
			{
				MemberIdentifier memberId = memberPair.Key;
				AssemblyDefinition assembly = ResolveAssembly(memberId.Assembly);
				TypeDefinition typeDefinition = ResolveTypeInAssembly(assembly, memberId.UniqueMemberIdentifier);
				string path = memberPair.Value;

				result.Add(typeDefinition, path);
			}
			return result;
		}

		public static IMemberDefinition ResolveMemberByToken(AssemblyDefinition assembly, IUniqueMemberIdentifier uniqueMemberIdentifier)
		{
			ModuleDefinition module = GetAssemblyModule(assembly, uniqueMemberIdentifier.ModuleFilePath);
			IMetadataTokenProvider result = module.LookupToken(uniqueMemberIdentifier.MetadataToken);
			return result as IMemberDefinition;
		}

		private static ModuleDefinition GetAssemblyModule(AssemblyDefinition assembly, string moduleFilePath)
		{
			foreach (ModuleDefinition module in assembly.Modules)
			{
				if (String.Equals(module.FilePath, moduleFilePath, StringComparison.OrdinalIgnoreCase))
				{
					return module;
				}
			}

			throw new Exception("Module not found in assembly.");
		}

		public static TypeDefinition ResolveTypeInAssembly(AssemblyDefinition assembly, IUniqueMemberIdentifier uniqueMemberIdentifier)
		{
			IMetadataTokenProvider result = ExternallyVisibleDecompilationUtilities.ResolveMemberByToken(assembly, uniqueMemberIdentifier);
			return result as TypeDefinition;
		}

		public static Dictionary<MemberIdentifier, CodeSpan> GenerateMemberMapping(string assemblyFilePath, StringWriter writer, List<WritingInfo> writingInfos)
		{
			string theText = writer.ToString();

			Dictionary<MemberIdentifier, CodeSpan> mapping = new Dictionary<MemberIdentifier, CodeSpan>();

			TwoDimensionalString twoDString = new TwoDimensionalString(theText, writer.NewLine, false);

			foreach (WritingInfo memberInfo in writingInfos)
			{
				foreach (KeyValuePair<IMemberDefinition, OffsetSpan> item in memberInfo.MemberDeclarationToCodePostionMap)
				{
					IMemberDefinition member = item.Key;
					OffsetSpan position = item.Value;

					// Get/set keywords are printed alone in a lane.
					// This should trim the tabulations before them.
					position = twoDString.TrimStart(position);

					CodeSpan codeSpan = GetSpan(twoDString, position);

					mapping.Add(GetIdentifier(assemblyFilePath, member), codeSpan);
				}
			}
			return mapping;
		}

		private static CodeSpan GetSpan(TwoDimensionalString twoDString, OffsetSpan position)
		{
			CodePosition start = twoDString.GetTwoDimensionalCordinatesFor(position.StartOffset);
			CodePosition end = twoDString.GetTwoDimensionalCordinatesFor(position.EndOffset);

			return new CodeSpan(start, end);
		}

		public static MemberIdentifier GetIdentifier(string assemblyFilePath, IMemberDefinition member)
		{
			TypeDefinition td = member as TypeDefinition;
			if (td == null)
			{
				td = member.DeclaringType;
			}

			MemberIdentifier result = new MemberIdentifier(new AssemblyIdentifier(assemblyFilePath), new UniqueMemberIdentifier(td.Module.FilePath, member.MetadataToken.ToInt32()));
			return result;
		}
	}
}