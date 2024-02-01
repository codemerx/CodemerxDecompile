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
			if (method.get_IsGetter() || method.get_IsSetter())
			{
				if (!context.TypeContext.MethodToPropertyMap.TryGetValue(method, out propertyDefinition))
				{
					throw new Exception("PropertyDefinition not found in method to property map.");
				}
				if (propertyDefinition.ShouldStaySplit())
				{
					string str = String.Concat("JustDecompileGenerated_", method.get_Name());
					context.TypeContext.MethodDefinitionToNameMap.Add(method, str);
					FieldDefinition compileGeneratedBackingField = Utilities.GetCompileGeneratedBackingField(propertyDefinition);
					if (compileGeneratedBackingField != null)
					{
						string str1 = compileGeneratedBackingField.get_Name().Replace(String.Concat("<", propertyDefinition.get_Name(), ">"), String.Concat("JustDecompileGenerated_", propertyDefinition.get_Name(), "_"));
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