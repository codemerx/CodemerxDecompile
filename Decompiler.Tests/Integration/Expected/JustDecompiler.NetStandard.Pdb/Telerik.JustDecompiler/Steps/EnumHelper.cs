using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Steps
{
	public static class EnumHelper
	{
		private static int GetEnumBitSize(TypeDefinition enumDefinition)
		{
			V_0 = enumDefinition.get_Fields().get_Item(0).get_FieldType();
			if (String.op_Equality(V_0.get_FullName(), "System.Int32") || String.op_Equality(V_0.get_FullName(), "System.UInt32"))
			{
				return 32;
			}
			if (String.op_Equality(V_0.get_FullName(), "System.Int64") || String.op_Equality(V_0.get_FullName(), "System.UInt64"))
			{
				return 64;
			}
			if (String.op_Equality(V_0.get_FullName(), "System.Int16") || String.op_Equality(V_0.get_FullName(), "System.UInt16"))
			{
				return 16;
			}
			if (!String.op_Equality(V_0.get_FullName(), "System.Byte") && !String.op_Equality(V_0.get_FullName(), "System.SByte"))
			{
				return -1;
			}
			return 8;
		}

		public static Expression GetEnumExpression(TypeDefinition enumDefinition, LiteralExpression targetedValue, TypeSystem typeSystem)
		{
			V_0 = EnumHelper.GetEnumBitSize(enumDefinition);
			V_1 = (long)0;
			V_4 = targetedValue.get_Value().GetType().get_FullName();
			if (V_4 != null)
			{
				if (String.op_Equality(V_4, "System.Int32"))
				{
					if (V_0 != 32)
					{
						V_1 = (long)((Int32)targetedValue.get_Value());
					}
					else
					{
						V_1 = (ulong)((Int32)targetedValue.get_Value());
					}
				}
				else
				{
					if (String.op_Equality(V_4, "System.Int64"))
					{
						V_1 = (Int64)targetedValue.get_Value();
					}
					else
					{
						if (String.op_Equality(V_4, "System.UInt32"))
						{
							V_1 = (ulong)((UInt32)targetedValue.get_Value());
						}
						else
						{
							if (String.op_Equality(V_4, "System.UInt64"))
							{
								V_1 = (UInt64)targetedValue.get_Value();
							}
							else
							{
								if (String.op_Equality(V_4, "System.Byte"))
								{
									V_1 = (ulong)((Byte)targetedValue.get_Value());
								}
								else
								{
									if (String.op_Equality(V_4, "System.SByte"))
									{
										V_1 = (long)((SByte)targetedValue.get_Value());
									}
									else
									{
										if (String.op_Equality(V_4, "System.Int16"))
										{
											V_1 = (long)((Int16)targetedValue.get_Value());
										}
										else
										{
											if (String.op_Equality(V_4, "System.UInt16"))
											{
												V_1 = (ulong)((UInt16)targetedValue.get_Value());
											}
										}
									}
								}
							}
						}
					}
				}
			}
			stackVariable10 = enumDefinition.get_Fields();
			V_2 = new List<FieldDefinition>();
			V_6 = stackVariable10.GetEnumerator();
			try
			{
				while (V_6.MoveNext())
				{
					V_7 = V_6.get_Current();
					if (V_7.get_Constant() == null || V_7.get_Constant().get_Value() == null)
					{
						continue;
					}
					V_8 = (long)0;
					V_4 = V_7.get_Constant().get_Value().GetType().get_FullName();
					if (V_4 != null)
					{
						if (String.op_Equality(V_4, "System.Int32"))
						{
							V_8 = (ulong)((Int32)V_7.get_Constant().get_Value());
						}
						else
						{
							if (String.op_Equality(V_4, "System.UInt32"))
							{
								V_8 = (ulong)((UInt32)V_7.get_Constant().get_Value());
							}
							else
							{
								if (String.op_Equality(V_4, "System.Byte"))
								{
									V_8 = (ulong)((Byte)V_7.get_Constant().get_Value());
								}
								else
								{
									if (String.op_Equality(V_4, "System.SByte"))
									{
										V_8 = (ulong)((byte)((SByte)V_7.get_Constant().get_Value()));
									}
									else
									{
										if (String.op_Equality(V_4, "System.Int16"))
										{
											V_8 = (ulong)((ushort)((Int16)V_7.get_Constant().get_Value()));
										}
										else
										{
											if (String.op_Equality(V_4, "System.UInt16"))
											{
												V_8 = (ulong)((UInt16)V_7.get_Constant().get_Value());
											}
											else
											{
												if (String.op_Equality(V_4, "System.Int64"))
												{
													V_8 = (Int64)V_7.get_Constant().get_Value();
												}
												else
												{
													if (String.op_Equality(V_4, "System.UInt64"))
													{
														V_8 = (UInt64)V_7.get_Constant().get_Value();
													}
												}
											}
										}
									}
								}
							}
						}
					}
					if (V_8 != V_1)
					{
						if (V_8 == 0 || V_8 | V_1 != V_1)
						{
							continue;
						}
						V_2.Add(V_7);
					}
					else
					{
						V_9 = new EnumExpression(V_7, targetedValue.get_UnderlyingSameMethodInstructions());
						goto Label1;
					}
				}
				goto Label0;
			}
			finally
			{
				V_6.Dispose();
			}
		Label1:
			return V_9;
		Label0:
			if (V_2.get_Count() < 2)
			{
				return targetedValue;
			}
			V_3 = new BinaryExpression(21, new EnumExpression(V_2.get_Item(0), null), new EnumExpression(V_2.get_Item(1), null), typeSystem, null, false);
			V_3.set_ExpressionType(enumDefinition);
			V_10 = 2;
			while (V_10 < V_2.get_Count())
			{
				V_3 = new BinaryExpression(21, V_3, new EnumExpression(V_2.get_Item(V_10), null), typeSystem, null, false);
				V_3.set_ExpressionType(enumDefinition);
				V_10 = V_10 + 1;
			}
			return V_3.CloneAndAttachInstructions(targetedValue.get_UnderlyingSameMethodInstructions());
		}
	}
}