using System;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Decompiler
{
    public class CompilerOptimizedSwitchByStringData : ICloneable
    {
        public CompilerOptimizedSwitchByStringData()
        {
            this.SwitchBlocksStartInstructions = new List<int>();
            this.SwitchBlocksToCasesMap = new Dictionary<int, List<int>>();
        }

        public List<int> SwitchBlocksStartInstructions { get; private set; }

        /// <summary>
        /// Maps every switch by string block to the corresponding cases blocks for this switch.
        /// The mapping is using the start instruction to represent a block.
        /// </summary>
        public Dictionary<int, List<int>> SwitchBlocksToCasesMap { get; private set; }

        public object Clone()
        {
            CompilerOptimizedSwitchByStringData clone = new CompilerOptimizedSwitchByStringData();

            clone.SwitchBlocksStartInstructions = new List<int>(this.SwitchBlocksStartInstructions);
            clone.SwitchBlocksToCasesMap = new Dictionary<int, List<int>>();
            foreach (KeyValuePair<int, List<int>> pair in this.SwitchBlocksToCasesMap)
            {
                clone.SwitchBlocksToCasesMap.Add(pair.Key, new List<int>(pair.Value));
            }

            return clone;
        }
    }
}
