using System;
using Telerik.JustDecompiler.Decompiler.Caching;
using Telerik.JustDecompiler.Decompiler.WriterContextServices;
using Telerik.JustDecompiler.Languages.CSharp;
using Mono.Cecil;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Decompiler;
/* AGPL */
using JustDecompiler.Shared;
/* End AGPL */

namespace Telerik.JustDecompiler.Languages.TestCaseWriters
{
    class IntermediateDecompilationCSharpLanguageWriter : TestCaseCSharpWriter
    {
		private readonly bool renameInvalidMembers = true;
		
		/* AGPL */
		public IntermediateDecompilationCSharpLanguageWriter(IFormatter formatter, IMemberDefinition member)
		/* End AGPL */
			: base(LanguageFactory.GetLanguage(CSharpVersion.None), formatter, new WriterSettings(writeExceptionsAsComments: true)) 
		{
			SimpleWriterContextService swcs = new SimpleWriterContextService(new DefaultDecompilationCacheService(), renameInvalidMembers);
			this.writerContextService = swcs;
			/* AGPL */
			this.currentWritingInfo = new WritingInfo(member);
			/* End AGPL */
		}

		public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
            Write(node.Variable.Name);
        }

        public override void VisitVariableDeclarationExpression(VariableDeclarationExpression node)
        {
			/* AGPL */
			WriteTypeAndName(node.Variable.VariableType, node.Variable.Name, node.Variable, TypeReferenceType.VariableType);
			/* End AGPL */
		}

		protected override string GetMethodName(MethodReference method)
		{
			return method.Name;
		}

		protected override string GetFieldName(FieldReference field)
		{
			return field.Name;
		}

		protected override string GetArgumentName(ParameterReference parameter)
		{
			return parameter.Name;
		}

		protected override ModuleSpecificContext ModuleContext
		{
			get
			{
				try
				{
					return base.ModuleContext;
				}
				catch (NullReferenceException)
				{
					return new ModuleSpecificContext();
				}
			}
		}
    }
}
