using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Steps;

namespace Telerik.JustDecompiler.Decompiler.Inlining
{
	internal class SimpleVariableInliner : BaseCodeTransformer, IVariableInliner
	{
		private const int MaxCount = 10;

		private readonly TypeSystem typeSystem;

		private VariableDefinition variableDef;

		protected Expression @value;

		protected SimpleVariableInliner.InliningResult status;

		protected bool valueHasSideEffects;

		public SimpleVariableInliner(TypeSystem typeSystem)
		{
			this.typeSystem = typeSystem;
		}

		private void Abort()
		{
			if (this.status != SimpleVariableInliner.InliningResult.Success)
			{
				this.status = SimpleVariableInliner.InliningResult.Abort;
			}
		}

		protected virtual ICodeNode GetNewValue(VariableReferenceExpression node)
		{
			return this.@value;
		}

		public bool TryInlineVariable(VariableDefinition variableDef, Expression value, ICodeNode target, bool aggressive, out ICodeNode result)
		{
			this.variableDef = variableDef;
			this.@value = value;
			if (!aggressive)
			{
				SimpleVariableInliner.ASTNodeCounter aSTNodeCounter = new SimpleVariableInliner.ASTNodeCounter();
				if (aSTNodeCounter.CountNodes(value) + aSTNodeCounter.CountNodes(target) - 1 > 10)
				{
					result = target;
					return false;
				}
			}
			this.valueHasSideEffects = (new SideEffectsFinder()).HasSideEffectsRecursive(value);
			this.status = SimpleVariableInliner.InliningResult.NotFound;
			result = this.Visit(target);
			return this.status == SimpleVariableInliner.InliningResult.Success;
		}

		public override ICodeNode Visit(ICodeNode node)
		{
			if (this.status == SimpleVariableInliner.InliningResult.NotFound)
			{
				node = base.Visit(node);
				if (this.valueHasSideEffects && this.status == SimpleVariableInliner.InliningResult.NotFound && SideEffectsFinder.HasSideEffects(node))
				{
					this.Abort();
				}
			}
			return node;
		}

		public override ICodeNode VisitBaseCtorExpression(BaseCtorExpression node)
		{
			node.InstanceReference = (Expression)this.Visit(node.InstanceReference);
			return base.VisitBaseCtorExpression(node);
		}

		public override ICodeNode VisitBinaryExpression(BinaryExpression node)
		{
			if (!node.IsAssignmentExpression || node.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression)
			{
				return base.VisitBinaryExpression(node);
			}
			node.Right = (Expression)this.Visit(node.Right);
			return node;
		}

		public override ICodeNode VisitBlockStatement(BlockStatement node)
		{
			return node;
		}

		public override ICodeNode VisitCatchClause(CatchClause node)
		{
			return node;
		}

		public override ICodeNode VisitConditionCase(ConditionCase node)
		{
			return node;
		}

		public override ICodeNode VisitDefaultCase(DefaultCase node)
		{
			return node;
		}

		public override ICodeNode VisitDoWhileStatement(DoWhileStatement node)
		{
			return node;
		}

		public override ICodeNode VisitFixedStatement(FixedStatement node)
		{
			node.Expression = (Expression)this.Visit(node.Expression);
			return node;
		}

		public override ICodeNode VisitForEachStatement(ForEachStatement node)
		{
			node.Collection = (Expression)this.Visit(node.Collection);
			return node;
		}

		public override ICodeNode VisitForStatement(ForStatement node)
		{
			node.Initializer = (Expression)this.Visit(node.Initializer);
			return node;
		}

		public override ICodeNode VisitIfElseIfStatement(IfElseIfStatement node)
		{
			List<KeyValuePair<Expression, BlockStatement>> conditionBlocks = node.ConditionBlocks;
			KeyValuePair<Expression, BlockStatement> item = node.ConditionBlocks[0];
			Expression expression = (Expression)this.Visit(item.Key);
			item = node.ConditionBlocks[0];
			conditionBlocks[0] = new KeyValuePair<Expression, BlockStatement>(expression, item.Value);
			return node;
		}

		public override ICodeNode VisitIfStatement(IfStatement node)
		{
			node.Condition = (Expression)this.Visit(node.Condition);
			return node;
		}

		public override ICodeNode VisitLockStatement(LockStatement node)
		{
			node.Expression = (Expression)this.Visit(node.Expression);
			return node;
		}

		public override ICodeNode VisitSwitchStatement(SwitchStatement node)
		{
			node.Condition = (Expression)this.Visit(node.Condition);
			return node;
		}

		public override ICodeNode VisitThisCtorExpression(ThisCtorExpression node)
		{
			node.InstanceReference = (Expression)this.Visit(node.InstanceReference);
			return base.VisitThisCtorExpression(node);
		}

		public override ICodeNode VisitTryStatement(TryStatement node)
		{
			return node;
		}

		public override ICodeNode VisitUnaryExpression(UnaryExpression node)
		{
			if (this.status == SimpleVariableInliner.InliningResult.Abort)
			{
				throw new Exception("Invalid state");
			}
			Expression operand = node.Operand;
			node.Operand = (Expression)this.Visit(node.Operand);
			if (node.Operator != UnaryOperator.LogicalNot || operand == node.Operand)
			{
				return node;
			}
			return Negator.Negate(node.Operand, this.typeSystem);
		}

		public override ICodeNode VisitUsingStatement(UsingStatement node)
		{
			node.Expression = (Expression)this.Visit(node.Expression);
			return node;
		}

		public override ICodeNode VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			if (this.status == SimpleVariableInliner.InliningResult.Abort)
			{
				throw new Exception("Invalid state");
			}
			if (node.Variable.Resolve() != this.variableDef)
			{
				return node;
			}
			this.status = SimpleVariableInliner.InliningResult.Success;
			return this.GetNewValue(node);
		}

		public override ICodeNode VisitWhileStatement(WhileStatement node)
		{
			return node;
		}

		private class ASTNodeCounter : BaseCodeVisitor
		{
			private int count;

			public ASTNodeCounter()
			{
			}

			public int CountNodes(ICodeNode node)
			{
				this.count = 0;
				this.Visit(node);
				return this.count;
			}

			public override void Visit(ICodeNode node)
			{
				this.count++;
				base.Visit(node);
			}

			public override void VisitFixedStatement(FixedStatement node)
			{
				this.Visit(node.Expression);
			}

			public override void VisitForEachStatement(ForEachStatement node)
			{
				this.Visit(node.Collection);
			}

			public override void VisitForStatement(ForStatement node)
			{
				this.Visit(node.Initializer);
			}

			public override void VisitIfElseIfStatement(IfElseIfStatement node)
			{
				this.Visit(node.ConditionBlocks[0].Key);
			}

			public override void VisitIfStatement(IfStatement node)
			{
				this.Visit(node.Condition);
			}

			public override void VisitImplicitCastExpression(ImplicitCastExpression node)
			{
				int num = this.count;
				this.Visit(node.Expression);
				this.count = num;
			}

			public override void VisitInitializerExpression(InitializerExpression node)
			{
				this.count--;
				this.Visit(node.Expression);
			}

			public override void VisitSwitchStatement(SwitchStatement node)
			{
				this.Visit(node.Condition);
			}

			public override void VisitUsingStatement(UsingStatement node)
			{
				this.Visit(node.Expression);
			}
		}

		protected enum InliningResult
		{
			NotFound,
			Success,
			Abort
		}
	}
}