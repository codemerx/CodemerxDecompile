using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Common;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	public class RenameVBVariables : RenameVariables
	{
		public override BlockStatement Process(DecompilationContext context, BlockStatement block)
		{
            this.context = context;
			this.suggestedNames.Add(GetMethodName(context.MethodContext.Method));
			return base.Process(context, block);
		}

		protected override void TryRenameVariable(VariableDefinition variable)
		{
			if (state != State.RenameVariables)
			{
				return;
			}

			string name = methodContext.VariableDefinitionToNameMap[variable.Resolve()];
			if (!IsValidNameInContext(name, variable))
			{
				this.methodContext.VariablesToRename.Add(variable);
			}
			base.TryRenameVariable(variable);
		}

		protected override bool IsValidNameInContext(string name, VariableDefinition variable)
		{
			if (!base.IsValidNameInContext(name, variable))
			{
				return false;
			}

			string enclosingMethodName = GetMethodName(methodContext.Method);
			if (this.context.Language.IdentifierComparer.Compare(name, enclosingMethodName) == 0)
			{
				return false;
			}

			return true;
		}

		private string GetMethodName(MethodDefinition method)
		{
			string name = method.GetFriendlyMemberName(null);
			if (name.Contains("<"))
			{
				name = name.Substring(0, name.IndexOf("<"));
			}
			if (method.IsGetter && name.IndexOf("get_") == 0 ||
				method.IsSetter && name.IndexOf("set_") == 0)
			{
				return name.Substring(4);
			}
			if (method.IsConstructor)
			{
				return method.DeclaringType.GetFriendlyTypeName(this.context.Language);
			}
			return name;
		}
	}
}
