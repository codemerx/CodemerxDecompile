using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Globalization;
using System.IO;

namespace Telerik.JustDecompiler.Cil
{
	public static class Formatter
	{
		public static string FormatInstruction(Instruction instruction)
		{
			StringWriter stringWriter = new StringWriter();
			Formatter.WriteInstruction(stringWriter, instruction);
			return stringWriter.ToString();
		}

		private static string FormatLabel(int offset)
		{
			string str = String.Concat("000", offset.ToString("x"));
			return String.Concat("IL_", str.Substring(str.Length - 4));
		}

		public static string FormatTypeReference(TypeReference type)
		{
			return Formatter.FormatTypeReference(type.get_FullName());
		}

		public static string FormatTypeReference(string typeName)
		{
			if (typeName != null)
			{
				if (typeName == "System.Void")
				{
					return "void";
				}
				if (typeName == "System.String")
				{
					return "string";
				}
				if (typeName == "System.Int32")
				{
					return "int32";
				}
				if (typeName == "System.Long")
				{
					return "int64";
				}
				if (typeName == "System.Boolean")
				{
					return "bool";
				}
				if (typeName == "System.Single")
				{
					return "float32";
				}
				if (typeName == "System.Double")
				{
					return "float64";
				}
			}
			return typeName;
		}

		public static string ToInvariantCultureString(object value)
		{
			IConvertible convertible = value as IConvertible;
			if (convertible == null)
			{
				return value.ToString();
			}
			return convertible.ToString(CultureInfo.InvariantCulture);
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
		}

		private static void WriteLabelList(TextWriter writer, Instruction[] instructions)
		{
			writer.Write("(");
			for (int i = 0; i < (int)instructions.Length; i++)
			{
				if (i != 0)
				{
					writer.Write(", ");
				}
				writer.Write(Formatter.FormatLabel(instructions[i].get_Offset()));
			}
			writer.Write(")");
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
			Collection<ParameterDefinition> parameters = method.get_Parameters();
			for (int i = 0; i < parameters.get_Count(); i++)
			{
				if (i > 0)
				{
					writer.Write(", ");
				}
				writer.Write(Formatter.FormatTypeReference(parameters.get_Item(i).get_ParameterType()));
			}
			writer.Write(")");
		}

		private static void WriteOperand(TextWriter writer, object operand)
		{
			if (operand == null)
			{
				throw new ArgumentNullException("operand");
			}
			Instruction instruction = operand as Instruction;
			if (instruction != null)
			{
				writer.Write(Formatter.FormatLabel(instruction.get_Offset()));
				return;
			}
			Instruction[] instructionArray = operand as Instruction[];
			if (instructionArray != null)
			{
				Formatter.WriteLabelList(writer, instructionArray);
				return;
			}
			VariableReference variableReference = operand as VariableReference;
			if (variableReference != null)
			{
				writer.Write(variableReference.get_Index().ToString());
				return;
			}
			MethodReference methodReference = operand as MethodReference;
			if (methodReference != null)
			{
				Formatter.WriteMethodReference(writer, methodReference);
				return;
			}
			string invariantCultureString = operand as String;
			if (invariantCultureString != null)
			{
				writer.Write(String.Concat("\"", invariantCultureString, "\""));
				return;
			}
			invariantCultureString = Formatter.ToInvariantCultureString(operand);
			writer.Write(invariantCultureString);
		}
	}
}