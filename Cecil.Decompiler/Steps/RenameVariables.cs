#region license
//
//	(C) 2007 - 2008 Novell, Inc. http://www.novell.com
//	(C) 2007 - 2008 Jb Evain http://evain.net
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion
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
    public class RenameVariables : BaseCodeVisitor, IDecompilationStep
    {
        protected State state = State.SearchForPossibleNames;
		protected readonly Stack<ExpressionKind> expressions = new Stack<ExpressionKind>();
		protected readonly Dictionary<VariableDefinition, VariableSuggestion> variableSuggestedNames = new Dictionary<VariableDefinition, VariableSuggestion>();
		protected readonly HashSet<string> suggestedNames;// = new HashSet<string>();
        protected DecompilationContext context;
		protected MethodSpecificContext methodContext;
		protected TypeSpecificContext typeContext;
        private int forInitializerStartSymbol = 105; // That's Utf32 code for the 'i' character
		private int forInitializerNumberSuffix = 0;

        public RenameVariables()
        {
            this.suggestedNames = new HashSet<string>();
        }

        public virtual BlockStatement Process(DecompilationContext context, BlockStatement block)
        {
            this.context = context;
            this.methodContext = context.MethodContext;
            this.typeContext = context.TypeContext;
            //this.suggestedNames.UnionWith(methodContext.UsedNames);
            //this.suggestedNames.UnionWith(context.VariableNames);
            Preprocess();
            VisitBlockStatement(block);
            ReplaceDeclarations(block);
            //this.methodContext.UsedNames.UnionWith(suggestedNames);
			CollectVariableNames();
            return block;
        }

        private void Preprocess()
        {
            int argumentIndex = 0;
            Dictionary<ParameterDefinition, string> changedParameterNames = new Dictionary<ParameterDefinition, string>();
            foreach (KeyValuePair<ParameterDefinition, string> parameterToNamePair in this.methodContext.ParameterDefinitionToNameMap)
            {
                ParameterDefinition parameterDef = parameterToNamePair.Key;
                string parameterOldName = parameterToNamePair.Value;

                string parameterNewName = parameterOldName;
                bool invalidIdentifier = !parameterNewName.IsValidIdentifier();
                while (invalidIdentifier /*|| HasArgumentWithSameName(parameterNewName, parameterDef)*/ || HasMethodParameterWithSameName(parameterNewName))
                {
                    invalidIdentifier = false;
                    parameterNewName = "argument" + argumentIndex++;
                }

                if (parameterNewName != parameterOldName)
                {
                    changedParameterNames.Add(parameterDef, parameterNewName);
                }
            }

            foreach (KeyValuePair<ParameterDefinition, string> parameterToNewNamePair in changedParameterNames)
            {
                this.methodContext.ParameterDefinitionToNameMap[parameterToNewNamePair.Key] = parameterToNewNamePair.Value;
            }

            foreach (ParameterDefinition parameter in this.methodContext.Body.Method.Parameters)
            {
                string parameterName = parameter.Name;
                if (string.IsNullOrEmpty(parameterName))
                {
                    parameterName = GetNameByType(parameter.ParameterType);
                }
                this.methodContext.ParameterDefinitionToNameMap.Add(parameter, parameterName);
            }

            if (this.methodContext.Method.IsSetter && this.methodContext.Method.Parameters.Count == 1)
            {
                ParameterDefinition setterValueParameter = this.methodContext.Method.Parameters[0];
                this.methodContext.ParameterDefinitionToNameMap[setterValueParameter] = "value";
            }

            foreach (VariableDefinition variable in this.methodContext.Body.Variables)
            {
                if (this.methodContext.ParameterDefinitionToNameMap.ContainsValue(variable.Name))
                {
                    this.methodContext.VariablesToRename.Add(variable);
                }
            }

            foreach (KeyValuePair<VariableDefinition, string> pair in methodContext.VariableDefinitionToNameMap)
            {
                if (this.methodContext.ParameterDefinitionToNameMap.ContainsValue(pair.Value))
                {
                    this.methodContext.VariablesToRename.Add(pair.Key);
                }
            }
        }

        private bool HasMethodParameterWithSameName(string name)
        {
            foreach (ParameterDefinition parameter in this.methodContext.Method.Parameters)
            {
                if (this.context.Language.IdentifierComparer.Compare(parameter.Name, name) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        private ExpressionKind GetExpressionKind()
        {
            ExpressionKind expressionKind = ExpressionKind.None;

            foreach (var expression in expressions)
            {
                if (expression == ExpressionKind.None)
                {
                    return ExpressionKind.None;
                }
                if (expression == ExpressionKind.RightAssignment)
                {
                    return ExpressionKind.RightAssignment;
                }
                if (expression == ExpressionKind.ForInitializer)
                {
                    if (expressionKind == ExpressionKind.LeftAssignment)
                    {
                        expressionKind = ExpressionKind.ForInitializer;
                    }
                }
                if (expressionKind == ExpressionKind.None)
                {
                    expressionKind = expression;
                }
            }
            return expressionKind;
        }

        private string GetForInitializerName()
        {
            string initializerName = char.ConvertFromUtf32(forInitializerStartSymbol);
            if (forInitializerNumberSuffix > 0)
            {
                initializerName += forInitializerNumberSuffix;
            }
            forInitializerStartSymbol++;
            // Check for reaching 'z'
            if (forInitializerStartSymbol == 122)
            {
                forInitializerStartSymbol = 97; // That's Utf32 code for the 'a' character
            }
            // Check for reaching 'i'
            if (forInitializerStartSymbol == 105)
            {
                forInitializerNumberSuffix++;
            }
            return initializerName;
        }

        public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
        {
            VariableDefinition variable = node.Variable.Resolve();
            if (!methodContext.VariableDefinitionToNameMap.ContainsKey(variable))
            {
                methodContext.VariableDefinitionToNameMap[variable] = variable.Name;
            }

            if (state == State.SearchForPossibleNames)
            {
                SuggestNameForVariable(variable);
            }

            if (state == State.RenameVariables && methodContext.UndeclaredLinqVariables.Remove(variable))
            {
                TryRenameVariable(variable);
            }

            base.VisitVariableReferenceExpression(node);
        }

        protected virtual void SuggestNameForVariable(VariableDefinition variableDefinition)
        {
            if (state == State.SearchForPossibleNames)
            {
                var expressionKind = GetExpressionKind();
                if ((expressionKind == ExpressionKind.ForInitializer) && (!IsForInitializerSuggestedForVariable(variableDefinition)))
                {
                    var variableSuggestion = TryAddNewSuggestionForVariable(variableDefinition, GetForInitializerName());
                    variableSuggestion.IsForInitializerSuggested = true;
                }
                if ((expressionKind == ExpressionKind.ForEachVariable) ||
                    (expressionKind == ExpressionKind.LeftAssignment))
                {
                    TrySetPendingForSuggestion(variableDefinition);

                    VariableSuggestion suggestion = null;
                    variableSuggestedNames.TryGetValue(variableDefinition, out suggestion);
                }
                if (expressionKind == ExpressionKind.ForEachExpression)
                {
                    VariableSuggestion variableSuggestion;
                    if (variableSuggestedNames.TryGetValue(variableDefinition, out variableSuggestion))
                    {
                        if (variableSuggestion.SuggestedNames.Count > 0)
                        {
                            TrySetPendingName(GetFirstSuggestedName(variableSuggestion.SuggestedNames), true);
                            return;
                        }
                    }
                    ClearPendingForSuggestion();
                }
            }
        }

        private string GetFirstSuggestedName(ICollection<string> suggestedNames)
        {
            foreach (var suggestedName in suggestedNames)
            {
                return suggestedName;
            }
            return string.Empty;
        }

        private bool IsForInitializerSuggestedForVariable(VariableDefinition variable)
        {
            VariableSuggestion variableSuggestion;
            if (variableSuggestedNames.TryGetValue(variable, out variableSuggestion))
            {
                return variableSuggestion.IsForInitializerSuggested;
            }
            return false;
        }

        protected virtual void TryRenameVariable(VariableDefinition variable)
        {
            if (state == State.RenameVariables)
            {
                VariableSuggestion variableSuggestion;
                if (variableSuggestedNames.TryGetValue(variable, out variableSuggestion))
                {
                    if (!variableSuggestion.IsRenamed)
                    {
                        variableSuggestion.IsRenamed = true;
                        TryRenameUsingSuggestions(variable, variableSuggestion);
                    }
                }
                else
                {
                    variableSuggestion = new VariableSuggestion();
                    variableSuggestion.IsRenamed = true;
                    TryRenameUsingSuggestions(variable, variableSuggestion);
                }
            }
        }

        private void TryRenameUsingSuggestions(VariableDefinition variable, VariableSuggestion variableSuggestion)
        {
            for (int suffix = 0; ; suffix++)
            {
                foreach (var name in variableSuggestion.SuggestedNames)
                {
                    if (TryRenameVariable(variable, name, suffix))
                    {
                        return;
                    }
                }
				string nameByType = GetNameByType(variable.VariableType);
				if (TryRenameVariable(variable, nameByType, suffix))
				{
                    return;
                }
            }
        }

        private bool TryRenameVariable(VariableDefinition variable, string name, int suffix)
		{
            bool isTempVariable = IsTempVariable(variable);
            if (!isTempVariable && !this.methodContext.VariablesToRename.Contains(variable))
            {
                return true;
            }

            string nameWithSuffix = GetNameWithSuffix(name, suffix);
            if (!suggestedNames.Contains(nameWithSuffix))
            {
                string escapedKeywordName = nameWithSuffix;
                if (!this.context.Language.IsValidIdentifier(nameWithSuffix))
                {
                    escapedKeywordName = this.context.Language.ReplaceInvalidCharactersInIdentifier(nameWithSuffix);
                }
                escapedKeywordName = EscapeIfGlobalKeyword(escapedKeywordName);
				if (!IsValidNameInContext(escapedKeywordName, variable))
				{
                    return false;
                }

                methodContext.VariableDefinitionToNameMap[variable] = escapedKeywordName;
                this.methodContext.VariablesToRename.Remove(variable);

                suggestedNames.Add(escapedKeywordName);
                return true;
            }
            return false;
        }

		private string GetNameWithSuffix(string name, int suffix)
		{
			string nameWithSuffix = name;
			if (suffix >= 1)
			{
				nameWithSuffix += suffix;
			}

			return nameWithSuffix;
		}

        private string EscapeIfGlobalKeyword(string name)
        {
            return this.context.Language.IsGlobalKeyword(name) ? this.context.Language.EscapeWord(name) : name;
        }

		protected virtual bool IsValidNameInContext(string name, VariableDefinition variable)
		{
			return !HasGenericParameterWithSameName(name) &&
				   !HasArgumentWithSameName(name) &&
				   !HasVariableWithSameName(name, variable);
		}

		private bool HasGenericParameterWithSameName(string name)
		{
			if (!this.methodContext.Method.HasGenericParameters)
			{
				return false;
			}

			foreach (GenericParameter parameter in this.methodContext.Method.GenericParameters)
			{
				if (this.context.Language.IdentifierComparer.Compare(parameter.GetFriendlyFullName(this.context.Language), name) == 0)
				{
					return true;
				}
			}
			return false;
		}

		private bool HasArgumentWithSameName(string name)
        {
            foreach (KeyValuePair<ParameterDefinition, string> pair in this.methodContext.ParameterDefinitionToNameMap)
            {
                if (this.context.Language.IdentifierComparer.Compare(pair.Value, name) == 0)
                {
                    return true;
                }
            }

            return false;
        }

		private bool HasVariableWithSameName(string name, VariableDefinition variable)
        {
            foreach (KeyValuePair<VariableDefinition, string> pair in this.methodContext.VariableDefinitionToNameMap)
            {
                if (pair.Key != variable && this.context.Language.IdentifierComparer.Compare(pair.Value, name) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsTempVariable(VariableReference variable)
        {
            var name = methodContext.VariableDefinitionToNameMap[variable.Resolve()];

            if (name.StartsWith("CS$") ||
				name.StartsWith("VB$") ||
				name.IndexOf("__init") > 0 ||
				name.StartsWith("stackVariable") ||
				name.StartsWith("exception_"))
            {
                return true;
            }
            return false;
        }

        private void TrySetPendingForSuggestion(VariableDefinition variable)
        {
            VariableSuggestion variableSuggestion;
            if (variableSuggestedNames.TryGetValue(variable, out variableSuggestion))
            {
                variableSuggestion.IsPendingForSuggestion = true;
            }
            else
            {
                variableSuggestion = new VariableSuggestion();
                variableSuggestion.IsPendingForSuggestion = true;
                variableSuggestedNames.Add(variable, variableSuggestion);
            }
        }

        private VariableSuggestion TryAddNewSuggestionForVariable(VariableDefinition variable, string name)
        {
            VariableSuggestion variableSuggestion;
            if (variableSuggestedNames.TryGetValue(variable, out variableSuggestion))
            {
                AddNewSuggestion(variableSuggestion.SuggestedNames, name);
            }
            else
            {
                variableSuggestion = new VariableSuggestion(name);
                variableSuggestedNames.Add(variable, variableSuggestion);
            }
            return variableSuggestion;
        }

		private void AddNewSuggestion(ICollection<string> suggestedNames, string name)
		{
			if (IsValidNameInContext(name, null))
			{
				suggestedNames.Add(name);
			}
		}

        public override void VisitVariableDeclarationExpression(VariableDeclarationExpression node)
        {
            ProcessVariableDeclaration(node);

            base.VisitVariableDeclarationExpression(node);
        }

        public override void VisitRefVariableDeclarationExpression(RefVariableDeclarationExpression node)
        {
            ProcessVariableDeclaration(node);
            
            base.VisitRefVariableDeclarationExpression(node);
        }

        private void ProcessVariableDeclaration(VariableDeclarationExpression node)
        {
            if (!methodContext.VariableDefinitionToNameMap.ContainsKey(node.Variable))
            {
                methodContext.VariableDefinitionToNameMap.Add(node.Variable, node.Variable.Name);
            }

            if (state == State.SearchForPossibleNames)
            {
                SuggestNameForVariable(node.Variable);
            }
            else if (state == State.RenameVariables)
            {
                TryRenameVariable(node.Variable);
            }
        }

        public override void VisitForStatement(ForStatement node)
        {
            expressions.Push(ExpressionKind.ForInitializer);
            Visit(node.Initializer);
            expressions.Pop();
            Visit(node.Condition);
            Visit(node.Increment);
            Visit(node.Body);
        }

        public override void VisitForEachStatement(ForEachStatement node)
        {
            expressions.Push(ExpressionKind.ForEachVariable);
            Visit(node.Variable);
            expressions.Pop();
            expressions.Push(ExpressionKind.ForEachExpression);
            Visit(node.Collection);
            expressions.Pop();
            ClearPendingForSuggestion();
            Visit(node.Body);
        }

        private void ClearPendingForSuggestion()
        {
            foreach (var keyValuePair in variableSuggestedNames)
            {
                keyValuePair.Value.IsPendingForSuggestion = false;
            }
        }

        private void SuggestNameForThePendingVariable(string name)
        {
            foreach (var keyValuePair in variableSuggestedNames)
            {
                if (keyValuePair.Value.IsPendingForSuggestion)
                {
                    AddNewSuggestion(keyValuePair.Value.SuggestedNames, name);
                    keyValuePair.Value.IsPendingForSuggestion = false;
                    return;
                }
            }
        }

        public override void VisitBinaryExpression(BinaryExpression node)
        {
			if (node.IsAssignmentExpression || (node.IsSelfAssign && !node.IsEventHandlerAddOrRemove))
			{
				VisitAssignExpression(node);
			}
			else
			{
				base.VisitBinaryExpression(node);
			}
        }

        private ICodeNode VisitAssignExpression(BinaryExpression node)
        {
            expressions.Push(ExpressionKind.LeftAssignment);
            Visit(node.Left);
            expressions.Pop();
            expressions.Push(ExpressionKind.RightAssignment);
            Visit(node.Right);
            expressions.Pop();
            ClearPendingForSuggestion();
            return node;
        }

        public override void VisitConditionExpression(ConditionExpression node)
        {
            expressions.Push(ExpressionKind.None);
            base.VisitConditionExpression(node);
            expressions.Pop();
        }

        private string GetSafeTypeName(string name, string suffix)
        {
            if (name.Contains("[") && name.Contains("]"))
            {
                if (!suffix.Contains("Array"))
                {
                    suffix += "Array";
                }
                return GetSafeTypeName(name.Substring(0, name.IndexOf("[")), suffix);
            }
            if (name.EndsWith("*") || name.EndsWith("&"))
            {
                if (!suffix.Contains("Pointer"))
                {
                    suffix += "Pointer";
                }
                return GetSafeTypeName(name.Substring(0, name.Length - 1), suffix);
            }
            if (name.Contains("`"))
            {
                return name.Substring(0, name.IndexOf("`")) + suffix;
            }
            return GetSafeBaseTypeName(name, suffix);
        }

        private string GetSafeBaseTypeName(string name, string suffix)
        {
            switch (name.ToLower())
            {
                case "decimal":
                case "float":
                case "byte":
                case "sbyte":
                case "short":
                case "int":
                case "long":
                case "ushort":
                case "uint":
                case "ulong":
                case "double":
                case "int16":
                case "int32":
                case "int64":
                case "uint16":
                case "uint32":
                case "uint64":
                    return "num" + suffix;
                case "char":
                    return "chr" + suffix;
                case "boolean":
                    return "flag" + suffix;
                case "bool":
                    return "flag" + suffix;
                case "string":
                    return "str" + suffix;
                case "object":
                    return "obj" + suffix;
                case "int32 modopt(system.runtime.compilerservices.islong)":
                    return "intPtr";
                default:
                    return name + suffix;
            }
        }

        public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
        {
            TrySetMethodInvocationPendingName(node.MethodExpression);
            ClearPendingForSuggestion();
            base.VisitMethodInvocationExpression(node);
        }

        public override void VisitObjectCreationExpression(ObjectCreationExpression node)
        {
            if (node.Constructor != null)
            {
                TrySetObjectCreationPendingName(node.Constructor.DeclaringType);
            }
            ClearPendingForSuggestion();
            base.VisitObjectCreationExpression(node);
        }

		public override void VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpression node)
		{
			if (node.Constructor != null)
			{
				TrySetObjectCreationPendingName(node.Constructor.DeclaringType);
			}
			ClearPendingForSuggestion();

			base.VisitAnonymousObjectCreationExpression(node);
		}

		public override void VisitArrayLengthExpression(ArrayLengthExpression node)
        {
            TrySetPendingName("Length", true);
            base.VisitArrayLengthExpression(node);
        }

		public override void VisitPropertyReferenceExpression(PropertyReferenceExpression node)
        {
            TrySetPendingName(node.Property.Name, true);
            base.VisitPropertyReferenceExpression(node);
        }

		public override void VisitArrayIndexerExpression(ArrayIndexerExpression node)
        {
            Visit(node.Target);
            expressions.Push(ExpressionKind.None);
            Visit(node.Indices);
            expressions.Pop();
        }

		public override void VisitArrayCreationExpression(ArrayCreationExpression node)
        {
            expressions.Push(ExpressionKind.None);
            Visit(node.Dimensions);
            expressions.Pop();
            Visit(node.Initializer);
        }

		public override void VisitFieldReferenceExpression(FieldReferenceExpression node)
        {
            if (node.Target is TypeReferenceExpression)
            {
                var typeReference = ((TypeReferenceExpression)node.Target).Type;
                if (typeReference is TypeDefinition)
                {
                    var typeDefinition = (TypeDefinition)typeReference;
                    if (typeDefinition.IsEnum)
                    {
                        base.VisitFieldReferenceExpression(node);
						return;
                    }
                }
            }

            FieldDefinition field = node.Field.Resolve();
            string fieldName;
            if (field != null && this.typeContext.BackingFieldToNameMap.ContainsKey(field))
            {
                fieldName = this.typeContext.BackingFieldToNameMap[field];
            }
            else
            {
                fieldName = node.Field.Name;
            }

            TrySetPendingName(fieldName, true);

            base.VisitFieldReferenceExpression(node);
        }

		public override void VisitArgumentReferenceExpression(ArgumentReferenceExpression node)
        {
            TrySetPendingName(node.Parameter.Name, false);
            base.VisitArgumentReferenceExpression(node);
        }

        private void TrySetObjectCreationPendingName(TypeReference typeReference)
        {
            if (state != State.SearchForPossibleNames)
                return;

            var expressionKind = GetExpressionKind();
            if (expressionKind == ExpressionKind.RightAssignment)
            {
                string name = GetNameByType(typeReference);
                SuggestNameForThePendingVariable(name);
            }
        }

        private void TrySetMethodInvocationPendingName(Expression methodExpression)
        {
            if (state != State.SearchForPossibleNames)
                return;

            var expressionKind = GetExpressionKind();
            if ((expressionKind != ExpressionKind.RightAssignment) &&
                (expressionKind != ExpressionKind.ForEachExpression))
                return;

            if (!(methodExpression is MethodReferenceExpression))
                return;

            var methodReference = ((MethodReferenceExpression)methodExpression).Method;
            var methodName = methodReference.Name ?? methodReference.FullName;
            if (methodName.Contains("`"))
            {
                methodName = methodName.Substring(0, methodName.IndexOf('`'));
            }
            if (TryNameByGetMethod(methodName, expressionKind))
                return;

            TryNameByToMethod(methodName, expressionKind);
        }

        private bool TryNameByGetMethod(string methodName, ExpressionKind expressionKind)
        {
            if (!methodName.ToLower().StartsWith("get"))
                return false;

            if (methodName.Length > 3)
            {
                if (char.IsUpper(methodName[3]))
                {
                    if (methodName.Length == 3)
                    {
                        return false;
                    }
                    SuggestNameForMethod(expressionKind, methodName, 3);
                    return true;
                }
                else if (methodName[3] == '_')
                {
                    if (methodName.Length == 4)
                    {
                        return false;
                    }
                    SuggestNameForMethod(expressionKind, methodName, 4);
                    return true;
                }
            }
            return false;
        }

        private void SuggestNameForMethod(ExpressionKind expressionKind, string methodName, int startIndex)
        {
            var comparison = Conversion.None;
            if (expressionKind == ExpressionKind.ForEachExpression)
            {
                comparison = Conversion.Singular;
            }
            methodName = methodName.Substring(startIndex);
            methodName = Camelize(methodName, comparison);

            //If methodName is system C#/VB word it should be renamed in some way
            //at the moment "default" is the only word that created problems due to
            //"get_Default" method call at {System.Collections.Generic.EqualityComparer`1<!0> System.Collections.Generic.EqualityComparer`1<T>::get_Default()}
            //the name default was assigned to the variable that takes the result
            //Make the name be escaped in the writer.
            //if (methodName == "default")
            //{
            //    methodName = methodName + "_";
            //    while (suggestedNames.Contains(methodName))
            //    {
            //        methodName = methodName + "_";
            //    }
            //}
            SuggestNameForThePendingVariable(methodName);
        }

        private bool TryNameByToMethod(string methodName, ExpressionKind expressionKind)
        {
            int index = methodName.IndexOf("To", 0);

            while (index >= 0)
            {
                if ((index + 2 < methodName.Length) && (char.IsUpper(methodName[index + 2])))
                {
                    SuggestNameForMethod(expressionKind, methodName, index + 2);
                    return true;
                }
                index = methodName.IndexOf("To", index + 1);
            }
            return false;
        }

        private string GetNameByType(TypeReference typeReference)
        {
            TypeDefinition typeDef = typeReference.Resolve();
            if (typeDef != null && typeDef.HasCompilerGeneratedAttribute())
            {
                return "variable";
            }

            if (ShouldBePluralized(typeReference))
            {
				if (typeReference.IsGenericInstance)
                {
                    GenericInstanceType genericInstance = (GenericInstanceType)typeReference;

                    TypeReference variableType = genericInstance.GenericArguments[0];
                    //TypeReference variableType;
                    if (genericInstance.PostionToArgument.ContainsKey(0))
                    {
                        variableType = genericInstance.PostionToArgument[0];
                    }
                    typeDef = variableType.Resolve();
                    if (typeDef != null && typeDef.HasCompilerGeneratedAttribute())
                    {
                        return "collection";
                    }

					string name = GetFriendlyGenericName(variableType);
                    return Camelize(name, Conversion.Plural);
                }
				else if (typeReference.GenericParameters.Count != 0) // cover TypeDefinitions of generic types.
                {
                    string name = GetFriendlyGenericName(typeReference);
                    return Camelize(name, Conversion.Singular);
                }
                else
				{
					string name = typeReference.GetFriendlyTypeName(null);
					return Camelize(name, Conversion.Plural);
				}
            }
            return GetFriendlyNameByType(typeReference);
        }

        protected virtual string GetFriendlyNameByType(TypeReference typeReference)
        {
            string name = GetFriendlyGenericName(typeReference);
            name = Camelize(name, Conversion.Singular);
			return name;
        }

        private string GetFriendlyGenericName(TypeReference typeReference)
        {
            string name = typeReference.GetFriendlyTypeName(null);
            if (name.Contains("<"))
            {
                name = name.Substring(0, name.IndexOf('<'));
			}
            return name;
        }

        private bool ShouldBePluralized(TypeReference typeReference)
        {
            if (typeReference.FullName == "System.String")
                return false;

            if (typeReference.IsPrimitive)
                return false;

            return IsCollection(typeReference);
        }

		protected bool IsCollection(TypeReference typeReference)
        {
            var typeDefinition = typeReference.Resolve();
            if (typeDefinition != null)
            {
                foreach (var implementedInterface in typeDefinition.Interfaces)
                {
                    if ((implementedInterface.FullName == "System.Collections.IEnumerable") ||
                        (implementedInterface.FullName == "System.Collections.IList") ||
                        (implementedInterface.FullName == "System.Collections.ICollection"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void TrySetPendingName(string name, bool suggestSameName)
        {
            if (state != State.SearchForPossibleNames)
                return;

            var expressionKind = GetExpressionKind();
            if ((expressionKind == ExpressionKind.ForEachExpression) ||
                (expressionKind == ExpressionKind.RightAssignment))
            {
                var conversion = (expressionKind == ExpressionKind.ForEachExpression) ? Conversion.Singular : Conversion.None;
                if (string.IsNullOrEmpty(name))
                {
                    ClearPendingForSuggestion();
                    return;
                }
                string camelizedName = Camelize(name, conversion);
                if (camelizedName != "_" + name)
                {
                    if (suggestSameName || (name != camelizedName))
                    {
                        SuggestNameForThePendingVariable(camelizedName);
                    }
                    ClearPendingForSuggestion();
                }
                else if (!suggestSameName)
                {
                    ClearPendingForSuggestion();
                }
            }
        }

        private void ReplaceDeclarations(BlockStatement block)
        {
            state = State.RenameVariables;
            VisitBlockStatement(block);
            state = State.SearchForPossibleNames;
            variableSuggestedNames.Clear();
            //suggestedNames.Clear();
            forInitializerStartSymbol = 105;
            forInitializerNumberSuffix = 0;
        }

		private void CollectVariableNames()
		{
			HashSet<string> result = new HashSet<string>(this.methodContext.VariableNamesCollection, this.context.Language.IdentifierComparer);

			foreach (VariableDefinition variable in this.methodContext.Body.Variables)
			{
				string variableName;
				if (!this.methodContext.VariableDefinitionToNameMap.TryGetValue(variable, out variableName))
				{
					variableName = variable.Name;
				}

				if (!result.Contains(variableName))
				{
					result.Add(variableName);
				}
			}

			foreach (string variableName in this.methodContext.VariableDefinitionToNameMap.Values)
			{
				if (!result.Contains(variableName))
				{
					result.Add(variableName);
				}
			}

			this.methodContext.VariableNamesCollection = result;
		}

		protected string Camelize(string name, Conversion conversion)
        {
            var camelizedName = name;
            if (name.StartsWith("I") && (name.Length > 1) && (char.IsUpper(name[1])))
            {
                camelizedName = name.Substring(1);
            }
            camelizedName = Inflector.Camelize(GetSafeTypeName(camelizedName, string.Empty));
            if (conversion == Conversion.Singular)
            {
                camelizedName = Inflector.Singularize(camelizedName);
            }
            if (conversion == Conversion.Plural)
            {
                camelizedName = Inflector.Pluralize(camelizedName);
            }
			if (camelizedName == name)
			{
				camelizedName = "_" + name;
			}
            return camelizedName;
        }

		protected enum State
        {
            SearchForPossibleNames,
            RenameVariables
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

		protected enum Conversion
        {
            None,
            Singular,
            Plural
        }

        protected class VariableSuggestion
        {
            public VariableSuggestion()
            {
                this.SuggestedNames = new List<string>();
            }

            public VariableSuggestion(string suggestedName)
                : this()
            {
                SuggestedNames.Add(suggestedName);
            }

            public ICollection<string> SuggestedNames { get; private set; }

            public bool IsPendingForSuggestion { get; set; }

            public bool IsRenamed { get; set; }

            public bool IsForInitializerSuggested { get; set; }
        }
    }
}