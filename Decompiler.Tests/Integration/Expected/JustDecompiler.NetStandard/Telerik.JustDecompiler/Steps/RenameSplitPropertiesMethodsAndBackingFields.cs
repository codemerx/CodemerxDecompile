using Mono.Cecil;
using Mono.Cecil.Extensions;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	public class RenameSplitPropertiesMethodsAndBackingFields : IDecompilationStep
	{
		public RenameSplitPropertiesMethodsAndBackingFields()
		{
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			PropertyDefinition propertyDefinition;
			MethodDefinition method = context.MethodContext.Method;
			if (method.IsGetter || method.IsSetter)
			{
				if (!context.TypeContext.MethodToPropertyMap.TryGetValue(method, out propertyDefinition))
				{
					throw new Exception("PropertyDefinition not found in method to property map.");
				}
				if (propertyDefinition.ShouldStaySplit())
				{
					string str = String.Concat("JustDecompileGenerated_", method.Name);
					context.TypeContext.MethodDefinitionToNameMap.Add(method, str);
					FieldDefinition compileGeneratedBackingField = Utilities.GetCompileGeneratedBackingField(propertyDefinition);
					if (compileGeneratedBackingField != null)
					{
						string str1 = compileGeneratedBackingField.Name.Replace(String.Concat("<", propertyDefinition.Name, ">"), String.Concat("JustDecompileGenerated_", propertyDefinition.Name, "_"));
						if (!context.TypeContext.BackingFieldToNameMap.ContainsKey(compileGeneratedBackingField))
						{
							context.TypeContext.BackingFieldToNameMap.Add(compileGeneratedBackingField, str1);
						}
					}
				}
			}
			return body;
		}
	}
}