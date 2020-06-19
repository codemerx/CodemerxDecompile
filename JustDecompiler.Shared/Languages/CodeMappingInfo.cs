using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Languages
{
    public class CodeMappingInfo
    {
        private Dictionary<ICodeNode, OffsetSpan> nodeToCodeMap;
        private Dictionary<IMemberDefinition, OffsetSpan> fieldConstantValueToCodeMap;
        private Dictionary<VariableDefinition, OffsetSpan> variableToCodeMap;
        private Dictionary<IMemberDefinition, Dictionary<int, OffsetSpan>> parameterToCodeMap;

        public CodeMappingInfo()
        {
            this.nodeToCodeMap = new Dictionary<ICodeNode, OffsetSpan>();
            this.InstructionToCodeMap = new Dictionary<Instruction, OffsetSpan>(new InstructionEqualityComparer());
            this.fieldConstantValueToCodeMap = new Dictionary<IMemberDefinition, OffsetSpan>();
            this.variableToCodeMap = new Dictionary<VariableDefinition, OffsetSpan>(new VariableDefinitionEqualityComparer());
            this.parameterToCodeMap = new Dictionary<IMemberDefinition, Dictionary<int, OffsetSpan>>();
        }

        // Exposed as internal for testing purposes
        internal Dictionary<Instruction, OffsetSpan> InstructionToCodeMap { get; private set; }

        public OffsetSpan this[ICodeNode node]
        {
            get
            {
                return this.nodeToCodeMap[node];
            }
        }

        public OffsetSpan this[Instruction instruction]
        {
            get
            {
                return this.InstructionToCodeMap[instruction];
            }
        }

        public void Add(ICodeNode node, OffsetSpan span)
        {
            this.nodeToCodeMap.Add(node, span);
        }

        public void Add(Instruction instruction, OffsetSpan span)
        {
            this.InstructionToCodeMap.Add(instruction, span);
        }

        public void Add(FieldDefinition field, OffsetSpan span)
        {
            this.fieldConstantValueToCodeMap.Add(field, span);
        }

        public void Add(VariableDefinition variable, OffsetSpan span)
        {
            this.variableToCodeMap.Add(variable, span);
        }

        public void Add(IMemberDefinition member, int index, OffsetSpan span)
        {
            if (!this.parameterToCodeMap.ContainsKey(member))
            {
                this.parameterToCodeMap.Add(member, new Dictionary<int, OffsetSpan>());
            }

            this.parameterToCodeMap[member].Add(index, span);
        }

        public bool ContainsKey(ICodeNode node)
        {
            return this.nodeToCodeMap.ContainsKey(node);
        }

        public bool ContainsKey(Instruction instruction)
        {
            return this.InstructionToCodeMap.ContainsKey(instruction);
        }
        
        public bool TryGetValue(Instruction instruction, out OffsetSpan span)
        {
            return this.InstructionToCodeMap.TryGetValue(instruction, out span);
        }

        public bool TryGetValue(IMemberDefinition field, out OffsetSpan span)
        {
            return this.fieldConstantValueToCodeMap.TryGetValue(field, out span);
        }

        public bool TryGetValue(VariableDefinition variable, out OffsetSpan span)
        {
            return this.variableToCodeMap.TryGetValue(variable, out span);
        }

        public bool TryGetValue(IMemberDefinition member, int parameterIndex, out OffsetSpan span)
        {
            span = default(OffsetSpan);

            Dictionary<int, OffsetSpan> indexToCodeMap;
            if (this.parameterToCodeMap.TryGetValue(member, out indexToCodeMap))
            {
                return indexToCodeMap.TryGetValue(parameterIndex, out span);
            }

            return false;
        }

        private class InstructionEqualityComparer : IEqualityComparer<Instruction>
        {
            public bool Equals(Instruction x, Instruction y)
            {
                return x.Offset == y.Offset &&
                       x.ContainingMethod.FullName == y.ContainingMethod.FullName &&
                       x.ContainingMethod.GenericParameters.Count == y.ContainingMethod.GenericParameters.Count;
            }

            public int GetHashCode(Instruction obj)
            {
                return (obj.Offset.ToString() +
                        obj.ContainingMethod.FullName +
                        obj.ContainingMethod.GenericParameters.Count.ToString()).GetHashCode();
            }
        }

        /// <summary>
        /// This class is used to enable searching for variables that are Mono.Cecil generated, i.e. taken from the assembly.
        /// It's needed, because the VariableDefinitions that the FullTextSearcher uses are diferent objects, than the one
        /// used by the writers, and when the highlighting tries to find the variable that needs to be highlighted, it does
        /// not find it. If it's from the assembly, it will always have index. In this case we use that index and the method
        /// in which the variable is defined to compare them. If not we do reference equality. This is kind a hack and needs
        /// to be removed once bug 321107 (in TeamPulse) is fixed.
        /// </summary>
        private class VariableDefinitionEqualityComparer : IEqualityComparer<VariableDefinition>
        {
            public bool Equals(VariableDefinition x, VariableDefinition y)
            {
                bool isXFromAssembly = x.Index != -1;
                bool isYFromAssembly = y.Index != -1;
                if (isXFromAssembly != isYFromAssembly)
                {
                    return false;
                }

                if (isXFromAssembly)
                {
                    return x.Index == y.Index &&
                           x.ContainingMethod.FullName == y.ContainingMethod.FullName &&
                           x.ContainingMethod.GenericParameters.Count == y.ContainingMethod.GenericParameters.Count;
                }
                else
                {
                    return x.Equals(y);
                }
            }

            public int GetHashCode(VariableDefinition obj)
            {
                bool isFromAssembly = obj.Index != -1;
                if (isFromAssembly)
                {
                    return (obj.Index.ToString() +
                            obj.ContainingMethod.FullName +
                            obj.ContainingMethod.GenericParameters.Count.ToString()).GetHashCode();
                }
                else
                {
                    return obj.GetHashCode();
                }
            }
        }
    }
}
