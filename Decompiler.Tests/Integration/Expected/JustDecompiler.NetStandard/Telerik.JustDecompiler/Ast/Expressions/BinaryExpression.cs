using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
				stackVariable1 = new BinaryExpression.u003cget_Childrenu003ed__18(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 24;
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
				return;
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
				return (object)this.type != (object)null;
			}
		}

		public bool IsAssignmentExpression
		{
			get
			{
				return this.get_Operator() == 26;
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
				V_0 = this.get_Operator();
				if (V_0 - 9 > 1 && V_0 - 13 > 3)
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
				if (this.get_Operator() != 2 && this.get_Operator() != 4)
				{
					return false;
				}
				return this.get_Left().get_CodeNodeType() == 48;
			}
		}

		public bool IsLogicalExpression
		{
			get
			{
				if (this.get_Operator() == 12)
				{
					return true;
				}
				return this.get_Operator() == 11;
			}
		}

		public bool IsObjectComparison
		{
			get
			{
				if (!this.isObjectComparison.get_HasValue())
				{
					if (!this.get_IsComparisonExpression() || this.get_Operator() != 9 && this.get_Operator() != 10)
					{
						this.isObjectComparison = new bool?(false);
					}
					else
					{
						if (!this.IsValidObjectComparisonNode(this.get_Left().get_CodeNodeType()) || !this.IsValidObjectComparisonNode(this.get_Right().get_CodeNodeType()))
						{
							this.isObjectComparison = new bool?(false);
						}
						else
						{
							if (this.get_Left().get_ExpressionType() as ArrayType != null || this.get_Right().get_ExpressionType() as ArrayType != null)
							{
								this.isObjectComparison = new bool?(false);
							}
							else
							{
								stackVariable39 = this.instructions;
								stackVariable40 = BinaryExpression.u003cu003ec.u003cu003e9__51_0;
								if (stackVariable40 == null)
								{
									dummyVar0 = stackVariable40;
									stackVariable40 = new Func<Instruction, bool>(BinaryExpression.u003cu003ec.u003cu003e9.u003cget_IsObjectComparisonu003eb__51_0);
									BinaryExpression.u003cu003ec.u003cu003e9__51_0 = stackVariable40;
								}
								if (!stackVariable39.Any<Instruction>(stackVariable40))
								{
									V_0 = !this.get_Left().get_ExpressionType().get_IsValueType();
									V_1 = !this.get_Right().get_ExpressionType().get_IsValueType();
									if (this.get_Left().get_ExpressionType().get_IsRequiredModifier() || this.get_Left().get_ExpressionType().get_IsOptionalModifier())
									{
										V_0 = !this.get_Left().get_ExpressionType().Resolve().get_IsValueType();
									}
									if (this.get_Right().get_ExpressionType().get_IsRequiredModifier() || this.get_Right().get_ExpressionType().get_IsOptionalModifier())
									{
										V_1 = !this.get_Right().get_ExpressionType().Resolve().get_IsValueType();
									}
									this.isObjectComparison = new bool?(V_0 & V_1);
								}
								else
								{
									this.isObjectComparison = new bool?(false);
								}
							}
						}
					}
				}
				return this.isObjectComparison.get_Value();
			}
			internal set
			{
				this.isObjectComparison = new bool?(value);
				return;
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
				if (this.get_Operator() == 2 || this.get_Operator() == 4 || this.get_Operator() == 8 || this.get_Operator() == 6 || this.get_Operator() == 18 || this.get_Operator() == 25 || this.get_Operator() == 29 || this.get_Operator() == 20 || this.get_Operator() == 28)
				{
					return true;
				}
				return this.get_Operator() == 30;
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
				return;
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
				V_0 = BinaryExpression.locker;
				V_1 = false;
				try
				{
					Monitor.Enter(V_0, ref V_1);
					if (BinaryExpression.operatorsPriority == null)
					{
						BinaryExpression.InitializeOperatorsPrecedence();
					}
				}
				finally
				{
					if (V_1)
					{
						Monitor.Exit(V_0);
					}
				}
				return BinaryExpression.operatorsPriority.get_Item(this.get_Operator());
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
				return;
			}
		}

		static BinaryExpression()
		{
			BinaryExpression.locker = new Object();
			return;
		}

		private BinaryExpression(BinaryOperator operator, Expression left, Expression right, bool isChecked, IEnumerable<Instruction> instructions, bool isOverridenOperation)
		{
			base(instructions);
			this.set_Operator(operator);
			this.left = left;
			this.right = right;
			this.set_IsOverridenOperation(isOverridenOperation);
			this.set_IsChecked(isChecked);
			this.FixIfNullComparison();
			return;
		}

		public BinaryExpression(BinaryOperator operator, Expression left, Expression right, TypeReference expressionType, TypeSystem typeSystem, IEnumerable<Instruction> instructions, bool isOverridenOperation = false)
		{
			this(operator, left, right, BinaryExpression.DetermineIsChecked(instructions), instructions, isOverridenOperation);
			this.set_ExpressionType(expressionType);
			this.typeSystem = typeSystem;
			return;
		}

		public BinaryExpression(BinaryOperator operator, Expression left, Expression right, TypeSystem typeSystem, IEnumerable<Instruction> instructions, bool isOverridenOperation = false)
		{
			this(operator, left, right, typeSystem, BinaryExpression.DetermineIsChecked(instructions), instructions, isOverridenOperation);
			return;
		}

		public BinaryExpression(BinaryOperator operator, Expression left, Expression right, TypeSystem typeSystem, bool isChecked, IEnumerable<Instruction> instructions, bool isOverridenOperation)
		{
			this(operator, left, right, isChecked, instructions, isOverridenOperation);
			this.typeSystem = typeSystem;
			this.UpdateType();
			return;
		}

		public override Expression Clone()
		{
			stackVariable15 = new BinaryExpression(this.get_Operator(), this.get_Left().Clone(), this.get_Right().Clone(), this.get_ExpressionType(), this.typeSystem, this.instructions, false);
			stackVariable15.set_IsChecked(this.get_IsChecked());
			stackVariable15.set_IsOverridenOperation(this.get_IsOverridenOperation());
			stackVariable15.set_IsObjectComparison(this.get_IsObjectComparison());
			return stackVariable15;
		}

		public override Expression CloneExpressionOnly()
		{
			stackVariable14 = new BinaryExpression(this.get_Operator(), this.get_Left().CloneExpressionOnly(), this.get_Right().CloneExpressionOnly(), this.get_ExpressionType(), this.typeSystem, null, false);
			stackVariable14.set_IsChecked(this.get_IsChecked());
			stackVariable14.set_IsOverridenOperation(this.get_IsOverridenOperation());
			stackVariable14.set_IsObjectComparison(this.get_IsObjectComparison());
			return stackVariable14;
		}

		public int CompareOperators(BinaryExpression other)
		{
			V_0 = this.get_OperatorPriority();
			return V_0.CompareTo(other.get_OperatorPriority());
		}

		private static bool DetermineIsChecked(IEnumerable<Instruction> instructions)
		{
			if (instructions == null)
			{
				return false;
			}
			V_0 = instructions.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					if (V_0.get_Current().get_OpCode().get_Code() - 180 > 5)
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
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
		Label1:
			return V_3;
		Label0:
			return false;
		}

		public override bool Equals(Expression other)
		{
			if (other as BinaryExpression == null)
			{
				return false;
			}
			V_0 = other as BinaryExpression;
			if (this.get_Operator() != V_0.get_Operator())
			{
				return false;
			}
			if (!this.get_Left().Equals(V_0.get_Left()))
			{
				return false;
			}
			return this.right.Equals(V_0.get_Right());
		}

		private void FixIfNullComparison()
		{
			if (!this.get_IsComparisonExpression() || this.get_Operator() == 9 || this.get_Operator() == 10)
			{
				return;
			}
			if (this.left.get_CodeNodeType() == 22 && (this.left as LiteralExpression).get_Value() == null || this.right.get_CodeNodeType() == 22 && (this.right as LiteralExpression).get_Value() == null)
			{
				if (this.get_Operator() == 13 || this.get_Operator() == 15)
				{
					stackVariable24 = 10;
				}
				else
				{
					stackVariable24 = 9;
				}
				this.set_Operator(stackVariable24);
			}
			return;
		}

		private TypeReference GetContainingType(TypeDefinition leftType, TypeDefinition rightType)
		{
			if ((object)leftType == (object)rightType)
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
			V_0 = this.GetTypeIndex(leftType);
			V_1 = this.GetTypeIndex(rightType);
			if (V_0.get_HasValue() && V_1.get_HasValue())
			{
				V_5 = V_0;
				V_4 = V_1;
				if (V_5.GetValueOrDefault() > V_4.GetValueOrDefault() & V_5.get_HasValue() & V_4.get_HasValue())
				{
					return leftType;
				}
				return rightType;
			}
			V_4 = V_0;
			V_5 = V_1;
			if (!V_4.GetValueOrDefault() == V_5.GetValueOrDefault() & V_4.get_HasValue() == V_5.get_HasValue() || !String.op_Equality(leftType.get_FullName(), rightType.get_FullName()))
			{
				if (V_0.get_HasValue())
				{
					stackVariable27 = "";
				}
				else
				{
					stackVariable27 = leftType.get_FullName();
				}
				V_2 = stackVariable27;
				if (V_1.get_HasValue())
				{
					stackVariable30 = "";
				}
				else
				{
					stackVariable30 = rightType.get_FullName();
				}
				V_3 = stackVariable30;
				stackVariable32 = new String[5];
				stackVariable32[0] = "Operation on type(s) of unknown size: ";
				stackVariable32[1] = V_2;
				stackVariable32[2] = " ";
				stackVariable32[3] = V_3;
				stackVariable32[4] = ". Result size is platform dependent and cannot be determined at decompile time.";
				throw new Exception(String.Concat(stackVariable32));
			}
			return leftType;
		}

		private void GetExpressionType()
		{
			V_0 = this.InferForPointers(this.get_Left().get_ExpressionType(), this.get_Right().get_ExpressionType());
			if (V_0 != null)
			{
				this.set_ExpressionType(V_0);
				return;
			}
			V_1 = this.GetExpressionTypeDefinition(this.get_Left());
			V_2 = this.GetExpressionTypeDefinition(this.get_Right());
			if (V_1 == null || V_2 == null)
			{
				return;
			}
			if (String.op_Equality(V_1.get_FullName(), V_2.get_FullName()))
			{
				this.set_ExpressionType(V_1);
				return;
			}
			if (this.HandleDateTime(V_1, V_2))
			{
				return;
			}
			if (!V_1.get_IsEnum() ^ V_2.get_IsEnum())
			{
				V_0 = this.GetContainingType(V_1, V_2);
				this.set_ExpressionType(V_0);
				return;
			}
			if (V_1.get_IsEnum())
			{
				this.set_ExpressionType(V_1);
				return;
			}
			this.set_ExpressionType(V_2);
			return;
		}

		private TypeDefinition GetExpressionTypeDefinition(Expression ex)
		{
			V_0 = ex.get_ExpressionType();
			if (V_0 == null)
			{
				return null;
			}
			if (!V_0.get_IsPointer())
			{
				V_1 = V_0.Resolve();
			}
			else
			{
				V_1 = this.typeSystem.get_IntPtr().Resolve();
			}
			return V_1;
		}

		private int? GetTypeIndex(TypeDefinition type)
		{
			if (String.op_Equality(type.get_FullName(), "System.UIntPtr") || String.op_Equality(type.get_FullName(), "System.IntPtr"))
			{
				if (type.get_Module().get_Architecture() == null || type.get_Module().get_Architecture() == 4)
				{
					return new int?(5);
				}
				if (type.get_Module().get_Architecture() == 2 || type.get_Module().get_Architecture() == 1)
				{
					return new int?(7);
				}
				if (type.get_Module().get_Architecture() == 3)
				{
					V_0 = null;
					return V_0;
				}
			}
			if (type.get_IsEnum())
			{
				V_1 = null;
				V_2 = type.get_Fields().GetEnumerator();
				try
				{
					while (V_2.MoveNext())
					{
						V_3 = V_2.get_Current();
						if (!String.op_Equality(V_3.get_Name(), "value__"))
						{
							continue;
						}
						V_1 = V_3;
						goto Label0;
					}
				}
				finally
				{
					V_2.Dispose();
				}
			Label0:
				type = V_1.get_FieldType().Resolve();
			}
			V_4 = type.get_FullName();
			if (V_4 != null)
			{
				if (String.op_Equality(V_4, "System.Boolean"))
				{
					return new int?(0);
				}
				if (String.op_Equality(V_4, "System.SByte") || String.op_Equality(V_4, "System.Byte"))
				{
					return new int?(1);
				}
				if (String.op_Equality(V_4, "System.Char"))
				{
					return new int?(2);
				}
				if (String.op_Equality(V_4, "System.Int16"))
				{
					return new int?(3);
				}
				if (String.op_Equality(V_4, "System.UInt16"))
				{
					return new int?(4);
				}
				if (String.op_Equality(V_4, "System.Int32"))
				{
					return new int?(5);
				}
				if (String.op_Equality(V_4, "System.UInt32"))
				{
					return new int?(6);
				}
				if (String.op_Equality(V_4, "System.Int64"))
				{
					return new int?(7);
				}
				if (String.op_Equality(V_4, "System.UInt64"))
				{
					return new int?(8);
				}
				if (String.op_Equality(V_4, "System.Single"))
				{
					return new int?(9);
				}
				if (String.op_Equality(V_4, "System.Double"))
				{
					return new int?(10);
				}
				if (String.op_Equality(V_4, "System.Decimal"))
				{
					return new int?(11);
				}
			}
			throw new NotSupportedException(String.Format("Not supported type {0}.", type.get_FullName()));
		}

		private bool HandleDateTime(TypeReference firstType, TypeReference secondType)
		{
			if (String.op_Equality(firstType.get_FullName(), "System.DateTime") && String.op_Equality(secondType.get_FullName(), "System.TimeSpan"))
			{
				this.set_ExpressionType(firstType);
				return true;
			}
			if (!String.op_Equality(secondType.get_FullName(), "System.DateTime") || !String.op_Equality(firstType.get_FullName(), "System.TimeSpan"))
			{
				return false;
			}
			this.set_ExpressionType(secondType);
			return true;
		}

		private TypeReference InferForPointers(TypeReference firstType, TypeReference secondType)
		{
			if (firstType == null || secondType == null)
			{
				return null;
			}
			if (firstType.get_IsPointer() || firstType.get_IsPinned() || firstType.get_IsByReference() && this.IsIntegerType(secondType))
			{
				return firstType;
			}
			if (!secondType.get_IsPointer() && !secondType.get_IsPinned() && !secondType.get_IsByReference() || !this.IsIntegerType(firstType))
			{
				return null;
			}
			return secondType;
		}

		private static void InitializeOperatorsPrecedence()
		{
			BinaryExpression.operatorsPriority = new Dictionary<BinaryOperator, int>();
			BinaryExpression.operatorsPriority.Add(5, 1);
			BinaryExpression.operatorsPriority.Add(7, 1);
			BinaryExpression.operatorsPriority.Add(24, 1);
			BinaryExpression.operatorsPriority.Add(1, 2);
			BinaryExpression.operatorsPriority.Add(3, 2);
			BinaryExpression.operatorsPriority.Add(17, 3);
			BinaryExpression.operatorsPriority.Add(19, 3);
			BinaryExpression.operatorsPriority.Add(13, 4);
			BinaryExpression.operatorsPriority.Add(14, 4);
			BinaryExpression.operatorsPriority.Add(15, 4);
			BinaryExpression.operatorsPriority.Add(16, 4);
			BinaryExpression.operatorsPriority.Add(9, 5);
			BinaryExpression.operatorsPriority.Add(10, 5);
			BinaryExpression.operatorsPriority.Add(22, 6);
			BinaryExpression.operatorsPriority.Add(23, 7);
			BinaryExpression.operatorsPriority.Add(21, 8);
			BinaryExpression.operatorsPriority.Add(12, 9);
			BinaryExpression.operatorsPriority.Add(11, 10);
			BinaryExpression.operatorsPriority.Add(27, 11);
			BinaryExpression.operatorsPriority.Add(26, 12);
			BinaryExpression.operatorsPriority.Add(2, 12);
			BinaryExpression.operatorsPriority.Add(4, 12);
			BinaryExpression.operatorsPriority.Add(6, 12);
			BinaryExpression.operatorsPriority.Add(8, 12);
			BinaryExpression.operatorsPriority.Add(18, 12);
			BinaryExpression.operatorsPriority.Add(20, 12);
			BinaryExpression.operatorsPriority.Add(29, 12);
			BinaryExpression.operatorsPriority.Add(28, 12);
			BinaryExpression.operatorsPriority.Add(30, 12);
			BinaryExpression.operatorsPriority.Add(25, 12);
			BinaryExpression.operatorsPriority.Add(0, 13);
			return;
		}

		private bool IsIntegerType(TypeReference type)
		{
			if (!String.op_Equality(type.get_FullName(), "System.Int32") && !String.op_Equality(type.get_FullName(), "System.UInt32") && !String.op_Equality(type.get_FullName(), "System.Byte") && !String.op_Equality(type.get_FullName(), "System.SByte") && !String.op_Equality(type.get_FullName(), "System.Int16") && !String.op_Equality(type.get_FullName(), "System.UInt16") && !String.op_Equality(type.get_FullName(), "System.Int64") && !String.op_Equality(type.get_FullName(), "System.UInt64"))
			{
				return false;
			}
			return true;
		}

		private static bool IsPointerType(TypeDefinition type)
		{
			if (String.op_Equality(type.get_FullName(), "System.UIntPtr"))
			{
				return true;
			}
			return String.op_Equality(type.get_FullName(), "System.IntPtr");
		}

		private bool IsValidObjectComparisonNode(Telerik.JustDecompiler.Ast.CodeNodeType codeNodeType)
		{
			if (codeNodeType == 72 || codeNodeType == 25 || codeNodeType == 30 || codeNodeType == 22 || codeNodeType == 19 || codeNodeType == 40 || codeNodeType == 42 || codeNodeType == 26 || codeNodeType == 28 || codeNodeType == 38 || codeNodeType == 39 || codeNodeType == 50 || codeNodeType == 81)
			{
				return true;
			}
			return codeNodeType == 23;
		}

		public void UpdateType()
		{
			if (this.get_Operator() == 27)
			{
				this.set_ExpressionType(this.get_Left().get_ExpressionType());
				return;
			}
			if (this.get_IsLogicalExpression() || this.get_IsComparisonExpression())
			{
				this.type = this.typeSystem.get_Boolean();
				return;
			}
			if (this.get_Left().get_HasType() && this.get_Right().get_HasType() && String.op_Equality(this.get_Left().get_ExpressionType().get_FullName(), this.get_Right().get_ExpressionType().get_FullName()))
			{
				this.type = this.get_Left().get_ExpressionType();
				return;
			}
			if (!this.get_IsAssignmentExpression() && !this.get_IsSelfAssign() || this.get_IsEventHandlerAddOrRemove())
			{
				this.GetExpressionType();
				return;
			}
			this.type = this.left.get_ExpressionType();
			return;
		}
	}
}