using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Telerik.JustDecompiler.Languages
{
	public abstract class AttributeWriter
	{
		protected readonly HashSet<string> attributesNotToShow;

		protected NamespaceImperativeLanguageWriter genericWriter;

		private Dictionary<SecurityAttribute, SecurityDeclaration> securityAttributeToDeclaration;

		private const string ASSEMBLYNOTRESOLVEDERROR = "JustDecompile was unable to locate the assembly where attribute parameters types are defined. Generating parameters values is impossible.";

		protected abstract string ClosingBracket
		{
			get;
		}

		protected abstract string EqualsSign
		{
			get;
		}

		protected abstract string OpeningBracket
		{
			get;
		}

		protected abstract string ParameterAttributeSeparator
		{
			get;
		}

		public AttributeWriter(NamespaceImperativeLanguageWriter writer)
		{
			stackVariable2 = new String[3];
			stackVariable2[0] = "System.ParamArrayAttribute";
			stackVariable2[1] = "System.Runtime.CompilerServices.IteratorStateMachineAttribute";
			stackVariable2[2] = "Microsoft.VisualBasic.CompilerServices.StandardModuleAttribute";
			this.attributesNotToShow = new HashSet<string>(stackVariable2);
			this.securityAttributeToDeclaration = new Dictionary<SecurityAttribute, SecurityDeclaration>();
			base();
			this.genericWriter = writer;
			return;
		}

		public virtual List<ICustomAttribute> CollectSecurityAttributes(IMemberDefinition member)
		{
			V_0 = new List<ICustomAttribute>();
			if (member as ISecurityDeclarationMemberDefinition != null)
			{
				V_1 = (member as ISecurityDeclarationMemberDefinition).get_SecurityDeclarations().GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_2 = V_1.get_Current();
						V_0.AddRange(this.GetSecurityDeclaration(V_2));
					}
				}
				finally
				{
					V_1.Dispose();
				}
			}
			return V_0;
		}

		private int CompareAttributes(ICustomAttribute x, ICustomAttribute y)
		{
			V_0 = x.get_AttributeType().get_Name();
			V_1 = y.get_AttributeType().get_Name();
			if (String.op_Inequality(V_0, V_1))
			{
				return V_0.CompareTo(V_1);
			}
			if (x as CustomAttribute != null && y as CustomAttribute != null)
			{
				return (x as CustomAttribute).CompareToCustomAttribute(y as CustomAttribute, false);
			}
			if (x as SecurityAttribute == null || y as SecurityAttribute == null)
			{
				return 0;
			}
			return this.CompareSecurityAttributes(x as SecurityAttribute, y as SecurityAttribute);
		}

		private int CompareSecurityAttributes(SecurityAttribute first, SecurityAttribute second)
		{
			if ((object)first == (object)second)
			{
				return 0;
			}
			V_0 = this.securityAttributeToDeclaration.get_Item(first);
			V_1 = this.securityAttributeToDeclaration.get_Item(second);
			return first.CompareToSecurityAttribute(second, V_0, V_1);
		}

		private string GetElementTypeName(CustomAttributeArgument argument)
		{
			return this.genericWriter.ToEscapedTypeString((argument.get_Type() as ArrayType).get_ElementType());
		}

		protected virtual CustomAttribute GetInAttribute(ParameterDefinition parameter)
		{
			if (parameter == null || !parameter.get_IsIn())
			{
				return null;
			}
			return this.GetInOrOutAttribute(parameter, true);
		}

		protected CustomAttribute GetInOrOutAttribute(ParameterDefinition parameter, bool isInAttribute)
		{
			V_0 = parameter.get_Method().get_ReturnType().get_Module();
			V_1 = V_0.get_TypeSystem().get_Corlib();
			if (isInAttribute)
			{
				stackVariable8 = "InAttribute";
			}
			else
			{
				stackVariable8 = "OutAttribute";
			}
			V_3 = new TypeReference("System.Runtime.InteropServices", stackVariable8, V_0, V_1);
			if (V_3 == null)
			{
				return null;
			}
			return new CustomAttribute(new MethodReference(".ctor", V_0.get_TypeSystem().get_Void(), V_3));
		}

		private ModuleDefinition GetModuleDefinition(IMemberDefinition member)
		{
			if (member as TypeDefinition != null)
			{
				return (member as TypeDefinition).get_Module();
			}
			return member.get_DeclaringType().get_Module();
		}

		protected virtual CustomAttribute GetOutAttribute(ParameterDefinition parameter)
		{
			if (parameter.IsOutParameter())
			{
				return null;
			}
			if (parameter == null || !parameter.get_IsOut())
			{
				return null;
			}
			return this.GetInOrOutAttribute(parameter, false);
		}

		private IEnumerable<ICustomAttribute> GetSecurityDeclaration(SecurityDeclaration securityDeclaration)
		{
			V_0 = new List<ICustomAttribute>();
			if (securityDeclaration.get_HasSecurityAttributes())
			{
				V_1 = securityDeclaration.get_SecurityAttributes().GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_2 = V_1.get_Current();
						V_0.Add(V_2);
						this.securityAttributeToDeclaration.Add(V_2, securityDeclaration);
					}
				}
				finally
				{
					V_1.Dispose();
				}
			}
			return V_0;
		}

		protected virtual List<ICustomAttribute> GetSortedReturnValueAttributes(IMethodSignature member)
		{
			V_0 = new List<ICustomAttribute>();
			if (member != null && member.get_MethodReturnType().get_HasCustomAttributes())
			{
				V_0.AddRange(member.get_MethodReturnType().get_CustomAttributes());
			}
			this.SortAttributes(V_0);
			return V_0;
		}

		private bool IsWinRTActivatableAttribute(CustomAttribute attribute)
		{
			if (String.op_Inequality(attribute.get_AttributeType().get_FullName(), "Windows.Foundation.Metadata.ActivatableAttribute") || attribute.get_ConstructorArguments().get_Count() != 2)
			{
				return false;
			}
			V_1 = attribute.get_ConstructorArguments().get_Item(0);
			V_0 = V_1.get_Value() as TypeDefinition;
			if (V_0 == null)
			{
				return false;
			}
			return V_0.get_IsWindowsRuntime();
		}

		private void SortAttributes(List<ICustomAttribute> attributes)
		{
			attributes.Sort(new Comparison<ICustomAttribute>(this.u003cSortAttributesu003eb__8_0));
			return;
		}

		private void WriteAssemblyAttribute(CustomAttribute attribute)
		{
			this.WriteGlobalAttribute(attribute, this.genericWriter.get_KeyWordWriter().get_Assembly());
			return;
		}

		public virtual void WriteAssemblyAttributes(AssemblyDefinition assembly, ICollection<string> attributesToIgnore = null)
		{
			V_0 = new List<ICustomAttribute>();
			this.securityAttributeToDeclaration = new Dictionary<SecurityAttribute, SecurityDeclaration>();
			V_0.Add(AttributesUtilities.GetAssemblyVersionAttribute(assembly));
			V_2 = assembly.get_CustomAttributes().GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					V_3.Resolve();
					V_0.Add(V_3);
				}
			}
			finally
			{
				V_2.Dispose();
			}
			if (assembly.get_HasSecurityDeclarations())
			{
				V_4 = assembly.get_SecurityDeclarations().GetEnumerator();
				try
				{
					while (V_4.MoveNext())
					{
						V_5 = V_4.get_Current();
						V_0.AddRange(this.GetSecurityDeclaration(V_5));
					}
				}
				finally
				{
					V_4.Dispose();
				}
			}
			if (assembly.get_MainModule().get_HasExportedTypes())
			{
				V_6 = assembly.get_MainModule().get_ExportedTypes().GetEnumerator();
				try
				{
					while (V_6.MoveNext())
					{
						V_7 = V_6.get_Current();
						if (V_7.get_Scope() as ModuleReference != null)
						{
							continue;
						}
						V_0.Add(AttributesUtilities.GetExportedTypeAttribute(V_7, assembly.get_MainModule()));
					}
				}
				finally
				{
					V_6.Dispose();
				}
			}
			V_0.Sort(new Comparison<ICustomAttribute>(this.u003cWriteAssemblyAttributesu003eb__18_0));
			V_8 = V_0.GetEnumerator();
			try
			{
				while (V_8.MoveNext())
				{
					V_9 = V_8.get_Current();
					if (attributesToIgnore != null && attributesToIgnore.Contains(V_9.get_AttributeType().get_FullName()))
					{
						continue;
					}
					if (V_9 as CustomAttribute == null)
					{
						if (V_9 as SecurityAttribute == null)
						{
							continue;
						}
						dummyVar0 = this.WriteSecurityAttribute(assembly.get_MainModule(), true, V_9 as SecurityAttribute, this.securityAttributeToDeclaration.get_Item(V_9 as SecurityAttribute), out V_10, false, false);
					}
					else
					{
						this.WriteAssemblyAttribute(V_9 as CustomAttribute);
					}
				}
			}
			finally
			{
				((IDisposable)V_8).Dispose();
			}
			return;
		}

		protected void WriteAttribute(CustomAttribute attribute, bool skipNewLine = false, bool isReturnValueAtrribute = false)
		{
			if (this.attributesNotToShow.Contains(attribute.get_AttributeType().get_FullName()))
			{
				return;
			}
			V_0 = false;
			attribute.Resolve();
			this.genericWriter.WriteToken(this.get_OpeningBracket());
			if (isReturnValueAtrribute)
			{
				this.WriteReturnValueAttributeKeyword();
			}
			V_0 = this.WriteAttributeSignature(attribute, V_0);
			this.genericWriter.WriteToken(this.get_ClosingBracket());
			if (V_0)
			{
				this.genericWriter.Write("    ");
				V_1 = this.genericWriter.get_Language().CommentLines("JustDecompile was unable to locate the assembly where attribute parameters types are defined. Generating parameters values is impossible.");
				this.genericWriter.Write(V_1.Remove(V_1.get_Length() - 2));
			}
			if (!skipNewLine)
			{
				this.genericWriter.WriteLine();
			}
			return;
		}

		private void WriteAttributeArgumentArray(CustomAttributeArgument argument)
		{
			V_0 = argument.get_Value() as CustomAttributeArgument[];
			this.genericWriter.WriteKeyword(this.genericWriter.get_KeyWordWriter().get_New());
			this.genericWriter.WriteSpace();
			V_1 = String.Format("{0}{1}{2}", this.GetElementTypeName(argument), this.genericWriter.get_IndexLeftBracket(), this.genericWriter.get_IndexRightBracket());
			this.genericWriter.WriteNamespaceIfTypeInCollision(argument.get_Type());
			this.genericWriter.WriteReference(V_1, argument.get_Type());
			this.genericWriter.WriteSpace();
			this.genericWriter.WriteToken("{");
			this.genericWriter.WriteSpace();
			V_2 = 0;
			while (V_2 < (int)V_0.Length)
			{
				this.WriteAttributeArgumentValue(V_0[V_2]);
				if (V_2 + 1 < (int)V_0.Length)
				{
					this.genericWriter.WriteToken(",");
					this.genericWriter.WriteSpace();
				}
				V_2 = V_2 + 1;
			}
			this.genericWriter.WriteSpace();
			this.genericWriter.WriteToken("}");
			return;
		}

		private void WriteAttributeArgumentValue(CustomAttributeArgument argument)
		{
			if (argument.get_Value() as CustomAttributeArgument != null)
			{
				this.WriteAttributeArgumentValue((CustomAttributeArgument)argument.get_Value());
				return;
			}
			if (argument.get_Value() as CustomAttributeArgument[] != null)
			{
				this.WriteAttributeArgumentArray(argument);
				return;
			}
			if (argument.get_Type().get_IsDefinition())
			{
				stackVariable11 = argument.get_Type() as TypeDefinition;
			}
			else
			{
				stackVariable11 = argument.get_Type().Resolve();
			}
			V_0 = stackVariable11;
			if (V_0 == null || !V_0.get_IsEnum())
			{
				if (String.op_Inequality(argument.get_Type().get_Name(), "Type") || String.op_Inequality(argument.get_Type().get_Namespace(), "System"))
				{
					this.genericWriter.WriteLiteralInLanguageSyntax(argument.get_Value());
					return;
				}
				this.genericWriter.WriteKeyword(this.genericWriter.get_KeyWordWriter().get_TypeOf());
				this.genericWriter.WriteToken("(");
				this.genericWriter.WriteGenericReference(argument.get_Value() as TypeReference);
				this.genericWriter.WriteToken(")");
				return;
			}
			V_1 = EnumValueToFieldCombinationMatcher.GetEnumFieldDefinitionByValue(V_0.get_Fields(), argument.get_Value(), V_0.get_CustomAttributes());
			if (V_1.get_Count() == 0)
			{
				this.genericWriter.WriteLiteralInLanguageSyntax(argument.get_Value());
				return;
			}
			V_2 = 0;
			while (V_2 < V_1.get_Count())
			{
				this.genericWriter.WriteReferenceAndNamespaceIfInCollision(V_1.get_Item(V_2).get_DeclaringType());
				this.genericWriter.WriteToken(".");
				this.genericWriter.WriteEnumValueField(V_1.get_Item(V_2));
				if (V_2 + 1 < V_1.get_Count())
				{
					this.genericWriter.WriteSpace();
					this.genericWriter.WriteBitwiseOr();
					this.genericWriter.WriteSpace();
				}
				V_2 = V_2 + 1;
			}
			return;
		}

		private bool WriteAttributeNamedArgs(TypeDefinition attributeType, Collection<CustomAttributeNamedArgument> namedArguments, bool fields, bool wroteArgument)
		{
			V_0 = 0;
			while (V_0 < namedArguments.get_Count())
			{
				if (V_0 == 0 & wroteArgument)
				{
					this.genericWriter.WriteToken(",");
					this.genericWriter.WriteSpace();
				}
				if (attributeType != null)
				{
					if (fields)
					{
						stackVariable13 = attributeType.get_Fields();
					}
					else
					{
						stackVariable13 = attributeType.get_Properties();
					}
					V_2 = null;
					V_3 = stackVariable13;
					V_4 = attributeType;
					do
					{
						V_2 = Utilities.FindMemberArgumentRefersTo(V_3, namedArguments.get_Item(V_0));
						if (V_4.get_BaseType() == null)
						{
							break;
						}
						V_4 = V_4.get_BaseType().Resolve();
						if (V_4 == null)
						{
							break;
						}
						if (fields)
						{
							stackVariable67 = V_4.get_Fields();
						}
						else
						{
							stackVariable67 = V_4.get_Properties();
						}
						V_3 = stackVariable67;
					}
					while (V_2 == null);
					if (V_2 == null)
					{
						stackVariable25 = this.genericWriter;
						V_1 = namedArguments.get_Item(V_0);
						stackVariable25.Write(V_1.get_Name());
					}
					else
					{
						this.genericWriter.WriteReference(V_2.get_Name(), V_2);
					}
				}
				else
				{
					stackVariable74 = this.genericWriter;
					V_1 = namedArguments.get_Item(V_0);
					stackVariable74.Write(V_1.get_Name());
				}
				this.genericWriter.WriteToken(this.get_EqualsSign());
				V_1 = namedArguments.get_Item(V_0);
				this.WriteAttributeArgumentValue(V_1.get_Argument());
				if (V_0 + 1 < namedArguments.get_Count())
				{
					this.genericWriter.WriteToken(",");
					this.genericWriter.WriteSpace();
				}
				wroteArgument = true;
				V_0 = V_0 + 1;
			}
			return wroteArgument;
		}

		private bool WriteAttributeSignature(CustomAttribute attribute, bool resolvingProblem)
		{
			if (attribute.get_AttributeType().get_Name().EndsWith("Attribute"))
			{
				stackVariable13 = attribute.get_AttributeType().get_Name().Remove(attribute.get_AttributeType().get_Name().LastIndexOf("Attribute"));
			}
			else
			{
				stackVariable13 = attribute.get_AttributeType().get_Name();
			}
			V_0 = stackVariable13;
			if (this.genericWriter.get_Language().IsGlobalKeyword(V_0))
			{
				V_0 = attribute.get_AttributeType().get_Name();
			}
			this.genericWriter.WriteNamespaceIfTypeInCollision(attribute.get_AttributeType());
			this.genericWriter.WriteReference(V_0, attribute.get_AttributeType());
			if (attribute.get_HasConstructorArguments() || attribute.get_HasFields() || attribute.get_HasProperties())
			{
				this.genericWriter.WriteToken("(");
				V_1 = false;
				V_2 = 0;
				while (V_2 < attribute.get_ConstructorArguments().get_Count())
				{
					V_1 = true;
					this.WriteAttributeArgumentValue(attribute.get_ConstructorArguments().get_Item(V_2));
					if (V_2 + 1 < attribute.get_ConstructorArguments().get_Count())
					{
						this.genericWriter.Write(",");
						this.genericWriter.WriteSpace();
					}
					V_2 = V_2 + 1;
				}
				if (attribute.get_HasProperties())
				{
					V_3 = attribute.get_AttributeType().Resolve();
					V_1 = this.WriteAttributeNamedArgs(V_3, attribute.get_Properties(), false, V_1);
				}
				if (attribute.get_HasFields())
				{
					V_4 = attribute.get_AttributeType().Resolve();
					dummyVar0 = this.WriteAttributeNamedArgs(V_4, attribute.get_Fields(), true, V_1);
				}
				this.genericWriter.WriteToken(")");
			}
			else
			{
				if (!attribute.get_IsResolved() && (int)attribute.GetBlob().Length > 4)
				{
					this.genericWriter.WriteToken("(");
					this.genericWriter.Write(",");
					this.genericWriter.WriteToken(")");
					resolvingProblem = true;
				}
			}
			return resolvingProblem;
		}

		protected void WriteAttributesInternal(IMemberDefinition member, List<ICustomAttribute> attributes, bool skipTheNewLine, bool areReturnValueAttributes)
		{
			V_0 = attributes.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (V_1 as CustomAttribute == null)
					{
						if (V_1 as SecurityAttribute != null)
						{
							dummyVar0 = this.WriteSecurityAttribute(this.GetModuleDefinition(member), false, V_1 as SecurityAttribute, this.securityAttributeToDeclaration.get_Item(V_1 as SecurityAttribute), out V_2, skipTheNewLine, areReturnValueAttributes);
						}
					}
					else
					{
						this.WriteAttribute(V_1 as CustomAttribute, skipTheNewLine, areReturnValueAttributes);
					}
					if (!skipTheNewLine & areReturnValueAttributes)
					{
						continue;
					}
					this.genericWriter.WriteSpace();
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		private IEnumerable<ICustomAttribute> WriteFieldDefinitionFieldsAsAttributes(FieldDefinition member)
		{
			V_0 = new List<ICustomAttribute>();
			if (member.get_IsNotSerialized())
			{
				V_0.Add(AttributesUtilities.GetFieldNotSerializedAttribute(member));
			}
			if (member.get_DeclaringType().get_IsExplicitLayout())
			{
				V_0.Add(AttributesUtilities.GetFieldFieldOffsetAttribute(member));
			}
			return V_0;
		}

		private void WriteGlobalAttribute(CustomAttribute attribute, string keyword)
		{
			if (this.attributesNotToShow.Contains(attribute.get_AttributeType().get_FullName()))
			{
				return;
			}
			V_0 = false;
			attribute.Resolve();
			this.genericWriter.WriteToken(this.get_OpeningBracket());
			this.genericWriter.WriteKeyword(keyword);
			this.genericWriter.Write(":");
			this.genericWriter.WriteSpace();
			V_0 = this.WriteAttributeSignature(attribute, V_0);
			this.genericWriter.WriteToken(this.get_ClosingBracket());
			if (V_0)
			{
				this.genericWriter.Write("    ");
				V_1 = this.genericWriter.get_Language().CommentLines("JustDecompile was unable to locate the assembly where attribute parameters types are defined. Generating parameters values is impossible.");
				this.genericWriter.Write(V_1.Remove(V_1.get_Length() - 2));
			}
			this.genericWriter.WriteLine();
			return;
		}

		public virtual void WriteMemberAttributesAndNewLine(IMemberDefinition member, IEnumerable<string> ignored = null, bool isWinRTImplementation = false)
		{
			if (ignored != null)
			{
				V_0 = ignored.GetEnumerator();
				try
				{
					while (V_0.MoveNext())
					{
						V_1 = V_0.get_Current();
						dummyVar0 = this.attributesNotToShow.Add(V_1);
					}
				}
				finally
				{
					if (V_0 != null)
					{
						V_0.Dispose();
					}
				}
			}
			this.WriteMemberAttributesInternal(member, isWinRTImplementation);
			if (ignored != null)
			{
				V_0 = ignored.GetEnumerator();
				try
				{
					while (V_0.MoveNext())
					{
						V_2 = V_0.get_Current();
						dummyVar1 = this.attributesNotToShow.Remove(V_2);
					}
				}
				finally
				{
					if (V_0 != null)
					{
						V_0.Dispose();
					}
				}
			}
			return;
		}

		protected virtual void WriteMemberAttributesInternal(IMemberDefinition member, bool isWinRTImplementation)
		{
			this.securityAttributeToDeclaration = new Dictionary<SecurityAttribute, SecurityDeclaration>();
			V_0 = this.CollectSecurityAttributes(member);
			V_1 = member.get_CustomAttributes().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_2.Resolve();
					if (isWinRTImplementation && this.IsWinRTActivatableAttribute(V_2))
					{
						continue;
					}
					V_0.Add(V_2);
				}
			}
			finally
			{
				V_1.Dispose();
			}
			V_0.AddRange(this.WritePropertiesAsAttributes(member));
			this.SortAttributes(V_0);
			this.WriteAttributesInternal(member, V_0, false, false);
			return;
		}

		public abstract void WriteMemberReturnValueAttributes(IMemberDefinition member);

		private IEnumerable<ICustomAttribute> WriteMethodFieldsAsAttributes(MethodDefinition method)
		{
			V_0 = new List<ICustomAttribute>();
			if (method.get_HasPInvokeInfo())
			{
				V_0.Add(AttributesUtilities.GetMethodDllImportAttribute(method));
			}
			if (method.get_HasImplAttributes() && AttributesUtilities.ShouldWriteMethodImplAttribute(method))
			{
				V_0.Add(AttributesUtilities.GetMethodImplAttribute(method));
			}
			return V_0;
		}

		private void WriteModuleAttribute(CustomAttribute attr)
		{
			this.WriteGlobalAttribute(attr, this.genericWriter.get_KeyWordWriter().get_Module());
			return;
		}

		public void WriteModuleAttributes(ModuleDefinition module, ICollection<string> attributesToIgnore = null)
		{
			V_0 = new List<CustomAttribute>();
			V_1 = module.get_CustomAttributes().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_2.Resolve();
					V_0.Add(V_2);
				}
			}
			finally
			{
				V_1.Dispose();
			}
			V_0.Sort(new Comparison<CustomAttribute>(this.u003cWriteModuleAttributesu003eb__45_0));
			V_3 = V_0.GetEnumerator();
			try
			{
				while (V_3.MoveNext())
				{
					V_4 = V_3.get_Current();
					if (attributesToIgnore != null && attributesToIgnore.Contains(V_4.get_AttributeType().get_FullName()))
					{
						continue;
					}
					this.WriteModuleAttribute(V_4);
				}
			}
			finally
			{
				((IDisposable)V_3).Dispose();
			}
			return;
		}

		public int WriteParameterAttributes(ParameterDefinition parameter, bool isWinRTMethodImplementation)
		{
			if (isWinRTMethodImplementation)
			{
				stackVariable1 = null;
			}
			else
			{
				stackVariable1 = this.GetInAttribute(parameter);
			}
			V_0 = stackVariable1;
			if (isWinRTMethodImplementation)
			{
				stackVariable3 = null;
			}
			else
			{
				stackVariable3 = this.GetOutAttribute(parameter);
			}
			V_1 = stackVariable3;
			V_2 = new List<CustomAttribute>();
			if (V_0 != null)
			{
				V_2.Add(V_0);
			}
			if (V_1 != null)
			{
				V_2.Add(V_1);
			}
			V_2.AddRange(parameter.get_CustomAttributes());
			V_3 = 0;
			V_4 = V_2.GetEnumerator();
			try
			{
				while (V_4.MoveNext())
				{
					V_5 = V_4.get_Current();
					if (this.attributesNotToShow.Contains(V_5.get_AttributeType().get_FullName()))
					{
						continue;
					}
					if (V_3 != 0)
					{
						this.genericWriter.Write(this.get_ParameterAttributeSeparator());
					}
					this.WriteAttribute(V_5, true, false);
					V_3 = V_3 + 1;
				}
			}
			finally
			{
				((IDisposable)V_4).Dispose();
			}
			return V_3;
		}

		protected virtual IEnumerable<ICustomAttribute> WritePropertiesAsAttributes(IMemberDefinition member)
		{
			V_0 = new List<ICustomAttribute>();
			if (member as FieldDefinition == null)
			{
				if (member as TypeDefinition == null)
				{
					if (member as MethodDefinition != null)
					{
						V_0.AddRange(this.WriteMethodFieldsAsAttributes(member as MethodDefinition));
					}
				}
				else
				{
					V_0.AddRange(this.WriteTypeDefinitionFieldsAsAttributes(member as TypeDefinition));
				}
			}
			else
			{
				V_0.AddRange(this.WriteFieldDefinitionFieldsAsAttributes(member as FieldDefinition));
			}
			return V_0;
		}

		protected abstract void WriteReturnValueAttributeKeyword();

		private bool WriteSecurityAttribute(ModuleDefinition module, bool isAssemblyDeclaration, SecurityAttribute attribute, SecurityDeclaration securityDeclaration, out bool wroteArgument, bool skipTheNewLine = false, bool isReturnValueAttribute = false)
		{
			this.genericWriter.WriteToken(this.get_OpeningBracket());
			if (!isAssemblyDeclaration)
			{
				if (isReturnValueAttribute)
				{
					this.WriteReturnValueAttributeKeyword();
				}
			}
			else
			{
				this.genericWriter.WriteKeyword(this.genericWriter.get_KeyWordWriter().get_Assembly());
				this.genericWriter.Write(":");
				this.genericWriter.WriteSpace();
			}
			if (attribute.get_AttributeType().get_Name().EndsWith("Attribute"))
			{
				stackVariable19 = attribute.get_AttributeType().get_Name().Remove(attribute.get_AttributeType().get_Name().LastIndexOf("Attribute"));
			}
			else
			{
				stackVariable19 = attribute.get_AttributeType().get_Name();
			}
			V_0 = stackVariable19;
			this.genericWriter.WriteNamespaceIfTypeInCollision(attribute.get_AttributeType());
			this.genericWriter.WriteReference(V_0, attribute.get_AttributeType());
			this.genericWriter.WriteToken("(");
			V_1 = securityDeclaration.GetSecurityActionTypeReference(module);
			if (V_1.get_IsDefinition())
			{
				stackVariable38 = V_1 as TypeDefinition;
			}
			else
			{
				stackVariable38 = V_1.Resolve();
			}
			V_2 = stackVariable38;
			if (V_2 != null && V_2.get_IsEnum())
			{
				V_3 = EnumValueToFieldCombinationMatcher.GetEnumFieldDefinitionByValue(V_2.get_Fields(), (Int32)securityDeclaration.get_Action(), V_2.get_CustomAttributes());
				if (V_3.get_Count() == 0)
				{
					this.WriteSecurityAttributeAction(securityDeclaration.get_Action());
				}
				else
				{
					V_4 = 0;
					while (V_4 < V_3.get_Count())
					{
						this.genericWriter.WriteReferenceAndNamespaceIfInCollision(V_3.get_Item(V_4).get_DeclaringType());
						this.genericWriter.WriteToken(".");
						this.genericWriter.WriteEnumValueField(V_3.get_Item(V_4));
						if (V_4 + 1 < V_3.get_Count())
						{
							this.genericWriter.WriteSpace();
							this.genericWriter.WriteBitwiseOr();
							this.genericWriter.WriteSpace();
						}
						V_4 = V_4 + 1;
					}
				}
			}
			wroteArgument = true;
			if (attribute.get_HasFields() || attribute.get_HasProperties())
			{
				V_5 = attribute.get_AttributeType().Resolve();
				if (attribute.get_HasProperties())
				{
					wroteArgument = this.WriteAttributeNamedArgs(V_5, attribute.get_Properties(), false, wroteArgument);
				}
				if (attribute.get_HasFields())
				{
					dummyVar0 = this.WriteAttributeNamedArgs(V_5, attribute.get_Fields(), true, wroteArgument);
				}
			}
			this.genericWriter.WriteToken(")");
			this.genericWriter.WriteToken(this.get_ClosingBracket());
			if (!skipTheNewLine)
			{
				this.genericWriter.WriteLine();
			}
			return wroteArgument;
		}

		private void WriteSecurityAttributeAction(SecurityAction action)
		{
			this.genericWriter.WriteSecurityDeclarationNamespaceIfNeeded();
			this.genericWriter.Write("SecurityAction");
			this.genericWriter.WriteToken(".");
			switch (action - 1)
			{
				case 0:
				{
					this.genericWriter.Write("");
					return;
				}
				case 1:
				case 12:
				{
					this.genericWriter.Write("Demand");
					return;
				}
				case 2:
				case 10:
				{
					this.genericWriter.Write("Assert");
					return;
				}
				case 3:
				case 11:
				{
					this.genericWriter.Write("Deny");
					return;
				}
				case 4:
				{
					this.genericWriter.Write("PermitOnly");
					return;
				}
				case 5:
				case 13:
				{
					this.genericWriter.Write("LinkDemand");
					return;
				}
				case 6:
				case 14:
				{
					this.genericWriter.Write("InheritanceDemand");
					return;
				}
				case 7:
				{
					this.genericWriter.Write("RequestMinimum");
					return;
				}
				case 8:
				{
					this.genericWriter.Write("RequestOptional");
					return;
				}
				case 9:
				{
					this.genericWriter.Write("RequestRefuse");
					return;
				}
				default:
				{
					return;
				}
			}
		}

		private IEnumerable<ICustomAttribute> WriteTypeDefinitionFieldsAsAttributes(TypeDefinition member)
		{
			V_0 = new List<ICustomAttribute>();
			if (member.get_IsSerializable())
			{
				V_0.Add(AttributesUtilities.GetTypeSerializableAttribute(member));
			}
			if (member.get_IsExplicitLayout())
			{
				V_0.Add(AttributesUtilities.GetTypeExplicitLayoutAttribute(member));
			}
			return V_0;
		}
	}
}