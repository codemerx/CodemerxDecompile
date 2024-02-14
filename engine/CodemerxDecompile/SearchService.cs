using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
                    yield return CreateSearchResult(SearchResultType.DeclaringType, type, type.Name, type);
                }

                IEnumerable<IMemberDefinition> members = type.GetMembersSorted(false, LanguageFactory.GetLanguage(CSharpVersion.V7));

                if (members.Count() == 0)
                {
                    continue;
                }

                foreach (var member in members)
                {
                    if (DoesMatchSearchCriteria(query, member.Name, matchCasing, matchWholeWord))
                    {
                        var memberSearchResultType = GetSearchResultTypeFromMemberDefinitionType(member);

                        // Skip adding nested types when traversing the type members as they are added when traversing the module types
                        if (memberSearchResultType != SearchResultType.DeclaringType)
                        {
                            yield return CreateSearchResult(memberSearchResultType, type, member.Name, member);
                        }
                    }

                    if (member is EventDefinition eventDefinition && DoesMatchSearchCriteria(query, eventDefinition.EventType, matchCasing, matchWholeWord))
                    {
                        yield return CreateSearchResult(SearchResultType.EventType, type, GetFriendlyName(eventDefinition.EventType), eventDefinition);
                    }
                    else if (member is FieldDefinition fieldDefinition && !fieldDefinition.DeclaringType.IsEnum && this.DoesMatchSearchCriteria(query, fieldDefinition.FieldType, matchCasing, matchWholeWord))
                    {
                        yield return CreateSearchResult(SearchResultType.FieldType, type, GetFriendlyName(fieldDefinition.FieldType), fieldDefinition);
                    }
                    else if (member is PropertyDefinition propertyDefinition && this.DoesMatchSearchCriteria(query, propertyDefinition.PropertyType, matchCasing, matchWholeWord))
                    {
                        yield return CreateSearchResult(SearchResultType.PropertyType, type, GetFriendlyName(propertyDefinition.PropertyType), propertyDefinition);
                    }
                    else if (member is MethodDefinition methodDefinition)
                    {
                        if (DoesMatchSearchCriteria(query, methodDefinition.ReturnType, matchCasing, matchWholeWord))
                        {
                            yield return CreateSearchResult(SearchResultType.MethodReturnType, type, GetFriendlyName(methodDefinition.ReturnType), methodDefinition);
                        }

                        if (methodDefinition.HasBody && methodDefinition.Body.HasVariables)
                        {
                            foreach (var variable in methodDefinition.Body.Variables)
                            {
                                if (DoesMatchSearchCriteria(query, variable.Name, matchCasing, matchWholeWord))
                                {
                                    yield return CreateSearchResult(SearchResultType.VariableName, type, variable.Name, variable);
                                }
                                else if (DoesMatchSearchCriteria(query, variable.VariableType, matchCasing, matchWholeWord))
                                {
                                    yield return CreateSearchResult(SearchResultType.VariableType, type, GetFriendlyName(variable.VariableType), variable);
                                }
                            }
                        }

                        if (methodDefinition.HasParameters)
                        {
                            foreach (var parameter in methodDefinition.Parameters)
                            {
                                if (DoesMatchSearchCriteria(query, parameter.Name, matchCasing, matchWholeWord))
                                {
                                    yield return CreateSearchResult(SearchResultType.ParameterName, type, parameter.Name, methodDefinition);
                                }
                                else if (DoesMatchSearchCriteria(query, parameter.ParameterType, matchCasing, matchWholeWord))
                                {
                                    yield return CreateSearchResult(SearchResultType.ParameterType, type, GetFriendlyName(parameter.ParameterType), parameter);
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
                                    yield return CreateSearchResult(SearchResultType.Instruction, type, GetFriendlyName(memberReference.DeclaringType), instruction);
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
                                yield return CreateSearchResult(SearchResultType.Instruction, type, memberReferenceName, instruction);
                            }
                        }
                        else if (operand is VariableReference variableReference && this.DoesMatchSearchCriteria(query, variableReference.Name, matchCasing, matchWholeWord))
                        {
                            yield return CreateSearchResult(SearchResultType.Instruction, type, variableReference.Name, instruction);
                        }
                        else if (operand is ParameterReference parameterReference && this.DoesMatchSearchCriteria(query, parameterReference.Name, matchCasing, matchWholeWord))
                        {
                            yield return CreateSearchResult(SearchResultType.Instruction, type, parameterReference.Name, instruction);
                        }
                        else if (operand is string stringLiteral && this.DoesMatchSearchCriteria(query, stringLiteral, matchCasing, matchWholeWord))
                        {
                            yield return CreateSearchResult(SearchResultType.Instruction, type, operand as string, instruction);
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
    
        switch (searchResult.Type)
        {
            case SearchResultType.ParameterName:
                {
                    var methodDefinition = searchResult.ObjectReference as MethodDefinition;
                    var parameterIndex = methodDefinition.Parameters.Select((p, i) => new { Item = p, Index = i })
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
                    var variableDefinition = searchResult.ObjectReference as VariableDefinition;
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
                    var instruction = searchResult.ObjectReference as Instruction;
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

    private SearchResult CreateSearchResult(SearchResultType type, TypeDefinition declaringType, string matchedString, object objectReference)
    {
        return new SearchResult
        {
            Type = type,
            DeclaringType = declaringType,
            MatchedString = matchedString,
            ObjectReference = objectReference
        };
    }

    private SearchResultType GetSearchResultTypeFromMemberDefinitionType(IMemberDefinition memberDefinition)
    {
        return memberDefinition switch
        {
            EventDefinition => SearchResultType.EventName,
            FieldDefinition => SearchResultType.FieldName,
            MethodDefinition => SearchResultType.MethodName,
            PropertyDefinition => SearchResultType.PropertyName,
            _ => SearchResultType.DeclaringType
        };
    }
}
