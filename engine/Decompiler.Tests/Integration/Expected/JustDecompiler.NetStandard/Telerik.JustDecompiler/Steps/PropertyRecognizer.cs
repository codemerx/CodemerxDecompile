using Mono.Cecil;
using Mono.Cecil.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
			this.typeSystem = typeSystem;
			this.typeContext = typeContext;
			this.language = language;
		}

		private bool IsAutoPropertyConstructorInitializerExpression(FieldReference fieldReference, out PropertyDefinition property)
		{
			FieldDefinition fieldDefinition = fieldReference.Resolve();
			if (fieldDefinition != null)
			{
				Dictionary<FieldDefinition, PropertyDefinition> fieldToPropertyMap = this.typeContext.GetFieldToPropertyMap(this.language);
				if (fieldToPropertyMap.ContainsKey(fieldDefinition) && fieldToPropertyMap[fieldDefinition] != null && !fieldToPropertyMap[fieldDefinition].ShouldStaySplit())
				{
					property = fieldToPropertyMap[fieldDefinition];
					return true;
				}
			}
			property = null;
			return false;
		}

		public ICodeNode VisitFieldReferenceExpression(FieldReferenceExpression node)
		{
			PropertyDefinition propertyDefinition;
			if (!this.IsAutoPropertyConstructorInitializerExpression(node.Field, out propertyDefinition))
			{
				return node;
			}
			return new AutoPropertyConstructorInitializerExpression(propertyDefinition, node.Target, node.MappedInstructions);
		}

		public ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			MethodReferenceExpression methodExpression = node.MethodExpression;
			if (methodExpression == null)
			{
				return null;
			}
			MethodDefinition method = methodExpression.Method as MethodDefinition;
			if (method == null)
			{
				MethodReference methodReference = methodExpression.Method;
				if (methodReference != null && !String.IsNullOrEmpty(methodReference.get_Name()) && (methodReference.get_Name().StartsWith("set_") || methodReference.get_Name().StartsWith("get_") || methodReference.get_Name().StartsWith("put_")))
				{
					method = methodReference.Resolve();
				}
			}
			if (method == null || !method.get_IsGetter() && !method.get_IsSetter())
			{
				return null;
			}
			PropertyReferenceExpression propertyReferenceExpression = new PropertyReferenceExpression(node, null);
			if (propertyReferenceExpression.Property == null)
			{
				return node;
			}
			Expression binaryExpression = propertyReferenceExpression;
			if (method.get_IsSetter())
			{
				int count = node.Arguments.Count - 1;
				binaryExpression = new BinaryExpression(BinaryOperator.Assign, propertyReferenceExpression, node.Arguments[count], this.typeSystem, null, false);
			}
			return binaryExpression;
		}
	}
}