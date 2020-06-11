#region license
//
//	(C) 2005 - 2007 db4objects Inc. http://www.db4o.com
//	(C) 2007 - 2008 Novell, Inc. http://www.novell.com
//	(C) 2007 - 2008 Jb Evain http://evain.net
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion

using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace Telerik.JustDecompiler.Cil
{
    public class ControlFlowGraph
    {
        public ControlFlowGraph(
            MethodBody body, 
            InstructionBlock[] blocks, 
            Dictionary<int, InstructionBlock> instructiontoBlockMapping,
            Dictionary<InstructionBlock, SwitchData> switchBlocksInformation, 
            Dictionary<int, Instruction> offsetToInstruction)
        {
            this.MethodBody = body;
            this.Blocks = blocks;
            this.InstructionToBlockMapping = instructiontoBlockMapping;
            this.SwitchBlocksInformation = switchBlocksInformation;
            this.OffsetToInstruction = offsetToInstruction;
        }

        public MethodBody MethodBody { get; private set; }

        public InstructionBlock[] Blocks { get; private set; }

        public Dictionary<int, Instruction> OffsetToInstruction { get; private set; }

        public Dictionary<int, InstructionBlock> InstructionToBlockMapping { get; private set; }

        public Dictionary<InstructionBlock, SwitchData> SwitchBlocksInformation { get; private set; }

        public Collection<ExceptionHandler> RawExceptionHandlers
        {
            get { return MethodBody.ExceptionHandlers; }
        }

        public static ControlFlowGraph Create(MethodDefinition method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            if (!method.HasBody)
            {
                throw new ArgumentException();
            }

            ControlFlowGraphBuilder builder = new ControlFlowGraphBuilder(method);
            return builder.CreateGraph();
        }

        internal void RemoveBlockAt(int index)
        {
            InstructionBlock removedBlock = Blocks[index];

            Instruction nextInstruction = removedBlock.Last.Next;

            if (removedBlock.Predecessors.Count > 0)
            {
                throw new Exception("The block to be removed cannot have predecessors");
            }

            InstructionBlock[] oldBlocks = Blocks;
            Blocks = new InstructionBlock[Blocks.Length - 1];

            for (int i = 0; i < Blocks.Length; i++)
            {
                int passedIndex = i >= index ? 1 : 0;
                Blocks[i] = oldBlocks[i + passedIndex];
                Blocks[i].Index = i;
                Blocks[i].Predecessors.Remove(removedBlock);

                //if (i == 0)
                //{
                //    Blocks[i].First.Previous = null;
                //}
                //else
                //{
                    if (Blocks[i].First.Previous == removedBlock.Last)
                    {
                        Blocks[i].First.Previous = removedBlock.First.Previous;
                    }
                    if (Blocks[i].Last.Next == removedBlock.First)
                    {
                        Blocks[i].Last.Next = removedBlock.Last.Next;
                    }
                //}
            }

            InstructionToBlockMapping.Remove(removedBlock.First.Offset);
            SwitchBlocksInformation.Remove(removedBlock);
            removedBlock.Successors = new InstructionBlock[0];

            foreach (Instruction instruction in removedBlock)
            {
                OffsetToInstruction.Remove(instruction.Offset);
            }

            foreach (ExceptionHandler exceptionHandler in MethodBody.ExceptionHandlers)
            {
                if (exceptionHandler.TryStart == removedBlock.First)
                {
                    exceptionHandler.TryStart = nextInstruction;
                }
                if (exceptionHandler.TryEnd == removedBlock.First)
                {
                    exceptionHandler.TryEnd = nextInstruction;
                }
                if (exceptionHandler.HandlerStart == removedBlock.First)
                {
                    exceptionHandler.HandlerStart = nextInstruction;
                }
                if (exceptionHandler.HandlerEnd == removedBlock.First)
                {
                    exceptionHandler.HandlerEnd = nextInstruction;
                }
                if (exceptionHandler.FilterStart == removedBlock.First)
                {
                    exceptionHandler.FilterStart = nextInstruction;
                }
                if (exceptionHandler.FilterEnd == removedBlock.First)
                {
                    exceptionHandler.FilterEnd = nextInstruction;
                }
            }

            removedBlock.Index = -1;
        }
    }
}
