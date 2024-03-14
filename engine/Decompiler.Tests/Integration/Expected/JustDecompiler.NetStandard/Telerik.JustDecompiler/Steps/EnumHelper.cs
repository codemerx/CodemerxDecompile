using Mono.Cecil;
using Mono.Cecil.Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Steps
{
	public static class EnumHelper
	{
		private static int GetEnumBitSize(TypeDefinition enumDefinition)
		{
			TypeReference fieldType = enumDefinition.get_Fields().get_Item(0).get_FieldType();
			if (fieldType.get_FullName() == "System.Int32" || fieldType.get_FullName() == "System.UInt32")
			{
				return 32;
			}
			if (fieldType.get_FullName() == "System.Int64" || fieldType.get_FullName() == "System.UInt64")
			{
				return 64;
			}
			if (fieldType.get_FullName() == "System.Int16" || fieldType.get_FullName() == "System.UInt16")
			{
				return 16;
			}
			if (!(fieldType.get_FullName() == "System.Byte") && !(fieldType.get_FullName() == "System.SByte"))
			{
				return -1;
			}
			return 8;
		}

		public static Expression GetEnumExpression(TypeDefinition enumDefinition, LiteralExpression targetedValue, TypeSystem typeSystem)
		{
			Expression enumExpression;
			int enumBitSize = EnumHelper.GetEnumBitSize(enumDefinition);
			ulong value = (ulong)0;
			string fullName = targetedValue.Value.GetType().FullName;
			if (fullName != null)
			{
				switch (fullName)
				{
					case "System.Int32":
					{
						if (enumBitSize != 32)
						{
							value = (ulong)((Int32)targetedValue.Value);
						}
						else
						{
							value = (ulong)((Int32)targetedValue.Value);
						}
						break;
					}
					case "System.Int64":
					{
						value = (ulong)targetedValue.Value;
						break;
					}
					case "System.UInt32":
					{
						value = (ulong)((UInt32)targetedValue.Value);
						break;
					}
					case "System.UInt64":
					{
						value = (UInt64)targetedValue.Value;
						break;
					}
					case "System.Byte":
					{
						value = (ulong)((Byte)targetedValue.Value);
						break;
					}
					case "System.SByte":
					{
						value = (ulong)((SByte)targetedValue.Value);
						break;
					}
					case "System.Int16":
					{
						value = (ulong)((Int16)targetedValue.Value);
						break;
					}
					case "System.UInt16":
					{
						value = (ulong)((UInt16)targetedValue.Value);
						break;
					}
				}
			}
			Collection<FieldDefinition> fields = enumDefinition.get_Fields();
			List<FieldDefinition> fieldDefinitions = new List<FieldDefinition>();
			Collection<FieldDefinition>.Enumerator enumerator = fields.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					FieldDefinition current = enumerator.get_Current();
					if (current.get_Constant() == null || current.get_Constant().get_Value() == null)
					{
						continue;
					}
					ulong num = (ulong)0;
					fullName = current.get_Constant().get_Value().GetType().FullName;
					if (fullName != null)
					{
						switch (fullName)
						{
							case "System.Int32":
							{
								num = (ulong)((Int32)current.get_Constant().get_Value());
								break;
							}
							case "System.UInt32":
							{
								num = (ulong)((UInt32)current.get_Constant().get_Value());
								break;
							}
							case "System.Byte":
							{
								num = (ulong)((Byte)current.get_Constant().get_Value());
								break;
							}
							case "System.SByte":
							{
								num = (ulong)((byte)((SByte)current.get_Constant().get_Value()));
								break;
							}
							case "System.Int16":
							{
								num = (ulong)((ushort)((Int16)current.get_Constant().get_Value()));
								break;
							}
							case "System.UInt16":
							{
								num = (ulong)((UInt16)current.get_Constant().get_Value());
								break;
							}
							case "System.Int64":
							{
								num = (ulong)current.get_Constant().get_Value();
								break;
							}
							case "System.UInt64":
							{
								num = (UInt64)current.get_Constant().get_Value();
								break;
							}
						}
					}
					if (num != value)
					{
						if (num == 0 || (num | value) != value)
						{
							continue;
						}
						fieldDefinitions.Add(current);
					}
					else
					{
						enumExpression = new EnumExpression(current, targetedValue.UnderlyingSameMethodInstructions);
						return enumExpression;
					}
				}
				if (fieldDefinitions.Count < 2)
				{
					return targetedValue;
				}
				Expression binaryExpression = new BinaryExpression(BinaryOperator.BitwiseOr, new EnumExpression(fieldDefinitions[0], null), new EnumExpression(fieldDefinitions[1], null), typeSystem, null, false)
				{
					ExpressionType = enumDefinition
				};
				for (int i = 2; i < fieldDefinitions.Count; i++)
				{
					binaryExpression = new BinaryExpression(BinaryOperator.BitwiseOr, binaryExpression, new EnumExpression(fieldDefinitions[i], null), typeSystem, null, false)
					{
						ExpressionType = enumDefinition
					};
				}
				return binaryExpression.CloneAndAttachInstructions(targetedValue.UnderlyingSameMethodInstructions);
			}
			finally
			{
				enumerator.Dispose();
			}
			return enumExpression;
		}
	}
}