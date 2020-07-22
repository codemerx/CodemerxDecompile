using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Decompiler.GotoElimination
{
    class ContinueAndBreakFinder : BaseCodeVisitor
    {
        public ICollection<BreakStatement> Breaks { get; private set; }
        public ICollection<ContinueStatement> Continues { get; private set; }

        public ContinueAndBreakFinder()
        {
            Breaks = new List<BreakStatement>();
            Continues = new List<ContinueStatement>();

        }

        public override void VisitWhileStatement(WhileStatement node)
        {
            return;
        }

        public override void VisitForEachStatement(ForEachStatement node)
        {
            return;
        }

        public override void VisitForStatement(ForStatement node)
        {
            return;
        }

        public override void VisitDoWhileStatement(DoWhileStatement node)
        {
            return;
        }

        public override void VisitSwitchStatement(SwitchStatement node)
        {
            return;
        }

        public override void VisitContinueStatement(ContinueStatement node)
        {
            Continues.Add(node);
            return;
        }

        public override void VisitBreakStatement(BreakStatement node)
        {
            Breaks.Add(node);
            return;
        }
    }
}
