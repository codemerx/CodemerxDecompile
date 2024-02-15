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
using System.IO;

using Mono.Collections.Generic;

namespace Mono.Cecil {

	public sealed class AssemblyDefinition : ICustomAttributeProvider, ISecurityDeclarationProvider {

		AssemblyNameDefinition name;

		internal ModuleDefinition main_module;
		Collection<ModuleDefinition> modules;
		Collection<CustomAttribute> custom_attributes;
		Collection<SecurityDeclaration> security_declarations;

		public AssemblyNameDefinition Name {
			get { return name; }
			set { name = value; }
		}

		public string FullName {
			get { return name != null ? name.FullName : string.Empty; }
		}

		public MetadataToken MetadataToken {
			get { return new MetadataToken (TokenType.Assembly, 1); }
			set { }
		}

		public Collection<ModuleDefinition> Modules {
			get {
				if (modules != null)
					return modules;

				if (main_module.HasImage)
				{
					/*Telerik Authorship*/
					main_module.Read(ref modules, this, (_, reader) => reader.ReadModules(main_module.AssemblyResolver));

					/*Telerik Authorship*/
					foreach (ModuleDefinition module in modules)
					{
						if (module != main_module)
						{
							module.Assembly = this;
						}
					}
					
					/*Telerik Authorship*/
					return modules;
				}

				return modules = new Collection<ModuleDefinition> (1) { main_module };
			}
		}

		public ModuleDefinition MainModule {
			get { return main_module; }
		}

		public MethodDefinition EntryPoint {
			get { return main_module.EntryPoint; }
			set { main_module.EntryPoint = value; }
		}

		/*Telerik Authorship*/
		private bool? hasCustomAttributes;
		public bool HasCustomAttributes
		{
			get
			{
				if (custom_attributes != null)
					return custom_attributes.Count > 0;

				/*Telerik Authorship*/
				if (hasCustomAttributes != null)
					return hasCustomAttributes == true;

				/*Telerik Authorship*/
				return this.GetHasCustomAttributes(ref hasCustomAttributes, main_module);
			}
		}

        /*Telerik Authorship*/
        private string targetFrameworkAttributeValue;
        /*Telerik Authorship*/
        public string TargetFrameworkAttributeValue
        {
            get
            {
                if (this.targetFrameworkAttributeValue == null)
                {
                    this.targetFrameworkAttributeValue = this.GetTargetFrameworkAttributeValue();
                }

                if (this.targetFrameworkAttributeValue == string.Empty)
                {
                    return null;
                }

                return this.targetFrameworkAttributeValue;
            }
        }

        /*Telerik Authorship*/
        /// <summary>
        /// Get the value of assembly's target framework attribute.
        /// </summary>
        /// <returns>Returns string.Empty if the attribute is not present or with invalid value. Otherwise returns it's value.</returns>
        private string GetTargetFrameworkAttributeValue()
        {
            foreach (CustomAttribute customAttr in this.CustomAttributes)
            {
                if (customAttr.AttributeType.FullName == "System.Runtime.Versioning.TargetFrameworkAttribute")
                {
                    if (!customAttr.IsResolved)
                    {
                        customAttr.Resolve();
                    }

                    if (customAttr.ConstructorArguments.Count == 0)
                    {
                        // sanity check.
                        return string.Empty;
                    }

                    string versionString = customAttr.ConstructorArguments[0].Value as string;
                    if (string.IsNullOrWhiteSpace(versionString))
                    {
                        return string.Empty;
                    }

                    return versionString;
                }
            }

            return string.Empty;
        }

		public Collection<CustomAttribute> CustomAttributes {
			get { return custom_attributes ?? (this.GetCustomAttributes (ref custom_attributes, main_module)); }
		}

		/*Telerik Authorship*/
		private bool? hasSecurityDeclarations;
		public bool HasSecurityDeclarations {
			get {
				if (security_declarations != null)
					return security_declarations.Count > 0;

				/*Telerik Authorship*/
				if (hasSecurityDeclarations != null)
					return hasSecurityDeclarations == true;

				/*Telerik Authorship*/
				return this.GetHasSecurityDeclarations (ref hasSecurityDeclarations, main_module);
			}
		}

		public Collection<SecurityDeclaration> SecurityDeclarations {
			get { return security_declarations ?? (this.GetSecurityDeclarations (ref security_declarations, main_module)); }
		}

		internal AssemblyDefinition ()
		{
			/*Telerik Authorship*/
			this.targetFrameworkAttributeValue = null;
		}

#if !READ_ONLY
		public static AssemblyDefinition CreateAssembly (AssemblyNameDefinition assemblyName, string moduleName, ModuleKind kind)
		{
			return CreateAssembly (assemblyName, moduleName, new ModuleParameters { Kind = kind });
		}

		public static AssemblyDefinition CreateAssembly (AssemblyNameDefinition assemblyName, string moduleName, ModuleParameters parameters)
		{
			if (assemblyName == null)
				throw new ArgumentNullException ("assemblyName");
			if (moduleName == null)
				throw new ArgumentNullException ("moduleName");
			Mixin.CheckParameters (parameters);
			if (parameters.Kind == ModuleKind.NetModule)
				throw new ArgumentException ("kind");

			var assembly = ModuleDefinition.CreateModule (moduleName, parameters).Assembly;
			assembly.Name = assemblyName;

			return assembly;
		}
#endif

		/*Telerik Authorship*/
		//public static AssemblyDefinition ReadAssembly (string fileName)
		//{
		//	return ReadAssembly (ModuleDefinition.ReadModule (fileName));
		//}

		public static AssemblyDefinition ReadAssembly (string fileName, ReaderParameters parameters)
		{
			return ReadAssembly (ModuleDefinition.ReadModule (fileName, parameters));
		}

		/*Telerik Authorship*/
		//public static AssemblyDefinition ReadAssembly (Stream stream)
		//{
		//	return ReadAssembly (ModuleDefinition.ReadModule (stream));
		//}

		public static AssemblyDefinition ReadAssembly (Stream stream, ReaderParameters parameters)
		{
			return ReadAssembly (ModuleDefinition.ReadModule (stream, parameters));
		}

		static AssemblyDefinition ReadAssembly (ModuleDefinition module)
		{
			var assembly = module.Assembly;
			if (assembly == null)
				throw new ArgumentException ();

			return assembly;
		}

#if !READ_ONLY	
		public void Write (string fileName)
		{
			Write (fileName, new WriterParameters ());
		}

		public void Write (Stream stream)
		{
			Write (stream, new WriterParameters ());
		}

		public void Write (string fileName, WriterParameters parameters)
		{
			main_module.Write (fileName, parameters);
		}

		public void Write (Stream stream, WriterParameters parameters)
		{
			main_module.Write (stream, parameters);
		}
#endif

		public override string ToString ()
		{
			return this.FullName;
		}
	}
}
