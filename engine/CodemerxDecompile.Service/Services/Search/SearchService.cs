﻿//    Copyright CodeMerx 2020
//    This file is part of CodemerxDecompile.

//    CodemerxDecompile is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.

//    CodemerxDecompile is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//    GNU Affero General Public License for more details.

//    You should have received a copy of the GNU Affero General Public License
//    along with CodemerxDecompile.  If not, see<https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

using Mono.Cecil.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;

using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Languages.CSharp;
using CodemerxDecompile.Service.Services.Search.Models;
using CodemerxDecompile.Service.Interfaces;
using CodemerxDecompile.Service.Services.DecompilationContext.Models;

namespace CodemerxDecompile.Service.Services.Search
{
    public class SearchService : ISearchService
    {
        private readonly IDecompilationContextService decompilationContext;
        private Dictionary<int, SearchResult> cachedSearchResults;
        private CancellationTokenSource searchOperationCancellationTokenSource;

        public SearchService(IDecompilationContextService decompilationContext)
        {
            this.decompilationContext = decompilationContext;
        }

        public IEnumerable<SearchResult> Search(string query, bool matchCasing = false, bool matchWholeWord = false)
        {
            this.cachedSearchResults = new Dictionary<int, SearchResult>();

            using (this.searchOperationCancellationTokenSource = new CancellationTokenSource())
            {
                CancellationToken token = this.searchOperationCancellationTokenSource.Token;

                IEnumerable<string> openedAssembliesFilePaths = this.decompilationContext.GetOpenedAssemliesPaths();

                foreach (string assemblyFilePath in openedAssembliesFilePaths)
                {
                    AssemblyDefinition assembly = GlobalAssemblyResolver.Instance.GetAssemblyDefinition(assemblyFilePath);

                    IEnumerable<TypeDefinition> types = assembly.Modules.SelectMany(m => m.GetTypes()).Where(t => !t.IsCompilerGenerated());

                    foreach (TypeDefinition type in types)
                    {
                        if (token.IsCancellationRequested)
                        {
                            this.searchOperationCancellationTokenSource = null;
                            yield break;
                        }

                        if (this.DoesMatchSearchCriteria(query, type, matchCasing, matchWholeWord))
                        {
                            yield return this.AddSearchResultToCache(SearchResultType.DeclaringType, type, type.Name, type);
                        }

                        IEnumerable<IMemberDefinition> members = type.GetMembersSorted(false, LanguageFactory.GetLanguage(CSharpVersion.V7));

                        if (members.Count() == 0)
                        {
                            continue;
                        }

                        foreach (IMemberDefinition member in members)
                        {
                            if (token.IsCancellationRequested)
                            {
                                this.searchOperationCancellationTokenSource = null;
                                yield break;
                            }

                            if (this.DoesMatchSearchCriteria(query, member.Name, matchCasing, matchWholeWord))
                            {
                                SearchResultType memberSearchResultType = this.GetSearchResultTypeFromMemberDefinitionType(member);

                                // Skip adding nested types when traversing the type members as they are added when traversing the module types
                                if (memberSearchResultType != SearchResultType.DeclaringType)
                                {
                                    yield return this.AddSearchResultToCache(memberSearchResultType, type, member.Name, member);
                                }
                            }

                            if (member is EventDefinition eventDefinition && this.DoesMatchSearchCriteria(query, eventDefinition.EventType, matchCasing, matchWholeWord))
                            {
                                yield return this.AddSearchResultToCache(SearchResultType.EventType, type, this.GetFriendlyName(eventDefinition.EventType), eventDefinition);
                            }
                            else if (member is FieldDefinition fieldDefinition && !fieldDefinition.DeclaringType.IsEnum && this.DoesMatchSearchCriteria(query, fieldDefinition.FieldType, matchCasing, matchWholeWord))
                            {
                                yield return this.AddSearchResultToCache(SearchResultType.FieldType, type, this.GetFriendlyName(fieldDefinition.FieldType), fieldDefinition);
                            }
                            else if (member is PropertyDefinition propertyDefinition && this.DoesMatchSearchCriteria(query, propertyDefinition.PropertyType, matchCasing, matchWholeWord))
                            {
                                yield return this.AddSearchResultToCache(SearchResultType.PropertyType, type, this.GetFriendlyName(propertyDefinition.PropertyType), propertyDefinition);
                            }
                            else if (member is MethodDefinition methodDefinition)
                            {
                                if (this.DoesMatchSearchCriteria(query, methodDefinition.ReturnType, matchCasing, matchWholeWord))
                                {
                                    yield return this.AddSearchResultToCache(SearchResultType.MethodReturnType, type, this.GetFriendlyName(methodDefinition.ReturnType), methodDefinition);
                                }

                                if (methodDefinition.HasBody && methodDefinition.Body.HasVariables)
                                {
                                    foreach (VariableDefinition variable in methodDefinition.Body.Variables)
                                    {
                                        if (this.DoesMatchSearchCriteria(query, variable.Name, matchCasing, matchWholeWord))
                                        {
                                            yield return this.AddSearchResultToCache(SearchResultType.VariableName, type, variable.Name, variable);
                                        }
                                        else if (this.DoesMatchSearchCriteria(query, variable.VariableType, matchCasing, matchWholeWord))
                                        {
                                            yield return this.AddSearchResultToCache(SearchResultType.VariableType, type, this.GetFriendlyName(variable.VariableType), variable);
                                        }
                                    }
                                }

                                if (methodDefinition.HasParameters)
                                {
                                    foreach (ParameterDefinition parameter in methodDefinition.Parameters)
                                    {
                                        if (this.DoesMatchSearchCriteria(query, parameter.Name, matchCasing, matchWholeWord))
                                        {
                                            yield return this.AddSearchResultToCache(SearchResultType.ParameterName, type, parameter.Name, methodDefinition);
                                        }
                                        else if (this.DoesMatchSearchCriteria(query, parameter.ParameterType, matchCasing, matchWholeWord))
                                        {
                                            yield return this.AddSearchResultToCache(SearchResultType.ParameterType, type, this.GetFriendlyName(parameter.ParameterType), parameter);
                                        }
                                    }
                                }
                            }

                            IEnumerable<Instruction> instructions = member.GetInstructions();

                            foreach (Instruction instruction in instructions)
                            {
                                object operand = instruction.Operand;
                                bool shouldCheckDeclaringTypeName = false;

                                if (operand is MemberReference memberReference)
                                {
                                    if (memberReference.DeclaringType != null)
                                    {
                                        TypeDefinition declaringTypeDefinition = memberReference.DeclaringType.Resolve();

                                        if (declaringTypeDefinition != null && declaringTypeDefinition.IsCompilerGenerated())
                                        {
                                            continue;
                                        }

                                        if (operand is EventReference eventReference)
                                        {
                                            EventDefinition resolvedEventReference = eventReference.Resolve();

                                            if (resolvedEventReference != null)
                                            {
                                                shouldCheckDeclaringTypeName = resolvedEventReference.IsStatic();
                                            }
                                        }
                                        else if (operand is FieldReference fieldReference)
                                        {
                                            FieldDefinition resolvedFieldReference = fieldReference.Resolve();

                                            if (resolvedFieldReference != null)
                                            {
                                                shouldCheckDeclaringTypeName = resolvedFieldReference.IsStatic;
                                            }
                                        }
                                        else if (operand is PropertyReference propertyReference)
                                        {
                                            PropertyDefinition resolvedPropertyReference = propertyReference.Resolve();

                                            if (resolvedPropertyReference != null)
                                            {
                                                shouldCheckDeclaringTypeName = resolvedPropertyReference.IsStatic();
                                            }
                                        }
                                        else if (operand is MethodReference methodReference)
                                        {
                                            MethodDefinition resolvedMethodReference = methodReference.Resolve();

                                            if (resolvedMethodReference != null)
                                            {
                                                shouldCheckDeclaringTypeName = resolvedMethodReference.IsStatic || resolvedMethodReference.IsConstructor;
                                            }
                                        }

                                        if (shouldCheckDeclaringTypeName && this.DoesMatchSearchCriteria(query, memberReference.DeclaringType, matchCasing, matchWholeWord))
                                        {
                                            yield return this.AddSearchResultToCache(SearchResultType.Instruction, type, this.GetFriendlyName(memberReference.DeclaringType), instruction);
                                        }
                                    }

                                    string memberReferenceName = memberReference.Name;

                                    if (memberReference is TypeReference typeReference)
                                    {
                                        memberReferenceName = this.GetFriendlyName(typeReference);
                                    }
                                    else if (memberReference is MethodReference methodReference && this.TryGetFriendlyPropertyName(methodReference, out string friendlyPropertyName))
                                    {
                                        memberReferenceName = friendlyPropertyName;
                                    }

                                    if (this.DoesMatchSearchCriteria(query, memberReferenceName, matchCasing, matchWholeWord))
                                    {
                                        yield return this.AddSearchResultToCache(SearchResultType.Instruction, type, memberReferenceName, instruction);
                                    }
                                }
                                else if (operand is VariableReference variableReference && this.DoesMatchSearchCriteria(query, variableReference.Name, matchCasing, matchWholeWord))
                                {
                                    yield return this.AddSearchResultToCache(SearchResultType.Instruction, type, variableReference.Name, instruction);
                                }
                                else if (operand is ParameterReference parameterReference && this.DoesMatchSearchCriteria(query, parameterReference.Name, matchCasing, matchWholeWord))
                                {
                                    yield return this.AddSearchResultToCache(SearchResultType.Instruction, type, parameterReference.Name, instruction);
                                }
                                else if (operand is string stringLiteral && this.DoesMatchSearchCriteria(query, stringLiteral, matchCasing, matchWholeWord))
                                {
                                    yield return this.AddSearchResultToCache(SearchResultType.Instruction, type, operand as string, instruction);
                                }
                            }
                        }
                    }
                }
            }

            this.searchOperationCancellationTokenSource = null;
        }

        public void CancelSearch()
        {
            this.searchOperationCancellationTokenSource?.Cancel();
        }

        public CodeSpan? GetSearchResultPosition(int searchResultIndex)
        {
            if (!this.cachedSearchResults.TryGetValue(searchResultIndex, out SearchResult searchResult) ||
                !this.decompilationContext.DecompilationContext.FilePathToType.TryGetValue(searchResult.DeclaringTypeFilePath, out TypeDefinition typeDefinition) ||
                !this.decompilationContext.TryGetTypeMetadataFromCache(typeDefinition, out DecompiledTypeMetadata typeMetadata))
            {
                return null;
            }

            CodeSpan codeSpan = default;

            switch (searchResult.Type)
            {
                case SearchResultType.ParameterName:
                    {
                        MethodDefinition methodDefinition = searchResult.ObjectReference as MethodDefinition;
                        int? parameterIndex = methodDefinition.Parameters.Select((p, i) => new { Item = p, Index = i })
                                                                         .FirstOrDefault(p => p.Item.Name == searchResult.MatchedString)?.Index;

                        if (parameterIndex.HasValue)
                        {
                            typeMetadata.CodeMappingInfo.TryGetValue((IMemberDefinition)searchResult.ObjectReference, parameterIndex.Value, out codeSpan);
                        }

                        break;
                    }
                case SearchResultType.EventType:
                    typeMetadata.CodeMappingInfo.EventDefinitionToEventTypeCodeMap.TryGetValue((IMemberDefinition)searchResult.ObjectReference, out codeSpan);
                    break;
                case SearchResultType.PropertyType:
                    typeMetadata.CodeMappingInfo.PropertyDefinitionToPropertyTypeCodeMap.TryGetValue((IMemberDefinition)searchResult.ObjectReference, out codeSpan);
                    break;
                case SearchResultType.FieldType:
                    typeMetadata.CodeMappingInfo.FieldDefinitionToFieldTypeCodeMap.TryGetValue((IMemberDefinition)searchResult.ObjectReference, out codeSpan);
                    break;
                case SearchResultType.MethodReturnType:
                    typeMetadata.CodeMappingInfo.MethodDefinitionToMethodReturnTypeCodeMap.TryGetValue((IMemberDefinition)searchResult.ObjectReference, out codeSpan);
                    break;
                case SearchResultType.ParameterType:
                    typeMetadata.CodeMappingInfo.ParameterDefinitionToParameterTypeCodeMap.TryGetValue((ParameterDefinition)searchResult.ObjectReference, out codeSpan);
                    break;
                case SearchResultType.VariableType:
                    {
                        VariableDefinition variableDefinition = searchResult.ObjectReference as VariableDefinition;
                        if (!typeMetadata.CodeMappingInfo.VariableDefinitionToVariableTypeCodeMap.TryGetValue(variableDefinition, out codeSpan))
                        {
                            typeMetadata.MemberDeclarationToCodeSpan.TryGetValue(variableDefinition.ContainingMethod, out codeSpan);
                        }
                    };
                    break;
                case SearchResultType.DeclaringType:
                case SearchResultType.EventName:
                case SearchResultType.PropertyName:
                case SearchResultType.FieldName:
                case SearchResultType.MethodName:
                    typeMetadata.MemberDeclarationToCodeSpan.TryGetValue((IMemberDefinition)searchResult.ObjectReference, out codeSpan);
                    break;
                case SearchResultType.VariableName:
                    typeMetadata.CodeMappingInfo.TryGetValue((VariableDefinition)searchResult.ObjectReference, out codeSpan);
                    break;
                case SearchResultType.Instruction:
                    {
                        Instruction instruction = searchResult.ObjectReference as Instruction;
                        if (!typeMetadata.CodeMappingInfo.TryGetValue(instruction, out codeSpan))
                        {
                            typeMetadata.MemberDeclarationToCodeSpan.TryGetValue(instruction.ContainingMethod, out codeSpan);
                        }
                    }
                    break;
                default:
                    return null;
            }

            return codeSpan;
        }

        private string GetFriendlyName(MemberReference memberReference) => memberReference.GetFriendlyFullName(LanguageFactory.GetLanguage(CSharpVersion.V7), false);

        private bool TryGetFriendlyPropertyName(MethodReference methodReference, out string friendlyPropertyName)
        {
            MethodDefinition methodDefinition = methodReference.Resolve();

            if (methodDefinition != null && (methodDefinition.IsGetter || methodDefinition.IsSetter) && (methodDefinition.Name.StartsWith("get_") || methodDefinition.Name.StartsWith("set_")))
            {
                // strip the "get_" or "set_" from method name
                friendlyPropertyName = methodDefinition.Name.Substring(4);
                return true;
            }

            friendlyPropertyName = null;
            return false;
        }

        private bool DoesMatchSearchCriteria(string query, TypeReference typeReference, bool matchCasing, bool matchWholeWord)
        {
            return this.DoesMatchSearchCriteria(query, this.GetFriendlyName(typeReference), matchCasing, matchWholeWord);
        }

        private bool DoesMatchSearchCriteria(string query, string entityName, bool matchCasing, bool matchWholeWord)
        {
            StringComparison stringComparisonType = matchCasing ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;

            return matchWholeWord ? entityName.Equals(query, stringComparisonType) : entityName.Contains(query, stringComparisonType);
        }

        private SearchResult AddSearchResultToCache(SearchResultType type, TypeDefinition declaringType, string matchedString, object objectReference)
        {
            if (this.decompilationContext.TryGetTypeFilePathFromCache(this.GetTopTypeDefinition(declaringType), out string typeFilePath))
            {
                int searchResultId = this.cachedSearchResults.Count + 1;
                SearchResult searchResult = new SearchResult(searchResultId, type, typeFilePath, matchedString, objectReference);

                this.cachedSearchResults[searchResultId] = searchResult;

                return searchResult;
            }
            else
            {
                throw new ArgumentException("Could not find type file path");
            }
        }

        private SearchResultType GetSearchResultTypeFromMemberDefinitionType(IMemberDefinition memberDefinition)
        {
            if (memberDefinition is EventDefinition)
            {
                return SearchResultType.EventName;
            }
            else if (memberDefinition is FieldDefinition)
            {
                return SearchResultType.FieldName;
            }
            else if (memberDefinition is MethodDefinition)
            {
                return SearchResultType.MethodName;
            }
            else if (memberDefinition is PropertyDefinition)
            {
                return SearchResultType.PropertyName;
            }
            else
            {
                return SearchResultType.DeclaringType;
            }
        }

        private TypeDefinition GetTopTypeDefinition(TypeDefinition typeDefinition)
        {
            TypeDefinition result = typeDefinition;

            while (result.DeclaringType != null)
            {
                result = result.DeclaringType;
            }

            return result;
        }
    }
}
