using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Steps
{
	internal class RemovePrivateImplementationDetailsStep
	{
		private readonly TypeSystem typeSystem;

		public RemovePrivateImplementationDetailsStep(TypeSystem typeSystem)
		{
			this.typeSystem = typeSystem;
		}

		private bool CheckElementsCount(ExpressionCollection elements, ExpressionCollection dimensions)
		{
			int value = 1;
			foreach (Expression dimension in dimensions)
			{
				value *= (Int32)(dimension as LiteralExpression).Value;
			}
			return value == elements.Count;
		}

		private ExpressionCollection ConvertInitialValues(byte[] initialValues, string typeName)
		{
			ExpressionCollection expressionCollection = new ExpressionCollection();
			if (typeName != null)
			{
				if (typeName == "Boolean")
				{
					for (int i = 0; i < (int)initialValues.Length; i++)
					{
						expressionCollection.Add(this.GetLiteralExpression(initialValues[i] != 0));
					}
					return expressionCollection;
				}
				if (typeName == "SByte")
				{
					for (int j = 0; j < (int)initialValues.Length; j++)
					{
						expressionCollection.Add(this.GetLiteralExpression((sbyte)initialValues[j]));
					}
					return expressionCollection;
				}
				if (typeName == "Byte")
				{
					for (int k = 0; k < (int)initialValues.Length; k++)
					{
						expressionCollection.Add(this.GetLiteralExpression(initialValues[k]));
					}
					return expressionCollection;
				}
				if (typeName == "Char")
				{
					for (int l = 0; l < (int)initialValues.Length / 2; l++)
					{
						expressionCollection.Add(this.GetLiteralExpression(BitConverter.ToChar(initialValues, l * 2)));
					}
					return expressionCollection;
				}
				if (typeName == "Int16")
				{
					for (int m = 0; m < (int)initialValues.Length / 2; m++)
					{
						expressionCollection.Add(this.GetLiteralExpression(BitConverter.ToInt16(initialValues, m * 2)));
					}
					return expressionCollection;
				}
				if (typeName == "UInt16")
				{
					for (int n = 0; n < (int)initialValues.Length / 2; n++)
					{
						expressionCollection.Add(this.GetLiteralExpression(BitConverter.ToUInt16(initialValues, n * 2)));
					}
					return expressionCollection;
				}
				if (typeName == "Int32")
				{
					for (int o = 0; o < (int)initialValues.Length / 4; o++)
					{
						expressionCollection.Add(this.GetLiteralExpression(BitConverter.ToInt32(initialValues, o * 4)));
					}
					return expressionCollection;
				}
				if (typeName == "UInt32")
				{
					for (int p = 0; p < (int)initialValues.Length / 4; p++)
					{
						expressionCollection.Add(this.GetLiteralExpression(BitConverter.ToUInt32(initialValues, p * 4)));
					}
					return expressionCollection;
				}
				if (typeName == "Int64")
				{
					for (int q = 0; q < (int)initialValues.Length / 8; q++)
					{
						expressionCollection.Add(this.GetLiteralExpression(BitConverter.ToInt64(initialValues, q * 8)));
					}
					return expressionCollection;
				}
				if (typeName == "UInt64")
				{
					for (int r = 0; r < (int)initialValues.Length / 8; r++)
					{
						expressionCollection.Add(this.GetLiteralExpression(BitConverter.ToUInt64(initialValues, r * 8)));
					}
					return expressionCollection;
				}
				if (typeName == "Single")
				{
					for (int s = 0; s < (int)initialValues.Length / 4; s++)
					{
						expressionCollection.Add(this.GetLiteralExpression(BitConverter.ToSingle(initialValues, s * 4)));
					}
					return expressionCollection;
				}
				if (typeName == "Double")
				{
					for (int t = 0; t < (int)initialValues.Length / 8; t++)
					{
						expressionCollection.Add(this.GetLiteralExpression(BitConverter.ToDouble(initialValues, t * 8)));
					}
					return expressionCollection;
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
			int count = elements.Count;
			for (int i = dimensions.Count - 1; i > 0; i--)
			{
				ExpressionCollection expressionCollection = new ExpressionCollection();
				int value = (Int32)(dimensions[i] as LiteralExpression).Value;
				for (int j = 0; j < count; j += value)
				{
					BlockExpression blockExpression = new BlockExpression(null)
					{
						Expressions = new ExpressionCollection()
					};
					for (int k = 0; k < value; k++)
					{
						blockExpression.Expressions.Add(elements[j + k]);
					}
					expressionCollection.Add(blockExpression);
				}
				elements = expressionCollection;
				count /= value;
			}
		}

		private bool TryFillInitializer(ArrayCreationExpression expression, MemberHandleExpression values)
		{
			if (!(values.MemberReference is FieldDefinition))
			{
				return false;
			}
			ExpressionCollection expressionCollection = this.ConvertInitialValues((values.MemberReference as FieldDefinition).get_InitialValue(), expression.ElementType.get_Name());
			if (expressionCollection == null || !this.CheckElementsCount(expressionCollection, expression.Dimensions))
			{
				return false;
			}
			this.RebuildDimensions(ref expressionCollection, expression.Dimensions);
			expression.Initializer.Expressions = expressionCollection;
			return true;
		}

		public ICodeNode VisitArrayCreationExpression(ArrayCreationExpression node)
		{
			if (node.Initializer == null || node.Initializer.Expressions == null || node.Initializer.Expressions.Count != 1)
			{
				return null;
			}
			MemberHandleExpression item = node.Initializer.Expressions[0] as MemberHandleExpression;
			if (item == null)
			{
				return null;
			}
			if (!this.TryFillInitializer(node, item))
			{
				return null;
			}
			return node;
		}
	}
}