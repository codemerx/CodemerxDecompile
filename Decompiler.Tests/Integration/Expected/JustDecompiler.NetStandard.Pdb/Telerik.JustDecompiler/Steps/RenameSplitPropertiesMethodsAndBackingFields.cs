using Mono.Cecil;
using System;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	public class RenameSplitPropertiesMethodsAndBackingFields : IDecompilationStep
	{
		public RenameSplitPropertiesMethodsAndBackingFields()
		{
			base();
			return;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			V_0 = context.get_MethodContext().get_Method();
			if (V_0.get_IsGetter() || V_0.get_IsSetter())
			{
				if (!context.get_TypeContext().get_MethodToPropertyMap().TryGetValue(V_0, out V_1))
				{
					throw new Exception("PropertyDefinition not found in method to property map.");
				}
				if (V_1.ShouldStaySplit())
				{
					V_2 = String.Concat("JustDecompileGenerated_", V_0.get_Name());
					context.get_TypeContext().get_MethodDefinitionToNameMap().Add(V_0, V_2);
					V_3 = Utilities.GetCompileGeneratedBackingField(V_1);
					if (V_3 != null)
					{
						V_4 = V_3.get_Name().Replace(String.Concat("<", V_1.get_Name(), ">"), String.Concat("JustDecompileGenerated_", V_1.get_Name(), "_"));
						if (!context.get_TypeContext().get_BackingFieldToNameMap().ContainsKey(V_3))
						{
							context.get_TypeContext().get_BackingFieldToNameMap().Add(V_3, V_4);
						}
					}
				}
			}
			return body;
		}
	}
}