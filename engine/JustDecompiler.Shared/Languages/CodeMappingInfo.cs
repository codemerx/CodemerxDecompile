using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Languages
{
    public class CodeMappingInfo<T>
    {
        public CodeMappingInfo()
        {
            this.NodeToCodeMap = new Dictionary<ICodeNode, T>();
            this.InstructionToCodeMap = new Dictionary<Instruction, T>(new InstructionEqualityComparer());
            this.FieldConstantValueToCodeMap = new Dictionary<IMemberDefinition, T>();
            this.VariableToCodeMap = new Dictionary<VariableDefinition, T>(new VariableDefinitionEqualityComparer());
            this.ParameterToCodeMap = new Dictionary<IMemberDefinition, Dictionary<int, T>>();
        }

        public Dictionary<ICodeNode, T> NodeToCodeMap { get; private set; }
        public Dictionary<Instruction, T> InstructionToCodeMap { get; private set; }
        public Dictionary<IMemberDefinition, T> FieldConstantValueToCodeMap { get; private set; }
        public Dictionary<VariableDefinition, T> VariableToCodeMap { get; private set; }
        public Dictionary<IMemberDefinition, Dictionary<int, T>> ParameterToCodeMap { get; private set; }

        public T this[ICodeNode node]
        {
            get
            {
                return this.NodeToCodeMap[node];
            }
        }

        public T this[Instruction instruction]
        {
            get
            {
                return this.InstructionToCodeMap[instruction];
            }
        }

        public void Add(ICodeNode node, T span)
        {
            this.NodeToCodeMap.Add(node, span);
        }

        public void Add(Instruction instruction, T span)
        {
            this.InstructionToCodeMap.Add(instruction, span);
        }

        public void Add(FieldDefinition field, T span)
        {
            this.FieldConstantValueToCodeMap.Add(field, span);
        }

        public void Add(VariableDefinition variable, T span)
        {
            this.VariableToCodeMap.Add(variable, span);
        }

        public void Add(IMemberDefinition member, int index, T span)
        {
            if (!this.ParameterToCodeMap.ContainsKey(member))
            {
                this.ParameterToCodeMap.Add(member, new Dictionary<int, T>());
            }

            this.ParameterToCodeMap[member].Add(index, span);
        }

        public bool ContainsKey(ICodeNode node)
        {
            return this.NodeToCodeMap.ContainsKey(node);
        }

        public bool ContainsKey(Instruction instruction)
        {
            return this.InstructionToCodeMap.ContainsKey(instruction);
        }
        
        public bool TryGetValue(Instruction instruction, out T span)
        {
            return this.InstructionToCodeMap.TryGetValue(instruction, out span);
        }

        public bool TryGetValue(IMemberDefinition field, out T span)
        {
            return this.FieldConstantValueToCodeMap.TryGetValue(field, out span);
        }

        public bool TryGetValue(VariableDefinition variable, out T span)
        {
            return this.VariableToCodeMap.TryGetValue(variable, out span);
        }

        public bool TryGetValue(IMemberDefinition member, int parameterIndex, out T span)
        {
            span = default;

            Dictionary<int, T> indexToCodeMap;
            if (this.ParameterToCodeMap.TryGetValue(member, out indexToCodeMap))
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
