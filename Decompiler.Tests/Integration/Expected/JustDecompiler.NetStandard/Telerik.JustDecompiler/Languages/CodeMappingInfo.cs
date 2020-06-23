using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Languages
{
	public class CodeMappingInfo
	{
		private Dictionary<ICodeNode, OffsetSpan> nodeToCodeMap;

		private Dictionary<IMemberDefinition, OffsetSpan> fieldConstantValueToCodeMap;

		private Dictionary<VariableDefinition, OffsetSpan> variableToCodeMap;

		private Dictionary<IMemberDefinition, Dictionary<int, OffsetSpan>> parameterToCodeMap;

		internal Dictionary<Instruction, OffsetSpan> InstructionToCodeMap
		{
			get;
			private set;
		}

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

		public CodeMappingInfo()
		{
			this.nodeToCodeMap = new Dictionary<ICodeNode, OffsetSpan>();
			this.InstructionToCodeMap = new Dictionary<Instruction, OffsetSpan>(new CodeMappingInfo.InstructionEqualityComparer());
			this.fieldConstantValueToCodeMap = new Dictionary<IMemberDefinition, OffsetSpan>();
			this.variableToCodeMap = new Dictionary<VariableDefinition, OffsetSpan>(new CodeMappingInfo.VariableDefinitionEqualityComparer());
			this.parameterToCodeMap = new Dictionary<IMemberDefinition, Dictionary<int, OffsetSpan>>();
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
			Dictionary<int, OffsetSpan> nums;
			span = new OffsetSpan();
			if (!this.parameterToCodeMap.TryGetValue(member, out nums))
			{
				return false;
			}
			return nums.TryGetValue(parameterIndex, out span);
		}

		private class InstructionEqualityComparer : IEqualityComparer<Instruction>
		{
			public InstructionEqualityComparer()
			{
			}

			public bool Equals(Instruction x, Instruction y)
			{
				if (x.Offset != y.Offset || !(x.ContainingMethod.FullName == y.ContainingMethod.FullName))
				{
					return false;
				}
				return x.ContainingMethod.GenericParameters.Count == y.ContainingMethod.GenericParameters.Count;
			}

			public int GetHashCode(Instruction obj)
			{
				string str = obj.Offset.ToString();
				string fullName = obj.ContainingMethod.FullName;
				int count = obj.ContainingMethod.GenericParameters.Count;
				return String.Concat(str, fullName, count.ToString()).GetHashCode();
			}
		}

		private class VariableDefinitionEqualityComparer : IEqualityComparer<VariableDefinition>
		{
			public VariableDefinitionEqualityComparer()
			{
			}

			public bool Equals(VariableDefinition x, VariableDefinition y)
			{
				bool index = x.Index != -1;
				if (index != y.Index != -1)
				{
					return false;
				}
				if (!index)
				{
					return x.Equals(y);
				}
				if (x.Index != y.Index || !(x.ContainingMethod.FullName == y.ContainingMethod.FullName))
				{
					return false;
				}
				return x.ContainingMethod.GenericParameters.Count == y.ContainingMethod.GenericParameters.Count;
			}

			public int GetHashCode(VariableDefinition obj)
			{
				if (obj.Index == -1)
				{
					return obj.GetHashCode();
				}
				string str = obj.Index.ToString();
				string fullName = obj.ContainingMethod.FullName;
				int count = obj.ContainingMethod.GenericParameters.Count;
				return String.Concat(str, fullName, count.ToString()).GetHashCode();
			}
		}
	}
}