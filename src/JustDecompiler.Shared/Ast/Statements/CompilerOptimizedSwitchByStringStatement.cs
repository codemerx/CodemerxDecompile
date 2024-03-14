using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Ast.Statements
{
    /// <summary>
    /// This statement is used to contain the switches by string, which were optimized by the C# 6.0 compiler.
    /// </summary>
    public class CompilerOptimizedSwitchByStringStatement : SwitchStatement
    {
        public CompilerOptimizedSwitchByStringStatement(Expression condition, IEnumerable<int> loadSwitchValueInstructionOffsets)
            : base(condition, null)
        {
            this.LoadSwitchValueInstructionOffsets = loadSwitchValueInstructionOffsets;
        }

        // Used to store the instructions which load the value used to switch on (e.g. variable) for all
        // cases except the first one. The instruction for the first case is stored in the condition expression.
        // This information is needed for the pdb generation, since we put sequence points on this instructions.
        public IEnumerable<int> LoadSwitchValueInstructionOffsets { get; private set; }
    }
}
