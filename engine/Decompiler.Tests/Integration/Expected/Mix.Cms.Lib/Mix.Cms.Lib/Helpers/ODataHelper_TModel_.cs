using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Mix.Cms.Lib.Helpers
{
	public class ODataHelper<TModel>
	{
		public ODataHelper()
		{
			base();
			return;
		}

		public static Expression<Func<T, bool>> CombineExpression<T>(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2, BinaryOperatorKind kind, string name = "model")
		{
			V_0 = Expression.Parameter(Type.GetTypeFromHandle(// 
			// Current member / type: System.Linq.Expressions.Expression`1<System.Func`2<T,System.Boolean>> Mix.Cms.Lib.Helpers.ODataHelper`1::CombineExpression(System.Linq.Expressions.Expression`1<System.Func`2<T,System.Boolean>>,System.Linq.Expressions.Expression`1<System.Func`2<T,System.Boolean>>,Microsoft.OData.UriParser.BinaryOperatorKind,System.String)
			// Exception in: System.Linq.Expressions.Expression<System.Func<T,System.Boolean>> CombineExpression(System.Linq.Expressions.Expression<System.Func<T,System.Boolean>>,System.Linq.Expressions.Expression<System.Func<T,System.Boolean>>,Microsoft.OData.UriParser.BinaryOperatorKind,System.String)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public static Expression<Func<TModel, bool>> FilterObjectSet(SingleValuePropertyAccessNode rule, ConstantNode constant, BinaryOperatorKind kind, string name = "model")
		{
			V_0 = Type.GetTypeFromHandle(// 
			// Current member / type: System.Linq.Expressions.Expression`1<System.Func`2<TModel,System.Boolean>> Mix.Cms.Lib.Helpers.ODataHelper`1::FilterObjectSet(Microsoft.OData.UriParser.SingleValuePropertyAccessNode,Microsoft.OData.UriParser.ConstantNode,Microsoft.OData.UriParser.BinaryOperatorKind,System.String)
			// Exception in: System.Linq.Expressions.Expression<System.Func<TModel,System.Boolean>> FilterObjectSet(Microsoft.OData.UriParser.SingleValuePropertyAccessNode,Microsoft.OData.UriParser.ConstantNode,Microsoft.OData.UriParser.BinaryOperatorKind,System.String)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		private static Expression GetPropertyExpression(Type type, string name)
		{
			stackVariable2 = Expression.Parameter(type, "model");
			return Expression.Property(stackVariable2, type.GetProperty(name));
		}

		private static Type GetPropertyType(Type type, string name)
		{
			V_1 = type.GetField(name);
			if (!FieldInfo.op_Equality(V_1, null))
			{
				V_0 = V_1.get_FieldType();
			}
			else
			{
				stackVariable11 = type.GetProperty(name);
				if (PropertyInfo.op_Equality(stackVariable11, null))
				{
					throw new Exception();
				}
				V_0 = stackVariable11.get_PropertyType();
			}
			return V_0;
		}

		public static object GetPropValue(object src, string propName)
		{
			return src.GetType().GetProperty(propName).GetValue(src, null);
		}

		public static void ParseFilter(SingleValueNode node, ref Expression<Func<TModel, bool>> result, int kind = -1)
		{
			if (node as BinaryOperatorNode != null)
			{
				V_0 = node as BinaryOperatorNode;
				V_1 = V_0.get_Left();
				V_2 = V_0.get_Right();
				if (V_1 as ConvertNode != null)
				{
					ODataHelper<TModel>.ParseFilter(((ConvertNode)V_1).get_Source(), ref result, -1);
				}
				if (V_1 as BinaryOperatorNode != null)
				{
					ODataHelper<TModel>.ParseFilter(V_1, ref result, -1);
				}
				if (V_2 as ConvertNode != null)
				{
					ODataHelper<TModel>.ParseFilter(((ConvertNode)V_2).get_Source(), ref result, V_0.get_OperatorKind());
				}
				if (V_2 as BinaryOperatorNode != null)
				{
					ODataHelper<TModel>.ParseFilter(V_2, ref result, V_0.get_OperatorKind());
				}
				if (V_1 as SingleValuePropertyAccessNode != null && V_2 as ConstantNode != null)
				{
					V_3 = V_1 as SingleValuePropertyAccessNode;
					V_4 = V_2 as ConstantNode;
					if (V_3 != null && V_3.get_Property() != null && V_4 != null && V_4.get_Value() != null)
					{
						V_5 = ODataHelper<TModel>.FilterObjectSet(V_3, V_4, V_0.get_OperatorKind(), "model");
						dummyVar0 = Expression.Parameter(Type.GetTypeFromHandle(// 
						// Current member / type: System.Void Mix.Cms.Lib.Helpers.ODataHelper`1::ParseFilter(Microsoft.OData.UriParser.SingleValueNode,System.Linq.Expressions.Expression`1<System.Func`2<TModel,System.Boolean>>&,System.Int32)
						// Exception in: System.Void ParseFilter(Microsoft.OData.UriParser.SingleValueNode,System.Linq.Expressions.Expression<System.Func<TModel,System.Boolean>>&,System.Int32)
						// Specified method is not supported.
						// 
						// mailto: JustDecompilePublicFeedback@telerik.com


		public static void TryNodeValue(SingleValueNode node, IDictionary<string, object> values)
		{
			if (node as BinaryOperatorNode != null)
			{
				V_0 = (BinaryOperatorNode)node;
				V_1 = V_0.get_Left();
				V_2 = V_0.get_Right();
				if (V_1 as ConvertNode != null)
				{
					ODataHelper<TModel>.TryNodeValue(((ConvertNode)V_1).get_Source(), values);
				}
				if (V_1 as BinaryOperatorNode != null)
				{
					ODataHelper<TModel>.TryNodeValue(V_1, values);
				}
				if (V_2 as ConvertNode != null)
				{
					ODataHelper<TModel>.TryNodeValue(((ConvertNode)V_2).get_Source(), values);
				}
				if (V_2 as BinaryOperatorNode != null)
				{
					ODataHelper<TModel>.TryNodeValue(V_2, values);
				}
				if (V_1 as SingleValuePropertyAccessNode != null && V_2 as ConstantNode != null)
				{
					V_3 = (SingleValuePropertyAccessNode)V_1;
					if (V_0.get_OperatorKind() == 2)
					{
						V_4 = ((ConstantNode)V_2).get_Value();
						values.Add(V_3.get_Property().get_Name(), V_4);
					}
				}
			}
			return;
		}

		private class ReplaceExpressionVisitor : ExpressionVisitor
		{
			private readonly Expression _oldValue;

			private readonly Expression _newValue;

			public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
			{
				base();
				this._oldValue = oldValue;
				this._newValue = newValue;
				return;
			}

			public override Expression Visit(Expression node)
			{
				if ((object)node == (object)this._oldValue)
				{
					return this._newValue;
				}
				return this.Visit(node);
			}
		}
	}
}