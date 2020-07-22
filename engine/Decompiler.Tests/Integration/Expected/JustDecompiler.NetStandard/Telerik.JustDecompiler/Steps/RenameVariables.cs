using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	public class RenameVariables : BaseCodeVisitor, IDecompilationStep
	{
		protected RenameVariables.State state;

		protected readonly Stack<RenameVariables.ExpressionKind> expressions;

		protected readonly Dictionary<VariableDefinition, RenameVariables.VariableSuggestion> variableSuggestedNames;

		protected readonly HashSet<string> suggestedNames;

		protected DecompilationContext context;

		protected MethodSpecificContext methodContext;

		protected TypeSpecificContext typeContext;

		private int forInitializerStartSymbol;

		private int forInitializerNumberSuffix;

		public RenameVariables()
		{
			this.expressions = new Stack<RenameVariables.ExpressionKind>();
			this.variableSuggestedNames = new Dictionary<VariableDefinition, RenameVariables.VariableSuggestion>();
			this.forInitializerStartSymbol = 105;
			base();
			this.suggestedNames = new HashSet<string>();
			return;
		}

		private void AddNewSuggestion(ICollection<string> suggestedNames, string name)
		{
			if (this.IsValidNameInContext(name, null))
			{
				suggestedNames.Add(name);
			}
			return;
		}

		protected string Camelize(string name, RenameVariables.Conversion conversion)
		{
			V_0 = name;
			if (name.StartsWith("I") && name.get_Length() > 1 && Char.IsUpper(name.get_Chars(1)))
			{
				V_0 = name.Substring(1);
			}
			V_0 = Inflector.Camelize(this.GetSafeTypeName(V_0, String.Empty));
			if (conversion == 1)
			{
				V_0 = Inflector.Singularize(V_0);
			}
			if (conversion == 2)
			{
				V_0 = Inflector.Pluralize(V_0);
			}
			if (String.op_Equality(V_0, name))
			{
				V_0 = String.Concat("_", name);
			}
			return V_0;
		}

		private void ClearPendingForSuggestion()
		{
			V_0 = this.variableSuggestedNames.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					V_1.get_Value().set_IsPendingForSuggestion(false);
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		private void CollectVariableNames()
		{
			V_0 = new HashSet<string>(this.methodContext.get_VariableNamesCollection(), this.context.get_Language().get_IdentifierComparer());
			V_1 = this.methodContext.get_Body().get_Variables().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (!this.methodContext.get_VariableDefinitionToNameMap().TryGetValue(V_2, out V_3))
					{
						V_3 = V_2.get_Name();
					}
					if (V_0.Contains(V_3))
					{
						continue;
					}
					dummyVar0 = V_0.Add(V_3);
				}
			}
			finally
			{
				V_1.Dispose();
			}
			V_4 = this.methodContext.get_VariableDefinitionToNameMap().get_Values().GetEnumerator();
			try
			{
				while (V_4.MoveNext())
				{
					V_5 = V_4.get_Current();
					if (V_0.Contains(V_5))
					{
						continue;
					}
					dummyVar1 = V_0.Add(V_5);
				}
			}
			finally
			{
				((IDisposable)V_4).Dispose();
			}
			this.methodContext.set_VariableNamesCollection(V_0);
			return;
		}

		private string EscapeIfGlobalKeyword(string name)
		{
			if (!this.context.get_Language().IsGlobalKeyword(name))
			{
				return name;
			}
			return this.context.get_Language().EscapeWord(name);
		}

		private RenameVariables.ExpressionKind GetExpressionKind()
		{
			V_0 = 0;
			V_1 = this.expressions.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (V_2 != RenameVariables.ExpressionKind.None)
					{
						if (V_2 != 5)
						{
							if (V_2 == 1 && V_0 == 4)
							{
								V_0 = 1;
							}
							if (V_0 != RenameVariables.ExpressionKind.None)
							{
								continue;
							}
							V_0 = V_2;
						}
						else
						{
							V_3 = 5;
							goto Label1;
						}
					}
					else
					{
						V_3 = 0;
						goto Label1;
					}
				}
				goto Label0;
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
		Label1:
			return V_3;
		Label0:
			return V_0;
		}

		private string GetFirstSuggestedName(ICollection<string> suggestedNames)
		{
			V_0 = suggestedNames.GetEnumerator();
			try
			{
				if (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
				}
				else
				{
					goto Label0;
				}
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
			return V_1;
		Label0:
			return String.Empty;
		}

		private string GetForInitializerName()
		{
			V_0 = Char.ConvertFromUtf32(this.forInitializerStartSymbol);
			if (this.forInitializerNumberSuffix > 0)
			{
				V_0 = String.Concat(V_0, this.forInitializerNumberSuffix.ToString());
			}
			this.forInitializerStartSymbol = this.forInitializerStartSymbol + 1;
			if (this.forInitializerStartSymbol == 122)
			{
				this.forInitializerStartSymbol = 97;
			}
			if (this.forInitializerStartSymbol == 105)
			{
				this.forInitializerNumberSuffix = this.forInitializerNumberSuffix + 1;
			}
			return V_0;
		}

		private string GetFriendlyGenericName(TypeReference typeReference)
		{
			V_0 = typeReference.GetFriendlyTypeName(null, "<", ">");
			if (V_0.Contains("<"))
			{
				V_0 = V_0.Substring(0, V_0.IndexOf('<'));
			}
			return V_0;
		}

		protected virtual string GetFriendlyNameByType(TypeReference typeReference)
		{
			return this.Camelize(this.GetFriendlyGenericName(typeReference), 1);
		}

		private string GetNameByType(TypeReference typeReference)
		{
			V_0 = typeReference.Resolve();
			if (V_0 != null && V_0.HasCompilerGeneratedAttribute())
			{
				return "variable";
			}
			if (!this.ShouldBePluralized(typeReference))
			{
				return this.GetFriendlyNameByType(typeReference);
			}
			if (!typeReference.get_IsGenericInstance())
			{
				if (typeReference.get_GenericParameters().get_Count() != 0)
				{
					return this.Camelize(this.GetFriendlyGenericName(typeReference), 1);
				}
				V_5 = typeReference.GetFriendlyTypeName(null, "<", ">");
				return this.Camelize(V_5, 2);
			}
			V_1 = (GenericInstanceType)typeReference;
			V_2 = V_1.get_GenericArguments().get_Item(0);
			if (V_1.get_PostionToArgument().ContainsKey(0))
			{
				V_2 = V_1.get_PostionToArgument().get_Item(0);
			}
			V_0 = V_2.Resolve();
			if (V_0 != null && V_0.HasCompilerGeneratedAttribute())
			{
				return "collection";
			}
			return this.Camelize(this.GetFriendlyGenericName(V_2), 2);
		}

		private string GetNameWithSuffix(string name, int suffix)
		{
			V_0 = name;
			if (suffix >= 1)
			{
				V_0 = String.Concat(V_0, suffix.ToString());
			}
			return V_0;
		}

		private string GetSafeBaseTypeName(string name, string suffix)
		{
			V_0 = name.ToLower();
			if (V_0 != null)
			{
				if (String.op_Equality(V_0, "decimal") || String.op_Equality(V_0, "float") || String.op_Equality(V_0, "byte") || String.op_Equality(V_0, "sbyte") || String.op_Equality(V_0, "short") || String.op_Equality(V_0, "int") || String.op_Equality(V_0, "long") || String.op_Equality(V_0, "ushort") || String.op_Equality(V_0, "uint") || String.op_Equality(V_0, "ulong") || String.op_Equality(V_0, "double") || String.op_Equality(V_0, "int16") || String.op_Equality(V_0, "int32") || String.op_Equality(V_0, "int64") || String.op_Equality(V_0, "uint16") || String.op_Equality(V_0, "uint32") || String.op_Equality(V_0, "uint64"))
				{
					return String.Concat("num", suffix);
				}
				if (String.op_Equality(V_0, "char"))
				{
					return String.Concat("chr", suffix);
				}
				if (String.op_Equality(V_0, "boolean"))
				{
					return String.Concat("flag", suffix);
				}
				if (String.op_Equality(V_0, "bool"))
				{
					return String.Concat("flag", suffix);
				}
				if (String.op_Equality(V_0, "string"))
				{
					return String.Concat("str", suffix);
				}
				if (String.op_Equality(V_0, "object"))
				{
					return String.Concat("obj", suffix);
				}
				if (String.op_Equality(V_0, "int32 modopt(system.runtime.compilerservices.islong)"))
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
			return this.GetSafeTypeName(name.Substring(0, name.get_Length() - 1), suffix);
		}

		private bool HasArgumentWithSameName(string name)
		{
			V_0 = this.methodContext.get_ParameterDefinitionToNameMap().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (this.context.get_Language().get_IdentifierComparer().Compare(V_1.get_Value(), name) != 0)
					{
						continue;
					}
					V_2 = true;
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
		Label1:
			return V_2;
		Label0:
			return false;
		}

		private bool HasGenericParameterWithSameName(string name)
		{
			if (!this.methodContext.get_Method().get_HasGenericParameters())
			{
				return false;
			}
			V_0 = this.methodContext.get_Method().get_GenericParameters().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (this.context.get_Language().get_IdentifierComparer().Compare(V_1.GetFriendlyFullName(this.context.get_Language()), name) != 0)
					{
						continue;
					}
					V_2 = true;
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				V_0.Dispose();
			}
		Label1:
			return V_2;
		Label0:
			return false;
		}

		private bool HasMethodParameterWithSameName(string name)
		{
			V_0 = this.methodContext.get_Method().get_Parameters().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (this.context.get_Language().get_IdentifierComparer().Compare(V_1.get_Name(), name) != 0)
					{
						continue;
					}
					V_2 = true;
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				V_0.Dispose();
			}
		Label1:
			return V_2;
		Label0:
			return false;
		}

		private bool HasVariableWithSameName(string name, VariableDefinition variable)
		{
			V_0 = this.methodContext.get_VariableDefinitionToNameMap().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if ((object)V_1.get_Key() == (object)variable || this.context.get_Language().get_IdentifierComparer().Compare(V_1.get_Value(), name) != 0)
					{
						continue;
					}
					V_2 = true;
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
		Label1:
			return V_2;
		Label0:
			return false;
		}

		protected bool IsCollection(TypeReference typeReference)
		{
			V_0 = typeReference.Resolve();
			if (V_0 != null)
			{
				V_1 = V_0.get_Interfaces().GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_2 = V_1.get_Current();
						if (!String.op_Equality(V_2.get_FullName(), "System.Collections.IEnumerable") && !String.op_Equality(V_2.get_FullName(), "System.Collections.IList") && !String.op_Equality(V_2.get_FullName(), "System.Collections.ICollection"))
						{
							continue;
						}
						V_3 = true;
						goto Label1;
					}
					goto Label0;
				}
				finally
				{
					V_1.Dispose();
				}
			Label1:
				return V_3;
			}
		Label0:
			return false;
		}

		private bool IsForInitializerSuggestedForVariable(VariableDefinition variable)
		{
			if (!this.variableSuggestedNames.TryGetValue(variable, out V_0))
			{
				return false;
			}
			return V_0.get_IsForInitializerSuggested();
		}

		private bool IsTempVariable(VariableReference variable)
		{
			V_0 = this.methodContext.get_VariableDefinitionToNameMap().get_Item(variable.Resolve());
			if (!V_0.StartsWith("CS$") && !V_0.StartsWith("VB$") && V_0.IndexOf("__init") <= 0 && !V_0.StartsWith("stackVariable") && !V_0.StartsWith("exception_"))
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
			V_0 = 0;
			V_1 = new Dictionary<ParameterDefinition, string>();
			V_2 = this.methodContext.get_ParameterDefinitionToNameMap().GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					V_4 = V_3.get_Key();
					V_5 = V_3.get_Value();
					V_6 = V_5;
					V_7 = !V_6.IsValidIdentifier();
					while (V_7 || this.HasMethodParameterWithSameName(V_6))
					{
						V_7 = false;
						stackVariable22 = V_0;
						V_0 = stackVariable22 + 1;
						V_8 = stackVariable22;
						V_6 = String.Concat("argument", V_8.ToString());
					}
					if (!String.op_Inequality(V_6, V_5))
					{
						continue;
					}
					V_1.Add(V_4, V_6);
				}
			}
			finally
			{
				((IDisposable)V_2).Dispose();
			}
			V_2 = V_1.GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_9 = V_2.get_Current();
					this.methodContext.get_ParameterDefinitionToNameMap().set_Item(V_9.get_Key(), V_9.get_Value());
				}
			}
			finally
			{
				((IDisposable)V_2).Dispose();
			}
			V_10 = this.methodContext.get_Body().get_Method().get_Parameters().GetEnumerator();
			try
			{
				while (V_10.MoveNext())
				{
					V_11 = V_10.get_Current();
					V_12 = V_11.get_Name();
					if (String.IsNullOrEmpty(V_12))
					{
						V_12 = this.GetNameByType(V_11.get_ParameterType());
					}
					this.methodContext.get_ParameterDefinitionToNameMap().Add(V_11, V_12);
				}
			}
			finally
			{
				V_10.Dispose();
			}
			if (this.methodContext.get_Method().get_IsSetter() && this.methodContext.get_Method().get_Parameters().get_Count() == 1)
			{
				V_13 = this.methodContext.get_Method().get_Parameters().get_Item(0);
				this.methodContext.get_ParameterDefinitionToNameMap().set_Item(V_13, "value");
			}
			V_14 = this.methodContext.get_Body().get_Variables().GetEnumerator();
			try
			{
				while (V_14.MoveNext())
				{
					V_15 = V_14.get_Current();
					if (!this.methodContext.get_ParameterDefinitionToNameMap().ContainsValue(V_15.get_Name()))
					{
						continue;
					}
					dummyVar0 = this.methodContext.get_VariablesToRename().Add(V_15);
				}
			}
			finally
			{
				V_14.Dispose();
			}
			V_16 = this.methodContext.get_VariableDefinitionToNameMap().GetEnumerator();
			try
			{
				while (V_16.MoveNext())
				{
					V_17 = V_16.get_Current();
					if (!this.methodContext.get_ParameterDefinitionToNameMap().ContainsValue(V_17.get_Value()))
					{
						continue;
					}
					dummyVar1 = this.methodContext.get_VariablesToRename().Add(V_17.get_Key());
				}
			}
			finally
			{
				((IDisposable)V_16).Dispose();
			}
			return;
		}

		public virtual BlockStatement Process(DecompilationContext context, BlockStatement block)
		{
			this.context = context;
			this.methodContext = context.get_MethodContext();
			this.typeContext = context.get_TypeContext();
			this.Preprocess();
			this.VisitBlockStatement(block);
			this.ReplaceDeclarations(block);
			this.CollectVariableNames();
			return block;
		}

		private void ProcessVariableDeclaration(VariableDeclarationExpression node)
		{
			if (!this.methodContext.get_VariableDefinitionToNameMap().ContainsKey(node.get_Variable()))
			{
				this.methodContext.get_VariableDefinitionToNameMap().Add(node.get_Variable(), node.get_Variable().get_Name());
			}
			if (this.state == RenameVariables.State.SearchForPossibleNames)
			{
				this.SuggestNameForVariable(node.get_Variable());
				return;
			}
			if (this.state == 1)
			{
				this.TryRenameVariable(node.get_Variable());
			}
			return;
		}

		private void ReplaceDeclarations(BlockStatement block)
		{
			this.state = 1;
			this.VisitBlockStatement(block);
			this.state = 0;
			this.variableSuggestedNames.Clear();
			this.forInitializerStartSymbol = 105;
			this.forInitializerNumberSuffix = 0;
			return;
		}

		private bool ShouldBePluralized(TypeReference typeReference)
		{
			if (String.op_Equality(typeReference.get_FullName(), "System.String"))
			{
				return false;
			}
			if (typeReference.get_IsPrimitive())
			{
				return false;
			}
			return this.IsCollection(typeReference);
		}

		private void SuggestNameForMethod(RenameVariables.ExpressionKind expressionKind, string methodName, int startIndex)
		{
			V_0 = 0;
			if (expressionKind == 3)
			{
				V_0 = 1;
			}
			methodName = methodName.Substring(startIndex);
			methodName = this.Camelize(methodName, V_0);
			this.SuggestNameForThePendingVariable(methodName);
			return;
		}

		private void SuggestNameForThePendingVariable(string name)
		{
			V_0 = this.variableSuggestedNames.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (!V_1.get_Value().get_IsPendingForSuggestion())
					{
						continue;
					}
					this.AddNewSuggestion(V_1.get_Value().get_SuggestedNames(), name);
					V_1.get_Value().set_IsPendingForSuggestion(false);
					goto Label0;
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
		Label0:
			return;
		}

		protected virtual void SuggestNameForVariable(VariableDefinition variableDefinition)
		{
			if (this.state == RenameVariables.State.SearchForPossibleNames)
			{
				V_0 = this.GetExpressionKind();
				if (V_0 == 1 && !this.IsForInitializerSuggestedForVariable(variableDefinition))
				{
					this.TryAddNewSuggestionForVariable(variableDefinition, this.GetForInitializerName()).set_IsForInitializerSuggested(true);
				}
				if (V_0 == 2 || V_0 == 4)
				{
					this.TrySetPendingForSuggestion(variableDefinition);
					V_1 = null;
					dummyVar0 = this.variableSuggestedNames.TryGetValue(variableDefinition, out V_1);
				}
				if (V_0 == 3)
				{
					if (this.variableSuggestedNames.TryGetValue(variableDefinition, out V_2) && V_2.get_SuggestedNames().get_Count() > 0)
					{
						this.TrySetPendingName(this.GetFirstSuggestedName(V_2.get_SuggestedNames()), true);
						return;
					}
					this.ClearPendingForSuggestion();
				}
			}
			return;
		}

		private RenameVariables.VariableSuggestion TryAddNewSuggestionForVariable(VariableDefinition variable, string name)
		{
			if (!this.variableSuggestedNames.TryGetValue(variable, out V_0))
			{
				V_0 = new RenameVariables.VariableSuggestion(name);
				this.variableSuggestedNames.Add(variable, V_0);
			}
			else
			{
				this.AddNewSuggestion(V_0.get_SuggestedNames(), name);
			}
			return V_0;
		}

		private bool TryNameByGetMethod(string methodName, RenameVariables.ExpressionKind expressionKind)
		{
			if (!methodName.ToLower().StartsWith("get"))
			{
				return false;
			}
			if (methodName.get_Length() > 3)
			{
				if (Char.IsUpper(methodName.get_Chars(3)))
				{
					if (methodName.get_Length() == 3)
					{
						return false;
					}
					this.SuggestNameForMethod(expressionKind, methodName, 3);
					return true;
				}
				if (methodName.get_Chars(3) == '\u005F')
				{
					if (methodName.get_Length() == 4)
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
			V_0 = methodName.IndexOf("To", 0);
			while (V_0 >= 0)
			{
				if (V_0 + 2 < methodName.get_Length() && Char.IsUpper(methodName.get_Chars(V_0 + 2)))
				{
					this.SuggestNameForMethod(expressionKind, methodName, V_0 + 2);
					return true;
				}
				V_0 = methodName.IndexOf("To", V_0 + 1);
			}
			return false;
		}

		private void TryRenameUsingSuggestions(VariableDefinition variable, RenameVariables.VariableSuggestion variableSuggestion)
		{
			V_0 = 0;
			while (true)
			{
				V_2 = variableSuggestion.get_SuggestedNames().GetEnumerator();
				try
				{
					while (V_2.MoveNext())
					{
						V_3 = V_2.get_Current();
						if (!this.TryRenameVariable(variable, V_3, V_0))
						{
							continue;
						}
						goto Label1;
					}
				}
				finally
				{
					if (V_2 != null)
					{
						V_2.Dispose();
					}
				}
				if (this.TryRenameVariable(variable, this.GetNameByType(variable.get_VariableType()), V_0))
				{
					goto Label0;
				}
				V_0 = V_0 + 1;
			}
		Label1:
			return;
		Label0:
			return;
		}

		protected virtual void TryRenameVariable(VariableDefinition variable)
		{
			if (this.state == 1)
			{
				if (!this.variableSuggestedNames.TryGetValue(variable, out V_0))
				{
					V_0 = new RenameVariables.VariableSuggestion();
					V_0.set_IsRenamed(true);
					this.TryRenameUsingSuggestions(variable, V_0);
				}
				else
				{
					if (!V_0.get_IsRenamed())
					{
						V_0.set_IsRenamed(true);
						this.TryRenameUsingSuggestions(variable, V_0);
						return;
					}
				}
			}
			return;
		}

		private bool TryRenameVariable(VariableDefinition variable, string name, int suffix)
		{
			if (!this.IsTempVariable(variable) && !this.methodContext.get_VariablesToRename().Contains(variable))
			{
				return true;
			}
			V_0 = this.GetNameWithSuffix(name, suffix);
			if (this.suggestedNames.Contains(V_0))
			{
				return false;
			}
			V_1 = V_0;
			if (!this.context.get_Language().IsValidIdentifier(V_0))
			{
				V_1 = this.context.get_Language().ReplaceInvalidCharactersInIdentifier(V_0);
			}
			V_1 = this.EscapeIfGlobalKeyword(V_1);
			if (!this.IsValidNameInContext(V_1, variable))
			{
				return false;
			}
			this.methodContext.get_VariableDefinitionToNameMap().set_Item(variable, V_1);
			dummyVar0 = this.methodContext.get_VariablesToRename().Remove(variable);
			dummyVar1 = this.suggestedNames.Add(V_1);
			return true;
		}

		private void TrySetMethodInvocationPendingName(Expression methodExpression)
		{
			if (this.state != RenameVariables.State.SearchForPossibleNames)
			{
				return;
			}
			V_0 = this.GetExpressionKind();
			if (V_0 != 5 && V_0 != 3)
			{
				return;
			}
			if (methodExpression as MethodReferenceExpression == null)
			{
				return;
			}
			V_1 = ((MethodReferenceExpression)methodExpression).get_Method();
			stackVariable12 = V_1.get_Name();
			if (stackVariable12 == null)
			{
				dummyVar0 = stackVariable12;
				stackVariable12 = V_1.get_FullName();
			}
			V_2 = stackVariable12;
			if (V_2.Contains("`"))
			{
				V_2 = V_2.Substring(0, V_2.IndexOf('\u0060'));
			}
			if (this.TryNameByGetMethod(V_2, V_0))
			{
				return;
			}
			dummyVar1 = this.TryNameByToMethod(V_2, V_0);
			return;
		}

		private void TrySetObjectCreationPendingName(TypeReference typeReference)
		{
			if (this.state != RenameVariables.State.SearchForPossibleNames)
			{
				return;
			}
			if (this.GetExpressionKind() == 5)
			{
				this.SuggestNameForThePendingVariable(this.GetNameByType(typeReference));
			}
			return;
		}

		private void TrySetPendingForSuggestion(VariableDefinition variable)
		{
			if (this.variableSuggestedNames.TryGetValue(variable, out V_0))
			{
				V_0.set_IsPendingForSuggestion(true);
				return;
			}
			V_0 = new RenameVariables.VariableSuggestion();
			V_0.set_IsPendingForSuggestion(true);
			this.variableSuggestedNames.Add(variable, V_0);
			return;
		}

		private void TrySetPendingName(string name, bool suggestSameName)
		{
			if (this.state != RenameVariables.State.SearchForPossibleNames)
			{
				return;
			}
			V_0 = this.GetExpressionKind();
			if (V_0 == 3 || V_0 == 5)
			{
				if (V_0 == 3)
				{
					stackVariable8 = 1;
				}
				else
				{
					stackVariable8 = 0;
				}
				V_1 = stackVariable8;
				if (String.IsNullOrEmpty(name))
				{
					this.ClearPendingForSuggestion();
					return;
				}
				V_2 = this.Camelize(name, V_1);
				if (String.op_Inequality(V_2, String.Concat("_", name)))
				{
					if (suggestSameName || String.op_Inequality(name, V_2))
					{
						this.SuggestNameForThePendingVariable(V_2);
					}
					this.ClearPendingForSuggestion();
					return;
				}
				if (!suggestSameName)
				{
					this.ClearPendingForSuggestion();
				}
			}
			return;
		}

		public override void VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpression node)
		{
			if (node.get_Constructor() != null)
			{
				this.TrySetObjectCreationPendingName(node.get_Constructor().get_DeclaringType());
			}
			this.ClearPendingForSuggestion();
			this.VisitAnonymousObjectCreationExpression(node);
			return;
		}

		public override void VisitArgumentReferenceExpression(ArgumentReferenceExpression node)
		{
			this.TrySetPendingName(node.get_Parameter().get_Name(), false);
			this.VisitArgumentReferenceExpression(node);
			return;
		}

		public override void VisitArrayCreationExpression(ArrayCreationExpression node)
		{
			this.expressions.Push(0);
			this.Visit(node.get_Dimensions());
			dummyVar0 = this.expressions.Pop();
			this.Visit(node.get_Initializer());
			return;
		}

		public override void VisitArrayIndexerExpression(ArrayIndexerExpression node)
		{
			this.Visit(node.get_Target());
			this.expressions.Push(0);
			this.Visit(node.get_Indices());
			dummyVar0 = this.expressions.Pop();
			return;
		}

		public override void VisitArrayLengthExpression(ArrayLengthExpression node)
		{
			this.TrySetPendingName("Length", true);
			this.VisitArrayLengthExpression(node);
			return;
		}

		private ICodeNode VisitAssignExpression(BinaryExpression node)
		{
			this.expressions.Push(4);
			this.Visit(node.get_Left());
			dummyVar0 = this.expressions.Pop();
			this.expressions.Push(5);
			this.Visit(node.get_Right());
			dummyVar1 = this.expressions.Pop();
			this.ClearPendingForSuggestion();
			return node;
		}

		public override void VisitBinaryExpression(BinaryExpression node)
		{
			if (!node.get_IsAssignmentExpression() && !node.get_IsSelfAssign() || node.get_IsEventHandlerAddOrRemove())
			{
				this.VisitBinaryExpression(node);
				return;
			}
			dummyVar0 = this.VisitAssignExpression(node);
			return;
		}

		public override void VisitConditionExpression(ConditionExpression node)
		{
			this.expressions.Push(0);
			this.VisitConditionExpression(node);
			dummyVar0 = this.expressions.Pop();
			return;
		}

		public override void VisitFieldReferenceExpression(FieldReferenceExpression node)
		{
			if (node.get_Target() as TypeReferenceExpression != null)
			{
				V_2 = ((TypeReferenceExpression)node.get_Target()).get_Type();
				if (V_2 as TypeDefinition != null && ((TypeDefinition)V_2).get_IsEnum())
				{
					this.VisitFieldReferenceExpression(node);
					return;
				}
			}
			V_0 = node.get_Field().Resolve();
			if (V_0 == null || !this.typeContext.get_BackingFieldToNameMap().ContainsKey(V_0))
			{
				V_1 = node.get_Field().get_Name();
			}
			else
			{
				V_1 = this.typeContext.get_BackingFieldToNameMap().get_Item(V_0);
			}
			this.TrySetPendingName(V_1, true);
			this.VisitFieldReferenceExpression(node);
			return;
		}

		public override void VisitForEachStatement(ForEachStatement node)
		{
			this.expressions.Push(2);
			this.Visit(node.get_Variable());
			dummyVar0 = this.expressions.Pop();
			this.expressions.Push(3);
			this.Visit(node.get_Collection());
			dummyVar1 = this.expressions.Pop();
			this.ClearPendingForSuggestion();
			this.Visit(node.get_Body());
			return;
		}

		public override void VisitForStatement(ForStatement node)
		{
			this.expressions.Push(1);
			this.Visit(node.get_Initializer());
			dummyVar0 = this.expressions.Pop();
			this.Visit(node.get_Condition());
			this.Visit(node.get_Increment());
			this.Visit(node.get_Body());
			return;
		}

		public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			this.TrySetMethodInvocationPendingName(node.get_MethodExpression());
			this.ClearPendingForSuggestion();
			this.VisitMethodInvocationExpression(node);
			return;
		}

		public override void VisitObjectCreationExpression(ObjectCreationExpression node)
		{
			if (node.get_Constructor() != null)
			{
				this.TrySetObjectCreationPendingName(node.get_Constructor().get_DeclaringType());
			}
			this.ClearPendingForSuggestion();
			this.VisitObjectCreationExpression(node);
			return;
		}

		public override void VisitPropertyReferenceExpression(PropertyReferenceExpression node)
		{
			this.TrySetPendingName(node.get_Property().get_Name(), true);
			this.VisitPropertyReferenceExpression(node);
			return;
		}

		public override void VisitRefVariableDeclarationExpression(RefVariableDeclarationExpression node)
		{
			this.ProcessVariableDeclaration(node);
			this.VisitRefVariableDeclarationExpression(node);
			return;
		}

		public override void VisitVariableDeclarationExpression(VariableDeclarationExpression node)
		{
			this.ProcessVariableDeclaration(node);
			this.VisitVariableDeclarationExpression(node);
			return;
		}

		public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			V_0 = node.get_Variable().Resolve();
			if (!this.methodContext.get_VariableDefinitionToNameMap().ContainsKey(V_0))
			{
				this.methodContext.get_VariableDefinitionToNameMap().set_Item(V_0, V_0.get_Name());
			}
			if (this.state == RenameVariables.State.SearchForPossibleNames)
			{
				this.SuggestNameForVariable(V_0);
			}
			if (this.state == 1 && this.methodContext.get_UndeclaredLinqVariables().Remove(V_0))
			{
				this.TryRenameVariable(V_0);
			}
			this.VisitVariableReferenceExpression(node);
			return;
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
				base();
				this.set_SuggestedNames(new List<string>());
				return;
			}

			public VariableSuggestion(string suggestedName)
			{
				this();
				this.get_SuggestedNames().Add(suggestedName);
				return;
			}
		}
	}
}