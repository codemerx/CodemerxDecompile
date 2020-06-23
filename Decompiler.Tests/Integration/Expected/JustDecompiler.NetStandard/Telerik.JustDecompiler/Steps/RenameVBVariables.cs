using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Steps
{
	public class RenameVBVariables : RenameVariables
	{
		public RenameVBVariables()
		{
		}

		private string GetMethodName(MethodDefinition method)
		{
			string friendlyMemberName = method.GetFriendlyMemberName(null);
			if (friendlyMemberName.Contains("<"))
			{
				friendlyMemberName = friendlyMemberName.Substring(0, friendlyMemberName.IndexOf("<"));
			}
			if (method.IsGetter && friendlyMemberName.IndexOf("get_") == 0 || method.IsSetter && friendlyMemberName.IndexOf("set_") == 0)
			{
				return friendlyMemberName.Substring(4);
			}
			if (!method.IsConstructor)
			{
				return friendlyMemberName;
			}
			return method.DeclaringType.GetFriendlyTypeName(this.context.Language, "<", ">");
		}

		protected override bool IsValidNameInContext(string name, VariableDefinition variable)
		{
			if (!base.IsValidNameInContext(name, variable))
			{
				return false;
			}
			string methodName = this.GetMethodName(this.methodContext.Method);
			if (this.context.Language.IdentifierComparer.Compare(name, methodName) == 0)
			{
				return false;
			}
			return true;
		}

		public override BlockStatement Process(DecompilationContext context, BlockStatement block)
		{
			this.context = context;
			this.suggestedNames.Add(this.GetMethodName(context.MethodContext.Method));
			return base.Process(context, block);
		}

		protected override void TryRenameVariable(VariableDefinition variable)
		{
			if (this.state != RenameVariables.State.RenameVariables)
			{
				return;
			}
			if (!this.IsValidNameInContext(this.methodContext.VariableDefinitionToNameMap[variable.Resolve()], variable))
			{
				this.methodContext.VariablesToRename.Add(variable);
			}
			base.TryRenameVariable(variable);
		}
	}
}