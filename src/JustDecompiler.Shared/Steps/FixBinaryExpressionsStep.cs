using System;
using System.Linq;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Mono.Cecil;
using Telerik.JustDecompiler.Decompiler;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps
{
    /// <summary>
    /// Performs fixes on all binary expressions, following the ExpressionDecompilerStep.
    /// Mostly changes literal values, to represent more correctly their supposed type,
    /// i.e. changes '0' to 'false', int to char and so on.
    /// </summary>
    class FixBinaryExpressionsStep : BaseCodeTransformer, IDecompilationStep
    {
        private readonly TypeSystem typeSystem;

        public FixBinaryExpressionsStep(TypeSystem typeSystem)
        {
            this.typeSystem = typeSystem;
        }

        /// <summary>
        /// The entry point for the step.
        /// </summary>
        /// <param name="context">The decompilation context.</param>
        /// <param name="body">The body of the method.</param>
        /// <returns>Returns the updated body of the method.</returns>
        public BlockStatement Process(DecompilationContext context, BlockStatement body)
        {
            MethodSpecificContext methodContext = context.MethodContext;
            foreach (int key in methodContext.Expressions.BlockExpressions.Keys)
            {
                IList<Expression> expressionList = methodContext.Expressions.BlockExpressions[key];
                Code lastInstructionCode = methodContext.ControlFlowGraph.InstructionToBlockMapping[key].Last.OpCode.Code;
                bool endsWithConditionalJump = lastInstructionCode == Code.Brtrue || lastInstructionCode == Code.Brtrue_S ||
                                               lastInstructionCode == Code.Brfalse || lastInstructionCode == Code.Brfalse_S;
                for (int i = 0; i < expressionList.Count; i++)
                {
                    expressionList[i] = (Expression)Visit(expressionList[i]);
                }
                if (endsWithConditionalJump)
                {
                    expressionList[expressionList.Count - 1] = (Expression)
                        FixBranchingExpression(expressionList[expressionList.Count - 1], methodContext.ControlFlowGraph.InstructionToBlockMapping[key].Last);
                }
                //if (lastInstructionCode == Code.Switch)
                //{
                //    //the type of this expression is needed if the switch instruction causes IrregularSwitchLC
                //    //so that correct expressions can be produced for case's conditions
                //    //Expression lastExpression = expressionList[expressionList.Count - 1];
                //}
            }
            return body;
        }

		private bool IsBooleanAssignmentOperator(BinaryOperator @operator)
		{
			return (@operator == BinaryOperator.Assign) || (@operator == BinaryOperator.AndAssign) || (@operator == BinaryOperator.OrAssign) || (@operator == BinaryOperator.XorAssign);
		}

        /// <summary>
        /// The method for updating BinaryExpression.
        /// </summary>
        /// <param name="expression">The binary expression to be updated.</param>
        /// <returns>Returns the updated binary expression.</returns>
        public override ICodeNode VisitBinaryExpression(BinaryExpression expression)
        {
            ///Update the nested expressions first.
            expression.Left = (Expression)Visit(expression.Left);
            expression.Right = (Expression)Visit(expression.Right);

            TypeReference leftType = expression.Left.ExpressionType;
            leftType = GetElementType(leftType);

            if (leftType != null)
            {
                /// Check for chars.
                if (leftType.FullName == typeSystem.Char.FullName)
                {
                    if (expression.Right.CodeNodeType == CodeNodeType.LiteralExpression)
                    {
                        if (IsArithmeticOperator(expression.Operator))
                        {
                            /// Might need to add cast to char at this point.
                            // return new CastExpression(expression, typeSystem.Char);
                            expression.ExpressionType = typeSystem.Char;
                            return expression;
                        }
                        if (expression.Right.HasType)
                        {
                            TypeReference rightLitType = GetElementType(expression.Right.ExpressionType);
                            if (leftType.FullName == rightLitType.FullName)
                            {
                                return expression;
                            }
                        }
                        LiteralExpression right = expression.Right as LiteralExpression;
                        expression.Right = GetLiteralExpression((char)((int)right.Value), right.MappedInstructions);
                    }
                    TypeReference rightType = GetElementType(expression.Right.ExpressionType);
                    if (rightType.FullName != typeSystem.Char.FullName)
                    {
						if (expression.Right.CodeNodeType == CodeNodeType.ExplicitCastExpression && expression.Right.ExpressionType.FullName == typeSystem.UInt16.FullName)
						{
							((ExplicitCastExpression)expression.Right).TargetType = typeSystem.Char;
						}
						else
						{
							expression.Right = new ExplicitCastExpression(expression.Right, typeSystem.Char, null);
						}
                    }
                }

                /// Check for bools.
                if (leftType.FullName == typeSystem.Boolean.FullName)
                {
                    if (expression.Right.CodeNodeType == CodeNodeType.LiteralExpression)
                    {
						if (expression.Operator == BinaryOperator.ValueEquality || expression.Operator == BinaryOperator.ValueInequality || IsBooleanAssignmentOperator(expression.Operator))
						{
							LiteralExpression right = expression.Right as LiteralExpression;
							bool newVal = true;
							if (right.Value == null || right.Value.Equals(0) || right.Value.Equals(false) || right.Value.Equals(null))
							{
								newVal = false;
							}
							expression.Right = GetLiteralExpression(newVal, right.MappedInstructions);
						}

                        if (expression.Operator == BinaryOperator.ValueEquality || expression.Operator == BinaryOperator.ValueInequality)
                        {
                            return SimplifyBooleanComparison(expression);
                        }
                    }
                }
            }

            if (expression.Operator == BinaryOperator.ValueEquality || expression.Operator == BinaryOperator.ValueInequality)
            {
                TypeReference rightType = GetElementType(expression.Right.ExpressionType);
                if (rightType != null && leftType != null && rightType.FullName != leftType.FullName)
                {
                    return FixEqualityComparisonExpression(expression);
                }
            }

            ///Adds downcasts in pointer types.
            if (expression.IsAssignmentExpression)
            {
                if (NeedsPointerCast(expression))
                {
					if (expression.Right.CodeNodeType == CodeNodeType.StackAllocExpression)
					{
						expression.Right.ExpressionType = expression.Left.ExpressionType;
					}
					else
					{
						expression.Right = new ExplicitCastExpression(expression.Right, expression.Left.ExpressionType, null);
					}
                }
                else if (expression.Left.HasType)
                {
                    if (expression.Left.ExpressionType.IsByReference || expression.Left.ExpressionType.IsPointer || expression.Left.ExpressionType.IsArray ||
                        !expression.Left.ExpressionType.IsPrimitive)
                    {
                        TypeDefinition leftResolved = expression.Left.ExpressionType.Resolve();
                        if (leftResolved != null && !leftResolved.IsEnum)
                        {
                            if (expression.Right is LiteralExpression)
                            {
                                LiteralExpression rightLit = expression.Right as LiteralExpression;
                                if (rightLit.Value != null && rightLit.Value.Equals(0))
                                {
                                    expression.Right = new LiteralExpression(null, typeSystem, expression.Right.UnderlyingSameMethodInstructions);
                                }
                            }
                        }
                    }
                }
            }

            if (expression.Operator == BinaryOperator.GreaterThan &&
                expression.MappedInstructions.Count() == 1 &&
                expression.MappedInstructions.First().OpCode.Code == Code.Cgt_Un)
            {
                LiteralExpression literal = null;
                if (expression.Right.CodeNodeType == CodeNodeType.LiteralExpression)
                {
                    literal = expression.Right as LiteralExpression;
                }
                else if (expression.Right.CodeNodeType == CodeNodeType.ExplicitCastExpression)
                {
                    ExplicitCastExpression cast = expression.Right as ExplicitCastExpression;
                    if (cast.Expression.CodeNodeType == CodeNodeType.LiteralExpression)
                    {
                        literal = cast.Expression as LiteralExpression;
                    }
                }

                if (literal != null)
                {
                    if (literal.Value == null || literal.Value.Equals(0))
                    {
                        expression.Operator = BinaryOperator.ValueInequality;
                    }
                }
            }

            /// Adds cast to object if needed.
            if (expression.IsObjectComparison)
            {
                Expression left = expression.Left;
                Expression right = expression.Right;
                TypeReference leftUnresolvedReference;
                TypeReference rightUnresolvedReference;
                if (CheckForOverloadedEqualityOperators(expression.Left, out leftUnresolvedReference) &&
                    CheckForOverloadedEqualityOperators(expression.Right, out rightUnresolvedReference))
                {
                    expression.Left = new ExplicitCastExpression(left, left.ExpressionType.Module.TypeSystem.Object, null, leftUnresolvedReference);
                    expression.Right = new ExplicitCastExpression(right, right.ExpressionType.Module.TypeSystem.Object, null, rightUnresolvedReference);
                }
            }

            return expression;
        }

        /// <summary>
        /// Check if current type or some type in the inheritance chain above it overloads the equality and inequality operators.
        /// </summary>
        /// <param name="expression">The expression that need it's expression type to be checked.</param>
        /// <param name="unresolvedReference">Out parameter that is filled with the MemberReference if there are unresolved references.</param>
        /// <returns>Returns true if there is overloaded operator in the inheritance chain. Returns false if oposite.</returns>
        private bool CheckForOverloadedEqualityOperators(Expression expression, out TypeReference unresolvedReference)
        {
            unresolvedReference = null;
            TypeReference lastResolvedType;
            bool? haveOverloadedOperator = Common.Extensions.ResolveToOverloadedEqualityOperator(expression.ExpressionType, out lastResolvedType);
            bool isResolved = haveOverloadedOperator.HasValue;
            if (isResolved)
            {
                return haveOverloadedOperator.Value;
            }
            else
            {
                unresolvedReference = lastResolvedType;
                return true;
            }
        }

        private bool NeedsPointerCast(BinaryExpression expression)
        {
            if (!expression.Left.ExpressionType.IsPointer && !expression.Left.ExpressionType.IsByReference)
            {
                return false;
            }
            return expression.Left.ExpressionType.GetElementType().FullName != expression.Right.ExpressionType.GetElementType().FullName;
        }

        /// <summary>
        /// Fixes expressions, generated for brtrue and brfalse. As 0,false and null are all represented as 0 in IL, this step adds the correct constant
        /// to the expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="isBrTrue">The type of jump.</param>
        /// <returns>Returns the fixed expression.</returns>
        private ICodeNode FixBranchingExpression(Expression expression, Instruction branch)
        {
            /// example:
            /// before the step:
            /// ...
            /// a //a is int32, opperation is brfalse
            /// After the step:
            /// ...
            /// a == 0

            bool isBrTrue = branch.OpCode.Code == Code.Brtrue || branch.OpCode.Code == Code.Brtrue_S;

            TypeReference expressionType = expression.ExpressionType;
            BinaryExpression newCondition;
            BinaryOperator operation = BinaryOperator.ValueEquality;
            Instruction[] instructions = new Instruction[] { branch };
            if (isBrTrue)
            {
                operation = BinaryOperator.ValueInequality;
            }
            if (expressionType.Name == "Boolean" || expressionType.Name.Contains("Boolean "))
            {
                Expression operand;
                if (!isBrTrue)
                {
                    operand = Negator.Negate(expression, typeSystem);
                }
                else
                {
                    operand = expression;
                }

                Expression result;
                if (expression is SafeCastExpression)
                {
                    result = new BinaryExpression(operation, expression, GetLiteralExpression(false, null), typeSystem, instructions);
                }
                else
                {
                    result = new UnaryExpression(UnaryOperator.None, operand, instructions);
                }

                return result;
            }
            if (expressionType.Name == "Char")
            {
                newCondition = new BinaryExpression(operation, expression, GetLiteralExpression('\u0000', null), typeSystem, instructions);
                newCondition.ExpressionType = typeSystem.Boolean;
            }
            if (!expressionType.IsPrimitive)
            {
                TypeDefinition expressionTypeDefinition = expressionType.Resolve();

                /// expressionTypeDefinition can resolve to null when dealing with generics
                if (expressionTypeDefinition != null && expressionTypeDefinition.IsEnum && !expressionType.IsArray)
                {
                    ///Find the field that corresponds to 0
                    FieldDefinition field = null;
                    foreach (FieldDefinition enumField in expressionTypeDefinition.Fields)
                    {
                        if (enumField.Constant != null && enumField.Constant.Value != null && enumField.Constant.Value.Equals(0))
                        {
                            field = enumField;
                            break;
                        }
                    }

                    if (field == null)
                    {
                        newCondition = new BinaryExpression(operation, expression, GetLiteralExpression(0, null), typeSystem, instructions);
                        newCondition.ExpressionType = typeSystem.Boolean;
                    }
                    else
                    {
                        newCondition = new BinaryExpression(operation, expression, new EnumExpression(field, null), typeSystem, instructions);
                        newCondition.ExpressionType = typeSystem.Boolean;
                    }
                }
                else
                {
                    /// If it is not primitive class, then the check should be against null
                    newCondition = new BinaryExpression(operation, expression, GetLiteralExpression(null, null), typeSystem, instructions);
                    newCondition.ExpressionType = typeSystem.Boolean;
                }
            }
            else
            {
                /// This is primitive type, the check should be against 0
                newCondition = new BinaryExpression(operation, expression, GetLiteralExpression(0, null), typeSystem, instructions);
                newCondition.ExpressionType = typeSystem.Boolean;
            }
            return newCondition;
        }

        /// <summary>
        /// Fixes expressions produced by ceq and beq.
        /// </summary>
        /// <param name="expression">The comparison expression.</param>
        /// <returns>Returns the updated binary expression.</returns>
        private ICodeNode FixEqualityComparisonExpression(BinaryExpression expression)
        {
            /// The generated expression is (ValueOnTopOfStack == 0)
            if (expression.Right is LiteralExpression)
            {
                TypeReference leftType = GetElementType(expression.Left.ExpressionType);
                if (leftType.FullName != typeSystem.Boolean.FullName)
                {
                    /// Left type is not boolean
                    TypeDefinition resolved = leftType.Resolve();
                    /// If the type is not primitive, the check should be against 'null', not against '0'
                    if (leftType != null && !leftType.IsPrimitive && (resolved != null && !resolved.IsEnum))
                    {
                        expression.Right = GetLiteralExpression(null, null);
                    }
                    /// The check against 0 is correct
                    return expression;
                }

                /// At this point the left side of 'expression' is boolean
                LiteralExpression rightLiteral = expression.Right as LiteralExpression;
                if (rightLiteral.Value.Equals(0) || rightLiteral.Value.Equals(null))
                {
                    return new UnaryExpression(UnaryOperator.LogicalNot, expression.Left, null);
                }
                else
                {
                    return expression.Left;
                }
            }
            return expression;
        }

        /// <summary>
        /// Simplifies boolean expressions.
        /// </summary>
        /// <param name="expression">The expression to be simplified.</param>
        /// <returns>Returns the simplified expression.</returns>
        private Expression SimplifyBooleanComparison(BinaryExpression expression)
        {
            bool isFalse = IsFalse(expression.Right);
            if (isFalse && expression.Operator == BinaryOperator.ValueEquality ||
                !isFalse && expression.Operator == BinaryOperator.ValueInequality)
            {
                List<Instruction> instructions = new List<Instruction>(expression.MappedInstructions);
                instructions.AddRange(expression.Right.UnderlyingSameMethodInstructions);
                return Negator.Negate(expression.Left, typeSystem).CloneAndAttachInstructions(instructions);
            }
            return expression;
        }

        /// <summary>
        /// Determines if given expression is logically equivalent to 'false'.
        /// </summary>
        /// <param name="expression">The expression to be evaluated.</param>
        /// <returns></returns>
        private bool IsFalse(Expression expression)
        {
            LiteralExpression litEx = expression as LiteralExpression;
            if (litEx == null)
            {
                /// If it is not literal expression, it cannot represent 'false'
                return false;
            }
            if (litEx.Value.Equals(false) || litEx.Value.Equals(0))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Chechs if <paramref name="operator"/> is arithmetic operator.
        /// </summary>
        /// <param name="operator">The operator to be checked.</param>
        /// <returns>Returns true, when <paramref name="operator"/> is arithmetic operator.</returns>
        private bool IsArithmeticOperator(BinaryOperator @operator)
        {
            return @operator == BinaryOperator.Add || @operator == BinaryOperator.Subtract ||
                   @operator == BinaryOperator.Multiply || @operator == BinaryOperator.Divide;
        }

        /// <summary>
        /// Resolves the actual type contained in <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The complex type.</param>
        /// <returns>Returns the underlaying type.</returns>
        private TypeReference GetElementType(TypeReference type)
        {
            if (type is IModifierType)
            {
                /// Locate the type without its modifiers.
                return (type as IModifierType).ElementType;
            }
            if (type is ByReferenceType)
            {
                /// Locate the type without its reference pointer.
                return (type as ByReferenceType).ElementType;
            }
            return type;
        }

        private LiteralExpression GetLiteralExpression(object value, IEnumerable<Instruction> instructions)
        {
            return new LiteralExpression(value, typeSystem, instructions);
        }
    }
}