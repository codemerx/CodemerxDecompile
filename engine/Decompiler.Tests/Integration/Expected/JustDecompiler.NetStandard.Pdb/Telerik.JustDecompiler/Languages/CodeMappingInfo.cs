using Mono.Cecil;
using Mono.Cecil.Cil;
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
				return this.nodeToCodeMap.get_Item(node);
			}
		}

		public OffsetSpan this[Instruction instruction]
		{
			get
			{
				return this.get_InstructionToCodeMap().get_Item(instruction);
			}
		}

		public CodeMappingInfo()
		{
			base();
			this.nodeToCodeMap = new Dictionary<ICodeNode, OffsetSpan>();
			this.set_InstructionToCodeMap(new Dictionary<Instruction, OffsetSpan>(new CodeMappingInfo.InstructionEqualityComparer()));
			this.fieldConstantValueToCodeMap = new Dictionary<IMemberDefinition, OffsetSpan>();
			this.variableToCodeMap = new Dictionary<VariableDefinition, OffsetSpan>(new CodeMappingInfo.VariableDefinitionEqualityComparer());
			this.parameterToCodeMap = new Dictionary<IMemberDefinition, Dictionary<int, OffsetSpan>>();
			return;
		}

		public void Add(ICodeNode node, OffsetSpan span)
		{
			this.nodeToCodeMap.Add(node, span);
			return;
		}

		public void Add(Instruction instruction, OffsetSpan span)
		{
			this.get_InstructionToCodeMap().Add(instruction, span);
			return;
		}

		public void Add(FieldDefinition field, OffsetSpan span)
		{
			this.fieldConstantValueToCodeMap.Add(field, span);
			return;
		}

		public void Add(VariableDefinition variable, OffsetSpan span)
		{
			this.variableToCodeMap.Add(variable, span);
			return;
		}

		public void Add(IMemberDefinition member, int index, OffsetSpan span)
		{
			if (!this.parameterToCodeMap.ContainsKey(member))
			{
				this.parameterToCodeMap.Add(member, new Dictionary<int, OffsetSpan>());
			}
			this.parameterToCodeMap.get_Item(member).Add(index, span);
			return;
		}

		public bool ContainsKey(ICodeNode node)
		{
			return this.nodeToCodeMap.ContainsKey(node);
		}

		public bool ContainsKey(Instruction instruction)
		{
			return this.get_InstructionToCodeMap().ContainsKey(instruction);
		}

		public bool TryGetValue(Instruction instruction, out OffsetSpan span)
		{
			return this.get_InstructionToCodeMap().TryGetValue(instruction, out span);
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
			span = new OffsetSpan();
			if (!this.parameterToCodeMap.TryGetValue(member, out V_0))
			{
				return false;
			}
			return V_0.TryGetValue(parameterIndex, out span);
		}

		private class InstructionEqualityComparer : IEqualityComparer<Instruction>
		{
			public InstructionEqualityComparer()
			{
				base();
				return;
			}

			public bool Equals(Instruction x, Instruction y)
			{
				if (x.get_Offset() != y.get_Offset() || !String.op_Equality(x.get_ContainingMethod().get_FullName(), y.get_ContainingMethod().get_FullName()))
				{
					return false;
				}
				return x.get_ContainingMethod().get_GenericParameters().get_Count() == y.get_ContainingMethod().get_GenericParameters().get_Count();
			}

			public int GetHashCode(Instruction obj)
			{
				stackVariable3 = obj.get_Offset().ToString();
				stackVariable6 = obj.get_ContainingMethod().get_FullName();
				V_0 = obj.get_ContainingMethod().get_GenericParameters().get_Count();
				return String.Concat(stackVariable3, stackVariable6, V_0.ToString()).GetHashCode();
			}
		}

		private class VariableDefinitionEqualityComparer : IEqualityComparer<VariableDefinition>
		{
			public VariableDefinitionEqualityComparer()
			{
				base();
				return;
			}

			public bool Equals(VariableDefinition x, VariableDefinition y)
			{
				V_0 = x.get_Index() != -1;
				if (V_0 != y.get_Index() != -1)
				{
					return false;
				}
				if (!V_0)
				{
					return x.Equals(y);
				}
				if (x.get_Index() != y.get_Index() || !String.op_Equality(x.get_ContainingMethod().get_FullName(), y.get_ContainingMethod().get_FullName()))
				{
					return false;
				}
				return x.get_ContainingMethod().get_GenericParameters().get_Count() == y.get_ContainingMethod().get_GenericParameters().get_Count();
			}

			public int GetHashCode(VariableDefinition obj)
			{
				if (obj.get_Index() == -1)
				{
					return obj.GetHashCode();
				}
				stackVariable11 = obj.get_Index().ToString();
				stackVariable14 = obj.get_ContainingMethod().get_FullName();
				V_0 = obj.get_ContainingMethod().get_GenericParameters().get_Count();
				return String.Concat(stackVariable11, stackVariable14, V_0.ToString()).GetHashCode();
			}
		}
	}
}