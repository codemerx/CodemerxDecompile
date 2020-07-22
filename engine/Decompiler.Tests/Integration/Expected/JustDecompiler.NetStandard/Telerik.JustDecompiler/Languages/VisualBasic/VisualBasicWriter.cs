using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Languages.VisualBasic
{
	public class VisualBasicWriter : NamespaceImperativeLanguageWriter
	{
		private readonly Stack<StatementState> statementStates;

		protected override string CharEnd
		{
			get
			{
				return "\"C";
			}
		}

		protected override string CharStart
		{
			get
			{
				return "\"";
			}
		}

		protected override string GenericLeftBracket
		{
			get
			{
				return "(Of ";
			}
		}

		protected override string GenericRightBracket
		{
			get
			{
				return ")";
			}
		}

		protected override string HexValuePrefix
		{
			get
			{
				return "&H";
			}
		}

		public override string IndexLeftBracket
		{
			get
			{
				return "(";
			}
		}

		public override string IndexRightBracket
		{
			get
			{
				return ")";
			}
		}

		protected override bool SupportsAutoProperties
		{
			get
			{
				return true;
			}
		}

		public VisualBasicWriter(ILanguage language, IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings)
		{
			this.statementStates = new Stack<StatementState>();
			base(language, formatter, exceptionFormatter, settings);
			return;
		}

		protected string ConvertCharOnVB(char ch)
		{
			if (ch == '\n')
			{
				return "\" & VbCrLf & \"";
			}
			if (ch == '\r')
			{
				return String.Empty;
			}
			return ch.ToString();
		}

		private int CountConstraints(GenericParameter parameter)
		{
			V_0 = 0;
			if (parameter.get_HasConstraints())
			{
				V_0 = V_0 + parameter.get_Constraints().get_Count();
			}
			if (parameter.get_HasDefaultConstructorConstraint())
			{
				V_0 = V_0 + 1;
			}
			if (parameter.get_HasReferenceTypeConstraint())
			{
				V_0 = V_0 + 1;
			}
			if (parameter.get_HasNotNullableValueTypeConstraint())
			{
				V_0 = V_0 + 1;
			}
			return V_0;
		}

		protected override Telerik.JustDecompiler.Languages.AttributeWriter CreateAttributeWriter()
		{
			return new VisualBasicAttributeWriter(this);
		}

		protected override IKeyWordWriter CreateKeyWordWriter()
		{
			return new VisualBasicKeyWordWriter();
		}

		protected override void DoWriteParameterTypeAndName(TypeReference type, string name, ParameterDefinition reference)
		{
			V_0 = new VisualBasicWriter.u003cu003ec__DisplayClass27_0();
			V_0.u003cu003e4__this = this;
			V_0.name = name;
			if (reference.IsParamArray())
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_ParamArray());
				this.WriteSpace();
			}
			if (!this.get_Language().IsValidIdentifier(V_0.name))
			{
				V_0.name = this.get_Language().ReplaceInvalidCharactersInIdentifier(V_0.name);
			}
			if (this.get_Language().IsGlobalKeyword(V_0.name))
			{
				V_0.name = Utilities.EscapeNameIfNeeded(V_0.name, this.get_Language());
			}
			if (!this.isWritingComment)
			{
				this.WriteAndMapParameterToCode(new Action(V_0.u003cDoWriteParameterTypeAndNameu003eb__0), reference.get_Index());
			}
			else
			{
				this.Write(V_0.name);
			}
			if (!String.IsNullOrEmpty(this.ToTypeString(type)))
			{
				this.WriteAsBetweenSpaces();
				this.WriteReferenceAndNamespaceIfInCollision(type);
			}
			return;
		}

		protected override void DoWriteTypeAndName(TypeReference typeReference, string name, object reference)
		{
			V_0 = this.formatter.get_CurrentPosition();
			this.WriteReference(name, reference);
			if (reference as IMemberDefinition != null)
			{
				V_1 = this.formatter.get_CurrentPosition() - 1;
				this.currentWritingInfo.get_MemberDeclarationToCodePostionMap().set_Item((IMemberDefinition)reference, new OffsetSpan(V_0, V_1));
			}
			this.WriteAsBetweenSpaces();
			this.WriteReferenceAndNamespaceIfInCollision(typeReference);
			return;
		}

		protected override void DoWriteTypeAndName(TypeReference typeReference, string name)
		{
			this.Write(name);
			this.WriteAsBetweenSpaces();
			this.WriteReferenceAndNamespaceIfInCollision(typeReference);
			return;
		}

		protected override void DoWriteVariableTypeAndName(VariableDefinition variable)
		{
			V_0 = new VisualBasicWriter.u003cu003ec__DisplayClass26_0();
			V_0.u003cu003e4__this = this;
			V_0.variable = variable;
			if (!this.isWritingComment)
			{
				this.WriteAndMapVariableToCode(new Action(V_0.u003cDoWriteVariableTypeAndNameu003eb__0), V_0.variable);
			}
			else
			{
				this.Write(this.GetVariableName(V_0.variable));
			}
			this.WriteAsBetweenSpaces();
			this.WriteReferenceAndNamespaceIfInCollision(V_0.variable.get_VariableType());
			return;
		}

		private string GetCastMethod(TypeReference type)
		{
			V_0 = type.get_Name();
			if (V_0 != null)
			{
				if (String.op_Equality(V_0, "Decimal"))
				{
					return "CDec";
				}
				if (String.op_Equality(V_0, "Single"))
				{
					return "CSng";
				}
				if (String.op_Equality(V_0, "Byte"))
				{
					return "CByte";
				}
				if (String.op_Equality(V_0, "SByte"))
				{
					return "CSByte";
				}
				if (String.op_Equality(V_0, "Char"))
				{
					return "CChar";
				}
				if (String.op_Equality(V_0, "Double"))
				{
					return "CDbl";
				}
				if (String.op_Equality(V_0, "Boolean"))
				{
					return "CBool";
				}
				if (String.op_Equality(V_0, "Int16"))
				{
					return "CShort";
				}
				if (String.op_Equality(V_0, "UInt16"))
				{
					return "CUShort";
				}
				if (String.op_Equality(V_0, "Int32"))
				{
					return "CInt";
				}
				if (String.op_Equality(V_0, "UInt32"))
				{
					return "CUInt";
				}
				if (String.op_Equality(V_0, "Int64"))
				{
					return "CLng";
				}
				if (String.op_Equality(V_0, "UInt64"))
				{
					return "CULng";
				}
				if (String.op_Equality(V_0, "String"))
				{
					return "CStr";
				}
				if (String.op_Equality(V_0, "Object"))
				{
					return "CObj";
				}
				if (String.op_Equality(V_0, "RuntimeArgumentHandle"))
				{
					return null;
				}
			}
			return null;
		}

		private Dictionary<StatementState, string> GetContinuableParents()
		{
			stackVariable0 = new Dictionary<StatementState, string>();
			stackVariable0.Add(5, "Do");
			stackVariable0.Add(2, "For");
			stackVariable0.Add(3, "For");
			stackVariable0.Add(4, "While");
			return stackVariable0;
		}

		private object GetDecrementedValue(LiteralExpression expression)
		{
			V_0 = expression.get_ExpressionType().get_Name();
			if (V_0 != null)
			{
				if (String.op_Equality(V_0, "Byte"))
				{
					return (byte)((Byte)expression.get_Value() - 1);
				}
				if (String.op_Equality(V_0, "SByte"))
				{
					return (sbyte)((SByte)expression.get_Value() - 1);
				}
				if (String.op_Equality(V_0, "Int16"))
				{
					return (short)((Int16)expression.get_Value() - 1);
				}
				if (String.op_Equality(V_0, "UInt16"))
				{
					return (ushort)((UInt16)expression.get_Value() - 1);
				}
				if (String.op_Equality(V_0, "Int32"))
				{
					return (Int32)expression.get_Value() - 1;
				}
				if (String.op_Equality(V_0, "UInt32"))
				{
					return (UInt32)expression.get_Value() - 1;
				}
				if (String.op_Equality(V_0, "Int64"))
				{
					return (Int64)expression.get_Value() - (long)1;
				}
				if (String.op_Equality(V_0, "UInt64"))
				{
					return (UInt64)expression.get_Value() - (long)1;
				}
				if (String.op_Equality(V_0, "Char"))
				{
					return (char)((Char)expression.get_Value() - 1);
				}
			}
			throw new ArgumentException("Invalid data type for dimension of an array.");
		}

		private Dictionary<StatementState, string> GetExitableParents()
		{
			stackVariable2 = new Dictionary<StatementState, string>(this.GetContinuableParents());
			stackVariable2.Add(6, "Select");
			return stackVariable2;
		}

		private TypeReference GetExpressionType(Expression expression)
		{
			V_0 = expression.get_CodeNodeType();
			if (V_0 > 26)
			{
				if (V_0 == 30)
				{
					return ((FieldReferenceExpression)expression).get_Field().get_FieldType();
				}
				if (V_0 == 42)
				{
					return ((PropertyReferenceExpression)expression).get_Property().get_PropertyType();
				}
			}
			else
			{
				if (V_0 == 19)
				{
					V_1 = ((MethodInvocationExpression)expression).get_MethodExpression();
					if (V_1.get_CodeNodeType() == 20)
					{
						return V_1.get_Method().get_ReturnType();
					}
				}
				else
				{
					if (V_0 == 26)
					{
						return ((VariableReferenceExpression)expression).get_Variable().get_VariableType();
					}
				}
			}
			return null;
		}

		private string GetExpressionType(LiteralExpression literalExpression)
		{
			if (literalExpression.get_Value() as Decimal != null)
			{
				return "Decimal";
			}
			if (literalExpression.get_Value() as Single != 0)
			{
				return "Float";
			}
			if (literalExpression.get_Value() as Byte != 0)
			{
				return "Byte";
			}
			if (literalExpression.get_Value() as SByte != 0)
			{
				return "SByte";
			}
			if (literalExpression.get_Value() as Char != 0)
			{
				return "Char";
			}
			if (literalExpression.get_Value() as Double != 0)
			{
				return "Double";
			}
			if (literalExpression.get_Value() as Boolean != false)
			{
				return "Boolean";
			}
			if (literalExpression.get_Value() as Int16 != 0)
			{
				return "Short";
			}
			if (literalExpression.get_Value() as Int32 != 0)
			{
				return "Integer";
			}
			if (literalExpression.get_Value() as Int64 != 0)
			{
				return "Long";
			}
			if (literalExpression.get_Value() as UInt16 != 0)
			{
				return "UShort";
			}
			if (literalExpression.get_Value() as UInt32 != 0)
			{
				return "UInteger";
			}
			if (literalExpression.get_Value() as UInt64 != 0)
			{
				return "ULong";
			}
			if (literalExpression.get_Value() as String != null)
			{
				return "String";
			}
			return "Object";
		}

		private Expression GetForStep(Expression incrementExpression)
		{
			if (incrementExpression.get_CodeNodeType() == 23)
			{
				V_0 = (UnaryExpression)incrementExpression;
				if (V_0.get_Operator() == 3 || V_0.get_Operator() == 5)
				{
					return new LiteralExpression((object)-1, this.get_MethodContext().get_Method().get_Module().get_TypeSystem(), null);
				}
			}
			if (incrementExpression.get_CodeNodeType() == 24 && (incrementExpression as BinaryExpression).get_Operator() == 4)
			{
				return this.GetNegatedExpression(((BinaryExpression)incrementExpression).get_Right());
			}
			if (incrementExpression.get_CodeNodeType() == 24 && (incrementExpression as BinaryExpression).get_Operator() == 2)
			{
				return ((BinaryExpression)incrementExpression).get_Right();
			}
			if (incrementExpression.get_CodeNodeType() == 24 && (incrementExpression as BinaryExpression).get_IsAssignmentExpression())
			{
				V_2 = (BinaryExpression)incrementExpression;
				if (V_2.get_Right().get_CodeNodeType() != 31)
				{
					V_3 = V_2.get_Right() as BinaryExpression;
				}
				else
				{
					V_4 = V_2.get_Right() as ExplicitCastExpression;
					while (V_4.get_Expression().get_CodeNodeType() == 31)
					{
						V_4 = V_4.get_Expression() as ExplicitCastExpression;
					}
					V_3 = V_4.get_Expression() as BinaryExpression;
				}
				if (V_3 != null)
				{
					V_5 = V_3.get_Right();
					if (V_3.get_Operator() != 3)
					{
						return V_5;
					}
					return this.GetNegatedExpression(V_5);
				}
			}
			return null;
		}

		private Collection<TypeReference> GetGenericExtensionMethodArguments(GenericInstanceMethod genericMethod)
		{
			V_0 = genericMethod.get_ElementMethod().get_Parameters().get_Item(0).get_ParameterType();
			if (!V_0.get_IsGenericInstance() && !V_0.get_IsGenericParameter())
			{
				return genericMethod.get_GenericArguments();
			}
			V_1 = new HashSet<GenericParameter>();
			if (!V_0.get_IsGenericInstance())
			{
				dummyVar1 = V_1.Add(V_0 as GenericParameter);
			}
			else
			{
				V_3 = new Queue<GenericInstanceType>();
				V_3.Enqueue(V_0 as GenericInstanceType);
				while (V_3.get_Count() > 0)
				{
					V_4 = V_3.Dequeue().get_GenericArguments().GetEnumerator();
					try
					{
						while (V_4.MoveNext())
						{
							V_5 = V_4.get_Current();
							if (!V_5.get_IsGenericInstance())
							{
								if (!V_5.get_IsGenericParameter())
								{
									continue;
								}
								dummyVar0 = V_1.Add(V_5 as GenericParameter);
							}
							else
							{
								V_3.Enqueue(V_5 as GenericInstanceType);
							}
						}
					}
					finally
					{
						V_4.Dispose();
					}
				}
			}
			V_2 = new Collection<TypeReference>();
			V_6 = 0;
			while (V_6 < genericMethod.get_ElementMethod().get_GenericParameters().get_Count())
			{
				if (!V_1.Contains(genericMethod.get_ElementMethod().get_GenericParameters().get_Item(V_6)))
				{
					V_2.Add(genericMethod.get_GenericArguments().get_Item(V_6));
				}
				V_6 = V_6 + 1;
			}
			return V_2;
		}

		private string GetGenericNameFromMemberReference(TypeReference type)
		{
			V_0 = type.Resolve();
			if (V_0 != null)
			{
				return V_0.GetGenericName(this.get_Language(), "(Of ", ")");
			}
			return type.GetGenericName(this.get_Language(), "(Of ", ")");
		}

		private Expression GetNegatedExpression(Expression expression)
		{
			return new UnaryExpression(0, expression, null);
		}

		private ICollection<ImplementedMember> GetNotExplicitlyImplementedMembers(ICollection<ImplementedMember> implementedMembers)
		{
			V_0 = new List<ImplementedMember>();
			V_1 = implementedMembers.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (this.get_TypeContext().get_ExplicitlyImplementedMembers().Contains(V_2.get_DeclaringType(), V_2.get_Member().get_FullName()))
					{
						continue;
					}
					V_0.Add(V_2);
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			return V_0;
		}

		private bool IsArrayCreationAssignment(BinaryExpression node)
		{
			if (this.IsArrayCreationToVariableDeclarationAssignment(node) || this.IsArrayCreationToVariableReferenceAssignment(node))
			{
				return true;
			}
			return this.IsArrayCreationToFieldReferenceAssignment(node);
		}

		private bool IsArrayCreationToFieldReferenceAssignment(BinaryExpression node)
		{
			if (node.get_Operator() != 26 || node.get_Left().get_CodeNodeType() != 84)
			{
				return false;
			}
			return node.get_Right().get_CodeNodeType() == 38;
		}

		private bool IsArrayCreationToVariableDeclarationAssignment(BinaryExpression node)
		{
			if (node.get_Operator() != 26 || node.get_Left().get_CodeNodeType() != 82)
			{
				return false;
			}
			return node.get_Right().get_CodeNodeType() == 38;
		}

		private bool IsArrayCreationToVariableReferenceAssignment(BinaryExpression node)
		{
			if (node.get_Operator() != 26 || node.get_Left().get_CodeNodeType() != 83)
			{
				return false;
			}
			return node.get_Right().get_CodeNodeType() == 38;
		}

		private bool IsMulticastDelegate(Expression expression)
		{
			if (expression as EventReferenceExpression != null)
			{
				return true;
			}
			V_0 = expression.get_ExpressionType().Resolve();
			if (V_0 == null)
			{
				return false;
			}
			if (V_0.get_BaseType() != null && String.op_Equality(V_0.get_BaseType().get_Name(), "MulticastDelegate"))
			{
				return true;
			}
			return false;
		}

		protected override string OnConvertString(string str)
		{
			V_0 = new StringBuilder();
			V_1 = str;
			V_2 = 0;
			while (V_2 < V_1.get_Length())
			{
				V_3 = V_1.get_Chars(V_2);
				if (V_3 != '\"')
				{
					dummyVar1 = V_0.Append(this.ConvertCharOnVB(V_3));
				}
				else
				{
					dummyVar0 = V_0.Append("\"\"");
				}
				V_2 = V_2 + 1;
			}
			return V_0.ToString();
		}

		protected override void PostWriteGenericParametersConstraints(IGenericDefinition generic)
		{
			if (!generic.get_HasGenericParameters())
			{
				return;
			}
			V_0 = generic.get_GenericParameters().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (!this.IsTypeParameterRedeclaration(V_1) || !this.ShouldWriteConstraintsAsComment(V_1))
					{
						continue;
					}
					this.WriteLine();
					this.StartWritingComment();
					this.WriteToken("Of ");
					this.WriteReference(V_1.get_Name(), null);
					this.WriteGenericParameterConstraints(V_1);
					this.EndWritingComment();
				}
			}
			finally
			{
				V_0.Dispose();
			}
			return;
		}

		protected override void PostWriteMethodReturnType(MethodDefinition method)
		{
			if (method.get_ReturnType() != null && String.op_Inequality(method.get_ReturnType().get_FullName(), "System.Void"))
			{
				this.WriteAsBetweenSpaces();
				this.get_AttributeWriter().WriteMemberReturnValueAttributes(method);
				this.WriteMethodReturnType(method);
			}
			return;
		}

		protected override void StartWritingComment()
		{
			this.WriteComment("");
			this.StartWritingComment();
			return;
		}

		internal override string ToEscapedTypeString(TypeReference reference)
		{
			if (!this.IsReferenceFromMscorlib(reference))
			{
				return this.ToTypeString(reference);
			}
			V_0 = this.ToTypeString(reference);
			if (String.op_Equality(V_0, "Enum") || String.op_Equality(V_0, "Delegate"))
			{
				V_0 = Utilities.Escape(V_0, this.get_Language());
			}
			return V_0;
		}

		protected override string ToString(BinaryOperator op, bool isOneSideNull = false)
		{
			switch (op - 1)
			{
				case 0:
				{
					return "+";
				}
				case 1:
				{
					return "+=";
				}
				case 2:
				{
					return "-";
				}
				case 3:
				{
					return "-=";
				}
				case 4:
				{
					return "*";
				}
				case 5:
				{
					return "*=";
				}
				case 6:
				{
					return "/";
				}
				case 7:
				{
					return "/=";
				}
				case 8:
				{
					if (isOneSideNull)
					{
						return "Is";
					}
					return "=";
				}
				case 9:
				{
					if (isOneSideNull)
					{
						return "IsNot";
					}
					return "<>";
				}
				case 10:
				{
					return "OrElse";
				}
				case 11:
				{
					return "AndAlso";
				}
				case 12:
				{
					return "<";
				}
				case 13:
				{
					return "<=";
				}
				case 14:
				{
					return ">";
				}
				case 15:
				{
					return ">=";
				}
				case 16:
				{
					return "<<";
				}
				case 17:
				{
					return "<<=";
				}
				case 18:
				{
					return ">>";
				}
				case 19:
				{
					return ">>=";
				}
				case 20:
				{
					return "Or";
				}
				case 21:
				{
					return "And";
				}
				case 22:
				{
					return "Xor";
				}
				case 23:
				{
					return "Mod";
				}
				case 24:
				{
				Label0:
					throw new ArgumentException();
				}
				case 25:
				{
					return "=";
				}
				default:
				{
					goto Label0;
				}
			}
		}

		protected override string ToString(UnaryOperator op)
		{
			switch (op)
			{
				case 0:
				{
					return "-";
				}
				case 1:
				{
					return "Not ";
				}
				case 2:
				{
					return "Not ";
				}
				case 3:
				{
					return " - 1";
				}
				case 4:
				{
					return " + 1";
				}
				case 5:
				{
					return "1 - ";
				}
				case 6:
				{
					return "1 + ";
				}
				case 7:
				case 8:
				case 9:
				{
				Label0:
					throw new ArgumentException(String.Format("The unary opperator {0} is not supported in VisualBasic", op));
				}
				case 10:
				{
					return "+";
				}
				case 11:
				{
					return String.Empty;
				}
				default:
				{
					goto Label0;
				}
			}
		}

		public override string ToTypeString(TypeReference type)
		{
			if (this.IsReferenceFromMscorlib(type))
			{
				V_0 = type.get_Name();
				if (V_0 != null)
				{
					if (String.op_Equality(V_0, "Decimal"))
					{
						return "Decimal";
					}
					if (String.op_Equality(V_0, "Single"))
					{
						return "Single";
					}
					if (String.op_Equality(V_0, "Byte"))
					{
						return "Byte";
					}
					if (String.op_Equality(V_0, "SByte"))
					{
						return "SByte";
					}
					if (String.op_Equality(V_0, "Char"))
					{
						return "Char";
					}
					if (String.op_Equality(V_0, "Double"))
					{
						return "Double";
					}
					if (String.op_Equality(V_0, "Boolean"))
					{
						return "Boolean";
					}
					if (String.op_Equality(V_0, "Int16"))
					{
						return "Short";
					}
					if (String.op_Equality(V_0, "Int32"))
					{
						return "Integer";
					}
					if (String.op_Equality(V_0, "Int64"))
					{
						return "Long";
					}
					if (String.op_Equality(V_0, "UInt16"))
					{
						return "UShort";
					}
					if (String.op_Equality(V_0, "UInt32"))
					{
						return "UInteger";
					}
					if (String.op_Equality(V_0, "UInt64"))
					{
						return "ULong";
					}
					if (String.op_Equality(V_0, "String"))
					{
						return "String";
					}
					if (String.op_Equality(V_0, "Void"))
					{
						return "Void";
					}
					if (String.op_Equality(V_0, "Object"))
					{
						return "Object";
					}
					if (String.op_Equality(V_0, "RuntimeArgumentHandle"))
					{
						return String.Empty;
					}
				}
			}
			return this.GetGenericNameFromMemberReference(type);
		}

		protected override bool TypeSupportsExplicitStaticMembers(TypeDefinition type)
		{
			return !type.get_IsStaticClass();
		}

		private void VisitAddressOfExpression(UnaryExpression node)
		{
			if (this.get_MethodReferences().get_Count() == 0)
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_AddressOf());
				this.WriteSpace();
			}
			this.Visit(node.get_Operand());
			return;
		}

		public override void VisitAnonymousPropertyInitializerExpression(AnonymousPropertyInitializerExpression node)
		{
			if (node.get_IsKey())
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_Key());
				this.WriteSpace();
			}
			this.WriteToken(".");
			this.VisitAnonymousPropertyInitializerExpression(node);
			return;
		}

		public override void VisitArrayAssignmentFieldReferenceExpression(ArrayAssignmentFieldReferenceExpression node)
		{
			if (!node.get_HasInitializer())
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_ReDim());
				this.WriteSpace();
			}
			this.Visit(node.get_Field());
			if (!node.get_HasInitializer())
			{
				this.WriteArrayDimensions(node.get_Dimensions(), node.get_ArrayType(), node.get_HasInitializer());
			}
			return;
		}

		public override void VisitArrayAssignmentVariableReferenceExpression(ArrayVariableReferenceExpression node)
		{
			if (!node.get_HasInitializer())
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_ReDim());
				this.WriteSpace();
			}
			this.Visit(node.get_Variable());
			if (!node.get_HasInitializer())
			{
				this.WriteArrayDimensions(node.get_Dimensions(), node.get_ArrayType(), node.get_HasInitializer());
			}
			return;
		}

		public override void VisitArrayCreationExpression(ArrayCreationExpression node)
		{
			this.VisitArrayCreationExpression(node);
			if (!Utilities.IsInitializerPresent(node.get_Initializer()))
			{
				this.WriteSpace();
				this.Write("{}");
			}
			return;
		}

		public override void VisitArrayVariableDeclarationExpression(ArrayVariableDeclarationExpression node)
		{
			V_0 = new VisualBasicWriter.u003cu003ec__DisplayClass113_0();
			V_0.u003cu003e4__this = this;
			if (node.get_Variable().get_Variable().get_VariableType().get_IsOptionalModifier() || node.get_Variable().get_Variable().get_VariableType().get_IsRequiredModifier() && !this.isWritingComment)
			{
				this.StartWritingComment();
				this.VisitVariableDeclarationExpression(node.get_Variable());
				this.EndWritingComment();
				this.WriteLine();
			}
			this.WriteDim();
			V_0.variableName = this.GetVariableName(node.get_Variable().get_Variable());
			this.WriteAndMapVariableToCode(new Action(V_0.u003cVisitArrayVariableDeclarationExpressionu003eb__0), node.get_Variable().get_Variable());
			this.WriteArrayDimensions(node.get_Dimensions(), node.get_ArrayType(), node.get_HasInitializer());
			this.WriteAsBetweenSpaces();
			this.WriteReferenceAndNamespaceIfInCollision(this.GetBaseElementType(node.get_ArrayType()));
			return;
		}

		public override void VisitBaseCtorExpression(BaseCtorExpression node)
		{
			this.VisitCtorExpression(node, "MyBase");
			return;
		}

		public override void VisitBinaryExpression(BinaryExpression node)
		{
			if (node.get_Operator() == 27)
			{
				this.VisitNullCoalesceExpression(node);
				return;
			}
			if (this.IsMulticastDelegate(node.get_Left()))
			{
				V_0 = "";
				if (node.get_Operator() != 2)
				{
					if (node.get_Operator() != 4)
					{
						this.VisitBinaryExpression(node);
						return;
					}
					V_0 = "RemoveHandler";
				}
				else
				{
					V_0 = "AddHandler";
				}
				this.WriteKeyword(V_0);
				this.WriteSpace();
				this.Visit(node.get_Left());
				this.Write(",");
				this.WriteSpace();
				this.WriteSpace();
				this.Visit(node.get_Right());
				return;
			}
			if (!this.IsArrayCreationAssignment(node))
			{
				this.VisitBinaryExpression(node);
				return;
			}
			this.Visit(node.get_Left());
			V_1 = node.get_Right() as ArrayCreationExpression;
			if (Utilities.IsInitializerPresent(V_1.get_Initializer()))
			{
				this.WriteSpace();
				this.Write(this.ToString(26, false));
				if (this.IsArrayCreationToVariableDeclarationAssignment(node))
				{
					this.StartInitializer(V_1.get_Initializer());
					this.Visit(V_1.get_Initializer());
					return;
				}
				this.WriteSpace();
				this.Visit(node.get_Right());
			}
			return;
		}

		public override void VisitBreakStatement(BreakStatement node)
		{
			this.WriteKeyword("Exit");
			this.WriteSpace();
			this.WriteInnermostParentFrom(this.GetExitableParents());
			return;
		}

		public override void VisitCatchClause(CatchClause node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_Catch());
			if (String.op_Inequality(node.get_Type().get_FullName(), "System.Object"))
			{
				this.WriteSpace();
				if (node.get_Variable() == null)
				{
					this.WriteReferenceAndNamespaceIfInCollision(node.get_Type());
				}
				else
				{
					this.statementStates.Push(7);
					this.Visit(node.get_Variable());
					dummyVar0 = this.statementStates.Pop();
				}
			}
			if (node.get_Filter() != null)
			{
				this.WriteSpace();
				this.WriteKeyword(this.get_KeyWordWriter().get_When());
				this.WriteSpace();
				this.Visit((node.get_Filter() as ExpressionStatement).get_Expression());
			}
			this.WriteLine();
			this.Visit(node.get_Body());
			return;
		}

		public override void VisitConditionCase(ConditionCase node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_Case());
			this.WriteSpace();
			this.Visit(node.get_Condition());
			this.WriteLine();
			this.Visit(node.get_Body());
			return;
		}

		public override void VisitConditionExpression(ConditionExpression node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_If());
			this.WriteToken("(");
			this.Visit(node.get_Condition());
			this.WriteToken(",");
			this.WriteSpace();
			this.Visit(node.get_Then());
			this.WriteToken(",");
			this.WriteSpace();
			this.Visit(node.get_Else());
			this.WriteToken(")");
			return;
		}

		public override void VisitContinueStatement(ContinueStatement node)
		{
			this.WriteKeyword("Continue");
			this.WriteSpace();
			this.WriteInnermostParentFrom(this.GetContinuableParents());
			return;
		}

		private void VisitCtorExpression(MethodInvocationExpression node, string ctorKeyword)
		{
			this.WriteKeyword(ctorKeyword);
			this.WriteToken(".");
			if (node.get_MethodExpression().get_CodeNodeType() != 20)
			{
				this.WriteKeyword("New");
			}
			else
			{
				this.WriteReference("New", node.get_MethodExpression().get_Method());
			}
			this.WriteToken("(");
			this.EnterMethodInvocation(node.get_MethodExpression().get_Method());
			this.VisitMethodParameters(node.get_Arguments());
			this.LeaveMethodInvocation();
			this.WriteToken(")");
			return;
		}

		public override void VisitDefaultCase(DefaultCase node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_Default());
			this.WriteSpace();
			this.WriteKeyword(this.get_KeyWordWriter().get_Else());
			this.WriteLine();
			this.Visit(node.get_Body());
			return;
		}

		public override void VisitDefaultObjectExpression(DefaultObjectExpression node)
		{
			this.WriteKeyword("Nothing");
			return;
		}

		public override void VisitDoWhileStatement(DoWhileStatement node)
		{
			this.statementStates.Push(5);
			this.WriteKeyword(this.get_KeyWordWriter().get_Do());
			this.WriteLine();
			this.Visit(node.get_Body());
			this.WriteKeyword(this.get_KeyWordWriter().get_LoopWhile());
			this.WriteSpace();
			this.WriteSpecialBetweenParenthesis(node.get_Condition());
			this.WriteEndOfStatement();
			dummyVar0 = this.statementStates.Pop();
			return;
		}

		public override void VisitExplicitCastExpression(ExplicitCastExpression node)
		{
			V_0 = this.GetCastMethod(node.get_TargetType());
			if (V_0 != null)
			{
				if (node.get_UnresolvedReferenceForAmbiguousCastToObject() != null)
				{
					dummyVar0 = Extensions.ResolveToOverloadedEqualityOperator(node.get_Expression().get_ExpressionType(), out V_1);
					this.WriteNotResolvedReference(V_0, V_1, String.Format("The cast to object might be unnecessary. Please, locate the assembly where \"{0}\" is defined.", V_1.get_Name()));
				}
				else
				{
					this.WriteKeyword(V_0);
				}
				this.WriteToken("(");
				this.WriteCastExpression(node);
			}
			else
			{
				this.WriteKeyword("DirectCast");
				this.WriteToken("(");
				this.WriteCastExpression(node);
				this.WriteToken(",");
				this.WriteSpace();
				this.WriteReferenceAndNamespaceIfInCollision(node.get_TargetType());
			}
			this.WriteToken(")");
			return;
		}

		public override void VisitFieldInitializerExpression(FieldInitializerExpression node)
		{
			this.WriteToken(".");
			this.VisitFieldInitializerExpression(node);
			return;
		}

		public override void VisitForEachStatement(ForEachStatement node)
		{
			this.statementStates.Push(3);
			this.WriteKeyword(this.get_KeyWordWriter().get_ForEach());
			this.WriteSpace();
			this.statementStates.Push(0);
			this.Visit(node.get_Variable());
			dummyVar0 = this.statementStates.Pop();
			this.WriteSpace();
			this.WriteKeyword(this.get_KeyWordWriter().get_In());
			this.WriteSpace();
			this.Visit(node.get_Collection());
			this.WriteLine();
			this.Visit(node.get_Body());
			this.WriteKeyword(this.get_KeyWordWriter().get_Next());
			dummyVar1 = this.statementStates.Pop();
			return;
		}

		public override void VisitForStatement(ForStatement node)
		{
			V_0 = node.get_Condition() as BinaryExpression;
			V_1 = node.get_Increment();
			if (V_0 == null || V_1 == null)
			{
				throw new Exception("Unexpected null value.");
			}
			if (V_0.get_IsAssignmentExpression())
			{
				return;
			}
			this.statementStates.Push(2);
			V_2 = this.GetForStep(V_1);
			this.WriteKeyword("For");
			this.WriteSpace();
			this.statementStates.Push(0);
			this.Visit(node.get_Initializer());
			dummyVar0 = this.statementStates.Pop();
			this.WriteSpace();
			this.WriteKeyword("To");
			this.WriteSpace();
			this.Visit(V_0.get_Right());
			if (V_2 != null)
			{
				this.WriteSpace();
				this.WriteKeyword("Step");
				this.WriteSpace();
				this.Visit(V_2);
			}
			this.WriteLine();
			this.Visit(node.get_Body());
			this.WriteKeyword(this.get_KeyWordWriter().get_Next());
			this.WriteLine();
			dummyVar1 = this.statementStates.Pop();
			return;
		}

		public override void VisitIfElseIfStatement(IfElseIfStatement node)
		{
			V_0 = 0;
			while (V_0 < node.get_ConditionBlocks().get_Count())
			{
				if (V_0 != 0)
				{
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
			this.WriteKeyword(this.get_KeyWordWriter().get_Else());
			this.WriteLine();
			this.Visit(node.get_Else());
			this.WriteSpecialEndBlock(this.get_KeyWordWriter().get_If());
			return;
		}

		public override void VisitLambdaExpression(LambdaExpression node)
		{
			this.VisitLambdaExpression(node);
			if (node.get_IsFunction())
			{
				stackVariable6 = this.get_KeyWordWriter().get_Function();
			}
			else
			{
				stackVariable6 = this.get_KeyWordWriter().get_Sub();
			}
			V_0 = stackVariable6;
			this.WriteKeyword(V_0);
			this.WriteToken("(");
			this.VisitMethodParameters(node.get_Arguments());
			this.WriteToken(")");
			V_1 = false;
			if (node.get_Body().get_Statements().get_Count() != 1)
			{
				this.get_ShouldOmitSemicolon().Push(false);
				this.WriteLine();
				this.Visit(node.get_Body());
				V_2 = true;
			}
			else
			{
				if (node.get_Body().get_Statements().get_Item(0).get_CodeNodeType() != 5)
				{
					this.get_ShouldOmitSemicolon().Push(false);
				}
				else
				{
					V_1 = true;
					this.get_ShouldOmitSemicolon().Push(true);
				}
				if (!V_1)
				{
					this.WriteLine();
				}
				else
				{
					this.WriteSpace();
				}
				this.Visit(node.get_Body().get_Statements().get_Item(0));
				V_2 = false;
			}
			if (!V_1)
			{
				if (!V_2)
				{
					this.WriteLine();
				}
				this.WriteEndBlock(V_0);
			}
			dummyVar0 = this.get_ShouldOmitSemicolon().Pop();
			return;
		}

		public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			V_0 = node.get_MethodExpression().get_Method() as GenericInstanceMethod;
			if (node.get_MethodExpression().get_Method().get_HasThis() || node.get_Arguments().get_Count() == 0 || V_0 == null || node.get_MethodExpression().get_MethodDefinition() == null || !node.get_MethodExpression().get_MethodDefinition().get_IsExtensionMethod())
			{
				this.VisitMethodInvocationExpression(node);
				return;
			}
			this.WriteMethodTarget(node.get_Arguments().get_Item(0));
			this.WriteGenericInstanceMethodWithArguments(V_0, this.GetGenericExtensionMethodArguments(V_0));
			this.WriteToken("(");
			this.VisitExtensionMethodParameters(node.get_Arguments());
			this.WriteToken(")");
			return;
		}

		private void VisitNullCoalesceExpression(BinaryExpression node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_If());
			this.WriteToken("(");
			this.Visit(node.get_Left());
			this.WriteToken(",");
			this.WriteSpace();
			this.Visit(node.get_Right());
			this.WriteToken(")");
			return;
		}

		public override void VisitPropertyInitializerExpression(PropertyInitializerExpression node)
		{
			this.WriteToken(".");
			this.VisitPropertyInitializerExpression(node);
			return;
		}

		public override void VisitPropertyReferenceExpression(PropertyReferenceExpression node)
		{
			if (node.get_Target() == null || node.get_Target().get_CodeNodeType() != 29 || !node.get_IsIndexer())
			{
				this.VisitPropertyReferenceExpression(node);
				return;
			}
			this.Visit(node.get_Target());
			this.WriteToken(".");
			this.WritePropertyName(node.get_Property());
			this.WriteIndexerArguments(node);
			return;
		}

		public override void VisitRaiseEventExpression(RaiseEventExpression node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_Fire());
			this.WriteSpace();
			this.WriteReference(node.get_Event().get_Name(), node.get_Event());
			this.EnterMethodInvocation(node.get_InvokeMethodReference());
			this.Write("(");
			this.VisitMethodParameters(node.get_Arguments());
			this.Write(")");
			this.LeaveMethodInvocation();
			return;
		}

		public override void VisitSafeCastExpression(SafeCastExpression node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_TryCast());
			this.WriteToken("(");
			this.Visit(node.get_Expression());
			this.WriteToken(",");
			this.WriteSpace();
			this.WriteReferenceAndNamespaceIfInCollision(node.get_TargetType());
			this.WriteToken(")");
			return;
		}

		public override void VisitSwitchStatement(SwitchStatement node)
		{
			V_0 = new VisualBasicWriter.u003cu003ec__DisplayClass71_0();
			V_0.u003cu003e4__this = this;
			V_0.node = node;
			this.statementStates.Push(6);
			this.WriteKeyword(this.get_KeyWordWriter().get_Switch());
			this.WriteSpace();
			this.WriteKeyword(this.get_KeyWordWriter().get_Case());
			this.WriteSpace();
			this.Visit(V_0.node.get_Condition());
			this.WriteLine();
			this.WriteBlock(new Action(V_0.u003cVisitSwitchStatementu003eb__0), "");
			this.WriteEndBlock(this.get_KeyWordWriter().get_Switch());
			dummyVar0 = this.statementStates.Pop();
			return;
		}

		public override void VisitThisCtorExpression(ThisCtorExpression node)
		{
			this.VisitCtorExpression(node, "MyClass");
			return;
		}

		public override void VisitTryStatement(TryStatement node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_Try());
			this.WriteLine();
			this.Visit(node.get_Try());
			V_0 = node.get_CatchClauses().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					this.Visit(V_1);
				}
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
			if (node.get_Finally() != null)
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_Finally());
				this.WriteLine();
				this.Visit(node.get_Finally());
			}
			this.WriteSpecialEndBlock(this.get_KeyWordWriter().get_Try());
			return;
		}

		public override void VisitUnaryExpression(UnaryExpression node)
		{
			if (node.get_Operator() == UnaryOperator.Negate || node.get_Operator() == 1 || node.get_Operator() == 2 || node.get_Operator() == 10)
			{
				this.Write(this.ToString(node.get_Operator()));
				this.VisitUnaryExpression(node);
				return;
			}
			if (node.get_Operator() == 9)
			{
				this.VisitAddressOfExpression(node);
				return;
			}
			if (node.get_Operator() == 8)
			{
				this.VisitUnaryExpression(node);
				return;
			}
			stackVariable27 = this.IsPostUnaryOperator(node.get_Operator());
			V_0 = false;
			if (node.get_Operator() == 3 || node.get_Operator() == 4 || node.get_Operator() == 5 || node.get_Operator() == 6)
			{
				V_0 = true;
				this.Visit(node.get_Operand());
				this.WriteSpace();
				this.WriteToken("=");
				this.WriteSpace();
			}
			if (!stackVariable27)
			{
				this.Write(this.ToString(node.get_Operator()));
			}
			if (V_0)
			{
				stackVariable44 = node.get_Operand().CloneExpressionOnly();
			}
			else
			{
				stackVariable44 = node.get_Operand();
			}
			this.Visit(stackVariable44);
			if (stackVariable27)
			{
				this.Write(this.ToString(node.get_Operator()));
			}
			return;
		}

		public override void VisitUsingStatement(UsingStatement node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_Using());
			this.WriteSpace();
			this.statementStates.Push(8);
			this.WriteSpecialBetweenParenthesis(node.get_Expression());
			dummyVar0 = this.statementStates.Pop();
			this.WriteLine();
			this.Visit(node.get_Body());
			this.WriteSpecialEndBlock(this.get_KeyWordWriter().get_Using());
			return;
		}

		public override void VisitVariableDeclarationExpression(VariableDeclarationExpression node)
		{
			V_0 = new VisualBasicWriter.u003cu003ec__DisplayClass110_0();
			V_0.u003cu003e4__this = this;
			if (node.get_Variable().get_VariableType().get_IsOptionalModifier() || node.get_Variable().get_VariableType().get_IsRequiredModifier() && !this.isWritingComment)
			{
				this.StartWritingComment();
				this.VisitVariableDeclarationExpression(node);
				this.EndWritingComment();
				this.WriteLine();
			}
			this.WriteDim();
			V_0.variable = node.get_Variable();
			if (!V_0.variable.get_VariableType().ContainsAnonymousType())
			{
				this.VisitVariableDeclarationExpression(node);
				return;
			}
			this.WriteAndMapVariableToCode(new Action(V_0.u003cVisitVariableDeclarationExpressionu003eb__0), node.get_Variable());
			return;
		}

		private void VisitVBSwitchCases(IEnumerable collection)
		{
			V_0 = collection.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = (ICodeNode)V_0.get_Current();
					this.Visit(V_1);
				}
			}
			finally
			{
				V_2 = V_0 as IDisposable;
				if (V_2 != null)
				{
					V_2.Dispose();
				}
			}
			return;
		}

		public override void VisitWhileStatement(WhileStatement node)
		{
			this.statementStates.Push(4);
			this.VisitWhileStatement(node);
			dummyVar0 = this.statementStates.Pop();
			return;
		}

		public override void VisitYieldBreakExpression(YieldBreakExpression node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_Return());
			return;
		}

		public override void VisitYieldReturnExpression(YieldReturnExpression node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_Return());
			this.WriteSpace();
			this.WriteKeyword(this.get_KeyWordWriter().get_New());
			this.WriteSpace();
			V_0 = this.GetExpressionType(node.get_Expression());
			if (V_0 == null)
			{
				V_1 = "Object";
				if (node.get_Expression().get_CodeNodeType() == 22)
				{
					V_1 = this.GetExpressionType((LiteralExpression)node.get_Expression());
				}
				this.WriteKeyword(V_1);
			}
			else
			{
				this.WriteReferenceAndNamespaceIfInCollision(V_0);
			}
			this.WriteToken("(");
			this.WriteToken(")");
			this.WriteSpace();
			this.WriteToken("{");
			this.WriteSpace();
			this.Visit(node.get_Expression());
			this.WriteSpace();
			this.WriteToken("}");
			return;
		}

		protected override void Write(PropertyDefinition property)
		{
			if (!property.IsAbstract() && !this.get_TypeContext().get_AutoImplementedProperties().Contains(property))
			{
				this.Write(property);
				return;
			}
			this.WritePropertyDeclaration(property);
			this.currentWritingInfo.get_MemberDefinitionToFoldingPositionMap().set_Item(property, new OffsetSpan(this.formatter.get_CurrentPosition(), this.formatter.get_CurrentPosition()));
			return;
		}

		protected override void WriteArrayDimensions(ExpressionCollection dimensions, TypeReference arrayType, bool isInitializerPresent)
		{
			V_0 = dimensions.Clone();
			V_1 = this.get_ModuleContext().get_Module().get_TypeSystem();
			V_2 = 0;
			while (V_2 < V_0.get_Count())
			{
				if (V_0.get_Item(V_2) as LiteralExpression == null)
				{
					V_0.set_Item(V_2, new BinaryExpression(3, V_0.get_Item(V_2), new LiteralExpression((object)1, V_1, null), V_1, null, false));
				}
				else
				{
					V_3 = V_0.get_Item(V_2) as LiteralExpression;
					V_3.set_Value(this.GetDecrementedValue(V_3));
				}
				V_2 = V_2 + 1;
			}
			this.WriteArrayDimensions(V_0, arrayType, isInitializerPresent);
			return;
		}

		private void WriteAsBetweenSpaces()
		{
			this.WriteSpace();
			this.WriteKeyword(this.get_KeyWordWriter().get_As());
			this.WriteSpace();
			return;
		}

		protected override void WriteBaseConstructorInvokation(MethodInvocationExpression baseConstructorInvokation)
		{
			if (!this.get_MethodContext().get_Method().get_IsStatic())
			{
				this.WriteLine();
				this.Indent();
				this.Visit(this.get_MethodContext().get_CtorInvokeExpression());
				this.Outdent();
			}
			return;
		}

		protected override void WriteBaseTypeInheritColon()
		{
			this.WriteLine();
			this.WriteKeyword("Inherits");
			this.WriteSpace();
			return;
		}

		protected override void WriteBlock(Action action, string label)
		{
			this.Indent();
			if (String.op_Inequality(label, ""))
			{
				this.WriteLabel(label);
			}
			action.Invoke();
			this.Outdent();
			return;
		}

		private void WriteCastExpression(ExplicitCastExpression node)
		{
			stackVariable3 = this.IsComplexTarget(node.get_Expression());
			if (stackVariable3)
			{
				this.WriteToken("(");
			}
			this.Visit(node.get_Expression());
			if (stackVariable3)
			{
				this.WriteToken(")");
			}
			return;
		}

		protected override void WriteDelegateCreation(ObjectCreationExpression node)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_New());
			this.WriteSpace();
			this.WriteReferenceAndNamespaceIfInCollision(node.get_ExpressionType());
			this.WriteToken("(");
			this.WriteKeyword(this.get_KeyWordWriter().get_AddressOf());
			this.WriteSpace();
			this.WriteDelegateArgument(node);
			this.WriteToken(")");
			return;
		}

		protected override void WriteDestructor(MethodDefinition method)
		{
			this.WriteMethod(method);
			return;
		}

		protected void WriteDim()
		{
			if (this.statementStates.get_Count() > 0)
			{
				V_0 = this.statementStates.Peek();
				if (V_0 == StatementState.ForEachInitializer || V_0 == 1 || V_0 == 7 || V_0 == 8)
				{
					return;
				}
			}
			this.WriteKeyword(this.get_KeyWordWriter().get_Dim());
			this.WriteSpace();
			return;
		}

		protected override void WriteEmptyMethodEndOfStatement(MethodDefinition method)
		{
			if (!method.get_IsAbstract())
			{
				this.WriteLine();
				this.WriteSpecialEndBlock(this.GetMethodKeyWord(method));
			}
			return;
		}

		protected override void WriteEndBlock(string statementName)
		{
			this.WriteEndBlockWithoutNewLine(statementName);
			return;
		}

		private void WriteEndBlockWithoutNewLine(string statementName)
		{
			this.WriteKeyword("End");
			this.WriteSpace();
			this.WriteKeyword(statementName);
			return;
		}

		protected override void WriteEnumBaseTypeInheritColon()
		{
			this.WriteAsBetweenSpaces();
			return;
		}

		protected override void WriteEscapeCharLiteral(char c)
		{
			V_0 = null;
			switch (c)
			{
				case 0:
				case 7:
				case 8:
				case 9:
				case 10:
				case 11:
				case 12:
				case 13:
				{
					V_0 = new int?(c);
					goto Label0;
				}
				case 1:
				case 2:
				case 3:
				case 4:
				case 5:
				case 6:
				{
				Label0:
					V_1 = Char.GetUnicodeCategory(c);
					V_2 = false;
					if (V_1 == 21 || V_1 == 26 || V_1 == 19 || V_1 == 8 || V_1 == 23 || V_1 == 22 || V_1 == 9 || V_1 == 1 || V_1 == 25 || V_1 == 20 || V_1 == 2 || V_1 == UnicodeCategory.UppercaseLetter || V_1 == 24)
					{
						V_2 = true;
					}
					if (!V_2)
					{
						V_0 = new int?(c);
					}
					if (V_0.get_HasValue())
					{
						this.Write("Strings.ChrW(");
						this.WriteLiteral(V_0.ToString());
						this.Write(")");
						return;
					}
					this.WriteLiteral(this.get_CharStart());
					this.formatter.WriteLiteral(c.ToString());
					this.WriteLiteral(this.get_CharEnd());
					return;
				}
				default:
				{
					if (c != '\"')
					{
						goto Label0;
					}
					this.WriteLiteral(this.get_CharStart());
					this.formatter.WriteLiteral("\"\"");
					this.WriteLiteral(this.get_CharEnd());
					return;
				}
			}
		}

		protected override void WriteEventAddOnParameters(EventDefinition event)
		{
			if (event.get_AddMethod().get_Parameters().Any<ParameterDefinition>())
			{
				this.Write("(");
				this.WriteParameters(event.get_AddMethod());
				this.Write(")");
			}
			return;
		}

		protected override void WriteEventInterfaceImplementations(EventDefinition event)
		{
			V_0 = event.GetImplementedEvents();
			if (!event.IsExplicitImplementation())
			{
				V_1 = this.GetNotExplicitlyImplementedMembers(V_0);
				if (V_1.Any<ImplementedMember>())
				{
					this.WriteImplementedMembers(V_1);
				}
			}
			else
			{
				if (V_0.Any<ImplementedMember>())
				{
					this.WriteImplementedMembers(V_0);
					return;
				}
			}
			return;
		}

		protected override void WriteEventRemoveOnParameters(EventDefinition event)
		{
			if (event.get_RemoveMethod().get_Parameters().Any<ParameterDefinition>())
			{
				this.Write("(");
				this.WriteParameters(event.get_RemoveMethod());
				this.Write(")");
			}
			return;
		}

		protected override void WriteEventTypeAndName(EventDefinition event)
		{
			if (!event.IsExplicitImplementation())
			{
				this.WriteEventTypeAndName(event);
				return;
			}
			V_0 = this.GetEventName(event);
			this.WriteTypeAndName(event.get_EventType(), V_0, event);
			return;
		}

		protected override void WriteFieldDeclaration(FieldDefinition field)
		{
			if (field.get_FieldType().get_IsOptionalModifier() || field.get_FieldType().get_IsRequiredModifier())
			{
				this.StartWritingComment();
				this.WriteFieldDeclaration(field);
				this.EndWritingComment();
				this.WriteLine();
			}
			this.WriteFieldDeclaration(field);
			return;
		}

		protected override void WriteGenericParameterConstraints(GenericParameter parameter)
		{
			stackVariable2 = this.CountConstraints(parameter);
			this.WriteSpace();
			this.WriteKeyword(this.get_KeyWordWriter().get_As());
			this.WriteSpace();
			if (stackVariable2 > 1)
			{
				this.WriteToken("{");
			}
			this.WriteSingleGenericParameterConstraintsList(parameter);
			if (stackVariable2 > 1)
			{
				this.WriteToken("}");
			}
			return;
		}

		private void WriteImplementedMembers(ICollection<ImplementedMember> implementedMembers)
		{
			if (!this.get_TypeContext().get_IsWinRTImplementation())
			{
				V_0 = implementedMembers.ToList<ImplementedMember>();
			}
			else
			{
				stackVariable36 = implementedMembers;
				stackVariable37 = VisualBasicWriter.u003cu003ec.u003cu003e9__33_0;
				if (stackVariable37 == null)
				{
					dummyVar0 = stackVariable37;
					stackVariable37 = new Func<ImplementedMember, bool>(VisualBasicWriter.u003cu003ec.u003cu003e9.u003cWriteImplementedMembersu003eb__33_0);
					VisualBasicWriter.u003cu003ec.u003cu003e9__33_0 = stackVariable37;
				}
				V_0 = stackVariable36.Where<ImplementedMember>(stackVariable37).ToList<ImplementedMember>();
				if (V_0.get_Count() == 0)
				{
					return;
				}
			}
			this.WriteSpace();
			this.WriteKeyword(this.get_KeyWordWriter().get_Implements());
			this.WriteSpace();
			V_1 = true;
			V_2 = V_0.GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					if (!V_1)
					{
						this.Write(",");
						this.WriteSpace();
					}
					this.WriteReferenceAndNamespaceIfInCollision(V_3.get_DeclaringType());
					this.Write(".");
					V_4 = this.GetMemberName(V_3.get_Member());
					this.WriteReference(V_4, V_3.get_Member());
					V_1 = false;
				}
			}
			finally
			{
				((IDisposable)V_2).Dispose();
			}
			return;
		}

		protected override void WriteIndexerKeywords()
		{
			this.WriteKeyword("Default");
			this.WriteSpace();
			return;
		}

		private void WriteInnermostParentFrom(Dictionary<StatementState, string> parents)
		{
			V_0 = this.statementStates.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (!parents.ContainsKey(V_1))
					{
						continue;
					}
					this.WriteKeyword(parents.get_Item(V_1));
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
		Label1:
			return;
		Label0:
			throw new Exception("Suitable parent for Continue/Exit statement not found.");
		}

		protected override void WriteInterfacesInheritColon(TypeDefinition type)
		{
			this.WriteLine();
			if (!type.get_IsInterface())
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_Implements());
			}
			else
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_Inherits());
			}
			this.WriteSpace();
			return;
		}

		public override void WriteMemberNavigationPathFullName(object member)
		{
			if (member as ParameterReference != null)
			{
				this.formatter.Write(((ParameterReference)member).get_Name());
				return;
			}
			if (member as TypeReference != null)
			{
				this.formatter.Write(((TypeReference)member).GetUIFriendlyTypeNameInVB(this.get_Language()));
				return;
			}
			if (member as MemberReference != null)
			{
				this.formatter.Write(((MemberReference)member).GetFriendlyFullNameInVB(this.get_Language()));
			}
			return;
		}

		protected override void WriteMethodDeclaration(MethodDefinition method, bool writeDocumentation)
		{
			V_0 = false;
			if (method.get_ReturnType().get_IsOptionalModifier() || method.get_ReturnType().get_IsRequiredModifier())
			{
				V_0 = true;
			}
			V_1 = 0;
			while (V_1 < method.get_Parameters().get_Count() && !V_0)
			{
				V_2 = method.get_Parameters().get_Item(V_1);
				if (V_2.get_ParameterType().get_IsOptionalModifier() || V_2.get_ParameterType().get_IsRequiredModifier())
				{
					V_0 = true;
				}
				V_1 = V_1 + 1;
			}
			if (V_0)
			{
				this.StartWritingComment();
				this.WriteMethodDeclaration(method, writeDocumentation);
				this.EndWritingComment();
				this.WriteLine();
			}
			this.WriteMethodDeclaration(method, writeDocumentation);
			return;
		}

		protected override void WriteMethodInterfaceImplementations(MethodDefinition method)
		{
			V_0 = method.GetImplementedMethods();
			if (!method.IsExplicitImplementation())
			{
				V_1 = this.GetNotExplicitlyImplementedMembers(V_0);
				if (V_1.Any<ImplementedMember>())
				{
					this.WriteImplementedMembers(V_1);
				}
			}
			else
			{
				if (V_0.Any<ImplementedMember>())
				{
					this.WriteImplementedMembers(V_0);
					return;
				}
			}
			return;
		}

		protected override void WriteOptional(ParameterDefinition parameter)
		{
			this.WriteKeyword("Optional");
			this.WriteSpace();
			return;
		}

		protected override void WriteOutOrRefKeyWord(ParameterDefinition parameter)
		{
			this.WriteKeyword(this.get_KeyWordWriter().get_ByRef());
			return;
		}

		protected override bool WritePropertyAsIndexer(PropertyDefinition property)
		{
			return false;
		}

		protected override void WritePropertyInterfaceImplementations(PropertyDefinition property)
		{
			V_0 = property.GetImplementedProperties();
			if (!property.IsExplicitImplementation())
			{
				V_1 = this.GetNotExplicitlyImplementedMembers(V_0);
				if (V_1.Any<ImplementedMember>())
				{
					this.WriteImplementedMembers(V_1);
				}
			}
			else
			{
				if (V_0.Any<ImplementedMember>())
				{
					this.WriteImplementedMembers(V_0);
					return;
				}
			}
			return;
		}

		protected override void WritePropertyTypeAndNameWithArguments(PropertyDefinition property)
		{
			V_0 = this.formatter.get_CurrentPosition();
			this.WriteReference(this.GetPropertyName(property), property);
			V_2 = this.formatter.get_CurrentPosition() - 1;
			this.currentWritingInfo.get_MemberDeclarationToCodePostionMap().set_Item(property, new OffsetSpan(V_0, V_2));
			if (this.HasArguments(property))
			{
				this.WriteToken("(");
				this.WritePropertyParameters(property);
				this.WriteToken(")");
			}
			this.WriteAsBetweenSpaces();
			this.get_AttributeWriter().WriteMemberReturnValueAttributes(property.get_GetMethod());
			this.WriteReferenceAndNamespaceIfInCollision(property.get_PropertyType());
			return;
		}

		protected override void WriteReadOnlyWriteOnlyProperty(PropertyDefinition property)
		{
			if (property.get_GetMethod() == null)
			{
				this.WriteKeyword(this.get_KeyWordWriter().get_WriteOnly());
				this.WriteSpace();
				return;
			}
			if (property.get_SetMethod() != null)
			{
				return;
			}
			this.WriteKeyword(this.get_KeyWordWriter().get_ReadOnly());
			this.WriteSpace();
			return;
		}

		protected override void WriteSpecialBetweenParenthesis(Expression expression)
		{
			this.Visit(expression);
			return;
		}

		protected override void WriteSpecialBetweenParenthesis(Action action)
		{
			this.WriteToken("(");
			action.Invoke();
			this.WriteToken(")");
			return;
		}

		protected override void WriteSpecialEndBlock(string statementName)
		{
			this.WriteEndBlock(statementName);
			return;
		}

		protected override bool WriteTypeBaseTypes(TypeDefinition type, bool isPartial = false)
		{
			this.Indent();
			stackVariable4 = this.WriteTypeBaseTypes(type, isPartial);
			this.Outdent();
			return stackVariable4;
		}

		protected override string WriteTypeDeclaration(TypeDefinition type, bool isPartial = false)
		{
			if (type.get_IsNested() && type.get_IsStaticClass())
			{
				throw new Exception("VB.NET does not support nested modules. Please, try using other language.");
			}
			return this.WriteTypeDeclaration(type, isPartial);
		}

		protected override void WriteTypeInterfaces(TypeDefinition type, bool isPartial, bool baseTypeWritten)
		{
			if ((object)this.get_TypeContext().get_CurrentType() != (object)type || !this.get_TypeContext().get_IsWinRTImplementation())
			{
				V_0 = type.get_Interfaces().ToList<TypeReference>();
			}
			else
			{
				stackVariable45 = type.get_Interfaces();
				stackVariable46 = VisualBasicWriter.u003cu003ec.u003cu003e9__18_0;
				if (stackVariable46 == null)
				{
					dummyVar0 = stackVariable46;
					stackVariable46 = new Func<TypeReference, bool>(VisualBasicWriter.u003cu003ec.u003cu003e9.u003cWriteTypeInterfacesu003eb__18_0);
					VisualBasicWriter.u003cu003ec.u003cu003e9__18_0 = stackVariable46;
				}
				V_0 = stackVariable45.Where<TypeReference>(stackVariable46).ToList<TypeReference>();
			}
			if (V_0.get_Count() > 0)
			{
				V_1 = 0;
				V_2 = 0;
				while (V_2 < V_0.get_Count())
				{
					if (!isPartial || this.IsImplemented(type, V_0.get_Item(V_2).Resolve()))
					{
						if (V_1 == 0)
						{
							this.Indent();
							this.WriteInterfacesInheritColon(type);
							this.Outdent();
						}
						if (V_1 > 0)
						{
							this.WriteInheritComma();
						}
						V_1 = V_1 + 1;
						this.WriteReferenceAndNamespaceIfInCollision(V_0.get_Item(V_2));
					}
					V_2 = V_2 + 1;
				}
			}
			return;
		}

		protected override void WriteTypeSpecification(TypeSpecification typeSpecification, int startingArgument = 0)
		{
			if (!typeSpecification.get_IsOptionalModifier() && !typeSpecification.get_IsRequiredModifier() || this.isWritingComment)
			{
				this.WriteTypeSpecification(typeSpecification, startingArgument);
				return;
			}
			this.WriteReferenceAndNamespaceIfInCollision(typeSpecification.get_ElementType());
			return;
		}

		protected override void WriteTypeStaticKeywordAndSpace()
		{
			return;
		}

		protected override void WriteVolatileType(TypeReference reference)
		{
			this.WriteReferenceAndNamespaceIfInCollision(reference);
			this.WriteSpace();
			this.WriteKeyword(this.get_KeyWordWriter().get_Volatile());
			return;
		}
	}
}