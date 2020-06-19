using System;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Cil
{
    public class SwitchData
    {
        public InstructionBlock SwitchBlock { get; private set; }
        public InstructionBlock DefaultCase { get; internal set; }
        public InstructionBlock[] OrderedCasesArray { get; private set; }

        public SwitchData(InstructionBlock switchBlock, InstructionBlock defaultCase, IList<InstructionBlock> orderedCases)
        {
            SwitchBlock = switchBlock;
            DefaultCase = defaultCase;
            OrderedCasesArray = new InstructionBlock[orderedCases.Count];
            for (int i = 0; i < orderedCases.Count; i++)
            {
                OrderedCasesArray[i] = orderedCases[i];
            }
        }
    }
}
