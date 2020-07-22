using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Languages.CSharp;
using Telerik.JustDecompiler.Languages.VisualBasic;
using Telerik.JustDecompiler.Languages.IL;
using System.Threading;
using Telerik.JustDecompiler.Decompiler.WriterContextServices;
using Telerik.JustDecompiler.Decompiler.Caching;
using Mono.Cecil.Extensions;
using System.IO;
using JustDecompile.Tools.MSBuildProjectBuilder;
using Telerik.JustDecompiler;
using Mono.Cecil.AssemblyResolver;
using Telerik.JustDecompiler.External.Interfaces;
using Telerik.JustDecompiler.External;
using JustDecompile.Tools.MSBuildProjectBuilder.ProjectFileManagers;
using JustDecompile.EngineInfrastructure;
using Telerik.JustDecompiler.Common.NamespaceHierarchy;

namespace JustDecompile.External.JustAssembly
{
	public static class Decompiler
	{
		public static bool IsValidCLRAssembly(string assemblyFilePath)
		{
			if (string.IsNullOrEmpty(assemblyFilePath) || !File.Exists(assemblyFilePath))
			{
				return false;
			}

			WeakAssemblyResolver assemblyResolver = new WeakAssemblyResolver(new AssemblyPathResolverCache());

			AssemblyDefinition assembly = assemblyResolver.GetAssemblyDefinition(assemblyFilePath);

			return assembly != null;
		}

		public static ICollection<uint> GetAssemblyModules(string assemblyFilePath)
		{
			AssemblyDefinition assembly = GlobalAssemblyResolver.Instance.GetAssemblyDefinition(assemblyFilePath);

			if (assembly != null)
			{
				return assembly.Modules.Select(x => x.MetadataToken.ToUInt32()).ToList();
			}

			return new List<uint>();
		}

		public static string GetModuleName(string assemblyFilePath, uint moduleToken)
		{
			ModuleDefinition module = GetModuleDefinition(assemblyFilePath, moduleToken);
			return module.Name;
		}

		public static ICollection<uint> GetModuleTypes(string assemblyFilePath, uint moduleToken)
		{
			ModuleDefinition module = GetModuleDefinition(assemblyFilePath, moduleToken);

			if (module != null)
			{
				return module.Types.Select(x => x.MetadataToken.ToUInt32()).ToList();
			}

			return new List<uint>();
		}

		public static ICollection<Tuple<MemberType, uint>> GetTypeMembers(string assemblyFilePath, uint moduleToken, uint typeToken, SupportedLanguage language)
		{
			ICollection<Tuple<MemberType, uint>> result = new List<Tuple<MemberType, uint>>();

			TypeDefinition type = GetTypeDefinition(assemblyFilePath, moduleToken, typeToken);

			IEnumerable<string> attributesToSkip = null;
			if (Utilities.ShouldBePartial(type))
			{
				attributesToSkip = new string[1] { "System.CodeDom.Compiler.GeneratedCodeAttribute" };
			}

			ModuleDefinition module = GetModuleDefinition(assemblyFilePath, moduleToken);

			IEnumerable<IMemberDefinition> typeMembers = type.GetMembersSorted(false, GetLanguage(language), attributesToSkip, null);

			return typeMembers.Select(x => new Tuple<MemberType, uint>(GetMemberType(x), x.MetadataToken.ToUInt32())).ToList();
		}

		public static string GetTypeNamespace(string assemblyFilePath, uint moduleToken, uint typeToken)
		{
			TypeDefinition type = GetTypeDefinition(assemblyFilePath, moduleToken, typeToken);
			return type.Namespace;
		}

		public static byte[] GetImageData(string assemblyFilePath, uint moduleToken)
		{
			ModuleDefinition module = GetModuleDefinition(assemblyFilePath, moduleToken);
			return module.GetCleanImageData();
		}

		public static string GetTypeName(string assemblyFilePath, uint moduleToken, uint typeToken, SupportedLanguage language)
		{
			TypeDefinition type = GetTypeDefinition(assemblyFilePath, moduleToken, typeToken);
			ILanguage decompilerLanguage = GetLanguage(language);
			return MemberNamingUtils.GetMemberDeclartionForLanguage(type, decompilerLanguage, true);
		}

		public static string GetTypeFullName(string assemblyFilePath, uint moduleToken, uint typeToken, SupportedLanguage language)
		{
			return GetTypeNamespace(assemblyFilePath, moduleToken, typeToken) + "." + GetTypeName(assemblyFilePath, moduleToken, typeToken, language);
		}

		public static string GetMemberName(string assemblyFilePath, uint moduleToken, uint typeToken, uint memberToken, SupportedLanguage language)
		{
			IMemberDefinition member = GetMember(assemblyFilePath, moduleToken, typeToken, memberToken, language);
			ILanguage decompilerLanguage = GetLanguage(language);
			return MemberNamingUtils.GetMemberDeclartionForLanguage(member, decompilerLanguage, true);
		}

		public static ICollection<IPropertyMethod> GetPropertyMethods(string assemblyFilePath, uint moduleToken, uint typeToken, uint propertyToken, SupportedLanguage language)
		{
            IMemberDefinition member = GetMember(assemblyFilePath, moduleToken, typeToken, propertyToken, language);

			if (!(member is PropertyDefinition))
			{
				throw new NotSupportedException("Member is not a property.");
			}

			PropertyDefinition property = member as PropertyDefinition;

			ICollection<IPropertyMethod> result = new List<IPropertyMethod>();

			if (property.GetMethod != null)
			{
				result.Add(new PropertyMethod(PropertyMethodType.GetMethod, property.GetMethod.MetadataToken.ToUInt32()));
			}

			if (property.SetMethod != null)
			{
				result.Add(new PropertyMethod(PropertyMethodType.SetMethod, property.SetMethod.MetadataToken.ToUInt32()));
			}

			return result;
		}

		public static ICollection<IEventMethod> GetEventMethods(string assemblyFilePath, uint moduleToken, uint typeToken, uint eventToken, SupportedLanguage language)
		{
			IMemberDefinition member = GetMember(assemblyFilePath, moduleToken, typeToken, eventToken, language);

			if (!(member is EventDefinition))
			{
				throw new NotSupportedException("Member is not an event.");
			}

			EventDefinition @event = member as EventDefinition;

			ICollection<IEventMethod> result = new List<IEventMethod>();

			if (@event.AddMethod != null)
			{
				result.Add(new EventMethod(EventMethodType.AddMethod, @event.AddMethod.MetadataToken.ToUInt32()));
			}

			if (@event.RemoveMethod != null)
			{
				result.Add(new EventMethod(EventMethodType.RemoveMethod, @event.RemoveMethod.MetadataToken.ToUInt32()));
			}

			if (@event.InvokeMethod != null)
			{
				result.Add(new EventMethod(EventMethodType.InvokeMethod, @event.InvokeMethod.MetadataToken.ToUInt32()));
			}

			return result;
		}

		public static AccessModifier GetTypeAccessModifier(string assemblyFilePath, uint moduleToken, uint typeToken)
		{
			TypeDefinition type = GetTypeDefinition(assemblyFilePath, moduleToken, typeToken);

			if (type.IsPublic)
				return AccessModifier.Public;
			else if (type.IsNestedPublic)
				return AccessModifier.Public;
			else if (type.IsNestedFamily)
				return AccessModifier.Protected;
			else if (type.IsNestedPrivate)
				return AccessModifier.Private;
			else if (type.IsNestedAssembly)
				return AccessModifier.Internal;
			else if (type.IsNestedFamilyOrAssembly)
				return AccessModifier.Protected;
			else if (type.IsNestedFamilyAndAssembly)
				return AccessModifier.Internal;
			else if (type.IsNotPublic)
				return AccessModifier.Internal;
			else
				throw new NotSupportedException("Unexpected type modifier.");
		}

		public static AccessModifier GetMemberAccessModifier(string assemblyFilePath, uint moduleToken, uint typeToken, uint memberToken, SupportedLanguage language)
		{
			IMemberDefinition member = GetMember(assemblyFilePath, moduleToken, typeToken, memberToken, language);

			if (member is MethodDefinition)
			{
				return GetVisibilityDefinitionAccessModifiers(member as MethodDefinition);
			}

			if (member is FieldDefinition)
			{
				return GetVisibilityDefinitionAccessModifiers(member as FieldDefinition);
			}

			if (member is PropertyDefinition)
			{
				PropertyDefinition property = member as PropertyDefinition;
				MethodDefinition moreVisibleMethod = property.GetMethod.GetMoreVisibleMethod(property.SetMethod);
				return GetVisibilityDefinitionAccessModifiers(moreVisibleMethod);
			}

			if (member is EventDefinition)
			{
				EventDefinition @event = member as EventDefinition;
				MethodDefinition moreVisibleMethod = @event.AddMethod.GetMoreVisibleMethod(@event.RemoveMethod);
				return GetVisibilityDefinitionAccessModifiers(moreVisibleMethod);
			}

			throw new NotSupportedException("Unexpected member type.");
		}

		private static AccessModifier GetVisibilityDefinitionAccessModifiers(IVisibilityDefinition member)
		{
			if (member.IsPrivate)
				return AccessModifier.Private;
			else if (member.IsPublic)
				return AccessModifier.Public;
			else if (member.IsFamily)
				return AccessModifier.Protected;
			else if (member.IsAssembly)
				return AccessModifier.Internal;
			else if (member.IsFamilyOrAssembly)
				return AccessModifier.Protected;
			else if (member.IsFamilyAndAssembly)
				return AccessModifier.Internal;
			else if ((member is MethodDefinition && (member as MethodDefinition).IsCompilerControlled) || (member is FieldDefinition && (member as FieldDefinition).IsCompilerControlled))
				return AccessModifier.Internal;
			else
				throw new NotSupportedException("Unexpected member modifier.");
		}

		public static ICollection<string> GetTypeAttributes(string assemblyFilePath, uint moduleToken, uint typeToken, SupportedLanguage language)
		{
			TypeDefinition type = GetTypeDefinition(assemblyFilePath, moduleToken, typeToken);

			StringWriter stringWriter = new StringWriter();
			AttributeWriter attributeWriter = GetAttributeWriter(type, language, stringWriter);
			attributeWriter.WriteMemberAttributesAndNewLine(type);

			return stringWriter.ToString().Split('\n');
		}

		public static ICollection<string> GetMemberAttributes(string assemblyFilePath, uint moduleToken, uint typeToken, uint memberToken, SupportedLanguage language)
		{
			IMemberDefinition member = GetMember(assemblyFilePath, moduleToken, typeToken, memberToken, language);

			StringWriter stringWriter = new StringWriter();
			AttributeWriter attributeWriter = GetAttributeWriter(member, language, stringWriter);
			attributeWriter.WriteMemberAttributesAndNewLine(member);

			return stringWriter.ToString().Split('\n').Where(s => !string.IsNullOrEmpty(s)).ToList();
		}

		private static AttributeWriter GetAttributeWriter(IMemberDefinition member, SupportedLanguage language, StringWriter stringWriter)
		{
			ILanguage decompilerLangauge = GetLanguage(language);
			IFormatter formatter = new PlainTextFormatter(stringWriter);
            IWriterSettings settings = new WriterSettings(writeExceptionsAsComments: true);

            switch (language)
			{
				case SupportedLanguage.CSharp:
					{
						JustAssemblyCSharpWriter cSharpWriter =
							new JustAssemblyCSharpWriter(member, new SimpleWriterContextService(new EmptyDecompilationCacheService(), true), decompilerLangauge, formatter, SimpleExceptionFormatter.Instance, settings);
						return new CSharpAttributeWriter(cSharpWriter);
					}
				case SupportedLanguage.VB:
					{
						JustAssemblyVisualBasicWriter visualBasicWriter = 
							new JustAssemblyVisualBasicWriter(member, new SimpleWriterContextService(new EmptyDecompilationCacheService(), true), decompilerLangauge, formatter, SimpleExceptionFormatter.Instance, settings);
						return new VisualBasicAttributeWriter(visualBasicWriter);
					}
				default:
					throw new NotSupportedException("Not supported AttributeWriter language.");
			}
		}

		public static int GetMaximumPossibleTargetPathLength(string assemblyFilePath, SupportedLanguage language, bool decompileDangerousResources)
		{
			AssemblyDefinition assembly = GlobalAssemblyResolver.Instance.GetAssemblyDefinition(assemblyFilePath);
			ILanguage decompilerLanguage = GetLanguage(language);
			JustAssemblyFilePathsAnalyzer filePathsAnalyzer = new JustAssemblyFilePathsAnalyzer(assembly, decompilerLanguage, decompileDangerousResources);
			return filePathsAnalyzer.GetMaximumPossibleTargetPathLength();
		}

		public static IAssemblyDecompilationResults GenerateFiles(string assemblyFilePath, AssemblyDefinition assembly, string targetPath, SupportedLanguage language,  CancellationToken cancellationToken, bool decompileDangerousResources, IFileGenerationNotifier notifier = null)
		{
			ILanguage decompilerLanguage = GetLanguage(language);
			string csprojFileName = Path.ChangeExtension(Path.GetFileName(assemblyFilePath), decompilerLanguage.VSProjectFileExtension);
			string csprojTargetPath = Path.Combine(targetPath, csprojFileName);

            Dictionary<ModuleDefinition, Mono.Collections.Generic.Collection<TypeDefinition>> assemblyUserDefinedTypes = Utilities.GetUserDefinedTypes(assembly, decompileDangerousResources);
            Dictionary<ModuleDefinition, Mono.Collections.Generic.Collection<Resource>> assemblyResources = Utilities.GetResources(assembly);

            IDecompilationPreferences decompilationPreferences = new DecompilationPreferences()
            {
                DecompileDangerousResources = decompileDangerousResources
            };

            JustAssemblyProjectBuilder projectBuilder;

            if (notifier != null)
            {
                projectBuilder = new JustAssemblyProjectBuilder(assemblyFilePath, assembly, assemblyUserDefinedTypes, assemblyResources, csprojTargetPath, decompilerLanguage, decompilationPreferences, new FileGenerationNotifier(notifier));
            }
            else
            {
                projectBuilder = new JustAssemblyProjectBuilder(assemblyFilePath, assembly, assemblyUserDefinedTypes, assemblyResources, csprojTargetPath, decompilerLanguage, decompilationPreferences, null);
            }

            return projectBuilder.GenerateFiles(cancellationToken);
		}

		internal static IMemberDefinition GetMember(string assemblyFilePath, uint moduleToken, uint typeToken, uint memberToken, SupportedLanguage language)
		{
			MembersCacheKey key = new MembersCacheKey() { AssemblyFilePath = assemblyFilePath, ModuleToken = moduleToken, TypeToken = typeToken, MemberToken = memberToken };

			IMemberDefinition result;
			if (!MembersCache.TryGetValue(key, out result))
			{
				TypeDefinition type = GetTypeDefinition(assemblyFilePath, moduleToken, typeToken);
				IEnumerable<IMemberDefinition> typeMembers = type.GetMembersSorted(true, GetLanguage(language));
				result = typeMembers.Where(x => x.MetadataToken.ToUInt32() == memberToken).FirstOrDefault();
				MembersCache.Add(key, result);
			}

			return result;
		}

		private static MemberType GetMemberType(IMemberDefinition member)
		{
			if (member is FieldDefinition)
			{
				return MemberType.Field;
			}

			if (member is PropertyDefinition)
			{
				return MemberType.Property;
			}

			if (member is MethodDefinition)
			{
				return MemberType.Method;
			}

			if (member is TypeDefinition)
			{
				return MemberType.Type;
			}

			if (member is EventDefinition)
			{
				return MemberType.Event;
			}

			throw new Exception("Unexpected member type.");
		}

		private static ModuleDefinition GetModuleDefinition(string assemblyFilePath, uint moduleToken)
		{
			AssemblyDefinition assembly = GlobalAssemblyResolver.Instance.GetAssemblyDefinition(assemblyFilePath);
			return assembly.Modules.Where(x => x.MetadataToken.ToUInt32() == moduleToken).FirstOrDefault();
		}

		// !!!!!!!!!!!!!!!!!!!!!!!!!!! Implemented only for faster test & debug.  Should be removed or implemented smarter !!!!!!!!!!!!!!!!!!!!!!!!!!!
		class TypesCacheKey : IEquatable<TypesCacheKey>
		{
			public string AssemblyFilePath { get; set; }
			public uint ModuleToken { get; set; }
			public uint TypeToken { get; set; }

			public bool Equals(TypesCacheKey other)
			{
				return this.AssemblyFilePath == other.AssemblyFilePath && this.ModuleToken == other.ModuleToken && this.TypeToken == other.TypeToken;
			}

			public override int GetHashCode()
			{
				return this.AssemblyFilePath.GetHashCode() + this.ModuleToken.GetHashCode() + this.TypeToken.GetHashCode();
			}
		}

		static Dictionary<MembersCacheKey, IMemberDefinition> MembersCache = new Dictionary<MembersCacheKey, IMemberDefinition>();

		class MembersCacheKey : IEquatable<MembersCacheKey>
		{
			public string AssemblyFilePath { get; set; }
			public uint ModuleToken { get; set; }
			public uint TypeToken { get; set; }
			public uint MemberToken { get; set; }

			public bool Equals(MembersCacheKey other)
			{
				return this.AssemblyFilePath == other.AssemblyFilePath && this.ModuleToken == other.ModuleToken && this.TypeToken == other.TypeToken && this.MemberToken == other.MemberToken;
			}

			public override int GetHashCode()
			{
				return this.AssemblyFilePath.GetHashCode() + this.ModuleToken.GetHashCode() + this.TypeToken.GetHashCode() + this.MemberToken.GetHashCode();
			}
		}

        class JustAssemblyProjectBuilderFrameworkVersionResolver : IFrameworkResolver
        {
            public FrameworkVersion GetDefaultFallbackFramework4Version()
            {
                return FrameworkVersion.v4_0;
            }
        }

        static Dictionary<TypesCacheKey, TypeDefinition> TypesCache = new Dictionary<TypesCacheKey, TypeDefinition>();
		// !!!!!!!!!!!!!!!!!!!!!!!!!!! Implemented only for faster test & debug.  Should be removed or implemented smarter !!!!!!!!!!!!!!!!!!!!!!!!!!!

		private static TypeDefinition GetTypeDefinition(string assemblyFilePath, uint moduleToken, uint typeToken)
		{
			TypesCacheKey key = new TypesCacheKey() { AssemblyFilePath = assemblyFilePath, ModuleToken = moduleToken, TypeToken = typeToken };
			TypeDefinition result;

			if (!TypesCache.TryGetValue(key, out result))
			{
				ModuleDefinition module = GetModuleDefinition(assemblyFilePath, moduleToken);
				result = module.AllTypes.Where(x => x.MetadataToken.ToUInt32() == typeToken).FirstOrDefault();
				TypesCache.Add(key, result);
			}

			return result;
		}

		private static ILanguage GetLanguage(SupportedLanguage language)
		{
			switch (language)
			{
				case SupportedLanguage.CSharp:
                    return LanguageFactory.GetLanguage(CSharpVersion.V6);
				case SupportedLanguage.VB:
                    return LanguageFactory.GetLanguage(VisualBasicVersion.V10);
				case SupportedLanguage.MSIL:
					return new IntermediateLanguage();
				default:
					throw new NotSupportedException("Unexpected language");
			}
		}

		class JustAssemblyCSharpWriter : CSharpWriter
		{
			public JustAssemblyCSharpWriter(IMemberDefinition member, IWriterContextService writerContextService, 
				ILanguage language, IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings)
				: base(language, formatter, exceptionFormatter, settings)
			{
				this.writerContextService = writerContextService;
				this.writerContext = this.writerContextService.GetWriterContext(member, language);
			}
		}

		class JustAssemblyVisualBasicWriter : VisualBasicWriter
		{
			public JustAssemblyVisualBasicWriter(IMemberDefinition member, IWriterContextService writerContextService, 
				ILanguage language, IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings)
				: base(language, formatter, exceptionFormatter, settings)
			{
				this.writerContextService = writerContextService;
				this.writerContext = this.writerContextService.GetWriterContext(member, language);
			}
		}

	}
}
