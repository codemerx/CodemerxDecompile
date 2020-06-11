namespace Telerik.JustDecompiler.Ast.Statements
{
    public class UnsafeBlockStatement:BlockStatement
    {
        public UnsafeBlockStatement(StatementCollection statements)
        {
            this.Statements = statements;
            foreach (Statement s in this.Statements)
            {
                s.Parent = this;
            }
        }

        public override Statement Clone()
        {
            UnsafeBlockStatement result = new UnsafeBlockStatement(new StatementCollection());
            foreach (Statement s in this.Statements)
            {
                result.AddStatement(s.Clone());
            }
            CopyParentAndLabel(result);
            return result;
        }

        public override Statement CloneStatementOnly()
        {
            UnsafeBlockStatement result = new UnsafeBlockStatement(new StatementCollection());
            foreach (Statement s in this.Statements)
            {
                result.AddStatement(s.CloneStatementOnly());
            }
            CopyParentAndLabel(result);
            return result;
        }

        public override CodeNodeType CodeNodeType
        {
            get
            {
                return CodeNodeType.UnsafeBlock;
            }
        }
    }
}
