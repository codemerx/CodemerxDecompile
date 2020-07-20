using Mono.Cecil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Steps
{
	internal class RemovePrivateImplementationDetailsStep
	{
		private readonly TypeSystem typeSystem;

		public RemovePrivateImplementationDetailsStep(TypeSystem typeSystem)
		{
			base();
			this.typeSystem = typeSystem;
			return;
		}

		private bool CheckElementsCount(ExpressionCollection elements, ExpressionCollection dimensions)
		{
			V_0 = 1;
			V_1 = dimensions.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_0 = V_0 * (Int32)(V_2 as LiteralExpression).get_Value();
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			return V_0 == elements.get_Count();
		}

		private ExpressionCollection ConvertInitialValues(byte[] initialValues, string typeName)
		{
			V_0 = new ExpressionCollection();
			if (typeName != null)
			{
				if (String.op_Equality(typeName, "Boolean"))
				{
					V_2 = 0;
					while (V_2 < (int)initialValues.Length)
					{
						V_0.Add(this.GetLiteralExpression(initialValues[V_2] != 0));
						V_2 = V_2 + 1;
					}
					return V_0;
				}
				if (String.op_Equality(typeName, "SByte"))
				{
					V_3 = 0;
					while (V_3 < (int)initialValues.Length)
					{
						V_0.Add(this.GetLiteralExpression((sbyte)initialValues[V_3]));
						V_3 = V_3 + 1;
					}
					return V_0;
				}
				if (String.op_Equality(typeName, "Byte"))
				{
					V_4 = 0;
					while (V_4 < (int)initialValues.Length)
					{
						V_0.Add(this.GetLiteralExpression(initialValues[V_4]));
						V_4 = V_4 + 1;
					}
					return V_0;
				}
				if (String.op_Equality(typeName, "Char"))
				{
					V_5 = 0;
					while (V_5 < (int)initialValues.Length / 2)
					{
						V_0.Add(this.GetLiteralExpression(BitConverter.ToChar(initialValues, V_5 * 2)));
						V_5 = V_5 + 1;
					}
					return V_0;
				}
				if (String.op_Equality(typeName, "Int16"))
				{
					V_6 = 0;
					while (V_6 < (int)initialValues.Length / 2)
					{
						V_0.Add(this.GetLiteralExpression(BitConverter.ToInt16(initialValues, V_6 * 2)));
						V_6 = V_6 + 1;
					}
					return V_0;
				}
				if (String.op_Equality(typeName, "UInt16"))
				{
					V_7 = 0;
					while (V_7 < (int)initialValues.Length / 2)
					{
						V_0.Add(this.GetLiteralExpression(BitConverter.ToUInt16(initialValues, V_7 * 2)));
						V_7 = V_7 + 1;
					}
					return V_0;
				}
				if (String.op_Equality(typeName, "Int32"))
				{
					V_8 = 0;
					while (V_8 < (int)initialValues.Length / 4)
					{
						V_0.Add(this.GetLiteralExpression(BitConverter.ToInt32(initialValues, V_8 * 4)));
						V_8 = V_8 + 1;
					}
					return V_0;
				}
				if (String.op_Equality(typeName, "UInt32"))
				{
					V_9 = 0;
					while (V_9 < (int)initialValues.Length / 4)
					{
						V_0.Add(this.GetLiteralExpression(BitConverter.ToUInt32(initialValues, V_9 * 4)));
						V_9 = V_9 + 1;
					}
					return V_0;
				}
				if (String.op_Equality(typeName, "Int64"))
				{
					V_10 = 0;
					while (V_10 < (int)initialValues.Length / 8)
					{
						V_0.Add(this.GetLiteralExpression(BitConverter.ToInt64(initialValues, V_10 * 8)));
						V_10 = V_10 + 1;
					}
					return V_0;
				}
				if (String.op_Equality(typeName, "UInt64"))
				{
					V_11 = 0;
					while (V_11 < (int)initialValues.Length / 8)
					{
						V_0.Add(this.GetLiteralExpression(BitConverter.ToUInt64(initialValues, V_11 * 8)));
						V_11 = V_11 + 1;
					}
					return V_0;
				}
				if (String.op_Equality(typeName, "Single"))
				{
					V_12 = 0;
					while (V_12 < (int)initialValues.Length / 4)
					{
						V_0.Add(this.GetLiteralExpression(BitConverter.ToSingle(initialValues, V_12 * 4)));
						V_12 = V_12 + 1;
					}
					return V_0;
				}
				if (String.op_Equality(typeName, "Double"))
				{
					V_13 = 0;
					while (V_13 < (int)initialValues.Length / 8)
					{
						V_0.Add(this.GetLiteralExpression(BitConverter.ToDouble(initialValues, V_13 * 8)));
						V_13 = V_13 + 1;
					}
					return V_0;
				}
			}
			return null;
		}

		private LiteralExpression GetLiteralExpression(object value)
		{
			return new LiteralExpression(value, this.typeSystem, null);
		}

		public void RebuildDimensions(ref ExpressionCollection elements, ExpressionCollection dimensions)
		{
			V_0 = elements.get_Count();
			V_1 = dimensions.get_Count() - 1;
			while (V_1 > 0)
			{
				V_2 = new ExpressionCollection();
				V_3 = (Int32)(dimensions.get_Item(V_1) as LiteralExpression).get_Value();
				V_4 = 0;
				while (V_4 < V_0)
				{
					V_5 = new BlockExpression(null);
					V_5.set_Expressions(new ExpressionCollection());
					V_6 = 0;
					while (V_6 < V_3)
					{
						V_5.get_Expressions().Add(elements.get_Item(V_4 + V_6));
						V_6 = V_6 + 1;
					}
					V_2.Add(V_5);
					V_4 = V_4 + V_3;
				}
				elements = V_2;
				V_0 = V_0 / V_3;
				V_1 = V_1 - 1;
			}
			return;
		}

		private bool TryFillInitializer(ArrayCreationExpression expression, MemberHandleExpression values)
		{
			if (values.get_MemberReference() as FieldDefinition == null)
			{
				return false;
			}
			V_0 = this.ConvertInitialValues((values.get_MemberReference() as FieldDefinition).get_InitialValue(), expression.get_ElementType().get_Name());
			if (V_0 == null || !this.CheckElementsCount(V_0, expression.get_Dimensions()))
			{
				return false;
			}
			this.RebuildDimensions(ref V_0, expression.get_Dimensions());
			expression.get_Initializer().set_Expressions(V_0);
			return true;
		}

		public ICodeNode VisitArrayCreationExpression(ArrayCreationExpression node)
		{
			if (node.get_Initializer() == null || node.get_Initializer().get_Expressions() == null || node.get_Initializer().get_Expressions().get_Count() != 1)
			{
				return null;
			}
			V_0 = node.get_Initializer().get_Expressions().get_Item(0) as MemberHandleExpression;
			if (V_0 == null)
			{
				return null;
			}
			if (!this.TryFillInitializer(node, V_0))
			{
				return null;
			}
			return node;
		}
	}
}