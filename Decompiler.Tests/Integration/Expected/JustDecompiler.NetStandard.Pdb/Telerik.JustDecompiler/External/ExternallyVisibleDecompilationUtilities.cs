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
			V_0 = new Dictionary<TypeDefinition, string>();
			if (originalMapping == null)
			{
				return V_0;
			}
			V_1 = originalMapping.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_3 = V_2.get_Key();
					V_4 = ExternallyVisibleDecompilationUtilities.ResolveTypeInAssembly(ExternallyVisibleDecompilationUtilities.ResolveAssembly(V_3.get_Assembly()), V_3.get_UniqueMemberIdentifier());
					V_0.Add(V_4, V_2.get_Value());
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			return V_0;
		}

		public static Dictionary<MemberIdentifier, CodeSpan> GenerateMemberMapping(string assemblyFilePath, StringWriter writer, List<WritingInfo> writingInfos)
		{
			stackVariable1 = writer.ToString();
			V_0 = new Dictionary<MemberIdentifier, CodeSpan>();
			V_1 = new TwoDimensionalString(stackVariable1, writer.get_NewLine(), false);
			V_2 = writingInfos.GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current().get_MemberDeclarationToCodePostionMap().GetEnumerator();
					try
					{
						while (V_3.MoveNext())
						{
							V_4 = V_3.get_Current();
							V_5 = V_4.get_Key();
							V_6 = V_4.get_Value();
							V_7 = ExternallyVisibleDecompilationUtilities.GetSpan(V_1, V_1.TrimStart(V_6));
							V_0.Add(ExternallyVisibleDecompilationUtilities.GetIdentifier(assemblyFilePath, V_5), V_7);
						}
					}
					finally
					{
						((IDisposable)V_3).Dispose();
					}
				}
			}
			finally
			{
				((IDisposable)V_2).Dispose();
			}
			return V_0;
		}

		private static ModuleDefinition GetAssemblyModule(AssemblyDefinition assembly, string moduleFilePath)
		{
			V_0 = assembly.get_Modules().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (!String.Equals(V_1.get_FilePath(), moduleFilePath, 5))
					{
						continue;
					}
					V_2 = V_1;
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				V_0.Dispose();
			}
		Label1:
			return V_2;
		Label0:
			throw new Exception("Module not found in assembly.");
		}

		public static MemberIdentifier GetIdentifier(string assemblyFilePath, IMemberDefinition member)
		{
			V_0 = member as TypeDefinition;
			if (V_0 == null)
			{
				V_0 = member.get_DeclaringType();
			}
			stackVariable4 = new AssemblyIdentifier(assemblyFilePath);
			stackVariable7 = V_0.get_Module().get_FilePath();
			V_1 = member.get_MetadataToken();
			return new MemberIdentifier(stackVariable4, new UniqueMemberIdentifier(stackVariable7, V_1.ToInt32()));
		}

		private static CodeSpan GetSpan(TwoDimensionalString twoDString, OffsetSpan position)
		{
			stackVariable3 = twoDString.GetTwoDimensionalCordinatesFor(position.StartOffset);
			return new CodeSpan(stackVariable3, twoDString.GetTwoDimensionalCordinatesFor(position.EndOffset));
		}

		public static AssemblyDefinition ResolveAssembly(AssemblyIdentifier assembly)
		{
			V_0 = GlobalAssemblyResolver.Instance;
			return V_0.LoadAssemblyDefinition(assembly.get_AssemblyPath(), new ReaderParameters(V_0, 2), true);
		}

		public static IMemberDefinition ResolveMemberByToken(AssemblyDefinition assembly, IUniqueMemberIdentifier uniqueMemberIdentifier)
		{
			return ExternallyVisibleDecompilationUtilities.GetAssemblyModule(assembly, uniqueMemberIdentifier.get_ModuleFilePath()).LookupToken(uniqueMemberIdentifier.get_MetadataToken()) as IMemberDefinition;
		}

		public static TypeDefinition ResolveTypeInAssembly(AssemblyDefinition assembly, IUniqueMemberIdentifier uniqueMemberIdentifier)
		{
			return ExternallyVisibleDecompilationUtilities.ResolveMemberByToken(assembly, uniqueMemberIdentifier) as TypeDefinition;
		}
	}
}