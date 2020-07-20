using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Pattern;

namespace Telerik.JustDecompiler.Steps
{
	public class SelfAssignment : BaseCodeTransformer, IDecompilationStep
	{
		private const string TargetKey = "Target";

		private const string OperatorKey = "Operator";

		private const string ValueKey = "Value";

		private const string RightSideKey = "RightSide";

		private TypeSystem typeSystem;

		private Dictionary<BinaryOperator, BinaryOperator> normalToAssignOperatorMap;

		private readonly static ICodePattern IncrementPattern;

		private readonly static ICodePattern AssignmentOperatorPattern;

		static SelfAssignment()
		{
			stackVariable0 = new Assignment();
			stackVariable1 = new SelfAssignment.SelfIncrementExpression();
			stackVariable1.set_Bind(new Func<Expression, MatchData>(SelfAssignment.u003cu003ec.u003cu003e9.u003cu002ecctoru003eb__17_0));
			stackVariable0.set_Target(stackVariable1);
			stackVariable5 = new Binary();
			stackVariable5.set_Bind(new Func<BinaryExpression, MatchData>(SelfAssignment.u003cu003ec.u003cu003e9.u003cu002ecctoru003eb__17_1));
			stackVariable9 = new ContextData();
			stackVariable9.set_Name("Target");
			stackVariable9.set_Comparer(new ExpressionComparer());
			stackVariable5.set_Left(stackVariable9);
			stackVariable12 = new Literal();
			stackVariable12.set_Value(1);
			stackVariable5.set_Right(stackVariable12);
			stackVariable5.set_IsChecked(new bool?(false));
			stackVariable0.set_Expression(stackVariable5);
			SelfAssignment.IncrementPattern = stackVariable0;
			stackVariable17 = new Assignment();
			stackVariable18 = new SelfAssignment.SelfAssignmentExpression();
			stackVariable18.set_Bind(new Func<Expression, MatchData>(SelfAssignment.u003cu003ec.u003cu003e9.u003cu002ecctoru003eb__17_2));
			stackVariable17.set_Target(stackVariable18);
			stackVariable22 = new Binary();
			stackVariable22.set_Bind(new Func<BinaryExpression, MatchData>(SelfAssignment.u003cu003ec.u003cu003e9.u003cu002ecctoru003eb__17_3));
			stackVariable26 = new ContextData();
			stackVariable26.set_Name("Target");
			stackVariable26.set_Comparer(new ExpressionComparer());
			stackVariable22.set_Left(stackVariable26);
			stackVariable29 = new SelfAssignment.SelfAssignmentValue();
			stackVariable29.set_Bind(new Func<Expression, MatchData>(SelfAssignment.u003cu003ec.u003cu003e9.u003cu002ecctoru003eb__17_4));
			stackVariable22.set_Right(stackVariable29);
			stackVariable22.set_IsChecked(new bool?(false));
			stackVariable17.set_Expression(stackVariable22);
			SelfAssignment.AssignmentOperatorPattern = stackVariable17;
			return;
		}

		public SelfAssignment()
		{
			base();
			this.normalToAssignOperatorMap = this.InitializeNormalToAssignOperatorMap();
			return;
		}

		private static UnaryOperator GetCorrespondingOperator(BinaryOperator operator)
		{
			if (operator == 1)
			{
				return 4;
			}
			if (operator != 3)
			{
				throw new ArgumentException();
			}
			return 3;
		}

		protected virtual Dictionary<BinaryOperator, BinaryOperator> InitializeNormalToAssignOperatorMap()
		{
			stackVariable0 = new Dictionary<BinaryOperator, BinaryOperator>();
			stackVariable0.Add(1, 2);
			stackVariable0.Add(3, 4);
			stackVariable0.Add(5, 6);
			stackVariable0.Add(7, 8);
			stackVariable0.Add(17, 18);
			stackVariable0.Add(19, 20);
			stackVariable0.Add(21, 29);
			stackVariable0.Add(22, 28);
			stackVariable0.Add(23, 30);
			stackVariable0.Add(24, 25);
			return stackVariable0;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.typeSystem = context.get_MethodContext().get_Method().get_Module().get_TypeSystem();
			return (BlockStatement)this.VisitBlockStatement(body);
		}

		private ICodeNode VisitAssignExpression(BinaryExpression node)
		{
			V_0 = CodePattern.Match(SelfAssignment.IncrementPattern, node);
			if (V_0.get_Success())
			{
				V_1 = (Expression)V_0.get_Item("Target");
				V_2 = (BinaryOperator)V_0.get_Item("Operator");
				if (V_2 == 1 || V_2 == 3)
				{
					return new UnaryExpression(SelfAssignment.GetCorrespondingOperator(V_2), V_1.CloneExpressionOnly(), node.get_UnderlyingSameMethodInstructions());
				}
			}
			V_0 = CodePattern.Match(SelfAssignment.AssignmentOperatorPattern, node);
			if (V_0.get_Success())
			{
				V_1 = (Expression)V_0.get_Item("Target");
				V_3 = (BinaryExpression)V_0.get_Item("RightSide");
				V_4 = (Expression)V_0.get_Item("Value");
				if (this.normalToAssignOperatorMap.ContainsKey(V_3.get_Operator()))
				{
					V_5 = new List<Instruction>();
					V_5.AddRange(V_3.get_MappedInstructions());
					V_5.AddRange(V_1.get_MappedInstructions());
					stackVariable41 = this.normalToAssignOperatorMap.get_Item(V_3.get_Operator());
					V_6 = V_1.CloneExpressionOnlyAndAttachInstructions(V_3.get_Left().get_MappedInstructions());
					return new BinaryExpression(stackVariable41, V_6, V_4, this.typeSystem, V_5, false);
				}
			}
			return this.VisitBinaryExpression(node);
		}

		public override ICodeNode VisitBinaryExpression(BinaryExpression node)
		{
			if (node.get_IsAssignmentExpression())
			{
				return this.VisitAssignExpression(node);
			}
			return this.VisitBinaryExpression(node);
		}

		private class SelfAssignmentExpression : CodePattern<Expression>
		{
			protected virtual bool ShouldSelfAssignPointers
			{
				get
				{
					return true;
				}
			}

			public SelfAssignmentExpression()
			{
				base();
				return;
			}

			protected override bool OnMatch(MatchContext context, Expression node)
			{
				return (new SelfAssignment.SelfAssignmentExpression.SelfAssignmentSafetyChecker(this.get_ShouldSelfAssignPointers())).IsSafeToSelfAssign(node);
			}

			private class SelfAssignmentSafetyChecker : BaseCodeVisitor
			{
				private bool isSafe;

				private bool includePointers;

				public SelfAssignmentSafetyChecker(bool includePointers)
				{
					base();
					this.isSafe = true;
					this.includePointers = includePointers;
					return;
				}

				public bool IsSafeToSelfAssign(Expression expression)
				{
					this.isSafe = true;
					this.Visit(expression);
					return this.isSafe;
				}

				public override void Visit(ICodeNode node)
				{
					if (!this.isSafe || node == null)
					{
						return;
					}
					if (node.get_CodeNodeType() == 23)
					{
						V_0 = node as UnaryExpression;
						if (V_0.get_Operator() == 8 && this.includePointers || V_0.get_Operand().get_HasType() && !V_0.get_Operand().get_ExpressionType().get_IsPointer())
						{
							this.Visit(node);
							return;
						}
					}
					V_1 = node.get_CodeNodeType();
					if (V_1 > 49)
					{
						if (V_1 != 66 && V_1 - 83 > 1 && V_1 != 87)
						{
							goto Label0;
						}
					}
					else
					{
						switch (V_1 - 22)
						{
							case 0:
							case 3:
							case 4:
							case 6:
							case 7:
							case 8:
							case 9:
							{
								break;
							}
							case 1:
							case 2:
							case 5:
							{
								goto Label0;
							}
							default:
							{
								if (V_1 == 39 || V_1 == 49)
								{
									break;
								}
								goto Label0;
							}
						}
					}
					this.Visit(node);
					return;
				Label0:
					this.isSafe = false;
					return;
				}
			}
		}

		private class SelfAssignmentValue : CodePattern<Expression>
		{
			public SelfAssignmentValue()
			{
				base();
				return;
			}

			protected override bool OnMatch(MatchContext context, Expression node)
			{
				return node.get_CodeNodeType() != 24;
			}
		}

		private class SelfIncrementExpression : SelfAssignment.SelfAssignmentExpression
		{
			protected override bool ShouldSelfAssignPointers
			{
				get
				{
					return false;
				}
			}

			public SelfIncrementExpression()
			{
				base();
				return;
			}
		}
	}
}