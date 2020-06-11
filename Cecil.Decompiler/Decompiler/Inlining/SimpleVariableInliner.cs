using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.JustDecompiler.Ast;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Steps;

namespace Telerik.JustDecompiler.Decompiler.Inlining
{
    class SimpleVariableInliner : BaseCodeTransformer, IVariableInliner
    {
        private const int MaxCount = 10;

        private readonly TypeSystem typeSystem;

        private VariableDefinition variableDef;
        protected Expression value;
        protected InliningResult status;
        protected bool valueHasSideEffects;

        public SimpleVariableInliner(TypeSystem typeSystem)
        {
            this.typeSystem = typeSystem;
        }

        public bool TryInlineVariable(VariableDefinition variableDef, Expression value, ICodeNode target, bool aggressive, out ICodeNode result)
        {
            this.variableDef = variableDef;
            this.value = value;

            if (!aggressive)
            {
                ASTNodeCounter counter = new ASTNodeCounter();
                if (counter.CountNodes(value) + counter.CountNodes(target) - 1 > MaxCount)
                {
                    result = target;
                    return false;
                }
            }

            SideEffectsFinder sideEffectsFinder = new SideEffectsFinder();
            valueHasSideEffects = sideEffectsFinder.HasSideEffectsRecursive(value);

            status = InliningResult.NotFound;
            result = Visit(target);
            return status == InliningResult.Success;
        }

        public override ICodeNode Visit(ICodeNode node)
        {
            if (status == InliningResult.NotFound)
            {
                node = base.Visit(node);
                if (valueHasSideEffects && status == InliningResult.NotFound && SideEffectsFinder.HasSideEffects(node))
                {
                    Abort();
                }
            }

            return node;
        }

        public override ICodeNode VisitBinaryExpression(BinaryExpression node)
        {
            if (node.IsAssignmentExpression)
            {
                if (node.Left.CodeNodeType == CodeNodeType.VariableReferenceExpression)
                {
                    node.Right = (Expression)Visit(node.Right);
                    return node;
                }
            }
            return base.VisitBinaryExpression(node);
        }

        public override ICodeNode VisitVariableReferenceExpression(VariableReferenceExpression node)
        {
            if (status == InliningResult.Abort)
            {
                //sanity check
                throw new Exception("Invalid state");
            }

            if (node.Variable.Resolve() == variableDef)
            {
                status = InliningResult.Success;
                return GetNewValue(node);
            }
            return node;
        }

        public override ICodeNode VisitThisCtorExpression(ThisCtorExpression node)
        {
            node.InstanceReference = (Expression)Visit(node.InstanceReference);
            return base.VisitThisCtorExpression(node);
        }

        public override ICodeNode VisitBaseCtorExpression(BaseCtorExpression node)
        {
            node.InstanceReference = (Expression)Visit(node.InstanceReference);
            return base.VisitBaseCtorExpression(node);
        }

        protected virtual ICodeNode GetNewValue(VariableReferenceExpression node)
        {
            return value;
        }

        public override ICodeNode VisitUnaryExpression(UnaryExpression node)
        {
            if (status == InliningResult.Abort)
            {
                //sanity check
                throw new Exception("Invalid state");
            }

            Expression originalOperand = node.Operand;
            node.Operand = (Expression)Visit(node.Operand);
            if (node.Operator == UnaryOperator.LogicalNot && originalOperand != node.Operand)
            {
                return Negator.Negate(node.Operand, typeSystem);
            }
            return node;
        }

        public override ICodeNode VisitIfStatement(Ast.Statements.IfStatement node)
        {
            node.Condition = (Expression)Visit(node.Condition);
            return node;
        }

        public override ICodeNode VisitTryStatement(Ast.Statements.TryStatement node)
        {
            return node;
        }

        public override ICodeNode VisitWhileStatement(Ast.Statements.WhileStatement node)
        {
            return node;
        }

        public override ICodeNode VisitUsingStatement(Ast.Statements.UsingStatement node)
        {
            node.Expression = (Expression)Visit(node.Expression);
            return node;
        }

        public override ICodeNode VisitBlockStatement(Ast.Statements.BlockStatement node)
        {
            return node;
        }

        public override ICodeNode VisitCatchClause(Ast.Statements.CatchClause node)
        {
            return node;
        }

        public override ICodeNode VisitForEachStatement(Ast.Statements.ForEachStatement node)
        {
            node.Collection = (Expression)Visit(node.Collection);
            return node;
        }

        public override ICodeNode VisitForStatement(Ast.Statements.ForStatement node)
        {
            node.Initializer = (Expression)Visit(node.Initializer);
            return node;
        }

        public override ICodeNode VisitFixedStatement(Ast.Statements.FixedStatement node)
        {
            node.Expression = (Expression)Visit(node.Expression);
            return node;
        }

        public override ICodeNode VisitDoWhileStatement(Ast.Statements.DoWhileStatement node)
        {
            return node;
        }

        public override ICodeNode VisitDefaultCase(Ast.Statements.DefaultCase node)
        {
            return node;
        }

        public override ICodeNode VisitConditionCase(Ast.Statements.ConditionCase node)
        {
            return node;
        }

        public override ICodeNode VisitSwitchStatement(Ast.Statements.SwitchStatement node)
        {
            node.Condition = (Expression)Visit(node.Condition);
            return node;
        }

        public override ICodeNode VisitIfElseIfStatement(Ast.Statements.IfElseIfStatement node)
        {
            node.ConditionBlocks[0] = new KeyValuePair<Expression,Ast.Statements.BlockStatement>((Expression)Visit(node.ConditionBlocks[0].Key), node.ConditionBlocks[0].Value);
            return node;
        }

        public override ICodeNode VisitLockStatement(Ast.Statements.LockStatement node)
        {
            node.Expression = (Expression)Visit(node.Expression);
            return node;
        }

        private void Abort()
        {
            if (status != InliningResult.Success)
            {
                status = InliningResult.Abort;
            }
        }

        private class ASTNodeCounter : BaseCodeVisitor
        {
            private int count;

            public int CountNodes(ICodeNode node)
            {
                count = 0;
                Visit(node);
                return count;
            }

            public override void Visit(ICodeNode node)
            {
                count++;
                base.Visit(node);
            }

            public override void VisitImplicitCastExpression(ImplicitCastExpression node)
            {
                int initialCount = count;

                Visit(node.Expression);

                count = initialCount;
            }

            public override void VisitIfStatement(Ast.Statements.IfStatement node)
            {
                Visit(node.Condition);
            }

            public override void VisitUsingStatement(Ast.Statements.UsingStatement node)
            {
                Visit(node.Expression);
            }

            public override void VisitForEachStatement(Ast.Statements.ForEachStatement node)
            {
                Visit(node.Collection);
            }

            public override void VisitForStatement(Ast.Statements.ForStatement node)
            {
                Visit(node.Initializer);
            }

            public override void VisitFixedStatement(Ast.Statements.FixedStatement node)
            {
                Visit(node.Expression);
            }

            public override void VisitSwitchStatement(Ast.Statements.SwitchStatement node)
            {
                Visit(node.Condition);
            }

            public override void VisitIfElseIfStatement(Ast.Statements.IfElseIfStatement node)
            {
                Visit(node.ConditionBlocks[0].Key);
            }

			public override void VisitInitializerExpression(InitializerExpression node)
			{
				count--;
				Visit(node.Expression);
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
