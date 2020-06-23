using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Common;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Languages.VisualBasic
{
	public class VisualBasicWriter : NamespaceImperativeLanguageWriter
	{
		private readonly Stack<StatementState> statementStates = new Stack<StatementState>();

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

		public VisualBasicWriter(ILanguage language, IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings) : base(language, formatter, exceptionFormatter, settings)
		{
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
			int count = 0;
			if (parameter.HasConstraints)
			{
				count += parameter.Constraints.Count;
			}
			if (parameter.HasDefaultConstructorConstraint)
			{
				count++;
			}
			if (parameter.HasReferenceTypeConstraint)
			{
				count++;
			}
			if (parameter.HasNotNullableValueTypeConstraint)
			{
				count++;
			}
			return count;
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
			string str = name;
			if (reference.IsParamArray())
			{
				this.WriteKeyword(base.KeyWordWriter.ParamArray);
				this.WriteSpace();
			}
			if (!base.Language.IsValidIdentifier(str))
			{
				str = base.Language.ReplaceInvalidCharactersInIdentifier(str);
			}
			if (base.Language.IsGlobalKeyword(str))
			{
				str = Utilities.EscapeNameIfNeeded(str, base.Language);
			}
			if (!this.isWritingComment)
			{
				base.WriteAndMapParameterToCode(() => this.Write(str), reference.Index);
			}
			else
			{
				this.Write(str);
			}
			if (!String.IsNullOrEmpty(this.ToTypeString(type)))
			{
				this.WriteAsBetweenSpaces();
				this.WriteReferenceAndNamespaceIfInCollision(type);
			}
		}

		protected override void DoWriteTypeAndName(TypeReference typeReference, string name, object reference)
		{
			int currentPosition = this.formatter.CurrentPosition;
			this.WriteReference(name, reference);
			if (reference is IMemberDefinition)
			{
				int num = this.formatter.CurrentPosition - 1;
				this.currentWritingInfo.MemberDeclarationToCodePostionMap[(IMemberDefinition)reference] = new OffsetSpan(currentPosition, num);
			}
			this.WriteAsBetweenSpaces();
			this.WriteReferenceAndNamespaceIfInCollision(typeReference);
		}

		protected override void DoWriteTypeAndName(TypeReference typeReference, string name)
		{
			this.Write(name);
			this.WriteAsBetweenSpaces();
			this.WriteReferenceAndNamespaceIfInCollision(typeReference);
		}

		protected override void DoWriteVariableTypeAndName(VariableDefinition variable)
		{
			if (!this.isWritingComment)
			{
				base.WriteAndMapVariableToCode(() => this.Write(base.GetVariableName(variable)), variable);
			}
			else
			{
				this.Write(base.GetVariableName(variable));
			}
			this.WriteAsBetweenSpaces();
			this.WriteReferenceAndNamespaceIfInCollision(variable.VariableType);
		}

		private string GetCastMethod(TypeReference type)
		{
			string name = type.Name;
			if (name != null)
			{
				if (name == "Decimal")
				{
					return "CDec";
				}
				if (name == "Single")
				{
					return "CSng";
				}
				if (name == "Byte")
				{
					return "CByte";
				}
				if (name == "SByte")
				{
					return "CSByte";
				}
				if (name == "Char")
				{
					return "CChar";
				}
				if (name == "Double")
				{
					return "CDbl";
				}
				if (name == "Boolean")
				{
					return "CBool";
				}
				if (name == "Int16")
				{
					return "CShort";
				}
				if (name == "UInt16")
				{
					return "CUShort";
				}
				if (name == "Int32")
				{
					return "CInt";
				}
				if (name == "UInt32")
				{
					return "CUInt";
				}
				if (name == "Int64")
				{
					return "CLng";
				}
				if (name == "UInt64")
				{
					return "CULng";
				}
				if (name == "String")
				{
					return "CStr";
				}
				if (name == "Object")
				{
					return "CObj";
				}
				if (name == "RuntimeArgumentHandle")
				{
					return null;
				}
			}
			return null;
		}

		private Dictionary<StatementState, string> GetContinuableParents()
		{
			return new Dictionary<StatementState, string>()
			{
				{ StatementState.DoWhile, "Do" },
				{ StatementState.For, "For" },
				{ StatementState.ForEach, "For" },
				{ StatementState.While, "While" }
			};
		}

		private object GetDecrementedValue(LiteralExpression expression)
		{
			string name = expression.ExpressionType.Name;
			if (name != null)
			{
				if (name == "Byte")
				{
					return (byte)((Byte)expression.Value - 1);
				}
				if (name == "SByte")
				{
					return (sbyte)((SByte)expression.Value - 1);
				}
				if (name == "Int16")
				{
					return (short)((Int16)expression.Value - 1);
				}
				if (name == "UInt16")
				{
					return (ushort)((UInt16)expression.Value - 1);
				}
				if (name == "Int32")
				{
					return (Int32)expression.Value - 1;
				}
				if (name == "UInt32")
				{
					return (UInt32)expression.Value - 1;
				}
				if (name == "Int64")
				{
					return (Int64)expression.Value - (long)1;
				}
				if (name == "UInt64")
				{
					return (UInt64)expression.Value - (long)1;
				}
				if (name == "Char")
				{
					return (char)((Char)expression.Value - 1);
				}
			}
			throw new ArgumentException("Invalid data type for dimension of an array.");
		}

		private Dictionary<StatementState, string> GetExitableParents()
		{
			return new Dictionary<StatementState, string>(this.GetContinuableParents())
			{
				{ StatementState.Switch, "Select" }
			};
		}

		private TypeReference GetExpressionType(Expression expression)
		{
			CodeNodeType codeNodeType = expression.CodeNodeType;
			if (codeNodeType > CodeNodeType.VariableReferenceExpression)
			{
				if (codeNodeType == CodeNodeType.FieldReferenceExpression)
				{
					return ((FieldReferenceExpression)expression).Field.FieldType;
				}
				if (codeNodeType == CodeNodeType.PropertyReferenceExpression)
				{
					return ((PropertyReferenceExpression)expression).Property.PropertyType;
				}
			}
			else if (codeNodeType == CodeNodeType.MethodInvocationExpression)
			{
				MethodReferenceExpression methodExpression = ((MethodInvocationExpression)expression).MethodExpression;
				if (methodExpression.CodeNodeType == CodeNodeType.MethodReferenceExpression)
				{
					return methodExpression.Method.ReturnType;
				}
			}
			else if (codeNodeType == CodeNodeType.VariableReferenceExpression)
			{
				return ((VariableReferenceExpression)expression).Variable.VariableType;
			}
			return null;
		}

		private string GetExpressionType(LiteralExpression literalExpression)
		{
			if (literalExpression.Value is Decimal)
			{
				return "Decimal";
			}
			if (literalExpression.Value is Single)
			{
				return "Float";
			}
			if (literalExpression.Value is Byte)
			{
				return "Byte";
			}
			if (literalExpression.Value is SByte)
			{
				return "SByte";
			}
			if (literalExpression.Value is Char)
			{
				return "Char";
			}
			if (literalExpression.Value is Double)
			{
				return "Double";
			}
			if (literalExpression.Value is Boolean)
			{
				return "Boolean";
			}
			if (literalExpression.Value is Int16)
			{
				return "Short";
			}
			if (literalExpression.Value is Int32)
			{
				return "Integer";
			}
			if (literalExpression.Value is Int64)
			{
				return "Long";
			}
			if (literalExpression.Value is UInt16)
			{
				return "UShort";
			}
			if (literalExpression.Value is UInt32)
			{
				return "UInteger";
			}
			if (literalExpression.Value is UInt64)
			{
				return "ULong";
			}
			if (literalExpression.Value is String)
			{
				return "String";
			}
			return "Object";
		}

		private Expression GetForStep(Expression incrementExpression)
		{
			BinaryExpression right;
			if (incrementExpression.CodeNodeType == CodeNodeType.UnaryExpression)
			{
				UnaryExpression unaryExpression = (UnaryExpression)incrementExpression;
				if (unaryExpression.Operator == UnaryOperator.PostDecrement || unaryExpression.Operator == UnaryOperator.PreDecrement)
				{
					return new LiteralExpression((object)-1, base.MethodContext.Method.Module.TypeSystem, null);
				}
			}
			if (incrementExpression.CodeNodeType == CodeNodeType.BinaryExpression && (incrementExpression as BinaryExpression).Operator == BinaryOperator.SubtractAssign)
			{
				return this.GetNegatedExpression(((BinaryExpression)incrementExpression).Right);
			}
			if (incrementExpression.CodeNodeType == CodeNodeType.BinaryExpression && (incrementExpression as BinaryExpression).Operator == BinaryOperator.AddAssign)
			{
				return ((BinaryExpression)incrementExpression).Right;
			}
			if (incrementExpression.CodeNodeType == CodeNodeType.BinaryExpression && (incrementExpression as BinaryExpression).IsAssignmentExpression)
			{
				BinaryExpression binaryExpression = (BinaryExpression)incrementExpression;
				if (binaryExpression.Right.CodeNodeType != CodeNodeType.ExplicitCastExpression)
				{
					right = binaryExpression.Right as BinaryExpression;
				}
				else
				{
					ExplicitCastExpression expression = binaryExpression.Right as ExplicitCastExpression;
					while (expression.Expression.CodeNodeType == CodeNodeType.ExplicitCastExpression)
					{
						expression = expression.Expression as ExplicitCastExpression;
					}
					right = expression.Expression as BinaryExpression;
				}
				if (right != null)
				{
					Expression right1 = right.Right;
					if (right.Operator != BinaryOperator.Subtract)
					{
						return right1;
					}
					return this.GetNegatedExpression(right1);
				}
			}
			return null;
		}

		private Mono.Collections.Generic.Collection<TypeReference> GetGenericExtensionMethodArguments(GenericInstanceMethod genericMethod)
		{
			TypeReference parameterType = genericMethod.ElementMethod.Parameters[0].ParameterType;
			if (!parameterType.IsGenericInstance && !parameterType.IsGenericParameter)
			{
				return genericMethod.GenericArguments;
			}
			HashSet<GenericParameter> genericParameters = new HashSet<GenericParameter>();
			if (!parameterType.IsGenericInstance)
			{
				genericParameters.Add(parameterType as GenericParameter);
			}
			else
			{
				Queue<GenericInstanceType> genericInstanceTypes = new Queue<GenericInstanceType>();
				genericInstanceTypes.Enqueue(parameterType as GenericInstanceType);
				while (genericInstanceTypes.Count > 0)
				{
					foreach (TypeReference genericArgument in genericInstanceTypes.Dequeue().GenericArguments)
					{
						if (!genericArgument.IsGenericInstance)
						{
							if (!genericArgument.IsGenericParameter)
							{
								continue;
							}
							genericParameters.Add(genericArgument as GenericParameter);
						}
						else
						{
							genericInstanceTypes.Enqueue(genericArgument as GenericInstanceType);
						}
					}
				}
			}
			Mono.Collections.Generic.Collection<TypeReference> typeReferences = new Mono.Collections.Generic.Collection<TypeReference>();
			for (int i = 0; i < genericMethod.ElementMethod.GenericParameters.Count; i++)
			{
				if (!genericParameters.Contains(genericMethod.ElementMethod.GenericParameters[i]))
				{
					typeReferences.Add(genericMethod.GenericArguments[i]);
				}
			}
			return typeReferences;
		}

		private string GetGenericNameFromMemberReference(TypeReference type)
		{
			IGenericDefinition genericDefinition = type.Resolve();
			if (genericDefinition != null)
			{
				return genericDefinition.GetGenericName(base.Language, "(Of ", ")");
			}
			return type.GetGenericName(base.Language, "(Of ", ")");
		}

		private Expression GetNegatedExpression(Expression expression)
		{
			return new UnaryExpression(UnaryOperator.Negate, expression, null);
		}

		private ICollection<ImplementedMember> GetNotExplicitlyImplementedMembers(ICollection<ImplementedMember> implementedMembers)
		{
			ICollection<ImplementedMember> implementedMembers1 = new List<ImplementedMember>();
			foreach (ImplementedMember implementedMember in implementedMembers)
			{
				if (this.TypeContext.ExplicitlyImplementedMembers.Contains(implementedMember.DeclaringType, implementedMember.Member.FullName))
				{
					continue;
				}
				implementedMembers1.Add(implementedMember);
			}
			return implementedMembers1;
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
			if (node.Operator != BinaryOperator.Assign || node.Left.CodeNodeType != CodeNodeType.ArrayAssignmentFieldReferenceExpression)
			{
				return false;
			}
			return node.Right.CodeNodeType == CodeNodeType.ArrayCreationExpression;
		}

		private bool IsArrayCreationToVariableDeclarationAssignment(BinaryExpression node)
		{
			if (node.Operator != BinaryOperator.Assign || node.Left.CodeNodeType != CodeNodeType.ArrayVariableCreationExpression)
			{
				return false;
			}
			return node.Right.CodeNodeType == CodeNodeType.ArrayCreationExpression;
		}

		private bool IsArrayCreationToVariableReferenceAssignment(BinaryExpression node)
		{
			if (node.Operator != BinaryOperator.Assign || node.Left.CodeNodeType != CodeNodeType.ArrayAssignmentVariableReferenceExpression)
			{
				return false;
			}
			return node.Right.CodeNodeType == CodeNodeType.ArrayCreationExpression;
		}

		private bool IsMulticastDelegate(Expression expression)
		{
			if (expression is EventReferenceExpression)
			{
				return true;
			}
			TypeDefinition typeDefinition = expression.ExpressionType.Resolve();
			if (typeDefinition == null)
			{
				return false;
			}
			if (typeDefinition.BaseType != null && typeDefinition.BaseType.Name == "MulticastDelegate")
			{
				return true;
			}
			return false;
		}

		protected override string OnConvertString(string str)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string str1 = str;
			for (int i = 0; i < str1.Length; i++)
			{
				char chr = str1[i];
				if (chr != '\"')
				{
					stringBuilder.Append(this.ConvertCharOnVB(chr));
				}
				else
				{
					stringBuilder.Append("\"\"");
				}
			}
			return stringBuilder.ToString();
		}

		protected override void PostWriteGenericParametersConstraints(IGenericDefinition generic)
		{
			if (!generic.HasGenericParameters)
			{
				return;
			}
			foreach (GenericParameter genericParameter in generic.GenericParameters)
			{
				if (!base.IsTypeParameterRedeclaration(genericParameter) || !base.ShouldWriteConstraintsAsComment(genericParameter))
				{
					continue;
				}
				this.WriteLine();
				this.StartWritingComment();
				this.WriteToken("Of ");
				this.WriteReference(genericParameter.Name, null);
				this.WriteGenericParameterConstraints(genericParameter);
				this.EndWritingComment();
			}
		}

		protected override void PostWriteMethodReturnType(MethodDefinition method)
		{
			if (method.ReturnType != null && method.ReturnType.FullName != "System.Void")
			{
				this.WriteAsBetweenSpaces();
				base.AttributeWriter.WriteMemberReturnValueAttributes(method);
				this.WriteMethodReturnType(method);
			}
		}

		protected override void StartWritingComment()
		{
			this.WriteComment("");
			base.StartWritingComment();
		}

		internal override string ToEscapedTypeString(TypeReference reference)
		{
			if (!base.IsReferenceFromMscorlib(reference))
			{
				return this.ToTypeString(reference);
			}
			string typeString = this.ToTypeString(reference);
			if (typeString == "Enum" || typeString == "Delegate")
			{
				typeString = Utilities.Escape(typeString, base.Language);
			}
			return typeString;
		}

		protected override string ToString(BinaryOperator op, bool isOneSideNull = false)
		{
			switch (op)
			{
				case BinaryOperator.Add:
				{
					return "+";
				}
				case BinaryOperator.AddAssign:
				{
					return "+=";
				}
				case BinaryOperator.Subtract:
				{
					return "-";
				}
				case BinaryOperator.SubtractAssign:
				{
					return "-=";
				}
				case BinaryOperator.Multiply:
				{
					return "*";
				}
				case BinaryOperator.MultiplyAssign:
				{
					return "*=";
				}
				case BinaryOperator.Divide:
				{
					return "/";
				}
				case BinaryOperator.DivideAssign:
				{
					return "/=";
				}
				case BinaryOperator.ValueEquality:
				{
					if (isOneSideNull)
					{
						return "Is";
					}
					return "=";
				}
				case BinaryOperator.ValueInequality:
				{
					if (isOneSideNull)
					{
						return "IsNot";
					}
					return "<>";
				}
				case BinaryOperator.LogicalOr:
				{
					return "OrElse";
				}
				case BinaryOperator.LogicalAnd:
				{
					return "AndAlso";
				}
				case BinaryOperator.LessThan:
				{
					return "<";
				}
				case BinaryOperator.LessThanOrEqual:
				{
					return "<=";
				}
				case BinaryOperator.GreaterThan:
				{
					return ">";
				}
				case BinaryOperator.GreaterThanOrEqual:
				{
					return ">=";
				}
				case BinaryOperator.LeftShift:
				{
					return "<<";
				}
				case BinaryOperator.LeftShiftAssign:
				{
					return "<<=";
				}
				case BinaryOperator.RightShift:
				{
					return ">>";
				}
				case BinaryOperator.RightShiftAssign:
				{
					return ">>=";
				}
				case BinaryOperator.BitwiseOr:
				{
					return "Or";
				}
				case BinaryOperator.BitwiseAnd:
				{
					return "And";
				}
				case BinaryOperator.BitwiseXor:
				{
					return "Xor";
				}
				case BinaryOperator.Modulo:
				{
					return "Mod";
				}
				case BinaryOperator.ModuloAssign:
				{
					throw new ArgumentException();
				}
				case BinaryOperator.Assign:
				{
					return "=";
				}
				default:
				{
					throw new ArgumentException();
				}
			}
		}

		protected override string ToString(UnaryOperator op)
		{
			switch (op)
			{
				case UnaryOperator.Negate:
				{
					return "-";
				}
				case UnaryOperator.LogicalNot:
				{
					return "Not ";
				}
				case UnaryOperator.BitwiseNot:
				{
					return "Not ";
				}
				case UnaryOperator.PostDecrement:
				{
					return " - 1";
				}
				case UnaryOperator.PostIncrement:
				{
					return " + 1";
				}
				case UnaryOperator.PreDecrement:
				{
					return "1 - ";
				}
				case UnaryOperator.PreIncrement:
				{
					return "1 + ";
				}
				case UnaryOperator.AddressReference:
				case UnaryOperator.AddressDereference:
				case UnaryOperator.AddressOf:
				{
					throw new ArgumentException(String.Format("The unary opperator {0} is not supported in VisualBasic", op));
				}
				case UnaryOperator.UnaryPlus:
				{
					return "+";
				}
				case UnaryOperator.None:
				{
					return String.Empty;
				}
				default:
				{
					throw new ArgumentException(String.Format("The unary opperator {0} is not supported in VisualBasic", op));
				}
			}
		}

		public override string ToTypeString(TypeReference type)
		{
			if (base.IsReferenceFromMscorlib(type))
			{
				string name = type.Name;
				if (name != null)
				{
					if (name == "Decimal")
					{
						return "Decimal";
					}
					if (name == "Single")
					{
						return "Single";
					}
					if (name == "Byte")
					{
						return "Byte";
					}
					if (name == "SByte")
					{
						return "SByte";
					}
					if (name == "Char")
					{
						return "Char";
					}
					if (name == "Double")
					{
						return "Double";
					}
					if (name == "Boolean")
					{
						return "Boolean";
					}
					if (name == "Int16")
					{
						return "Short";
					}
					if (name == "Int32")
					{
						return "Integer";
					}
					if (name == "Int64")
					{
						return "Long";
					}
					if (name == "UInt16")
					{
						return "UShort";
					}
					if (name == "UInt32")
					{
						return "UInteger";
					}
					if (name == "UInt64")
					{
						return "ULong";
					}
					if (name == "String")
					{
						return "String";
					}
					if (name == "Void")
					{
						return "Void";
					}
					if (name == "Object")
					{
						return "Object";
					}
					if (name == "RuntimeArgumentHandle")
					{
						return String.Empty;
					}
				}
			}
			return this.GetGenericNameFromMemberReference(type);
		}

		protected override bool TypeSupportsExplicitStaticMembers(TypeDefinition type)
		{
			return !type.IsStaticClass;
		}

		private void VisitAddressOfExpression(UnaryExpression node)
		{
			if (base.MethodReferences.Count == 0)
			{
				this.WriteKeyword(base.KeyWordWriter.AddressOf);
				this.WriteSpace();
			}
			this.Visit(node.Operand);
		}

		public override void VisitAnonymousPropertyInitializerExpression(AnonymousPropertyInitializerExpression node)
		{
			if (node.IsKey)
			{
				this.WriteKeyword(base.KeyWordWriter.Key);
				this.WriteSpace();
			}
			this.WriteToken(".");
			base.VisitAnonymousPropertyInitializerExpression(node);
		}

		public override void VisitArrayAssignmentFieldReferenceExpression(ArrayAssignmentFieldReferenceExpression node)
		{
			if (!node.HasInitializer)
			{
				this.WriteKeyword(base.KeyWordWriter.ReDim);
				this.WriteSpace();
			}
			this.Visit(node.Field);
			if (!node.HasInitializer)
			{
				this.WriteArrayDimensions(node.Dimensions, node.ArrayType, node.HasInitializer);
			}
		}

		public override void VisitArrayAssignmentVariableReferenceExpression(ArrayVariableReferenceExpression node)
		{
			if (!node.HasInitializer)
			{
				this.WriteKeyword(base.KeyWordWriter.ReDim);
				this.WriteSpace();
			}
			this.Visit(node.Variable);
			if (!node.HasInitializer)
			{
				this.WriteArrayDimensions(node.Dimensions, node.ArrayType, node.HasInitializer);
			}
		}

		public override void VisitArrayCreationExpression(ArrayCreationExpression node)
		{
			base.VisitArrayCreationExpression(node);
			if (!Utilities.IsInitializerPresent(node.Initializer))
			{
				this.WriteSpace();
				this.Write("{}");
			}
		}

		public override void VisitArrayVariableDeclarationExpression(ArrayVariableDeclarationExpression node)
		{
			if ((node.Variable.Variable.VariableType.IsOptionalModifier || node.Variable.Variable.VariableType.IsRequiredModifier) && !this.isWritingComment)
			{
				this.StartWritingComment();
				this.VisitVariableDeclarationExpression(node.Variable);
				this.EndWritingComment();
				this.WriteLine();
			}
			this.WriteDim();
			string variableName = base.GetVariableName(node.Variable.Variable);
			base.WriteAndMapVariableToCode(() => this.Write(variableName), node.Variable.Variable);
			this.WriteArrayDimensions(node.Dimensions, node.ArrayType, node.HasInitializer);
			this.WriteAsBetweenSpaces();
			this.WriteReferenceAndNamespaceIfInCollision(base.GetBaseElementType(node.ArrayType));
		}

		public override void VisitBaseCtorExpression(BaseCtorExpression node)
		{
			this.VisitCtorExpression(node, "MyBase");
		}

		public override void VisitBinaryExpression(BinaryExpression node)
		{
			if (node.Operator == BinaryOperator.NullCoalesce)
			{
				this.VisitNullCoalesceExpression(node);
				return;
			}
			if (this.IsMulticastDelegate(node.Left))
			{
				string str = "";
				if (node.Operator != BinaryOperator.AddAssign)
				{
					if (node.Operator != BinaryOperator.SubtractAssign)
					{
						base.VisitBinaryExpression(node);
						return;
					}
					str = "RemoveHandler";
				}
				else
				{
					str = "AddHandler";
				}
				this.WriteKeyword(str);
				this.WriteSpace();
				this.Visit(node.Left);
				this.Write(",");
				this.WriteSpace();
				this.WriteSpace();
				this.Visit(node.Right);
				return;
			}
			if (!this.IsArrayCreationAssignment(node))
			{
				base.VisitBinaryExpression(node);
				return;
			}
			this.Visit(node.Left);
			ArrayCreationExpression right = node.Right as ArrayCreationExpression;
			if (Utilities.IsInitializerPresent(right.Initializer))
			{
				this.WriteSpace();
				this.Write(this.ToString(BinaryOperator.Assign, false));
				if (this.IsArrayCreationToVariableDeclarationAssignment(node))
				{
					base.StartInitializer(right.Initializer);
					this.Visit(right.Initializer);
					return;
				}
				this.WriteSpace();
				this.Visit(node.Right);
			}
		}

		public override void VisitBreakStatement(BreakStatement node)
		{
			this.WriteKeyword("Exit");
			this.WriteSpace();
			this.WriteInnermostParentFrom(this.GetExitableParents());
		}

		public override void VisitCatchClause(CatchClause node)
		{
			this.WriteKeyword(base.KeyWordWriter.Catch);
			if (node.Type.FullName != "System.Object")
			{
				this.WriteSpace();
				if (node.Variable == null)
				{
					this.WriteReferenceAndNamespaceIfInCollision(node.Type);
				}
				else
				{
					this.statementStates.Push(StatementState.Catch);
					this.Visit(node.Variable);
					this.statementStates.Pop();
				}
			}
			if (node.Filter != null)
			{
				this.WriteSpace();
				this.WriteKeyword(base.KeyWordWriter.When);
				this.WriteSpace();
				this.Visit((node.Filter as ExpressionStatement).Expression);
			}
			this.WriteLine();
			this.Visit(node.Body);
		}

		public override void VisitConditionCase(ConditionCase node)
		{
			this.WriteKeyword(base.KeyWordWriter.Case);
			this.WriteSpace();
			this.Visit(node.Condition);
			this.WriteLine();
			this.Visit(node.Body);
		}

		public override void VisitConditionExpression(ConditionExpression node)
		{
			this.WriteKeyword(base.KeyWordWriter.If);
			this.WriteToken("(");
			this.Visit(node.Condition);
			this.WriteToken(",");
			this.WriteSpace();
			this.Visit(node.Then);
			this.WriteToken(",");
			this.WriteSpace();
			this.Visit(node.Else);
			this.WriteToken(")");
		}

		public override void VisitContinueStatement(ContinueStatement node)
		{
			this.WriteKeyword("Continue");
			this.WriteSpace();
			this.WriteInnermostParentFrom(this.GetContinuableParents());
		}

		private void VisitCtorExpression(MethodInvocationExpression node, string ctorKeyword)
		{
			this.WriteKeyword(ctorKeyword);
			this.WriteToken(".");
			if (node.MethodExpression.CodeNodeType != CodeNodeType.MethodReferenceExpression)
			{
				this.WriteKeyword("New");
			}
			else
			{
				this.WriteReference("New", node.MethodExpression.Method);
			}
			this.WriteToken("(");
			base.EnterMethodInvocation(node.MethodExpression.Method);
			base.VisitMethodParameters(node.Arguments);
			base.LeaveMethodInvocation();
			this.WriteToken(")");
		}

		public override void VisitDefaultCase(DefaultCase node)
		{
			this.WriteKeyword(base.KeyWordWriter.Default);
			this.WriteSpace();
			this.WriteKeyword(base.KeyWordWriter.Else);
			this.WriteLine();
			this.Visit(node.Body);
		}

		public override void VisitDefaultObjectExpression(DefaultObjectExpression node)
		{
			this.WriteKeyword("Nothing");
		}

		public override void VisitDoWhileStatement(DoWhileStatement node)
		{
			this.statementStates.Push(StatementState.DoWhile);
			this.WriteKeyword(base.KeyWordWriter.Do);
			this.WriteLine();
			this.Visit(node.Body);
			this.WriteKeyword(base.KeyWordWriter.LoopWhile);
			this.WriteSpace();
			this.WriteSpecialBetweenParenthesis(node.Condition);
			this.WriteEndOfStatement();
			this.statementStates.Pop();
		}

		public override void VisitExplicitCastExpression(ExplicitCastExpression node)
		{
			TypeReference typeReference;
			string castMethod = this.GetCastMethod(node.TargetType);
			if (castMethod != null)
			{
				if (node.UnresolvedReferenceForAmbiguousCastToObject != null)
				{
					Telerik.JustDecompiler.Common.Extensions.ResolveToOverloadedEqualityOperator(node.Expression.ExpressionType, out typeReference);
					this.WriteNotResolvedReference(castMethod, typeReference, String.Format("The cast to object might be unnecessary. Please, locate the assembly where \"{0}\" is defined.", typeReference.Name));
				}
				else
				{
					this.WriteKeyword(castMethod);
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
				this.WriteReferenceAndNamespaceIfInCollision(node.TargetType);
			}
			this.WriteToken(")");
		}

		public override void VisitFieldInitializerExpression(FieldInitializerExpression node)
		{
			this.WriteToken(".");
			base.VisitFieldInitializerExpression(node);
		}

		public override void VisitForEachStatement(ForEachStatement node)
		{
			this.statementStates.Push(StatementState.ForEach);
			this.WriteKeyword(base.KeyWordWriter.ForEach);
			this.WriteSpace();
			this.statementStates.Push(StatementState.ForEachInitializer);
			this.Visit(node.Variable);
			this.statementStates.Pop();
			this.WriteSpace();
			this.WriteKeyword(base.KeyWordWriter.In);
			this.WriteSpace();
			this.Visit(node.Collection);
			this.WriteLine();
			this.Visit(node.Body);
			this.WriteKeyword(base.KeyWordWriter.Next);
			this.statementStates.Pop();
		}

		public override void VisitForStatement(ForStatement node)
		{
			BinaryExpression condition = node.Condition as BinaryExpression;
			Expression increment = node.Increment;
			if (condition == null || increment == null)
			{
				throw new Exception("Unexpected null value.");
			}
			if (condition.IsAssignmentExpression)
			{
				return;
			}
			this.statementStates.Push(StatementState.For);
			Expression forStep = this.GetForStep(increment);
			this.WriteKeyword("For");
			this.WriteSpace();
			this.statementStates.Push(StatementState.ForEachInitializer);
			this.Visit(node.Initializer);
			this.statementStates.Pop();
			this.WriteSpace();
			this.WriteKeyword("To");
			this.WriteSpace();
			this.Visit(condition.Right);
			if (forStep != null)
			{
				this.WriteSpace();
				this.WriteKeyword("Step");
				this.WriteSpace();
				this.Visit(forStep);
			}
			this.WriteLine();
			this.Visit(node.Body);
			this.WriteKeyword(base.KeyWordWriter.Next);
			this.WriteLine();
			this.statementStates.Pop();
		}

		public override void VisitIfElseIfStatement(IfElseIfStatement node)
		{
			for (int i = 0; i < node.ConditionBlocks.Count; i++)
			{
				if (i != 0)
				{
					this.WriteKeyword(base.KeyWordWriter.ElseIf);
				}
				else
				{
					this.WriteKeyword(base.KeyWordWriter.If);
				}
				this.WriteSpace();
				KeyValuePair<Expression, BlockStatement> item = node.ConditionBlocks[i];
				base.WriteBetweenParenthesis(item.Key);
				if (base.KeyWordWriter.Then != null)
				{
					this.WriteSpace();
					this.WriteKeyword(base.KeyWordWriter.Then);
				}
				this.WriteLine();
				item = node.ConditionBlocks[i];
				this.Visit(item.Value);
			}
			if (node.Else != null)
			{
				this.WriteKeyword(base.KeyWordWriter.Else);
				this.WriteLine();
				this.Visit(node.Else);
			}
			this.WriteSpecialEndBlock(base.KeyWordWriter.If);
		}

		public override void VisitIfStatement(IfStatement node)
		{
			this.WriteKeyword(base.KeyWordWriter.If);
			this.WriteSpace();
			base.WriteBetweenParenthesis(node.Condition);
			if (base.KeyWordWriter.Then != null)
			{
				this.WriteSpace();
				this.WriteKeyword(base.KeyWordWriter.Then);
			}
			this.WriteLine();
			this.Visit(node.Then);
			if (node.Else == null)
			{
				this.WriteSpecialEndBlock(base.KeyWordWriter.If);
				return;
			}
			this.WriteKeyword(base.KeyWordWriter.Else);
			this.WriteLine();
			this.Visit(node.Else);
			this.WriteSpecialEndBlock(base.KeyWordWriter.If);
		}

		public override void VisitLambdaExpression(LambdaExpression node)
		{
			bool flag;
			base.VisitLambdaExpression(node);
			string str = (node.IsFunction ? base.KeyWordWriter.Function : base.KeyWordWriter.Sub);
			this.WriteKeyword(str);
			this.WriteToken("(");
			base.VisitMethodParameters(node.Arguments);
			this.WriteToken(")");
			bool flag1 = false;
			if (node.Body.Statements.Count != 1)
			{
				base.ShouldOmitSemicolon.Push(false);
				this.WriteLine();
				this.Visit(node.Body);
				flag = true;
			}
			else
			{
				if (node.Body.Statements[0].CodeNodeType != CodeNodeType.ExpressionStatement)
				{
					base.ShouldOmitSemicolon.Push(false);
				}
				else
				{
					flag1 = true;
					base.ShouldOmitSemicolon.Push(true);
				}
				if (!flag1)
				{
					this.WriteLine();
				}
				else
				{
					this.WriteSpace();
				}
				this.Visit(node.Body.Statements[0]);
				flag = false;
			}
			if (!flag1)
			{
				if (!flag)
				{
					this.WriteLine();
				}
				this.WriteEndBlock(str);
			}
			base.ShouldOmitSemicolon.Pop();
		}

		public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			GenericInstanceMethod method = node.MethodExpression.Method as GenericInstanceMethod;
			if (node.MethodExpression.Method.HasThis || node.Arguments.Count == 0 || method == null || node.MethodExpression.MethodDefinition == null || !node.MethodExpression.MethodDefinition.IsExtensionMethod)
			{
				base.VisitMethodInvocationExpression(node);
				return;
			}
			base.WriteMethodTarget(node.Arguments[0]);
			base.WriteGenericInstanceMethodWithArguments(method, this.GetGenericExtensionMethodArguments(method));
			this.WriteToken("(");
			base.VisitExtensionMethodParameters(node.Arguments);
			this.WriteToken(")");
		}

		private void VisitNullCoalesceExpression(BinaryExpression node)
		{
			this.WriteKeyword(base.KeyWordWriter.If);
			this.WriteToken("(");
			this.Visit(node.Left);
			this.WriteToken(",");
			this.WriteSpace();
			this.Visit(node.Right);
			this.WriteToken(")");
		}

		public override void VisitPropertyInitializerExpression(PropertyInitializerExpression node)
		{
			this.WriteToken(".");
			base.VisitPropertyInitializerExpression(node);
		}

		public override void VisitPropertyReferenceExpression(PropertyReferenceExpression node)
		{
			if (node.Target == null || node.Target.CodeNodeType != CodeNodeType.BaseReferenceExpression || !node.IsIndexer)
			{
				base.VisitPropertyReferenceExpression(node);
				return;
			}
			this.Visit(node.Target);
			this.WriteToken(".");
			this.WritePropertyName(node.Property);
			base.WriteIndexerArguments(node);
		}

		public override void VisitRaiseEventExpression(RaiseEventExpression node)
		{
			this.WriteKeyword(base.KeyWordWriter.Fire);
			this.WriteSpace();
			this.WriteReference(node.Event.Name, node.Event);
			base.EnterMethodInvocation(node.InvokeMethodReference);
			this.Write("(");
			base.VisitMethodParameters(node.Arguments);
			this.Write(")");
			base.LeaveMethodInvocation();
		}

		public override void VisitSafeCastExpression(SafeCastExpression node)
		{
			this.WriteKeyword(base.KeyWordWriter.TryCast);
			this.WriteToken("(");
			this.Visit(node.Expression);
			this.WriteToken(",");
			this.WriteSpace();
			this.WriteReferenceAndNamespaceIfInCollision(node.TargetType);
			this.WriteToken(")");
		}

		public override void VisitSwitchStatement(SwitchStatement node)
		{
			this.statementStates.Push(StatementState.Switch);
			this.WriteKeyword(base.KeyWordWriter.Switch);
			this.WriteSpace();
			this.WriteKeyword(base.KeyWordWriter.Case);
			this.WriteSpace();
			this.Visit(node.Condition);
			this.WriteLine();
			this.WriteBlock(() => this.VisitVBSwitchCases(node.Cases), "");
			this.WriteEndBlock(base.KeyWordWriter.Switch);
			this.statementStates.Pop();
		}

		public override void VisitThisCtorExpression(ThisCtorExpression node)
		{
			this.VisitCtorExpression(node, "MyClass");
		}

		public override void VisitTryStatement(TryStatement node)
		{
			this.WriteKeyword(base.KeyWordWriter.Try);
			this.WriteLine();
			this.Visit(node.Try);
			foreach (ICodeNode catchClause in node.CatchClauses)
			{
				this.Visit(catchClause);
			}
			if (node.Finally != null)
			{
				this.WriteKeyword(base.KeyWordWriter.Finally);
				this.WriteLine();
				this.Visit(node.Finally);
			}
			this.WriteSpecialEndBlock(base.KeyWordWriter.Try);
		}

		public override void VisitUnaryExpression(UnaryExpression node)
		{
			if (node.Operator == UnaryOperator.Negate || node.Operator == UnaryOperator.LogicalNot || node.Operator == UnaryOperator.BitwiseNot || node.Operator == UnaryOperator.UnaryPlus)
			{
				this.Write(this.ToString(node.Operator));
				base.VisitUnaryExpression(node);
				return;
			}
			if (node.Operator == UnaryOperator.AddressOf)
			{
				this.VisitAddressOfExpression(node);
				return;
			}
			if (node.Operator == UnaryOperator.AddressDereference)
			{
				base.VisitUnaryExpression(node);
				return;
			}
			bool flag = base.IsPostUnaryOperator(node.Operator);
			bool flag1 = false;
			if (node.Operator == UnaryOperator.PostDecrement || node.Operator == UnaryOperator.PostIncrement || node.Operator == UnaryOperator.PreDecrement || node.Operator == UnaryOperator.PreIncrement)
			{
				flag1 = true;
				this.Visit(node.Operand);
				this.WriteSpace();
				this.WriteToken("=");
				this.WriteSpace();
			}
			if (!flag)
			{
				this.Write(this.ToString(node.Operator));
			}
			this.Visit((flag1 ? node.Operand.CloneExpressionOnly() : node.Operand));
			if (flag)
			{
				this.Write(this.ToString(node.Operator));
			}
		}

		public override void VisitUsingStatement(UsingStatement node)
		{
			this.WriteKeyword(base.KeyWordWriter.Using);
			this.WriteSpace();
			this.statementStates.Push(StatementState.Using);
			this.WriteSpecialBetweenParenthesis(node.Expression);
			this.statementStates.Pop();
			this.WriteLine();
			this.Visit(node.Body);
			this.WriteSpecialEndBlock(base.KeyWordWriter.Using);
		}

		public override void VisitVariableDeclarationExpression(VariableDeclarationExpression node)
		{
			if ((node.Variable.VariableType.IsOptionalModifier || node.Variable.VariableType.IsRequiredModifier) && !this.isWritingComment)
			{
				this.StartWritingComment();
				this.VisitVariableDeclarationExpression(node);
				this.EndWritingComment();
				this.WriteLine();
			}
			this.WriteDim();
			VariableDefinition variable = node.Variable;
			if (!variable.VariableType.ContainsAnonymousType())
			{
				base.VisitVariableDeclarationExpression(node);
				return;
			}
			base.WriteAndMapVariableToCode(() => this.WriteToken(base.GetVariableName(variable)), node.Variable);
		}

		private void VisitVBSwitchCases(IEnumerable collection)
		{
			foreach (ICodeNode codeNode in collection)
			{
				this.Visit(codeNode);
			}
		}

		public override void VisitWhileStatement(WhileStatement node)
		{
			this.statementStates.Push(StatementState.While);
			base.VisitWhileStatement(node);
			this.statementStates.Pop();
		}

		public override void VisitYieldBreakExpression(YieldBreakExpression node)
		{
			this.WriteKeyword(base.KeyWordWriter.Return);
		}

		public override void VisitYieldReturnExpression(YieldReturnExpression node)
		{
			this.WriteKeyword(base.KeyWordWriter.Return);
			this.WriteSpace();
			this.WriteKeyword(base.KeyWordWriter.New);
			this.WriteSpace();
			TypeReference expressionType = this.GetExpressionType(node.Expression);
			if (expressionType == null)
			{
				string str = "Object";
				if (node.Expression.CodeNodeType == CodeNodeType.LiteralExpression)
				{
					str = this.GetExpressionType((LiteralExpression)node.Expression);
				}
				this.WriteKeyword(str);
			}
			else
			{
				this.WriteReferenceAndNamespaceIfInCollision(expressionType);
			}
			this.WriteToken("(");
			this.WriteToken(")");
			this.WriteSpace();
			this.WriteToken("{");
			this.WriteSpace();
			this.Visit(node.Expression);
			this.WriteSpace();
			this.WriteToken("}");
		}

		protected override void Write(PropertyDefinition property)
		{
			if (!property.IsAbstract() && !this.TypeContext.AutoImplementedProperties.Contains(property))
			{
				base.Write(property);
				return;
			}
			this.WritePropertyDeclaration(property);
			this.currentWritingInfo.MemberDefinitionToFoldingPositionMap[property] = new OffsetSpan(this.formatter.CurrentPosition, this.formatter.CurrentPosition);
		}

		protected override void WriteArrayDimensions(ExpressionCollection dimensions, TypeReference arrayType, bool isInitializerPresent)
		{
			ExpressionCollection binaryExpression = dimensions.Clone();
			TypeSystem typeSystem = this.ModuleContext.Module.TypeSystem;
			for (int i = 0; i < binaryExpression.Count; i++)
			{
				if (!(binaryExpression[i] is LiteralExpression))
				{
					binaryExpression[i] = new BinaryExpression(BinaryOperator.Subtract, binaryExpression[i], new LiteralExpression((object)1, typeSystem, null), typeSystem, null, false);
				}
				else
				{
					LiteralExpression item = binaryExpression[i] as LiteralExpression;
					item.Value = this.GetDecrementedValue(item);
				}
			}
			base.WriteArrayDimensions(binaryExpression, arrayType, isInitializerPresent);
		}

		private void WriteAsBetweenSpaces()
		{
			this.WriteSpace();
			this.WriteKeyword(base.KeyWordWriter.As);
			this.WriteSpace();
		}

		protected override void WriteBaseConstructorInvokation(MethodInvocationExpression baseConstructorInvokation)
		{
			if (!base.MethodContext.Method.IsStatic)
			{
				this.WriteLine();
				this.Indent();
				this.Visit(base.MethodContext.CtorInvokeExpression);
				this.Outdent();
			}
		}

		protected override void WriteBaseTypeInheritColon()
		{
			this.WriteLine();
			this.WriteKeyword("Inherits");
			this.WriteSpace();
		}

		protected override void WriteBlock(Action action, string label)
		{
			this.Indent();
			if (label != "")
			{
				base.WriteLabel(label);
			}
			action();
			this.Outdent();
		}

		private void WriteCastExpression(ExplicitCastExpression node)
		{
			bool flag = this.IsComplexTarget(node.Expression);
			if (flag)
			{
				this.WriteToken("(");
			}
			this.Visit(node.Expression);
			if (flag)
			{
				this.WriteToken(")");
			}
		}

		protected override void WriteDelegateCreation(ObjectCreationExpression node)
		{
			this.WriteKeyword(base.KeyWordWriter.New);
			this.WriteSpace();
			this.WriteReferenceAndNamespaceIfInCollision(node.ExpressionType);
			this.WriteToken("(");
			this.WriteKeyword(base.KeyWordWriter.AddressOf);
			this.WriteSpace();
			base.WriteDelegateArgument(node);
			this.WriteToken(")");
		}

		protected override void WriteDestructor(MethodDefinition method)
		{
			base.WriteMethod(method);
		}

		protected void WriteDim()
		{
			if (this.statementStates.Count > 0)
			{
				StatementState statementState = this.statementStates.Peek();
				if (statementState == StatementState.ForEachInitializer || statementState == StatementState.ForInitializer || statementState == StatementState.Catch || statementState == StatementState.Using)
				{
					return;
				}
			}
			this.WriteKeyword(base.KeyWordWriter.Dim);
			this.WriteSpace();
		}

		protected override void WriteEmptyMethodEndOfStatement(MethodDefinition method)
		{
			if (!method.IsAbstract)
			{
				this.WriteLine();
				this.WriteSpecialEndBlock(base.GetMethodKeyWord(method));
			}
		}

		protected override void WriteEndBlock(string statementName)
		{
			this.WriteEndBlockWithoutNewLine(statementName);
		}

		private void WriteEndBlockWithoutNewLine(string statementName)
		{
			this.WriteKeyword("End");
			this.WriteSpace();
			this.WriteKeyword(statementName);
		}

		protected override void WriteEnumBaseTypeInheritColon()
		{
			this.WriteAsBetweenSpaces();
		}

		protected override void WriteEscapeCharLiteral(char c)
		{
			UnicodeCategory unicodeCategory;
			bool flag;
			int? nullable = null;
			switch (c)
			{
				case '\0':
				case '\a':
				case '\b':
				case '\t':
				case '\n':
				case '\v':
				case '\f':
				case '\r':
				{
					nullable = new int?(c);
					unicodeCategory = Char.GetUnicodeCategory(c);
					flag = false;
					if (unicodeCategory == UnicodeCategory.ClosePunctuation || unicodeCategory == UnicodeCategory.CurrencySymbol || unicodeCategory == UnicodeCategory.DashPunctuation || unicodeCategory == UnicodeCategory.DecimalDigitNumber || unicodeCategory == UnicodeCategory.FinalQuotePunctuation || unicodeCategory == UnicodeCategory.InitialQuotePunctuation || unicodeCategory == UnicodeCategory.LetterNumber || unicodeCategory == UnicodeCategory.LowercaseLetter || unicodeCategory == UnicodeCategory.MathSymbol || unicodeCategory == UnicodeCategory.OpenPunctuation || unicodeCategory == UnicodeCategory.TitlecaseLetter || unicodeCategory == UnicodeCategory.UppercaseLetter || unicodeCategory == UnicodeCategory.OtherPunctuation)
					{
						flag = true;
					}
					if (!flag)
					{
						nullable = new int?(c);
					}
					if (nullable.HasValue)
					{
						this.Write("Strings.ChrW(");
						this.WriteLiteral(nullable.ToString());
						this.Write(")");
						return;
					}
					this.WriteLiteral(this.CharStart);
					this.formatter.WriteLiteral(c.ToString());
					this.WriteLiteral(this.CharEnd);
					return;
				}
				case '\u0001':
				case '\u0002':
				case '\u0003':
				case '\u0004':
				case '\u0005':
				case '\u0006':
				{
					unicodeCategory = Char.GetUnicodeCategory(c);
					flag = false;
					if (unicodeCategory == UnicodeCategory.ClosePunctuation || unicodeCategory == UnicodeCategory.CurrencySymbol || unicodeCategory == UnicodeCategory.DashPunctuation || unicodeCategory == UnicodeCategory.DecimalDigitNumber || unicodeCategory == UnicodeCategory.FinalQuotePunctuation || unicodeCategory == UnicodeCategory.InitialQuotePunctuation || unicodeCategory == UnicodeCategory.LetterNumber || unicodeCategory == UnicodeCategory.LowercaseLetter || unicodeCategory == UnicodeCategory.MathSymbol || unicodeCategory == UnicodeCategory.OpenPunctuation || unicodeCategory == UnicodeCategory.TitlecaseLetter || unicodeCategory == UnicodeCategory.UppercaseLetter || unicodeCategory == UnicodeCategory.OtherPunctuation)
					{
						flag = true;
					}
					if (!flag)
					{
						nullable = new int?(c);
					}
					if (nullable.HasValue)
					{
						this.Write("Strings.ChrW(");
						this.WriteLiteral(nullable.ToString());
						this.Write(")");
						return;
					}
					this.WriteLiteral(this.CharStart);
					this.formatter.WriteLiteral(c.ToString());
					this.WriteLiteral(this.CharEnd);
					return;
				}
				default:
				{
					if (c != '\"')
					{
						unicodeCategory = Char.GetUnicodeCategory(c);
						flag = false;
						if (unicodeCategory == UnicodeCategory.ClosePunctuation || unicodeCategory == UnicodeCategory.CurrencySymbol || unicodeCategory == UnicodeCategory.DashPunctuation || unicodeCategory == UnicodeCategory.DecimalDigitNumber || unicodeCategory == UnicodeCategory.FinalQuotePunctuation || unicodeCategory == UnicodeCategory.InitialQuotePunctuation || unicodeCategory == UnicodeCategory.LetterNumber || unicodeCategory == UnicodeCategory.LowercaseLetter || unicodeCategory == UnicodeCategory.MathSymbol || unicodeCategory == UnicodeCategory.OpenPunctuation || unicodeCategory == UnicodeCategory.TitlecaseLetter || unicodeCategory == UnicodeCategory.UppercaseLetter || unicodeCategory == UnicodeCategory.OtherPunctuation)
						{
							flag = true;
						}
						if (!flag)
						{
							nullable = new int?(c);
						}
						if (nullable.HasValue)
						{
							this.Write("Strings.ChrW(");
							this.WriteLiteral(nullable.ToString());
							this.Write(")");
							return;
						}
						this.WriteLiteral(this.CharStart);
						this.formatter.WriteLiteral(c.ToString());
						this.WriteLiteral(this.CharEnd);
						return;
					}
					this.WriteLiteral(this.CharStart);
					this.formatter.WriteLiteral("\"\"");
					this.WriteLiteral(this.CharEnd);
					return;
				}
			}
		}

		protected override void WriteEventAddOnParameters(EventDefinition @event)
		{
			if (@event.AddMethod.Parameters.Any<ParameterDefinition>())
			{
				this.Write("(");
				this.WriteParameters(@event.AddMethod);
				this.Write(")");
			}
		}

		protected override void WriteEventInterfaceImplementations(EventDefinition @event)
		{
			ICollection<ImplementedMember> implementedEvents = @event.GetImplementedEvents();
			if (!@event.IsExplicitImplementation())
			{
				ICollection<ImplementedMember> notExplicitlyImplementedMembers = this.GetNotExplicitlyImplementedMembers(implementedEvents);
				if (notExplicitlyImplementedMembers.Any<ImplementedMember>())
				{
					this.WriteImplementedMembers(notExplicitlyImplementedMembers);
				}
			}
			else if (implementedEvents.Any<ImplementedMember>())
			{
				this.WriteImplementedMembers(implementedEvents);
				return;
			}
		}

		protected override void WriteEventRemoveOnParameters(EventDefinition @event)
		{
			if (@event.RemoveMethod.Parameters.Any<ParameterDefinition>())
			{
				this.Write("(");
				this.WriteParameters(@event.RemoveMethod);
				this.Write(")");
			}
		}

		protected override void WriteEventTypeAndName(EventDefinition @event)
		{
			if (!@event.IsExplicitImplementation())
			{
				base.WriteEventTypeAndName(@event);
				return;
			}
			string eventName = this.GetEventName(@event);
			this.WriteTypeAndName(@event.EventType, eventName, @event);
		}

		protected override void WriteFieldDeclaration(FieldDefinition field)
		{
			if (field.FieldType.IsOptionalModifier || field.FieldType.IsRequiredModifier)
			{
				this.StartWritingComment();
				base.WriteFieldDeclaration(field);
				this.EndWritingComment();
				this.WriteLine();
			}
			base.WriteFieldDeclaration(field);
		}

		protected override void WriteGenericParameterConstraints(GenericParameter parameter)
		{
			int num = this.CountConstraints(parameter);
			this.WriteSpace();
			this.WriteKeyword(base.KeyWordWriter.As);
			this.WriteSpace();
			if (num > 1)
			{
				this.WriteToken("{");
			}
			base.WriteSingleGenericParameterConstraintsList(parameter);
			if (num > 1)
			{
				this.WriteToken("}");
			}
		}

		private void WriteImplementedMembers(ICollection<ImplementedMember> implementedMembers)
		{
			List<ImplementedMember> list;
			if (!this.TypeContext.IsWinRTImplementation)
			{
				list = implementedMembers.ToList<ImplementedMember>();
			}
			else
			{
				list = implementedMembers.Where<ImplementedMember>((ImplementedMember member) => {
					TypeDefinition typeDefinition = member.DeclaringType.Resolve();
					if (typeDefinition == null || !typeDefinition.IsInterface)
					{
						return true;
					}
					return !typeDefinition.IsWindowsRuntime;
				}).ToList<ImplementedMember>();
				if (list.Count == 0)
				{
					return;
				}
			}
			this.WriteSpace();
			this.WriteKeyword(base.KeyWordWriter.Implements);
			this.WriteSpace();
			bool flag = true;
			foreach (ImplementedMember implementedMember in list)
			{
				if (!flag)
				{
					this.Write(",");
					this.WriteSpace();
				}
				this.WriteReferenceAndNamespaceIfInCollision(implementedMember.DeclaringType);
				this.Write(".");
				string memberName = base.GetMemberName(implementedMember.Member);
				this.WriteReference(memberName, implementedMember.Member);
				flag = false;
			}
		}

		protected override void WriteIndexerKeywords()
		{
			this.WriteKeyword("Default");
			this.WriteSpace();
		}

		private void WriteInnermostParentFrom(Dictionary<StatementState, string> parents)
		{
			Stack<StatementState>.Enumerator enumerator = this.statementStates.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					StatementState current = enumerator.Current;
					if (!parents.ContainsKey(current))
					{
						continue;
					}
					this.WriteKeyword(parents[current]);
					return;
				}
				throw new Exception("Suitable parent for Continue/Exit statement not found.");
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}

		protected override void WriteInterfacesInheritColon(TypeDefinition type)
		{
			this.WriteLine();
			if (!type.IsInterface)
			{
				this.WriteKeyword(base.KeyWordWriter.Implements);
			}
			else
			{
				this.WriteKeyword(base.KeyWordWriter.Inherits);
			}
			this.WriteSpace();
		}

		public override void WriteMemberNavigationPathFullName(object member)
		{
			if (member is ParameterReference)
			{
				this.formatter.Write(((ParameterReference)member).Name);
				return;
			}
			if (member is TypeReference)
			{
				this.formatter.Write(((TypeReference)member).GetUIFriendlyTypeNameInVB(base.Language));
				return;
			}
			if (member is MemberReference)
			{
				this.formatter.Write(((MemberReference)member).GetFriendlyFullNameInVB(base.Language));
			}
		}

		protected override void WriteMethodDeclaration(MethodDefinition method, bool writeDocumentation)
		{
			bool flag = false;
			if (method.ReturnType.IsOptionalModifier || method.ReturnType.IsRequiredModifier)
			{
				flag = true;
			}
			for (int i = 0; i < method.Parameters.Count && !flag; i++)
			{
				ParameterDefinition item = method.Parameters[i];
				if (item.ParameterType.IsOptionalModifier || item.ParameterType.IsRequiredModifier)
				{
					flag = true;
				}
			}
			if (flag)
			{
				this.StartWritingComment();
				base.WriteMethodDeclaration(method, writeDocumentation);
				this.EndWritingComment();
				this.WriteLine();
			}
			base.WriteMethodDeclaration(method, writeDocumentation);
		}

		protected override void WriteMethodInterfaceImplementations(MethodDefinition method)
		{
			ICollection<ImplementedMember> implementedMethods = method.GetImplementedMethods();
			if (!method.IsExplicitImplementation())
			{
				ICollection<ImplementedMember> notExplicitlyImplementedMembers = this.GetNotExplicitlyImplementedMembers(implementedMethods);
				if (notExplicitlyImplementedMembers.Any<ImplementedMember>())
				{
					this.WriteImplementedMembers(notExplicitlyImplementedMembers);
				}
			}
			else if (implementedMethods.Any<ImplementedMember>())
			{
				this.WriteImplementedMembers(implementedMethods);
				return;
			}
		}

		protected override void WriteOptional(ParameterDefinition parameter)
		{
			this.WriteKeyword("Optional");
			this.WriteSpace();
		}

		protected override void WriteOutOrRefKeyWord(ParameterDefinition parameter)
		{
			this.WriteKeyword(base.KeyWordWriter.ByRef);
		}

		protected override bool WritePropertyAsIndexer(PropertyDefinition property)
		{
			return false;
		}

		protected override void WritePropertyInterfaceImplementations(PropertyDefinition property)
		{
			ICollection<ImplementedMember> implementedProperties = property.GetImplementedProperties();
			if (!property.IsExplicitImplementation())
			{
				ICollection<ImplementedMember> notExplicitlyImplementedMembers = this.GetNotExplicitlyImplementedMembers(implementedProperties);
				if (notExplicitlyImplementedMembers.Any<ImplementedMember>())
				{
					this.WriteImplementedMembers(notExplicitlyImplementedMembers);
				}
			}
			else if (implementedProperties.Any<ImplementedMember>())
			{
				this.WriteImplementedMembers(implementedProperties);
				return;
			}
		}

		protected override void WritePropertyTypeAndNameWithArguments(PropertyDefinition property)
		{
			int currentPosition = this.formatter.CurrentPosition;
			this.WriteReference(this.GetPropertyName(property), property);
			int num = this.formatter.CurrentPosition - 1;
			this.currentWritingInfo.MemberDeclarationToCodePostionMap[property] = new OffsetSpan(currentPosition, num);
			if (base.HasArguments(property))
			{
				this.WriteToken("(");
				base.WritePropertyParameters(property);
				this.WriteToken(")");
			}
			this.WriteAsBetweenSpaces();
			base.AttributeWriter.WriteMemberReturnValueAttributes(property.GetMethod);
			this.WriteReferenceAndNamespaceIfInCollision(property.PropertyType);
		}

		protected override void WriteReadOnlyWriteOnlyProperty(PropertyDefinition property)
		{
			if (property.GetMethod == null)
			{
				this.WriteKeyword(base.KeyWordWriter.WriteOnly);
				this.WriteSpace();
				return;
			}
			if (property.SetMethod != null)
			{
				return;
			}
			this.WriteKeyword(base.KeyWordWriter.ReadOnly);
			this.WriteSpace();
		}

		protected override void WriteSpecialBetweenParenthesis(Expression expression)
		{
			this.Visit(expression);
		}

		protected override void WriteSpecialBetweenParenthesis(Action action)
		{
			this.WriteToken("(");
			action();
			this.WriteToken(")");
		}

		protected override void WriteSpecialEndBlock(string statementName)
		{
			this.WriteEndBlock(statementName);
		}

		protected override bool WriteTypeBaseTypes(TypeDefinition type, bool isPartial = false)
		{
			this.Indent();
			bool flag = base.WriteTypeBaseTypes(type, isPartial);
			this.Outdent();
			return flag;
		}

		protected override string WriteTypeDeclaration(TypeDefinition type, bool isPartial = false)
		{
			if (type.IsNested && type.IsStaticClass)
			{
				throw new Exception("VB.NET does not support nested modules. Please, try using other language.");
			}
			return base.WriteTypeDeclaration(type, isPartial);
		}

		protected override void WriteTypeInterfaces(TypeDefinition type, bool isPartial, bool baseTypeWritten)
		{
			List<TypeReference> typeReferences;
			typeReferences = (this.TypeContext.CurrentType != type || !this.TypeContext.IsWinRTImplementation ? type.Interfaces.ToList<TypeReference>() : type.Interfaces.Where<TypeReference>((TypeReference @interface) => {
				TypeDefinition typeDefinition = @interface.Resolve();
				if (typeDefinition == null)
				{
					return true;
				}
				return !typeDefinition.IsWindowsRuntime;
			}).ToList<TypeReference>());
			if (typeReferences.Count > 0)
			{
				int num = 0;
				for (int i = 0; i < typeReferences.Count; i++)
				{
					if (!isPartial || base.IsImplemented(type, typeReferences[i].Resolve()))
					{
						if (num == 0)
						{
							this.Indent();
							this.WriteInterfacesInheritColon(type);
							this.Outdent();
						}
						if (num > 0)
						{
							base.WriteInheritComma();
						}
						num++;
						this.WriteReferenceAndNamespaceIfInCollision(typeReferences[i]);
					}
				}
			}
		}

		protected override void WriteTypeSpecification(TypeSpecification typeSpecification, int startingArgument = 0)
		{
			if (!typeSpecification.IsOptionalModifier && !typeSpecification.IsRequiredModifier || this.isWritingComment)
			{
				base.WriteTypeSpecification(typeSpecification, startingArgument);
				return;
			}
			this.WriteReferenceAndNamespaceIfInCollision(typeSpecification.ElementType);
		}

		protected override void WriteTypeStaticKeywordAndSpace()
		{
		}

		protected override void WriteVolatileType(TypeReference reference)
		{
			this.WriteReferenceAndNamespaceIfInCollision(reference);
			this.WriteSpace();
			this.WriteKeyword(base.KeyWordWriter.Volatile);
		}
	}
}