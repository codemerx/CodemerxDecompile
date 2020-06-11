using System;
using System.Collections.Generic;
using Mono.Cecil;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Ast.Expressions;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Common;

namespace Telerik.JustDecompiler.Steps.DynamicVariables
{
    internal class CallSiteInvocationReplacer : BaseCodeTransformer
    {
        private const string InvalidIsEventString = "Invalid IsEvent construction";
        private readonly Dictionary<FieldDefinition, CallSiteInfo> fieldToCallSiteInfoMap;
        private readonly Dictionary<VariableReference, CallSiteInfo> variableToCallSiteInfoMap;
        private readonly Dictionary<IfStatement, MethodInvocationExpression> isEventIfStatements = new Dictionary<IfStatement, MethodInvocationExpression>();
        private readonly TypeReference objectTypeRef;
        private readonly TypeSystem typeSystem;
        private IfStatement closestIf = null;

        public static BlockStatement ReplaceInvocations(BlockStatement block,
            Dictionary<FieldDefinition, CallSiteInfo> fieldToCallSiteInfoMap,
            Dictionary<VariableReference, CallSiteInfo> variableToCallSiteInfoMap,
            HashSet<Statement> statementsToRemove,
            TypeSystem typeSystem)
        {
            CallSiteInvocationReplacer replacer = new CallSiteInvocationReplacer(fieldToCallSiteInfoMap, variableToCallSiteInfoMap, typeSystem);
            BlockStatement body = (BlockStatement)replacer.Visit(block);
            replacer.ManageIsEventOperations(statementsToRemove);
            return body;
        }

        private CallSiteInvocationReplacer(Dictionary<FieldDefinition, CallSiteInfo> fieldToCallSiteInfoMap,
            Dictionary<VariableReference, CallSiteInfo> variableToCallSiteInfoMap,
            TypeSystem typeSystem)
        {
            this.fieldToCallSiteInfoMap = fieldToCallSiteInfoMap;
            this.variableToCallSiteInfoMap = variableToCallSiteInfoMap;
            this.typeSystem = typeSystem;
            this.objectTypeRef = typeSystem.Object;
        }

        public override ICodeNode VisitIfStatement(IfStatement node)
        {
            closestIf = node;
            return base.VisitIfStatement(node);
        }

        public override ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
        {
            Expression visitedExpression = (Expression)base.VisitMethodInvocationExpression(node);
            MethodInvocationExpression methodInvocation = visitedExpression as MethodInvocationExpression;
            if (methodInvocation != null && methodInvocation.MethodExpression.Target != null && methodInvocation.MethodExpression.Method.Name == "Invoke")
            {
                if (methodInvocation.MethodExpression.Target.CodeNodeType == CodeNodeType.FieldReferenceExpression &&
                    (methodInvocation.MethodExpression.Target as FieldReferenceExpression).Field.Name == "Target" &&
                    (methodInvocation.MethodExpression.Target as FieldReferenceExpression).Target != null &&
                    (methodInvocation.MethodExpression.Target as FieldReferenceExpression).Target.CodeNodeType == CodeNodeType.FieldReferenceExpression)
                {
                    FieldDefinition fieldDef = ((methodInvocation.MethodExpression.Target as FieldReferenceExpression).Target as FieldReferenceExpression).Field.Resolve();
                    CallSiteInfo callSiteInfo;
                    if (fieldDef != null && fieldToCallSiteInfoMap.TryGetValue(fieldDef, out callSiteInfo))
                    {
                        if (callSiteInfo.BinderType != CallSiteBinderType.IsEvent)
                        {
                            return GenerateExpression(callSiteInfo, methodInvocation.Arguments, methodInvocation.InvocationInstructions);
                        }
                        else
                        {
                            isEventIfStatements.Add(closestIf, methodInvocation);
                        }
                    }
                }
                else if (methodInvocation.MethodExpression.Target.CodeNodeType == CodeNodeType.VariableReferenceExpression)
                {
                    VariableReference varRef = (methodInvocation.MethodExpression.Target as VariableReferenceExpression).Variable;
                    CallSiteInfo callSiteInfo;
                    if (variableToCallSiteInfoMap.TryGetValue(varRef, out callSiteInfo))
                    {
                        if (callSiteInfo.BinderType != CallSiteBinderType.IsEvent)
                        {
                            return GenerateExpression(callSiteInfo, methodInvocation.Arguments, methodInvocation.InvocationInstructions);
                        }
                        else
                        {
                            isEventIfStatements.Add(closestIf, methodInvocation);
                        }
                    }
                }
            }
            return visitedExpression;
        }

        private Expression GenerateExpression(CallSiteInfo callSiteInfo, IEnumerable<Expression> originalArguments, IEnumerable<Instruction> instructions)
        {
            IList<Expression> arguments = GetAllButFirst(originalArguments);

            MarkDynamicArguments(callSiteInfo, arguments);

            switch (callSiteInfo.BinderType)
            {
                case CallSiteBinderType.BinaryOperation:
                    return GenerateBinaryExpression(callSiteInfo, arguments, instructions);
                case CallSiteBinderType.Convert:
                    return GenerateConvertExpression(callSiteInfo, arguments, instructions);
                case CallSiteBinderType.GetIndex:
                    return GenerateGetIndexExpression(arguments, instructions);
                case CallSiteBinderType.GetMember:
                    return GenerateGetMemberExpression(callSiteInfo, arguments, instructions);
                case CallSiteBinderType.Invoke:
                    return GenerateInvokeExpression(callSiteInfo, arguments, instructions);
                case CallSiteBinderType.InvokeConstructor:
                    return GenerateInvokeConstructorExpression(arguments, instructions);
                case CallSiteBinderType.InvokeMember:
                    return GenerateInvokeMemeberExpression(callSiteInfo, arguments, instructions);
                case CallSiteBinderType.SetIndex:
                    return GenerateSetIndexExpression(arguments, instructions);
                case CallSiteBinderType.SetMember:
                    return GenerateSetMemberExpression(callSiteInfo, arguments, instructions);
                case CallSiteBinderType.UnaryOperation:
                    return GenerateUnaryExpression(callSiteInfo, arguments, instructions);
            }

            throw new Exception("Invalid binder type.");
        }

        private void MarkDynamicArguments(CallSiteInfo callSiteInfo, IList<Expression> arguments)
        {
            foreach (int index in callSiteInfo.DynamicArgumentIndices)
            {
                if (arguments[index].CodeNodeType != CodeNodeType.BinaryExpression &&
                    (arguments[index].CodeNodeType != CodeNodeType.UnaryExpression || (arguments[index] as UnaryExpression).Operator != UnaryOperator.None) &&
                    arguments[index].CodeNodeType != CodeNodeType.DynamicIndexerExpression &&
                    arguments[index].CodeNodeType != CodeNodeType.DynamicMemberReferenceExpression &&
                    !DynamicElementAnalyzer.Analyze(arguments[index]))
                {
                    ExplicitCastExpression theCastExpression = new ExplicitCastExpression(arguments[index], objectTypeRef, null);
                    theCastExpression.DynamicPositioningFlags = new bool[] { true };
                    arguments[index] = theCastExpression;
                }
            }
        }

        private IList<Expression> GetAllButFirst(IEnumerable<Expression> expressionEnumeration)
        {
            List<Expression> expressionList = new List<Expression>();
            using (IEnumerator<Expression> enumerator = expressionEnumeration.GetEnumerator())
            {
                enumerator.MoveNext();
                while (enumerator.MoveNext())
                {
                    expressionList.Add(RemoveUnneededCast(enumerator.Current));
                }
            }

            return expressionList;
        }

        private Expression RemoveUnneededCast(Expression expression)
        {
            while (expression.CodeNodeType == CodeNodeType.ExplicitCastExpression && (expression as ExplicitCastExpression).TargetType.Name[0] == '!')
            {
                expression = (expression as ExplicitCastExpression).Expression;
            }

            return expression;
        }

        #region Expression generation
        private Expression GenerateBinaryExpression(CallSiteInfo callSiteInfo, IList<Expression> arguments, IEnumerable<Instruction> instructions)
        {
            if (arguments.Count != 2)
            {
                throw new Exception("Invalid number of arguments for binary expression.");
            }

            return new BinaryExpression(
                DynamicHelper.GetBinaryOperator(callSiteInfo.Operator),
                arguments[0],
                arguments[1],
                objectTypeRef,
                typeSystem, instructions
                );
        }

        private Expression GenerateConvertExpression(CallSiteInfo callSiteInfo, IList<Expression> arguments, IEnumerable<Instruction> instructions)
        {
            if (arguments.Count != 1)
            {
                throw new Exception("Invalid number of arguments for convert expression.");
            }

            return new ExplicitCastExpression(arguments[0], callSiteInfo.ConvertType, instructions);
        }

        private Expression GenerateGetIndexExpression(IList<Expression> arguments, IEnumerable<Instruction> instructions)
        {
            if (arguments.Count < 2)
            {
                throw new Exception("Invalid number of arguments for get index expression.");
            }

            DynamicIndexerExpression indexerExpression = new DynamicIndexerExpression(arguments[0], objectTypeRef, instructions);
            for (int i = 1; i < arguments.Count; i++)
            {
                indexerExpression.Indices.Add(arguments[i]);
            }
            return indexerExpression;
        }

        private Expression GenerateGetMemberExpression(CallSiteInfo callSiteInfo, IList<Expression> arguments, IEnumerable<Instruction> instructions)
        {
            if (arguments.Count != 1)
            {
                throw new Exception("Invalid number of arguments for get member expression.");
            }

            return new DynamicMemberReferenceExpression(arguments[0], callSiteInfo.MemberName, objectTypeRef, instructions);
        }

        private Expression GenerateInvokeExpression(CallSiteInfo callSiteInfo, IList<Expression> arguments, IEnumerable<Instruction> instructions)
        {
            return GenerateInvokeMemeberExpression(callSiteInfo, arguments, instructions);
        }

        private Expression GenerateInvokeConstructorExpression(IList<Expression> arguments, IEnumerable<Instruction> instructions)
        {
            if (arguments.Count < 1)
            {
                throw new Exception("Invalid number of arguments for invoke constructor expression.");
            }

            TypeReference typeRef;
            if (arguments[0].CodeNodeType != CodeNodeType.MethodInvocationExpression ||
                !(arguments[0] as MethodInvocationExpression).IsTypeOfExpression(out typeRef))
            {
                throw new Exception("Invalid type argument for invoke constructor expression.");
            }

            return new DynamicConstructorInvocationExpression(typeRef, GetAllButFirst(arguments), instructions);
        }

        private Expression GenerateInvokeMemeberExpression(CallSiteInfo callSiteInfo, IList<Expression> arguments, IEnumerable<Instruction> instructions)
		{
            if (arguments.Count < 1)
            {
                throw new Exception("Invalid number of arguments for invoke expression.");
            }

            Expression target;
            TypeReference typeRef;
            if (arguments[0].CodeNodeType == CodeNodeType.MethodInvocationExpression &&
                (arguments[0] as MethodInvocationExpression).IsTypeOfExpression(out typeRef))
            {
                target = new TypeReferenceExpression(typeRef, arguments[0].UnderlyingSameMethodInstructions);
            }
            else
            {
                target = arguments[0];
            }

			DynamicMemberReferenceExpression dynamicMemberReferenceExpression = new DynamicMemberReferenceExpression(target, callSiteInfo.MemberName, objectTypeRef, instructions,
                GetAllButFirst(arguments), callSiteInfo.GenericTypeArguments);
			return dynamicMemberReferenceExpression;
		}

        private Expression GenerateSetIndexExpression(IList<Expression> arguments, IEnumerable<Instruction> instructions)
        {
            if (arguments.Count < 3)
            {
                throw new Exception("Invalid number of arguments for set index expression.");
            }

            DynamicIndexerExpression indexerExpression = new DynamicIndexerExpression(arguments[0], objectTypeRef, instructions);
            for (int i = 1; i < arguments.Count - 1; i++)
            {
                indexerExpression.Indices.Add(arguments[i]);
            }

            return new BinaryExpression(BinaryOperator.Assign, indexerExpression, arguments[arguments.Count - 1], typeSystem ,null);
        }

        private Expression GenerateSetMemberExpression(CallSiteInfo callSiteInfo, IList<Expression> arguments, IEnumerable<Instruction> instructions)
        {
            if (arguments.Count != 2)
            {
                throw new Exception("Invalid number of arguments for set index expression.");
            }

            return new BinaryExpression(BinaryOperator.Assign,
                new DynamicMemberReferenceExpression(arguments[0], callSiteInfo.MemberName, objectTypeRef, instructions),
                arguments[1], typeSystem, null);
        }

        private Expression GenerateUnaryExpression(CallSiteInfo callSiteInfo, IList<Expression> arguments, IEnumerable<Instruction> instructions)
        {
            if (arguments.Count != 1)
            {
                throw new Exception("Invalid number of arguments for unary expression.");
            }

            if (callSiteInfo.Operator != ExpressionType.IsTrue)
            {
                return new UnaryExpression(DynamicHelper.GetUnaryOperator(callSiteInfo.Operator), arguments[0], instructions);
            }
            else
            {
                return arguments[0];
            }
        }
        #endregion

        public override ICodeNode VisitExpressionStatement(ExpressionStatement node)
        {
            if (node.IsAssignmentStatement())
            {
                BinaryExpression assignExpression = node.Expression as BinaryExpression;
                if (assignExpression.Left.CodeNodeType == CodeNodeType.VariableReferenceExpression &&
                    assignExpression.Right.CodeNodeType == CodeNodeType.FieldReferenceExpression)
                {
                    FieldDefinition fieldDef = (assignExpression.Right as FieldReferenceExpression).Field.Resolve();
                    if (fieldDef != null && fieldToCallSiteInfoMap.ContainsKey(fieldDef))
                    {
                        return null;
                    }
                }
            }
            return base.VisitExpressionStatement(node);
        }

		#region IsEvent operation
        private void ManageIsEventOperations(HashSet<Statement> statementsToRemove)
        {
			foreach (KeyValuePair<IfStatement, MethodInvocationExpression> pair in isEventIfStatements)
			{
                IfStatement theIf = pair.Key;
                MethodInvocationExpression theInvoke = pair.Value;

				if (theIf.Condition.CodeNodeType == CodeNodeType.UnaryExpression &&
					(theIf.Condition as UnaryExpression).Operator == UnaryOperator.None 
					&& (theIf.Condition as UnaryExpression).Operand == theInvoke)
				{
					/// In case the if is the last statement of the method, return instructions are included in bot the then and else
					/// blocks. This causes the else block being removed, and all statements contained inside it are being put after the if.
					/// To keep the pattern behavior the same as the one before the if changes, we need to remove all statements following the if.
					/// There is no need to check for inversed condition, as the "smaller block" (the one with less statements) will allways be
					/// the then block and the condition will not be negated (the pattern is such).
					Statement lastThenStatement = theIf.Then.Statements[theIf.Then.Statements.Count - 1];
					if (lastThenStatement.CodeNodeType == CodeNodeType.ExpressionStatement &&
						((ExpressionStatement)lastThenStatement).Expression.CodeNodeType == CodeNodeType.ReturnExpression)
					{
						BlockStatement ifParent = (BlockStatement)theIf.Parent;
						int ifIndex = ifParent.Statements.IndexOf(theIf);
						for (int i = ifIndex + 1; i < ifParent.Statements.Count; i++)
						{
							statementsToRemove.Add(ifParent.Statements[i]);
						}
					}
					ReplaceIfWith(theIf, theIf.Then);
				}
				else if (theIf.Condition.CodeNodeType == CodeNodeType.UnaryExpression &&
					(theIf.Condition as UnaryExpression).Operator == UnaryOperator.Negate
					&& (theIf.Condition as UnaryExpression).Operand == theInvoke) 
				{
					ReplaceIfWith(theIf, theIf.Else);
				}
				else
				{
					throw new Exception("Invalid invocation of IsEvent operation.");
				}

                foreach (Statement statement in theIf.Then.Statements)
                {
                    statementsToRemove.Remove(statement);
                }
				if (theIf.Else != null)
				{
					foreach (Statement statement in theIf.Else.Statements)
					{
						statementsToRemove.Remove(statement);
					}
				}
			}
        }

		private void ReplaceIfWith(IfStatement theIf, BlockStatement statementBlock)
		{
			if (!CanReplaceIf(statementBlock)) 
			{
				throw new Exception(InvalidIsEventString);
			}

            DynamicMemberReferenceExpression dynamicMethodInvocation = (statementBlock.Statements[1] as ExpressionStatement).Expression as
                DynamicMemberReferenceExpression;

			if (dynamicMethodInvocation == null) // the expression was an assignment
			{ 
				dynamicMethodInvocation = ((statementBlock.Statements[1] as ExpressionStatement).Expression as BinaryExpression).Right as DynamicMemberReferenceExpression;
			}

            if (dynamicMethodInvocation.MemberName == null || !dynamicMethodInvocation.IsMethodInvocation ||
                dynamicMethodInvocation.IsGenericMethod || dynamicMethodInvocation.InvocationArguments.Count != 1)
            {
                throw new Exception(InvalidIsEventString);
            }

            int charIndex = dynamicMethodInvocation.MemberName.IndexOf('_');
            if (charIndex != 3 && charIndex != 6) //"add_" - 3, "remove_" - 6
            {
                throw new Exception(InvalidIsEventString);
            }

            DynamicMemberReferenceExpression dynamicMemberRef = new DynamicMemberReferenceExpression(dynamicMethodInvocation.Target,
                dynamicMethodInvocation.MemberName.Substring(charIndex + 1), dynamicMethodInvocation.ExpressionType, dynamicMethodInvocation.MappedInstructions);

            BinaryExpression theBinaryExpression = new BinaryExpression(charIndex == 3 ? BinaryOperator.AddAssign : BinaryOperator.SubtractAssign, dynamicMemberRef,
                dynamicMethodInvocation.InvocationArguments[0], dynamicMemberRef.ExpressionType, typeSystem, null);

            BlockStatement parent = (BlockStatement)theIf.Parent;
            int ifIndex = parent.Statements.IndexOf(theIf);

            ExpressionStatement theAssignStatement = new ExpressionStatement(theBinaryExpression);
            theAssignStatement.Parent = parent;
            parent.Statements[ifIndex] = theAssignStatement;

            if (statementBlock.Statements.Count == 3)
            {
                parent.AddStatementAt(ifIndex + 1, statementBlock.Statements[2].Clone());
            }
		}
  
		private bool CanReplaceIf(BlockStatement statementBlock)
		{
			if (statementBlock.Statements.Count != 2 && statementBlock.Statements.Count != 3 ||
				statementBlock.Statements[1].CodeNodeType != CodeNodeType.ExpressionStatement ||
				(statementBlock.Statements[1] as ExpressionStatement).Expression.CodeNodeType != CodeNodeType.DynamicMemberReferenceExpression)
			{
				if ((statementBlock.Statements[1] as ExpressionStatement).Expression.CodeNodeType == CodeNodeType.BinaryExpression)
				{
					BinaryExpression expr = (statementBlock.Statements[1] as ExpressionStatement).Expression as BinaryExpression;
					if (!expr.IsAssignmentExpression ||
						expr.Right.CodeNodeType != CodeNodeType.DynamicMemberReferenceExpression)
					{
						return false;
					}
					return true;
				}
				return false;
			}

			if (statementBlock.Statements.Count == 3 &&
				(statementBlock.Statements[2].CodeNodeType != CodeNodeType.ExpressionStatement ||
				 (statementBlock.Statements[2] as ExpressionStatement).Expression.CodeNodeType != CodeNodeType.ReturnExpression))
			{
				return false;
			}
			return true;
		}
		
		#endregion

    }
}
