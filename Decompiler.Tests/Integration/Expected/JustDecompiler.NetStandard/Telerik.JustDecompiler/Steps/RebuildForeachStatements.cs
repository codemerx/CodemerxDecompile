using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class RebuildForeachStatements : BaseCodeVisitor, IDecompilationStep
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

		private void AttachForeach()
		{
			this.GenerateForeachStatement();
			if (!this.isEnumeratorUsedInsideForEach)
			{
				BlockStatement parent = this.theTry.Parent as BlockStatement;
				parent.Statements.Remove(this.enumeratorAssignmentStatement);
				int num = parent.Statements.IndexOf(this.theTry);
				parent.Statements.RemoveAt(num);
				parent.AddStatementAt(num, this.@foreach);
				(new RebuildForeachStatements.YieldStateMachineCodeRemover(this.@foreach, this.theEnumerator)).ProcessForEachStatement();
				this.CopyLabel();
				this.CheckVariable();
				this.ClearState();
				this.VisitForEachStatement(parent.Statements[num] as ForEachStatement);
			}
		}

		private bool CanContainForeach(TryStatement tryStatement)
		{
			if (tryStatement.CatchClauses.Count != 0 || tryStatement.Try.Statements.Count != 1 || tryStatement.Finally == null || tryStatement.Finally.Body.Statements.Count != 1 && tryStatement.Finally.Body.Statements.Count != 2)
			{
				return false;
			}
			return this.IsValidFinally(tryStatement.Finally.Body);
		}

		private void CheckVariable()
		{
			if (this.foreachVariables.Contains(this.foreachVariable))
			{
				VariableDefinition variableDefinition = this.foreachVariable;
				this.foreachVariable = new VariableDefinition(this.foreachVariable.VariableType, this.methodContext.Method);
				this.foreachVariableInstructions.Clear();
				this.methodContext.Variables.Add(this.foreachVariable);
				this.methodContext.VariablesToRename.Add(this.foreachVariable);
				(new RebuildForeachStatements.ForeachVariableChanger(variableDefinition, this.foreachVariable)).Visit(this.@foreach);
			}
			this.foreachVariables.Add(this.foreachVariable);
		}

		private void ClearState()
		{
			this.insideTry = false;
			this.foundEnumeratorAssignment = false;
			this.foundWhile = false;
			this.foreachCollection = null;
			this.foreachVariable = null;
			this.foreachVariableInstructions.Clear();
			this.foreachCollectionInstructions.Clear();
			this.foreachBody = new BlockStatement();
			this.theEnumerator = null;
			this.theTry = null;
			this.@foreach = null;
			this.enumeratorAssignmentStatement = null;
			this.foreachVariableType = null;
			this.isEnumeratorUsedInsideForEach = false;
			this.foreachConditionInstructions = null;
		}

		private void CopyLabel()
		{
			if (this.theTry.Label != String.Empty)
			{
				this.@foreach.Label = this.theTry.Label;
				return;
			}
			if (this.theTry.Try.Label != String.Empty)
			{
				this.@foreach.Label = this.theTry.Try.Label;
				return;
			}
			if (this.theTry.Try.Statements[0].Label != String.Empty)
			{
				this.@foreach.Label = this.theTry.Try.Statements[0].Label;
			}
		}

		private void GenerateForeachStatement()
		{
			if (this.foreachVariable == null)
			{
				this.foreachVariable = new VariableDefinition(this.foreachVariableType, this.methodContext.Method);
				this.foreachVariableInstructions.Clear();
				this.methodContext.VariablesToRename.Add(this.foreachVariable);
			}
			VariableDeclarationExpression variableDeclarationExpression = new VariableDeclarationExpression(this.foreachVariable, this.foreachVariableInstructions);
			Expression thisReferenceExpression = this.foreachCollection.CloneAndAttachInstructions(this.foreachCollectionInstructions);
			if (thisReferenceExpression is BaseReferenceExpression)
			{
				thisReferenceExpression = new ThisReferenceExpression(this.methodContext.Method.DeclaringType, thisReferenceExpression.UnderlyingSameMethodInstructions);
			}
			this.@foreach = new ForEachStatement(variableDeclarationExpression, thisReferenceExpression, this.foreachBody, this.foreachConditionInstructions, this.theTry.Finally.UnderlyingSameMethodInstructions);
			(new RebuildForeachStatements.GetCurrentFixer(this.theEnumerator, this.foreachVariable)).Visit(this.@foreach);
			RebuildForeachStatements.IsEnumeratorUsedVisitor isEnumeratorUsedVisitor = new RebuildForeachStatements.IsEnumeratorUsedVisitor(this.theEnumerator);
			isEnumeratorUsedVisitor.Visit(this.@foreach);
			this.isEnumeratorUsedInsideForEach = isEnumeratorUsedVisitor.IsEnumeratorUsed;
		}

		private bool IsEnumeratorAssignment(Expression expression)
		{
			BinaryExpression binaryExpression = expression as BinaryExpression;
			if (binaryExpression == null || !binaryExpression.IsAssignmentExpression)
			{
				return false;
			}
			Expression right = binaryExpression.Right;
			if (right is MethodInvocationExpression)
			{
				MethodInvocationExpression methodInvocationExpression = right as MethodInvocationExpression;
				if (this.IsGetEnumerator(methodInvocationExpression))
				{
					if (!(binaryExpression.Left is VariableReferenceExpression))
					{
						return false;
					}
					this.foreachCollectionInstructions.Clear();
					this.foreachCollectionInstructions.AddRange(binaryExpression.Left.UnderlyingSameMethodInstructions);
					this.foreachCollectionInstructions.AddRange(binaryExpression.MappedInstructions);
					this.foreachCollectionInstructions.AddRange(methodInvocationExpression.InvocationInstructions);
					this.theEnumerator = (binaryExpression.Left as VariableReferenceExpression).Variable;
					return true;
				}
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

		private bool IsForeach(WhileStatement node)
		{
			if (node.Condition is UnaryExpression)
			{
				UnaryExpression condition = node.Condition as UnaryExpression;
				if (condition.Operator == UnaryOperator.None && condition.Operand is MethodInvocationExpression)
				{
					return this.IsMoveNextCall(condition.Operand as MethodInvocationExpression);
				}
			}
			return false;
		}

		private bool IsForeachVariableAssignment(BinaryExpression assignment)
		{
			Expression right;
			if (assignment == null)
			{
				return false;
			}
			if (!(assignment.Right is ExplicitCastExpression))
			{
				if (!(assignment.Right is MethodInvocationExpression) && !(assignment.Right is PropertyReferenceExpression))
				{
					return false;
				}
				right = assignment.Right;
			}
			else
			{
				right = (assignment.Right as ExplicitCastExpression).Expression;
			}
			if (!this.IsGetCurrent(right))
			{
				return false;
			}
			VariableReferenceExpression left = assignment.Left as VariableReferenceExpression;
			if (left == null)
			{
				return false;
			}
			this.foreachVariable = left.Variable.Resolve();
			this.foreachVariableInstructions.AddRange(assignment.UnderlyingSameMethodInstructions);
			this.foreachVariableType = assignment.Right.ExpressionType;
			return true;
		}

		private bool IsGetCurrent(Expression expression)
		{
			MethodInvocationExpression methodInvocationExpression = expression as MethodInvocationExpression;
			if (methodInvocationExpression == null)
			{
				PropertyReferenceExpression propertyReferenceExpression = expression as PropertyReferenceExpression;
				if (propertyReferenceExpression == null)
				{
					return false;
				}
				return propertyReferenceExpression.Property.Name == "Current";
			}
			if (!(methodInvocationExpression.MethodExpression.Target is VariableReferenceExpression) || methodInvocationExpression.MethodExpression.Method.Name != "get_Current")
			{
				return false;
			}
			if ((methodInvocationExpression.MethodExpression.Target as VariableReferenceExpression).Variable != this.theEnumerator)
			{
				return false;
			}
			return true;
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

		private bool IsMoveNextCall(MethodInvocationExpression invocation)
		{
			if (invocation == null)
			{
				return false;
			}
			if (invocation.MethodExpression.Method.Name == "MoveNext")
			{
				VariableReferenceExpression target = invocation.MethodExpression.Target as VariableReferenceExpression;
				if (target != null && target.Variable == this.theEnumerator)
				{
					return true;
				}
			}
			return false;
		}

		private bool IsValidFinally(BlockStatement blockStatement)
		{
			IfStatement item;
			if (blockStatement.Statements.Count != 1)
			{
				item = blockStatement.Statements[1] as IfStatement;
			}
			else
			{
				item = blockStatement.Statements[0] as IfStatement;
				if (item == null)
				{
					ExpressionStatement expressionStatement = blockStatement.Statements[0] as ExpressionStatement;
					if (expressionStatement == null)
					{
						return false;
					}
					return this.IsEnumeratorDispose(expressionStatement.Expression as MethodInvocationExpression);
				}
			}
			if (item == null)
			{
				return false;
			}
			if (item.Then.Statements.Count != 1)
			{
				return false;
			}
			ExpressionStatement item1 = item.Then.Statements[0] as ExpressionStatement;
			if (item1 == null)
			{
				return false;
			}
			return this.IsEnumeratorDispose(item1.Expression as MethodInvocationExpression);
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.methodContext = context.MethodContext;
			this.ClearState();
			this.foreachBody = new BlockStatement();
			this.Visit(body);
			return body;
		}

		public override void VisitBlockStatement(BlockStatement node)
		{
			for (int i = 0; i < node.Statements.Count; i++)
			{
				int count = node.Statements.Count;
				this.Visit(node.Statements[i]);
				int num = node.Statements.Count;
				if (count > num)
				{
					i = i - (count - num);
				}
			}
		}

		public override void VisitExpressionStatement(ExpressionStatement node)
		{
			if (!this.foundWhile)
			{
				if (this.IsEnumeratorAssignment(node.Expression))
				{
					this.foundEnumeratorAssignment = true;
					this.enumeratorAssignmentStatement = node;
					return;
				}
			}
			else if (node.Expression is BinaryExpression)
			{
				if (!this.IsForeachVariableAssignment(node.Expression as BinaryExpression) && this.shouldAdd)
				{
					this.foreachBody.AddStatement(node);
					return;
				}
			}
			else if (node.Expression is MethodInvocationExpression || node.Expression is PropertyReferenceExpression)
			{
				if (this.IsGetCurrent(node.Expression))
				{
					this.foreachVariableType = node.Expression.ExpressionType;
					return;
				}
				if (this.shouldAdd)
				{
					this.foreachBody.AddStatement(node);
					return;
				}
			}
			else if (node.Expression is ExplicitCastExpression)
			{
				ExplicitCastExpression expression = node.Expression as ExplicitCastExpression;
				if (this.IsGetCurrent(expression.Expression))
				{
					this.foreachVariableType = expression.ExpressionType;
					return;
				}
				if (this.shouldAdd)
				{
					this.foreachBody.AddStatement(node);
					return;
				}
			}
			else if (node.Expression is BoxExpression)
			{
				BoxExpression boxExpression = node.Expression as BoxExpression;
				if (this.IsGetCurrent(boxExpression.BoxedExpression))
				{
					this.foreachVariableType = boxExpression.BoxedAs;
					return;
				}
				if (this.shouldAdd)
				{
					this.foreachBody.AddStatement(node);
					return;
				}
			}
			else if (this.shouldAdd)
			{
				this.foreachBody.AddStatement(node);
			}
		}

		public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			if (!this.foundWhile && (this.IsGetCurrent(node) || this.IsMoveNextCall(node)))
			{
				this.ClearState();
				return;
			}
			if (this.IsGetCurrent(node))
			{
				this.foreachVariableType = node.ExpressionType;
			}
		}

		public override void VisitPropertyReferenceExpression(PropertyReferenceExpression node)
		{
			if (!this.foundWhile && this.IsGetCurrent(node))
			{
				this.ClearState();
				return;
			}
			if (this.IsGetCurrent(node))
			{
				this.foreachVariableType = node.ExpressionType;
			}
		}

		public override void VisitTryStatement(TryStatement node)
		{
			if (!this.foundEnumeratorAssignment)
			{
				this.insideTry = false;
				base.VisitTryStatement(node);
				return;
			}
			if (this.CanContainForeach(node))
			{
				this.insideTry = true;
				this.theTry = node;
				base.VisitTryStatement(node);
			}
			this.insideTry = false;
		}

		public override void VisitWhileStatement(WhileStatement node)
		{
			if (!this.foundEnumeratorAssignment || !this.insideTry)
			{
				base.VisitWhileStatement(node);
			}
			else
			{
				if (!this.IsForeach(node))
				{
					this.ClearState();
					base.VisitWhileStatement(node);
					return;
				}
				BlockStatement parent = this.theTry.Parent as BlockStatement;
				if (parent == null || parent != this.enumeratorAssignmentStatement.Parent || parent.Statements.IndexOf(this.enumeratorAssignmentStatement) + 1 != parent.Statements.IndexOf(this.theTry))
				{
					this.ClearState();
					base.VisitWhileStatement(node);
					return;
				}
				this.foreachConditionInstructions = node.Condition.UnderlyingSameMethodInstructions;
				this.foundWhile = true;
				this.shouldAdd = true;
				foreach (Statement statement in node.Body.Statements)
				{
					if (statement is ExpressionStatement)
					{
						this.VisitExpressionStatement(statement as ExpressionStatement);
					}
					else
					{
						this.foreachBody.AddStatement(statement);
					}
					if (this.foreachVariableType != null)
					{
						continue;
					}
					RebuildForeachStatements.ForeachElementTypeFinder foreachElementTypeFinder = new RebuildForeachStatements.ForeachElementTypeFinder(this.theEnumerator);
					foreachElementTypeFinder.Visit(statement);
					this.foreachVariableType = foreachElementTypeFinder.ResultingType;
				}
				if (this.foreachVariableType == null)
				{
					this.ClearState();
					base.VisitWhileStatement(node);
					return;
				}
				this.AttachForeach();
				if (this.isEnumeratorUsedInsideForEach)
				{
					this.ClearState();
					base.VisitWhileStatement(node);
					return;
				}
			}
		}

		private class ForeachElementTypeFinder : BaseCodeVisitor
		{
			private readonly VariableReference theEnumerator;

			public TypeReference ResultingType
			{
				get;
				set;
			}

			public ForeachElementTypeFinder(VariableReference theEnumerator)
			{
				this.theEnumerator = theEnumerator;
			}

			private bool IsGetCurrent(Expression expression)
			{
				MethodInvocationExpression methodInvocationExpression = expression as MethodInvocationExpression;
				if (methodInvocationExpression == null)
				{
					PropertyReferenceExpression propertyReferenceExpression = expression as PropertyReferenceExpression;
					if (propertyReferenceExpression == null)
					{
						return false;
					}
					return propertyReferenceExpression.Property.Name == "Current";
				}
				if (!(methodInvocationExpression.MethodExpression.Target is VariableReferenceExpression) || methodInvocationExpression.MethodExpression.Method.Name != "get_Current")
				{
					return false;
				}
				if ((methodInvocationExpression.MethodExpression.Target as VariableReferenceExpression).Variable != this.theEnumerator)
				{
					return false;
				}
				return true;
			}

			public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
			{
				if (this.IsGetCurrent(node))
				{
					this.ResultingType = node.ExpressionType;
				}
				base.VisitMethodInvocationExpression(node);
			}

			public override void VisitPropertyReferenceExpression(PropertyReferenceExpression node)
			{
				if (this.IsGetCurrent(node))
				{
					this.ResultingType = node.ExpressionType;
				}
				base.VisitPropertyReferenceExpression(node);
			}

			public override void VisitTryStatement(TryStatement node)
			{
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
				if (node.Variable == this.oldVariable)
				{
					node.Variable = this.newVariable;
				}
			}

			public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
			{
				if (node.Variable == this.oldVariable)
				{
					node.Variable = this.newVariable;
				}
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
				if (!(node.MethodExpression.Target is VariableReferenceExpression) || (node.MethodExpression.Target as VariableReferenceExpression).Variable != this.enumerator || !(node.MethodExpression.Method.Name == "get_Current"))
				{
					return base.VisitMethodInvocationExpression(node);
				}
				return new VariableReferenceExpression(this.foreachVariable, null);
			}

			public override ICodeNode VisitPropertyReferenceExpression(PropertyReferenceExpression node)
			{
				if (node.Property.Name == "Current")
				{
					VariableReferenceExpression target = node.Target as VariableReferenceExpression;
					if (target != null && target.Variable == this.enumerator)
					{
						return new VariableReferenceExpression(this.foreachVariable, null);
					}
				}
				return base.VisitPropertyReferenceExpression(node);
			}
		}

		private class IsEnumeratorUsedVisitor : BaseCodeVisitor
		{
			private readonly VariableReference enumerator;

			public bool IsEnumeratorUsed
			{
				get;
				set;
			}

			public IsEnumeratorUsedVisitor(VariableReference enumerator)
			{
				this.enumerator = enumerator;
				this.IsEnumeratorUsed = false;
			}

			public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
			{
				if (node.MethodExpression.Target is VariableReferenceExpression && (node.MethodExpression.Target as VariableReferenceExpression).Variable == this.enumerator && node.MethodExpression.Method.Name != "get_Current")
				{
					this.IsEnumeratorUsed = true;
				}
				base.VisitMethodInvocationExpression(node);
			}

			public override void VisitPropertyReferenceExpression(PropertyReferenceExpression node)
			{
				if (node.Property.Name != "Current")
				{
					VariableReferenceExpression target = node.Target as VariableReferenceExpression;
					if (target != null && target.Variable == this.enumerator)
					{
						this.IsEnumeratorUsed = true;
					}
				}
				base.VisitPropertyReferenceExpression(node);
			}

			public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
			{
				if (node.Variable == this.enumerator)
				{
					this.IsEnumeratorUsed = true;
				}
				base.VisitVariableReferenceExpression(node);
			}
		}

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
				this.RemoveLastForeachStatementIfNeeded();
				this.RemoveFirstSuccessorIfNeeded();
			}

			private void RemoveFirstSuccessorIfNeeded()
			{
				BlockStatement parent = this.@foreach.Parent as BlockStatement;
				int num = parent.Statements.IndexOf(this.@foreach);
				if (parent.Statements.Count <= num + 1)
				{
					return;
				}
				ExpressionStatement item = parent.Statements[num + 1] as ExpressionStatement;
				if (item == null)
				{
					return;
				}
				BinaryExpression expression = item.Expression as BinaryExpression;
				if (expression == null)
				{
					return;
				}
				VariableReferenceExpression left = expression.Left as VariableReferenceExpression;
				if (left == null)
				{
					return;
				}
				if (left.Variable != this.enumeratorVariable)
				{
					return;
				}
				LiteralExpression right = expression.Right as LiteralExpression;
				if (right != null && right.Value == null)
				{
					this.RemoveStatement(parent.Statements, item);
				}
				if (expression.Right is ObjectCreationExpression)
				{
					this.RemoveStatement(parent.Statements, item);
				}
			}

			private void RemoveLastForeachStatementIfNeeded()
			{
				if (this.@foreach.Body.Statements.Count == 0)
				{
					return;
				}
				ExpressionStatement expressionStatement = this.@foreach.Body.Statements.Last<Statement>() as ExpressionStatement;
				if (expressionStatement == null)
				{
					return;
				}
				BinaryExpression expression = expressionStatement.Expression as BinaryExpression;
				if (expression == null)
				{
					return;
				}
				if (!expression.IsAssignmentExpression)
				{
					return;
				}
				VariableReferenceExpression left = expression.Left as VariableReferenceExpression;
				if (left == null || left.Variable != this.@foreach.Variable.Variable)
				{
					return;
				}
				LiteralExpression right = expression.Right as LiteralExpression;
				if (right != null && right.Value == null)
				{
					this.RemoveStatement(this.@foreach.Body.Statements, expressionStatement);
					return;
				}
				if (!(expression.Right is DefaultObjectExpression))
				{
					return;
				}
				this.RemoveStatement(this.@foreach.Body.Statements, expressionStatement);
			}

			private void RemoveStatement(StatementCollection collection, Statement statement)
			{
				collection.Remove(statement);
			}
		}
	}
}