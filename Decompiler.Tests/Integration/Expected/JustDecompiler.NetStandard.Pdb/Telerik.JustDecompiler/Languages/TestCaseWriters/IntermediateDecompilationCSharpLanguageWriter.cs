using Mono.Cecil;
using System;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.WriterContextServices;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Languages.TestCaseWriters
{
	internal class IntermediateDecompilationCSharpLanguageWriter : TestCaseCSharpWriter
	{
		private readonly bool renameInvalidMembers;

		protected override ModuleSpecificContext ModuleContext
		{
			get
			{
				try
				{
					V_0 = this.get_ModuleContext();
				}
				catch (NullReferenceException exception_0)
				{
					dummyVar0 = exception_0;
					V_0 = new ModuleSpecificContext();
				}
				return V_0;
			}
		}

		public IntermediateDecompilationCSharpLanguageWriter(IFormatter formatter)
		{
			this.renameInvalidMembers = true;
			base(LanguageFactory.GetLanguage(0), formatter, new WriterSettings(true, false, false, false, false, false, true, false));
			this.writerContextService = new SimpleWriterContextService(new DefaultDecompilationCacheService(), this.renameInvalidMembers);
			return;
		}

		protected override string GetArgumentName(ParameterReference parameter)
		{
			return parameter.get_Name();
		}

		protected override string GetFieldName(FieldReference field)
		{
			return field.get_Name();
		}

		protected override string GetMethodName(MethodReference method)
		{
			return method.get_Name();
		}

		public override void VisitVariableDeclarationExpression(VariableDeclarationExpression node)
		{
			this.WriteTypeAndName(node.get_Variable().get_VariableType(), node.get_Variable().get_Name());
			return;
		}

		public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			this.Write(node.get_Variable().get_Name());
			return;
		}
	}
}