using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	public class RenameVBVariables : RenameVariables
	{
		public RenameVBVariables()
		{
			base();
			return;
		}

		private string GetMethodName(MethodDefinition method)
		{
			V_0 = method.GetFriendlyMemberName(null);
			if (V_0.Contains("<"))
			{
				V_0 = V_0.Substring(0, V_0.IndexOf("<"));
			}
			if (method.get_IsGetter() && V_0.IndexOf("get_") == 0 || method.get_IsSetter() && V_0.IndexOf("set_") == 0)
			{
				return V_0.Substring(4);
			}
			if (!method.get_IsConstructor())
			{
				return V_0;
			}
			return method.get_DeclaringType().GetFriendlyTypeName(this.context.get_Language(), "<", ">");
		}

		protected override bool IsValidNameInContext(string name, VariableDefinition variable)
		{
			if (!this.IsValidNameInContext(name, variable))
			{
				return false;
			}
			V_0 = this.GetMethodName(this.methodContext.get_Method());
			if (this.context.get_Language().get_IdentifierComparer().Compare(name, V_0) == 0)
			{
				return false;
			}
			return true;
		}

		public override BlockStatement Process(DecompilationContext context, BlockStatement block)
		{
			this.context = context;
			dummyVar0 = this.suggestedNames.Add(this.GetMethodName(context.get_MethodContext().get_Method()));
			return this.Process(context, block);
		}

		protected override void TryRenameVariable(VariableDefinition variable)
		{
			if (this.state != 1)
			{
				return;
			}
			if (!this.IsValidNameInContext(this.methodContext.get_VariableDefinitionToNameMap().get_Item(variable.Resolve()), variable))
			{
				dummyVar0 = this.methodContext.get_VariablesToRename().Add(variable);
			}
			this.TryRenameVariable(variable);
			return;
		}
	}
}