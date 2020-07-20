using Mono.Cecil;
using System;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.External.Interfaces;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Languages.CSharp;

namespace Telerik.JustDecompiler.Languages.TestCaseWriters
{
	public class TestCaseCSharpWriter : CSharpWriter, ILanguageTestCaseWriter, ILanguageWriter, IExceptionThrownNotifier
	{
		public TestCaseCSharpWriter(ILanguage language, IFormatter formatter, IWriterSettings settings)
		{
			base(language, formatter, TestCaseExceptionFormatter.get_Instance(), settings);
			return;
		}

		protected override string GetEventName(EventReference event)
		{
			if (this.writerContext == null || this.writerContext.get_ModuleContext().get_Module() == null)
			{
				return event.get_Name();
			}
			return this.GetEventName(event);
		}

		protected override string GetFieldName(FieldReference field)
		{
			if (this.writerContext == null || this.writerContext.get_ModuleContext().get_Module() == null)
			{
				return field.get_Name();
			}
			return this.GetFieldName(field);
		}

		protected override string GetMethodName(MethodReference method)
		{
			if (method == null)
			{
				return null;
			}
			if (this.writerContext == null || this.writerContext.get_ModuleContext().get_Module() == null)
			{
				return method.get_Name();
			}
			return this.GetMethodName(method);
		}

		protected override string GetPropertyName(PropertyReference property)
		{
			if (this.writerContext == null || this.writerContext.get_ModuleContext().get_Module() == null)
			{
				return property.get_Name();
			}
			return this.GetPropertyName(property);
		}

		protected override string GetTypeName(TypeReference type)
		{
			if (this.writerContext != null && this.writerContext.get_ModuleContext().get_Module() != null)
			{
				return this.GetTypeName(type);
			}
			return GenericHelper.GetNonGenericName(type.get_Name());
		}

		protected override bool IsTypeNameInCollision(string typeName)
		{
			if (this.writerContext == null)
			{
				return false;
			}
			return this.IsTypeNameInCollision(typeName);
		}

		public void ResetContext()
		{
			this.writerContext = null;
			this.membersStack.Clear();
			return;
		}

		public void SetContext(DecompilationContext context)
		{
			this.writerContext = new WriterContext(context.get_AssemblyContext(), context.get_ModuleContext(), context.get_TypeContext(), new Dictionary<string, MethodSpecificContext>(), new Dictionary<string, Statement>());
			this.writerContext.get_MethodContexts().Add(Utilities.GetMemberUniqueName(context.get_MethodContext().get_Method()), context.get_MethodContext());
			this.membersStack.Push(context.get_MethodContext().get_Method());
			return;
		}

		public void SetContext(IMemberDefinition member)
		{
			this.writerContext = this.writerContextService.GetWriterContext(member, this.get_Language());
			return;
		}

		public override void Visit(ICodeNode node)
		{
			this.WriteCodeNodeLabel(node);
			this.DoVisit(node);
			return;
		}

		public override void VisitMemberHandleExpression(MemberHandleExpression node)
		{
			this.WriteKeyword("__ldtoken__");
			this.WriteToken("(");
			if (node.get_MemberReference() as MethodReference == null)
			{
				if (node.get_MemberReference() as FieldReference == null)
				{
					if (node.get_MemberReference() as TypeReference == null)
					{
						this.Write(String.Format("Invalid member reference: {0} {1}.", node.get_MemberReference().GetType(), node.get_MemberReference().get_FullName()));
					}
					else
					{
						V_2 = node.get_MemberReference() as TypeReference;
						this.WriteReference(this.GetTypeName(V_2), V_2);
					}
				}
				else
				{
					V_1 = node.get_MemberReference() as FieldReference;
					this.WriteReference(this.GetTypeName(V_1.get_DeclaringType()), V_1.get_DeclaringType());
					this.WriteToken(".");
					this.WriteReference(this.GetFieldName(V_1), V_1);
				}
			}
			else
			{
				V_0 = node.get_MemberReference() as MethodReference;
				this.WriteReference(this.GetTypeName(V_0.get_DeclaringType()), V_0.get_DeclaringType());
				this.WriteToken(".");
				this.WriteReference(this.GetMethodName(V_0), V_0);
			}
			this.WriteToken(")");
			return;
		}

		public new void Write(Statement statement)
		{
			this.Write(statement);
			return;
		}

		public new void Write(Expression expression)
		{
			this.Write(expression);
			return;
		}

		protected override void WriteInternal(IMemberDefinition member)
		{
			if (this.isStopped)
			{
				return;
			}
			this.membersStack.Push(member);
			if (member as TypeDefinition == null || member == this.get_CurrentType())
			{
				this.formatter.PreserveIndent(member);
			}
			if (this.get_Settings().get_WriteDocumentation())
			{
				this.WriteDocumentation(member);
			}
			if (member as TypeDefinition == null)
			{
				this.WriteAttributes(member, null);
				if (member as MethodDefinition == null)
				{
					if (member as PropertyDefinition == null)
					{
						if (member as EventDefinition == null)
						{
							if (member as FieldDefinition != null)
							{
								this.Write((FieldDefinition)member);
							}
						}
						else
						{
							this.Write((EventDefinition)member);
						}
					}
					else
					{
						this.Write((PropertyDefinition)member);
					}
				}
				else
				{
					this.Write((MethodDefinition)member);
				}
			}
			else
			{
				this.WriteTypeInANewWriterIfNeeded((TypeDefinition)member);
			}
			if (member as TypeDefinition == null || member == this.get_CurrentType())
			{
				this.formatter.RemovePreservedIndent(member);
			}
			dummyVar0 = this.membersStack.Pop();
			return;
		}

		public new void WriteMethodDeclaration(MethodDefinition method, bool writeDocumentation = false)
		{
			this.currentWritingInfo = new WritingInfo(method);
			this.WriteMethodDeclaration(method, writeDocumentation);
			return;
		}
	}
}