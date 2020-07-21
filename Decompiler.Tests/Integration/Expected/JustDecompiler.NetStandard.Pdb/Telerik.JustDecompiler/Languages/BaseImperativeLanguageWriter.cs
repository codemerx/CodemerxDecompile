using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Languages
{
	public abstract class BaseImperativeLanguageWriter : BaseLanguageWriter
	{
		private Telerik.JustDecompiler.Languages.AttributeWriter attributeWriter;

		private readonly Stack<bool> shouldOmitSemicolon;

		private readonly Stack<MethodReference> methodReferences;

		private const string DoublePositiveInfinityFieldSignature = "System.Double System.Double::PositiveInfinity";

		private const string DoubleNegativeInfinityFieldSignature = "System.Double System.Double::NegativeInfinity";

		private const string DoubleNanFieldSignature = "System.Double System.Double::NaN";

		private const string DoubleMaxValueFieldSignature = "System.Double System.Double::MaxValue";

		private const string DoubleMinValueFieldSignature = "System.Double System.Double::MinValue";

		private const string DoubleEpsilonFieldSignature = "System.Double System.Double::Epsilon";

		private const string FloatPositiveInfinityFieldSignature = "System.Single System.Single::PositiveInfinity";

		private const string FloatNegativeInfinityFieldSignature = "System.Single System.Single::NegativeInfinity";

		private const string FloatNanFieldSignature = "System.Single System.Single::NaN";

		private const string FloatMaxValueFieldSignature = "System.Single System.Single::MaxValue";

		private const string FloatMinValueFieldSignature = "System.Single System.Single::MinValue";

		private const string FloatEpsilonFieldSignature = "System.Single System.Single::Epsilon";

		protected Telerik.JustDecompiler.Languages.AttributeWriter AttributeWriter
		{
			get
			{
				if (this.attributeWriter == null)
				{
					this.attributeWriter = this.CreateAttributeWriter();
				}
				return this.attributeWriter;
			}
		}

		protected abstract string CharEnd
		{
			get;
		}

		protected abstract string CharStart
		{
			get;
		}

		protected abstract string GenericLeftBracket
		{
			get;
		}

		protected abstract string GenericRightBracket
		{
			get;
		}

		protected abstract string HexValuePrefix
		{
			get;
		}

		public abstract string IndexLeftBracket
		{
			get;
		}

		public abstract string IndexRightBracket
		{
			get;
		}

		public IKeyWordWriter KeyWordWriter
		{
			get;
			private set;
		}

		protected Stack<MethodReference> MethodReferences
		{
			get
			{
				return this.methodReferences;
			}
		}

		protected virtual bool RemoveBaseConstructorInvocation
		{
			get
			{
				return false;
			}
		}

		protected Stack<bool> ShouldOmitSemicolon
		{
			get
			{
				return this.shouldOmitSemicolon;
			}
		}

		protected virtual bool ShouldWriteOutAndRefOnInvocation
		{
			get
			{
				return false;
			}
		}

		protected virtual bool SupportsAutoProperties
		{
			get
			{
				return false;
			}
		}

		protected virtual bool SupportsSpecialNullable
		{
			get
			{
				return false;
			}
		}

		public BaseImperativeLanguageWriter(ILanguage language, IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings)
		{
			this.shouldOmitSemicolon = new Stack<bool>();
			this.methodReferences = new Stack<MethodReference>();
			base(language, formatter, exceptionFormatter, settings);
			this.set_KeyWordWriter(this.CreateKeyWordWriter());
			return;
		}

		private bool CheckIfParameterIsByRef(MethodReference methodReference, int parameterIndex)
		{
			return methodReference.get_Parameters().get_Item(parameterIndex).get_ParameterType().get_IsByReference();
		}

		private ExpressionCollection CopyMethodParametersAsArguments(MethodDefinition method)
		{
			V_0 = new ExpressionCollection();
			if (method.get_HasParameters())
			{
				V_1 = method.get_Parameters().GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_2 = V_1.get_Current();
						V_0.Add(new ArgumentReferenceExpression(V_2, null));
					}
				}
				finally
				{
					V_1.Dispose();
				}
			}
			return V_0;
		}

		protected abstract Telerik.JustDecompiler.Languages.AttributeWriter CreateAttributeWriter();

		protected abstract IKeyWordWriter CreateKeyWordWriter();

		protected void DoVisit(ICodeNode node)
		{
			this.Visit(node);
			return;
		}

		protected void EnterMethodInvocation(MethodReference methodReference)
		{
			this.methodReferences.Push(methodReference);
			return;
		}

		private OffsetSpan ExecuteAndGetOffsetSpan(Action toBeExecuted)
		{
			V_0 = new BaseImperativeLanguageWriter.u003cu003ec__DisplayClass62_0();
			V_0.u003cu003e4__this = this;
			V_0.startPosition = this.formatter.get_CurrentPosition();
			V_0.onFirstNonWhiteSpaceCharacter = new EventHandler<int>(V_0.u003cExecuteAndGetOffsetSpanu003eb__0);
			V_1 = new EventHandler(V_0.u003cExecuteAndGetOffsetSpanu003eb__1);
			this.formatter.add_FirstNonWhiteSpaceCharacterOnLineWritten(V_0.onFirstNonWhiteSpaceCharacter);
			this.formatter.add_NewLineWritten(V_1);
			toBeExecuted.Invoke();
			this.formatter.remove_FirstNonWhiteSpaceCharacterOnLineWritten(V_0.onFirstNonWhiteSpaceCharacter);
			this.formatter.remove_NewLineWritten(V_1);
			V_2 = this.formatter.get_CurrentPosition();
			return new OffsetSpan(V_0.startPosition, V_2);
		}

		protected virtual string GetArgumentName(ParameterReference parameter)
		{
			V_0 = parameter.Resolve();
			if (!this.GetCurrentMethodContext().get_ParameterDefinitionToNameMap().TryGetValue(V_0, out V_1))
			{
				V_1 = V_0.get_Name();
			}
			if (!this.get_Language().IsValidIdentifier(V_1))
			{
				V_1 = this.get_Language().ReplaceInvalidCharactersInIdentifier(V_1);
			}
			if (this.get_Language().IsGlobalKeyword(V_1))
			{
				V_1 = Utilities.EscapeNameIfNeeded(V_1, this.get_Language());
			}
			return V_1;
		}

		protected TypeReference GetBaseElementType(TypeReference type)
		{
			V_0 = type;
			while (V_0 as ArrayType != null)
			{
				V_0 = (V_0 as ArrayType).get_ElementType();
			}
			return V_0;
		}

		private TypeReference GetCollidingType(TypeReference typeReference)
		{
			if (typeReference as TypeSpecification == null)
			{
				return typeReference;
			}
			V_0 = typeReference as TypeSpecification;
			if (V_0 as PointerType != null)
			{
				return V_0.get_ElementType();
			}
			if (V_0 as PinnedType != null)
			{
				if (V_0.get_ElementType() as ByReferenceType == null)
				{
					return V_0.get_ElementType();
				}
				return (V_0.get_ElementType() as ByReferenceType).get_ElementType();
			}
			if (V_0 as ByReferenceType != null)
			{
				return V_0.get_ElementType();
			}
			if (V_0 as ArrayType != null)
			{
				V_2 = V_0.get_ElementType();
				while (V_2 as ArrayType != null)
				{
					V_2 = (V_2 as ArrayType).get_ElementType();
				}
				return V_2;
			}
			V_1 = V_0 as GenericInstanceType;
			if (V_1 == null)
			{
				return V_0;
			}
			if (!this.get_SupportsSpecialNullable() || V_1.GetFriendlyFullName(this.get_Language()).IndexOf("System.Nullable<") != 0 || !V_1.get_GenericArguments().get_Item(0).get_IsValueType())
			{
				return V_1;
			}
			V_3 = V_1.get_GenericArguments().get_Item(0);
			if (V_1.get_PostionToArgument().ContainsKey(0))
			{
				V_3 = V_1.get_PostionToArgument().get_Item(0);
			}
			return V_3;
		}

		private string GetCollidingTypeName(TypeReference typeReference)
		{
			if (!String.op_Equality(typeReference.get_Namespace(), "System"))
			{
				return typeReference.get_Name();
			}
			return this.ToEscapedTypeString(typeReference);
		}

		private MethodSpecificContext GetCurrentMethodContext()
		{
			if (this.membersStack.Peek() as FieldDefinition == null)
			{
				return this.get_MethodContext();
			}
			V_0 = this.membersStack.Peek() as FieldDefinition;
			return this.GetMethodContext(this.get_TypeContext().get_AssignmentData().get_Item(V_0.get_FullName()).get_AssignmentMethod());
		}

		private DefaultObjectExpression GetDefaultValueExpression(ParameterDefinition parameter)
		{
			V_0 = parameter.get_ParameterType();
			if (V_0.get_IsByReference())
			{
				V_0 = (V_0 as ByReferenceType).get_ElementType();
			}
			return new DefaultObjectExpression(V_0, null);
		}

		private TypeReference GetMemberType(IMemberDefinition member)
		{
			if (member as MethodDefinition != null)
			{
				return ((MethodDefinition)member).get_FixedReturnType();
			}
			if (member as FieldDefinition != null)
			{
				return ((FieldDefinition)member).get_FieldType();
			}
			if (member as PropertyDefinition != null)
			{
				return ((PropertyDefinition)member).get_PropertyType();
			}
			if (member as EventDefinition == null)
			{
				throw new NotSupportedException();
			}
			return ((EventDefinition)member).get_EventType();
		}

		protected string GetMethodKeyWord(MethodDefinition method)
		{
			if (this.IsOperator(method))
			{
				return this.get_KeyWordWriter().get_Operator();
			}
			if (method.IsFunction())
			{
				return this.get_KeyWordWriter().get_Function();
			}
			return this.get_KeyWordWriter().get_Sub();
		}

		private string GetMethodOriginalName(MethodDefinition method)
		{
			if (!this.get_TypeContext().get_MethodDefinitionToNameMap().ContainsKey(method))
			{
				return method.get_Name();
			}
			return this.get_TypeContext().get_MethodDefinitionToNameMap().get_Item(method);
		}

		protected virtual string GetOriginalFieldName(FieldDefinition field)
		{
			if (!this.get_TypeContext().get_BackingFieldToNameMap().ContainsKey(field))
			{
				return field.get_Name();
			}
			return this.get_TypeContext().get_BackingFieldToNameMap().get_Item(field);
		}

		protected string GetParameterName(ParameterDefinition parameter)
		{
			V_0 = parameter.get_Name();
			if (this.get_MethodContext() != null && this.get_MethodContext().get_Method().get_Body().get_Instructions().Count<Instruction>() > 0 || this.get_MethodContext().get_Method().get_IsJustDecompileGenerated() && this.get_MethodContext().get_Method() == parameter.get_Method() && !this.get_MethodContext().get_ParameterDefinitionToNameMap().TryGetValue(parameter, out V_0))
			{
				V_0 = parameter.get_Name();
			}
			return V_0;
		}

		private ICollection<string> GetUsedNamespaces()
		{
			V_0 = new HashSet<string>();
			V_2 = this.get_TypeContext().get_UsedNamespaces().GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					dummyVar0 = V_0.Add(V_3);
				}
			}
			finally
			{
				if (V_2 != null)
				{
					V_2.Dispose();
				}
			}
			stackVariable15 = this.get_TypeContext().get_CurrentType().get_Namespace();
			stackVariable17 = new Char[1];
			stackVariable17[0] = '.';
			V_1 = stackVariable15.Split(stackVariable17);
			if (V_1.Count<string>() > 0)
			{
				V_4 = new StringBuilder();
				V_5 = 0;
				while (V_5 < (int)V_1.Length)
				{
					if (V_5 > 0)
					{
						dummyVar1 = V_4.Append(".");
					}
					dummyVar2 = V_4.Append(V_1[V_5]);
					V_6 = V_4.ToString();
					if (!V_0.Contains(V_6))
					{
						dummyVar3 = V_0.Add(V_6);
					}
					V_5 = V_5 + 1;
				}
			}
			return V_0;
		}

		protected string GetVariableName(VariableReference variable)
		{
			V_0 = variable.Resolve();
			if (!this.GetCurrentMethodContext().get_VariableDefinitionToNameMap().TryGetValue(V_0, out V_1))
			{
				V_1 = V_0.get_Name();
			}
			return V_1;
		}

		protected bool HasArguments(PropertyDefinition property)
		{
			V_1 = 0;
			if (property.get_GetMethod() == null)
			{
				V_0 = property.get_SetMethod();
				V_1 = 1;
			}
			else
			{
				V_0 = property.get_GetMethod();
			}
			if (V_0 == null)
			{
				return false;
			}
			return V_0.get_Parameters().get_Count() > V_1;
		}

		private bool HasNoEmptyStatements(StatementCollection statements)
		{
			V_0 = statements.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					if (V_0.get_Current() as EmptyStatement != null)
					{
						continue;
					}
					V_1 = true;
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
		Label1:
			return V_1;
		Label0:
			return false;
		}

		private bool HaveSameConstraints(GenericParameter outerParameter, GenericParameter innerParameter)
		{
			if (innerParameter.get_HasNotNullableValueTypeConstraint() ^ outerParameter.get_HasNotNullableValueTypeConstraint())
			{
				return false;
			}
			if (innerParameter.get_HasReferenceTypeConstraint() ^ outerParameter.get_HasReferenceTypeConstraint())
			{
				return false;
			}
			if (!innerParameter.get_HasDefaultConstructorConstraint())
			{
				stackVariable12 = false;
			}
			else
			{
				stackVariable12 = !innerParameter.get_HasNotNullableValueTypeConstraint();
			}
			if (!outerParameter.get_HasDefaultConstructorConstraint())
			{
				stackVariable15 = false;
			}
			else
			{
				stackVariable15 = !outerParameter.get_HasNotNullableValueTypeConstraint();
			}
			if (stackVariable12 ^ stackVariable15)
			{
				return false;
			}
			if (innerParameter.get_HasConstraints() ^ outerParameter.get_HasConstraints())
			{
				return false;
			}
			if (innerParameter.get_Constraints().get_Count() != outerParameter.get_Constraints().get_Count())
			{
				return false;
			}
			V_0 = new List<TypeReference>(innerParameter.get_Constraints());
			V_1 = new List<TypeReference>(outerParameter.get_Constraints());
			stackVariable34 = V_0;
			stackVariable35 = BaseImperativeLanguageWriter.u003cu003ec.u003cu003e9__273_0;
			if (stackVariable35 == null)
			{
				dummyVar0 = stackVariable35;
				stackVariable35 = new Comparison<TypeReference>(BaseImperativeLanguageWriter.u003cu003ec.u003cu003e9.u003cHaveSameConstraintsu003eb__273_0);
				BaseImperativeLanguageWriter.u003cu003ec.u003cu003e9__273_0 = stackVariable35;
			}
			stackVariable34.Sort(stackVariable35);
			stackVariable36 = V_1;
			stackVariable37 = BaseImperativeLanguageWriter.u003cu003ec.u003cu003e9__273_1;
			if (stackVariable37 == null)
			{
				dummyVar1 = stackVariable37;
				stackVariable37 = new Comparison<TypeReference>(BaseImperativeLanguageWriter.u003cu003ec.u003cu003e9.u003cHaveSameConstraintsu003eb__273_1);
				BaseImperativeLanguageWriter.u003cu003ec.u003cu003e9__273_1 = stackVariable37;
			}
			stackVariable36.Sort(stackVariable37);
			V_2 = 0;
			while (V_2 < V_0.get_Count())
			{
				if (String.op_Inequality(V_0.get_Item(V_2).get_FullName(), V_1.get_Item(V_2).get_FullName()))
				{
					return false;
				}
				V_2 = V_2 + 1;
			}
			return true;
		}

		protected virtual bool IsComplexTarget(Expression target)
		{
			if (target.get_CodeNodeType() == 23)
			{
				V_0 = target as UnaryExpression;
				if (V_0.get_Operator() == 8 && V_0.get_Operand().get_CodeNodeType() == 25 && (V_0.get_Operand() as ArgumentReferenceExpression).get_ExpressionType().get_IsByReference())
				{
					return false;
				}
			}
			if (target.get_CodeNodeType() == 24 || target.get_CodeNodeType() == 23 || target.get_CodeNodeType() == 38 || target.get_CodeNodeType() == 40)
			{
				return true;
			}
			return target.get_CodeNodeType() == 50;
		}

		protected bool IsImplemented(TypeDefinition type, TypeDefinition baseType)
		{
			if (baseType == null || type == null)
			{
				return true;
			}
			if (!baseType.get_IsAbstract())
			{
				return true;
			}
			V_0 = baseType.get_Methods().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = new BaseImperativeLanguageWriter.u003cu003ec__DisplayClass21_0();
					V_1.method = V_0.get_Current();
					if (!V_1.method.get_IsAbstract() || type.get_Methods().FirstOrDefault<MethodDefinition>(new Func<MethodDefinition, bool>(V_1.u003cIsImplementedu003eb__0)) != null)
					{
						continue;
					}
					V_2 = false;
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				V_0.Dispose();
			}
		Label1:
			return V_2;
		Label0:
			return true;
		}

		private bool IsIndexerPropertyHiding(PropertyDefinition property)
		{
			if (property.get_GetMethod() != null)
			{
				stackVariable3 = property.get_GetMethod();
			}
			else
			{
				stackVariable3 = property.get_SetMethod();
			}
			V_0 = stackVariable3;
			if (property.get_DeclaringType().get_BaseType() != null)
			{
				stackVariable10 = property.get_DeclaringType().get_BaseType().Resolve();
			}
			else
			{
				stackVariable10 = null;
			}
			V_1 = stackVariable10;
		Label2:
			while (V_1 != null)
			{
				V_2 = V_1.get_Properties().GetEnumerator();
				try
				{
					while (V_2.MoveNext())
					{
						V_3 = V_2.get_Current();
						if (V_3.IsPrivate() || !V_3.IsIndexer())
						{
							continue;
						}
						if (V_3.get_GetMethod() != null)
						{
							stackVariable26 = V_3.get_GetMethod();
						}
						else
						{
							stackVariable26 = V_3.get_SetMethod();
						}
						V_4 = stackVariable26;
						if (V_4.get_Parameters().get_Count() != V_0.get_Parameters().get_Count())
						{
							continue;
						}
						V_5 = true;
						V_6 = 0;
						while (V_6 < V_4.get_Parameters().get_Count())
						{
							if (!String.op_Inequality(V_4.get_Parameters().get_Item(V_6).get_ParameterType().get_FullName(), V_0.get_Parameters().get_Item(V_6).get_ParameterType().get_FullName()))
							{
								V_6 = V_6 + 1;
							}
							else
							{
								V_5 = false;
								break;
							}
						}
						if (!V_5)
						{
							continue;
						}
						V_7 = true;
						goto Label1;
					}
					goto Label0;
				}
				finally
				{
					V_2.Dispose();
				}
			Label1:
				return V_7;
			}
			return false;
		Label0:
			if (V_1.get_BaseType() != null)
			{
				stackVariable64 = V_1.get_BaseType().Resolve();
			}
			else
			{
				stackVariable64 = null;
			}
			V_1 = stackVariable64;
			goto Label2;
		}

		private bool IsMethodHiding(MethodDefinition method)
		{
			if (method.get_DeclaringType().get_BaseType() != null)
			{
				stackVariable6 = method.get_DeclaringType().get_BaseType().Resolve();
			}
			else
			{
				stackVariable6 = null;
			}
			V_0 = stackVariable6;
		Label2:
			while (V_0 != null)
			{
				V_1 = V_0.get_Methods().GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_2 = V_1.get_Current();
						if (V_2.get_IsPrivate() || !String.op_Equality(V_2.get_Name(), method.get_Name()) || V_2.get_Parameters().get_Count() != method.get_Parameters().get_Count())
						{
							continue;
						}
						V_3 = true;
						V_4 = 0;
						while (V_4 < V_2.get_Parameters().get_Count())
						{
							if (!String.op_Inequality(V_2.get_Parameters().get_Item(V_4).get_ParameterType().get_FullName(), method.get_Parameters().get_Item(V_4).get_ParameterType().get_FullName()))
							{
								V_4 = V_4 + 1;
							}
							else
							{
								V_3 = false;
								break;
							}
						}
						if (!V_3)
						{
							continue;
						}
						V_5 = true;
						goto Label1;
					}
					goto Label0;
				}
				finally
				{
					V_1.Dispose();
				}
			Label1:
				return V_5;
			}
			return false;
		Label0:
			if (V_0.get_BaseType() != null)
			{
				stackVariable58 = V_0.get_BaseType().Resolve();
			}
			else
			{
				stackVariable58 = null;
			}
			V_0 = stackVariable58;
			goto Label2;
		}

		private bool IsNewDelegate(ObjectCreationExpression node)
		{
			if (node.get_Constructor() == null || node.get_Constructor().get_DeclaringType() == null)
			{
				return false;
			}
			V_0 = node.get_Constructor().get_DeclaringType().Resolve();
			if (V_0 != null && V_0.get_BaseType() != null && String.op_Equality(V_0.get_BaseType().get_FullName(), Type.GetTypeFromHandle(// 
			// Current member / type: System.Boolean Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter::IsNewDelegate(Telerik.JustDecompiler.Ast.Expressions.ObjectCreationExpression)
			// Exception in: System.Boolean IsNewDelegate(Telerik.JustDecompiler.Ast.Expressions.ObjectCreationExpression)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		private bool IsNull(Expression node)
		{
			if (node.get_CodeNodeType() != 22)
			{
				return false;
			}
			return (object)(node as LiteralExpression).get_Value() == (object)null;
		}

		private bool IsOperator(MethodDefinition method)
		{
			if (!method.get_IsOperator())
			{
				return false;
			}
			if (!this.get_Language().TryGetOperatorName(method.get_OperatorName(), out V_0))
			{
				return false;
			}
			return true;
		}

		protected bool IsPostUnaryOperator(UnaryOperator op)
		{
			if (op - 3 <= 1)
			{
				return true;
			}
			return false;
		}

		private bool IsPropertyHiding(PropertyDefinition property)
		{
			if (property.get_DeclaringType().get_BaseType() != null)
			{
				stackVariable6 = property.get_DeclaringType().get_BaseType().Resolve();
			}
			else
			{
				stackVariable6 = null;
			}
			V_0 = stackVariable6;
		Label2:
			while (V_0 != null)
			{
				V_1 = V_0.get_Properties().GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_2 = V_1.get_Current();
						if (V_2.IsPrivate() || !String.op_Equality(V_2.get_Name(), property.get_Name()))
						{
							continue;
						}
						V_3 = true;
						goto Label1;
					}
					goto Label0;
				}
				finally
				{
					V_1.Dispose();
				}
			Label1:
				return V_3;
			}
			return false;
		Label0:
			if (V_0.get_BaseType() != null)
			{
				stackVariable28 = V_0.get_BaseType().Resolve();
			}
			else
			{
				stackVariable28 = null;
			}
			V_0 = stackVariable28;
			goto Label2;
		}

		protected bool IsReferenceFromMscorlib(TypeReference reference)
		{
			if (String.op_Equality(reference.get_Scope().get_Name(), "mscorlib") || String.op_Equality(reference.get_Scope().get_Name(), "CommonLanguageRuntimeLibrary"))
			{
				return true;
			}
			return String.op_Equality(reference.get_Scope().get_Name(), "System.Runtime");
		}

		protected virtual bool IsTypeNameInCollision(string typeName)
		{
			if (this.IsTypeNameInCollisionWithParameters(typeName))
			{
				return true;
			}
			if (this.IsTypeNameInCollisionWithNamespace(typeName))
			{
				return true;
			}
			if (this.IsTypeNameInCollisionWithOtherType(typeName))
			{
				return true;
			}
			if (this.IsTypeNameInCollisionWithMembers(typeName))
			{
				return true;
			}
			if (this.IsTypeNameinCollisionWithVariables(typeName))
			{
				return true;
			}
			return false;
		}

		private bool IsTypeNameInCollisionWithMembers(string typeName)
		{
			return this.get_TypeContext().get_VisibleMembersNames().Contains(typeName);
		}

		private bool IsTypeNameInCollisionWithNamespace(string typeName)
		{
			stackVariable3 = this.get_TypeContext().get_CurrentType().get_Namespace();
			stackVariable5 = new Char[1];
			stackVariable5[0] = '.';
			V_0 = stackVariable3.Split(stackVariable5);
			if (V_0.Count<string>() > 0)
			{
				V_1 = new StringBuilder();
				V_2 = 0;
				while (V_2 < (int)V_0.Length)
				{
					if (V_2 > 0)
					{
						dummyVar0 = V_1.Append(".");
					}
					dummyVar1 = V_1.Append(V_0[V_2]);
					V_3 = V_1.ToString();
					if (this.get_ModuleContext().get_NamespaceHieararchy().TryGetValue(V_3, out V_4) && V_4.Contains(typeName))
					{
						return true;
					}
					V_2 = V_2 + 1;
				}
			}
			return false;
		}

		private bool IsTypeNameInCollisionWithOtherType(string typeName)
		{
			V_0 = this.GetUsedNamespaces();
			if (this.get_ModuleContext().get_CollisionTypesData().TryGetValue(typeName, out V_1) && V_1.Intersect<string>(V_0).Count<string>() > 1)
			{
				return true;
			}
			return false;
		}

		private bool IsTypeNameInCollisionWithParameters(string typeName)
		{
			if (this.get_CurrentMethod() != null)
			{
				V_0 = this.get_CurrentMethod().get_Parameters().GetEnumerator();
				try
				{
					while (V_0.MoveNext())
					{
						V_1 = V_0.get_Current();
						if (this.get_Language().get_IdentifierComparer().Compare(V_1.get_Name(), typeName) != 0)
						{
							continue;
						}
						V_2 = true;
						goto Label1;
					}
					goto Label0;
				}
				finally
				{
					V_0.Dispose();
				}
			Label1:
				return V_2;
			}
		Label0:
			return false;
		}

		private bool IsTypeNameinCollisionWithVariables(string typeName)
		{
			if (this.get_MethodContext() == null)
			{
				return false;
			}
			return this.get_MethodContext().get_VariableNamesCollection().Contains(typeName);
		}

		protected bool IsTypeParameterRedeclaration(GenericParameter genericParameter)
		{
			V_0 = genericParameter.get_Owner() as TypeReference;
			if (V_0 != null && V_0.get_IsNested())
			{
				V_1 = V_0.get_DeclaringType();
				if (V_1.get_HasGenericParameters() && genericParameter.get_Position() < V_1.get_GenericParameters().get_Count())
				{
					return true;
				}
			}
			return false;
		}

		protected void LeaveMethodInvocation()
		{
			dummyVar0 = this.methodReferences.Pop();
			return;
		}

		private void NormalizeNameContainingGenericSymbols(string[] tokensCollection, char genericSymbol, StringBuilder stringBuilder)
		{
			V_0 = 0;
			while (V_0 < (int)tokensCollection.Length)
			{
				if (V_0 > 0)
				{
					dummyVar0 = stringBuilder.Append(genericSymbol);
				}
				if (!this.NormalizeNameIfContainingGenericSymbols(tokensCollection[V_0], stringBuilder))
				{
					stackVariable19 = tokensCollection[V_0];
					stackVariable21 = new Char[1];
					stackVariable21[0] = ',';
					V_2 = stackVariable19.Split(stackVariable21);
					V_3 = 0;
					while (V_3 < (int)V_2.Length)
					{
						if (V_3 > 0)
						{
							dummyVar1 = stringBuilder.Append(", ");
						}
						dummyVar2 = stringBuilder.Append(this.get_Language().ReplaceInvalidCharactersInIdentifier(V_2[V_3]));
						V_3 = V_3 + 1;
					}
				}
				V_0 = V_0 + 1;
			}
			return;
		}

		private bool NormalizeNameIfContainingGenericSymbols(string name, StringBuilder stringBuilder)
		{
			V_0 = false;
			stackVariable3 = new Char[1];
			stackVariable3[0] = '<';
			V_1 = name.Split(stackVariable3);
			stackVariable9 = new Char[1];
			stackVariable9[0] = '>';
			V_2 = name.Split(stackVariable9);
			if ((int)V_1.Length <= 1)
			{
				if ((int)V_2.Length > 1)
				{
					V_0 = true;
					this.NormalizeNameContainingGenericSymbols(V_2, '>', stringBuilder);
				}
			}
			else
			{
				V_0 = true;
				this.NormalizeNameContainingGenericSymbols(V_1, '<', stringBuilder);
			}
			return V_0;
		}

		protected virtual void PostWriteGenericParametersConstraints(IGenericDefinition generic)
		{
			return;
		}

		protected virtual void PostWriteMethodReturnType(MethodDefinition method)
		{
			return;
		}

		protected bool ShouldWriteConstraintsAsComment(GenericParameter genericParameter)
		{
			V_0 = genericParameter.get_Owner() as TypeReference;
			if (V_0 == null)
			{
				return false;
			}
			V_1 = V_0.get_DeclaringType();
			if (V_1 == null || !V_1.get_HasGenericParameters() || V_1.get_GenericParameters().get_Count() <= genericParameter.get_Position())
			{
				return false;
			}
			V_2 = V_1.get_GenericParameters().get_Item(genericParameter.get_Position());
			if (V_2 == null)
			{
				return false;
			}
			if (this.HaveSameConstraints(V_2, genericParameter))
			{
				return false;
			}
			return true;
		}

		protected void StartInitializer(InitializerExpression node)
		{
			if (node.get_IsMultiLine())
			{
				this.WriteLine();
				return;
			}
			this.WriteSpace();
			return;
		}

		internal abstract string ToEscapedTypeString(TypeReference reference);

		protected abstract string ToString(BinaryOperator op, bool isOneSideNull = false);

		protected abstract string ToString(UnaryOperator op);

		public abstract string ToTypeString(TypeReference type);

		private bool TryWriteMethodAsOperator(MethodDefinition method)
		{
			if (!method.get_IsOperator())
			{
				return false;
			}
			if (!this.get_Language().TryGetOperatorName(method.get_OperatorName(), out V_0))
			{
				return false;
			}
			V_1 = false;
			if (String.op_Equality(method.get_OperatorName(), "Implicit") || String.op_Equality(method.get_OperatorName(), "Explicit"))
			{
				if (!String.op_Equality(method.get_OperatorName(), "Implicit"))
				{
					this.WriteKeyword(this.get_KeyWordWriter().get_Explicit());
				}
				else
				{
					this.WriteKeyword(this.get_KeyWordWriter().get_Implicit());
				}
				if (String.op_Equality(V_0, ""))
				{
					V_0 = method.get_ReturnType().GetGenericName(this.get_Language(), this.get_GenericLeftBracket(), this.get_GenericRightBracket());
				}
				this.WriteSpace();
				V_1 = true;
			}
			if (!V_1 && this.get_KeyWordWriter().get_Sub() == null)
			{
				this.WriteMethodReturnType(method);
				this.WriteSpace();
			}
			this.WriteKeyword(this.get_KeyWordWriter().get_Operator());
			this.WriteSpace();
			V_2 = this.formatter.get_CurrentPosition();
			this.WriteReference(V_0, method);
			V_3 = this.formatter.get_CurrentPosition() - 1;
			this.currentWritingInfo.get_MemberDeclarationToCodePostionMap().set_Item(method, new OffsetSpan(V_2, V_3));
			return true;
		}

		protected abstract bool TypeSupportsExplicitStaticMembers(TypeDefinition type);

		public override void Visit(ICodeNode node)
		{
			V_0 = new BaseImperativeLanguageWriter.u003cu003ec__DisplayClass61_0();
			V_0.u003cu003e4__this = this;
			V_0.node = node;
			if (!this.isStopped)
			{
				this.WriteCodeNodeLabel(V_0.node);
				if (V_0.node == null || V_0.node.get_CodeNodeType() == 58)
				{
					this.DoVisit(V_0.node);
				}
				else
				{
					V_1 = this.ExecuteAndGetOffsetSpan(new Action(V_0.u003cVisitu003eb__0));
					if (V_0.node != null)
					{
						this.currentWritingInfo.get_CodeMappingInfo().Add(V_0.node, new OffsetSpan(V_1.StartOffset, V_1.EndOffset - 1));
						if (V_0.node as Expression != null)
						{
							try
							{
								V_2 = (V_0.node as Expression).get_MappedInstructions().GetEnumerator();
								try
								{
									while (V_2.MoveNext())
									{
										V_3 = V_2.get_Current();
										this.currentWritingInfo.get_CodeMappingInfo().Add(V_3, V_1);
									}
								}
								finally
								{
									if (V_2 != null)
									{
										V_2.Dispose();
									}
								}
							}
							catch (ArgumentException exception_0)
							{
								this.OnExceptionThrown(exception_0);
							}
						}
					}
				}
			}
			return;
		}

		public override void Visit(IEnumerable collection)
		{
			V_0 = false;
			V_1 = collection.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = (ICodeNode)V_1.get_Current();
					if (V_2.get_CodeNodeType() != 58 || V_2 as Statement != null && String.op_Inequality((V_2 as Statement).get_Label(), ""))
					{
						if (!V_0)
						{
							V_0 = true;
						}
						else
						{
							this.WriteLine();
						}
					}
					this.Visit(V_2);
				}
			}
			finally
			{
				V_3 = V_1 as IDisposable;
				if (V_3 != null)
				{
					V_3.Dispose();
				}
			}
			return;
		}

		private void VisitAddressDereferenceExpression(UnaryExpression node)
		{
			if (node.get_Operand().get_CodeNodeType() == 25 && node.get_Operand().get_ExpressionType().get_IsByReference())
			{
				this.Visit(node.get_Operand());
				return;
			}
			this.WriteKeyword(this.get_KeyWordWriter().get_Dereference());
			this.Visit(node.get_Operand());
			return;
		}

		public override void VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpression node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_New());
			if (this.get_KeyWordWriter().get_ObjectInitializer() != null)
			{
				this.WriteSpace();
				this.WriteKeyword(this.get_KeyWordWriter().get_ObjectInitializer());
			}
			this.StartInitializer(node.get_Initializer());
			this.Visit(node.get_Initializer());
			return;
		}

		public override void VisitAnonymousPropertyInitializerExpression(AnonymousPropertyInitializerExpression node)
		{
			this.WritePropertyName(node.get_Property());
			return;
		}

		public override void VisitArgumentReferenceExpression(ArgumentReferenceExpression node)
		{
			this.Write(this.GetArgumentName(node.get_Parameter()));
			return;
		}

		public override void VisitArrayAssignmentFieldReferenceExpression(ArrayAssignmentFieldReferenceExpression node)
		{
			this.Visit(node.get_Field());
			return;
		}

		public override void VisitArrayAssignmentVariableReferenceExpression(ArrayVariableReferenceExpression node)
		{
			this.Visit(node.get_Variable());
			return;
		}

		public override void VisitArrayCreationExpression(ArrayCreationExpression node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_New());
			this.WriteSpace();
			this.WriteReferenceAndNamespaceIfInCollision(this.GetBaseElementType(node.get_ElementType()));
			V_0 = Utilities.IsInitializerPresent(node.get_Initializer());
			this.WriteArrayDimensions(node.get_Dimensions(), node.get_ElementType(), V_0);
			if (V_0)
			{
				this.StartInitializer(node.get_Initializer());
				this.Visit(node.get_Initializer());
			}
			return;
		}

		public override void VisitArrayLengthExpression(ArrayLengthExpression node)
		{
			stackVariable3 = this.IsComplexTarget(node.get_Target());
			if (stackVariable3)
			{
				this.WriteToken("(");
			}
			this.Visit(node.get_Target());
			if (stackVariable3)
			{
				this.WriteToken(")");
			}
			this.WriteToken(".");
			this.WriteReference("Length", null);
			return;
		}

		public override void VisitArrayVariableDeclarationExpression(ArrayVariableDeclarationExpression node)
		{
			this.Visit(node.get_Variable());
			return;
		}

		public override void VisitAutoPropertyConstructorInitializerExpression(AutoPropertyConstructorInitializerExpression node)
		{
			if (node.get_Target() == null)
			{
				this.WriteReferenceAndNamespaceIfInCollision(node.get_Property().get_DeclaringType());
			}
			else
			{
				if (node.get_Target() as ThisReferenceExpression == null)
				{
					throw new ArgumentException();
				}
				this.Visit(node.get_Target());
			}
			this.WriteToken(".");
			this.WritePropertyName(node.get_Property());
			return;
		}

		public override void VisitAwaitExpression(AwaitExpression node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_Await());
			this.WriteSpace();
			this.Visit(node.get_Expression());
			return;
		}

		public override void VisitBaseReferenceExpression(BaseReferenceExpression node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_Base());
			return;
		}

		public override void VisitBinaryExpression(BinaryExpression node)
		{
			this.Visit(node.get_Left());
			this.WriteSpace();
			if (this.IsNull(node.get_Left()))
			{
				stackVariable8 = true;
			}
			else
			{
				stackVariable8 = this.IsNull(node.get_Right());
			}
			V_1 = this.ToString(node.get_Operator(), stackVariable8);
			if (!this.get_Language().IsOperatorKeyword(V_1))
			{
				this.Write(V_1);
			}
			else
			{
				this.WriteKeyword(V_1);
			}
			if (node.get_Right().get_CodeNodeType() != 88)
			{
				this.WriteSpace();
			}
			else
			{
				this.StartInitializer(node.get_Right() as InitializerExpression);
			}
			this.WriteRightPartOfBinaryExpression(node);
			return;
		}

		public override void VisitBlockExpression(BlockExpression node)
		{
			this.WriteToken("{ ");
			this.VisitList(node.get_Expressions());
			this.WriteToken(" }");
			return;
		}

		public override void VisitBlockStatement(BlockStatement node)
		{
			V_0 = new BaseImperativeLanguageWriter.u003cu003ec__DisplayClass105_0();
			V_0.u003cu003e4__this = this;
			V_0.node = node;
			this.WriteBlock(new Action(V_0.u003cVisitBlockStatementu003eb__0), V_0.node.get_Label());
			return;
		}

		public override void VisitCanCastExpression(CanCastExpression node)
		{
			if (this.get_KeyWordWriter().get_IsType() != null)
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_IsType());
				this.WriteSpace();
			}
			this.Visit(node.get_Expression());
			this.WriteSpace();
			this.WriteKeyword(this.get_KeyWordWriter().get_Is());
			this.WriteSpace();
			this.WriteReferenceAndNamespaceIfInCollision(node.get_TargetType());
			return;
		}

		public override void VisitCaseGotoStatement(CaseGotoStatement node)
		{
			this.VisitGotoStatement(node);
			return;
		}

		public override void VisitDelegateCreationExpression(DelegateCreationExpression node)
		{
			if (!node.get_TypeIsImplicitlyInferable())
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_New());
				this.WriteSpace();
				this.WriteReference(node.get_Type());
				this.Write("(");
			}
			if (node.get_MethodExpression().get_CodeNodeType() != 50)
			{
				this.Write(node.get_Target());
				this.Write(".");
			}
			this.Write(node.get_MethodExpression());
			if (!node.get_TypeIsImplicitlyInferable())
			{
				this.Write(")");
			}
			return;
		}

		public override void VisitDelegateInvokeExpression(DelegateInvokeExpression node)
		{
			stackVariable3 = this.IsComplexTarget(node.get_Target());
			V_0 = false;
			if (stackVariable3)
			{
				this.WriteToken("(");
				if (node.get_Target().get_CodeNodeType() == 50 && (node.get_Target() as LambdaExpression).get_HasType())
				{
					V_0 = true;
					this.WriteToken("(");
					this.WriteReferenceAndNamespaceIfInCollision((node.get_Target() as LambdaExpression).get_ExpressionType());
					this.WriteToken(")");
					this.WriteToken("(");
				}
			}
			this.Visit(node.get_Target());
			if (stackVariable3)
			{
				if (V_0)
				{
					this.WriteToken(")");
				}
				this.WriteToken(")");
			}
			this.WriteToken("(");
			this.EnterMethodInvocation(node.get_InvokeMethodReference());
			this.VisitMethodParameters(node.get_Arguments());
			this.LeaveMethodInvocation();
			this.WriteToken(")");
			return;
		}

		public override void VisitDoWhileStatement(DoWhileStatement node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_Do());
			this.WriteLine();
			this.Visit(node.get_Body());
			this.WriteLine();
			this.WriteKeyword(this.get_KeyWordWriter().get_LoopWhile());
			this.WriteSpace();
			this.WriteSpecialBetweenParenthesis(node.get_Condition());
			this.WriteEndOfStatement();
			return;
		}

		public override void VisitEnumExpression(EnumExpression node)
		{
			if (node.get_FieldName() == null)
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_Null());
				return;
			}
			this.WriteReferenceAndNamespaceIfInCollision(node.get_ExpressionType());
			this.Write(".");
			V_0 = this.GetFieldName(node.get_Field());
			this.WriteReference(V_0, node.get_Field());
			return;
		}

		public override void VisitEventReferenceExpression(EventReferenceExpression node)
		{
			if (node.get_Target() == null)
			{
				this.WriteReferenceAndNamespaceIfInCollision(node.get_Event().get_DeclaringType());
			}
			else
			{
				stackVariable17 = this.IsComplexTarget(node.get_Target());
				if (stackVariable17)
				{
					this.WriteToken("(");
				}
				this.Visit(node.get_Target());
				if (stackVariable17)
				{
					this.WriteToken(")");
				}
			}
			this.WriteToken(".");
			this.WriteReference(node.get_Event().get_Name(), node.get_Event());
			return;
		}

		public override void VisitExpressionStatement(ExpressionStatement node)
		{
			this.Visit(node.get_Expression());
			if (this.get_ShouldOmitSemicolon().get_Count() == 0 || !this.get_ShouldOmitSemicolon().Peek())
			{
				this.WriteEndOfStatement();
			}
			return;
		}

		protected void VisitExtensionMethodParameters(IList<Expression> list)
		{
			this.VisitMethodParametersInternal(list, true);
			return;
		}

		public override void VisitFieldInitializerExpression(FieldInitializerExpression node)
		{
			this.WriteFieldName(node.get_Field());
			return;
		}

		public override void VisitFieldReferenceExpression(FieldReferenceExpression node)
		{
			if (node.get_Target() == null)
			{
				this.WriteReferenceAndNamespaceIfInCollision(node.get_Field().get_DeclaringType());
			}
			else
			{
				stackVariable14 = this.IsComplexTarget(node.get_Target());
				if (stackVariable14)
				{
					this.WriteToken("(");
				}
				this.Visit(node.get_Target());
				if (stackVariable14)
				{
					this.WriteToken(")");
				}
			}
			this.WriteToken(".");
			this.WriteFieldName(node.get_Field());
			return;
		}

		public override void VisitFixedStatement(FixedStatement expression)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_Fixed());
			this.WriteSpace();
			this.WriteBetweenParenthesis(expression.get_Expression());
			this.WriteLine();
			this.Visit(expression.get_Body());
			this.WriteSpecialEndBlock(this.get_KeyWordWriter().get_Fixed());
			return;
		}

		public override void VisitFromClause(FromClause node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_LinqFrom());
			this.WriteSpace();
			this.Visit(node.get_Identifier());
			this.WriteSpace();
			this.WriteKeyword(this.get_KeyWordWriter().get_LinqIn());
			this.WriteSpace();
			this.Visit(node.get_Collection());
			return;
		}

		public override void VisitGotoStatement(GotoStatement node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_GoTo());
			this.WriteSpace();
			this.Write(node.get_TargetLabel());
			this.WriteEndOfStatement();
			return;
		}

		public override void VisitGroupClause(GroupClause node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_LinqGroup());
			this.WriteSpace();
			this.Visit(node.get_Expression());
			this.WriteSpace();
			this.WriteKeyword(this.get_KeyWordWriter().get_LinqBy());
			this.WriteSpace();
			this.Visit(node.get_GroupKey());
			return;
		}

		public override void VisitIfElseIfStatement(IfElseIfStatement node)
		{
			V_0 = 0;
			while (V_0 < node.get_ConditionBlocks().get_Count())
			{
				if (V_0 != 0)
				{
					this.WriteLine();
					this.WriteKeyword(this.get_KeyWordWriter().get_ElseIf());
				}
				else
				{
					this.WriteKeyword(this.get_KeyWordWriter().get_If());
				}
				this.WriteSpace();
				V_1 = node.get_ConditionBlocks().get_Item(V_0);
				this.WriteBetweenParenthesis(V_1.get_Key());
				if (this.get_KeyWordWriter().get_Then() != null)
				{
					this.WriteSpace();
					this.WriteKeyword(this.get_KeyWordWriter().get_Then());
				}
				this.WriteLine();
				V_1 = node.get_ConditionBlocks().get_Item(V_0);
				this.Visit(V_1.get_Value());
				V_0 = V_0 + 1;
			}
			if (node.get_Else() != null)
			{
				this.WriteLine();
				this.WriteKeyword(this.get_KeyWordWriter().get_Else());
				this.WriteLine();
				this.Visit(node.get_Else());
			}
			this.WriteSpecialEndBlock(this.get_KeyWordWriter().get_If());
			return;
		}

		public override void VisitIfStatement(IfStatement node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_If());
			this.WriteSpace();
			this.WriteBetweenParenthesis(node.get_Condition());
			if (this.get_KeyWordWriter().get_Then() != null)
			{
				this.WriteSpace();
				this.WriteKeyword(this.get_KeyWordWriter().get_Then());
			}
			this.WriteLine();
			this.Visit(node.get_Then());
			if (node.get_Else() == null)
			{
				this.WriteSpecialEndBlock(this.get_KeyWordWriter().get_If());
				return;
			}
			this.WriteLine();
			this.WriteKeyword(this.get_KeyWordWriter().get_Else());
			this.WriteLine();
			this.Visit(node.get_Else());
			this.WriteSpecialEndBlock(this.get_KeyWordWriter().get_If());
			return;
		}

		protected override void VisitIIndexerExpression(IIndexerExpression node)
		{
			stackVariable3 = this.IsComplexTarget(node.get_Target());
			if (stackVariable3)
			{
				this.WriteToken("(");
			}
			this.Visit(node.get_Target());
			if (stackVariable3)
			{
				this.WriteToken(")");
			}
			this.WriteToken(this.get_IndexLeftBracket());
			this.VisitList(node.get_Indices());
			this.WriteToken(this.get_IndexRightBracket());
			return;
		}

		public override void VisitInitializerExpression(InitializerExpression node)
		{
			this.WriteToken("{");
			if (!node.get_IsMultiLine())
			{
				this.WriteSpace();
				this.VisitList(node.get_Expressions());
				this.WriteSpace();
			}
			else
			{
				this.Indent();
				this.WriteLine();
				this.VisitMultilineList(node.get_Expressions());
				this.WriteLine();
				this.Outdent();
			}
			this.WriteToken("}");
			return;
		}

		public override void VisitIntoClause(IntoClause node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_LinqInto());
			this.WriteSpace();
			this.Visit(node.get_Identifier());
			return;
		}

		public override void VisitJoinClause(JoinClause node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_LinqJoin());
			this.WriteSpace();
			this.Visit(node.get_InnerIdentifier());
			this.WriteSpace();
			this.WriteKeyword(this.get_KeyWordWriter().get_LinqIn());
			this.WriteSpace();
			this.Visit(node.get_InnerCollection());
			this.WriteSpace();
			this.WriteKeyword(this.get_KeyWordWriter().get_LinqOn());
			this.WriteSpace();
			this.Visit(node.get_OuterKey());
			this.WriteSpace();
			this.WriteKeyword(this.get_KeyWordWriter().get_LinqEquals());
			this.WriteSpace();
			this.Visit(node.get_InnerKey());
			return;
		}

		public override void VisitLambdaExpression(LambdaExpression node)
		{
			if (node.get_IsAsync() && this.get_KeyWordWriter().get_Async() != null)
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_Async());
				this.WriteSpace();
			}
			return;
		}

		public override void VisitLambdaParameterExpression(LambdaParameterExpression node)
		{
			if (!node.get_DisplayType())
			{
				this.Write(this.GetArgumentName(node.get_Parameter()));
				return;
			}
			this.WriteTypeAndName(node.get_ExpressionType(), this.GetArgumentName(node.get_Parameter()));
			return;
		}

		public override void VisitLetClause(LetClause node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_LinqLet());
			this.WriteSpace();
			this.Visit(node.get_Identifier());
			this.Write(" = ");
			this.Visit(node.get_Expression());
			return;
		}

		protected void VisitList(IList<Expression> list)
		{
			V_0 = 0;
			while (V_0 < list.get_Count())
			{
				if (V_0 > 0)
				{
					this.WriteToken(",");
					this.WriteSpace();
				}
				this.Visit(list.get_Item(V_0));
				V_0 = V_0 + 1;
			}
			return;
		}

		public override void VisitLiteralExpression(LiteralExpression node)
		{
			this.WriteLiteralInLanguageSyntax(node.get_Value());
			return;
		}

		public override void VisitLockStatement(LockStatement expression)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_Lock());
			this.WriteSpace();
			this.WriteSpecialBetweenParenthesis(expression.get_Expression());
			this.WriteLine();
			this.Visit(expression.get_Body());
			this.WriteSpecialEndBlock(this.get_KeyWordWriter().get_Lock());
			return;
		}

		public override void VisitMakeRefExpression(MakeRefExpression node)
		{
			this.WriteKeyword("__makeref");
			this.WriteToken("(");
			this.Visit(node.get_Expression());
			this.WriteToken(")");
			return;
		}

		public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			V_0 = false;
			if (node.get_MethodExpression().get_CodeNodeType() == 20)
			{
				V_2 = node.get_MethodExpression();
				V_3 = V_2.get_Method();
				if (V_3 != null && !V_3.get_HasThis() && V_2.get_MethodDefinition() != null)
				{
					V_0 = V_2.get_MethodDefinition().get_IsExtensionMethod();
				}
			}
			if (!V_0)
			{
				if (node.get_MethodExpression().get_Target() != null)
				{
					this.WriteMethodTarget(node.get_MethodExpression().get_Target());
				}
				if (!node.get_MethodExpression().get_Method().get_HasThis() && node.get_MethodExpression().get_MethodDefinition() != null && !node.get_MethodExpression().get_MethodDefinition().get_IsExtensionMethod() || node.get_MethodExpression().get_MethodDefinition() == null)
				{
					this.WriteReferenceAndNamespaceIfInCollision(node.get_MethodExpression().get_Method().get_DeclaringType());
					this.WriteToken(".");
				}
			}
			else
			{
				if (node.get_Arguments().get_Count() > 0)
				{
					this.WriteMethodTarget(node.get_Arguments().get_Item(0));
				}
			}
			this.WriteMethodReference(node.get_MethodExpression());
			V_1 = false;
			if (V_1)
			{
				stackVariable20 = this.get_IndexLeftBracket();
			}
			else
			{
				stackVariable20 = "(";
			}
			this.WriteToken(stackVariable20);
			if (node.get_MethodExpression() == null)
			{
				this.VisitMethodParameters(node.get_Arguments());
			}
			else
			{
				this.EnterMethodInvocation(node.get_MethodExpression().get_Method());
				if (V_0)
				{
					this.VisitExtensionMethodParameters(node.get_Arguments());
				}
				else
				{
					this.VisitMethodParameters(node.get_Arguments());
				}
				this.LeaveMethodInvocation();
			}
			if (V_1)
			{
				stackVariable29 = this.get_IndexRightBracket();
			}
			else
			{
				stackVariable29 = ")";
			}
			this.WriteToken(stackVariable29);
			return;
		}

		protected void VisitMethodParameters(IList<Expression> list)
		{
			this.VisitMethodParametersInternal(list, false);
			return;
		}

		private void VisitMethodParametersInternal(IList<Expression> list, bool isExtensionMethod)
		{
			if (isExtensionMethod)
			{
				stackVariable1 = 1;
			}
			else
			{
				stackVariable1 = 0;
			}
			V_0 = stackVariable1;
			while (V_0 < list.get_Count())
			{
				if (V_0 > 0 && !isExtensionMethod || V_0 > 1 & isExtensionMethod)
				{
					this.WriteToken(",");
					this.WriteSpace();
				}
				V_1 = false;
				if (list.get_Item(V_0).get_CodeNodeType() == 25 && (list.get_Item(V_0) as ArgumentReferenceExpression).get_Parameter().get_ParameterType().get_IsByReference())
				{
					V_1 = true;
				}
				if (list.get_Item(V_0) as UnaryExpression != null && (list.get_Item(V_0) as UnaryExpression).get_Operator() == 7 && this.get_MethodReferences().get_Count() > 0)
				{
					V_1 = true;
				}
				if (list.get_Item(V_0).get_CodeNodeType() == 19 && (list.get_Item(V_0) as MethodInvocationExpression).get_IsByReference())
				{
					V_1 = this.CheckIfParameterIsByRef(this.get_MethodReferences().Peek(), V_0);
				}
				if (list.get_Item(V_0).get_CodeNodeType() == 26 && (list.get_Item(V_0) as VariableReferenceExpression).get_Variable().get_VariableType().get_IsByReference())
				{
					V_1 = this.CheckIfParameterIsByRef(this.get_MethodReferences().Peek(), V_0);
				}
				if (V_1)
				{
					V_2 = this.get_MethodReferences().Peek().Resolve();
					if (V_2 == null)
					{
						if (this.get_ShouldWriteOutAndRefOnInvocation())
						{
							this.WriteKeyword(this.get_KeyWordWriter().get_ByRef());
							this.WriteSpace();
						}
					}
					else
					{
						if (this.get_ShouldWriteOutAndRefOnInvocation())
						{
							this.WriteOutOrRefKeyWord(V_2.get_Parameters().get_Item(V_0));
							this.WriteSpace();
						}
					}
					if (list.get_Item(V_0) as UnaryExpression == null)
					{
						goto Label1;
					}
					this.Visit((list.get_Item(V_0) as UnaryExpression).get_Operand());
					goto Label0;
				}
			Label1:
				this.Visit(list.get_Item(V_0));
			Label0:
				V_0 = V_0 + 1;
			}
			return;
		}

		public override void VisitMethodReferenceExpression(MethodReferenceExpression node)
		{
			if (node.get_Target() == null)
			{
				this.WriteReferenceAndNamespaceIfInCollision(node.get_Method().get_DeclaringType());
				this.WriteToken(".");
			}
			else
			{
				this.WriteMethodTarget(node.get_Target());
			}
			this.WriteMethodReference(node);
			return;
		}

		private void VisitMultilineList(ExpressionCollection expressions)
		{
			V_0 = 0;
			while (V_0 < expressions.get_Count())
			{
				this.Visit(expressions.get_Item(V_0));
				if (V_0 != expressions.get_Count() - 1)
				{
					this.WriteToken(",");
					this.WriteLine();
				}
				V_0 = V_0 + 1;
			}
			return;
		}

		public override void VisitObjectCreationExpression(ObjectCreationExpression node)
		{
			if (this.IsNewDelegate(node))
			{
				this.WriteDelegateCreation(node);
				return;
			}
			this.WriteKeyword(this.get_KeyWordWriter().get_New());
			this.WriteSpace();
			if (node.get_Constructor() == null)
			{
				this.WriteReferenceAndNamespaceIfInCollision(node.get_Type());
			}
			else
			{
				this.WriteConstructorInvocation(node);
			}
			this.WriteToken("(");
			this.EnterMethodInvocation(node.get_Constructor());
			this.VisitMethodParameters(node.get_Arguments());
			this.LeaveMethodInvocation();
			this.WriteToken(")");
			if (node.get_Initializer() != null)
			{
				if (node.get_Initializer().get_InitializerType() != 1)
				{
					if (node.get_Initializer().get_InitializerType() == InitializerType.CollectionInitializer && this.get_KeyWordWriter().get_CollectionInitializer() != null)
					{
						this.WriteSpace();
						this.WriteKeyword(this.get_KeyWordWriter().get_CollectionInitializer());
					}
				}
				else
				{
					if (this.get_KeyWordWriter().get_ObjectInitializer() != null)
					{
						this.WriteSpace();
						this.WriteKeyword(this.get_KeyWordWriter().get_ObjectInitializer());
					}
				}
				this.StartInitializer(node.get_Initializer());
				this.Visit(node.get_Initializer());
			}
			return;
		}

		public override void VisitOrderByClause(OrderByClause node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_LinqOrderBy());
			this.WriteSpace();
			V_0 = 0;
			while (V_0 < node.get_ExpressionToOrderDirectionMap().get_Count())
			{
				if (V_0 > 0)
				{
					this.Write(", ");
				}
				V_1 = node.get_ExpressionToOrderDirectionMap().get_Item(V_0);
				this.Visit(V_1.get_Key());
				if (node.get_ExpressionToOrderDirectionMap().get_Item(V_0).get_Value() == 1)
				{
					this.WriteSpace();
					this.WriteKeyword(this.get_KeyWordWriter().get_LinqDescending());
				}
				V_0 = V_0 + 1;
			}
			return;
		}

		public override void VisitParenthesesExpression(ParenthesesExpression parenthesesExpression)
		{
			this.WriteToken("(");
			this.Visit(parenthesesExpression.get_Expression());
			this.WriteToken(")");
			return;
		}

		public override void VisitPropertyInitializerExpression(PropertyInitializerExpression node)
		{
			this.WritePropertyName(node.get_Property());
			return;
		}

		public override void VisitPropertyReferenceExpression(PropertyReferenceExpression node)
		{
			if (node.get_Target() == null)
			{
				this.WriteReferenceAndNamespaceIfInCollision(node.get_DeclaringType());
			}
			else
			{
				stackVariable17 = this.IsComplexTarget(node.get_Target());
				if (stackVariable17)
				{
					this.WriteToken("(");
				}
				this.Visit(node.get_Target());
				if (stackVariable17)
				{
					this.WriteToken(")");
				}
			}
			if (node.get_IsIndexer())
			{
				this.WriteIndexerArguments(node);
				return;
			}
			this.WriteToken(".");
			this.WritePropertyName(node.get_Property());
			return;
		}

		public override void VisitReturnExpression(ReturnExpression node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_Return());
			if (node.get_Value() != null)
			{
				this.WriteSpace();
				this.Visit(node.get_Value());
			}
			return;
		}

		public override void VisitSelectClause(SelectClause node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_LinqSelect());
			this.WriteSpace();
			this.Visit(node.get_Expression());
			return;
		}

		public override void VisitShortFormReturnExpression(ShortFormReturnExpression node)
		{
			if (node.get_Value() != null)
			{
				this.Visit(node.get_Value());
			}
			return;
		}

		public override void VisitSizeOfExpression(SizeOfExpression node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_SizeOf());
			this.WriteToken("(");
			this.WriteReferenceAndNamespaceIfInCollision(node.get_Type());
			this.WriteToken(")");
			return;
		}

		public override void VisitStackAllocExpression(StackAllocExpression node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_Stackalloc());
			this.WriteSpace();
			this.WriteReference((node.get_ExpressionType() as PointerType).get_ElementType());
			this.WriteToken(this.get_IndexLeftBracket());
			this.Visit(node.get_Expression());
			this.WriteToken(this.get_IndexRightBracket());
			return;
		}

		public override void VisitThisReferenceExpression(ThisReferenceExpression node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_This());
			return;
		}

		public override void VisitThrowExpression(ThrowExpression node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_Throw());
			if (node.get_Expression() != null)
			{
				this.WriteSpace();
				this.Visit(node.get_Expression());
			}
			return;
		}

		public override void VisitTryStatement(TryStatement node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_Try());
			this.WriteLine();
			this.Visit(node.get_Try());
			if (node.get_CatchClauses().get_Count() != 0)
			{
				this.WriteLine();
				this.Visit(node.get_CatchClauses());
			}
			if (node.get_Finally() != null)
			{
				this.WriteLine();
				this.WriteKeyword(this.get_KeyWordWriter().get_Finally());
				this.WriteLine();
				this.Visit(node.get_Finally());
			}
			this.WriteSpecialEndBlock(this.get_KeyWordWriter().get_Try());
			return;
		}

		public override void VisitTypeOfExpression(TypeOfExpression node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_TypeOf());
			this.WriteToken("(");
			this.WriteGenericReference(node.get_Type());
			this.WriteToken(")");
			return;
		}

		public override void VisitTypeReferenceExpression(TypeReferenceExpression node)
		{
			this.WriteReferenceAndNamespaceIfInCollision(node.get_Type());
			return;
		}

		public override void VisitUnaryExpression(UnaryExpression node)
		{
			if (node.get_Operator() == 8)
			{
				this.VisitAddressDereferenceExpression(node);
				return;
			}
			this.Visit(node.get_Operand());
			return;
		}

		public override void VisitVariableDeclarationExpression(VariableDeclarationExpression node)
		{
			this.WriteVariableTypeAndName(node.get_Variable());
			return;
		}

		public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			this.Write(this.GetVariableName(node.get_Variable()));
			return;
		}

		public override void VisitWhereClause(WhereClause node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_LinqWhere());
			this.WriteSpace();
			this.Visit(node.get_Condition());
			return;
		}

		public override void VisitWhileStatement(WhileStatement node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_While());
			this.WriteSpace();
			this.WriteSpecialBetweenParenthesis(node.get_Condition());
			this.WriteLine();
			this.Visit(node.get_Body());
			this.WriteSpecialEndBlock(this.get_KeyWordWriter().get_While());
			return;
		}

		protected override void Write(FieldDefinition field)
		{
			if (this.TryWriteEnumField(field))
			{
				return;
			}
			this.WriteFieldDeclaration(field);
			if (this.get_TypeContext().get_AssignmentData().ContainsKey(field.get_FullName()) && this.get_TypeContext().get_AssignmentData().get_Item(field.get_FullName()) != null)
			{
				this.WriteSpace();
				this.WriteToken("=");
				this.WriteSpace();
				this.Visit(this.get_TypeContext().get_AssignmentData().get_Item(field.get_FullName()).get_AssignmentExpression());
			}
			this.WriteEndOfStatement();
			return;
		}

		protected override void Write(Statement statement)
		{
			this.Visit(statement);
			return;
		}

		protected override void Write(EventDefinition event)
		{
			V_0 = new BaseImperativeLanguageWriter.u003cu003ec__DisplayClass66_0();
			V_0.u003cu003e4__this = this;
			V_0.event = event;
			if (V_0.event.get_AddMethod() == null || V_0.event.get_AddMethod().get_Body() != null)
			{
				if (V_0.event.get_RemoveMethod() == null)
				{
					stackVariable11 = false;
				}
				else
				{
					stackVariable11 = V_0.event.get_RemoveMethod().get_Body() == null;
				}
			}
			else
			{
				stackVariable11 = true;
			}
			V_1 = true;
			if (!stackVariable11)
			{
				V_1 = this.get_TypeContext().get_AutoImplementedEvents().Contains(V_0.event);
			}
			this.WriteEventDeclaration(V_0.event, V_1);
			if (V_1)
			{
				this.WriteEndOfStatement();
				return;
			}
			V_2 = this.formatter.get_CurrentPosition();
			this.formatter.WriteStartBlock();
			this.WriteLine();
			this.WriteBlock(new Action(V_0.u003cWriteu003eb__0), "");
			this.WriteSpecialEndBlock(this.get_KeyWordWriter().get_Event());
			this.currentWritingInfo.get_MemberDefinitionToFoldingPositionMap().set_Item(V_0.event, new OffsetSpan(V_2, this.formatter.get_CurrentPosition() - 1));
			this.formatter.WriteEndBlock();
			return;
		}

		protected override void Write(PropertyDefinition property)
		{
			V_0 = new BaseImperativeLanguageWriter.u003cu003ec__DisplayClass71_0();
			V_0.u003cu003e4__this = this;
			V_0.property = property;
			if (V_0.property.ShouldStaySplit() && !this.get_MethodContextsMissing())
			{
				this.WriteSplitProperty(V_0.property);
				return;
			}
			if (this.get_Language().get_SupportsInlineInitializationOfAutoProperties() && this.get_TypeContext().get_AutoImplementedProperties().Contains(V_0.property) && this.get_TypeContext().get_AssignmentData().ContainsKey(V_0.property.get_FullName()) && this.get_TypeContext().get_AssignmentData().get_Item(V_0.property.get_FullName()) != null)
			{
				this.WriteInitializedAutoProperty(V_0.property, this.get_TypeContext().get_AssignmentData().get_Item(V_0.property.get_FullName()).get_AssignmentExpression());
				return;
			}
			this.WritePropertyDeclaration(V_0.property);
			V_1 = this.formatter.get_CurrentPosition();
			this.formatter.WriteStartBlock();
			this.WriteLine();
			this.WriteBlock(new Action(V_0.u003cWriteu003eb__0), "");
			if (this.get_KeyWordWriter().get_Property() != null)
			{
				this.WriteSpecialEndBlock(this.get_KeyWordWriter().get_Property());
			}
			this.currentWritingInfo.get_MemberDefinitionToFoldingPositionMap().set_Item(V_0.property, new OffsetSpan(V_1, this.formatter.get_CurrentPosition() - 1));
			this.formatter.WriteEndBlock();
			return;
		}

		protected override void Write(MethodDefinition method)
		{
			if (this.get_MethodContext() != null && this.get_MethodContext().get_IsDestructor())
			{
				this.WriteDestructor(method);
				return;
			}
			this.WriteMethod(method);
			return;
		}

		protected override void Write(Expression expression)
		{
			this.Visit(expression);
			return;
		}

		protected virtual void WriteAddOn(EventDefinition event)
		{
			V_7 = event.get_AddMethod().get_MetadataToken();
			V_0 = V_7.ToUInt32();
			this.membersStack.Push(event.get_AddMethod());
			V_1 = this.formatter.get_CurrentPosition();
			stackVariable13 = this.get_AttributeWriter();
			stackVariable15 = event.get_AddMethod();
			stackVariable17 = new String[1];
			stackVariable17[0] = "System.Runtime.CompilerServices.CompilerGeneratedAttribute";
			stackVariable13.WriteMemberAttributesAndNewLine(stackVariable15, stackVariable17, false);
			this.AddMemberAttributesOffsetSpan(V_0, V_1, this.formatter.get_CurrentPosition());
			V_2 = this.formatter.get_CurrentPosition();
			V_3 = this.formatter.get_CurrentPosition();
			this.WriteMoreRestrictiveMethodVisibility(event.get_AddMethod(), event.get_RemoveMethod());
			V_4 = this.formatter.get_CurrentPosition();
			this.WriteKeyword(this.get_KeyWordWriter().get_AddOn());
			this.WriteEventAddOnParameters(event);
			V_5 = this.formatter.get_CurrentPosition() - 1;
			this.currentWritingInfo.get_MemberDeclarationToCodePostionMap().set_Item(event.get_AddMethod(), new OffsetSpan(V_4, V_5));
			this.WriteLine();
			this.Write(this.GetStatement(event.get_AddMethod()));
			this.WriteSpecialEndBlock(this.get_KeyWordWriter().get_AddOn());
			this.currentWritingInfo.get_MemberDefinitionToFoldingPositionMap().Add(event.get_AddMethod(), new OffsetSpan(V_2, this.formatter.get_CurrentPosition() - 1));
			this.currentWritingInfo.get_MemberTokenToDecompiledCodeMap().Add(V_0, new OffsetSpan(V_3, this.formatter.get_CurrentPosition() - 1));
			dummyVar0 = this.membersStack.Pop();
			return;
		}

		protected void WriteAndMapParameterToCode(Action write, int index)
		{
			V_0 = this.formatter.get_CurrentPosition();
			write.Invoke();
			V_1 = this.membersStack.Peek();
			V_2 = new OffsetSpan(V_0, this.formatter.get_CurrentPosition());
			this.currentWritingInfo.get_CodeMappingInfo().Add(V_1, index, V_2);
			return;
		}

		protected void WriteAndMapVariableToCode(Action write, VariableDefinition variable)
		{
			V_0 = this.formatter.get_CurrentPosition();
			write.Invoke();
			V_1 = new OffsetSpan(V_0, this.formatter.get_CurrentPosition());
			try
			{
				this.currentWritingInfo.get_CodeMappingInfo().Add(variable, V_1);
			}
			catch (ArgumentException exception_0)
			{
				this.OnExceptionThrown(exception_0);
			}
			return;
		}

		protected virtual void WriteArrayDimensions(ExpressionCollection dimensions, TypeReference arrayType, bool isInitializerPresent)
		{
			V_0 = new List<int>();
			V_1 = arrayType;
			while (V_1 as ArrayType != null)
			{
				V_2 = V_1 as ArrayType;
				V_0.Add(V_2.get_Dimensions().get_Count());
				V_1 = V_2.get_ElementType();
			}
			this.WriteToken(this.get_IndexLeftBracket());
			V_3 = 0;
			while (V_3 < dimensions.get_Count())
			{
				if (V_3 > 0)
				{
					this.WriteToken(",");
					if (!isInitializerPresent)
					{
						this.WriteToken(" ");
					}
				}
				if (!isInitializerPresent)
				{
					this.Visit(dimensions.get_Item(V_3));
				}
				V_3 = V_3 + 1;
			}
			this.WriteToken(this.get_IndexRightBracket());
			V_4 = V_0.GetEnumerator();
			try
			{
				while (V_4.MoveNext())
				{
					V_5 = V_4.get_Current();
					this.WriteToken(this.get_IndexLeftBracket());
					V_6 = 1;
					while (V_6 < V_5)
					{
						this.WriteToken(",");
						V_6 = V_6 + 1;
					}
					this.WriteToken(this.get_IndexRightBracket());
				}
			}
			finally
			{
				((IDisposable)V_4).Dispose();
			}
			return;
		}

		protected override void WriteAttributes(IMemberDefinition member, IEnumerable<string> ignoredAttributes = null)
		{
			V_0 = member as MethodDefinition;
			if (V_0 != null && V_0.IsAsync())
			{
				stackVariable17 = new String[2];
				stackVariable17[0] = "System.Diagnostics.DebuggerStepThroughAttribute";
				stackVariable17[1] = "System.Runtime.CompilerServices.AsyncStateMachineAttribute";
				V_1 = stackVariable17;
				if (ignoredAttributes != null)
				{
					ignoredAttributes = new List<string>(ignoredAttributes);
					((List<string>)ignoredAttributes).AddRange(V_1);
				}
				else
				{
					ignoredAttributes = new List<string>(V_1);
				}
			}
			stackVariable4 = this.get_AttributeWriter();
			stackVariable5 = member;
			stackVariable6 = ignoredAttributes;
			if (this.get_TypeContext().get_CurrentType() != member)
			{
				stackVariable11 = false;
			}
			else
			{
				stackVariable11 = this.get_TypeContext().get_IsWinRTImplementation();
			}
			stackVariable4.WriteMemberAttributesAndNewLine(stackVariable5, stackVariable6, stackVariable11);
			return;
		}

		protected virtual void WriteBaseConstructorInvokation(MethodInvocationExpression baseConstructorInvokation)
		{
			return;
		}

		protected virtual void WriteBaseTypeInheritColon()
		{
			return;
		}

		protected void WriteBetweenParenthesis(Expression expression)
		{
			this.WriteToken("(");
			this.Visit(expression);
			this.WriteToken(")");
			return;
		}

		public void WriteBitwiseOr()
		{
			this.WriteKeyword(this.ToString(21, false));
			return;
		}

		protected virtual void WriteBlock(Action action, string label)
		{
			return;
		}

		protected override void WriteBodyInternal(IMemberDefinition member)
		{
			this.membersStack.Push(member);
			if (member as MethodDefinition == null)
			{
				if (member as PropertyDefinition == null)
				{
					if (member as EventDefinition != null)
					{
						V_3 = (EventDefinition)member;
						if (V_3.get_AddMethod() != null && V_3.get_AddMethod().get_Body() == null || V_3.get_RemoveMethod() != null && V_3.get_RemoveMethod().get_Body() == null)
						{
							return;
						}
						this.WriteEventMethods(V_3);
					}
				}
				else
				{
					this.WritePropertyMethods((PropertyDefinition)member, false);
				}
			}
			else
			{
				V_0 = (MethodDefinition)member;
				if (V_0.get_Body() != null)
				{
					V_1 = this.GetStatement(V_0);
					if (this.get_MethodContext().get_Method().get_IsConstructor() && !this.get_MethodContext().get_Method().get_IsStatic() && this.get_MethodContext().get_CtorInvokeExpression() != null && !this.get_RemoveBaseConstructorInvocation())
					{
						this.WriteBaseConstructorInvokation(this.get_MethodContext().get_CtorInvokeExpression());
					}
					this.Write(V_1);
				}
				else
				{
					this.WriteEndOfStatement();
				}
			}
			dummyVar0 = this.membersStack.Pop();
			return;
		}

		protected void WriteCodeNodeLabel(ICodeNode node)
		{
			if (node as Statement != null && node.get_CodeNodeType() != CodeNodeType.BlockStatement)
			{
				V_0 = node as Statement;
				this.WriteLabel(V_0.get_Label());
				if (node.get_CodeNodeType() != 58 && String.op_Inequality(V_0.get_Label(), ""))
				{
					this.WriteLine();
				}
			}
			return;
		}

		protected virtual void WriteConstructorGenericConstraint()
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_New());
			return;
		}

		private void WriteConstructorInvocation(ObjectCreationExpression node)
		{
			if (node.get_Constructor().get_DeclaringType() as TypeSpecification != null)
			{
				V_0 = node.get_Constructor().get_DeclaringType() as GenericInstanceType;
				if (V_0 != null && this.get_SupportsSpecialNullable() && V_0.GetFriendlyFullName(this.get_Language()).IndexOf("System.Nullable<") == 0 && V_0.get_GenericArguments().get_Item(0).get_IsValueType())
				{
					V_1 = V_0.get_GenericArguments().get_Item(0);
					if (V_0.get_PostionToArgument().ContainsKey(0))
					{
						V_1 = V_0.get_PostionToArgument().get_Item(0);
					}
					this.WriteReferenceAndNamespaceIfInCollision(V_1);
					this.WriteToken("?");
					return;
				}
			}
			if (String.op_Equality(node.get_Constructor().get_DeclaringType().get_Namespace(), "System"))
			{
				this.WriteReferenceAndNamespaceIfInCollision(node.get_Constructor().get_DeclaringType());
				return;
			}
			if (node.get_Constructor().get_DeclaringType().get_DeclaringType() == null)
			{
				V_9 = this.IsTypeNameInCollision(node.get_Constructor().get_DeclaringType().get_Name());
				this.WriteNamespace(node.get_Constructor().get_DeclaringType().GetElementType(), V_9);
				this.WriteConstructorNameAndGenericArguments(node, true, 0);
				return;
			}
			V_2 = node.get_Constructor().get_DeclaringType().get_DeclaringType();
			if (node.get_Constructor().get_DeclaringType().get_IsGenericInstance())
			{
				V_3 = node.get_Constructor().get_DeclaringType() as GenericInstanceType;
				if (V_2.get_HasGenericParameters())
				{
					V_4 = new GenericInstanceType(V_2);
					V_5 = new Collection<TypeReference>(V_3.get_GenericArguments());
					V_6 = new Collection<TypeReference>(V_4.get_GenericArguments());
					V_7 = V_2.get_GenericParameters().get_Count();
					V_8 = 0;
					while (V_8 < V_7)
					{
						V_4.AddGenericArgument(V_3.get_GenericArguments().get_Item(V_8));
						V_4.get_GenericArguments().Add(V_3.get_GenericArguments().get_Item(V_8));
						V_8 = V_8 + 1;
					}
					this.WriteReferenceAndNamespaceIfInCollision(V_4);
					this.Write(".");
					if (V_3.get_GenericArguments().get_Count() - V_7 <= 0)
					{
						this.WriteConstructorNameAndGenericArguments(node, false, 0);
					}
					else
					{
						this.WriteConstructorNameAndGenericArguments(node, true, V_7);
					}
					V_3.get_GenericArguments().Clear();
					V_3.get_GenericArguments().AddRange(V_5);
					V_4.get_GenericArguments().Clear();
					V_4.get_GenericArguments().AddRange(V_6);
					return;
				}
			}
			this.WriteReferenceAndNamespaceIfInCollision(V_2);
			this.Write(".");
			this.WriteConstructorNameAndGenericArguments(node, true, 0);
			return;
		}

		private void WriteConstructorName(MethodDefinition method)
		{
			V_0 = this.GetTypeName(method.get_DeclaringType());
			if (this.get_KeyWordWriter().get_Constructor() != null)
			{
				if (this.get_KeyWordWriter().get_Sub() != null)
				{
					this.WriteKeyword(this.get_KeyWordWriter().get_Sub());
					this.WriteSpace();
				}
				V_0 = this.get_KeyWordWriter().get_Constructor();
			}
			V_1 = this.formatter.get_CurrentPosition();
			this.WriteReference(V_0, method);
			V_2 = this.formatter.get_CurrentPosition() - 1;
			this.currentWritingInfo.get_MemberDeclarationToCodePostionMap().set_Item(method, new OffsetSpan(V_1, V_2));
			return;
		}

		private void WriteConstructorNameAndGenericArguments(ObjectCreationExpression node, bool writeGenericArguments = true, int startArgumentIndex = 0)
		{
			V_0 = this.GetTypeName(node.get_Constructor().get_DeclaringType());
			this.WriteReference(V_0, node.get_Constructor());
			if (writeGenericArguments && node.get_ExpressionType().get_IsGenericInstance())
			{
				this.WriteToken(this.get_GenericLeftBracket());
				V_1 = (node.get_ExpressionType() as GenericInstanceType).get_GenericArguments();
				V_2 = startArgumentIndex;
				while (V_2 < V_1.get_Count())
				{
					if (V_2 > startArgumentIndex)
					{
						this.WriteToken(",");
						this.WriteSpace();
					}
					this.WriteReferenceAndNamespaceIfInCollision(V_1.get_Item(V_2));
					V_2 = V_2 + 1;
				}
				this.WriteToken(this.get_GenericRightBracket());
			}
			return;
		}

		protected override void WriteDelegate(TypeDefinition delegateDefinition)
		{
			this.WriteTypeVisiblity(delegateDefinition);
			this.WriteSpace();
			this.WriteKeyword(this.get_KeyWordWriter().get_Delegate());
			this.WriteSpace();
			V_0 = delegateDefinition.get_Methods().get_Item(0);
			V_1 = 1;
			while (V_1 < delegateDefinition.get_Methods().get_Count() && String.op_Inequality(V_0.get_Name(), "Invoke"))
			{
				V_0 = delegateDefinition.get_Methods().get_Item(V_1);
				V_1 = V_1 + 1;
			}
			if (this.get_KeyWordWriter().get_Sub() == null || this.get_KeyWordWriter().get_Function() == null)
			{
				this.WriteMethodReturnType(V_0);
				this.WriteSpace();
			}
			else
			{
				this.WriteKeyword(this.GetMethodKeyWord(V_0));
				this.WriteSpace();
			}
			this.WriteGenericName(delegateDefinition);
			this.WriteToken("(");
			this.WriteParameters(V_0);
			this.WriteToken(")");
			this.PostWriteMethodReturnType(V_0);
			if (delegateDefinition.get_HasGenericParameters())
			{
				this.PostWriteGenericParametersConstraints(delegateDefinition);
			}
			this.WriteEndOfStatement();
			return;
		}

		protected void WriteDelegateArgument(ObjectCreationExpression node)
		{
			if (node.get_Arguments().get_Item(0).get_CodeNodeType() == 22 && (node.get_Arguments().get_Item(0) as LiteralExpression).get_Value() == null || node.get_Arguments().get_Item(1) as MethodReferenceExpression == null || (node.get_Arguments().get_Item(1) as MethodReferenceExpression).get_Target() != null)
			{
				this.Write(node.get_Arguments().get_Item(1));
				return;
			}
			this.Write(node.get_Arguments().get_Item(0));
			this.WriteToken(".");
			this.WriteMethodReference(node.get_Arguments().get_Item(1) as MethodReferenceExpression);
			return;
		}

		protected virtual void WriteDelegateCreation(ObjectCreationExpression node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_New());
			this.WriteSpace();
			this.WriteReferenceAndNamespaceIfInCollision(node.get_ExpressionType());
			this.WriteToken("(");
			this.WriteDelegateArgument(node);
			this.WriteToken(")");
			return;
		}

		protected abstract void WriteDestructor(MethodDefinition method);

		private void WriteDoubleConstantValue(IMemberDefinition currentMember, double value, string fieldName)
		{
			V_0 = new BaseImperativeLanguageWriter.u003cu003ec__DisplayClass123_0();
			V_0.fieldName = fieldName;
			V_1 = null;
			if (currentMember != null)
			{
				if (currentMember as TypeDefinition == null)
				{
					V_1 = currentMember.get_DeclaringType().get_Module().get_TypeSystem().get_Double().Resolve();
				}
				else
				{
					V_1 = (currentMember as TypeDefinition).get_Module().get_TypeSystem().get_Double().Resolve();
				}
			}
			if (String.op_Equality(currentMember.get_FullName(), V_0.fieldName))
			{
				if (Double.IsInfinity(value) || Double.IsNaN(value))
				{
					this.WriteSpecialDoubleConstants(value, currentMember);
					return;
				}
				this.WriteLiteral(value.ToString("R", CultureInfo.get_InvariantCulture()));
				return;
			}
			V_2 = null;
			if (V_1 != null)
			{
				V_2 = V_1.get_Fields().First<FieldDefinition>(new Func<FieldDefinition, bool>(V_0.u003cWriteDoubleConstantValueu003eb__0));
				this.WriteReferenceAndNamespaceIfInCollision(V_1);
				this.WriteToken(".");
			}
			if (V_2 != null)
			{
				this.WriteReference(V_2.get_Name(), V_2);
				return;
			}
			this.WriteToken(V_0.fieldName.Substring(V_0.fieldName.IndexOf("::") + 2));
			return;
		}

		private void WriteDoubleInfinity(IMemberDefinition infinityMember, double value)
		{
			stackVariable2 = infinityMember.get_DeclaringType().get_Fields();
			stackVariable3 = BaseImperativeLanguageWriter.u003cu003ec.u003cu003e9__126_0;
			if (stackVariable3 == null)
			{
				dummyVar0 = stackVariable3;
				stackVariable3 = new Func<FieldDefinition, bool>(BaseImperativeLanguageWriter.u003cu003ec.u003cu003e9.u003cWriteDoubleInfinityu003eb__126_0);
				BaseImperativeLanguageWriter.u003cu003ec.u003cu003e9__126_0 = stackVariable3;
			}
			V_0 = stackVariable2.First<FieldDefinition>(stackVariable3);
			if (!Double.IsPositiveInfinity(value))
			{
				this.WriteLiteral("-1");
			}
			else
			{
				this.WriteLiteral("1");
			}
			this.WriteSpace();
			this.Write("/");
			this.WriteSpace();
			this.WriteReference("Epsilon", V_0);
			return;
		}

		private void WriteDoubleLiteral(object value)
		{
			V_0 = (Double)value;
			V_1 = null;
			if (this.membersStack.get_Count() > 0)
			{
				V_1 = this.membersStack.Peek();
			}
			if (Double.IsPositiveInfinity(V_0))
			{
				this.WriteDoubleConstantValue(V_1, V_0, "System.Double System.Double::PositiveInfinity");
				return;
			}
			if (Double.IsNegativeInfinity(V_0))
			{
				this.WriteDoubleConstantValue(V_1, V_0, "System.Double System.Double::NegativeInfinity");
				return;
			}
			if (Double.IsNaN(V_0))
			{
				this.WriteDoubleConstantValue(V_1, V_0, "System.Double System.Double::NaN");
				return;
			}
			if (MaxValue == V_0)
			{
				this.WriteDoubleConstantValue(V_1, V_0, "System.Double System.Double::MaxValue");
				return;
			}
			if (MinValue == V_0)
			{
				this.WriteDoubleConstantValue(V_1, V_0, "System.Double System.Double::MinValue");
				return;
			}
			this.WriteLiteral(V_0.ToString(CultureInfo.get_InvariantCulture()));
			return;
		}

		private void WriteDoubleNan(IMemberDefinition nanMember)
		{
			stackVariable2 = nanMember.get_DeclaringType().get_Fields();
			stackVariable3 = BaseImperativeLanguageWriter.u003cu003ec.u003cu003e9__125_0;
			if (stackVariable3 == null)
			{
				dummyVar0 = stackVariable3;
				stackVariable3 = new Func<FieldDefinition, bool>(BaseImperativeLanguageWriter.u003cu003ec.u003cu003e9.u003cWriteDoubleNanu003eb__125_0);
				BaseImperativeLanguageWriter.u003cu003ec.u003cu003e9__125_0 = stackVariable3;
			}
			V_0 = stackVariable2.First<FieldDefinition>(stackVariable3);
			this.WriteReference("PositiveInfinity", V_0);
			this.WriteSpace();
			this.Write("/");
			this.WriteSpace();
			this.WriteReference("PositiveInfinity", V_0);
			return;
		}

		protected virtual void WriteEmptyMethodEndOfStatement(MethodDefinition method)
		{
			this.WriteEndOfStatement();
			return;
		}

		protected virtual void WriteEnumBaseTypeInheritColon()
		{
			this.WriteBaseTypeInheritColon();
			return;
		}

		protected virtual void WriteEscapeCharLiteral(char p)
		{
			return;
		}

		protected virtual void WriteEventAddOnParameters(EventDefinition event)
		{
			return;
		}

		protected override void WriteEventDeclaration(EventDefinition event)
		{
			this.WriteEventDeclaration(event, true);
			return;
		}

		private void WriteEventDeclaration(EventDefinition event, bool isAutoGenerated)
		{
			V_0 = event.get_AddMethod().GetMoreVisibleMethod(event.get_RemoveMethod());
			this.WriteMethodVisibilityAndSpace(V_0);
			if (this.TypeSupportsExplicitStaticMembers(V_0.get_DeclaringType()) && V_0.get_IsStatic())
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_Static());
				this.WriteSpace();
			}
			if (!isAutoGenerated && this.get_KeyWordWriter().get_Custom() != null)
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_Custom());
				this.WriteSpace();
			}
			if (event.IsVirtual() && !event.get_DeclaringType().get_IsInterface() && this.WriteEventKeywords(event))
			{
				this.WriteSpace();
			}
			this.WriteKeyword(this.get_KeyWordWriter().get_Event());
			this.WriteSpace();
			this.WriteEventTypeAndName(event);
			this.WriteEventInterfaceImplementations(event);
			return;
		}

		protected virtual void WriteEventInterfaceImplementations(EventDefinition event)
		{
			return;
		}

		private bool WriteEventKeywords(EventDefinition event)
		{
			if (!event.IsNewSlot())
			{
				if (event.IsFinal())
				{
					this.WriteKeyword(this.get_KeyWordWriter().get_SealedMethod());
					this.WriteSpace();
				}
				this.WriteKeyword(this.get_KeyWordWriter().get_Override());
				return true;
			}
			if (event.IsAbstract())
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_AbstractMember());
				return true;
			}
			if (event.IsFinal())
			{
				return false;
			}
			this.WriteKeyword(this.get_KeyWordWriter().get_Virtual());
			return true;
		}

		protected void WriteEventMethods(EventDefinition eventDef)
		{
			if (eventDef.get_AddMethod() != null)
			{
				this.WriteAddOn(eventDef);
			}
			if (eventDef.get_RemoveMethod() != null)
			{
				if (eventDef.get_AddMethod() != null)
				{
					this.WriteLine();
				}
				this.WriteRemoveOn(eventDef);
			}
			dummyVar0 = eventDef.get_InvokeMethod();
			return;
		}

		protected virtual void WriteEventRemoveOnParameters(EventDefinition event)
		{
			return;
		}

		protected virtual void WriteEventTypeAndName(EventDefinition event)
		{
			this.WriteTypeAndName(event.get_EventType(), event.get_Name(), event);
			return;
		}

		private void WriteExternAndSpaceIfNecessary(MethodDefinition method)
		{
			if (method.IsExtern() && this.get_KeyWordWriter().get_Extern() != null)
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_Extern());
				this.WriteSpace();
			}
			return;
		}

		protected virtual void WriteFieldDeclaration(FieldDefinition field)
		{
			if (!this.get_TypeContext().get_BackingFieldToNameMap().ContainsKey(field) && this.get_ModuleContext().get_RenamedMembers().Contains(field.get_MetadataToken().ToUInt32()))
			{
				this.WriteComment(this.GetOriginalFieldName(field));
				this.WriteLine();
			}
			this.WriteMemberVisibility(field);
			this.WriteSpace();
			if (field.get_IsInitOnly())
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_ReadOnly());
				this.WriteSpace();
			}
			if (!field.get_IsLiteral())
			{
				if (this.TypeSupportsExplicitStaticMembers(field.get_DeclaringType()) && field.get_IsStatic())
				{
					this.WriteKeyword(this.get_KeyWordWriter().get_Static());
					this.WriteSpace();
				}
			}
			else
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_Const());
				this.WriteSpace();
			}
			this.WriteFieldTypeAndName(field);
			if (field.get_HasConstant())
			{
				this.WriteSpace();
				this.WriteToken("=");
				this.WriteSpace();
				V_2 = this.formatter.get_CurrentPosition();
				V_3 = field.get_FieldType().Resolve();
				if (V_3 == null || !V_3.get_IsEnum())
				{
					this.WriteLiteralInLanguageSyntax(field.get_Constant().get_Value());
				}
				else
				{
					V_4 = new LiteralExpression(field.get_Constant().get_Value(), field.get_DeclaringType().get_Module().get_TypeSystem(), null);
					V_5 = EnumHelper.GetEnumExpression(V_3, V_4, field.get_DeclaringType().get_Module().get_TypeSystem());
					this.Write(V_5);
				}
				this.currentWritingInfo.get_CodeMappingInfo().Add(field, new OffsetSpan(V_2, this.formatter.get_CurrentPosition()));
			}
			return;
		}

		protected virtual void WriteFieldName(FieldReference field)
		{
			this.WriteReference(this.GetFieldName(field), field);
			return;
		}

		protected virtual void WriteFieldTypeAndName(FieldDefinition field)
		{
			V_0 = this.GetFieldName(field);
			this.WriteTypeAndName(field.get_FieldType(), V_0, field);
			return;
		}

		protected virtual void WriteFire(EventDefinition event)
		{
			V_6 = event.get_InvokeMethod().get_MetadataToken();
			V_0 = V_6.ToUInt32();
			V_1 = this.formatter.get_CurrentPosition();
			stackVariable9 = this.get_AttributeWriter();
			stackVariable11 = event.get_InvokeMethod();
			stackVariable13 = new String[1];
			stackVariable13[0] = "System.Runtime.CompilerServices.CompilerGeneratedAttribute";
			stackVariable9.WriteMemberAttributesAndNewLine(stackVariable11, stackVariable13, false);
			this.AddMemberAttributesOffsetSpan(V_0, V_1, this.formatter.get_CurrentPosition());
			V_2 = this.formatter.get_CurrentPosition();
			this.WriteMethodVisibilityAndSpace(event.get_InvokeMethod());
			V_3 = this.formatter.get_CurrentPosition();
			this.WriteKeyword(this.get_KeyWordWriter().get_Fire());
			V_4 = this.formatter.get_CurrentPosition();
			this.currentWritingInfo.get_MemberDeclarationToCodePostionMap().set_Item(event.get_InvokeMethod(), new OffsetSpan(V_3, V_4));
			this.WriteToken("(");
			this.WriteParameters(event.get_InvokeMethod());
			this.WriteToken(")");
			this.WriteLine();
			this.Write(this.GetStatement(event.get_InvokeMethod()));
			this.WriteSpecialEndBlock(this.get_KeyWordWriter().get_Fire());
			this.currentWritingInfo.get_MemberTokenToDecompiledCodeMap().Add(V_0, new OffsetSpan(V_2, this.formatter.get_CurrentPosition() - 1));
			return;
		}

		private void WriteFloatInfinity(IMemberDefinition infinityMember, double value)
		{
			stackVariable2 = infinityMember.get_DeclaringType().get_Fields();
			stackVariable3 = BaseImperativeLanguageWriter.u003cu003ec.u003cu003e9__137_0;
			if (stackVariable3 == null)
			{
				dummyVar0 = stackVariable3;
				stackVariable3 = new Func<FieldDefinition, bool>(BaseImperativeLanguageWriter.u003cu003ec.u003cu003e9.u003cWriteFloatInfinityu003eb__137_0);
				BaseImperativeLanguageWriter.u003cu003ec.u003cu003e9__137_0 = stackVariable3;
			}
			V_0 = stackVariable2.First<FieldDefinition>(stackVariable3);
			if (!Double.IsPositiveInfinity(value))
			{
				this.WriteLiteral("-1");
			}
			else
			{
				this.WriteLiteral("1");
			}
			this.WriteSpace();
			this.Write("/");
			this.WriteSpace();
			this.WriteReference("Epsilon", V_0);
			return;
		}

		private void WriteFloatInfinityValue(IMemberDefinition currentMember, float value, string fieldName)
		{
			V_0 = new BaseImperativeLanguageWriter.u003cu003ec__DisplayClass134_0();
			V_0.fieldName = fieldName;
			V_1 = null;
			if (currentMember != null)
			{
				if (currentMember as TypeDefinition == null)
				{
					V_1 = currentMember.get_DeclaringType().get_Module().get_TypeSystem().get_Single().Resolve();
				}
				else
				{
					V_1 = (currentMember as TypeDefinition).get_Module().get_TypeSystem().get_Single().Resolve();
				}
			}
			if (String.op_Equality(currentMember.get_FullName(), V_0.fieldName))
			{
				if (Single.IsInfinity(value) || Single.IsNaN(value))
				{
					this.WriteSpecialFloatValue(value, currentMember);
					return;
				}
				this.WriteLiteral(String.Concat(value.ToString("R", CultureInfo.get_InvariantCulture()), this.get_Language().get_FloatingLiteralsConstant()));
				return;
			}
			V_2 = null;
			if (V_1 != null)
			{
				V_2 = V_1.get_Fields().First<FieldDefinition>(new Func<FieldDefinition, bool>(V_0.u003cWriteFloatInfinityValueu003eb__0));
				this.WriteReferenceAndNamespaceIfInCollision(V_1);
				this.WriteToken(".");
			}
			if (V_2 != null)
			{
				this.WriteReference(V_2.get_Name(), V_2);
				return;
			}
			this.WriteToken(V_0.fieldName.Substring(V_0.fieldName.IndexOf("::") + 2));
			return;
		}

		private void WriteFloatLiteral(object value)
		{
			V_0 = (Single)value;
			V_1 = null;
			if (this.membersStack.get_Count() > 0)
			{
				V_1 = this.membersStack.Peek();
			}
			if (Single.IsPositiveInfinity(V_0))
			{
				this.WriteFloatInfinityValue(V_1, V_0, "System.Single System.Single::PositiveInfinity");
				return;
			}
			if (Single.IsNegativeInfinity(V_0))
			{
				this.WriteFloatInfinityValue(V_1, V_0, "System.Single System.Single::NegativeInfinity");
				return;
			}
			if (MinValue == V_0)
			{
				this.WriteFloatInfinityValue(V_1, V_0, "System.Single System.Single::MinValue");
				return;
			}
			if (MaxValue == V_0)
			{
				this.WriteFloatInfinityValue(V_1, V_0, "System.Single System.Single::MaxValue");
				return;
			}
			if (Single.IsNaN(V_0))
			{
				this.WriteFloatInfinityValue(V_1, V_0, "System.Single System.Single::NaN");
				return;
			}
			this.WriteLiteral(String.Concat(V_0.ToString("R", CultureInfo.get_InvariantCulture()), this.get_Language().get_FloatingLiteralsConstant()));
			return;
		}

		private void WriteFloatNan(IMemberDefinition nanMember)
		{
			stackVariable2 = nanMember.get_DeclaringType().get_Fields();
			stackVariable3 = BaseImperativeLanguageWriter.u003cu003ec.u003cu003e9__136_0;
			if (stackVariable3 == null)
			{
				dummyVar0 = stackVariable3;
				stackVariable3 = new Func<FieldDefinition, bool>(BaseImperativeLanguageWriter.u003cu003ec.u003cu003e9.u003cWriteFloatNanu003eb__136_0);
				BaseImperativeLanguageWriter.u003cu003ec.u003cu003e9__136_0 = stackVariable3;
			}
			V_0 = stackVariable2.First<FieldDefinition>(stackVariable3);
			this.WriteReference("PositiveInfinity", V_0);
			this.WriteSpace();
			this.Write("/");
			this.WriteSpace();
			this.WriteReference("PositiveInfinity", V_0);
			return;
		}

		protected void WriteGenericInstance(GenericInstanceType genericInstance, int startingArgument = 0)
		{
			this.WriteReference(this.GetTypeName(genericInstance), genericInstance.get_ElementType());
			this.WriteToken(this.get_GenericLeftBracket());
			V_1 = startingArgument;
			while (V_1 < genericInstance.get_GenericArguments().get_Count())
			{
				if (V_1 > startingArgument)
				{
					this.WriteToken(",");
					this.WriteSpace();
				}
				V_2 = genericInstance.get_GenericArguments().get_Item(V_1);
				if (genericInstance.get_PostionToArgument().ContainsKey(V_1))
				{
					V_2 = genericInstance.get_PostionToArgument().get_Item(V_1);
				}
				this.WriteReferenceAndNamespaceIfInCollision(V_2);
				V_1 = V_1 + 1;
			}
			this.WriteToken(this.get_GenericRightBracket());
			return;
		}

		protected virtual void WriteGenericInstanceMethod(GenericInstanceMethod genericMethod)
		{
			this.WriteGenericInstanceMethodWithArguments(genericMethod, genericMethod.get_GenericArguments());
			return;
		}

		protected void WriteGenericInstanceMethodWithArguments(GenericInstanceMethod genericMethod, Collection<TypeReference> genericArguments)
		{
			V_0 = genericMethod.get_ElementMethod();
			this.WriteReference(this.GetMethodName(V_0), V_0);
			if (genericMethod.HasAnonymousArgument())
			{
				return;
			}
			if (genericArguments.get_Count() == 0)
			{
				return;
			}
			this.WriteToken(this.get_GenericLeftBracket());
			V_2 = 0;
			while (V_2 < genericArguments.get_Count())
			{
				if (V_2 > 0)
				{
					this.WriteToken(",");
					this.WriteSpace();
				}
				this.WriteReferenceAndNamespaceIfInCollision(genericArguments.get_Item(V_2));
				V_2 = V_2 + 1;
			}
			this.WriteToken(this.get_GenericRightBracket());
			return;
		}

		private void WriteGenericInstanceTypeArguments(IGenericInstance genericInstance)
		{
			this.WriteToken(this.get_GenericLeftBracket());
			V_0 = 0;
			while (V_0 < genericInstance.get_GenericArguments().get_Count())
			{
				if (V_0 > 0)
				{
					this.WriteToken(",");
					this.WriteSpace();
				}
				V_1 = genericInstance.get_GenericArguments().get_Item(V_0);
				if (genericInstance.get_PostionToArgument().ContainsKey(V_0))
				{
					V_1 = genericInstance.get_PostionToArgument().get_Item(V_0);
				}
				this.WriteTypeReferenceNavigationName(V_1);
				V_0 = V_0 + 1;
			}
			this.WriteToken(this.get_GenericRightBracket());
			return;
		}

		protected virtual void WriteGenericName(IGenericDefinition genericDefinition)
		{
			if (genericDefinition as MethodDefinition == null)
			{
				V_1 = this.GetTypeName(genericDefinition as TypeDefinition);
				V_0 = genericDefinition as TypeDefinition;
			}
			else
			{
				V_1 = this.GetMethodName(genericDefinition as MethodDefinition);
				V_0 = genericDefinition as MethodDefinition;
			}
			this.WriteReference(V_1, genericDefinition);
			stackVariable13 = this;
			this.WriteGenericParametersToDefinition(V_0, new Action<GenericParameter>(stackVariable13.WriteGenericParameterConstraints), true);
			return;
		}

		protected virtual void WriteGenericParameterConstraints(GenericParameter parameter)
		{
			return;
		}

		private void WriteGenericParametersToDefinition(IGenericParameterProvider genericDefinition, Action<GenericParameter> writeParamConstraints, bool renameInvalidCharacters)
		{
			V_0 = 0;
			if (genericDefinition as TypeDefinition != null)
			{
				V_1 = (genericDefinition as TypeDefinition).get_DeclaringType();
				if (V_1 != null && V_1.get_HasGenericParameters())
				{
					V_0 = V_1.get_GenericParameters().get_Count();
				}
			}
			if (V_0 < genericDefinition.get_GenericParameters().get_Count())
			{
				this.WriteToken(this.get_GenericLeftBracket());
				while (V_0 < genericDefinition.get_GenericParameters().get_Count())
				{
					V_2 = genericDefinition.get_GenericParameters().get_Item(V_0);
					if (V_2.get_IsCovariant())
					{
						this.WriteKeyword(this.get_KeyWordWriter().get_Covariant());
						this.WriteSpace();
					}
					if (V_2.get_IsContravariant())
					{
						this.WriteKeyword(this.get_KeyWordWriter().get_Contravariant());
						this.WriteSpace();
					}
					if (renameInvalidCharacters)
					{
						stackVariable28 = GenericHelper.ReplaceInvalidCharactersName(this.get_Language(), V_2.get_Name());
					}
					else
					{
						stackVariable28 = V_2.get_Name();
					}
					this.WriteReference(stackVariable28, null);
					if (V_2.get_HasConstraints() || V_2.get_HasDefaultConstructorConstraint() || V_2.get_HasReferenceTypeConstraint() || V_2.get_HasNotNullableValueTypeConstraint() && writeParamConstraints != null)
					{
						writeParamConstraints.Invoke(V_2);
					}
					if (V_0 != genericDefinition.get_GenericParameters().get_Count() - 1)
					{
						this.WriteToken(",");
						this.WriteSpace();
					}
					V_0 = V_0 + 1;
				}
				this.WriteToken(this.get_GenericRightBracket());
			}
			return;
		}

		public virtual void WriteGenericReference(TypeReference type)
		{
			if (type.get_IsNested() && !type.get_IsGenericParameter())
			{
				this.WriteGenericReference(type.get_DeclaringType());
				this.WriteToken(".");
			}
			if (type as TypeSpecification != null)
			{
				this.WriteTypeSpecification(type as TypeSpecification, 0);
				return;
			}
			if (!type.get_IsNested())
			{
				this.WriteNamespaceIfTypeInCollision(type);
			}
			if (type.get_IsGenericParameter())
			{
				this.WriteReference(this.ToEscapedTypeString(type), null);
				return;
			}
			V_0 = type.Resolve();
			if (V_0 == null || !V_0.get_HasGenericParameters())
			{
				if (String.op_Inequality(type.get_Namespace(), "System"))
				{
					this.WriteReference(this.GetTypeName(type), type);
					return;
				}
				this.WriteReference(this.ToEscapedTypeString(type), type);
				return;
			}
			this.WriteReference(this.GetTypeName(V_0), V_0);
			this.WriteToken(this.get_GenericLeftBracket());
			V_2 = 1;
			while (V_2 < V_0.get_GenericParameters().get_Count())
			{
				this.WriteToken(",");
				V_2 = V_2 + 1;
			}
			this.WriteToken(this.get_GenericRightBracket());
			return;
		}

		protected void WriteGetMethod(PropertyDefinition property, bool isAutoImplemented)
		{
			V_6 = property.get_GetMethod().get_MetadataToken();
			V_0 = V_6.ToUInt32();
			this.membersStack.Push(property.get_GetMethod());
			V_1 = this.formatter.get_CurrentPosition();
			stackVariable13 = this.get_AttributeWriter();
			stackVariable15 = property.get_GetMethod();
			stackVariable17 = new String[1];
			stackVariable17[0] = "System.Runtime.CompilerServices.CompilerGeneratedAttribute";
			stackVariable13.WriteMemberAttributesAndNewLine(stackVariable15, stackVariable17, false);
			this.AddMemberAttributesOffsetSpan(V_0, V_1, this.formatter.get_CurrentPosition());
			V_2 = this.formatter.get_CurrentPosition();
			V_3 = this.formatter.get_CurrentPosition();
			this.WriteMoreRestrictiveMethodVisibility(property.get_GetMethod(), property.get_SetMethod());
			V_4 = this.formatter.get_CurrentPosition();
			this.WriteKeyword(this.get_KeyWordWriter().get_Get());
			V_5 = this.formatter.get_CurrentPosition() - 1;
			this.currentWritingInfo.get_MemberDeclarationToCodePostionMap().set_Item(property.get_GetMethod(), new OffsetSpan(V_4, V_5));
			if (property.get_GetMethod().get_Body() == null || this.get_SupportsAutoProperties() & isAutoImplemented)
			{
				this.WriteEndOfStatement();
			}
			else
			{
				this.WriteLine();
				this.Write(this.GetStatement(property.get_GetMethod()));
			}
			this.WriteSpecialEndBlock(this.get_KeyWordWriter().get_Get());
			this.currentWritingInfo.get_MemberDefinitionToFoldingPositionMap().Add(property.get_GetMethod(), new OffsetSpan(V_2, this.formatter.get_CurrentPosition() - 1));
			this.currentWritingInfo.get_MemberTokenToDecompiledCodeMap().Add(V_0, new OffsetSpan(V_3, this.formatter.get_CurrentPosition() - 1));
			dummyVar0 = this.membersStack.Pop();
			return;
		}

		protected void WriteIndexerArguments(PropertyReferenceExpression node)
		{
			this.WriteToken(this.get_IndexLeftBracket());
			V_0 = 0;
			while (V_0 < node.get_Arguments().get_Count())
			{
				V_1 = node.get_Arguments().get_Item(V_0);
				if (V_0 > 0)
				{
					this.WriteToken(",");
					this.WriteSpace();
				}
				this.Write(V_1);
				V_0 = V_0 + 1;
			}
			this.WriteToken(this.get_IndexRightBracket());
			return;
		}

		protected abstract void WriteIndexerKeywords();

		protected void WriteInheritComma()
		{
			this.WriteToken(",");
			this.WriteSpace();
			return;
		}

		private void WriteInitializedAutoProperty(PropertyDefinition property, Expression assignment)
		{
			this.WritePropertyDeclaration(property);
			this.WriteBeginBlock(true);
			this.WriteSpace();
			this.WritePropertyMethods(property, true);
			this.WriteSpace();
			this.WriteEndBlock(property.get_Name());
			this.WriteSpace();
			this.WriteToken("=");
			this.WriteSpace();
			this.Visit(assignment);
			this.WriteEndOfStatement();
			return;
		}

		protected virtual void WriteInterfacesInheritColon(TypeDefinition type)
		{
			return;
		}

		protected void WriteLabel(string label)
		{
			if (String.op_Inequality(label, ""))
			{
				this.Outdent();
				this.Write(label);
				this.WriteToken(":");
				this.Indent();
			}
			return;
		}

		public void WriteLiteralInLanguageSyntax(object value)
		{
			if (value == null)
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_Null());
				return;
			}
			switch (Type.GetTypeCode(value.GetType()) - 3)
			{
				case 0:
				{
					if ((Boolean)value)
					{
						stackVariable12 = this.get_KeyWordWriter().get_True();
					}
					else
					{
						stackVariable12 = this.get_KeyWordWriter().get_False();
					}
					this.WriteKeyword(stackVariable12);
					return;
				}
				case 1:
				{
					this.WriteEscapeCharLiteral((Char)value);
					return;
				}
				case 2:
				case 3:
				case 12:
				case 13:
				case 14:
				{
				Label0:
					this.WriteLiteral(value.ToString());
					return;
				}
				case 4:
				{
					V_0 = (Int16)value;
					if (V_0 < 0xff || !this.get_Settings().get_WriteLargeNumbersInHex())
					{
						this.WriteLiteral(V_0.ToString());
						return;
					}
					this.WriteLiteral(String.Concat(this.get_HexValuePrefix(), V_0.ToString("X").ToLowerInvariant()));
					return;
				}
				case 5:
				{
					V_2 = (UInt16)value;
					if (V_2 < 0xff || !this.get_Settings().get_WriteLargeNumbersInHex())
					{
						this.WriteLiteral(V_2.ToString());
						return;
					}
					this.WriteLiteral(String.Concat(this.get_HexValuePrefix(), V_2.ToString("X").ToLowerInvariant()));
					return;
				}
				case 6:
				{
					V_1 = (Int32)value;
					if (V_1 < 0xff || !this.get_Settings().get_WriteLargeNumbersInHex())
					{
						this.WriteLiteral(V_1.ToString());
						return;
					}
					this.WriteLiteral(String.Concat(this.get_HexValuePrefix(), V_1.ToString("X").ToLowerInvariant()));
					return;
				}
				case 7:
				{
					V_3 = (UInt32)value;
					if (V_3 < 0xff || !this.get_Settings().get_WriteLargeNumbersInHex())
					{
						this.WriteLiteral(V_3.ToString());
						return;
					}
					this.WriteLiteral(String.Concat(this.get_HexValuePrefix(), V_3.ToString("X").ToLowerInvariant()));
					return;
				}
				case 8:
				{
					V_4 = (Int64)value;
					if (V_4 < (long)0xff || !this.get_Settings().get_WriteLargeNumbersInHex())
					{
						this.WriteLiteral(String.Concat(V_4.ToString(), "L"));
						return;
					}
					this.WriteLiteral(String.Concat(this.get_HexValuePrefix(), V_4.ToString("X").ToLowerInvariant(), "L"));
					return;
				}
				case 9:
				{
					V_5 = (UInt64)value;
					if (V_5 < (long)0xff || !this.get_Settings().get_WriteLargeNumbersInHex())
					{
						this.WriteLiteral(String.Concat(V_5.ToString(), "L"));
						return;
					}
					this.WriteLiteral(String.Concat(this.get_HexValuePrefix(), V_5.ToString("X").ToLowerInvariant(), "L"));
					return;
				}
				case 10:
				{
					this.WriteFloatLiteral(value);
					return;
				}
				case 11:
				{
					this.WriteDoubleLiteral(value);
					return;
				}
				case 15:
				{
					this.WriteLiteral("\"");
					this.WriteEscapeLiteral(value.ToString());
					this.WriteLiteral("\"");
					return;
				}
				default:
				{
					goto Label0;
				}
			}
		}

		protected string WriteLogicalToken(BinaryOperator logical)
		{
			if (logical == 11)
			{
				return this.ToString(11, false);
			}
			if (logical != 12)
			{
				return String.Empty;
			}
			return this.ToString(12, false);
		}

		public override void WriteMemberNavigationName(object memberDefinition)
		{
			if (memberDefinition as MethodReference != null)
			{
				this.WriteMethodReferenceNavigationName(memberDefinition as MethodReference);
				return;
			}
			if (memberDefinition as TypeReference != null)
			{
				this.WriteTypeReferenceNavigationName(memberDefinition as TypeReference);
				return;
			}
			if (memberDefinition as IMemberDefinition != null)
			{
				V_0 = memberDefinition as IMemberDefinition;
				V_1 = GenericHelper.GetNonGenericName(V_0.get_Name());
				if (!Utilities.IsExplicitInterfaceImplementataion(V_0))
				{
					if (this.get_Settings().get_RenameInvalidMembers())
					{
						V_1 = this.get_Language().ReplaceInvalidCharactersInIdentifier(V_1);
					}
				}
				else
				{
					stackVariable33 = new Char[1];
					stackVariable33[0] = '.';
					V_3 = V_1.Split(stackVariable33);
					V_4 = new StringBuilder();
					V_5 = 0;
					while (V_5 < (int)V_3.Length)
					{
						if (V_5 > 0)
						{
							dummyVar0 = V_4.Append('.');
						}
						if (!this.get_Settings().get_RenameInvalidMembers())
						{
							dummyVar2 = V_4.Append(V_3[V_5]);
						}
						else
						{
							if (!this.NormalizeNameIfContainingGenericSymbols(V_3[V_5], V_4))
							{
								dummyVar1 = V_4.Append(this.get_Language().ReplaceInvalidCharactersInIdentifier(V_3[V_5]));
							}
						}
						V_5 = V_5 + 1;
					}
					V_1 = V_4.ToString();
				}
				this.formatter.Write(V_1);
				this.formatter.Write(" : ");
				this.WriteTypeReferenceNavigationName(this.GetMemberType(V_0));
			}
			return;
		}

		protected void WriteMemberVisibility(IVisibilityDefinition member)
		{
			if (member.get_IsPrivate())
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_Private());
				return;
			}
			if (member.get_IsPublic())
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_Public());
				return;
			}
			if (member.get_IsFamily())
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_Protected());
				return;
			}
			if (member.get_IsAssembly())
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_Internal());
				return;
			}
			if (member.get_IsFamilyOrAssembly() || member.get_IsFamilyAndAssembly())
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_Protected());
				this.WriteSpace();
				this.WriteKeyword(this.get_KeyWordWriter().get_Internal());
				return;
			}
			if (member as MethodDefinition == null || !(member as MethodDefinition).get_IsCompilerControlled() && member as FieldDefinition == null || !(member as FieldDefinition).get_IsCompilerControlled())
			{
				throw new NotSupportedException();
			}
			this.WriteComment("privatescope");
			this.WriteLine();
			this.WriteKeyword(this.get_KeyWordWriter().get_Internal());
			return;
		}

		protected void WriteMethod(MethodDefinition method)
		{
			this.membersStack.Push(method);
			V_0 = false;
			try
			{
				if (method.get_Body() == null || method.get_Body().get_Instructions().get_Count() <= 0 && !method.get_IsJustDecompileGenerated())
				{
					stackVariable6 = null;
				}
				else
				{
					stackVariable6 = this.GetStatement(method);
				}
				V_1 = stackVariable6;
				this.WriteMethodDeclaration(method, false);
				if (method.get_Body() != null)
				{
					if (method.get_Body().get_Instructions().get_Count() != 0 || method.get_IsJustDecompileGenerated())
					{
						if (this.get_MethodContext() != null && this.get_MethodContext().get_Method().get_IsConstructor() && this.get_MethodContext().get_CtorInvokeExpression() != null)
						{
							this.WriteBaseConstructorInvokation(this.get_MethodContext().get_CtorInvokeExpression());
						}
						V_2 = method.IsExtern();
						V_3 = 0;
						if (!method.get_IsAbstract() && !V_2)
						{
							V_3 = this.formatter.get_CurrentPosition();
							this.formatter.WriteStartBlock();
							V_0 = true;
						}
						this.WriteLine();
						this.Write(V_1);
						if (this.get_KeyWordWriter().get_Sub() != null && this.get_KeyWordWriter().get_Function() != null && this.get_KeyWordWriter().get_Operator() != null)
						{
							this.WriteSpecialEndBlock(this.GetMethodKeyWord(method));
						}
						if (!method.get_IsAbstract() && !V_2)
						{
							this.currentWritingInfo.get_MemberDefinitionToFoldingPositionMap().set_Item(method, new OffsetSpan(V_3, this.formatter.get_CurrentPosition() - 1));
							this.formatter.WriteEndBlock();
						}
					}
					else
					{
						this.WriteBeginBlock(false);
						this.WriteLine();
						this.WriteEndBlock(this.GetMethodName(method));
						dummyVar1 = this.membersStack.Pop();
						goto Label0;
					}
				}
				else
				{
					this.WriteEmptyMethodEndOfStatement(method);
					dummyVar0 = this.membersStack.Pop();
					goto Label0;
				}
			}
			catch (Exception exception_0)
			{
				dummyVar2 = exception_0;
				if (V_0)
				{
					this.formatter.WriteEndBlock();
				}
				dummyVar3 = this.membersStack.Pop();
				throw;
			}
			dummyVar4 = this.membersStack.Pop();
		Label0:
			return;
		}

		protected override void WriteMethodDeclaration(MethodDefinition method, bool writeDocumentation = false)
		{
			if (!method.get_IsConstructor() && !method.get_HasOverrides() && this.get_ModuleContext().get_RenamedMembers().Contains(method.get_MetadataToken().ToUInt32()))
			{
				this.WriteComment(GenericHelper.GetNonGenericName(this.GetMethodOriginalName(method)));
				this.WriteLine();
			}
			if (!method.get_DeclaringType().get_IsInterface())
			{
				if (!method.get_IsStatic() || !method.get_IsConstructor())
				{
					this.WriteMethodVisibilityAndSpace(method);
				}
				if (method.get_IsVirtual() && this.WriteMethodKeywords(method))
				{
					this.WriteSpace();
				}
				if (this.TypeSupportsExplicitStaticMembers(method.get_DeclaringType()) && method.get_IsStatic())
				{
					this.WriteKeyword(this.get_KeyWordWriter().get_Static());
					this.WriteSpace();
				}
				if (!method.get_IsConstructor() && !method.get_IsVirtual() || method.get_IsNewSlot() && this.IsMethodHiding(method))
				{
					this.WriteKeyword(this.get_KeyWordWriter().get_Hiding());
					this.WriteSpace();
				}
				this.WriteExternAndSpaceIfNecessary(method);
				if (method.get_IsUnsafe() && String.op_Inequality(this.get_KeyWordWriter().get_Unsafe(), String.Empty))
				{
					this.WriteKeyword(this.get_KeyWordWriter().get_Unsafe());
					this.WriteSpace();
				}
				if (this.get_KeyWordWriter().get_Async() != null && method.IsAsync())
				{
					this.WriteKeyword(this.get_KeyWordWriter().get_Async());
					this.WriteSpace();
				}
				if (method.get_IsConstructor())
				{
					this.WriteConstructorName(method);
				}
				else
				{
					if (!this.TryWriteMethodAsOperator(method))
					{
						this.WriteMethodName(method);
					}
				}
			}
			else
			{
				this.WriteMethodName(method);
			}
			this.WriteToken("(");
			this.WriteParameters(method);
			this.WriteToken(")");
			this.PostWriteMethodReturnType(method);
			if (method.get_HasGenericParameters())
			{
				this.PostWriteGenericParametersConstraints(method);
			}
			if (!method.get_IsGetter() && !method.get_IsSetter())
			{
				this.WriteMethodInterfaceImplementations(method);
			}
			return;
		}

		protected virtual void WriteMethodInterfaceImplementations(MethodDefinition method)
		{
			return;
		}

		private bool WriteMethodKeywords(MethodDefinition method)
		{
			if (method.get_IsNewSlot())
			{
				if (method.get_IsAbstract())
				{
					this.WriteKeyword(this.get_KeyWordWriter().get_AbstractMember());
					return true;
				}
				if (method.get_IsFinal() || method.get_DeclaringType().get_IsSealed())
				{
					return false;
				}
				this.WriteKeyword(this.get_KeyWordWriter().get_Virtual());
				return true;
			}
			if (method.get_IsFinal())
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_SealedMethod());
				this.WriteSpace();
			}
			this.WriteKeyword(this.get_KeyWordWriter().get_Override());
			if (method.get_IsAbstract())
			{
				this.WriteSpace();
				this.WriteKeyword(this.get_KeyWordWriter().get_AbstractMember());
			}
			return true;
		}

		private void WriteMethodName(MethodDefinition method)
		{
			if (this.get_KeyWordWriter().get_Sub() == null || this.get_KeyWordWriter().get_Function() == null)
			{
				this.WriteMethodReturnType(method);
				this.WriteSpace();
			}
			else
			{
				this.WriteKeyword(this.GetMethodKeyWord(method));
				this.WriteSpace();
			}
			V_0 = this.formatter.get_CurrentPosition();
			this.WriteGenericName(method);
			V_1 = this.formatter.get_CurrentPosition() - 1;
			this.currentWritingInfo.get_MemberDeclarationToCodePostionMap().set_Item(method, new OffsetSpan(V_0, V_1));
			return;
		}

		protected virtual void WriteMethodReference(MethodReferenceExpression methodReferenceExpression)
		{
			V_0 = methodReferenceExpression.get_Method();
			if (V_0 as GenericInstanceMethod != null)
			{
				this.WriteGenericInstanceMethod(V_0 as GenericInstanceMethod);
				return;
			}
			this.WriteReference(this.GetMethodName(V_0), V_0);
			return;
		}

		private void WriteMethodReferenceNavigationName(MethodReference method)
		{
			V_0 = method.get_Name();
			if (!method.get_IsConstructor())
			{
				stackVariable4 = false;
			}
			else
			{
				stackVariable4 = (object)method.get_DeclaringType() != (object)null;
			}
			V_1 = stackVariable4;
			if (V_1)
			{
				V_0 = method.get_DeclaringType().get_Name();
			}
			stackVariable7 = GenericHelper.GetNonGenericName(V_0);
			stackVariable9 = new Char[1];
			stackVariable9[0] = '.';
			V_2 = stackVariable7.Split(stackVariable9);
			V_3 = new StringBuilder();
			V_4 = 0;
			while (V_4 < (int)V_2.Length)
			{
				if (V_4 > 0)
				{
					dummyVar0 = V_3.Append('.');
				}
				if (!this.get_Settings().get_RenameInvalidMembers())
				{
					dummyVar2 = V_3.Append(V_2[V_4]);
				}
				else
				{
					if (!this.NormalizeNameIfContainingGenericSymbols(V_2[V_4], V_3))
					{
						dummyVar1 = V_3.Append(this.get_Language().ReplaceInvalidCharactersInIdentifier(V_2[V_4]));
					}
				}
				V_4 = V_4 + 1;
			}
			this.formatter.Write(V_3.ToString());
			if (method as GenericInstanceMethod == null)
			{
				if (method.get_HasGenericParameters())
				{
					this.WriteGenericParametersToDefinition(method, null, this.get_Settings().get_RenameInvalidMembers());
				}
			}
			else
			{
				this.WriteGenericInstanceTypeArguments((GenericInstanceMethod)method);
			}
			this.formatter.Write("(");
			if (method.get_HasParameters())
			{
				V_5 = 0;
				while (V_5 < method.get_Parameters().get_Count())
				{
					if (V_5 > 0)
					{
						this.formatter.Write(", ");
					}
					V_6 = method.get_Parameters().get_Item(V_5);
					this.WriteTypeReferenceNavigationName(V_6.get_ParameterType());
					V_5 = V_5 + 1;
				}
			}
			this.formatter.Write(")");
			if (!V_1)
			{
				this.formatter.Write(" : ");
				this.WriteTypeReferenceNavigationName(method.get_FixedReturnType());
			}
			return;
		}

		protected virtual void WriteMethodReturnType(MethodDefinition method)
		{
			this.WriteReferenceAndNamespaceIfInCollision(method.get_ReturnType());
			return;
		}

		protected void WriteMethodTarget(Expression expression)
		{
			stackVariable2 = this.IsComplexTarget(expression);
			if (stackVariable2)
			{
				this.WriteToken("(");
			}
			this.Visit(expression);
			if (stackVariable2)
			{
				this.WriteToken(")");
			}
			this.WriteToken(".");
			return;
		}

		protected virtual bool WriteMethodVisibility(MethodDefinition method)
		{
			if (method.get_DeclaringType().get_IsInterface())
			{
				return false;
			}
			this.WriteMemberVisibility(method);
			return true;
		}

		protected void WriteMethodVisibilityAndSpace(MethodDefinition currentMethod)
		{
			if (this.WriteMethodVisibility(currentMethod))
			{
				this.WriteSpace();
			}
			return;
		}

		protected void WriteMoreRestrictiveMethodVisibility(MethodDefinition currentMethod, MethodDefinition otherMethod)
		{
			if (otherMethod == null)
			{
				return;
			}
			if (currentMethod.get_IsPrivate() && !otherMethod.get_IsPrivate())
			{
				this.WriteMethodVisibilityAndSpace(currentMethod);
			}
			if (currentMethod.get_IsFamily() || currentMethod.get_IsAssembly() && otherMethod.get_IsPublic() || otherMethod.get_IsFamilyOrAssembly())
			{
				this.WriteMethodVisibilityAndSpace(currentMethod);
			}
			if (currentMethod.get_IsFamilyOrAssembly() && otherMethod.get_IsPublic())
			{
				this.WriteMethodVisibilityAndSpace(currentMethod);
			}
			return;
		}

		public virtual void WriteNamespaceIfTypeInCollision(TypeReference reference)
		{
			if (reference.get_IsPrimitive() && reference.get_MetadataType() != 24 && reference.get_MetadataType() != 25 || reference.get_MetadataType() == 14 || reference.get_MetadataType() == 28)
			{
				return;
			}
			V_0 = this.GetCollidingType(reference);
			while (V_0.get_DeclaringType() != null)
			{
				V_0 = this.GetCollidingType(V_0.get_DeclaringType());
			}
			if (this.IsTypeNameInCollision(this.GetCollidingTypeName(V_0)))
			{
				this.WriteNamespace(V_0, true);
			}
			return;
		}

		protected virtual void WriteOptional(ParameterDefinition parameter)
		{
			return;
		}

		protected virtual void WriteOutOrRefKeyWord(ParameterDefinition parameter)
		{
			if (parameter.IsOutParameter())
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_Out());
				return;
			}
			this.WriteKeyword(this.get_KeyWordWriter().get_ByRef());
			return;
		}

		protected virtual void WriteParameters(MethodDefinition method)
		{
			V_0 = 0;
			while (V_0 < method.get_Parameters().get_Count())
			{
				V_1 = method.get_Parameters().get_Item(V_0);
				if (V_0 > 0)
				{
					this.WriteToken(",");
					this.WriteSpace();
				}
				if (V_0 == 0 && this.get_KeyWordWriter().get_ExtensionThis() != null && method.get_IsExtensionMethod())
				{
					this.WriteKeyword(this.get_KeyWordWriter().get_ExtensionThis());
					this.WriteSpace();
				}
				stackVariable13 = this.get_AttributeWriter();
				stackVariable14 = V_1;
				if (!this.get_TypeContext().get_IsWinRTImplementation())
				{
					stackVariable18 = false;
				}
				else
				{
					stackVariable18 = (object)this.get_TypeContext().get_CurrentType() == (object)method.get_DeclaringType();
				}
				if (stackVariable13.WriteParameterAttributes(stackVariable14, stackVariable18) > 0)
				{
					this.WriteSpace();
				}
				if (V_1.get_IsOptional())
				{
					this.WriteOptional(V_1);
				}
				V_2 = V_1.get_ParameterType();
				if (V_2 as ByReferenceType == null)
				{
					if (this.get_KeyWordWriter().get_ByVal() != null)
					{
						this.WriteKeyword(this.get_KeyWordWriter().get_ByVal());
						this.WriteSpace();
					}
				}
				else
				{
					this.WriteOutOrRefKeyWord(V_1);
					this.WriteSpace();
					V_2 = (V_2 as TypeSpecification).get_ElementType();
				}
				this.WriteParameterTypeAndName(V_2, this.GetParameterName(V_1), V_1);
				if (V_1.get_IsOptional())
				{
					this.WriteSpace();
					this.Write("=");
					this.WriteSpace();
					if (!V_1.get_HasDefault())
					{
						this.Write(this.GetDefaultValueExpression(V_1));
					}
					else
					{
						this.WriteLiteralInLanguageSyntax(V_1.get_Constant().get_Value());
					}
				}
				V_0 = V_0 + 1;
			}
			return;
		}

		protected virtual void WriteParameterTypeAndName(TypeReference typeReference, string name, ParameterDefinition reference)
		{
			return;
		}

		protected abstract bool WritePropertyAsIndexer(PropertyDefinition property);

		protected override void WritePropertyDeclaration(PropertyDefinition property)
		{
			V_0 = property.get_GetMethod().GetMoreVisibleMethod(property.get_SetMethod());
			if (property.IsIndexer())
			{
				this.WriteIndexerKeywords();
			}
			if (this.get_ModuleContext().get_RenamedMembers().Contains(property.get_MetadataToken().ToUInt32()))
			{
				this.WriteComment(property.get_Name());
				this.WriteLine();
			}
			if (!property.IsExplicitImplementation())
			{
				this.WriteMethodVisibilityAndSpace(V_0);
			}
			if (!property.IsVirtual() || property.IsNewSlot())
			{
				V_2 = property.IsIndexer();
				if (V_2 && this.IsIndexerPropertyHiding(property) || !V_2 && this.IsPropertyHiding(property))
				{
					this.WriteKeyword(this.get_KeyWordWriter().get_Hiding());
					this.WriteSpace();
				}
			}
			if (property.IsVirtual() && !property.get_DeclaringType().get_IsInterface() && this.WritePropertyKeywords(property))
			{
				this.WriteSpace();
			}
			this.WriteReadOnlyWriteOnlyProperty(property);
			if (this.TypeSupportsExplicitStaticMembers(property.get_DeclaringType()) && property.IsStatic())
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_Static());
				this.WriteSpace();
			}
			if (this.get_KeyWordWriter().get_Property() != null)
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_Property());
				this.WriteSpace();
			}
			if (property.IsIndexer() && this.WritePropertyAsIndexer(property))
			{
				return;
			}
			this.WritePropertyTypeAndNameWithArguments(property);
			this.WritePropertyInterfaceImplementations(property);
			return;
		}

		protected virtual void WritePropertyInterfaceImplementations(PropertyDefinition property)
		{
			return;
		}

		private bool WritePropertyKeywords(PropertyDefinition property)
		{
			if (!property.IsNewSlot())
			{
				if (property.IsFinal())
				{
					this.WriteKeyword(this.get_KeyWordWriter().get_SealedMethod());
					this.WriteSpace();
				}
				this.WriteKeyword(this.get_KeyWordWriter().get_Override());
				return true;
			}
			if (property.IsAbstract())
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_AbstractMember());
				return true;
			}
			if (property.IsFinal() || property.get_DeclaringType().get_IsSealed())
			{
				return false;
			}
			this.WriteKeyword(this.get_KeyWordWriter().get_Virtual());
			return true;
		}

		protected void WritePropertyMethods(PropertyDefinition property, bool inline = false)
		{
			V_0 = this.get_TypeContext().get_AutoImplementedProperties().Contains(property);
			if (property.get_GetMethod() != null)
			{
				this.WriteGetMethod(property, V_0);
			}
			if (property.get_SetMethod() != null)
			{
				if (property.get_GetMethod() != null)
				{
					if (!inline)
					{
						this.WriteLine();
					}
					else
					{
						this.WriteSpace();
					}
				}
				this.WriteSetMethod(property, V_0);
			}
			return;
		}

		protected virtual void WritePropertyName(PropertyDefinition property)
		{
			this.WriteReference(this.GetPropertyName(property), property);
			return;
		}

		protected void WritePropertyParameters(PropertyDefinition property)
		{
			V_1 = 0;
			if (property.get_GetMethod() == null)
			{
				V_0 = property.get_SetMethod();
				V_1 = 1;
			}
			else
			{
				V_0 = property.get_GetMethod();
			}
			if (V_0 == null)
			{
				return;
			}
			V_2 = 0;
			while (V_2 < V_0.get_Parameters().get_Count() - V_1)
			{
				V_3 = V_0.get_Parameters().get_Item(V_2);
				if (V_2 > 0)
				{
					this.WriteToken(",");
					this.WriteSpace();
				}
				V_4 = V_3.get_ParameterType();
				if (V_4 as ByReferenceType == null)
				{
					if (this.get_KeyWordWriter().get_ByVal() != null)
					{
						this.WriteKeyword(this.get_KeyWordWriter().get_ByVal());
						this.WriteSpace();
					}
				}
				else
				{
					this.WriteOutOrRefKeyWord(V_3);
					this.WriteSpace();
					V_4 = (V_4 as TypeSpecification).get_ElementType();
				}
				V_5 = V_3.get_Name();
				if (this.get_MethodContext() != null && this.get_MethodContext().get_Method() == V_3.get_Method() && this.get_MethodContext().get_ParameterDefinitionToNameMap().ContainsKey(V_3))
				{
					V_5 = this.get_MethodContext().get_ParameterDefinitionToNameMap().get_Item(V_3);
				}
				this.WriteParameterTypeAndName(V_4, V_5, V_3);
				V_2 = V_2 + 1;
			}
			return;
		}

		protected virtual void WritePropertyTypeAndNameWithArguments(PropertyDefinition property)
		{
			return;
		}

		protected virtual void WriteReadOnlyWriteOnlyProperty(PropertyDefinition property)
		{
			return;
		}

		private void WriteReference(TypeReference reference)
		{
			if (reference.get_DeclaringType() != null && !reference.get_IsGenericParameter())
			{
				V_1 = reference.get_DeclaringType();
				if (!reference.get_IsGenericInstance())
				{
					this.WriteReference(V_1);
					this.Write(".");
				}
				else
				{
					V_2 = reference as GenericInstanceType;
					if (V_1.get_HasGenericParameters())
					{
						V_3 = new GenericInstanceType(V_1);
						V_4 = new Collection<TypeReference>(V_2.get_GenericArguments());
						V_5 = new Collection<TypeReference>(V_3.get_GenericArguments());
						V_6 = V_1.get_GenericParameters().get_Count();
						V_7 = 0;
						while (V_7 < V_6)
						{
							V_3.AddGenericArgument(V_2.get_GenericArguments().get_Item(V_7));
							V_3.get_GenericArguments().Add(V_2.get_GenericArguments().get_Item(V_7));
							V_7 = V_7 + 1;
						}
						this.WriteReference(V_3);
						this.Write(".");
						if (V_2.get_GenericArguments().get_Count() - V_6 <= 0)
						{
							this.WriteReference(this.GetTypeName(reference), reference);
						}
						else
						{
							this.WriteTypeSpecification(V_2, V_6);
						}
						V_2.get_GenericArguments().Clear();
						V_2.get_GenericArguments().AddRange(V_4);
						V_3.get_GenericArguments().Clear();
						V_3.get_GenericArguments().AddRange(V_5);
						return;
					}
					this.WriteReference(V_1);
					this.Write(".");
				}
			}
			if (reference as TypeSpecification != null)
			{
				this.WriteTypeSpecification(reference as TypeSpecification, 0);
				return;
			}
			if (!String.op_Inequality(reference.get_Namespace(), "System"))
			{
				V_0 = this.ToEscapedTypeString(reference);
				if (!this.IsReferenceFromMscorlib(reference))
				{
					V_0 = Utilities.EscapeTypeNameIfNeeded(V_0, this.get_Language());
				}
			}
			else
			{
				V_0 = this.GetTypeName(reference);
			}
			if (reference.get_HasGenericParameters() || !this.get_Language().IsEscapedWord(V_0) && !this.get_Language().IsValidIdentifier(V_0))
			{
				if (reference as TypeDefinition == null || !reference.get_HasGenericParameters())
				{
					V_0 = this.GetTypeName(reference);
				}
				else
				{
					V_0 = this.GetTypeName(reference);
					V_9 = 0;
					if (reference.get_DeclaringType() != null && reference.get_DeclaringType().get_HasGenericParameters())
					{
						V_9 = reference.get_DeclaringType().get_GenericParameters().get_Count();
					}
					if (V_9 < reference.get_GenericParameters().get_Count())
					{
						V_0 = String.Concat(V_0, this.get_GenericLeftBracket());
						V_10 = V_9;
						while (V_10 < reference.get_GenericParameters().get_Count())
						{
							if (V_10 > V_9)
							{
								V_0 = String.Concat(V_0, ", ");
							}
							if (this.get_Language().IsValidIdentifier(reference.get_GenericParameters().get_Item(V_10).get_Name()))
							{
								V_0 = String.Concat(V_0, reference.get_GenericParameters().get_Item(V_10).get_Name());
							}
							else
							{
								V_0 = String.Concat(V_0, this.get_Language().ReplaceInvalidCharactersInIdentifier(reference.get_GenericParameters().get_Item(V_10).get_Name()));
							}
							V_10 = V_10 + 1;
						}
						V_0 = String.Concat(V_0, this.get_GenericRightBracket());
					}
				}
			}
			if (reference.get_IsGenericParameter())
			{
				this.WriteReference(V_0, null);
				return;
			}
			this.WriteReference(V_0, reference);
			return;
		}

		public virtual void WriteReferenceAndNamespaceIfInCollision(TypeReference reference)
		{
			this.WriteNamespaceIfTypeInCollision(reference);
			this.WriteReference(reference);
			return;
		}

		protected virtual void WriteRemoveOn(EventDefinition event)
		{
			V_7 = event.get_RemoveMethod().get_MetadataToken();
			V_0 = V_7.ToUInt32();
			this.membersStack.Push(event.get_RemoveMethod());
			V_1 = this.formatter.get_CurrentPosition();
			stackVariable13 = this.get_AttributeWriter();
			stackVariable15 = event.get_RemoveMethod();
			stackVariable17 = new String[1];
			stackVariable17[0] = "System.Runtime.CompilerServices.CompilerGeneratedAttribute";
			stackVariable13.WriteMemberAttributesAndNewLine(stackVariable15, stackVariable17, false);
			this.AddMemberAttributesOffsetSpan(V_0, V_1, this.formatter.get_CurrentPosition());
			V_2 = this.formatter.get_CurrentPosition();
			V_3 = this.formatter.get_CurrentPosition();
			this.WriteMoreRestrictiveMethodVisibility(event.get_RemoveMethod(), event.get_AddMethod());
			V_4 = this.formatter.get_CurrentPosition();
			this.WriteKeyword(this.get_KeyWordWriter().get_RemoveOn());
			this.WriteEventRemoveOnParameters(event);
			V_5 = this.formatter.get_CurrentPosition() - 1;
			this.currentWritingInfo.get_MemberDeclarationToCodePostionMap().set_Item(event.get_RemoveMethod(), new OffsetSpan(V_4, V_5));
			this.WriteLine();
			this.Write(this.GetStatement(event.get_RemoveMethod()));
			this.WriteSpecialEndBlock(this.get_KeyWordWriter().get_RemoveOn());
			this.currentWritingInfo.get_MemberDefinitionToFoldingPositionMap().Add(event.get_RemoveMethod(), new OffsetSpan(V_2, this.formatter.get_CurrentPosition() - 1));
			this.currentWritingInfo.get_MemberTokenToDecompiledCodeMap().Add(V_0, new OffsetSpan(V_3, this.formatter.get_CurrentPosition() - 1));
			dummyVar0 = this.membersStack.Pop();
			return;
		}

		protected virtual void WriteRightPartOfBinaryExpression(BinaryExpression binaryExpression)
		{
			this.Visit(binaryExpression.get_Right());
			return;
		}

		protected void WriteSetMethod(PropertyDefinition property, bool isAutoImplemented)
		{
			V_6 = property.get_SetMethod().get_MetadataToken();
			V_0 = V_6.ToUInt32();
			this.membersStack.Push(property.get_SetMethod());
			V_1 = this.formatter.get_CurrentPosition();
			stackVariable13 = this.get_AttributeWriter();
			stackVariable15 = property.get_SetMethod();
			stackVariable17 = new String[1];
			stackVariable17[0] = "System.Runtime.CompilerServices.CompilerGeneratedAttribute";
			stackVariable13.WriteMemberAttributesAndNewLine(stackVariable15, stackVariable17, false);
			this.AddMemberAttributesOffsetSpan(V_0, V_1, this.formatter.get_CurrentPosition());
			V_2 = this.formatter.get_CurrentPosition();
			V_3 = this.formatter.get_CurrentPosition();
			this.WriteMoreRestrictiveMethodVisibility(property.get_SetMethod(), property.get_GetMethod());
			V_4 = this.formatter.get_CurrentPosition();
			this.WriteKeyword(this.get_KeyWordWriter().get_Set());
			V_5 = this.formatter.get_CurrentPosition() - 1;
			this.currentWritingInfo.get_MemberDeclarationToCodePostionMap().set_Item(property.get_SetMethod(), new OffsetSpan(V_4, V_5));
			if (this.get_KeyWordWriter().get_ByVal() != null)
			{
				this.WriteToken("(");
				this.WriteKeyword(this.get_KeyWordWriter().get_ByVal());
				this.WriteSpace();
				this.WriteTypeAndName(property.get_PropertyType(), "value", null);
				this.WriteToken(")");
			}
			if (property.get_SetMethod().get_Body() == null || this.get_SupportsAutoProperties() & isAutoImplemented)
			{
				this.WriteEndOfStatement();
			}
			else
			{
				this.WriteLine();
				this.Write(this.GetStatement(property.get_SetMethod()));
			}
			this.WriteSpecialEndBlock(this.get_KeyWordWriter().get_Set());
			this.currentWritingInfo.get_MemberDefinitionToFoldingPositionMap().Add(property.get_SetMethod(), new OffsetSpan(V_2, this.formatter.get_CurrentPosition() - 1));
			this.currentWritingInfo.get_MemberTokenToDecompiledCodeMap().Add(V_0, new OffsetSpan(V_3, this.formatter.get_CurrentPosition() - 1));
			dummyVar0 = this.membersStack.Pop();
			return;
		}

		protected void WriteSingleGenericParameterConstraintsList(GenericParameter genericParameter)
		{
			V_0 = false;
			if (genericParameter.get_HasNotNullableValueTypeConstraint())
			{
				if (V_0)
				{
					this.WriteToken(",");
					this.WriteSpace();
				}
				V_0 = true;
				this.WriteKeyword(this.get_KeyWordWriter().get_Struct());
			}
			if (genericParameter.get_HasReferenceTypeConstraint())
			{
				if (V_0)
				{
					this.WriteToken(",");
					this.WriteSpace();
				}
				V_0 = true;
				this.WriteKeyword(this.get_KeyWordWriter().get_Class());
			}
			V_1 = genericParameter.get_Constraints().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (genericParameter.get_HasNotNullableValueTypeConstraint() && String.op_Equality(V_2.get_FullName(), "System.ValueType"))
					{
						continue;
					}
					if (V_0)
					{
						this.WriteToken(",");
						this.WriteSpace();
					}
					this.WriteReferenceAndNamespaceIfInCollision(V_2);
					V_0 = true;
				}
			}
			finally
			{
				V_1.Dispose();
			}
			if (genericParameter.get_HasDefaultConstructorConstraint() && !genericParameter.get_HasNotNullableValueTypeConstraint())
			{
				if (V_0)
				{
					this.WriteToken(",");
					this.WriteSpace();
				}
				V_0 = true;
				this.WriteConstructorGenericConstraint();
			}
			return;
		}

		protected virtual void WriteSpecialBetweenParenthesis(Expression expression)
		{
			return;
		}

		protected virtual void WriteSpecialBetweenParenthesis(Action action)
		{
			return;
		}

		private void WriteSpecialDoubleConstants(double value, IMemberDefinition currentMember)
		{
			if (Double.IsNaN(value))
			{
				this.WriteDoubleNan(currentMember);
			}
			if (Double.IsInfinity(value))
			{
				this.WriteDoubleInfinity(currentMember, value);
			}
			return;
		}

		protected virtual void WriteSpecialEndBlock(string statementName)
		{
			return;
		}

		private void WriteSpecialFloatValue(float value, IMemberDefinition currentMember)
		{
			if (Single.IsNaN(value))
			{
				this.WriteFloatNan(currentMember);
			}
			if (Single.IsInfinity(value))
			{
				this.WriteFloatInfinity(currentMember, (double)value);
			}
			return;
		}

		private void WriteSplitProperty(PropertyDefinition property)
		{
			V_0 = property.get_GetMethod().GetMoreVisibleMethod(property.get_SetMethod());
			this.WriteMethodVisibilityAndSpace(V_0);
			if (this.get_KeyWordWriter().get_Property() != null)
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_Property());
				this.WriteSpace();
			}
			this.WritePropertyTypeAndNameWithArguments(property);
			this.WritePropertyInterfaceImplementations(property);
			this.WriteBeginBlock(false);
			this.WriteLine();
			this.Indent();
			if (property.get_GetMethod() != null)
			{
				V_5 = property.get_GetMethod().get_MetadataToken();
				V_2 = V_5.ToUInt32();
				V_3 = this.formatter.get_CurrentPosition();
				stackVariable102 = this.get_AttributeWriter();
				stackVariable104 = property.get_GetMethod();
				stackVariable106 = new String[1];
				stackVariable106[0] = "System.Runtime.CompilerServices.CompilerGeneratedAttribute";
				stackVariable102.WriteMemberAttributesAndNewLine(stackVariable104, stackVariable106, false);
				this.AddMemberAttributesOffsetSpan(V_2, V_3, this.formatter.get_CurrentPosition());
				this.membersStack.Push(property.get_GetMethod());
				V_4 = this.formatter.get_CurrentPosition();
				this.WriteSplitPropertyGetter(property);
				this.currentWritingInfo.get_MemberTokenToDecompiledCodeMap().Add(V_2, new OffsetSpan(V_4, this.formatter.get_CurrentPosition() - 1));
				dummyVar0 = this.membersStack.Pop();
				this.WriteLine();
			}
			if (property.get_SetMethod() != null)
			{
				V_5 = property.get_SetMethod().get_MetadataToken();
				V_6 = V_5.ToUInt32();
				V_7 = this.formatter.get_CurrentPosition();
				stackVariable55 = this.get_AttributeWriter();
				stackVariable57 = property.get_SetMethod();
				stackVariable59 = new String[1];
				stackVariable59[0] = "System.Runtime.CompilerServices.CompilerGeneratedAttribute";
				stackVariable55.WriteMemberAttributesAndNewLine(stackVariable57, stackVariable59, false);
				this.AddMemberAttributesOffsetSpan(V_6, V_7, this.formatter.get_CurrentPosition());
				this.membersStack.Push(property.get_SetMethod());
				V_8 = this.formatter.get_CurrentPosition();
				this.WriteSplitPropertySetter(property);
				this.currentWritingInfo.get_MemberTokenToDecompiledCodeMap().Add(V_6, new OffsetSpan(V_8, this.formatter.get_CurrentPosition() - 1));
				dummyVar1 = this.membersStack.Pop();
				this.WriteLine();
			}
			this.Outdent();
			this.WriteEndBlock("Property");
			V_1 = Utilities.GetCompileGeneratedBackingField(property);
			if (V_1 != null)
			{
				this.WriteLine();
				this.WriteLine();
				this.Write(V_1);
			}
			if (property.get_GetMethod() != null)
			{
				this.WriteLine();
				this.WriteLine();
				this.Write(property.get_GetMethod());
			}
			if (property.get_SetMethod() != null)
			{
				this.WriteLine();
				this.WriteLine();
				this.Write(property.get_SetMethod());
			}
			return;
		}

		private void WriteSplitPropertyGetter(PropertyDefinition property)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_Get());
			this.WriteBeginBlock(false);
			this.WriteLine();
			this.Indent();
			stackVariable14 = new MethodInvocationExpression(new MethodReferenceExpression(null, property.get_GetMethod(), null), null);
			stackVariable14.set_Arguments(this.CopyMethodParametersAsArguments(property.get_GetMethod()));
			this.Write(new ExpressionStatement(new ReturnExpression(stackVariable14, null)));
			this.WriteLine();
			this.Outdent();
			this.WriteEndBlock("Get");
			return;
		}

		private void WriteSplitPropertySetter(PropertyDefinition property)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_Set());
			this.WriteBeginBlock(false);
			this.WriteLine();
			this.Indent();
			stackVariable14 = new MethodInvocationExpression(new MethodReferenceExpression(null, property.get_SetMethod(), null), null);
			stackVariable14.set_Arguments(this.CopyMethodParametersAsArguments(property.get_SetMethod()));
			this.Write(new ExpressionStatement(stackVariable14));
			this.WriteLine();
			this.Outdent();
			this.WriteEndBlock("Set");
			return;
		}

		protected void WriteTokenBetweenSpace(string token)
		{
			this.WriteSpace();
			this.WriteToken(token);
			this.WriteSpace();
			return;
		}

		protected virtual void WriteTypeAndName(TypeReference typeReference, string name, object reference)
		{
			return;
		}

		protected virtual void WriteTypeAndName(TypeReference typeReference, string name)
		{
			return;
		}

		protected virtual bool WriteTypeBaseTypes(TypeDefinition type, bool isPartial = false)
		{
			V_0 = false;
			if (type.get_BaseType() != null && String.op_Inequality(type.get_BaseType().get_FullName(), "System.Object") && !isPartial || this.IsImplemented(type, type.get_BaseType().Resolve()))
			{
				V_0 = true;
				this.WriteBaseTypeInheritColon();
				this.WriteReferenceAndNamespaceIfInCollision(type.get_BaseType());
			}
			return V_0;
		}

		protected virtual void WriteTypeBaseTypesAndInterfaces(TypeDefinition type, bool isPartial)
		{
			V_0 = false;
			if (!type.get_IsEnum() && !type.get_IsValueType())
			{
				V_0 = this.WriteTypeBaseTypes(type, isPartial);
			}
			if (!type.get_IsEnum())
			{
				this.WriteTypeInterfaces(type, isPartial, V_0);
			}
			return;
		}

		protected override string WriteTypeDeclaration(TypeDefinition type, bool isPartial = false)
		{
			V_0 = GenericHelper.GetNonGenericName(type.get_Name());
			if (this.get_ModuleContext().get_RenamedMembers().Contains(type.get_MetadataToken().ToUInt32()))
			{
				this.WriteComment(V_0);
				this.WriteLine();
			}
			V_1 = String.Empty;
			this.WriteTypeVisiblity(type);
			this.WriteSpace();
			if (isPartial)
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_Partial());
				this.WriteSpace();
			}
			if (!type.get_IsEnum())
			{
				if (!type.get_IsValueType())
				{
					if (type.get_IsClass())
					{
						if (!type.get_IsStaticClass())
						{
							if (type.get_IsSealed())
							{
								this.WriteKeyword(this.get_KeyWordWriter().get_SealedType());
								this.WriteSpace();
							}
							if (type.get_IsAbstract())
							{
								this.WriteKeyword(this.get_KeyWordWriter().get_AbstractType());
								this.WriteSpace();
							}
							V_1 = this.get_KeyWordWriter().get_Class();
						}
						else
						{
							this.WriteTypeStaticKeywordAndSpace();
							V_1 = this.get_KeyWordWriter().get_StaticClass();
						}
					}
					if (type.get_IsInterface())
					{
						V_1 = this.get_KeyWordWriter().get_Interface();
					}
				}
				else
				{
					V_1 = this.get_KeyWordWriter().get_Struct();
				}
			}
			else
			{
				V_1 = this.get_KeyWordWriter().get_Enum();
			}
			this.WriteKeyword(V_1);
			this.WriteSpace();
			V_2 = this.formatter.get_CurrentPosition();
			this.WriteGenericName(type);
			V_3 = this.formatter.get_CurrentPosition() - 1;
			this.currentWritingInfo.get_MemberDeclarationToCodePostionMap().set_Item(type, new OffsetSpan(V_2, V_3));
			if (!type.get_IsDefaultEnum() && type.get_IsEnum())
			{
				V_5 = type.get_Fields().get_Item(0).get_FieldType();
				if (String.op_Inequality(V_5.get_Name(), "Int32"))
				{
					this.WriteEnumBaseTypeInheritColon();
					this.WriteReferenceAndNamespaceIfInCollision(V_5);
				}
			}
			this.WriteTypeBaseTypesAndInterfaces(type, isPartial);
			if (type.get_HasGenericParameters())
			{
				this.PostWriteGenericParametersConstraints(type);
			}
			return V_1;
		}

		protected abstract void WriteTypeInterfaces(TypeDefinition type, bool isPartial, bool baseTypeWritten);

		protected override void WriteTypeOpeningBlock(TypeDefinition type)
		{
			this.WriteLine();
			this.Indent();
			return;
		}

		private void WriteTypeReferenceNavigationName(TypeReference type)
		{
			if (type.get_IsOptionalModifier() || type.get_IsRequiredModifier())
			{
				this.WriteTypeReferenceNavigationName((type as IModifierType).get_ElementType());
				return;
			}
			if (type.get_IsByReference())
			{
				type = (type as ByReferenceType).get_ElementType();
				this.WriteTypeReferenceNavigationName(type);
				this.formatter.Write("&");
				return;
			}
			if (type.get_IsPointer())
			{
				type = (type as PointerType).get_ElementType();
				this.WriteTypeReferenceNavigationName(type);
				this.formatter.Write("*");
				return;
			}
			if (type.get_IsArray())
			{
				V_1 = (type as ArrayType).get_Dimensions().get_Count();
				type = (type as ArrayType).get_ElementType();
				this.WriteTypeReferenceNavigationName(type);
				this.formatter.Write(this.get_IndexLeftBracket());
				this.formatter.Write(new String(',', V_1 - 1));
				this.formatter.Write(this.get_IndexRightBracket());
				return;
			}
			V_0 = GenericHelper.GetNonGenericName(type.get_Name());
			if (this.get_Settings().get_RenameInvalidMembers())
			{
				V_0 = this.get_Language().ReplaceInvalidCharactersInIdentifier(V_0);
			}
			this.formatter.Write(V_0);
			if (type as GenericInstanceType != null)
			{
				this.WriteGenericInstanceTypeArguments((GenericInstanceType)type);
				return;
			}
			if (type.get_HasGenericParameters())
			{
				this.WriteGenericParametersToDefinition(type, null, this.get_Settings().get_RenameInvalidMembers());
			}
			return;
		}

		protected virtual void WriteTypeSpecification(TypeSpecification typeSpecification, int startingArgument = 0)
		{
			if (typeSpecification as PointerType != null)
			{
				this.WriteReference(typeSpecification.get_ElementType());
				this.WriteToken("*");
				return;
			}
			if (typeSpecification as PinnedType != null)
			{
				if (typeSpecification.get_ElementType() as ByReferenceType == null)
				{
					this.WriteReference(typeSpecification.get_ElementType());
					return;
				}
				this.WriteReference((typeSpecification.get_ElementType() as ByReferenceType).get_ElementType());
				this.WriteToken("*");
				return;
			}
			if (typeSpecification as ByReferenceType != null)
			{
				this.WriteReference(typeSpecification.get_ElementType());
				this.WriteToken("&");
				return;
			}
			V_0 = typeSpecification as ArrayType;
			if (V_0 != null)
			{
				V_2 = new List<int>();
				V_3 = typeSpecification.get_ElementType();
				while (V_3 as ArrayType != null)
				{
					V_4 = V_3 as ArrayType;
					V_2.Add(V_4.get_Dimensions().get_Count());
					V_3 = V_4.get_ElementType();
				}
				this.WriteReference(V_3);
				this.WriteToken(this.get_IndexLeftBracket());
				if (V_0.get_Dimensions() != null)
				{
					V_5 = 1;
					while (V_5 < V_0.get_Dimensions().get_Count())
					{
						this.WriteToken(",");
						V_5 = V_5 + 1;
					}
				}
				this.WriteToken(this.get_IndexRightBracket());
				V_6 = V_2.GetEnumerator();
				try
				{
					while (V_6.MoveNext())
					{
						V_7 = V_6.get_Current();
						this.WriteToken(this.get_IndexLeftBracket());
						V_8 = 1;
						while (V_8 < V_7)
						{
							this.WriteToken(",");
							V_8 = V_8 + 1;
						}
						this.WriteToken(this.get_IndexRightBracket());
					}
				}
				finally
				{
					((IDisposable)V_6).Dispose();
				}
				return;
			}
			V_1 = typeSpecification as GenericInstanceType;
			if (V_1 == null)
			{
				if (typeSpecification.get_MetadataType() == 32)
				{
					this.Write(typeSpecification.get_Name());
					return;
				}
				if (!typeSpecification.get_IsRequiredModifier())
				{
					throw new NotSupportedException();
				}
				if (this.isWritingComment && String.op_Equality((typeSpecification as RequiredModifierType).get_ModifierType().get_FullName(), "System.Runtime.CompilerServices.IsVolatile"))
				{
					this.WriteVolatileType(typeSpecification.get_ElementType());
					return;
				}
				this.WriteReference(typeSpecification.get_ElementType());
				return;
			}
			if (!this.get_SupportsSpecialNullable() || V_1.GetFriendlyFullName(this.get_Language()).IndexOf("System.Nullable<") != 0 || !V_1.get_GenericArguments().get_Item(0).get_IsValueType())
			{
				this.WriteGenericInstance(V_1, startingArgument);
				return;
			}
			V_9 = V_1.get_GenericArguments().get_Item(0);
			if (V_1.get_PostionToArgument().ContainsKey(0))
			{
				V_9 = V_1.get_PostionToArgument().get_Item(0);
			}
			this.WriteReference(V_9);
			this.WriteToken("?");
			return;
		}

		protected virtual void WriteTypeStaticKeywordAndSpace()
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_Static());
			this.WriteSpace();
			return;
		}

		protected void WriteTypeVisiblity(TypeDefinition type)
		{
			if (this.get_TypeContext().get_IsWinRTImplementation())
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_Public());
				return;
			}
			if (type.get_IsPublic())
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_Public());
				return;
			}
			if (type.get_IsNestedPublic())
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_Public());
				return;
			}
			if (type.get_IsNestedFamily())
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_Protected());
				return;
			}
			if (type.get_IsNestedPrivate())
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_Private());
				return;
			}
			if (type.get_IsNestedAssembly())
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_Internal());
				return;
			}
			if (!type.get_IsNestedFamilyOrAssembly() && !type.get_IsNestedFamilyAndAssembly())
			{
				if (!type.get_IsNotPublic())
				{
					throw new NotSupportedException();
				}
				this.WriteKeyword(this.get_KeyWordWriter().get_Internal());
				return;
			}
			this.WriteKeyword(this.get_KeyWordWriter().get_Protected());
			this.WriteSpace();
			this.WriteKeyword(this.get_KeyWordWriter().get_Internal());
			return;
		}

		protected virtual void WriteVariableTypeAndName(VariableDefinition variable)
		{
			return;
		}

		protected virtual void WriteVolatileType(TypeReference reference)
		{
			return;
		}
	}
}