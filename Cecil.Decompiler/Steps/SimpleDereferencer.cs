using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Steps
{
    public class SimpleDereferencer : BaseCodeTransformer
    {
        public override ICodeNode VisitUnaryExpression(UnaryExpression node)
        {
            if (node.Operator == UnaryOperator.AddressDereference)
            {
                UnaryExpression unaryOperand = node.Operand as UnaryExpression;
                if (unaryOperand != null && (unaryOperand.Operator == UnaryOperator.AddressReference || unaryOperand.Operator == UnaryOperator.AddressOf))
                {
                    return Visit(unaryOperand.Operand);
                }
            }

            return base.VisitUnaryExpression(node);
        }
    }
}
