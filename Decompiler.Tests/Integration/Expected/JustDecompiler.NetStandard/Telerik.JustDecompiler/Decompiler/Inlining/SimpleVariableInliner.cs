using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Decompiler.Inlining
{
	internal class SimpleVariableInliner : BaseCodeTransformer, IVariableInliner
	{
		private const int MaxCount = 10;

		private readonly TypeSystem typeSystem;

		private VariableDefinition variableDef;

		protected Expression value;

		protected SimpleVariableInliner.InliningResult status;

		protected bool valueHasSideEffects;

		public SimpleVariableInliner(TypeSystem typeSystem)
		{
			base();
			this.typeSystem = typeSystem;
			return;
		}

		private void Abort()
		{
			if (this.status != 1)
			{
				this.status = 2;
			}
			return;
		}

		protected virtual ICodeNode GetNewValue(VariableReferenceExpression node)
		{
			return this.value;
		}

		public bool TryInlineVariable(VariableDefinition variableDef, Expression value, ICodeNode target, bool aggressive, out ICodeNode result)
		{
			this.variableDef = variableDef;
			this.value = value;
			if (!aggressive)
			{
				V_1 = new SimpleVariableInliner.ASTNodeCounter();
				if (V_1.CountNodes(value) + V_1.CountNodes(target) - 1 > 10)
				{
					result = target;
					return false;
				}
			}
			this.valueHasSideEffects = (new SideEffectsFinder()).HasSideEffectsRecursive(value);
			this.status = 0;
			result = this.Visit(target);
			return this.status == 1;
		}

		public override ICodeNode Visit(ICodeNode node)
		{
			if (this.status == SimpleVariableInliner.InliningResult.NotFound)
			{
				node = this.Visit(node);
				if (this.valueHasSideEffects && this.status == SimpleVariableInliner.InliningResult.NotFound && SideEffectsFinder.HasSideEffects(node))
				{
					this.Abort();
				}
			}
			return node;
		}

		public override ICodeNode VisitBaseCtorExpression(BaseCtorExpression node)
		{
			node.set_InstanceReference((Expression)this.Visit(node.get_InstanceReference()));
			return this.VisitBaseCtorExpression(node);
		}

		public override ICodeNode VisitBinaryExpression(BinaryExpression node)
		{
			if (!node.get_IsAssignmentExpression() || node.get_Left().get_CodeNodeType() != 26)
			{
				return this.VisitBinaryExpression(node);
			}
			node.set_Right((Expression)this.Visit(node.get_Right()));
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
			node.set_Expression((Expression)this.Visit(node.get_Expression()));
			return node;
		}

		public override ICodeNode VisitForEachStatement(ForEachStatement node)
		{
			node.set_Collection((Expression)this.Visit(node.get_Collection()));
			return node;
		}

		public override ICodeNode VisitForStatement(ForStatement node)
		{
			node.set_Initializer((Expression)this.Visit(node.get_Initializer()));
			return node;
		}

		public override ICodeNode VisitIfElseIfStatement(IfElseIfStatement node)
		{
			stackVariable1 = node.get_ConditionBlocks();
			V_0 = node.get_ConditionBlocks().get_Item(0);
			stackVariable11 = (Expression)this.Visit(V_0.get_Key());
			V_0 = node.get_ConditionBlocks().get_Item(0);
			stackVariable1.set_Item(0, new KeyValuePair<Expression, BlockStatement>(stackVariable11, V_0.get_Value()));
			return node;
		}

		public override ICodeNode VisitIfStatement(IfStatement node)
		{
			node.set_Condition((Expression)this.Visit(node.get_Condition()));
			return node;
		}

		public override ICodeNode VisitLockStatement(LockStatement node)
		{
			node.set_Expression((Expression)this.Visit(node.get_Expression()));
			return node;
		}

		public override ICodeNode VisitSwitchStatement(SwitchStatement node)
		{
			node.set_Condition((Expression)this.Visit(node.get_Condition()));
			return node;
		}

		public override ICodeNode VisitThisCtorExpression(ThisCtorExpression node)
		{
			node.set_InstanceReference((Expression)this.Visit(node.get_InstanceReference()));
			return this.VisitThisCtorExpression(node);
		}

		public override ICodeNode VisitTryStatement(TryStatement node)
		{
			return node;
		}

		public override ICodeNode VisitUnaryExpression(UnaryExpression node)
		{
			if (this.status == 2)
			{
				throw new Exception("Invalid state");
			}
			V_0 = node.get_Operand();
			node.set_Operand((Expression)this.Visit(node.get_Operand()));
			if (node.get_Operator() != 1 || V_0 == node.get_Operand())
			{
				return node;
			}
			return Negator.Negate(node.get_Operand(), this.typeSystem);
		}

		public override ICodeNode VisitUsingStatement(UsingStatement node)
		{
			node.set_Expression((Expression)this.Visit(node.get_Expression()));
			return node;
		}

		public override ICodeNode VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			if (this.status == 2)
			{
				throw new Exception("Invalid state");
			}
			if ((object)node.get_Variable().Resolve() != (object)this.variableDef)
			{
				return node;
			}
			this.status = 1;
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
				base();
				return;
			}

			public int CountNodes(ICodeNode node)
			{
				this.count = 0;
				this.Visit(node);
				return this.count;
			}

			public override void Visit(ICodeNode node)
			{
				this.count = this.count + 1;
				this.Visit(node);
				return;
			}

			public override void VisitFixedStatement(FixedStatement node)
			{
				this.Visit(node.get_Expression());
				return;
			}

			public override void VisitForEachStatement(ForEachStatement node)
			{
				this.Visit(node.get_Collection());
				return;
			}

			public override void VisitForStatement(ForStatement node)
			{
				this.Visit(node.get_Initializer());
				return;
			}

			public override void VisitIfElseIfStatement(IfElseIfStatement node)
			{
				V_0 = node.get_ConditionBlocks().get_Item(0);
				this.Visit(V_0.get_Key());
				return;
			}

			public override void VisitIfStatement(IfStatement node)
			{
				this.Visit(node.get_Condition());
				return;
			}

			public override void VisitImplicitCastExpression(ImplicitCastExpression node)
			{
				V_0 = this.count;
				this.Visit(node.get_Expression());
				this.count = V_0;
				return;
			}

			public override void VisitInitializerExpression(InitializerExpression node)
			{
				this.count = this.count - 1;
				this.Visit(node.get_Expression());
				return;
			}

			public override void VisitSwitchStatement(SwitchStatement node)
			{
				this.Visit(node.get_Condition());
				return;
			}

			public override void VisitUsingStatement(UsingStatement node)
			{
				this.Visit(node.get_Expression());
				return;
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