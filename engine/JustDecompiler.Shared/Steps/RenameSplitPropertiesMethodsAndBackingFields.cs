using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Ast.Statements;
using Mono.Cecil;
using Mono.Cecil.Extensions;

namespace Telerik.JustDecompiler.Steps
{
	public class RenameSplitPropertiesMethodsAndBackingFields : IDecompilationStep
	{
		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{		
			MethodDefinition method = context.MethodContext.Method;
			if (method.IsGetter || method.IsSetter)
			{
				PropertyDefinition property;
				if (!context.TypeContext.MethodToPropertyMap.TryGetValue(method, out property))
				{
					throw new Exception("PropertyDefinition not found in method to property map.");
				}

				if (property.ShouldStaySplit())
				{
					string methodDefinitionNewName = Constants.JustDecompileGenerated + "_" + method.Name;
					context.TypeContext.MethodDefinitionToNameMap.Add(method, methodDefinitionNewName);

					FieldDefinition backingField = Utilities.GetCompileGeneratedBackingField(property);
					if (backingField != null)
					{
						string backingFieldNewName = backingField.Name.Replace("<" + property.Name + ">", Constants.JustDecompileGenerated + "_" + property.Name + "_");
						if (!context.TypeContext.BackingFieldToNameMap.ContainsKey(backingField))
						{
							context.TypeContext.BackingFieldToNameMap.Add(backingField, backingFieldNewName);
						}
					}
				}
			}

			return body;
		}
	}
}
