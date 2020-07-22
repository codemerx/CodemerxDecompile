using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.IO;

namespace Telerik.JustDecompiler.Cil
{
	public static class Formatter
	{
		public static string FormatInstruction(Instruction instruction)
		{
			stackVariable0 = new StringWriter();
			Formatter.WriteInstruction(stackVariable0, instruction);
			return stackVariable0.ToString();
		}

		private static string FormatLabel(int offset)
		{
			V_0 = String.Concat("000", offset.ToString("x"));
			return String.Concat("IL_", V_0.Substring(V_0.get_Length() - 4));
		}

		public static string FormatTypeReference(TypeReference type)
		{
			return Formatter.FormatTypeReference(type.get_FullName());
		}

		public static string FormatTypeReference(string typeName)
		{
			if (typeName != null)
			{
				if (String.op_Equality(typeName, "System.Void"))
				{
					return "void";
				}
				if (String.op_Equality(typeName, "System.String"))
				{
					return "string";
				}
				if (String.op_Equality(typeName, "System.Int32"))
				{
					return "int32";
				}
				if (String.op_Equality(typeName, "System.Long"))
				{
					return "int64";
				}
				if (String.op_Equality(typeName, "System.Boolean"))
				{
					return "bool";
				}
				if (String.op_Equality(typeName, "System.Single"))
				{
					return "float32";
				}
				if (String.op_Equality(typeName, "System.Double"))
				{
					return "float64";
				}
			}
			return typeName;
		}

		public static string ToInvariantCultureString(object value)
		{
			V_0 = value as IConvertible;
			if (V_0 == null)
			{
				return value.ToString();
			}
			return V_0.ToString(CultureInfo.get_InvariantCulture());
		}

		public static void WriteInstruction(TextWriter writer, Instruction instruction)
		{
			writer.Write(Formatter.FormatLabel(instruction.get_Offset()));
			writer.Write(": ");
			writer.Write(instruction.get_OpCode().get_Name());
			if (instruction.get_Operand() != null)
			{
				writer.Write(' ');
				Formatter.WriteOperand(writer, instruction.get_Operand());
			}
			return;
		}

		private static void WriteLabelList(TextWriter writer, Instruction[] instructions)
		{
			writer.Write("(");
			V_0 = 0;
			while (V_0 < (int)instructions.Length)
			{
				if (V_0 != 0)
				{
					writer.Write(", ");
				}
				writer.Write(Formatter.FormatLabel(instructions[V_0].get_Offset()));
				V_0 = V_0 + 1;
			}
			writer.Write(")");
			return;
		}

		private static void WriteMethodReference(TextWriter writer, MethodReference method)
		{
			writer.Write(Formatter.FormatTypeReference(method.get_FixedReturnType()));
			if (method.get_DeclaringType() != null)
			{
				writer.Write(' ');
				writer.Write(Formatter.FormatTypeReference(method.get_DeclaringType()));
			}
			writer.Write("::");
			writer.Write(method.get_Name());
			writer.Write("(");
			V_0 = method.get_Parameters();
			V_1 = 0;
			while (V_1 < V_0.get_Count())
			{
				if (V_1 > 0)
				{
					writer.Write(", ");
				}
				writer.Write(Formatter.FormatTypeReference(V_0.get_Item(V_1).get_ParameterType()));
				V_1 = V_1 + 1;
			}
			writer.Write(")");
			return;
		}

		private static void WriteOperand(TextWriter writer, object operand)
		{
			if (operand == null)
			{
				throw new ArgumentNullException("operand");
			}
			V_0 = operand as Instruction;
			if (V_0 != null)
			{
				writer.Write(Formatter.FormatLabel(V_0.get_Offset()));
				return;
			}
			V_1 = operand as Instruction[];
			if (V_1 != null)
			{
				Formatter.WriteLabelList(writer, V_1);
				return;
			}
			V_2 = operand as VariableReference;
			if (V_2 != null)
			{
				writer.Write(V_2.get_Index().ToString());
				return;
			}
			V_3 = operand as MethodReference;
			if (V_3 != null)
			{
				Formatter.WriteMethodReference(writer, V_3);
				return;
			}
			V_4 = operand as String;
			if (V_4 != null)
			{
				writer.Write(String.Concat("\"", V_4, "\""));
				return;
			}
			V_4 = Formatter.ToInvariantCultureString(operand);
			writer.Write(V_4);
			return;
		}
	}
}