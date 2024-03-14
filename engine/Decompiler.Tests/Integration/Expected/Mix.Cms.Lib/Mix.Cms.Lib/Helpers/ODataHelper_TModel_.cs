using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Mix.Cms.Lib.Helpers
{
	public class ODataHelper<TModel>
	{
		public ODataHelper()
		{
		}

		public static Expression<Func<T, bool>> CombineExpression<T>(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2, BinaryOperatorKind kind, string name = "model")
		{
			ParameterExpression parameterExpression = Expression.Parameter(typeof(T), name);
			Expression expression = (new ODataHelper<TModel>.ReplaceExpressionVisitor(expr1.Parameters[0], parameterExpression)).Visit(expr1.Body);
			Expression expression1 = (new ODataHelper<TModel>.ReplaceExpressionVisitor(expr2.Parameters[0], parameterExpression)).Visit(expr2.Body);
			switch (kind)
			{
				case 0:
				{
					return Expression.Lambda<Func<T, bool>>(Expression.Or(expression, expression1), new ParameterExpression[] { parameterExpression });
				}
				case 1:
				{
					return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expression, expression1), new ParameterExpression[] { parameterExpression });
				}
				case 2:
				case 3:
				case 4:
				case 5:
				case 6:
				case 7:
				case 8:
				case 9:
				case 10:
				case 11:
				case 12:
				case 13:
				{
					return null;
				}
				default:
				{
					return null;
				}
			}
		}

		public static Expression<Func<TModel, bool>> FilterObjectSet(SingleValuePropertyAccessNode rule, ConstantNode constant, BinaryOperatorKind kind, string name = "model")
		{
			Type fieldType;
			Expression expression;
			ParameterExpression[] parameterExpressionArray;
			Type type = typeof(TModel);
			ParameterExpression parameterExpression = Expression.Parameter(type, name);
			FieldInfo field = type.GetField(rule.get_Property().get_Name());
			if (field != null)
			{
				fieldType = field.FieldType;
				expression = Expression.Field(parameterExpression, field);
			}
			else
			{
				PropertyInfo property = type.GetProperty(rule.get_Property().get_Name());
				if (property == null)
				{
					throw new Exception();
				}
				fieldType = property.PropertyType;
				expression = Expression.Property(parameterExpression, property);
			}
			object obj = null;
			obj = (!fieldType.IsGenericType || !(fieldType.GetGenericTypeDefinition() == typeof(Nullable<>)) ? Convert.ChangeType(constant.get_LiteralText(), fieldType) : TypeDescriptor.GetConverter(fieldType).ConvertFrom(constant.get_LiteralText()));
			if (fieldType == typeof(string))
			{
				obj = obj.ToString().Replace("'", "");
			}
			BinaryExpression binaryExpression = null;
			switch (kind)
			{
				case 0:
				{
					binaryExpression = Expression.Or(expression, Expression.Constant(obj, fieldType));
					parameterExpressionArray = new ParameterExpression[] { parameterExpression };
					return Expression.Lambda<Func<TModel, bool>>(binaryExpression, parameterExpressionArray);
				}
				case 1:
				{
					binaryExpression = Expression.And(expression, Expression.Constant(obj, fieldType));
					parameterExpressionArray = new ParameterExpression[] { parameterExpression };
					return Expression.Lambda<Func<TModel, bool>>(binaryExpression, parameterExpressionArray);
				}
				case 2:
				{
					binaryExpression = Expression.Equal(expression, Expression.Constant(obj, fieldType));
					parameterExpressionArray = new ParameterExpression[] { parameterExpression };
					return Expression.Lambda<Func<TModel, bool>>(binaryExpression, parameterExpressionArray);
				}
				case 3:
				{
					binaryExpression = Expression.NotEqual(expression, Expression.Constant(obj, fieldType));
					parameterExpressionArray = new ParameterExpression[] { parameterExpression };
					return Expression.Lambda<Func<TModel, bool>>(binaryExpression, parameterExpressionArray);
				}
				case 4:
				{
					binaryExpression = Expression.GreaterThan(expression, Expression.Constant(obj));
					parameterExpressionArray = new ParameterExpression[] { parameterExpression };
					return Expression.Lambda<Func<TModel, bool>>(binaryExpression, parameterExpressionArray);
				}
				case 5:
				{
					binaryExpression = Expression.GreaterThanOrEqual(expression, Expression.Constant(obj, fieldType));
					parameterExpressionArray = new ParameterExpression[] { parameterExpression };
					return Expression.Lambda<Func<TModel, bool>>(binaryExpression, parameterExpressionArray);
				}
				case 6:
				{
					binaryExpression = Expression.LessThan(expression, Expression.Constant(obj, fieldType));
					parameterExpressionArray = new ParameterExpression[] { parameterExpression };
					return Expression.Lambda<Func<TModel, bool>>(binaryExpression, parameterExpressionArray);
				}
				case 7:
				{
					binaryExpression = Expression.LessThanOrEqual(expression, Expression.Constant(obj, fieldType));
					parameterExpressionArray = new ParameterExpression[] { parameterExpression };
					return Expression.Lambda<Func<TModel, bool>>(binaryExpression, parameterExpressionArray);
				}
				case 8:
				{
					binaryExpression = Expression.Add(expression, Expression.Constant(obj, fieldType));
					parameterExpressionArray = new ParameterExpression[] { parameterExpression };
					return Expression.Lambda<Func<TModel, bool>>(binaryExpression, parameterExpressionArray);
				}
				case 9:
				{
					binaryExpression = Expression.Subtract(expression, Expression.Constant(obj, fieldType));
					parameterExpressionArray = new ParameterExpression[] { parameterExpression };
					return Expression.Lambda<Func<TModel, bool>>(binaryExpression, parameterExpressionArray);
				}
				case 10:
				{
					binaryExpression = Expression.Multiply(expression, Expression.Constant(obj, fieldType));
					parameterExpressionArray = new ParameterExpression[] { parameterExpression };
					return Expression.Lambda<Func<TModel, bool>>(binaryExpression, parameterExpressionArray);
				}
				case 11:
				{
					binaryExpression = Expression.Divide(expression, Expression.Constant(obj, fieldType));
					parameterExpressionArray = new ParameterExpression[] { parameterExpression };
					return Expression.Lambda<Func<TModel, bool>>(binaryExpression, parameterExpressionArray);
				}
				case 12:
				{
					binaryExpression = Expression.Modulo(expression, Expression.Constant(obj, fieldType));
					parameterExpressionArray = new ParameterExpression[] { parameterExpression };
					return Expression.Lambda<Func<TModel, bool>>(binaryExpression, parameterExpressionArray);
				}
				case 13:
				{
					parameterExpressionArray = new ParameterExpression[] { parameterExpression };
					return Expression.Lambda<Func<TModel, bool>>(binaryExpression, parameterExpressionArray);
				}
				default:
				{
					parameterExpressionArray = new ParameterExpression[] { parameterExpression };
					return Expression.Lambda<Func<TModel, bool>>(binaryExpression, parameterExpressionArray);
				}
			}
		}

		private static Expression GetPropertyExpression(Type type, string name)
		{
			return Expression.Property(Expression.Parameter(type, "model"), type.GetProperty(name));
		}

		private static Type GetPropertyType(Type type, string name)
		{
			Type fieldType;
			FieldInfo field = type.GetField(name);
			if (field != null)
			{
				fieldType = field.FieldType;
			}
			else
			{
				PropertyInfo property = type.GetProperty(name);
				if (property == null)
				{
					throw new Exception();
				}
				fieldType = property.PropertyType;
			}
			return fieldType;
		}

		public static object GetPropValue(object src, string propName)
		{
			return src.GetType().GetProperty(propName).GetValue(src, null);
		}

		public static void ParseFilter(SingleValueNode node, ref Expression<Func<TModel, bool>> result, int kind = -1)
		{
			if (node is BinaryOperatorNode)
			{
				BinaryOperatorNode binaryOperatorNode = node as BinaryOperatorNode;
				SingleValueNode left = binaryOperatorNode.get_Left();
				SingleValueNode right = binaryOperatorNode.get_Right();
				if (left is ConvertNode)
				{
					ODataHelper<TModel>.ParseFilter(((ConvertNode)left).get_Source(), ref result, -1);
				}
				if (left is BinaryOperatorNode)
				{
					ODataHelper<TModel>.ParseFilter(left, ref result, -1);
				}
				if (right is ConvertNode)
				{
					ODataHelper<TModel>.ParseFilter(((ConvertNode)right).get_Source(), ref result, binaryOperatorNode.get_OperatorKind());
				}
				if (right is BinaryOperatorNode)
				{
					ODataHelper<TModel>.ParseFilter(right, ref result, binaryOperatorNode.get_OperatorKind());
				}
				if (left is SingleValuePropertyAccessNode && right is ConstantNode)
				{
					SingleValuePropertyAccessNode singleValuePropertyAccessNode = left as SingleValuePropertyAccessNode;
					ConstantNode constantNode = right as ConstantNode;
					if (singleValuePropertyAccessNode != null && singleValuePropertyAccessNode.get_Property() != null && constantNode != null && constantNode.get_Value() != null)
					{
						Expression<Func<TModel, bool>> expression = ODataHelper<TModel>.FilterObjectSet(singleValuePropertyAccessNode, constantNode, binaryOperatorNode.get_OperatorKind(), "model");
						Expression.Parameter(typeof(TModel), "model");
						if (kind >= 0 && result != null)
						{
							BinaryOperatorKind binaryOperatorKind = kind;
							result = ODataHelper<TModel>.CombineExpression<TModel>(result, expression, binaryOperatorKind, "model");
							return;
						}
						result = expression;
					}
				}
			}
		}

		public static void TryNodeValue(SingleValueNode node, IDictionary<string, object> values)
		{
			if (node is BinaryOperatorNode)
			{
				BinaryOperatorNode binaryOperatorNode = (BinaryOperatorNode)node;
				SingleValueNode left = binaryOperatorNode.get_Left();
				SingleValueNode right = binaryOperatorNode.get_Right();
				if (left is ConvertNode)
				{
					ODataHelper<TModel>.TryNodeValue(((ConvertNode)left).get_Source(), values);
				}
				if (left is BinaryOperatorNode)
				{
					ODataHelper<TModel>.TryNodeValue(left, values);
				}
				if (right is ConvertNode)
				{
					ODataHelper<TModel>.TryNodeValue(((ConvertNode)right).get_Source(), values);
				}
				if (right is BinaryOperatorNode)
				{
					ODataHelper<TModel>.TryNodeValue(right, values);
				}
				if (left is SingleValuePropertyAccessNode && right is ConstantNode)
				{
					SingleValuePropertyAccessNode singleValuePropertyAccessNode = (SingleValuePropertyAccessNode)left;
					if (binaryOperatorNode.get_OperatorKind() == 2)
					{
						object value = ((ConstantNode)right).get_Value();
						values.Add(singleValuePropertyAccessNode.get_Property().get_Name(), value);
					}
				}
			}
		}

		private class ReplaceExpressionVisitor : ExpressionVisitor
		{
			private readonly Expression _oldValue;

			private readonly Expression _newValue;

			public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
			{
				this._oldValue = oldValue;
				this._newValue = newValue;
			}

			public override Expression Visit(Expression node)
			{
				if (node == this._oldValue)
				{
					return this._newValue;
				}
				return base.Visit(node);
			}
		}
	}
}