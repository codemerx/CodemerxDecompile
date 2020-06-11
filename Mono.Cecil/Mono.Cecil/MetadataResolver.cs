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

using Mono.Collections.Generic;
/*Telerik authorship*/
using Mono.Cecil.AssemblyResolver;
using System.IO;
using System.Collections.Generic;
using Mono.Cecil.Extensions;

namespace Mono.Cecil {

	public interface IAssemblyResolver {

		/*Telerik Authorship*/
		AssemblyDefinition Resolve(AssemblyNameReference name, string path, TargetArchitecture platform, SpecialTypeAssembly special, bool provideFailedEventToUser = true);
		AssemblyDefinition Resolve(AssemblyNameReference name, string path, TargetArchitecture platform, SpecialTypeAssembly special, bool addToFailedCache, bool provideFailedEventToUser = true);
		AssemblyDefinition Resolve(string fullName, ReaderParameters parameters, TargetArchitecture platform, SpecialTypeAssembly special, bool provideFailedEventToUser = true);

		/*Telerik Authorship*/
		AssemblyDefinition GetAssemblyDefinition(string filePath);

        /*Telerik Authorship*/
		string ResolveAssemblyPath(string strongName, SpecialTypeAssembly special);

		/*Telerik Authorship*/
		event AssemblyResolveEventHandler ResolveFailure;
		event AssemblyDefinitionFailureEventHandler AssemblyDefinitionFailure;
		void AddToAssemblyCache(string filePath, TargetArchitecture platform, bool storeAssemblyDefInCahce = false);
        /*Telerik Authorship*/
		string FindAssemblyPath(AssemblyName assemblyNameReference, string fallbackDir, AssemblyStrongNameExtended assemblyKey, bool bubbleToUserIfFailed = true);
		TargetPlatform GetTargetPlatform(string assemblyFilePath);
		void ClearCache();
		void AddSearchDirectory(string directory);
		void RemoveSearchDirectory(string directory);
		void RemoveFromAssemblyCache(string fileName);
		void RemoveFromFailedAssemblies(AssemblyStrongNameExtended assemblyName);

		/*Telerik Authorship*/
		IEnumerable<AssemblyStrongNameExtended> GetNotResolvedAssemblyNames();
		IEnumerable<string> GetUserDefiniedAssemblies();
        /*Telerik Authorship*/
        void SetNotResolvedAssembliesForCurrentSession(IList<AssemblyStrongNameExtended> list);
		void AddResolvedAssembly(string filePath);
		void ClearAssemblyFailedResolverCache();
		AssemblyDefinition LoadAssemblyDefinition(string filePath, ReaderParameters parameters, bool loadPdb);
	}

	public interface IMetadataResolver {
		TypeDefinition Resolve (TypeReference type);
		FieldDefinition Resolve (FieldReference field);
		MethodDefinition Resolve (MethodReference method);
	}

#if !SILVERLIGHT && !CF
	[Serializable]
#endif
	public class ResolutionException : Exception {

		readonly MemberReference member;

		public MemberReference Member {
			get { return member; }
		}

		public IMetadataScope Scope {
			get {
				var type = member as TypeReference;
				if (type != null)
					return type.Scope;

				var declaring_type = member.DeclaringType;
				if (declaring_type != null)
					return declaring_type.Scope;

				throw new NotSupportedException ();
			}
		}

		public ResolutionException (MemberReference member)
			: base ("Failed to resolve " + member.FullName)
		{
			if (member == null)
				throw new ArgumentNullException ("member");

			this.member = member;
		}

#if !SILVERLIGHT && !CF
		protected ResolutionException (
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context)
			: base (info, context)
		{
		}
#endif
	}

	public class MetadataResolver : IMetadataResolver {

		readonly IAssemblyResolver assembly_resolver;

		public IAssemblyResolver AssemblyResolver {
			get { return assembly_resolver; }
		}

		public MetadataResolver (IAssemblyResolver assemblyResolver)
		{
			if (assemblyResolver == null)
				throw new ArgumentNullException ("assemblyResolver");

			assembly_resolver = assemblyResolver;
		}

		public virtual TypeDefinition Resolve (TypeReference type)
		{
			/*Telerik Authorship*/
			return this.Resolve(type, new HashSet<string>());
		}

		/*Telerik Authorship*/
		internal TypeDefinition Resolve(TypeReference type, ICollection<string> visitedDlls)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			// TODO: The following code must be uncommented when bug 284860 (the one with the Resolver) is fixed.
			//if (type is ArrayType)
			//{
			//	type = type.Module.TypeSystem.LookupType("System", "Array");
			//}
			//else
			//{
			type = type.GetElementType();
			//}

			var scope = type.Scope;

			if (scope == null)
				return null;

			switch (scope.MetadataScopeType) {
				case MetadataScopeType.AssemblyNameReference:
					/*Telerik Authorship*/
					TargetArchitecture architecture = type.Module.GetModuleArchitecture();
                    SpecialTypeAssembly special = type.Module.IsReferenceAssembly() ? SpecialTypeAssembly.Reference : SpecialTypeAssembly.None;
					var assembly = assembly_resolver.Resolve((AssemblyNameReference)scope, type.Module.ModuleDirectoryPath, architecture, special);
					if (assembly == null)
						return null;

					/*Telerik Authorship*/
					if (visitedDlls.Contains(assembly.MainModule.FilePath))
					{
						return null;
					}
					visitedDlls.Add(assembly.MainModule.FilePath);

					return GetType(assembly.MainModule, type, visitedDlls);
				case MetadataScopeType.ModuleDefinition:
					/*Telerik Authorship*/
					ModuleDefinition theModule = (ModuleDefinition)scope;
					if (visitedDlls.Contains(theModule.FilePath))
					{
						return null;
					}
					visitedDlls.Add(theModule.FilePath);

					return GetType(theModule, type, visitedDlls);
				case MetadataScopeType.ModuleReference:
					var modules = type.Module.Assembly.Modules;
					var module_ref = (ModuleReference)scope;
					for (int i = 0; i < modules.Count; i++)
					{
						var netmodule = modules[i];
						if (netmodule.Name == module_ref.Name)
						{
							/*Telerik Authorship*/
							if (visitedDlls.Contains(netmodule.FilePath))
							{
								return null;
							}
							visitedDlls.Add(netmodule.FilePath);
							return GetType(netmodule, type, visitedDlls);
						}
					}
					break;
			}

			throw new NotSupportedException();
		}

		static TypeDefinition GetType(ModuleDefinition module, TypeReference reference, /*Telerik Authorship*/ ICollection<string> visitedDlls)
		{
			var type = GetTypeDefinition (module, reference);
			if (type != null)
				return type;

			if (!module.HasExportedTypes)
				return null;

			var exported_types = module.ExportedTypes;

			for (int i = 0; i < exported_types.Count; i++) {
				var exported_type = exported_types [i];
				if (exported_type.Name != reference.Name)
					continue;

				if (exported_type.Namespace != reference.Namespace)
					continue;

					/*Telerik Authorship*/
					TypeDefinition resolved = exported_type.Resolve(visitedDlls);
					if (resolved == null)
					{
						reference.Scope = exported_type.Scope;
					}
					return resolved;
			}

			return null;
		}

		static TypeDefinition GetTypeDefinition (ModuleDefinition module, TypeReference type)
		{
			if (!type.IsNested)
				return module.GetType (type.Namespace, type.Name);

			var declaring_type = type.DeclaringType.Resolve ();
			if (declaring_type == null)
				return null;

			return declaring_type.GetNestedType (type.TypeFullName ());
		}

		public virtual FieldDefinition Resolve (FieldReference field)
		{
			if (field == null)
				throw new ArgumentNullException ("field");

			var type = Resolve (field.DeclaringType);
			if (type == null)
				return null;

			if (!type.HasFields)
				return null;

			return GetField (type, field);
		}

		FieldDefinition GetField (TypeDefinition type, FieldReference reference)
		{
			while (type != null) {
				var field = GetField (type.Fields, reference);
				if (field != null)
					return field;

				if (type.BaseType == null)
					return null;

				type = Resolve (type.BaseType);
			}

			return null;
		}

		static FieldDefinition GetField (Collection<FieldDefinition> fields, FieldReference reference)
		{
			for (int i = 0; i < fields.Count; i++) {
				var field = fields [i];

				if (field.Name != reference.Name)
					continue;

				if (!AreSame (field.FieldType, reference.FieldType))
					continue;

				return field;
			}

			return null;
		}

		public virtual MethodDefinition Resolve (MethodReference method)
		{
			if (method == null)
				throw new ArgumentNullException ("method");

			var type = Resolve (method.DeclaringType);
			if (type == null)
				return null;

			method = method.GetElementMethod ();

			if (!type.HasMethods)
				return null;

			return GetMethod (type, method);
		}

		MethodDefinition GetMethod (TypeDefinition type, MethodReference reference)
		{
			while (type != null) {
				var method = GetMethod (type.Methods, reference);
				if (method != null)
					return method;

				if (type.BaseType == null)
					return null;

				type = Resolve (type.BaseType);
			}

			return null;
		}

		public static MethodDefinition GetMethod (Collection<MethodDefinition> methods, MethodReference reference)
		{
			for (int i = 0; i < methods.Count; i++) {
				var method = methods [i];

				if (method.Name != reference.Name)
					continue;

				if (method.HasGenericParameters != reference.HasGenericParameters)
					continue;

				if (method.HasGenericParameters && method.GenericParameters.Count != reference.GenericParameters.Count)
					continue;

				if (!AreSame (method.ReturnType, reference.ReturnType))
					continue;

				if (method.HasParameters != reference.HasParameters)
					continue;

				if (!method.HasParameters && !reference.HasParameters)
					return method;

				if (!AreSame (method.Parameters, reference.Parameters))
					continue;

				return method;
			}

			return null;
		}

		static bool AreSame (Collection<ParameterDefinition> a, Collection<ParameterDefinition> b)
		{
			var count = a.Count;

			if (count != b.Count)
				return false;

			if (count == 0)
				return true;

			for (int i = 0; i < count; i++)
				if (!AreSame (a [i].ParameterType, b [i].ParameterType))
					return false;

			return true;
		}

		static bool AreSame (TypeSpecification a, TypeSpecification b)
		{
			if (!AreSame (a.ElementType, b.ElementType))
				return false;

			if (a.IsGenericInstance)
				return AreSame ((GenericInstanceType) a, (GenericInstanceType) b);

			if (a.IsRequiredModifier || a.IsOptionalModifier)
				return AreSame ((IModifierType) a, (IModifierType) b);

			if (a.IsArray)
				return AreSame ((ArrayType) a, (ArrayType) b);

			return true;
		}

		static bool AreSame (ArrayType a, ArrayType b)
		{
			if (a.Rank != b.Rank)
				return false;

			// TODO: dimensions

			return true;
		}

		static bool AreSame (IModifierType a, IModifierType b)
		{
			return AreSame (a.ModifierType, b.ModifierType);
		}

		static bool AreSame (GenericInstanceType a, GenericInstanceType b)
		{
			if (a.GenericArguments.Count != b.GenericArguments.Count)
				return false;

			for (int i = 0; i < a.GenericArguments.Count; i++)
				if (!AreSame (a.GenericArguments [i], b.GenericArguments [i]))
					return false;

			return true;
		}

		static bool AreSame (GenericParameter a, GenericParameter b)
		{
			return a.Position == b.Position;
		}

		static bool AreSame (TypeReference a, TypeReference b)
		{
			if (ReferenceEquals (a, b))
				return true;

			if (a == null || b == null)
				return false;

			if (a.etype != b.etype)
				return false;

			if (a.IsGenericParameter)
				return AreSame ((GenericParameter) a, (GenericParameter) b);

			if (a.IsTypeSpecification ())
				return AreSame ((TypeSpecification) a, (TypeSpecification) b);

			if (a.Name != b.Name || a.Namespace != b.Namespace)
				return false;

			//TODO: check scope

			return AreSame (a.DeclaringType, b.DeclaringType);
		}
	}
}
