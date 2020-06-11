using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
    class EmptyBlockLogicalConstruct : CFGBlockLogicalConstruct
    {
        private readonly int index;

        public EmptyBlockLogicalConstruct(int index)
            : base(null, new List<Expression>())
        {
            this.index = index;
        }

        public override int Index
        {
            get
            {
                return this.index;
            }
        }
    }
}
