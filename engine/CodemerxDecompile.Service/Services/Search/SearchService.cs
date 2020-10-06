using CodemerxDecompile.Service.Interfaces;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.JustDecompiler.Languages;
using Mono.Cecil.Extensions;
using Mono.Cecil.Cil;
using CodemerxDecompile.Service.Services.Search.Models;
using CodemerxDecompile.Service.Services.DecompilationContext.Models;

namespace CodemerxDecompile.Service.Services.Search
{
    public class SearchService : ISearchService
    {
        private readonly IDecompilationContext decompilationContext;
        private Dictionary<int, SearchResult> cachedSearchResults;

        public SearchService(IDecompilationContext decompilationContext)
        {
            this.decompilationContext = decompilationContext;
        }

        public IEnumerable<SearchResult> Search(string searchString, bool matchCase = false)
        {
            this.cachedSearchResults = new Dictionary<int, SearchResult>();

            IEnumerable<string> openedAssembliesFilePaths = this.decompilationContext.GetOpenedAssemliesPaths();

            foreach (string assemblyFilePath in openedAssembliesFilePaths)
            {
                AssemblyDefinition assembly = GlobalAssemblyResolver.Instance.GetAssemblyDefinition(assemblyFilePath);

                IEnumerable<TypeDefinition> types = assembly.Modules.SelectMany(m => m.GetTypes());

                foreach (TypeDefinition type in types)
                {
                    if (type.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase))
                    {
                        yield return this.AddSearchResultToCache(SearchResultType.DeclaringType, type, type.Name, type);
                    }

                    IEnumerable<IMemberDefinition> members = type.GetMembersSorted(false, LanguageFactory.GetLanguage(Telerik.JustDecompiler.Languages.CSharp.CSharpVersion.V7));

                    if (members.Count() == 0)
                    {
                        continue;
                    }

                    foreach (IMemberDefinition member in members)
                    {
                        if (member.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase))
                        {
                            SearchResultType memberSearchResultType = this.GetSearchResultTypeFromMemberDefinitionType(member);

                            // Skip adding nested types when traversing the type members as they are added when traversing the module types
                            if (memberSearchResultType != SearchResultType.DeclaringType)
                            {
                                yield return this.AddSearchResultToCache(memberSearchResultType, type, member.Name, member);
                            }
                        }

                        if (member is EventDefinition eventDefinition && eventDefinition.EventType.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase))
                        {
                            yield return this.AddSearchResultToCache(SearchResultType.EventType, type, eventDefinition.EventType.Name, eventDefinition);
                        }
                        else if (member is FieldDefinition fieldDefinition && !fieldDefinition.DeclaringType.IsEnum && fieldDefinition.FieldType.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase))
                        {
                            yield return this.AddSearchResultToCache(SearchResultType.FieldType, type, fieldDefinition.FieldType.Name, fieldDefinition);
                        }
                        else if (member is PropertyDefinition propertyDefinition && propertyDefinition.PropertyType.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase))
                        {
                            yield return this.AddSearchResultToCache(SearchResultType.PropertyType, type, propertyDefinition.PropertyType.Name, propertyDefinition);
                        }
                        else if (member is MethodDefinition methodDefinition)
                        {
                            if (methodDefinition.ReturnType.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase))
                            {
                                yield return this.AddSearchResultToCache(SearchResultType.MethodReturnType, type, methodDefinition.ReturnType.Name, methodDefinition);
                            }

                            if (methodDefinition.HasBody && methodDefinition.Body.HasVariables)
                            {
                                foreach (VariableDefinition variable in methodDefinition.Body.Variables)
                                {
                                    if (variable.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        yield return this.AddSearchResultToCache(SearchResultType.VariableName, type, variable.Name, variable);
                                    }
                                    else if (variable.VariableType.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        yield return this.AddSearchResultToCache(SearchResultType.VariableType, type, variable.VariableType.Name, variable);
                                    }
                                }
                            }

                            if (methodDefinition.HasParameters)
                            {
                                foreach (ParameterDefinition parameter in methodDefinition.Parameters)
                                {
                                    if (parameter.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        yield return this.AddSearchResultToCache(SearchResultType.ParameterName, type, parameter.Name, methodDefinition);
                                    }
                                    else if (parameter.ParameterType.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        yield return this.AddSearchResultToCache(SearchResultType.ParameterType, type, parameter.ParameterType.Name, parameter);
                                    }
                                }
                            }
                        }

                        IEnumerable<Instruction> instructions = member.GetInstructions();

                        foreach (Instruction instruction in instructions)
                        {
                            object operand = instruction.Operand;
                            bool shouldCheckDeclaringTypeName = false;

                            if (operand is TypeReference)
                            {
                                TypeDefinition resolvedTypeReference = (operand as TypeReference).Resolve();

                                if (resolvedTypeReference != null)
                                {
                                    shouldCheckDeclaringTypeName = resolvedTypeReference.IsStaticClass || resolvedTypeReference.IsEnum;
                                }
                            }
                            else if (operand is EventReference)
                            {
                                EventDefinition resolvedEventReference = (operand as EventReference).Resolve();

                                if (resolvedEventReference != null)
                                {
                                    shouldCheckDeclaringTypeName = resolvedEventReference.IsStatic();
                                }
                            }
                            else if (operand is FieldReference)
                            {
                                FieldDefinition resolvedFieldReference = (operand as FieldReference).Resolve();

                                if (resolvedFieldReference != null)
                                {
                                    shouldCheckDeclaringTypeName = resolvedFieldReference.IsStatic;
                                }
                            }
                            else if (operand is MethodReference)
                            {
                                MethodDefinition resolvedMethodReference = (operand as MethodReference).Resolve();

                                if (resolvedMethodReference != null)
                                {
                                    shouldCheckDeclaringTypeName = resolvedMethodReference.IsStatic || resolvedMethodReference.IsConstructor;
                                }
                            }
                            else if (operand is PropertyReference)
                            {
                                PropertyDefinition resolvedPropertyReference = (operand as PropertyReference).Resolve();

                                if (resolvedPropertyReference != null)
                                {
                                    shouldCheckDeclaringTypeName = resolvedPropertyReference.IsStatic();
                                }
                            }

                            if (operand is MemberReference memberReference)
                            {
                                if (memberReference.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    yield return this.AddSearchResultToCache(SearchResultType.Instruction, type, memberReference.Name, instruction);
                                }
                                else if (shouldCheckDeclaringTypeName && (memberReference.DeclaringType?.Name?.Contains(searchString, StringComparison.InvariantCultureIgnoreCase) ?? false))
                                {
                                    yield return this.AddSearchResultToCache(SearchResultType.Instruction, type, memberReference.DeclaringType.Name, instruction);
                                }
                            }
                            else if (operand is VariableReference variableReference && variableReference.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase))
                            {
                                yield return this.AddSearchResultToCache(SearchResultType.Instruction, type, variableReference.Name, instruction);
                            }
                            else if (operand is ParameterReference parameterReference && parameterReference.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase))
                            {
                                yield return this.AddSearchResultToCache(SearchResultType.Instruction, type, parameterReference.Name, instruction);
                            }
                            else if (operand is string stringLiteral && stringLiteral.Contains(searchString, StringComparison.InvariantCultureIgnoreCase))
                            {
                                yield return this.AddSearchResultToCache(SearchResultType.Instruction, type, operand as string, instruction);
                            }
                        }
                    }
                }
            }
        }

        public CodeSpan? GetSearchResultPosition(int searchResultIndex)
        {
            if (!this.cachedSearchResults.TryGetValue(searchResultIndex, out SearchResult searchResult) ||
                !this.decompilationContext.FilePathToType.TryGetValue(searchResult.DeclaringTypeFilePath, out TypeDefinition typeDefinition) ||
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
                    typeMetadata.CodeMappingInfo.VariableDefinitionToVariableTypeCodeMap.TryGetValue((VariableDefinition)searchResult.ObjectReference, out codeSpan);
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
                    typeMetadata.CodeMappingInfo.TryGetValue((Instruction)searchResult.ObjectReference, out codeSpan);
                    break;
                default:
                    return null;
            }

            return codeSpan;
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
