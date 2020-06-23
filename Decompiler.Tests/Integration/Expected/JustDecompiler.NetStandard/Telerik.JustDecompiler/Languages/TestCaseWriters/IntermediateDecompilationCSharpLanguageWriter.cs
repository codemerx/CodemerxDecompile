using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.Caching;
using Telerik.JustDecompiler.Decompiler.WriterContextServices;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Languages.CSharp;

namespace Telerik.JustDecompiler.Languages.TestCaseWriters
{
	internal class IntermediateDecompilationCSharpLanguageWriter : TestCaseCSharpWriter
	{
		private readonly bool renameInvalidMembers = true;

		protected override ModuleSpecificContext ModuleContext
		{
			get
			{
				ModuleSpecificContext moduleContext;
				try
				{
					moduleContext = base.ModuleContext;
				}
				catch (NullReferenceException nullReferenceException)
				{
					moduleContext = new ModuleSpecificContext();
				}
				return moduleContext;
			}
		}

		public IntermediateDecompilationCSharpLanguageWriter(IFormatter formatter) : base(LanguageFactory.GetLanguage(CSharpVersion.None), formatter, new WriterSettings(true, false, false, false, false, false, true, false))
		{
			this.writerContextService = new SimpleWriterContextService(new DefaultDecompilationCacheService(), this.renameInvalidMembers);
		}

		protected override string GetArgumentName(ParameterReference parameter)
		{
			return parameter.Name;
		}

		protected override string GetFieldName(FieldReference field)
		{
			return field.Name;
		}

		protected override string GetMethodName(MethodReference method)
		{
			return method.Name;
		}

		public override void VisitVariableDeclarationExpression(VariableDeclarationExpression node)
		{
			this.WriteTypeAndName(node.Variable.VariableType, node.Variable.Name);
		}

		public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			this.Write(node.Variable.Name);
		}
	}
}