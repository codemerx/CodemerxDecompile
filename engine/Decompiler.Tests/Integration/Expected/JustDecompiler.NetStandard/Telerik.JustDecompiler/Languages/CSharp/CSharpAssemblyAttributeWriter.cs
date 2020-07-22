using System;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Languages.CSharp
{
	public class CSharpAssemblyAttributeWriter : BaseAssemblyAttributeWriter
	{
		private readonly CSharpAssemblyAttributeWriter.CSharpAssemblyAttributeInternalWriter writer;

		public CSharpAssemblyAttributeWriter(ILanguage language, IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings)
		{
			base(language, exceptionFormatter, settings);
			this.writer = new CSharpAssemblyAttributeWriter.CSharpAssemblyAttributeInternalWriter(language, formatter, exceptionFormatter, settings);
			this.writer.add_ExceptionThrown(new EventHandler<Exception>(this.OnExceptionThrown));
			return;
		}

		protected override Telerik.JustDecompiler.Languages.AttributeWriter CreateAttributeWriter()
		{
			return new CSharpAttributeWriter(this.writer);
		}

		protected override NamespaceImperativeLanguageWriter GetLanguageWriter()
		{
			return this.writer;
		}

		protected override void SetAssemblyContext(AssemblySpecificContext assemblyContext)
		{
			this.writer.set_InternalAssemblyContext(assemblyContext);
			return;
		}

		protected override void SetModuleContext(ModuleSpecificContext moduleContext)
		{
			this.writer.set_InternalModuleContext(moduleContext);
			return;
		}

		private class CSharpAssemblyAttributeInternalWriter : CSharpWriter
		{
			protected override AssemblySpecificContext AssemblyContext
			{
				get
				{
					return this.get_InternalAssemblyContext();
				}
			}

			public AssemblySpecificContext InternalAssemblyContext
			{
				get;
				set;
			}

			public ModuleSpecificContext InternalModuleContext
			{
				get;
				set;
			}

			protected override ModuleSpecificContext ModuleContext
			{
				get
				{
					return this.get_InternalModuleContext();
				}
			}

			protected override TypeSpecificContext TypeContext
			{
				get
				{
					return null;
				}
			}

			public CSharpAssemblyAttributeInternalWriter(ILanguage language, IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings)
			{
				base(language, formatter, exceptionFormatter, settings);
				return;
			}

			protected override bool IsTypeNameInCollision(string typeName)
			{
				return Utilities.IsTypeNameInCollisionOnAssemblyLevel(typeName, this.get_AssemblyContext(), this.get_ModuleContext());
			}
		}
	}
}