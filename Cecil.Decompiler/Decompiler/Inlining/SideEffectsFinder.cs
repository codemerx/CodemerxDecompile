using System;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Decompiler.Inlining
{
    class SideEffectsFinder : BaseCodeVisitor
    {
        public static bool HasSideEffects(ICodeNode node)
        {
            if (node == null)
            {
                return false;
            }

            switch (node.CodeNodeType)
            {
                case CodeNodeType.MethodInvocationExpression:
                case CodeNodeType.TypeOfExpression:
                case CodeNodeType.ArrayCreationExpression:
                case CodeNodeType.ObjectCreationExpression:
                case CodeNodeType.StackAllocExpression:
                case CodeNodeType.EventReferenceExpression:
                case CodeNodeType.DelegateInvokeExpression:
                case CodeNodeType.BaseCtorExpression:
                case CodeNodeType.ThisCtorExpression:
                case CodeNodeType.BoxExpression:
                case CodeNodeType.ArrayLengthExpression:
                case CodeNodeType.DynamicConstructorInvocationExpression:
                case CodeNodeType.DynamicIndexerExpression:
                case CodeNodeType.DynamicMemberReferenceExpression:
                case CodeNodeType.AnonymousObjectCreationExpression:
                case CodeNodeType.AwaitExpression:
                case CodeNodeType.LambdaExpression:
                case CodeNodeType.DelegateCreationExpression:
                    return true;
                case CodeNodeType.PropertyReferenceExpression:
                    return !(node as PropertyReferenceExpression).IsSetter;
                case CodeNodeType.ExplicitCastExpression:
                    return !(node as ExplicitCastExpression).IsExplicitInterfaceCast;
                case CodeNodeType.FieldReferenceExpression:
                    return !(node as FieldReferenceExpression).IsSimpleStore;
                case CodeNodeType.ArrayIndexerExpression:
                    return !(node as ArrayIndexerExpression).IsSimpleStore;
                case CodeNodeType.BinaryExpression:
                    BinaryExpression binaryExpression = node as BinaryExpression;
                    return binaryExpression.IsChecked || binaryExpression.Operator == BinaryOperator.Divide || binaryExpression.Operator == BinaryOperator.Modulo;
            }
            return false;
        }

        bool hasSideEffects;
        public bool HasSideEffectsRecursive(ICodeNode node)
        {
            hasSideEffects = false;
            Visit(node);
            return hasSideEffects;
        }

        public override void Visit(ICodeNode node)
        {
            hasSideEffects = HasSideEffects(node);
            if (!hasSideEffects)
            {
                base.Visit(node);
            }
        }
    }
}
