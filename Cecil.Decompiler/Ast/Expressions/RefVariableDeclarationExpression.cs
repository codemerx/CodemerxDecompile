using System.Collections.Generic;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public class RefVariableDeclarationExpression : VariableDeclarationExpression
    {
        public RefVariableDeclarationExpression(VariableDefinition variable, IEnumerable<Instruction> instructions)
            : base(variable, instructions)
        {
        }

        public override bool Equals(Expression other)
        {
            if (!(other is RefVariableDeclarationExpression))
            {
                return false;
            }
            RefVariableDeclarationExpression otherVar = other as RefVariableDeclarationExpression;
            return this.Variable.Resolve() == otherVar.Variable.Resolve();
        }

        public override Expression Clone()
        {
            return new RefVariableDeclarationExpression(Variable, instructions);
        }

        public override Expression CloneExpressionOnly()
        {
            RefVariableDeclarationExpression result = new RefVariableDeclarationExpression(Variable, null);
            return result;
        }

        public override CodeNodeType CodeNodeType
        {
            get
            {
                return CodeNodeType.RefVariableDeclarationExpression;
            }
        }
    }
}
