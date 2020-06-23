using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class BinaryExpression : Expression
	{
		private bool typeSet;

		private Expression left;

		private Expression right;

		private TypeReference type;

		private readonly TypeSystem typeSystem;

		private static Dictionary<BinaryOperator, int> operatorsPriority;

		private bool? isObjectComparison;

		private readonly static object locker;

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				BinaryExpression binaryExpression = null;
				yield return binaryExpression.left;
				yield return binaryExpression.right;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.BinaryExpression;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				if (this.type == null)
				{
					this.UpdateType();
				}
				return this.type;
			}
			set
			{
				this.typeSet = true;
				this.type = value;
			}
		}

		public override bool HasType
		{
			get
			{
				if (this.type == null)
				{
					this.UpdateType();
				}
				return this.type != null;
			}
		}

		public bool IsAssignmentExpression
		{
			get
			{
				return this.Operator == BinaryOperator.Assign;
			}
		}

		public bool IsChecked
		{
			get;
			private set;
		}

		public bool IsComparisonExpression
		{
			get
			{
				BinaryOperator @operator = this.Operator;
				if ((int)@operator - (int)BinaryOperator.ValueEquality > (int)BinaryOperator.Add && (int)@operator - (int)BinaryOperator.LessThan > (int)BinaryOperator.Subtract)
				{
					return false;
				}
				return true;
			}
		}

		public bool IsEventHandlerAddOrRemove
		{
			get
			{
				if (this.Operator != BinaryOperator.AddAssign && this.Operator != BinaryOperator.SubtractAssign)
				{
					return false;
				}
				return this.Left.CodeNodeType == Telerik.JustDecompiler.Ast.CodeNodeType.EventReferenceExpression;
			}
		}

		public bool IsLogicalExpression
		{
			get
			{
				if (this.Operator == BinaryOperator.LogicalAnd)
				{
					return true;
				}
				return this.Operator == BinaryOperator.LogicalOr;
			}
		}

		public bool IsObjectComparison
		{
			get
			{
				if (!this.isObjectComparison.HasValue)
				{
					if (!this.IsComparisonExpression || this.Operator != BinaryOperator.ValueEquality && this.Operator != BinaryOperator.ValueInequality)
					{
						this.isObjectComparison = new bool?(false);
					}
					else if (!this.IsValidObjectComparisonNode(this.Left.CodeNodeType) || !this.IsValidObjectComparisonNode(this.Right.CodeNodeType))
					{
						this.isObjectComparison = new bool?(false);
					}
					else if (this.Left.ExpressionType is ArrayType || this.Right.ExpressionType is ArrayType)
					{
						this.isObjectComparison = new bool?(false);
					}
					else if (!this.instructions.Any<Instruction>((Instruction i) => {
						if (i.OpCode.Name != "call")
						{
							return false;
						}
						if ((i.Operand as MethodReference).Name == "op_Equality")
						{
							return true;
						}
						return (i.Operand as MethodReference).Name == "op_Inequality";
					}))
					{
						bool isValueType = !this.Left.ExpressionType.IsValueType;
						bool flag = !this.Right.ExpressionType.IsValueType;
						if (this.Left.ExpressionType.IsRequiredModifier || this.Left.ExpressionType.IsOptionalModifier)
						{
							isValueType = !this.Left.ExpressionType.Resolve().IsValueType;
						}
						if (this.Right.ExpressionType.IsRequiredModifier || this.Right.ExpressionType.IsOptionalModifier)
						{
							flag = !this.Right.ExpressionType.Resolve().IsValueType;
						}
						this.isObjectComparison = new bool?(isValueType & flag);
					}
					else
					{
						this.isObjectComparison = new bool?(false);
					}
				}
				return this.isObjectComparison.Value;
			}
			internal set
			{
				this.isObjectComparison = new bool?(value);
			}
		}

		public bool IsOverridenOperation
		{
			get;
			private set;
		}

		public bool IsSelfAssign
		{
			get
			{
				if (this.Operator == BinaryOperator.AddAssign || this.Operator == BinaryOperator.SubtractAssign || this.Operator == BinaryOperator.DivideAssign || this.Operator == BinaryOperator.MultiplyAssign || this.Operator == BinaryOperator.LeftShiftAssign || this.Operator == BinaryOperator.ModuloAssign || this.Operator == BinaryOperator.OrAssign || this.Operator == BinaryOperator.RightShiftAssign || this.Operator == BinaryOperator.AndAssign)
				{
					return true;
				}
				return this.Operator == BinaryOperator.XorAssign;
			}
		}

		public Expression Left
		{
			get
			{
				return this.left;
			}
			set
			{
				this.left = value;
				if (!this.typeSet)
				{
					this.UpdateType();
				}
			}
		}

		public BinaryOperator Operator
		{
			get;
			set;
		}

		public int OperatorPriority
		{
			get
			{
				lock (BinaryExpression.locker)
				{
					if (BinaryExpression.operatorsPriority == null)
					{
						BinaryExpression.InitializeOperatorsPrecedence();
					}
				}
				return BinaryExpression.operatorsPriority[this.Operator];
			}
		}

		public Expression Right
		{
			get
			{
				return this.right;
			}
			set
			{
				this.right = value;
				if (!this.typeSet)
				{
					this.UpdateType();
				}
			}
		}

		static BinaryExpression()
		{
			BinaryExpression.locker = new Object();
		}

		private BinaryExpression(BinaryOperator @operator, Expression left, Expression right, bool isChecked, IEnumerable<Instruction> instructions, bool isOverridenOperation) : base(instructions)
		{
			this.Operator = @operator;
			this.left = left;
			this.right = right;
			this.IsOverridenOperation = isOverridenOperation;
			this.IsChecked = isChecked;
			this.FixIfNullComparison();
		}

		public BinaryExpression(BinaryOperator @operator, Expression left, Expression right, TypeReference expressionType, TypeSystem typeSystem, IEnumerable<Instruction> instructions, bool isOverridenOperation = false) : this(@operator, left, right, BinaryExpression.DetermineIsChecked(instructions), instructions, isOverridenOperation)
		{
			this.ExpressionType = expressionType;
			this.typeSystem = typeSystem;
		}

		public BinaryExpression(BinaryOperator @operator, Expression left, Expression right, TypeSystem typeSystem, IEnumerable<Instruction> instructions, bool isOverridenOperation = false) : this(@operator, left, right, typeSystem, BinaryExpression.DetermineIsChecked(instructions), instructions, isOverridenOperation)
		{
		}

		public BinaryExpression(BinaryOperator @operator, Expression left, Expression right, TypeSystem typeSystem, bool isChecked, IEnumerable<Instruction> instructions, bool isOverridenOperation) : this(@operator, left, right, isChecked, instructions, isOverridenOperation)
		{
			this.typeSystem = typeSystem;
			this.UpdateType();
		}

		public override Expression Clone()
		{
			return new BinaryExpression(this.Operator, this.Left.Clone(), this.Right.Clone(), this.ExpressionType, this.typeSystem, this.instructions, false)
			{
				IsChecked = this.IsChecked,
				IsOverridenOperation = this.IsOverridenOperation,
				IsObjectComparison = this.IsObjectComparison
			};
		}

		public override Expression CloneExpressionOnly()
		{
			return new BinaryExpression(this.Operator, this.Left.CloneExpressionOnly(), this.Right.CloneExpressionOnly(), this.ExpressionType, this.typeSystem, null, false)
			{
				IsChecked = this.IsChecked,
				IsOverridenOperation = this.IsOverridenOperation,
				IsObjectComparison = this.IsObjectComparison
			};
		}

		public int CompareOperators(BinaryExpression other)
		{
			return this.OperatorPriority.CompareTo(other.OperatorPriority);
		}

		private static bool DetermineIsChecked(IEnumerable<Instruction> instructions)
		{
			bool flag;
			if (instructions == null)
			{
				return false;
			}
			using (IEnumerator<Instruction> enumerator = instructions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if ((int)enumerator.Current.OpCode.Code - (int)Code.Add_Ovf > (int)Code.Ldarg_3)
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			return flag;
		}

		public override bool Equals(Expression other)
		{
			if (!(other is BinaryExpression))
			{
				return false;
			}
			BinaryExpression binaryExpression = other as BinaryExpression;
			if (this.Operator != binaryExpression.Operator)
			{
				return false;
			}
			if (!this.Left.Equals(binaryExpression.Left))
			{
				return false;
			}
			return this.right.Equals(binaryExpression.Right);
		}

		private void FixIfNullComparison()
		{
			if (!this.IsComparisonExpression || this.Operator == BinaryOperator.ValueEquality || this.Operator == BinaryOperator.ValueInequality)
			{
				return;
			}
			if (this.left.CodeNodeType == Telerik.JustDecompiler.Ast.CodeNodeType.LiteralExpression && (this.left as LiteralExpression).Value == null || this.right.CodeNodeType == Telerik.JustDecompiler.Ast.CodeNodeType.LiteralExpression && (this.right as LiteralExpression).Value == null)
			{
				this.Operator = (this.Operator == BinaryOperator.LessThan || this.Operator == BinaryOperator.GreaterThan ? BinaryOperator.ValueInequality : BinaryOperator.ValueEquality);
			}
		}

		private TypeReference GetContainingType(TypeDefinition leftType, TypeDefinition rightType)
		{
			int? nullable;
			int? nullable1;
			if (leftType == rightType)
			{
				return rightType;
			}
			if (leftType == null)
			{
				return rightType;
			}
			if (rightType == null)
			{
				return leftType;
			}
			int? typeIndex = this.GetTypeIndex(leftType);
			int? typeIndex1 = this.GetTypeIndex(rightType);
			if (typeIndex.HasValue && typeIndex1.HasValue)
			{
				nullable1 = typeIndex;
				nullable = typeIndex1;
				if (nullable1.GetValueOrDefault() > nullable.GetValueOrDefault() & nullable1.HasValue & nullable.HasValue)
				{
					return leftType;
				}
				return rightType;
			}
			nullable = typeIndex;
			nullable1 = typeIndex1;
			if (!(nullable.GetValueOrDefault() == nullable1.GetValueOrDefault() & nullable.HasValue == nullable1.HasValue) || !(leftType.FullName == rightType.FullName))
			{
				string str = (typeIndex.HasValue ? "" : leftType.FullName);
				string str1 = (typeIndex1.HasValue ? "" : rightType.FullName);
				throw new Exception(String.Concat(new String[] { "Operation on type(s) of unknown size: ", str, " ", str1, ". Result size is platform dependent and cannot be determined at decompile time." }));
			}
			return leftType;
		}

		private void GetExpressionType()
		{
			TypeReference containingType = this.InferForPointers(this.Left.ExpressionType, this.Right.ExpressionType);
			if (containingType != null)
			{
				this.ExpressionType = containingType;
				return;
			}
			TypeDefinition expressionTypeDefinition = this.GetExpressionTypeDefinition(this.Left);
			TypeDefinition typeDefinition = this.GetExpressionTypeDefinition(this.Right);
			if (expressionTypeDefinition == null || typeDefinition == null)
			{
				return;
			}
			if (expressionTypeDefinition.FullName == typeDefinition.FullName)
			{
				this.ExpressionType = expressionTypeDefinition;
				return;
			}
			if (this.HandleDateTime(expressionTypeDefinition, typeDefinition))
			{
				return;
			}
			if (!(expressionTypeDefinition.IsEnum ^ typeDefinition.IsEnum))
			{
				containingType = this.GetContainingType(expressionTypeDefinition, typeDefinition);
				this.ExpressionType = containingType;
				return;
			}
			if (expressionTypeDefinition.IsEnum)
			{
				this.ExpressionType = expressionTypeDefinition;
				return;
			}
			this.ExpressionType = typeDefinition;
		}

		private TypeDefinition GetExpressionTypeDefinition(Expression ex)
		{
			TypeDefinition typeDefinition;
			TypeReference expressionType = ex.ExpressionType;
			if (expressionType == null)
			{
				return null;
			}
			typeDefinition = (!expressionType.IsPointer ? expressionType.Resolve() : this.typeSystem.IntPtr.Resolve());
			return typeDefinition;
		}

		private int? GetTypeIndex(TypeDefinition type)
		{
			if (type.FullName == "System.UIntPtr" || type.FullName == "System.IntPtr")
			{
				if (type.Module.Architecture == TargetArchitecture.I386 || type.Module.Architecture == TargetArchitecture.ARMv7)
				{
					return new int?(5);
				}
				if (type.Module.Architecture == TargetArchitecture.IA64 || type.Module.Architecture == TargetArchitecture.AMD64)
				{
					return new int?(7);
				}
				if (type.Module.Architecture == TargetArchitecture.AnyCPU)
				{
					return null;
				}
			}
			if (type.IsEnum)
			{
				FieldDefinition fieldDefinition = null;
				foreach (FieldDefinition field in type.Fields)
				{
					if (field.Name != "value__")
					{
						continue;
					}
					fieldDefinition = field;
					goto Label0;
				}
			Label0:
				type = fieldDefinition.FieldType.Resolve();
			}
			string fullName = type.FullName;
			if (fullName != null)
			{
				if (fullName == "System.Boolean")
				{
					return new int?(0);
				}
				if (fullName == "System.SByte" || fullName == "System.Byte")
				{
					return new int?(1);
				}
				if (fullName == "System.Char")
				{
					return new int?(2);
				}
				if (fullName == "System.Int16")
				{
					return new int?(3);
				}
				if (fullName == "System.UInt16")
				{
					return new int?(4);
				}
				if (fullName == "System.Int32")
				{
					return new int?(5);
				}
				if (fullName == "System.UInt32")
				{
					return new int?(6);
				}
				if (fullName == "System.Int64")
				{
					return new int?(7);
				}
				if (fullName == "System.UInt64")
				{
					return new int?(8);
				}
				if (fullName == "System.Single")
				{
					return new int?(9);
				}
				if (fullName == "System.Double")
				{
					return new int?(10);
				}
				if (fullName == "System.Decimal")
				{
					return new int?(11);
				}
			}
			throw new NotSupportedException(String.Format("Not supported type {0}.", type.FullName));
		}

		private bool HandleDateTime(TypeReference firstType, TypeReference secondType)
		{
			if (firstType.FullName == "System.DateTime" && secondType.FullName == "System.TimeSpan")
			{
				this.ExpressionType = firstType;
				return true;
			}
			if (!(secondType.FullName == "System.DateTime") || !(firstType.FullName == "System.TimeSpan"))
			{
				return false;
			}
			this.ExpressionType = secondType;
			return true;
		}

		private TypeReference InferForPointers(TypeReference firstType, TypeReference secondType)
		{
			if (firstType == null || secondType == null)
			{
				return null;
			}
			if ((firstType.IsPointer || firstType.IsPinned || firstType.IsByReference) && this.IsIntegerType(secondType))
			{
				return firstType;
			}
			if (!secondType.IsPointer && !secondType.IsPinned && !secondType.IsByReference || !this.IsIntegerType(firstType))
			{
				return null;
			}
			return secondType;
		}

		private static void InitializeOperatorsPrecedence()
		{
			BinaryExpression.operatorsPriority = new Dictionary<BinaryOperator, int>()
			{
				{ BinaryOperator.Multiply, 1 },
				{ BinaryOperator.Divide, 1 },
				{ BinaryOperator.Modulo, 1 },
				{ BinaryOperator.Add, 2 },
				{ BinaryOperator.Subtract, 2 },
				{ BinaryOperator.LeftShift, 3 },
				{ BinaryOperator.RightShift, 3 },
				{ BinaryOperator.LessThan, 4 },
				{ BinaryOperator.LessThanOrEqual, 4 },
				{ BinaryOperator.GreaterThan, 4 },
				{ BinaryOperator.GreaterThanOrEqual, 4 },
				{ BinaryOperator.ValueEquality, 5 },
				{ BinaryOperator.ValueInequality, 5 },
				{ BinaryOperator.BitwiseAnd, 6 },
				{ BinaryOperator.BitwiseXor, 7 },
				{ BinaryOperator.BitwiseOr, 8 },
				{ BinaryOperator.LogicalAnd, 9 },
				{ BinaryOperator.LogicalOr, 10 },
				{ BinaryOperator.NullCoalesce, 11 },
				{ BinaryOperator.Assign, 12 },
				{ BinaryOperator.AddAssign, 12 },
				{ BinaryOperator.SubtractAssign, 12 },
				{ BinaryOperator.MultiplyAssign, 12 },
				{ BinaryOperator.DivideAssign, 12 },
				{ BinaryOperator.LeftShiftAssign, 12 },
				{ BinaryOperator.RightShiftAssign, 12 },
				{ BinaryOperator.OrAssign, 12 },
				{ BinaryOperator.AndAssign, 12 },
				{ BinaryOperator.XorAssign, 12 },
				{ BinaryOperator.ModuloAssign, 12 },
				{ BinaryOperator.None, 13 }
			};
		}

		private bool IsIntegerType(TypeReference type)
		{
			if (!(type.FullName == "System.Int32") && !(type.FullName == "System.UInt32") && !(type.FullName == "System.Byte") && !(type.FullName == "System.SByte") && !(type.FullName == "System.Int16") && !(type.FullName == "System.UInt16") && !(type.FullName == "System.Int64") && !(type.FullName == "System.UInt64"))
			{
				return false;
			}
			return true;
		}

		private static bool IsPointerType(TypeDefinition type)
		{
			if (type.FullName == "System.UIntPtr")
			{
				return true;
			}
			return type.FullName == "System.IntPtr";
		}

		private bool IsValidObjectComparisonNode(Telerik.JustDecompiler.Ast.CodeNodeType codeNodeType)
		{
			if (codeNodeType == Telerik.JustDecompiler.Ast.CodeNodeType.AnonymousObjectCreationExpression || codeNodeType == Telerik.JustDecompiler.Ast.CodeNodeType.ArgumentReferenceExpression || codeNodeType == Telerik.JustDecompiler.Ast.CodeNodeType.FieldReferenceExpression || codeNodeType == Telerik.JustDecompiler.Ast.CodeNodeType.LiteralExpression || codeNodeType == Telerik.JustDecompiler.Ast.CodeNodeType.MethodInvocationExpression || codeNodeType == Telerik.JustDecompiler.Ast.CodeNodeType.ObjectCreationExpression || codeNodeType == Telerik.JustDecompiler.Ast.CodeNodeType.PropertyReferenceExpression || codeNodeType == Telerik.JustDecompiler.Ast.CodeNodeType.VariableReferenceExpression || codeNodeType == Telerik.JustDecompiler.Ast.CodeNodeType.ThisReferenceExpression || codeNodeType == Telerik.JustDecompiler.Ast.CodeNodeType.ArrayCreationExpression || codeNodeType == Telerik.JustDecompiler.Ast.CodeNodeType.ArrayIndexerExpression || codeNodeType == Telerik.JustDecompiler.Ast.CodeNodeType.LambdaExpression || codeNodeType == Telerik.JustDecompiler.Ast.CodeNodeType.LinqQueryExpression)
			{
				return true;
			}
			return codeNodeType == Telerik.JustDecompiler.Ast.CodeNodeType.UnaryExpression;
		}

		public void UpdateType()
		{
			if (this.Operator == BinaryOperator.NullCoalesce)
			{
				this.ExpressionType = this.Left.ExpressionType;
				return;
			}
			if (this.IsLogicalExpression || this.IsComparisonExpression)
			{
				this.type = this.typeSystem.Boolean;
				return;
			}
			if (this.Left.HasType && this.Right.HasType && this.Left.ExpressionType.FullName == this.Right.ExpressionType.FullName)
			{
				this.type = this.Left.ExpressionType;
				return;
			}
			if (!this.IsAssignmentExpression && (!this.IsSelfAssign || this.IsEventHandlerAddOrRemove))
			{
				this.GetExpressionType();
				return;
			}
			this.type = this.left.ExpressionType;
		}
	}
}