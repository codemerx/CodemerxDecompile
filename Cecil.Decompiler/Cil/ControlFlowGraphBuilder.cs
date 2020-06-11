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
    class ControlFlowGraphBuilder
    {
        private readonly MethodBody body;
        private readonly Dictionary<int, InstructionBlock> blocks = new Dictionary<int, InstructionBlock>();
        private readonly HashSet<int> exceptionObjectsOffsets;
        private readonly Dictionary<InstructionBlock, SwitchData> switchBlocksInformation = new Dictionary<InstructionBlock, SwitchData>();
        private Dictionary<int, Instruction> offsetToInstruction;

        internal ControlFlowGraphBuilder(MethodDefinition method)
        {
            body = method.Body;

            if (body.ExceptionHandlers.Count > 0)
            {
                exceptionObjectsOffsets = new HashSet<int>();
            }
        }

        public ControlFlowGraph CreateGraph()
        {
            FillOffsetToInstruction();
            DelimitBlocks();
            ConnectBlocks();

            return new ControlFlowGraph(body, ToArray(), blocks, switchBlocksInformation, offsetToInstruction);
        }

        private void FillOffsetToInstruction()
        {
            this.offsetToInstruction = new Dictionary<int, Instruction>();
            foreach (Instruction instruction in body.Instructions)
            {
                offsetToInstruction.Add(instruction.Offset, instruction);
            }
        }

        private void DelimitBlocks()
        {
            Collection<Instruction> instructions = body.Instructions;
            MarkBlockStarts(instructions);

            Collection<ExceptionHandler> exceptions = body.ExceptionHandlers;
            MarkBlockStarts(exceptions);

            MarkBlockEnds(instructions);
        }

        private void MarkBlockStarts(Collection<ExceptionHandler> handlers)
        {
            for (int i = 0; i < handlers.Count; i++)
            {
                ExceptionHandler handler = handlers[i];
                MarkBlockStart(handler.TryStart);
                MarkBlockStart(handler.HandlerStart);

                if (handler.HandlerType == ExceptionHandlerType.Filter)
                {
                    MarkExceptionObjectPosition(handler.FilterStart);
                    MarkBlockStart(handler.FilterStart);
                }
                else if (handler.HandlerType == ExceptionHandlerType.Catch)
                {
                    MarkExceptionObjectPosition(handler.HandlerStart);
                }
            }
        }

        private void MarkExceptionObjectPosition(Instruction instruction)
        {
            exceptionObjectsOffsets.Add(instruction.Offset);
        }

        private void MarkBlockStarts(Collection<Instruction> instructions)
        {
            // the first instruction starts a block
            for (int i = 0; i < instructions.Count; ++i)
            {
                Instruction instruction = instructions[i];

                if (i == 0)
                {
                    MarkBlockStart(instruction);
                }

                if (!IsBlockDelimiter(instruction))
                {
                    continue;
                }

                if (HasMultipleBranches(instruction))
                // each switch case first instruction starts a block
                {
                    // each switch case first instruction starts a block
                    foreach (Instruction target in GetBranchTargets(instruction))
                    {
                        if (target != null)
                        {
                            MarkBlockStart(target);
                        }
                    }
                }
                else
                {
                    // the target of a branch starts a block
                    Instruction target = GetBranchTarget(instruction);
                    if (target != null)
                    {
                        MarkBlockStart(target);
                    }
                }

                // the next instruction after a branch starts a block
                if (instruction.Next != null)
                {
                    MarkBlockStart(instruction.Next);
                }
            }
        }

        private void MarkBlockEnds(Collection<Instruction> instructions)
        {
            InstructionBlock[] blocks = ToArray();
            if (blocks.Length <= 0)
            {
                return;
            }

            InstructionBlock current = blocks[0];

            for (int i = 1; i < blocks.Length; ++i)
            {
                var block = blocks[i];
                current.Last = block.First.Previous;
                current = block;
            }

            current.Last = instructions[instructions.Count - 1];
        }

        private static bool IsBlockDelimiter(Instruction instruction)
        {
            switch (instruction.OpCode.FlowControl)
            {
                case FlowControl.Break:
                case FlowControl.Branch:
                case FlowControl.Return:
                case FlowControl.Cond_Branch:
                case FlowControl.Throw:
                    return true;
            }
            return false;
        }

        private void MarkBlockStart(Instruction instruction)
        {
            InstructionBlock block = GetBlock(instruction);
            if (block != null)
            {
                return;
            }

            block = new InstructionBlock(instruction);
            RegisterBlock(block);
        }

        private InstructionBlock[] ToArray()
        {
            try
            {
                InstructionBlock[] result = new InstructionBlock[blocks.Count];
                blocks.Values.CopyTo(result, 0);
                Array.Sort(result);
                ComputeIndexes(result);
                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidProgramException(ex.Message);
            }
        }

        private static void ComputeIndexes(InstructionBlock[] blocks)
        {
            for (int i = 0; i < blocks.Length; i++)
            {
                blocks[i].Index = i;
            }
        }

        private void ConnectBlocks()
        {
            foreach (InstructionBlock block in blocks.Values)
            {
                ConnectBlock(block);
            }
        }

        private void ConnectBlock(InstructionBlock block)
        {
            if (block.Last == null)
            {
                throw new ArgumentException("Undelimited block at offset " + block.First.Offset);
            }

            Instruction instruction = block.Last;
            switch (instruction.OpCode.FlowControl)
            {
                case FlowControl.Branch:
                case FlowControl.Cond_Branch:
                    {
                        if (HasMultipleBranches(instruction))
                        {

                            InstructionBlock[] blocks = GetBranchTargetsBlocks(instruction);
                            InstructionBlock defaultExit = null;
                            if (instruction.Next != null)
                            {
                                defaultExit = GetBlock(instruction.Next);
                            }

                            switchBlocksInformation.Add(block, new SwitchData(block, defaultExit, blocks));

                            if (defaultExit != null)
                            {
                                blocks = AddBlock(defaultExit, blocks);
                            }

                            block.Successors = blocks;
                            break;
                        }

                        InstructionBlock target = GetBranchTargetBlock(instruction);
                        if (instruction.OpCode.FlowControl == FlowControl.Cond_Branch && instruction.Next != null)
                        {
                            block.Successors = new[] { target, GetBlock(instruction.Next) };
                        }
                        else
                        {
                            block.Successors = new[] { target };
                        }
                        break;
                    }
                case FlowControl.Call:
                case FlowControl.Next:
                    if (null != instruction.Next)
                    {
                        block.Successors = new[] { GetBlock(instruction.Next) };
                    }
                    break;
                case FlowControl.Return:
                case FlowControl.Throw:
                    break;
                default:
                    throw new NotSupportedException(
                        string.Format("Unhandled instruction flow behavior {0}: {1}",
                                       instruction.OpCode.FlowControl,
                                       Formatter.FormatInstruction(instruction)));
            }
        }

        private static InstructionBlock[] AddBlock(InstructionBlock block, InstructionBlock[] blocks)
        {
            InstructionBlock[] result = new InstructionBlock[blocks.Length + 1];
            Array.Copy(blocks, result, blocks.Length);
            result[result.Length - 1] = block;

            return result;
        }

        private static bool HasMultipleBranches(Instruction instruction)
        {
            return instruction.OpCode.Code == Code.Switch;
        }

        private InstructionBlock[] GetBranchTargetsBlocks(Instruction instruction)
        {
            Instruction[] targets = GetBranchTargets(instruction);
            InstructionBlock[] blocks = new InstructionBlock[targets.Length];
            for (int i = 0; i < targets.Length; i++)
            {
                blocks[i] = GetBlock(targets[i]);
            }

            return blocks;
        }

        private static Instruction[] GetBranchTargets(Instruction instruction)
        {
            return (Instruction[])instruction.Operand;
        }

        private InstructionBlock GetBranchTargetBlock(Instruction instruction)
        {
            return GetBlock(GetBranchTarget(instruction));
        }

        private static Instruction GetBranchTarget(Instruction instruction)
        {
            return (Instruction)instruction.Operand;
        }

        private void RegisterBlock(InstructionBlock block)
        {
            blocks.Add(block.First.Offset, block);
        }

        private InstructionBlock GetBlock(Instruction firstInstruction)
        {
            InstructionBlock block;
            blocks.TryGetValue(firstInstruction.Offset, out block);
            return block;
        }
    }
}
