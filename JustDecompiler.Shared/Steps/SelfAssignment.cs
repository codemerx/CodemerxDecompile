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
using System;
using System.Collections;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Pattern;
using Mono.Cecil;
using System.Collections.Generic;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Steps
{
	class ExpressionComparer : IEqualityComparer
	{
		public bool Equals(object x, object y)
		{
			if (x == y)
				return true;

			if (x == null)
				return y == null;
            
            Expression xAsExpression = x as Expression;
            Expression yAsExpression = y as Expression;
            if (xAsExpression != null && yAsExpression != null)
            {
                return xAsExpression.Equals(yAsExpression);
            }

			return false;
		}

		public int GetHashCode(object obj)
		{
			return obj.GetHashCode();
		}
	}

	public class SelfAssignment : BaseCodeTransformer, IDecompilationStep
	{
		const string TargetKey = "Target";
		const string OperatorKey = "Operator";
        const string ValueKey = "Value";
        const string RightSideKey = "RightSide";

        private TypeSystem typeSystem;
        private Dictionary<BinaryOperator, BinaryOperator> normalToAssignOperatorMap;

        public SelfAssignment()
        {
            this.normalToAssignOperatorMap = this.InitializeNormalToAssignOperatorMap();
        }

        static readonly Pattern.ICodePattern IncrementPattern = new Pattern.Assignment
		{
            Target = new SelfIncrementExpression() { Bind = target => new Pattern.MatchData(TargetKey, target) },
			Expression = new Pattern.Binary
			{
				Bind = binary => new Pattern.MatchData(OperatorKey, binary.Operator),
				Left = new Pattern.ContextData { Name = TargetKey, Comparer = new ExpressionComparer() },
				Right = new Pattern.Literal { Value = 1 },
                IsChecked = false
			}
		};

        private static readonly ICodePattern AssignmentOperatorPattern = new Assignment
        {
            Target = new SelfAssignmentExpression() { Bind = target => new Pattern.MatchData(TargetKey, target) },
            Expression = new Binary
            {
                Bind = binary => new MatchData(RightSideKey, binary),
                Left = new ContextData { Name = TargetKey, Comparer = new ExpressionComparer() },
                Right = new SelfAssignmentValue() { Bind = value => new MatchData(ValueKey, value) },
                IsChecked = false
            }
        };

        public override ICodeNode VisitBinaryExpression(BinaryExpression node)
        {
            if (node.IsAssignmentExpression)
            {
                return VisitAssignExpression(node);
            }
            return base.VisitBinaryExpression(node);
        }

		private ICodeNode VisitAssignExpression(BinaryExpression node)
		{
            MatchContext result;
            Expression target;

            result = Pattern.CodePattern.Match(IncrementPattern, node);
            if (result.Success)
            {
                target = (Expression)result[TargetKey];
                BinaryOperator @operator = (BinaryOperator)result[OperatorKey];

                switch (@operator)
                {
                    case BinaryOperator.Add:
                    case BinaryOperator.Subtract:
                        return new UnaryExpression(
                            GetCorrespondingOperator(@operator), target.CloneExpressionOnly(), node.UnderlyingSameMethodInstructions);
                }
            }

            result = Pattern.CodePattern.Match(AssignmentOperatorPattern, node);
            if (result.Success)
            {
                target = (Expression)result[TargetKey];
                BinaryExpression rightSide = (BinaryExpression)result[RightSideKey];
                Expression value = (Expression)result[ValueKey];

                if (this.normalToAssignOperatorMap.ContainsKey(rightSide.Operator))
                {
                    List<Instruction> instructions = new List<Instruction>();
                    instructions.AddRange(rightSide.MappedInstructions);
                    instructions.AddRange(target.MappedInstructions);

                    BinaryOperator newOperator = this.normalToAssignOperatorMap[rightSide.Operator];
                    Expression newLeft = target.CloneExpressionOnlyAndAttachInstructions(rightSide.Left.MappedInstructions);

                    return new BinaryExpression(newOperator, newLeft, value, this.typeSystem, instructions);
                }
            }

            return base.VisitBinaryExpression(node);
		}
        
        static UnaryOperator GetCorrespondingOperator(BinaryOperator @operator)
		{
			switch (@operator)
			{
				case BinaryOperator.Add:
					return UnaryOperator.PostIncrement;
				case BinaryOperator.Subtract:
					return UnaryOperator.PostDecrement;
				default:
					throw new ArgumentException();
			}
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
            this.typeSystem = context.MethodContext.Method.Module.TypeSystem;

            return (BlockStatement) VisitBlockStatement(body);
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

        private class SelfAssignmentExpression : CodePattern<Expression>
        {
            protected override bool OnMatch(MatchContext context, Expression node)
            {
                SelfAssignmentSafetyChecker checker = new SelfAssignmentSafetyChecker(this.ShouldSelfAssignPointers);

                return checker.IsSafeToSelfAssign(node);
            }

            protected virtual bool ShouldSelfAssignPointers
            {
                get
                {
                    return true;
                }
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
                    if (!isSafe || node == null)
                    {
                        return;
                    }

                    if (node.CodeNodeType == CodeNodeType.UnaryExpression)
                    {
                        UnaryExpression unary = node as UnaryExpression;
                        if (unary.Operator == UnaryOperator.AddressDereference)
                        {
                            if (this.includePointers ||
                                (unary.Operand.HasType && !unary.Operand.ExpressionType.IsPointer))
                            {
                                base.Visit(node);

                                return;
                            }
                        }
                    }

                    switch (node.CodeNodeType)
                    {
                        case CodeNodeType.LiteralExpression:
                        case CodeNodeType.ArgumentReferenceExpression:
                        case CodeNodeType.VariableReferenceExpression:
                        case CodeNodeType.ThisReferenceExpression:
                        case CodeNodeType.BaseReferenceExpression:
                        case CodeNodeType.FieldReferenceExpression:
                        case CodeNodeType.ExplicitCastExpression:
                        case CodeNodeType.ArrayIndexerExpression:
                        case CodeNodeType.EnumExpression:
                        case CodeNodeType.ArrayLengthExpression:
                        case CodeNodeType.ArrayAssignmentVariableReferenceExpression:
                        case CodeNodeType.ArrayAssignmentFieldReferenceExpression:
                        case CodeNodeType.ParenthesesExpression:
                            base.Visit(node);
                            return;
                    }

                    this.isSafe = false;
                    return;
                }
            }
        }

        private class SelfIncrementExpression : SelfAssignmentExpression
        {
            protected override bool ShouldSelfAssignPointers
            {
                get
                {
                    return false;
                }
            }
        }

        private class SelfAssignmentValue : CodePattern<Expression>
        {
            protected override bool OnMatch(MatchContext context, Expression node)
            {
                return node.CodeNodeType != CodeNodeType.BinaryExpression;
            }
        }
    }
}