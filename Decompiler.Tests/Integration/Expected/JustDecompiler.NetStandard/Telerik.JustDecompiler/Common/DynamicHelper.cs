using Mono.Cecil;
using System;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Common
{
	internal static class DynamicHelper
	{
		public static BinaryOperator GetBinaryOperator(ExpressionType operator)
		{
			switch (operator)
			{
				case 0:
				case 1:
				{
				Label1:
					return 1;
				}
				case 2:
				{
				Label2:
					return 22;
				}
				case 3:
				{
					return 12;
				}
				case 4:
				case 5:
				case 6:
				case 8:
				case 9:
				case 10:
				case 11:
				case 17:
				case 18:
				case 22:
				case 23:
				case 24:
				{
				Label0:
					throw new Exception("Operator is not supported.");
				}
				case 7:
				{
					return 27;
				}
				case 12:
				{
				Label3:
					return 7;
				}
				case 13:
				{
					return 9;
				}
				case 14:
				{
				Label4:
					return 23;
				}
				case 15:
				{
					return 15;
				}
				case 16:
				{
					return 16;
				}
				case 19:
				{
				Label5:
					return 17;
				}
				case 20:
				{
					return 13;
				}
				case 21:
				{
					return 14;
				}
				case 25:
				{
				Label6:
					return 24;
				}
				case 26:
				case 27:
				{
				Label7:
					return 5;
				}
				default:
				{
					switch (operator - 35)
					{
						case 0:
						{
							return 10;
						}
						case 1:
						{
						Label8:
							return 21;
						}
						case 2:
						{
							return 11;
						}
						case 3:
						case 4:
						case 5:
						case 9:
						case 10:
						{
							goto Label0;
						}
						case 6:
						{
						Label9:
							return 19;
						}
						case 7:
						case 8:
						{
						Label10:
							return 3;
						}
						case 11:
						{
							return 26;
						}
						default:
						{
							switch (operator - 63)
							{
								case 0:
								case 11:
								{
									goto Label1;
								}
								case 1:
								{
									goto Label2;
								}
								case 2:
								{
									goto Label3;
								}
								case 3:
								{
									goto Label4;
								}
								case 4:
								{
									goto Label5;
								}
								case 5:
								{
									goto Label6;
								}
								case 6:
								case 12:
								{
									goto Label7;
								}
								case 7:
								{
									goto Label8;
								}
								case 8:
								{
									goto Label0;
								}
								case 9:
								{
									goto Label9;
								}
								case 10:
								case 13:
								{
									goto Label10;
								}
								default:
								{
									goto Label0;
								}
							}
							break;
						}
					}
					break;
				}
			}
		}

		public static bool[] GetDynamicPositioningFlags(CustomAttribute dynamicAttribute)
		{
			dynamicAttribute.Resolve();
			if (!dynamicAttribute.get_IsResolved())
			{
				throw new Exception("Could not resolve DynamicAttribute");
			}
			if (dynamicAttribute.get_ConstructorArguments().get_Count() == 0)
			{
				stackVariable45 = new Boolean[1];
				stackVariable45[0] = true;
				return stackVariable45;
			}
			if (String.op_Inequality(dynamicAttribute.get_ConstructorArguments().get_Item(0).get_Type().get_FullName(), "System.Boolean[]"))
			{
				throw new Exception("Invalid argument type for DynamicAttribute");
			}
			V_2 = dynamicAttribute.get_ConstructorArguments().get_Item(0);
			V_0 = (CustomAttributeArgument[])V_2.get_Value();
			V_1 = new Boolean[(int)V_0.Length];
			V_3 = 0;
			while (V_3 < (int)V_0.Length)
			{
				V_1[V_3] = (Boolean)V_0[V_3].get_Value();
				V_3 = V_3 + 1;
			}
			return V_1;
		}

		public static UnaryOperator GetUnaryOperator(ExpressionType operator)
		{
			if (operator > 49)
			{
				if (operator != 54)
				{
					switch (operator - 77)
					{
						case 0:
						{
							break;
						}
						case 1:
						{
							goto Label0;
						}
						case 2:
						{
							return 4;
						}
						case 3:
						{
							return 3;
						}
						case 4:
						case 6:
						{
							goto Label2;
						}
						case 5:
						{
							return 2;
						}
						case 7:
						{
							goto Label1;
						}
						default:
						{
							goto Label2;
						}
					}
				}
				return 6;
			}
			else
			{
				switch (operator - 28)
				{
					case 0:
					case 2:
					{
						return 0;
					}
					case 1:
					{
						return 10;
					}
					case 3:
					case 4:
					case 5:
					{
						break;
					}
					case 6:
					{
						goto Label1;
					}
					default:
					{
						if (operator == 49)
						{
							goto Label0;
						}
						break;
					}
				}
			}
		Label2:
			throw new Exception("Operator is not supported.");
		Label0:
			return 5;
		Label1:
			return 1;
		}
	}
}