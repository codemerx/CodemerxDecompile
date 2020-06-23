using Mono.Cecil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.WriterContextServices;
using Telerik.JustDecompiler.External.Interfaces;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Languages.CSharp;

namespace Telerik.JustDecompiler.Languages.TestCaseWriters
{
	public class TestCaseCSharpWriter : CSharpWriter, ILanguageTestCaseWriter, ILanguageWriter, IExceptionThrownNotifier
	{
		public TestCaseCSharpWriter(ILanguage language, IFormatter formatter, IWriterSettings settings) : base(language, formatter, TestCaseExceptionFormatter.Instance, settings)
		{
		}

		protected override string GetEventName(EventReference @event)
		{
			if (this.writerContext == null || this.writerContext.ModuleContext.Module == null)
			{
				return @event.Name;
			}
			return base.GetEventName(@event);
		}

		protected override string GetFieldName(FieldReference field)
		{
			if (this.writerContext == null || this.writerContext.ModuleContext.Module == null)
			{
				return field.Name;
			}
			return base.GetFieldName(field);
		}

		protected override string GetMethodName(MethodReference method)
		{
			if (method == null)
			{
				return null;
			}
			if (this.writerContext == null || this.writerContext.ModuleContext.Module == null)
			{
				return method.Name;
			}
			return base.GetMethodName(method);
		}

		protected override string GetPropertyName(PropertyReference property)
		{
			if (this.writerContext == null || this.writerContext.ModuleContext.Module == null)
			{
				return property.Name;
			}
			return base.GetPropertyName(property);
		}

		protected override string GetTypeName(TypeReference type)
		{
			if (this.writerContext != null && this.writerContext.ModuleContext.Module != null)
			{
				return base.GetTypeName(type);
			}
			return GenericHelper.GetNonGenericName(type.Name);
		}

		protected override bool IsTypeNameInCollision(string typeName)
		{
			if (this.writerContext == null)
			{
				return false;
			}
			return base.IsTypeNameInCollision(typeName);
		}

		public void ResetContext()
		{
			this.writerContext = null;
			this.membersStack.Clear();
		}

		public void SetContext(DecompilationContext context)
		{
			this.writerContext = new WriterContext(context.AssemblyContext, context.ModuleContext, context.TypeContext, new Dictionary<string, MethodSpecificContext>(), new Dictionary<string, Statement>());
			this.writerContext.MethodContexts.Add(Utilities.GetMemberUniqueName(context.MethodContext.Method), context.MethodContext);
			this.membersStack.Push(context.MethodContext.Method);
		}

		public void SetContext(IMemberDefinition member)
		{
			this.writerContext = this.writerContextService.GetWriterContext(member, base.Language);
		}

		public override void Visit(ICodeNode node)
		{
			base.WriteCodeNodeLabel(node);
			base.DoVisit(node);
		}

		public override void VisitMemberHandleExpression(MemberHandleExpression node)
		{
			this.WriteKeyword("__ldtoken__");
			this.WriteToken("(");
			if (node.MemberReference is MethodReference)
			{
				MethodReference memberReference = node.MemberReference as MethodReference;
				this.WriteReference(this.GetTypeName(memberReference.DeclaringType), memberReference.DeclaringType);
				this.WriteToken(".");
				this.WriteReference(this.GetMethodName(memberReference), memberReference);
			}
			else if (node.MemberReference is FieldReference)
			{
				FieldReference fieldReference = node.MemberReference as FieldReference;
				this.WriteReference(this.GetTypeName(fieldReference.DeclaringType), fieldReference.DeclaringType);
				this.WriteToken(".");
				this.WriteReference(this.GetFieldName(fieldReference), fieldReference);
			}
			else if (!(node.MemberReference is TypeReference))
			{
				this.Write(String.Format("Invalid member reference: {0} {1}.", node.MemberReference.GetType(), node.MemberReference.FullName));
			}
			else
			{
				TypeReference typeReference = node.MemberReference as TypeReference;
				this.WriteReference(this.GetTypeName(typeReference), typeReference);
			}
			this.WriteToken(")");
		}

		public new void Write(Statement statement)
		{
			base.Write(statement);
		}

		public new void Write(Expression expression)
		{
			base.Write(expression);
		}

		protected override void WriteInternal(IMemberDefinition member)
		{
			if (this.isStopped)
			{
				return;
			}
			this.membersStack.Push(member);
			if (!(member is TypeDefinition) || member == base.CurrentType)
			{
				this.formatter.PreserveIndent(member);
			}
			if (base.Settings.WriteDocumentation)
			{
				base.WriteDocumentation(member);
			}
			if (!(member is TypeDefinition))
			{
				this.WriteAttributes(member, null);
				if (member is MethodDefinition)
				{
					this.Write((MethodDefinition)member);
				}
				else if (member is PropertyDefinition)
				{
					this.Write((PropertyDefinition)member);
				}
				else if (member is EventDefinition)
				{
					this.Write((EventDefinition)member);
				}
				else if (member is FieldDefinition)
				{
					this.Write((FieldDefinition)member);
				}
			}
			else
			{
				this.WriteTypeInANewWriterIfNeeded((TypeDefinition)member);
			}
			if (!(member is TypeDefinition) || member == base.CurrentType)
			{
				this.formatter.RemovePreservedIndent(member);
			}
			this.membersStack.Pop();
		}

		public new void WriteMethodDeclaration(MethodDefinition method, bool writeDocumentation = false)
		{
			this.currentWritingInfo = new WritingInfo(method);
			base.WriteMethodDeclaration(method, writeDocumentation);
		}
	}
}