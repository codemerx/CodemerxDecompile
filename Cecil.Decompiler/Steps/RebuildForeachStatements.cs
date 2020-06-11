using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Ast.Expressions;
using Mono.Cecil.Cil;
using Mono.Cecil;

namespace Telerik.JustDecompiler.Steps
{
    class RebuildForeachStatements : BaseCodeVisitor, IDecompilationStep
    {
        private bool insideTry;
        private bool foundEnumeratorAssignment;
        private bool foundWhile;
        private Expression foreachCollection;
        private VariableDefinition foreachVariable;
        private BlockStatement foreachBody;
        private VariableReference theEnumerator;
        private TryStatement theTry;
        private ForEachStatement @foreach;
        private TypeReference foreachVariableType;
        private ExpressionStatement enumeratorAssignmentStatement;
        private bool shouldAdd;
		private bool isEnumeratorUsedInsideForEach;
        private MethodSpecificContext methodContext;
        private readonly HashSet<VariableDefinition> foreachVariables = new HashSet<VariableDefinition>();
        private readonly List<Instruction> foreachVariableInstructions = new List<Instruction>();
        private readonly List<Instruction> foreachCollectionInstructions = new List<Instruction>();
        private IEnumerable<Instruction> foreachConditionInstructions;

        public RebuildForeachStatements()
        {
            this.insideTry = false;
            this.foundEnumeratorAssignment = false;
            this.foundWhile = false;
        }

        public BlockStatement Process(DecompilationContext context, BlockStatement body)
        {
            this.methodContext = context.MethodContext;
            ClearState();
            this.foreachBody = new BlockStatement();
            Visit(body);
            return body;
        }

        public override void VisitBlockStatement(BlockStatement node)
        {
            for (int i = 0; i < node.Statements.Count; i++)
            {
                int oldCount = node.Statements.Count;
                Visit(node.Statements[i]);
                int newCount = node.Statements.Count;
                if (oldCount > newCount)
                {
                    i -= oldCount - newCount;
                }
            }
        }

        public override void VisitTryStatement(TryStatement node)
        {
            if (!foundEnumeratorAssignment)
            {
                insideTry = false;
                base.VisitTryStatement(node);
                return;
            }
            if (CanContainForeach(node))
            {
                insideTry = true;
                theTry = node;
                base.VisitTryStatement(node);
            }
            insideTry = false;
        }

        public override void VisitExpressionStatement(ExpressionStatement node)
        {
            if (!foundWhile)
            {
                if (IsEnumeratorAssignment(node.Expression))
                {
                    foundEnumeratorAssignment = true;
                    enumeratorAssignmentStatement = node;
                    return;
                }
            }
            else
            {
				if (node.Expression is BinaryExpression)
                {
					if (!IsForeachVariableAssignment(node.Expression as BinaryExpression))
					{
						if (shouldAdd)
						{
							foreachBody.AddStatement(node);
						}
					}
                }
				else if (node.Expression is MethodInvocationExpression || node.Expression is PropertyReferenceExpression)
                {
					if (IsGetCurrent(node.Expression))
                    {
                        //covers the case where the foreach variable is not used
                        this.foreachVariableType = node.Expression.ExpressionType;
                    }
                    else
					{
						if (shouldAdd)
						{
							foreachBody.AddStatement(node);
						}
					}
                }
				else if (node.Expression is ExplicitCastExpression)
                {
                    ExplicitCastExpression theCast = node.Expression as ExplicitCastExpression;
					if (IsGetCurrent(theCast.Expression))
                    {
                        this.foreachVariableType = theCast.ExpressionType;
                    }
                    else
					{
						if (shouldAdd)
						{
							foreachBody.AddStatement(node);
						}
					}
                }
				else if (node.Expression is BoxExpression)
				{
					BoxExpression theBox = node.Expression as BoxExpression;
					if (IsGetCurrent(theBox.BoxedExpression))
					{
						this.foreachVariableType = theBox.BoxedAs;
					}
					else
					{
						if (shouldAdd)
						{
							foreachBody.AddStatement(node);
						}
					}
					
				}
				else
				{
					if (shouldAdd)
					{
						foreachBody.AddStatement(node);
					}
				}
            }
        }

        public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
        {
            if (!foundWhile && (IsGetCurrent(node) || IsMoveNextCall(node)))
            {
                ClearState();
            }
            else
            {
                if (IsGetCurrent(node))
                {
                    foreachVariableType = node.ExpressionType;
                }
            }
        }

        public override void VisitPropertyReferenceExpression(PropertyReferenceExpression node)
        {
            if (!foundWhile && IsGetCurrent(node))
            {
                //TryStatement ts = theTry;
                ClearState();
                //theTry = ts;
                //insideTry = ts != null;
            }
            else
            {
                if (IsGetCurrent(node))
                {
                    foreachVariableType = node.ExpressionType;
                }
            }
        }

        public override void VisitWhileStatement(WhileStatement node)
        {
            if (foundEnumeratorAssignment && insideTry)
            {
                if (IsForeach(node))
                {
                    BlockStatement tryParent = theTry.Parent as BlockStatement;
                    if (tryParent == null || tryParent != enumeratorAssignmentStatement.Parent ||
                        tryParent.Statements.IndexOf(enumeratorAssignmentStatement) + 1 != tryParent.Statements.IndexOf(theTry))
                    {
                        ClearState();
                        base.VisitWhileStatement(node);
                        return;
                    }
                    foreachConditionInstructions = node.Condition.UnderlyingSameMethodInstructions;
                    foundWhile = true;
                    shouldAdd = true;
                    foreach (Statement st in node.Body.Statements)
                    {
                        if (!(st is ExpressionStatement))
                        {
                            foreachBody.AddStatement(st);
                            //TODO: Must traverse the statement tree, in order to find GetCurrent method and obtain the type of the foreach
                        }
                        else
                        {
                            VisitExpressionStatement(st as ExpressionStatement);
                        }
                        if (foreachVariableType == null)
                        {
                            ForeachElementTypeFinder fet = new ForeachElementTypeFinder(theEnumerator);
                            fet.Visit(st);
                            foreachVariableType = fet.ResultingType;
                        }
                    }
                    if (foreachVariableType != null)
					{
                        AttachForeach();

						if (isEnumeratorUsedInsideForEach)
						{
							ClearState();
							base.VisitWhileStatement(node);
						}
					}
                    else
                    {
                        //this can happen if enumerator.get_Current is not called anywhere in the loop.
                        //in this case there is no foreach.
                        //think of a better way to 
                        ClearState();
                        base.VisitWhileStatement(node);
                    }
                }
                else
                {
                    ClearState();
                    base.VisitWhileStatement(node);
                }
            }
            else
            {
                base.VisitWhileStatement(node);
            }
        }

        private void AttachForeach()
        {
            GenerateForeachStatement();

			if (!isEnumeratorUsedInsideForEach)
			{
				BlockStatement parentBlock = theTry.Parent as BlockStatement;
				parentBlock.Statements.Remove(enumeratorAssignmentStatement);
				int tryIndex = parentBlock.Statements.IndexOf(theTry);

				parentBlock.Statements.RemoveAt(tryIndex);
				parentBlock.AddStatementAt(tryIndex, @foreach);

                YieldStateMachineCodeRemover ysmcr = new YieldStateMachineCodeRemover(@foreach, theEnumerator);
                ysmcr.ProcessForEachStatement();

				CopyLabel();

                CheckVariable();

				ClearState();

				VisitForEachStatement(parentBlock.Statements[tryIndex] as ForEachStatement);
			}
        }

        private void CheckVariable()
        {
            if (foreachVariables.Contains(foreachVariable))
            {
                VariableDefinition oldForeachVariable = foreachVariable;
                foreachVariable = new VariableDefinition(foreachVariable.VariableType, this.methodContext.Method);
                foreachVariableInstructions.Clear();
                this.methodContext.Variables.Add(foreachVariable);
                this.methodContext.VariablesToRename.Add(foreachVariable);

                ForeachVariableChanger variableChanger = new ForeachVariableChanger(oldForeachVariable, foreachVariable);
                variableChanger.Visit(@foreach);
            }

            foreachVariables.Add(foreachVariable);
        }

        private void CopyLabel()
        {
            if(theTry.Label != string.Empty)
            {
                @foreach.Label = theTry.Label;
            }
            else if(theTry.Try.Label != string.Empty)
            {
                @foreach.Label = theTry.Try.Label;
            }
            else if(theTry.Try.Statements[0].Label != string.Empty)
            {
                @foreach.Label = theTry.Try.Statements[0].Label;
            }
        }

        private void ClearState()
        {
            insideTry = false;
            foundEnumeratorAssignment = false;
            foundWhile = false;
            foreachCollection = null;
            foreachVariable = null;
            foreachVariableInstructions.Clear();
            foreachCollectionInstructions.Clear();
            foreachBody = new BlockStatement();
            theEnumerator = null;
            theTry = null;
            @foreach = null;
            enumeratorAssignmentStatement = null;
            foreachVariableType = null;
			isEnumeratorUsedInsideForEach = false;
            foreachConditionInstructions = null;
        }

        private void GenerateForeachStatement()
        {
            if (foreachVariable == null)
            {
                foreachVariable = new VariableDefinition(foreachVariableType, this.methodContext.Method);
                foreachVariableInstructions.Clear();
                this.methodContext.VariablesToRename.Add(foreachVariable);
            }
            VariableDeclarationExpression vd = new VariableDeclarationExpression(foreachVariable, foreachVariableInstructions);

			Expression foreachCollectionExpression = foreachCollection.CloneAndAttachInstructions(foreachCollectionInstructions);
			if (foreachCollectionExpression is BaseReferenceExpression)
			{
				foreachCollectionExpression = new ThisReferenceExpression(this.methodContext.Method.DeclaringType, foreachCollectionExpression.UnderlyingSameMethodInstructions);
			}

			@foreach = new ForEachStatement(vd, foreachCollectionExpression, foreachBody, foreachConditionInstructions, theTry.Finally.UnderlyingSameMethodInstructions);
            GetCurrentFixer gcf = new GetCurrentFixer(theEnumerator, foreachVariable);
            gcf.Visit(@foreach);

			IsEnumeratorUsedVisitor enumeratorUsedVisitor = new IsEnumeratorUsedVisitor(theEnumerator);
			enumeratorUsedVisitor.Visit(@foreach);
			isEnumeratorUsedInsideForEach = enumeratorUsedVisitor.IsEnumeratorUsed;
        }

        private bool IsForeach(WhileStatement node)
        {
			if (node.Condition is UnaryExpression)
			{
				UnaryExpression unary = node.Condition as UnaryExpression;
				if (unary.Operator == UnaryOperator.None && unary.Operand is MethodInvocationExpression)
				{
					MethodInvocationExpression expr = unary.Operand as MethodInvocationExpression;
					return IsMoveNextCall(expr);
				}
			}
			return false;
        }

        private bool IsMoveNextCall(MethodInvocationExpression invocation)
        {
            if (invocation == null)
            {
                return false;
            }
            if (invocation.MethodExpression.Method.Name == "MoveNext")
            {
                VariableReferenceExpression target = invocation.MethodExpression.Target as VariableReferenceExpression;
                if (target != null)
                {
                    if (target.Variable == theEnumerator)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsForeachVariableAssignment(BinaryExpression assignment)
        {
            if (assignment == null)
            {
                return false;
            }
            Expression getCurrentInvocation;
            if (assignment.Right is ExplicitCastExpression)
            {
                getCurrentInvocation = (assignment.Right as ExplicitCastExpression).Expression;
            }
            else if (assignment.Right is MethodInvocationExpression || assignment.Right is PropertyReferenceExpression)
            {
                getCurrentInvocation = assignment.Right;
            }
            else
            {
                return false;
            }
            if (IsGetCurrent(getCurrentInvocation))
            {
                VariableReferenceExpression variable = assignment.Left as VariableReferenceExpression;
                if (variable == null)
                {
                    return false;
                }
                foreachVariable = variable.Variable.Resolve();
                foreachVariableInstructions.AddRange(assignment.UnderlyingSameMethodInstructions);
                foreachVariableType = assignment.Right.ExpressionType;
                return true;
            }
            return false;
        }

        private bool IsGetCurrent(Expression expression)
        {
            MethodInvocationExpression methodInvocation = expression as MethodInvocationExpression;
            if (methodInvocation == null)
            {
                PropertyReferenceExpression propertyExpression = expression as PropertyReferenceExpression;
                return propertyExpression!=null && propertyExpression.Property.Name == "Current";
            }
            if (methodInvocation.MethodExpression.Target as VariableReferenceExpression == null || methodInvocation.MethodExpression.Method.Name != "get_Current")
            {
                return false;
            }
            if ((methodInvocation.MethodExpression.Target as VariableReferenceExpression).Variable != theEnumerator)
            {
                return false;
            }
            return true;
        }

        private bool IsEnumeratorAssignment(Expression expression)
        {
            BinaryExpression assignment = expression as BinaryExpression;
            if (assignment == null || !assignment.IsAssignmentExpression)
            {
                return false;
            }
            Expression right = assignment.Right;
            MethodInvocationExpression supposedGetEnumerator;
            if (right is MethodInvocationExpression)
            {
                supposedGetEnumerator = right as MethodInvocationExpression;
                if (IsGetEnumerator(supposedGetEnumerator))
                {
                    if (assignment.Left as VariableReferenceExpression == null)
                    {
                        return false;
                    }

                    foreachCollectionInstructions.Clear();
                    foreachCollectionInstructions.AddRange(assignment.Left.UnderlyingSameMethodInstructions);
                    foreachCollectionInstructions.AddRange(assignment.MappedInstructions);
                    foreachCollectionInstructions.AddRange(supposedGetEnumerator.InvocationInstructions);
                    theEnumerator = (assignment.Left as VariableReferenceExpression).Variable;
                    return true;
                }
            }
            return false;
        }

        private bool IsGetEnumerator(MethodInvocationExpression supposedGetEnumerator)
        {
            if (supposedGetEnumerator.MethodExpression.Method.Name != "GetEnumerator")
            {
                return false;
            }
            if (supposedGetEnumerator.MethodExpression.Target == null)
            {
                return false;
            }
            this.foreachCollection = supposedGetEnumerator.MethodExpression.Target;
            return true;
        }

        private bool CanContainForeach(TryStatement tryStatement)
        {
            if ((tryStatement.CatchClauses.Count == 0) &&
                      (tryStatement.Try.Statements.Count == 1) &&
                      (tryStatement.Finally != null) &&
                      ((tryStatement.Finally.Body.Statements.Count == 1) ||
                      (tryStatement.Finally.Body.Statements.Count == 2)))
            {
                return IsValidFinally(tryStatement.Finally.Body);
            }
            return false;
        }

        private bool IsValidFinally(BlockStatement blockStatement)
        {
            IfStatement supposedIf;
            if (blockStatement.Statements.Count == 1)
            {
                supposedIf = blockStatement.Statements[0] as IfStatement;
                if (supposedIf == null)
                {
                    ExpressionStatement supposedDispose = blockStatement.Statements[0] as ExpressionStatement;
                    if (supposedDispose == null)
                    {
                        return false;
                    }
                    return IsEnumeratorDispose(supposedDispose.Expression as MethodInvocationExpression);
                }
            }
            else
            {
                supposedIf = blockStatement.Statements[1] as IfStatement;
            }
            if (supposedIf == null)
            {
                return false;
            }
            if (supposedIf.Then.Statements.Count == 1)
            {
                ExpressionStatement supposedDispose = supposedIf.Then.Statements[0] as ExpressionStatement;
                if (supposedDispose == null)
                {
                    return false;
                }
                return IsEnumeratorDispose(supposedDispose.Expression as MethodInvocationExpression);
            }
            return false;
        }

        private bool IsEnumeratorDispose(MethodInvocationExpression methodInvocationExpression)
        {
            if (methodInvocationExpression == null)
            {
                return false;
            }
            if (methodInvocationExpression.MethodExpression.Method.Name == "Dispose")
            {
                return true;
            }
            return false;
        }

		private class IsEnumeratorUsedVisitor : BaseCodeVisitor
		{
			private readonly VariableReference enumerator;

			public bool IsEnumeratorUsed { get; set; }

			public IsEnumeratorUsedVisitor(VariableReference enumerator)
			{
				this.enumerator = enumerator;
				this.IsEnumeratorUsed = false;
			}

			public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
			{
				if (node.MethodExpression.Target is VariableReferenceExpression)
				{
					if ((node.MethodExpression.Target as VariableReferenceExpression).Variable == enumerator)
					{
						if (node.MethodExpression.Method.Name != "get_Current")
						{
							IsEnumeratorUsed = true;
						}
					}
				}

				base.VisitMethodInvocationExpression(node);
			}

			public override void VisitPropertyReferenceExpression(PropertyReferenceExpression node)
			{
				if (node.Property.Name != "Current")
				{
					VariableReferenceExpression nodeTarget = node.Target as VariableReferenceExpression;
					if (nodeTarget != null && nodeTarget.Variable == enumerator)
					{
						IsEnumeratorUsed = true;
					}
				}

				base.VisitPropertyReferenceExpression(node);
			}

			public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
			{
				if (node.Variable == enumerator)
				{
					IsEnumeratorUsed = true;
				}

				base.VisitVariableReferenceExpression(node);
			}
		}

        private class GetCurrentFixer : BaseCodeTransformer
        {
            private readonly VariableReference enumerator;
            private readonly VariableReference foreachVariable;

            public GetCurrentFixer(VariableReference enumerator, VariableReference foreachVariable)
            {
                this.enumerator = enumerator;
                this.foreachVariable = foreachVariable;
            }

            public override ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
            {
                if (node.MethodExpression.Target is VariableReferenceExpression)
                {
                    if ((node.MethodExpression.Target as VariableReferenceExpression).Variable == enumerator)
                    {
                        if (node.MethodExpression.Method.Name == "get_Current")
                        {
							return new VariableReferenceExpression(foreachVariable, null);
                        }
                    }
                }
                return base.VisitMethodInvocationExpression(node);
            }

            public override ICodeNode VisitPropertyReferenceExpression(PropertyReferenceExpression node)
            {
                if (node.Property.Name == "Current")
                {
                    VariableReferenceExpression nodeTarget = node.Target as VariableReferenceExpression;
                    if (nodeTarget != null && nodeTarget.Variable == enumerator)
                    {
						return new VariableReferenceExpression(foreachVariable, null);
                    }
                }
                return base.VisitPropertyReferenceExpression(node);
            }
        }

        private class ForeachElementTypeFinder : BaseCodeVisitor
        {
            private readonly VariableReference theEnumerator;
            public TypeReference ResultingType { get;set; }

            public ForeachElementTypeFinder(VariableReference theEnumerator)
            {
                this.theEnumerator = theEnumerator;
            }

            public override void VisitTryStatement(TryStatement node)
            {
            }

            public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
            {
                if (IsGetCurrent(node))
                {
                    ResultingType = node.ExpressionType;
                }
                base.VisitMethodInvocationExpression(node);
            }

            public override void VisitPropertyReferenceExpression(PropertyReferenceExpression node)
            {
                if (IsGetCurrent(node))
                {
                    ResultingType = node.ExpressionType;
                }
                base.VisitPropertyReferenceExpression(node);
            }

            private bool IsGetCurrent(Expression expression)
            {
                MethodInvocationExpression methodInvocation = expression as MethodInvocationExpression;
                if (methodInvocation == null)
                {
                    PropertyReferenceExpression propertyExpression = expression as PropertyReferenceExpression;
                    return propertyExpression != null && propertyExpression.Property.Name == "Current";
                }
                if (methodInvocation.MethodExpression.Target as VariableReferenceExpression == null || methodInvocation.MethodExpression.Method.Name != "get_Current")
                {
                    return false;
                }
                if ((methodInvocation.MethodExpression.Target as VariableReferenceExpression).Variable != theEnumerator)
                {
                    return false;
                }
                return true;
            }
        }

        private class ForeachVariableChanger : BaseCodeVisitor
        {
            private readonly VariableDefinition oldVariable;
            private readonly VariableDefinition newVariable;

            public ForeachVariableChanger(VariableDefinition oldVariable, VariableDefinition newVariable)
            {
                this.oldVariable = oldVariable;
                this.newVariable = newVariable;
            }

            public override void VisitVariableDeclarationExpression(VariableDeclarationExpression node)
            {
                if (node.Variable == oldVariable)
                {
                    node.Variable = newVariable;
                }
            }

            public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
            {
                if (node.Variable == oldVariable)
                {
                    node.Variable = newVariable;
                }
            }
        }

        /// <summary>
        /// This class takes care for removing redundant code from the yield state machine.
        /// </summary>
        private class YieldStateMachineCodeRemover
        {
            private ForEachStatement @foreach;
            private VariableReference enumeratorVariable;

            public YieldStateMachineCodeRemover(ForEachStatement @foreach, VariableReference enumeratorVariable)
            {
                this.@foreach = @foreach;
                this.enumeratorVariable = enumeratorVariable;
            }

            public void ProcessForEachStatement()
            {
                RemoveLastForeachStatementIfNeeded();
                RemoveFirstSuccessorIfNeeded();
            }

            private void RemoveLastForeachStatementIfNeeded()
            {
                if (@foreach.Body.Statements.Count == 0)
                {
                    return;
                }

                ExpressionStatement expressionStatement = @foreach.Body.Statements.Last() as ExpressionStatement;
                if (expressionStatement == null)
                {
                    return;
                }

                BinaryExpression binaryExpression = expressionStatement.Expression as BinaryExpression;
                if (binaryExpression == null)
                {
                    return;
                }

                if (!binaryExpression.IsAssignmentExpression)
                {
                    return;
                }

                VariableReferenceExpression variableRef = binaryExpression.Left as VariableReferenceExpression;
                if (variableRef == null || variableRef.Variable != @foreach.Variable.Variable)
                {
                    return;
                }

                LiteralExpression literalExpression = binaryExpression.Right as LiteralExpression;
                if (literalExpression != null && literalExpression.Value == null)
                {
                    RemoveStatement(this.@foreach.Body.Statements, expressionStatement);
                    return;
                }

                DefaultObjectExpression defaultObjectExpression = binaryExpression.Right as DefaultObjectExpression;
                if (defaultObjectExpression != null)
                {
                    RemoveStatement(this.@foreach.Body.Statements, expressionStatement);
                    return;
                }
            }

            private void RemoveFirstSuccessorIfNeeded()
            {
                BlockStatement block = @foreach.Parent as BlockStatement;
                int foreachIndex = block.Statements.IndexOf(@foreach);

                if (block.Statements.Count <= foreachIndex + 1)
                {
                    return;
                }

                ExpressionStatement expressionStatement = block.Statements[foreachIndex + 1] as ExpressionStatement;
                if (expressionStatement == null)
                {
                    return;
                }

                BinaryExpression binaryExpression = expressionStatement.Expression as BinaryExpression;
                if (binaryExpression == null)
                {
                    return;
                }

                VariableReferenceExpression variableRef = binaryExpression.Left as VariableReferenceExpression;
                if (variableRef == null)
                {
                    return;
                }

                if (variableRef.Variable != this.enumeratorVariable)
                {
                    return;
                }

                LiteralExpression literal = binaryExpression.Right as LiteralExpression;
                if (literal != null && literal.Value == null)
                {
                    RemoveStatement(block.Statements, expressionStatement);
                }

                ObjectCreationExpression objectCreation = binaryExpression.Right as ObjectCreationExpression;
                if (objectCreation != null)
                {
                    RemoveStatement(block.Statements, expressionStatement);
                }
            }

            private void RemoveStatement(StatementCollection collection, Statement statement)
            {
                collection.Remove(statement);
            }
        }
    }
}
