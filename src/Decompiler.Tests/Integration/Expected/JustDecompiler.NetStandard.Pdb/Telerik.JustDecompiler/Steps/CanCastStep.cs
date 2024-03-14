using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Pattern;

namespace Telerik.JustDecompiler.Steps
{
	public class CanCastStep
	{
		private const string SafeCastKey = "SafeCast";

		private readonly BaseCodeTransformer codeTransformer;

		private readonly static ICodePattern canCastToReferenceTypePattern;

		private readonly static ICodePattern negatedCanCastToReferenceTypePattern;

		private readonly static ICodePattern canCastToValueTypePattern;

		private readonly static ICodePattern negatedCanCastToValueTypePattern;

		private readonly static ICodePattern canCastComparisonToNullPattern;

		private readonly static ICodePattern negatedCanCastComparisonToNullPattern;

		private readonly static ICodePattern canCastComparisonToZeroPattern;

		private readonly static ICodePattern negatedCanCastComparisonToZeroPattern;

		private readonly static ICodePattern canCastComparisonToFalsePattern;

		private readonly static ICodePattern negatedCanCastComparisonToFalsePattern;

		static CanCastStep()
		{
			CanCastStep.canCastToReferenceTypePattern = new Binary()
			{
				Left = new SafeCast()
				{
					Bind = (SafeCastExpression sc) => new MatchData("SafeCast", sc)
				},
				Operator = new Constant()
				{
					Value = BinaryOperator.ValueInequality
				},
				Right = new Literal()
				{
					Value = null
				}
			};
			CanCastStep.negatedCanCastToReferenceTypePattern = new Binary()
			{
				Left = new SafeCast()
				{
					Bind = (SafeCastExpression sc) => new MatchData("SafeCast", sc)
				},
				Operator = new Constant()
				{
					Value = BinaryOperator.ValueEquality
				},
				Right = new Literal()
				{
					Value = null
				}
			};
			CanCastStep.canCastToValueTypePattern = new Binary()
			{
				Left = new SafeCast()
				{
					Bind = (SafeCastExpression sc) => new MatchData("SafeCast", sc)
				},
				Operator = new Constant()
				{
					Value = BinaryOperator.ValueInequality
				},
				Right = new Literal()
				{
					Value = 0
				}
			};
			CanCastStep.negatedCanCastToValueTypePattern = new Binary()
			{
				Left = new SafeCast()
				{
					Bind = (SafeCastExpression sc) => new MatchData("SafeCast", sc)
				},
				Operator = new Constant()
				{
					Value = BinaryOperator.ValueEquality
				},
				Right = new Literal()
				{
					Value = 0
				}
			};
			CanCastStep.canCastComparisonToNullPattern = new Binary()
			{
				Left = new SafeCast()
				{
					Bind = (SafeCastExpression sc) => new MatchData("SafeCast", sc)
				},
				Operator = new Constant()
				{
					Value = BinaryOperator.GreaterThan
				},
				Right = new Literal()
				{
					Value = null
				}
			};
			CanCastStep.negatedCanCastComparisonToNullPattern = new Binary()
			{
				Left = new SafeCast()
				{
					Bind = (SafeCastExpression sc) => new MatchData("SafeCast", sc)
				},
				Operator = new Constant()
				{
					Value = BinaryOperator.LessThanOrEqual
				},
				Right = new Literal()
				{
					Value = null
				}
			};
			CanCastStep.canCastComparisonToZeroPattern = new Binary()
			{
				Left = new SafeCast()
				{
					Bind = (SafeCastExpression sc) => new MatchData("SafeCast", sc)
				},
				Operator = new Constant()
				{
					Value = BinaryOperator.GreaterThan
				},
				Right = new Literal()
				{
					Value = 0
				}
			};
			CanCastStep.negatedCanCastComparisonToZeroPattern = new Binary()
			{
				Left = new SafeCast()
				{
					Bind = (SafeCastExpression sc) => new MatchData("SafeCast", sc)
				},
				Operator = new Constant()
				{
					Value = BinaryOperator.LessThanOrEqual
				},
				Right = new Literal()
				{
					Value = 0
				}
			};
			CanCastStep.canCastComparisonToFalsePattern = new Binary()
			{
				Left = new SafeCast()
				{
					Bind = (SafeCastExpression sc) => new MatchData("SafeCast", sc)
				},
				Operator = new Constant()
				{
					Value = BinaryOperator.ValueInequality
				},
				Right = new Literal()
				{
					Value = false
				}
			};
			CanCastStep.negatedCanCastComparisonToFalsePattern = new Binary()
			{
				Left = new SafeCast()
				{
					Bind = (SafeCastExpression sc) => new MatchData("SafeCast", sc)
				},
				Operator = new Constant()
				{
					Value = BinaryOperator.ValueEquality
				},
				Right = new Literal()
				{
					Value = false
				}
			};
		}

		public CanCastStep(BaseCodeTransformer codeTransformer)
		{
			this.codeTransformer = codeTransformer;
		}

		private CanCastExpression CreateCanCastExpression(MatchContext matchContext, BinaryExpression node)
		{
			SafeCastExpression item = (SafeCastExpression)matchContext["SafeCast"];
			List<Instruction> instructions = new List<Instruction>();
			instructions.AddRange(item.MappedInstructions);
			instructions.AddRange(node.MappedInstructions);
			instructions.AddRange(node.Right.UnderlyingSameMethodInstructions);
			return new CanCastExpression((Expression)this.codeTransformer.Visit(item.Expression), item.TargetType, instructions);
		}

		private bool TryMatchCanCastPattern(BinaryExpression node, IEnumerable<ICodePattern> patterns, out CanCastExpression result)
		{
			bool flag;
			using (IEnumerator<ICodePattern> enumerator = patterns.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MatchContext matchContext = CodePattern.Match(enumerator.Current, node);
					if (!matchContext.Success)
					{
						continue;
					}
					result = this.CreateCanCastExpression(matchContext, node);
					flag = true;
					return flag;
				}
				result = null;
				return false;
			}
			return flag;
		}

		public ICodeNode VisitBinaryExpression(BinaryExpression node)
		{
			CanCastExpression canCastExpression;
			if (this.TryMatchCanCastPattern(node, (IEnumerable<ICodePattern>)(new ICodePattern[] { CanCastStep.canCastToReferenceTypePattern, CanCastStep.canCastToValueTypePattern, CanCastStep.canCastComparisonToNullPattern, CanCastStep.canCastComparisonToZeroPattern, CanCastStep.canCastComparisonToFalsePattern }), out canCastExpression))
			{
				return canCastExpression;
			}
			if (!this.TryMatchCanCastPattern(node, (IEnumerable<ICodePattern>)(new ICodePattern[] { CanCastStep.negatedCanCastToReferenceTypePattern, CanCastStep.negatedCanCastToValueTypePattern, CanCastStep.negatedCanCastComparisonToNullPattern, CanCastStep.negatedCanCastComparisonToZeroPattern, CanCastStep.negatedCanCastComparisonToFalsePattern }), out canCastExpression))
			{
				return null;
			}
			return new UnaryExpression(UnaryOperator.LogicalNot, canCastExpression, null);
		}
	}
}