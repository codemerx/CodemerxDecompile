using Mono.Cecil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Steps
{
	public class PropertyRecognizer
	{
		private readonly TypeSystem typeSystem;

		private readonly TypeSpecificContext typeContext;

		private readonly ILanguage language;

		public PropertyRecognizer(TypeSystem typeSystem, TypeSpecificContext typeContext, ILanguage language)
		{
			base();
			this.typeSystem = typeSystem;
			this.typeContext = typeContext;
			this.language = language;
			return;
		}

		private bool IsAutoPropertyConstructorInitializerExpression(FieldReference fieldReference, out PropertyDefinition property)
		{
			V_0 = fieldReference.Resolve();
			if (V_0 != null)
			{
				V_1 = this.typeContext.GetFieldToPropertyMap(this.language);
				if (V_1.ContainsKey(V_0) && V_1.get_Item(V_0) != null && !V_1.get_Item(V_0).ShouldStaySplit())
				{
					property = V_1.get_Item(V_0);
					return true;
				}
			}
			property = null;
			return false;
		}

		public ICodeNode VisitFieldReferenceExpression(FieldReferenceExpression node)
		{
			if (!this.IsAutoPropertyConstructorInitializerExpression(node.get_Field(), out V_0))
			{
				return node;
			}
			return new AutoPropertyConstructorInitializerExpression(V_0, node.get_Target(), node.get_MappedInstructions());
		}

		public ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			V_0 = node.get_MethodExpression();
			if (V_0 == null)
			{
				return null;
			}
			V_1 = V_0.get_Method() as MethodDefinition;
			if (V_1 == null)
			{
				V_2 = V_0.get_Method();
				if (V_2 != null && !String.IsNullOrEmpty(V_2.get_Name()) && V_2.get_Name().StartsWith("set_") || V_2.get_Name().StartsWith("get_") || V_2.get_Name().StartsWith("put_"))
				{
					V_1 = V_2.Resolve();
				}
			}
			if (V_1 == null || !V_1.get_IsGetter() && !V_1.get_IsSetter())
			{
				return null;
			}
			V_3 = new PropertyReferenceExpression(node, null);
			if (V_3.get_Property() == null)
			{
				return node;
			}
			V_4 = V_3;
			if (V_1.get_IsSetter())
			{
				V_5 = node.get_Arguments().get_Count() - 1;
				V_4 = new BinaryExpression(26, V_3, node.get_Arguments().get_Item(V_5), this.typeSystem, null, false);
			}
			return V_4;
		}
	}
}