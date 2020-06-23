using Mono.Cecil;
using Mono.Cecil.Cil;
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
			SelfAssignment.IncrementPattern = new Assignment()
			{
				Target = new SelfAssignment.SelfIncrementExpression()
				{
					Bind = (Expression target) => new MatchData("Target", target)
				},
				Expression = new Binary()
				{
					Bind = (BinaryExpression binary) => new MatchData("Operator", (object)binary.Operator),
					Left = new ContextData()
					{
						Name = "Target",
						Comparer = new ExpressionComparer()
					},
					Right = new Literal()
					{
						Value = 1
					},
					IsChecked = new bool?(false)
				}
			};
			SelfAssignment.AssignmentOperatorPattern = new Assignment()
			{
				Target = new SelfAssignment.SelfAssignmentExpression()
				{
					Bind = (Expression target) => new MatchData("Target", target)
				},
				Expression = new Binary()
				{
					Bind = (BinaryExpression binary) => new MatchData("RightSide", binary),
					Left = new ContextData()
					{
						Name = "Target",
						Comparer = new ExpressionComparer()
					},
					Right = new SelfAssignment.SelfAssignmentValue()
					{
						Bind = (Expression value) => new MatchData("Value", value)
					},
					IsChecked = new bool?(false)
				}
			};
		}

		public SelfAssignment()
		{
			this.normalToAssignOperatorMap = this.InitializeNormalToAssignOperatorMap();
		}

		private static UnaryOperator GetCorrespondingOperator(BinaryOperator @operator)
		{
			if (@operator == BinaryOperator.Add)
			{
				return UnaryOperator.PostIncrement;
			}
			if (@operator != BinaryOperator.Subtract)
			{
				throw new ArgumentException();
			}
			return UnaryOperator.PostDecrement;
		}

		protected virtual Dictionary<BinaryOperator, BinaryOperator> InitializeNormalToAssignOperatorMap()
		{
			return new Dictionary<BinaryOperator, BinaryOperator>()
			{
				{ BinaryOperator.Add, BinaryOperator.AddAssign },
				{ BinaryOperator.Subtract, BinaryOperator.SubtractAssign },
				{ BinaryOperator.Multiply, BinaryOperator.MultiplyAssign },
				{ BinaryOperator.Divide, BinaryOperator.DivideAssign },
				{ BinaryOperator.LeftShift, BinaryOperator.LeftShiftAssign },
				{ BinaryOperator.RightShift, BinaryOperator.RightShiftAssign },
				{ BinaryOperator.BitwiseOr, BinaryOperator.OrAssign },
				{ BinaryOperator.BitwiseAnd, BinaryOperator.AndAssign },
				{ BinaryOperator.BitwiseXor, BinaryOperator.XorAssign },
				{ BinaryOperator.Modulo, BinaryOperator.ModuloAssign }
			};
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.typeSystem = context.MethodContext.Method.Module.TypeSystem;
			return (BlockStatement)this.VisitBlockStatement(body);
		}

		private ICodeNode VisitAssignExpression(BinaryExpression node)
		{
			Expression item;
			MatchContext matchContext = CodePattern.Match(SelfAssignment.IncrementPattern, node);
			if (matchContext.Success)
			{
				item = (Expression)matchContext["Target"];
				BinaryOperator binaryOperator = (BinaryOperator)matchContext["Operator"];
				if (binaryOperator == BinaryOperator.Add || binaryOperator == BinaryOperator.Subtract)
				{
					return new UnaryExpression(SelfAssignment.GetCorrespondingOperator(binaryOperator), item.CloneExpressionOnly(), node.UnderlyingSameMethodInstructions);
				}
			}
			matchContext = CodePattern.Match(SelfAssignment.AssignmentOperatorPattern, node);
			if (matchContext.Success)
			{
				item = (Expression)matchContext["Target"];
				BinaryExpression binaryExpression = (BinaryExpression)matchContext["RightSide"];
				Expression expression = (Expression)matchContext["Value"];
				if (this.normalToAssignOperatorMap.ContainsKey(binaryExpression.Operator))
				{
					List<Instruction> instructions = new List<Instruction>();
					instructions.AddRange(binaryExpression.MappedInstructions);
					instructions.AddRange(item.MappedInstructions);
					BinaryOperator item1 = this.normalToAssignOperatorMap[binaryExpression.Operator];
					Expression expression1 = item.CloneExpressionOnlyAndAttachInstructions(binaryExpression.Left.MappedInstructions);
					return new BinaryExpression(item1, expression1, expression, this.typeSystem, instructions, false);
				}
			}
			return base.VisitBinaryExpression(node);
		}

		public override ICodeNode VisitBinaryExpression(BinaryExpression node)
		{
			if (node.IsAssignmentExpression)
			{
				return this.VisitAssignExpression(node);
			}
			return base.VisitBinaryExpression(node);
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
			}

			protected override bool OnMatch(MatchContext context, Expression node)
			{
				return (new SelfAssignment.SelfAssignmentExpression.SelfAssignmentSafetyChecker(this.ShouldSelfAssignPointers)).IsSafeToSelfAssign(node);
			}

			private class SelfAssignmentSafetyChecker : BaseCodeVisitor
			{
				private bool isSafe;

				private bool includePointers;

				public SelfAssignmentSafetyChecker(bool includePointers)
				{
					this.isSafe = true;
					this.includePointers = includePointers;
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
					if (node.CodeNodeType == CodeNodeType.UnaryExpression)
					{
						UnaryExpression unaryExpression = node as UnaryExpression;
						if (unaryExpression.Operator == UnaryOperator.AddressDereference && (this.includePointers || unaryExpression.Operand.HasType && !unaryExpression.Operand.ExpressionType.IsPointer))
						{
							base.Visit(node);
							return;
						}
					}
					CodeNodeType codeNodeType = node.CodeNodeType;
					if (codeNodeType <= CodeNodeType.EnumExpression)
					{
						switch (codeNodeType)
						{
							case CodeNodeType.LiteralExpression:
							case CodeNodeType.ArgumentReferenceExpression:
							case CodeNodeType.VariableReferenceExpression:
							case CodeNodeType.ThisReferenceExpression:
							case CodeNodeType.BaseReferenceExpression:
							case CodeNodeType.FieldReferenceExpression:
							case CodeNodeType.ExplicitCastExpression:
							{
								break;
							}
							case CodeNodeType.UnaryExpression:
							case CodeNodeType.BinaryExpression:
							case CodeNodeType.VariableDeclarationExpression:
							{
								this.isSafe = false;
								return;
							}
							default:
							{
								if (codeNodeType == CodeNodeType.ArrayIndexerExpression || codeNodeType == CodeNodeType.EnumExpression)
								{
									break;
								}
								this.isSafe = false;
								return;
							}
						}
					}
					else if (codeNodeType != CodeNodeType.ArrayLengthExpression && (int)codeNodeType - (int)CodeNodeType.ArrayAssignmentVariableReferenceExpression > (int)CodeNodeType.UnsafeBlock && codeNodeType != CodeNodeType.ParenthesesExpression)
					{
						this.isSafe = false;
						return;
					}
					base.Visit(node);
				}
			}
		}

		private class SelfAssignmentValue : CodePattern<Expression>
		{
			public SelfAssignmentValue()
			{
			}

			protected override bool OnMatch(MatchContext context, Expression node)
			{
				return node.CodeNodeType != CodeNodeType.BinaryExpression;
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
			}
		}
	}
}