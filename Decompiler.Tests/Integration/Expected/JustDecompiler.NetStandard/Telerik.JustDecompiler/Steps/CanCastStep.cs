using System;
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
			stackVariable0 = new Binary();
			stackVariable1 = new SafeCast();
			stackVariable1.set_Bind(new Func<SafeCastExpression, MatchData>(CanCastStep.u003cu003ec.u003cu003e9.u003cu002ecctoru003eb__16_0));
			stackVariable0.set_Left(stackVariable1);
			stackVariable5 = new Constant();
			stackVariable5.set_Value((BinaryOperator)10);
			stackVariable0.set_Operator(stackVariable5);
			stackVariable8 = new Literal();
			stackVariable8.set_Value(null);
			stackVariable0.set_Right(stackVariable8);
			CanCastStep.canCastToReferenceTypePattern = stackVariable0;
			stackVariable10 = new Binary();
			stackVariable11 = new SafeCast();
			stackVariable11.set_Bind(new Func<SafeCastExpression, MatchData>(CanCastStep.u003cu003ec.u003cu003e9.u003cu002ecctoru003eb__16_1));
			stackVariable10.set_Left(stackVariable11);
			stackVariable15 = new Constant();
			stackVariable15.set_Value((BinaryOperator)9);
			stackVariable10.set_Operator(stackVariable15);
			stackVariable18 = new Literal();
			stackVariable18.set_Value(null);
			stackVariable10.set_Right(stackVariable18);
			CanCastStep.negatedCanCastToReferenceTypePattern = stackVariable10;
			stackVariable20 = new Binary();
			stackVariable21 = new SafeCast();
			stackVariable21.set_Bind(new Func<SafeCastExpression, MatchData>(CanCastStep.u003cu003ec.u003cu003e9.u003cu002ecctoru003eb__16_2));
			stackVariable20.set_Left(stackVariable21);
			stackVariable25 = new Constant();
			stackVariable25.set_Value((BinaryOperator)10);
			stackVariable20.set_Operator(stackVariable25);
			stackVariable28 = new Literal();
			stackVariable28.set_Value(0);
			stackVariable20.set_Right(stackVariable28);
			CanCastStep.canCastToValueTypePattern = stackVariable20;
			stackVariable31 = new Binary();
			stackVariable32 = new SafeCast();
			stackVariable32.set_Bind(new Func<SafeCastExpression, MatchData>(CanCastStep.u003cu003ec.u003cu003e9.u003cu002ecctoru003eb__16_3));
			stackVariable31.set_Left(stackVariable32);
			stackVariable36 = new Constant();
			stackVariable36.set_Value((BinaryOperator)9);
			stackVariable31.set_Operator(stackVariable36);
			stackVariable39 = new Literal();
			stackVariable39.set_Value(0);
			stackVariable31.set_Right(stackVariable39);
			CanCastStep.negatedCanCastToValueTypePattern = stackVariable31;
			stackVariable42 = new Binary();
			stackVariable43 = new SafeCast();
			stackVariable43.set_Bind(new Func<SafeCastExpression, MatchData>(CanCastStep.u003cu003ec.u003cu003e9.u003cu002ecctoru003eb__16_4));
			stackVariable42.set_Left(stackVariable43);
			stackVariable47 = new Constant();
			stackVariable47.set_Value((BinaryOperator)15);
			stackVariable42.set_Operator(stackVariable47);
			stackVariable50 = new Literal();
			stackVariable50.set_Value(null);
			stackVariable42.set_Right(stackVariable50);
			CanCastStep.canCastComparisonToNullPattern = stackVariable42;
			stackVariable52 = new Binary();
			stackVariable53 = new SafeCast();
			stackVariable53.set_Bind(new Func<SafeCastExpression, MatchData>(CanCastStep.u003cu003ec.u003cu003e9.u003cu002ecctoru003eb__16_5));
			stackVariable52.set_Left(stackVariable53);
			stackVariable57 = new Constant();
			stackVariable57.set_Value((BinaryOperator)14);
			stackVariable52.set_Operator(stackVariable57);
			stackVariable60 = new Literal();
			stackVariable60.set_Value(null);
			stackVariable52.set_Right(stackVariable60);
			CanCastStep.negatedCanCastComparisonToNullPattern = stackVariable52;
			stackVariable62 = new Binary();
			stackVariable63 = new SafeCast();
			stackVariable63.set_Bind(new Func<SafeCastExpression, MatchData>(CanCastStep.u003cu003ec.u003cu003e9.u003cu002ecctoru003eb__16_6));
			stackVariable62.set_Left(stackVariable63);
			stackVariable67 = new Constant();
			stackVariable67.set_Value((BinaryOperator)15);
			stackVariable62.set_Operator(stackVariable67);
			stackVariable70 = new Literal();
			stackVariable70.set_Value(0);
			stackVariable62.set_Right(stackVariable70);
			CanCastStep.canCastComparisonToZeroPattern = stackVariable62;
			stackVariable73 = new Binary();
			stackVariable74 = new SafeCast();
			stackVariable74.set_Bind(new Func<SafeCastExpression, MatchData>(CanCastStep.u003cu003ec.u003cu003e9.u003cu002ecctoru003eb__16_7));
			stackVariable73.set_Left(stackVariable74);
			stackVariable78 = new Constant();
			stackVariable78.set_Value((BinaryOperator)14);
			stackVariable73.set_Operator(stackVariable78);
			stackVariable81 = new Literal();
			stackVariable81.set_Value(0);
			stackVariable73.set_Right(stackVariable81);
			CanCastStep.negatedCanCastComparisonToZeroPattern = stackVariable73;
			stackVariable84 = new Binary();
			stackVariable85 = new SafeCast();
			stackVariable85.set_Bind(new Func<SafeCastExpression, MatchData>(CanCastStep.u003cu003ec.u003cu003e9.u003cu002ecctoru003eb__16_8));
			stackVariable84.set_Left(stackVariable85);
			stackVariable89 = new Constant();
			stackVariable89.set_Value((BinaryOperator)10);
			stackVariable84.set_Operator(stackVariable89);
			stackVariable92 = new Literal();
			stackVariable92.set_Value(false);
			stackVariable84.set_Right(stackVariable92);
			CanCastStep.canCastComparisonToFalsePattern = stackVariable84;
			stackVariable95 = new Binary();
			stackVariable96 = new SafeCast();
			stackVariable96.set_Bind(new Func<SafeCastExpression, MatchData>(CanCastStep.u003cu003ec.u003cu003e9.u003cu002ecctoru003eb__16_9));
			stackVariable95.set_Left(stackVariable96);
			stackVariable100 = new Constant();
			stackVariable100.set_Value((BinaryOperator)9);
			stackVariable95.set_Operator(stackVariable100);
			stackVariable103 = new Literal();
			stackVariable103.set_Value(false);
			stackVariable95.set_Right(stackVariable103);
			CanCastStep.negatedCanCastComparisonToFalsePattern = stackVariable95;
			return;
		}

		public CanCastStep(BaseCodeTransformer codeTransformer)
		{
			base();
			this.codeTransformer = codeTransformer;
			return;
		}

		private CanCastExpression CreateCanCastExpression(MatchContext matchContext, BinaryExpression node)
		{
			V_0 = (SafeCastExpression)matchContext.get_Item("SafeCast");
			V_1 = new List<Instruction>();
			V_1.AddRange(V_0.get_MappedInstructions());
			V_1.AddRange(node.get_MappedInstructions());
			V_1.AddRange(node.get_Right().get_UnderlyingSameMethodInstructions());
			return new CanCastExpression((Expression)this.codeTransformer.Visit(V_0.get_Expression()), V_0.get_TargetType(), V_1);
		}

		private bool TryMatchCanCastPattern(BinaryExpression node, IEnumerable<ICodePattern> patterns, out CanCastExpression result)
		{
			V_0 = patterns.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = CodePattern.Match(V_0.get_Current(), node);
					if (!V_1.get_Success())
					{
						continue;
					}
					result = this.CreateCanCastExpression(V_1, node);
					V_2 = true;
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
			return V_2;
		Label0:
			result = null;
			return false;
		}

		public ICodeNode VisitBinaryExpression(BinaryExpression node)
		{
			stackVariable3 = new ICodePattern[5];
			stackVariable3[0] = CanCastStep.canCastToReferenceTypePattern;
			stackVariable3[1] = CanCastStep.canCastToValueTypePattern;
			stackVariable3[2] = CanCastStep.canCastComparisonToNullPattern;
			stackVariable3[3] = CanCastStep.canCastComparisonToZeroPattern;
			stackVariable3[4] = CanCastStep.canCastComparisonToFalsePattern;
			if (this.TryMatchCanCastPattern(node, (IEnumerable<ICodePattern>)stackVariable3, out V_0))
			{
				return V_0;
			}
			stackVariable19 = new ICodePattern[5];
			stackVariable19[0] = CanCastStep.negatedCanCastToReferenceTypePattern;
			stackVariable19[1] = CanCastStep.negatedCanCastToValueTypePattern;
			stackVariable19[2] = CanCastStep.negatedCanCastComparisonToNullPattern;
			stackVariable19[3] = CanCastStep.negatedCanCastComparisonToZeroPattern;
			stackVariable19[4] = CanCastStep.negatedCanCastComparisonToFalsePattern;
			if (!this.TryMatchCanCastPattern(node, (IEnumerable<ICodePattern>)stackVariable19, out V_0))
			{
				return null;
			}
			return new UnaryExpression(1, V_0, null);
		}
	}
}