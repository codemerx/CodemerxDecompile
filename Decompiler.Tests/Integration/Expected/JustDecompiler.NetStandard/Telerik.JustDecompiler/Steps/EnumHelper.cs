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
			TypeReference fieldType = enumDefinition.Fields[0].FieldType;
			if (fieldType.FullName == "System.Int32" || fieldType.FullName == "System.UInt32")
			{
				return 32;
			}
			if (fieldType.FullName == "System.Int64" || fieldType.FullName == "System.UInt64")
			{
				return 64;
			}
			if (fieldType.FullName == "System.Int16" || fieldType.FullName == "System.UInt16")
			{
				return 16;
			}
			if (!(fieldType.FullName == "System.Byte") && !(fieldType.FullName == "System.SByte"))
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
			Collection<FieldDefinition> fields = enumDefinition.Fields;
			List<FieldDefinition> fieldDefinitions = new List<FieldDefinition>();
			Collection<FieldDefinition>.Enumerator enumerator = fields.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					FieldDefinition current = enumerator.Current;
					if (current.Constant == null || current.Constant.Value == null)
					{
						continue;
					}
					ulong num = (ulong)0;
					fullName = current.Constant.Value.GetType().FullName;
					if (fullName != null)
					{
						switch (fullName)
						{
							case "System.Int32":
							{
								num = (ulong)((Int32)current.Constant.Value);
								break;
							}
							case "System.UInt32":
							{
								num = (ulong)((UInt32)current.Constant.Value);
								break;
							}
							case "System.Byte":
							{
								num = (ulong)((Byte)current.Constant.Value);
								break;
							}
							case "System.SByte":
							{
								num = (ulong)((byte)((SByte)current.Constant.Value));
								break;
							}
							case "System.Int16":
							{
								num = (ulong)((ushort)((Int16)current.Constant.Value));
								break;
							}
							case "System.UInt16":
							{
								num = (ulong)((UInt16)current.Constant.Value);
								break;
							}
							case "System.Int64":
							{
								num = (ulong)current.Constant.Value;
								break;
							}
							case "System.UInt64":
							{
								num = (UInt64)current.Constant.Value;
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
				((IDisposable)enumerator).Dispose();
			}
			return enumExpression;
		}
	}
}