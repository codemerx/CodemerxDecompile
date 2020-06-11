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
using Telerik.JustDecompiler.Ast;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Decompiler;
using Mono.Cecil.Extensions;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Mono.Cecil;
using Mono.Collections.Generic;
using Telerik.JustDecompiler.Decompiler.AssignmentAnalysis;

namespace Telerik.JustDecompiler.Steps
{
	public class DeclareTopLevelVariables : BaseCodeVisitor, IDecompilationStep
	{
        private readonly Stack<CodeNodeType> codeNodeTypes = new Stack<CodeNodeType>();
        private readonly Dictionary<VariableReference, bool> variableReferences = new Dictionary<VariableReference, bool>();
        private TypeSystem typeSystem;
        private Collection<VariableDefinition> methodVariables;
        private int inLambdaCount = 0;
        private DecompilationContext context;

		public BlockStatement Process(DecompilationContext context, BlockStatement block)
		{
            this.context = context;
            this.typeSystem = context.MethodContext.Method.Module.TypeSystem;
            this.methodVariables = new Collection<VariableDefinition>();
            foreach (VariableDefinition variable in context.MethodContext.Variables)
            {
                if (!context.MethodContext.VariablesToNotDeclare.Contains(variable))
                {
                    methodVariables.Add(variable);
                }
            }
			codeNodeTypes.Push(CodeNodeType.BlockStatement);
			Visit(block);
			codeNodeTypes.Pop();
			InsertTopLevelDeclarations(block);

			return block;
		}

		private void InsertTopLevelDeclarations(BlockStatement block)
		{
			int insertIndex = 0;
			if (context.MethodContext.Method.IsConstructor)
			{
				insertIndex = GetIndexOfCtorCall(block) + 1;
			}
			for (int i = 0; i < methodVariables.Count; i++, insertIndex++)
			{
                AssignmentType assignmentType;
                bool hasAssignmentData = this.context.MethodContext.VariableAssignmentData.TryGetValue(methodVariables[i], out assignmentType);
                if (hasAssignmentData && assignmentType == AssignmentType.NotUsed)
                {
                    --insertIndex;
                    continue;
                }

				bool isFirstUsageAssignment;
				if (variableReferences.TryGetValue(methodVariables[i], out isFirstUsageAssignment))
				{
					if (!isFirstUsageAssignment || hasAssignmentData && assignmentType == AssignmentType.NotAssigned)
					{
						InsertVariableDeclarationAndAssignment(block, insertIndex, i);
						continue;
					}
				}
				InsertVariableDeclaration(block, insertIndex, i);
			}
		}
  
		private int GetIndexOfCtorCall(BlockStatement block)
		{
			for (int i = 0; i < block.Statements.Count; i++)
			{
				if (block.Statements[i].CodeNodeType == CodeNodeType.ExpressionStatement)
				{
					Expression ex = (block.Statements[i] as ExpressionStatement).Expression;
					if (ex.CodeNodeType == CodeNodeType.BaseCtorExpression ||
						ex.CodeNodeType == CodeNodeType.ThisCtorExpression)
					{
						return i;
					}
				}
			}
			return -1;
		}

        public override void VisitLambdaExpression(LambdaExpression node)
        {
            inLambdaCount++;
            base.VisitLambdaExpression(node);
            inLambdaCount--;
            return;
        }

		private void InsertVariableDeclarationAndAssignment(BlockStatement block, int insertIndex, int variableIndex)
		{
            Expression defaultValueExpression = methodVariables[variableIndex].VariableType.GetDefaultValueExpression(typeSystem);
			
			if (defaultValueExpression == null)
			{
				InsertVariableDeclaration(block, insertIndex, variableIndex);
				return;
			}

			BinaryExpression assignExpression =
                new BinaryExpression(BinaryOperator.Assign, new VariableDeclarationExpression(methodVariables[variableIndex], null),
                    defaultValueExpression, typeSystem, null);
			block.AddStatementAt(insertIndex, new ExpressionStatement(assignExpression));
		}

		private void InsertVariableDeclaration(BlockStatement block, int insertIndex, int variableIndex)
		{
			block.AddStatementAt(insertIndex, new ExpressionStatement(new VariableDeclarationExpression(methodVariables[variableIndex], null)));
		}

        public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
        {
			if (!variableReferences.ContainsKey(node.Variable))
			{
				variableReferences.Add(node.Variable, (codeNodeTypes.Peek() == CodeNodeType.BinaryExpression) && inLambdaCount == 0);
			}
			base.VisitVariableReferenceExpression(node);
		}

        public override void VisitBinaryExpression(BinaryExpression node)
        {
            if (node.IsAssignmentExpression)
            {
                VisitAssignExpression(node);
                return;
            }
            base.VisitBinaryExpression(node);
        }

		private void VisitAssignExpression(BinaryExpression node)
		{
            bool pushBinary = node.Left.CodeNodeType == CodeNodeType.VariableReferenceExpression ||
                node.Left.CodeNodeType == CodeNodeType.VariableDeclarationExpression;
            if(pushBinary)
            {
			    codeNodeTypes.Push(CodeNodeType.BinaryExpression);
            }

			Visit(node.Left);

            if (pushBinary)
            {
                codeNodeTypes.Pop();
            }

			Visit(node.Right);
		}

        public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
        {
            MethodDefinition methodDef = node.MethodExpression.MethodDefinition;
            if (methodDef == null)
            {
                base.VisitMethodInvocationExpression(node);
                return;
            }

            Visit(node.MethodExpression);

            for (int i = 0; i < node.Arguments.Count; i++)
            {
                UnaryExpression unaryArgument = node.Arguments[i] as UnaryExpression;
                if (unaryArgument != null && unaryArgument.Operator == UnaryOperator.AddressReference &&
                    unaryArgument.Operand.CodeNodeType == CodeNodeType.VariableReferenceExpression &&
                    methodDef.Parameters[i].IsOutParameter())
                {
                    VariableReference varReference = (unaryArgument.Operand as VariableReferenceExpression).Variable;
                    if (!variableReferences.ContainsKey(varReference))
                    {
                        variableReferences.Add(varReference, true);
                    }
                }
                else
                {
                    Visit(node.Arguments[i]);
                }
            }
        }

		public override void VisitDelegateCreationExpression(DelegateCreationExpression node)
		{
			if (node.MethodExpression.CodeNodeType == CodeNodeType.LambdaExpression)
			{
				VisitLambdaExpression((LambdaExpression)node.MethodExpression);
			}
			else
			{
				base.VisitDelegateCreationExpression(node);
			}
		}
	}
}