/*
    Copyright CodeMerx 2024
    This file is part of CodemerxDecompile.

    CodemerxDecompile is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    CodemerxDecompile is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with CodemerxDecompile.  If not, see<https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CodemerxDecompile.SearchResults;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Languages.CSharp;

namespace CodemerxDecompile;

public class SearchService
{
    private CancellationTokenSource? searchOperationCancellationTokenSource;

    public IEnumerable<SearchResult> Search(IEnumerable<string> openedAssembliesFilePaths, string query,
        bool matchCasing = false, bool matchWholeWord = false)
    {
        using (searchOperationCancellationTokenSource = new CancellationTokenSource())
        {
            var token = searchOperationCancellationTokenSource.Token;
            
            foreach (var searchResult in SearchInternal(openedAssembliesFilePaths, query, matchCasing, matchWholeWord))
            {
                if (token.IsCancellationRequested)
                {
                    searchOperationCancellationTokenSource = null;
                    yield break;
                }

                yield return searchResult;
            }
        }

        searchOperationCancellationTokenSource = null;
    }
    
    private IEnumerable<SearchResult> SearchInternal(IEnumerable<string> openedAssembliesFilePaths, string query, bool matchCasing = false, bool matchWholeWord = false)
    {
        foreach (var assemblyFilePath in openedAssembliesFilePaths)
        {
            var assembly = GlobalAssemblyResolver.Instance.GetAssemblyDefinition(assemblyFilePath);

            IEnumerable<TypeDefinition> types = assembly.Modules.SelectMany(m => m.GetTypes()).Where(t => !t.IsCompilerGenerated());

            foreach (var type in types)
            {
                if (DoesMatchSearchCriteria(query, type, matchCasing, matchWholeWord))
                {
                    yield return new TypeNameSearchResult
                    {
                        DeclaringType = type,
                        MatchedString = type.Name,
                        TypeDefinition = type
                    };
                }

                IEnumerable<IMemberDefinition> members = type.GetMembersSorted(false, LanguageFactory.GetLanguage(CSharpVersion.V7));

                if (members.Count() == 0)
                {
                    continue;
                }

                foreach (var member in members)
                {
                    if (member is EventDefinition eventDefinition)
                    {
                        if (DoesMatchSearchCriteria(query, eventDefinition.Name, matchCasing, matchWholeWord))
                        {
                            yield return new EventNameSearchResult
                            {
                                DeclaringType = type,
                                MatchedString = eventDefinition.Name,
                                EventDefinition = eventDefinition
                            };
                        }
                        
                        if (DoesMatchSearchCriteria(query, eventDefinition.EventType, matchCasing, matchWholeWord))
                        {
                            yield return new EventTypeSearchResult
                            {
                                DeclaringType = type,
                                MatchedString = GetFriendlyName(eventDefinition.EventType),
                                EventDefinition = eventDefinition
                            };
                        }
                    }
                    else if (member is FieldDefinition fieldDefinition)
                    {
                        if (DoesMatchSearchCriteria(query, fieldDefinition.Name, matchCasing, matchWholeWord))
                        {
                            yield return new FieldNameSearchResult
                            {
                                DeclaringType = type,
                                MatchedString = fieldDefinition.Name,
                                FieldDefinition = fieldDefinition
                            };
                        }
                        
                        if (!fieldDefinition.DeclaringType.IsEnum && DoesMatchSearchCriteria(query, fieldDefinition.FieldType, matchCasing, matchWholeWord))
                        {
                            yield return new FieldTypeSearchResult
                            {
                                DeclaringType = type,
                                MatchedString = GetFriendlyName(fieldDefinition.FieldType),
                                FieldDefinition = fieldDefinition
                            };
                        }
                    }
                    else if (member is PropertyDefinition propertyDefinition)
                    {
                        if (DoesMatchSearchCriteria(query, propertyDefinition.Name, matchCasing, matchWholeWord))
                        {
                            yield return new PropertyNameSearchResult
                            {
                                DeclaringType = type,
                                MatchedString = propertyDefinition.Name,
                                PropertyDefinition = propertyDefinition
                            };
                        }
                        
                        if (DoesMatchSearchCriteria(query, propertyDefinition.PropertyType, matchCasing, matchWholeWord))
                        {
                            yield return new PropertyTypeSearchResult
                            {
                                DeclaringType = type,
                                MatchedString = GetFriendlyName(propertyDefinition.PropertyType),
                                PropertyDefinition = propertyDefinition
                            };
                        }
                    }
                    else if (member is MethodDefinition methodDefinition)
                    {
                        if (DoesMatchSearchCriteria(query, methodDefinition.ReturnType, matchCasing, matchWholeWord))
                        {
                            yield return new MethodReturnTypeSearchResult
                            {
                                DeclaringType = type,
                                MatchedString = GetFriendlyName(methodDefinition.ReturnType),
                                MethodDefinition = methodDefinition
                            };
                        }

                        if (methodDefinition.HasBody && methodDefinition.Body.HasVariables)
                        {
                            foreach (var variable in methodDefinition.Body.Variables)
                            {
                                if (DoesMatchSearchCriteria(query, variable.Name, matchCasing, matchWholeWord))
                                {
                                    yield return new VariableNameSearchResult
                                    {
                                        DeclaringType = type,
                                        MatchedString = variable.Name,
                                        VariableDefinition = variable
                                    };
                                }
                                else if (DoesMatchSearchCriteria(query, variable.VariableType, matchCasing, matchWholeWord))
                                {
                                    yield return new VariableTypeSearchResult
                                    {
                                        DeclaringType = type,
                                        MatchedString = GetFriendlyName(variable.VariableType),
                                        VariableDefinition = variable
                                    };
                                }
                            }
                        }

                        if (methodDefinition.HasParameters)
                        {
                            foreach (var parameter in methodDefinition.Parameters)
                            {
                                if (DoesMatchSearchCriteria(query, parameter.Name, matchCasing, matchWholeWord))
                                {
                                    yield return new ParameterNameSearchResult
                                    {
                                        DeclaringType = type,
                                        MatchedString = parameter.Name,
                                        MethodDefinition = methodDefinition,
                                        ParameterDefinition = parameter
                                    };
                                }
                                else if (DoesMatchSearchCriteria(query, parameter.ParameterType, matchCasing, matchWholeWord))
                                {
                                    yield return new ParameterTypeSearchResult
                                    {
                                        DeclaringType = type,
                                        MatchedString = GetFriendlyName(parameter.ParameterType),
                                        ParameterDefinition = parameter
                                    };
                                }
                            }
                        }
                    }

                    IEnumerable<Instruction> instructions = member.GetInstructions();

                    foreach (var instruction in instructions)
                    {
                        var operand = instruction.Operand;
                        var shouldCheckDeclaringTypeName = false;

                        if (operand is MemberReference memberReference)
                        {
                            if (memberReference.DeclaringType != null)
                            {
                                var declaringTypeDefinition = memberReference.DeclaringType.Resolve();

                                if (declaringTypeDefinition != null && declaringTypeDefinition.IsCompilerGenerated())
                                {
                                    continue;
                                }

                                if (operand is EventReference eventReference)
                                {
                                    var resolvedEventReference = eventReference.Resolve();

                                    if (resolvedEventReference != null)
                                    {
                                        shouldCheckDeclaringTypeName = resolvedEventReference.IsStatic();
                                    }
                                }
                                else if (operand is FieldReference fieldReference)
                                {
                                    var resolvedFieldReference = fieldReference.Resolve();

                                    if (resolvedFieldReference != null)
                                    {
                                        shouldCheckDeclaringTypeName = resolvedFieldReference.IsStatic;
                                    }
                                }
                                else if (operand is PropertyReference propertyReference)
                                {
                                    var resolvedPropertyReference = propertyReference.Resolve();

                                    if (resolvedPropertyReference != null)
                                    {
                                        shouldCheckDeclaringTypeName = resolvedPropertyReference.IsStatic();
                                    }
                                }
                                else if (operand is MethodReference methodReference)
                                {
                                    var resolvedMethodReference = methodReference.Resolve();

                                    if (resolvedMethodReference != null)
                                    {
                                        shouldCheckDeclaringTypeName = resolvedMethodReference.IsStatic || resolvedMethodReference.IsConstructor;
                                    }
                                }

                                if (shouldCheckDeclaringTypeName && DoesMatchSearchCriteria(query, memberReference.DeclaringType, matchCasing, matchWholeWord))
                                {
                                    yield return new InstructionSearchResult
                                    {
                                        DeclaringType = type,
                                        MatchedString = GetFriendlyName(memberReference.DeclaringType),
                                        Instruction = instruction
                                    };
                                }
                            }

                            var memberReferenceName = memberReference.Name;

                            if (memberReference is TypeReference typeReference)
                            {
                                memberReferenceName = GetFriendlyName(typeReference);
                            }
                            else if (memberReference is MethodReference methodReference && TryGetFriendlyPropertyName(methodReference, out var friendlyPropertyName))
                            {
                                memberReferenceName = friendlyPropertyName;
                            }

                            if (DoesMatchSearchCriteria(query, memberReferenceName, matchCasing, matchWholeWord))
                            {
                                yield return new InstructionSearchResult
                                {
                                    DeclaringType = type,
                                    MatchedString = memberReferenceName,
                                    Instruction = instruction
                                };
                            }
                        }
                        else if (operand is VariableReference variableReference && this.DoesMatchSearchCriteria(query, variableReference.Name, matchCasing, matchWholeWord))
                        {
                            yield return new InstructionSearchResult
                            {
                                DeclaringType = type,
                                MatchedString = variableReference.Name,
                                Instruction = instruction
                            };
                        }
                        else if (operand is ParameterReference parameterReference && this.DoesMatchSearchCriteria(query, parameterReference.Name, matchCasing, matchWholeWord))
                        {
                            yield return new InstructionSearchResult
                            {
                                DeclaringType = type,
                                MatchedString = parameterReference.Name,
                                Instruction = instruction
                            };
                        }
                        else if (operand is string stringLiteral && this.DoesMatchSearchCriteria(query, stringLiteral, matchCasing, matchWholeWord))
                        {
                            yield return new StringLiteralSearchResult
                            {
                                DeclaringType = type,
                                MatchedString = stringLiteral,
                                Instruction = instruction
                            };
                        }
                    }
                }
            }
        }
    }

    public void CancelSearch()
    {
        searchOperationCancellationTokenSource?.Cancel();
    }

    public CodeSpan? GetSearchResultPosition(SearchResult searchResult, DecompiledTypeMetadata typeMetadata)
    {
        CodeSpan codeSpan = default;
    
        switch (searchResult)
        {
            case ParameterNameSearchResult parameterNameSearchResult:
            {
                var methodDefinition = parameterNameSearchResult.MethodDefinition;
                var parameterIndex = methodDefinition.Parameters.IndexOf(parameterNameSearchResult.ParameterDefinition);

                if (parameterIndex != -1)
                {
                    typeMetadata.CodeMappingInfo.TryGetValue(methodDefinition, parameterIndex, out codeSpan);
                }

                break;
            }
            case EventTypeSearchResult eventTypeSearchResult:
                typeMetadata.CodeMappingInfo.EventDefinitionToEventTypeCodeMap.TryGetValue(eventTypeSearchResult.EventDefinition, out codeSpan);
                break;
            case PropertyTypeSearchResult propertyTypeSearchResult:
                typeMetadata.CodeMappingInfo.PropertyDefinitionToPropertyTypeCodeMap.TryGetValue(propertyTypeSearchResult.PropertyDefinition, out codeSpan);
                break;
            case FieldTypeSearchResult fieldTypeSearchResult:
                typeMetadata.CodeMappingInfo.FieldDefinitionToFieldTypeCodeMap.TryGetValue(fieldTypeSearchResult.FieldDefinition, out codeSpan);
                break;
            case MethodReturnTypeSearchResult methodReturnTypeSearchResult:
                typeMetadata.CodeMappingInfo.MethodDefinitionToMethodReturnTypeCodeMap.TryGetValue(methodReturnTypeSearchResult.MethodDefinition, out codeSpan);
                break;
            case ParameterTypeSearchResult parameterTypeSearchResult:
                typeMetadata.CodeMappingInfo.ParameterDefinitionToParameterTypeCodeMap.TryGetValue(parameterTypeSearchResult.ParameterDefinition, out codeSpan);
                break;
            case VariableTypeSearchResult variableTypeSearchResult:
            {
                var variableDefinition = variableTypeSearchResult.VariableDefinition;
                if (!typeMetadata.CodeMappingInfo.VariableDefinitionToVariableTypeCodeMap.TryGetValue(variableDefinition, out codeSpan))
                {
                    typeMetadata.MemberDeclarationToCodeSpan.TryGetValue(variableDefinition.ContainingMethod, out codeSpan);
                }
                break;
            }
            case TypeNameSearchResult typeNameSearchResult:
                typeMetadata.MemberDeclarationToCodeSpan.TryGetValue(typeNameSearchResult.TypeDefinition, out codeSpan);
                break;
            case EventNameSearchResult eventNameSearchResult:
                typeMetadata.MemberDeclarationToCodeSpan.TryGetValue(eventNameSearchResult.EventDefinition, out codeSpan);
                break;
            case PropertyNameSearchResult propertyNameSearchResult:
                typeMetadata.MemberDeclarationToCodeSpan.TryGetValue(propertyNameSearchResult.PropertyDefinition, out codeSpan);
                break;
            case FieldNameSearchResult fieldNameSearchResult:
                typeMetadata.MemberDeclarationToCodeSpan.TryGetValue(fieldNameSearchResult.FieldDefinition, out codeSpan);
                break;
            case MethodNameSearchResult methodNameSearchResult:
                typeMetadata.MemberDeclarationToCodeSpan.TryGetValue(methodNameSearchResult.MethodDefinition, out codeSpan);
                break;
            case VariableNameSearchResult variableNameSearchResult:
                typeMetadata.CodeMappingInfo.TryGetValue(variableNameSearchResult.VariableDefinition, out codeSpan);
                break;
            case InstructionSearchResult instructionSearchResult:
                {
                    var instruction = instructionSearchResult.Instruction;
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
        var methodDefinition = methodReference.Resolve();

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
        return DoesMatchSearchCriteria(query, GetFriendlyName(typeReference), matchCasing, matchWholeWord);
    }

    private bool DoesMatchSearchCriteria(string query, string entityName, bool matchCasing, bool matchWholeWord)
    {
        var stringComparisonType = matchCasing ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;

        return matchWholeWord ? entityName.Equals(query, stringComparisonType) : entityName.Contains(query, stringComparisonType);
    }
}
