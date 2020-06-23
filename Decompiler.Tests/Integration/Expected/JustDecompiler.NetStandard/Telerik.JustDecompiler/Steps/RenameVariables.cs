using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Common;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Steps
{
	public class RenameVariables : BaseCodeVisitor, IDecompilationStep
	{
		protected RenameVariables.State state;

		protected readonly Stack<RenameVariables.ExpressionKind> expressions = new Stack<RenameVariables.ExpressionKind>();

		protected readonly Dictionary<VariableDefinition, RenameVariables.VariableSuggestion> variableSuggestedNames = new Dictionary<VariableDefinition, RenameVariables.VariableSuggestion>();

		protected readonly HashSet<string> suggestedNames;

		protected DecompilationContext context;

		protected MethodSpecificContext methodContext;

		protected TypeSpecificContext typeContext;

		private int forInitializerStartSymbol = 105;

		private int forInitializerNumberSuffix;

		public RenameVariables()
		{
			this.suggestedNames = new HashSet<string>();
		}

		private void AddNewSuggestion(ICollection<string> suggestedNames, string name)
		{
			if (this.IsValidNameInContext(name, null))
			{
				suggestedNames.Add(name);
			}
		}

		protected string Camelize(string name, RenameVariables.Conversion conversion)
		{
			string str = name;
			if (name.StartsWith("I") && name.Length > 1 && Char.IsUpper(name[1]))
			{
				str = name.Substring(1);
			}
			str = Inflector.Camelize(this.GetSafeTypeName(str, String.Empty));
			if (conversion == RenameVariables.Conversion.Singular)
			{
				str = Inflector.Singularize(str);
			}
			if (conversion == RenameVariables.Conversion.Plural)
			{
				str = Inflector.Pluralize(str);
			}
			if (str == name)
			{
				str = String.Concat("_", name);
			}
			return str;
		}

		private void ClearPendingForSuggestion()
		{
			foreach (KeyValuePair<VariableDefinition, RenameVariables.VariableSuggestion> variableSuggestedName in this.variableSuggestedNames)
			{
				variableSuggestedName.Value.IsPendingForSuggestion = false;
			}
		}

		private void CollectVariableNames()
		{
			string name;
			HashSet<string> strs = new HashSet<string>(this.methodContext.VariableNamesCollection, this.context.Language.IdentifierComparer);
			foreach (VariableDefinition variable in this.methodContext.Body.Variables)
			{
				if (!this.methodContext.VariableDefinitionToNameMap.TryGetValue(variable, out name))
				{
					name = variable.Name;
				}
				if (strs.Contains(name))
				{
					continue;
				}
				strs.Add(name);
			}
			foreach (string value in this.methodContext.VariableDefinitionToNameMap.Values)
			{
				if (strs.Contains(value))
				{
					continue;
				}
				strs.Add(value);
			}
			this.methodContext.VariableNamesCollection = strs;
		}

		private string EscapeIfGlobalKeyword(string name)
		{
			if (!this.context.Language.IsGlobalKeyword(name))
			{
				return name;
			}
			return this.context.Language.EscapeWord(name);
		}

		private RenameVariables.ExpressionKind GetExpressionKind()
		{
			RenameVariables.ExpressionKind expressionKind;
			RenameVariables.ExpressionKind expressionKind1 = RenameVariables.ExpressionKind.None;
			Stack<RenameVariables.ExpressionKind>.Enumerator enumerator = this.expressions.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					RenameVariables.ExpressionKind current = enumerator.Current;
					if (current == RenameVariables.ExpressionKind.None)
					{
						expressionKind = RenameVariables.ExpressionKind.None;
						return expressionKind;
					}
					else if (current != RenameVariables.ExpressionKind.RightAssignment)
					{
						if (current == RenameVariables.ExpressionKind.ForInitializer && expressionKind1 == RenameVariables.ExpressionKind.LeftAssignment)
						{
							expressionKind1 = RenameVariables.ExpressionKind.ForInitializer;
						}
						if (expressionKind1 != RenameVariables.ExpressionKind.None)
						{
							continue;
						}
						expressionKind1 = current;
					}
					else
					{
						expressionKind = RenameVariables.ExpressionKind.RightAssignment;
						return expressionKind;
					}
				}
				return expressionKind1;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return expressionKind;
		}

		private string GetFirstSuggestedName(ICollection<string> suggestedNames)
		{
			string current;
			using (IEnumerator<string> enumerator = suggestedNames.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					current = enumerator.Current;
				}
				else
				{
					return String.Empty;
				}
			}
			return current;
		}

		private string GetForInitializerName()
		{
			string str = Char.ConvertFromUtf32(this.forInitializerStartSymbol);
			if (this.forInitializerNumberSuffix > 0)
			{
				str = String.Concat(str, this.forInitializerNumberSuffix.ToString());
			}
			this.forInitializerStartSymbol++;
			if (this.forInitializerStartSymbol == 122)
			{
				this.forInitializerStartSymbol = 97;
			}
			if (this.forInitializerStartSymbol == 105)
			{
				this.forInitializerNumberSuffix++;
			}
			return str;
		}

		private string GetFriendlyGenericName(TypeReference typeReference)
		{
			string friendlyTypeName = typeReference.GetFriendlyTypeName(null, "<", ">");
			if (friendlyTypeName.Contains("<"))
			{
				friendlyTypeName = friendlyTypeName.Substring(0, friendlyTypeName.IndexOf('<'));
			}
			return friendlyTypeName;
		}

		protected virtual string GetFriendlyNameByType(TypeReference typeReference)
		{
			return this.Camelize(this.GetFriendlyGenericName(typeReference), RenameVariables.Conversion.Singular);
		}

		private string GetNameByType(TypeReference typeReference)
		{
			TypeDefinition typeDefinition = typeReference.Resolve();
			if (typeDefinition != null && typeDefinition.HasCompilerGeneratedAttribute())
			{
				return "variable";
			}
			if (!this.ShouldBePluralized(typeReference))
			{
				return this.GetFriendlyNameByType(typeReference);
			}
			if (!typeReference.IsGenericInstance)
			{
				if (typeReference.GenericParameters.Count != 0)
				{
					return this.Camelize(this.GetFriendlyGenericName(typeReference), RenameVariables.Conversion.Singular);
				}
				string friendlyTypeName = typeReference.GetFriendlyTypeName(null, "<", ">");
				return this.Camelize(friendlyTypeName, RenameVariables.Conversion.Plural);
			}
			GenericInstanceType genericInstanceType = (GenericInstanceType)typeReference;
			TypeReference item = genericInstanceType.GenericArguments[0];
			if (genericInstanceType.PostionToArgument.ContainsKey(0))
			{
				item = genericInstanceType.PostionToArgument[0];
			}
			typeDefinition = item.Resolve();
			if (typeDefinition != null && typeDefinition.HasCompilerGeneratedAttribute())
			{
				return "collection";
			}
			return this.Camelize(this.GetFriendlyGenericName(item), RenameVariables.Conversion.Plural);
		}

		private string GetNameWithSuffix(string name, int suffix)
		{
			string str = name;
			if (suffix >= 1)
			{
				str = String.Concat(str, suffix.ToString());
			}
			return str;
		}

		private string GetSafeBaseTypeName(string name, string suffix)
		{
			string lower = name.ToLower();
			if (lower != null)
			{
				if (lower == "decimal" || lower == "float" || lower == "byte" || lower == "sbyte" || lower == "short" || lower == "int" || lower == "long" || lower == "ushort" || lower == "uint" || lower == "ulong" || lower == "double" || lower == "int16" || lower == "int32" || lower == "int64" || lower == "uint16" || lower == "uint32" || lower == "uint64")
				{
					return String.Concat("num", suffix);
				}
				if (lower == "char")
				{
					return String.Concat("chr", suffix);
				}
				if (lower == "boolean")
				{
					return String.Concat("flag", suffix);
				}
				if (lower == "bool")
				{
					return String.Concat("flag", suffix);
				}
				if (lower == "string")
				{
					return String.Concat("str", suffix);
				}
				if (lower == "object")
				{
					return String.Concat("obj", suffix);
				}
				if (lower == "int32 modopt(system.runtime.compilerservices.islong)")
				{
					return "intPtr";
				}
			}
			return String.Concat(name, suffix);
		}

		private string GetSafeTypeName(string name, string suffix)
		{
			if (name.Contains("[") && name.Contains("]"))
			{
				if (!suffix.Contains("Array"))
				{
					suffix = String.Concat(suffix, "Array");
				}
				return this.GetSafeTypeName(name.Substring(0, name.IndexOf("[")), suffix);
			}
			if (!name.EndsWith("*") && !name.EndsWith("&"))
			{
				if (!name.Contains("`"))
				{
					return this.GetSafeBaseTypeName(name, suffix);
				}
				return String.Concat(name.Substring(0, name.IndexOf("`")), suffix);
			}
			if (!suffix.Contains("Pointer"))
			{
				suffix = String.Concat(suffix, "Pointer");
			}
			return this.GetSafeTypeName(name.Substring(0, name.Length - 1), suffix);
		}

		private bool HasArgumentWithSameName(string name)
		{
			bool flag;
			Dictionary<ParameterDefinition, string>.Enumerator enumerator = this.methodContext.ParameterDefinitionToNameMap.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<ParameterDefinition, string> current = enumerator.Current;
					if (this.context.Language.IdentifierComparer.Compare(current.Value, name) != 0)
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		private bool HasGenericParameterWithSameName(string name)
		{
			bool flag;
			if (!this.methodContext.Method.HasGenericParameters)
			{
				return false;
			}
			Collection<GenericParameter>.Enumerator enumerator = this.methodContext.Method.GenericParameters.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					GenericParameter current = enumerator.Current;
					if (this.context.Language.IdentifierComparer.Compare(current.GetFriendlyFullName(this.context.Language), name) != 0)
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		private bool HasMethodParameterWithSameName(string name)
		{
			bool flag;
			Collection<ParameterDefinition>.Enumerator enumerator = this.methodContext.Method.Parameters.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ParameterDefinition current = enumerator.Current;
					if (this.context.Language.IdentifierComparer.Compare(current.Name, name) != 0)
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		private bool HasVariableWithSameName(string name, VariableDefinition variable)
		{
			bool flag;
			Dictionary<VariableDefinition, string>.Enumerator enumerator = this.methodContext.VariableDefinitionToNameMap.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<VariableDefinition, string> current = enumerator.Current;
					if (current.Key == variable || this.context.Language.IdentifierComparer.Compare(current.Value, name) != 0)
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		protected bool IsCollection(TypeReference typeReference)
		{
			bool flag;
			TypeDefinition typeDefinition = typeReference.Resolve();
			if (typeDefinition != null)
			{
				Collection<TypeReference>.Enumerator enumerator = typeDefinition.Interfaces.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						TypeReference current = enumerator.Current;
						if (!(current.FullName == "System.Collections.IEnumerable") && !(current.FullName == "System.Collections.IList") && !(current.FullName == "System.Collections.ICollection"))
						{
							continue;
						}
						flag = true;
						return flag;
					}
					return false;
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				return flag;
			}
			return false;
		}

		private bool IsForInitializerSuggestedForVariable(VariableDefinition variable)
		{
			RenameVariables.VariableSuggestion variableSuggestion;
			if (!this.variableSuggestedNames.TryGetValue(variable, out variableSuggestion))
			{
				return false;
			}
			return variableSuggestion.IsForInitializerSuggested;
		}

		private bool IsTempVariable(VariableReference variable)
		{
			string item = this.methodContext.VariableDefinitionToNameMap[variable.Resolve()];
			if (!item.StartsWith("CS$") && !item.StartsWith("VB$") && item.IndexOf("__init") <= 0 && !item.StartsWith("stackVariable") && !item.StartsWith("exception_"))
			{
				return false;
			}
			return true;
		}

		protected virtual bool IsValidNameInContext(string name, VariableDefinition variable)
		{
			if (this.HasGenericParameterWithSameName(name) || this.HasArgumentWithSameName(name))
			{
				return false;
			}
			return !this.HasVariableWithSameName(name, variable);
		}

		private void Preprocess()
		{
			int num = 0;
			Dictionary<ParameterDefinition, string> parameterDefinitions = new Dictionary<ParameterDefinition, string>();
			foreach (KeyValuePair<ParameterDefinition, string> parameterDefinitionToNameMap in this.methodContext.ParameterDefinitionToNameMap)
			{
				ParameterDefinition key = parameterDefinitionToNameMap.Key;
				string value = parameterDefinitionToNameMap.Value;
				string str = value;
				bool flag = !str.IsValidIdentifier();
				while (flag || this.HasMethodParameterWithSameName(str))
				{
					flag = false;
					int num1 = num;
					num = num1 + 1;
					str = String.Concat("argument", num1.ToString());
				}
				if (str == value)
				{
					continue;
				}
				parameterDefinitions.Add(key, str);
			}
			foreach (KeyValuePair<ParameterDefinition, string> parameterDefinition in parameterDefinitions)
			{
				this.methodContext.ParameterDefinitionToNameMap[parameterDefinition.Key] = parameterDefinition.Value;
			}
			foreach (ParameterDefinition parameter in this.methodContext.Body.Method.Parameters)
			{
				string name = parameter.Name;
				if (String.IsNullOrEmpty(name))
				{
					name = this.GetNameByType(parameter.ParameterType);
				}
				this.methodContext.ParameterDefinitionToNameMap.Add(parameter, name);
			}
			if (this.methodContext.Method.IsSetter && this.methodContext.Method.Parameters.Count == 1)
			{
				ParameterDefinition item = this.methodContext.Method.Parameters[0];
				this.methodContext.ParameterDefinitionToNameMap[item] = "value";
			}
			foreach (VariableDefinition variable in this.methodContext.Body.Variables)
			{
				if (!this.methodContext.ParameterDefinitionToNameMap.ContainsValue(variable.Name))
				{
					continue;
				}
				this.methodContext.VariablesToRename.Add(variable);
			}
			foreach (KeyValuePair<VariableDefinition, string> variableDefinitionToNameMap in this.methodContext.VariableDefinitionToNameMap)
			{
				if (!this.methodContext.ParameterDefinitionToNameMap.ContainsValue(variableDefinitionToNameMap.Value))
				{
					continue;
				}
				this.methodContext.VariablesToRename.Add(variableDefinitionToNameMap.Key);
			}
		}

		public virtual BlockStatement Process(DecompilationContext context, BlockStatement block)
		{
			this.context = context;
			this.methodContext = context.MethodContext;
			this.typeContext = context.TypeContext;
			this.Preprocess();
			this.VisitBlockStatement(block);
			this.ReplaceDeclarations(block);
			this.CollectVariableNames();
			return block;
		}

		private void ProcessVariableDeclaration(VariableDeclarationExpression node)
		{
			if (!this.methodContext.VariableDefinitionToNameMap.ContainsKey(node.Variable))
			{
				this.methodContext.VariableDefinitionToNameMap.Add(node.Variable, node.Variable.Name);
			}
			if (this.state == RenameVariables.State.SearchForPossibleNames)
			{
				this.SuggestNameForVariable(node.Variable);
				return;
			}
			if (this.state == RenameVariables.State.RenameVariables)
			{
				this.TryRenameVariable(node.Variable);
			}
		}

		private void ReplaceDeclarations(BlockStatement block)
		{
			this.state = RenameVariables.State.RenameVariables;
			this.VisitBlockStatement(block);
			this.state = RenameVariables.State.SearchForPossibleNames;
			this.variableSuggestedNames.Clear();
			this.forInitializerStartSymbol = 105;
			this.forInitializerNumberSuffix = 0;
		}

		private bool ShouldBePluralized(TypeReference typeReference)
		{
			if (typeReference.FullName == "System.String")
			{
				return false;
			}
			if (typeReference.IsPrimitive)
			{
				return false;
			}
			return this.IsCollection(typeReference);
		}

		private void SuggestNameForMethod(RenameVariables.ExpressionKind expressionKind, string methodName, int startIndex)
		{
			RenameVariables.Conversion conversion = RenameVariables.Conversion.None;
			if (expressionKind == RenameVariables.ExpressionKind.ForEachExpression)
			{
				conversion = RenameVariables.Conversion.Singular;
			}
			methodName = methodName.Substring(startIndex);
			methodName = this.Camelize(methodName, conversion);
			this.SuggestNameForThePendingVariable(methodName);
		}

		private void SuggestNameForThePendingVariable(string name)
		{
			foreach (KeyValuePair<VariableDefinition, RenameVariables.VariableSuggestion> variableSuggestedName in this.variableSuggestedNames)
			{
				if (!variableSuggestedName.Value.IsPendingForSuggestion)
				{
					continue;
				}
				this.AddNewSuggestion(variableSuggestedName.Value.SuggestedNames, name);
				variableSuggestedName.Value.IsPendingForSuggestion = false;
				return;
			}
		}

		protected virtual void SuggestNameForVariable(VariableDefinition variableDefinition)
		{
			RenameVariables.VariableSuggestion variableSuggestion;
			if (this.state == RenameVariables.State.SearchForPossibleNames)
			{
				RenameVariables.ExpressionKind expressionKind = this.GetExpressionKind();
				if (expressionKind == RenameVariables.ExpressionKind.ForInitializer && !this.IsForInitializerSuggestedForVariable(variableDefinition))
				{
					this.TryAddNewSuggestionForVariable(variableDefinition, this.GetForInitializerName()).IsForInitializerSuggested = true;
				}
				if (expressionKind == RenameVariables.ExpressionKind.ForEachVariable || expressionKind == RenameVariables.ExpressionKind.LeftAssignment)
				{
					this.TrySetPendingForSuggestion(variableDefinition);
					RenameVariables.VariableSuggestion variableSuggestion1 = null;
					this.variableSuggestedNames.TryGetValue(variableDefinition, out variableSuggestion1);
				}
				if (expressionKind == RenameVariables.ExpressionKind.ForEachExpression)
				{
					if (this.variableSuggestedNames.TryGetValue(variableDefinition, out variableSuggestion) && variableSuggestion.SuggestedNames.Count > 0)
					{
						this.TrySetPendingName(this.GetFirstSuggestedName(variableSuggestion.SuggestedNames), true);
						return;
					}
					this.ClearPendingForSuggestion();
				}
			}
		}

		private RenameVariables.VariableSuggestion TryAddNewSuggestionForVariable(VariableDefinition variable, string name)
		{
			RenameVariables.VariableSuggestion variableSuggestion;
			if (!this.variableSuggestedNames.TryGetValue(variable, out variableSuggestion))
			{
				variableSuggestion = new RenameVariables.VariableSuggestion(name);
				this.variableSuggestedNames.Add(variable, variableSuggestion);
			}
			else
			{
				this.AddNewSuggestion(variableSuggestion.SuggestedNames, name);
			}
			return variableSuggestion;
		}

		private bool TryNameByGetMethod(string methodName, RenameVariables.ExpressionKind expressionKind)
		{
			if (!methodName.ToLower().StartsWith("get"))
			{
				return false;
			}
			if (methodName.Length > 3)
			{
				if (Char.IsUpper(methodName[3]))
				{
					if (methodName.Length == 3)
					{
						return false;
					}
					this.SuggestNameForMethod(expressionKind, methodName, 3);
					return true;
				}
				if (methodName[3] == '\u005F')
				{
					if (methodName.Length == 4)
					{
						return false;
					}
					this.SuggestNameForMethod(expressionKind, methodName, 4);
					return true;
				}
			}
			return false;
		}

		private bool TryNameByToMethod(string methodName, RenameVariables.ExpressionKind expressionKind)
		{
			for (int i = methodName.IndexOf("To", 0); i >= 0; i = methodName.IndexOf("To", i + 1))
			{
				if (i + 2 < methodName.Length && Char.IsUpper(methodName[i + 2]))
				{
					this.SuggestNameForMethod(expressionKind, methodName, i + 2);
					return true;
				}
			}
			return false;
		}

		private void TryRenameUsingSuggestions(VariableDefinition variable, RenameVariables.VariableSuggestion variableSuggestion)
		{
			int num = 0;
			while (true)
			{
				foreach (string suggestedName in variableSuggestion.SuggestedNames)
				{
					if (!this.TryRenameVariable(variable, suggestedName, num))
					{
						continue;
					}
					return;
				}
				if (this.TryRenameVariable(variable, this.GetNameByType(variable.VariableType), num))
				{
					return;
				}
				num++;
			}
		}

		protected virtual void TryRenameVariable(VariableDefinition variable)
		{
			RenameVariables.VariableSuggestion variableSuggestion;
			if (this.state == RenameVariables.State.RenameVariables)
			{
				if (!this.variableSuggestedNames.TryGetValue(variable, out variableSuggestion))
				{
					variableSuggestion = new RenameVariables.VariableSuggestion()
					{
						IsRenamed = true
					};
					this.TryRenameUsingSuggestions(variable, variableSuggestion);
				}
				else if (!variableSuggestion.IsRenamed)
				{
					variableSuggestion.IsRenamed = true;
					this.TryRenameUsingSuggestions(variable, variableSuggestion);
					return;
				}
			}
		}

		private bool TryRenameVariable(VariableDefinition variable, string name, int suffix)
		{
			if (!this.IsTempVariable(variable) && !this.methodContext.VariablesToRename.Contains(variable))
			{
				return true;
			}
			string nameWithSuffix = this.GetNameWithSuffix(name, suffix);
			if (this.suggestedNames.Contains(nameWithSuffix))
			{
				return false;
			}
			string str = nameWithSuffix;
			if (!this.context.Language.IsValidIdentifier(nameWithSuffix))
			{
				str = this.context.Language.ReplaceInvalidCharactersInIdentifier(nameWithSuffix);
			}
			str = this.EscapeIfGlobalKeyword(str);
			if (!this.IsValidNameInContext(str, variable))
			{
				return false;
			}
			this.methodContext.VariableDefinitionToNameMap[variable] = str;
			this.methodContext.VariablesToRename.Remove(variable);
			this.suggestedNames.Add(str);
			return true;
		}

		private void TrySetMethodInvocationPendingName(Expression methodExpression)
		{
			if (this.state != RenameVariables.State.SearchForPossibleNames)
			{
				return;
			}
			RenameVariables.ExpressionKind expressionKind = this.GetExpressionKind();
			if (expressionKind != RenameVariables.ExpressionKind.RightAssignment && expressionKind != RenameVariables.ExpressionKind.ForEachExpression)
			{
				return;
			}
			if (!(methodExpression is MethodReferenceExpression))
			{
				return;
			}
			MethodReference method = ((MethodReferenceExpression)methodExpression).Method;
			string name = method.Name ?? method.FullName;
			if (name.Contains("`"))
			{
				name = name.Substring(0, name.IndexOf('\u0060'));
			}
			if (this.TryNameByGetMethod(name, expressionKind))
			{
				return;
			}
			this.TryNameByToMethod(name, expressionKind);
		}

		private void TrySetObjectCreationPendingName(TypeReference typeReference)
		{
			if (this.state != RenameVariables.State.SearchForPossibleNames)
			{
				return;
			}
			if (this.GetExpressionKind() == RenameVariables.ExpressionKind.RightAssignment)
			{
				this.SuggestNameForThePendingVariable(this.GetNameByType(typeReference));
			}
		}

		private void TrySetPendingForSuggestion(VariableDefinition variable)
		{
			RenameVariables.VariableSuggestion variableSuggestion;
			if (this.variableSuggestedNames.TryGetValue(variable, out variableSuggestion))
			{
				variableSuggestion.IsPendingForSuggestion = true;
				return;
			}
			variableSuggestion = new RenameVariables.VariableSuggestion()
			{
				IsPendingForSuggestion = true
			};
			this.variableSuggestedNames.Add(variable, variableSuggestion);
		}

		private void TrySetPendingName(string name, bool suggestSameName)
		{
			if (this.state != RenameVariables.State.SearchForPossibleNames)
			{
				return;
			}
			RenameVariables.ExpressionKind expressionKind = this.GetExpressionKind();
			if (expressionKind == RenameVariables.ExpressionKind.ForEachExpression || expressionKind == RenameVariables.ExpressionKind.RightAssignment)
			{
				RenameVariables.Conversion conversion = (expressionKind == RenameVariables.ExpressionKind.ForEachExpression ? RenameVariables.Conversion.Singular : RenameVariables.Conversion.None);
				if (String.IsNullOrEmpty(name))
				{
					this.ClearPendingForSuggestion();
					return;
				}
				string str = this.Camelize(name, conversion);
				if (str != String.Concat("_", name))
				{
					if (suggestSameName || name != str)
					{
						this.SuggestNameForThePendingVariable(str);
					}
					this.ClearPendingForSuggestion();
					return;
				}
				if (!suggestSameName)
				{
					this.ClearPendingForSuggestion();
				}
			}
		}

		public override void VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpression node)
		{
			if (node.Constructor != null)
			{
				this.TrySetObjectCreationPendingName(node.Constructor.DeclaringType);
			}
			this.ClearPendingForSuggestion();
			base.VisitAnonymousObjectCreationExpression(node);
		}

		public override void VisitArgumentReferenceExpression(ArgumentReferenceExpression node)
		{
			this.TrySetPendingName(node.Parameter.Name, false);
			base.VisitArgumentReferenceExpression(node);
		}

		public override void VisitArrayCreationExpression(ArrayCreationExpression node)
		{
			this.expressions.Push(RenameVariables.ExpressionKind.None);
			this.Visit(node.Dimensions);
			this.expressions.Pop();
			this.Visit(node.Initializer);
		}

		public override void VisitArrayIndexerExpression(ArrayIndexerExpression node)
		{
			this.Visit(node.Target);
			this.expressions.Push(RenameVariables.ExpressionKind.None);
			this.Visit(node.Indices);
			this.expressions.Pop();
		}

		public override void VisitArrayLengthExpression(ArrayLengthExpression node)
		{
			this.TrySetPendingName("Length", true);
			base.VisitArrayLengthExpression(node);
		}

		private ICodeNode VisitAssignExpression(BinaryExpression node)
		{
			this.expressions.Push(RenameVariables.ExpressionKind.LeftAssignment);
			this.Visit(node.Left);
			this.expressions.Pop();
			this.expressions.Push(RenameVariables.ExpressionKind.RightAssignment);
			this.Visit(node.Right);
			this.expressions.Pop();
			this.ClearPendingForSuggestion();
			return node;
		}

		public override void VisitBinaryExpression(BinaryExpression node)
		{
			if (!node.IsAssignmentExpression && (!node.IsSelfAssign || node.IsEventHandlerAddOrRemove))
			{
				base.VisitBinaryExpression(node);
				return;
			}
			this.VisitAssignExpression(node);
		}

		public override void VisitConditionExpression(ConditionExpression node)
		{
			this.expressions.Push(RenameVariables.ExpressionKind.None);
			base.VisitConditionExpression(node);
			this.expressions.Pop();
		}

		public override void VisitFieldReferenceExpression(FieldReferenceExpression node)
		{
			string str;
			if (node.Target is TypeReferenceExpression)
			{
				TypeReference type = ((TypeReferenceExpression)node.Target).Type;
				if (type is TypeDefinition && ((TypeDefinition)type).IsEnum)
				{
					base.VisitFieldReferenceExpression(node);
					return;
				}
			}
			FieldDefinition fieldDefinition = node.Field.Resolve();
			str = (fieldDefinition == null || !this.typeContext.BackingFieldToNameMap.ContainsKey(fieldDefinition) ? node.Field.Name : this.typeContext.BackingFieldToNameMap[fieldDefinition]);
			this.TrySetPendingName(str, true);
			base.VisitFieldReferenceExpression(node);
		}

		public override void VisitForEachStatement(ForEachStatement node)
		{
			this.expressions.Push(RenameVariables.ExpressionKind.ForEachVariable);
			this.Visit(node.Variable);
			this.expressions.Pop();
			this.expressions.Push(RenameVariables.ExpressionKind.ForEachExpression);
			this.Visit(node.Collection);
			this.expressions.Pop();
			this.ClearPendingForSuggestion();
			this.Visit(node.Body);
		}

		public override void VisitForStatement(ForStatement node)
		{
			this.expressions.Push(RenameVariables.ExpressionKind.ForInitializer);
			this.Visit(node.Initializer);
			this.expressions.Pop();
			this.Visit(node.Condition);
			this.Visit(node.Increment);
			this.Visit(node.Body);
		}

		public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			this.TrySetMethodInvocationPendingName(node.MethodExpression);
			this.ClearPendingForSuggestion();
			base.VisitMethodInvocationExpression(node);
		}

		public override void VisitObjectCreationExpression(ObjectCreationExpression node)
		{
			if (node.Constructor != null)
			{
				this.TrySetObjectCreationPendingName(node.Constructor.DeclaringType);
			}
			this.ClearPendingForSuggestion();
			base.VisitObjectCreationExpression(node);
		}

		public override void VisitPropertyReferenceExpression(PropertyReferenceExpression node)
		{
			this.TrySetPendingName(node.Property.Name, true);
			base.VisitPropertyReferenceExpression(node);
		}

		public override void VisitRefVariableDeclarationExpression(RefVariableDeclarationExpression node)
		{
			this.ProcessVariableDeclaration(node);
			base.VisitRefVariableDeclarationExpression(node);
		}

		public override void VisitVariableDeclarationExpression(VariableDeclarationExpression node)
		{
			this.ProcessVariableDeclaration(node);
			base.VisitVariableDeclarationExpression(node);
		}

		public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			VariableDefinition name = node.Variable.Resolve();
			if (!this.methodContext.VariableDefinitionToNameMap.ContainsKey(name))
			{
				this.methodContext.VariableDefinitionToNameMap[name] = name.Name;
			}
			if (this.state == RenameVariables.State.SearchForPossibleNames)
			{
				this.SuggestNameForVariable(name);
			}
			if (this.state == RenameVariables.State.RenameVariables && this.methodContext.UndeclaredLinqVariables.Remove(name))
			{
				this.TryRenameVariable(name);
			}
			base.VisitVariableReferenceExpression(node);
		}

		protected enum Conversion
		{
			None,
			Singular,
			Plural
		}

		protected enum ExpressionKind
		{
			None,
			ForInitializer,
			ForEachVariable,
			ForEachExpression,
			LeftAssignment,
			RightAssignment
		}

		protected enum State
		{
			SearchForPossibleNames,
			RenameVariables
		}

		protected class VariableSuggestion
		{
			public bool IsForInitializerSuggested
			{
				get;
				set;
			}

			public bool IsPendingForSuggestion
			{
				get;
				set;
			}

			public bool IsRenamed
			{
				get;
				set;
			}

			public ICollection<string> SuggestedNames
			{
				get;
				private set;
			}

			public VariableSuggestion()
			{
				this.SuggestedNames = new List<string>();
			}

			public VariableSuggestion(string suggestedName) : this()
			{
				this.SuggestedNames.Add(suggestedName);
			}
		}
	}
}