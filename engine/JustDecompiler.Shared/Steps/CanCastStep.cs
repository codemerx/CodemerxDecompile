#region license
//
//	(C) 2007 - 2008 Novell, Inc. http://www.novell.com
//	(C) 2007 - 2008 Jb Evain http://evain.net
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Mono.Cecil.Cil;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Steps
{
    public class CanCastStep
    {
        const string SafeCastKey = "SafeCast";
        private readonly BaseCodeTransformer codeTransformer;

        public CanCastStep(BaseCodeTransformer codeTransformer)
        {
            this.codeTransformer = codeTransformer;
        }

        static readonly Pattern.ICodePattern canCastToReferenceTypePattern = new Pattern.Binary
        {
            Left = new Pattern.SafeCast { Bind = sc => new Pattern.MatchData(SafeCastKey, sc) },
            Operator = new Pattern.Constant { Value = BinaryOperator.ValueInequality },
            Right = new Pattern.Literal { Value = null }
        };

        static readonly Pattern.ICodePattern negatedCanCastToReferenceTypePattern = new Pattern.Binary
        {
            Left = new Pattern.SafeCast { Bind = sc => new Pattern.MatchData(SafeCastKey, sc) },
            Operator = new Pattern.Constant { Value = BinaryOperator.ValueEquality },
            Right = new Pattern.Literal { Value = null }
        };

        static readonly Pattern.ICodePattern canCastToValueTypePattern = new Pattern.Binary
        {
            Left = new Pattern.SafeCast { Bind = sc => new Pattern.MatchData(SafeCastKey, sc) },
            Operator = new Pattern.Constant { Value = BinaryOperator.ValueInequality },
            Right = new Pattern.Literal { Value = 0 }
        };

        static readonly Pattern.ICodePattern negatedCanCastToValueTypePattern = new Pattern.Binary
        {
            Left = new Pattern.SafeCast { Bind = sc => new Pattern.MatchData(SafeCastKey, sc) },
            Operator = new Pattern.Constant { Value = BinaryOperator.ValueEquality },
            Right = new Pattern.Literal { Value = 0 }
        };

        static readonly Pattern.ICodePattern canCastComparisonToNullPattern = new Pattern.Binary
        {
            Left = new Pattern.SafeCast { Bind = sc => new Pattern.MatchData(SafeCastKey, sc) },
            Operator = new Pattern.Constant { Value = BinaryOperator.GreaterThan },
            Right = new Pattern.Literal { Value = null }
        };

        static readonly Pattern.ICodePattern negatedCanCastComparisonToNullPattern = new Pattern.Binary
        {
            Left = new Pattern.SafeCast { Bind = sc => new Pattern.MatchData(SafeCastKey, sc) },
            Operator = new Pattern.Constant { Value = BinaryOperator.LessThanOrEqual },
            Right = new Pattern.Literal { Value = null }
        };

        static readonly Pattern.ICodePattern canCastComparisonToZeroPattern = new Pattern.Binary
        {
            Left = new Pattern.SafeCast { Bind = sc => new Pattern.MatchData(SafeCastKey, sc) },
            Operator = new Pattern.Constant { Value = BinaryOperator.GreaterThan },
            Right = new Pattern.Literal { Value = 0 }
        };

        static readonly Pattern.ICodePattern negatedCanCastComparisonToZeroPattern = new Pattern.Binary
        {
            Left = new Pattern.SafeCast { Bind = sc => new Pattern.MatchData(SafeCastKey, sc) },
            Operator = new Pattern.Constant { Value = BinaryOperator.LessThanOrEqual },
            Right = new Pattern.Literal { Value = 0 }
        };

        static readonly Pattern.ICodePattern canCastComparisonToFalsePattern = new Pattern.Binary
        {
            Left = new Pattern.SafeCast { Bind = sc => new Pattern.MatchData(SafeCastKey, sc) },
            Operator = new Pattern.Constant { Value = BinaryOperator.ValueInequality },
            Right = new Pattern.Literal { Value = false }
        };

        static readonly Pattern.ICodePattern negatedCanCastComparisonToFalsePattern = new Pattern.Binary
        {
            Left = new Pattern.SafeCast { Bind = sc => new Pattern.MatchData(SafeCastKey, sc) },
            Operator = new Pattern.Constant { Value = BinaryOperator.ValueEquality },
            Right = new Pattern.Literal { Value = false }
        };

        public ICodeNode VisitBinaryExpression(BinaryExpression node)
        {
            CanCastExpression canCastExpression;
            if (TryMatchCanCastPattern(node, new Pattern.ICodePattern[]
                                             {
                                                 canCastToReferenceTypePattern, canCastToValueTypePattern, canCastComparisonToNullPattern,
                                                 canCastComparisonToZeroPattern, canCastComparisonToFalsePattern
                                             }, out canCastExpression))
            {
                return canCastExpression;
            }
            else if (TryMatchCanCastPattern(node, new Pattern.ICodePattern[]
                                                  {
                                                      negatedCanCastToReferenceTypePattern, negatedCanCastToValueTypePattern, negatedCanCastComparisonToNullPattern,
                                                      negatedCanCastComparisonToZeroPattern, negatedCanCastComparisonToFalsePattern
                                                  }, out canCastExpression))
            {
                return new UnaryExpression(UnaryOperator.LogicalNot, canCastExpression, null);
            }

            return null;
        }

        private bool TryMatchCanCastPattern(BinaryExpression node, IEnumerable<Pattern.ICodePattern> patterns, out CanCastExpression result)
        {
            foreach (Pattern.ICodePattern pattern in patterns)
            {
                Pattern.MatchContext matchContext = Pattern.CodePattern.Match(pattern, node);
                if (matchContext.Success)
                {
                    result = CreateCanCastExpression(matchContext, node);
                    return true;
                }
            }

            result = null;
            return false;
        }

        private CanCastExpression CreateCanCastExpression(Pattern.MatchContext matchContext, BinaryExpression node)
        {
            SafeCastExpression safeCast = (SafeCastExpression)matchContext[SafeCastKey];

            List<Instruction> instructions = new List<Instruction>();
            instructions.AddRange(safeCast.MappedInstructions);
            instructions.AddRange(node.MappedInstructions);
            instructions.AddRange(node.Right.UnderlyingSameMethodInstructions);

            return new CanCastExpression(
                (Expression)codeTransformer.Visit(safeCast.Expression),
                safeCast.TargetType,
                instructions);
        }
    }
}