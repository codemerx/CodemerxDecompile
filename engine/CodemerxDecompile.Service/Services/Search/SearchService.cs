using CodemerxDecompile.Service.Interfaces;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.JustDecompiler.Languages;
using Mono.Cecil.Extensions;
using Mono.Cecil.Cil;
using CodemerxDecompile.Service.Services.Search.Models;

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
                        yield return this.AddSearchResultToCache(SearchResultType.TypeDefinition, type, type.Name, type);
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
                            yield return this.AddSearchResultToCache(SearchResultType.MemberDefinition, type, member.Name, member);
                        }

                        if (member is EventDefinition eventDefinition && eventDefinition.EventType.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase))
                        {
                            yield return this.AddSearchResultToCache(SearchResultType.MemberDefinitionType, type, eventDefinition.EventType.Name, eventDefinition);
                        }
                        else if (member is FieldDefinition fieldDefinition && fieldDefinition.FieldType.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase))
                        {
                            yield return this.AddSearchResultToCache(SearchResultType.MemberDefinitionType, type, fieldDefinition.FieldType.Name, fieldDefinition);
                        }
                        else if (member is PropertyDefinition propertyDefinition && propertyDefinition.PropertyType.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase))
                        {
                            yield return this.AddSearchResultToCache(SearchResultType.MemberDefinitionType, type, propertyDefinition.PropertyType.Name, propertyDefinition);
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
                                        yield return this.AddSearchResultToCache(SearchResultType.VariableDefinition, type, variable.Name, variable);
                                    }
                                    else if (variable.VariableType.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        yield return this.AddSearchResultToCache(SearchResultType.VariableDefinitionType, type, variable.VariableType.Name, variable);
                                    }
                                }
                            }

                            if (methodDefinition.HasParameters)
                            {
                                foreach (ParameterDefinition parameter in methodDefinition.Parameters)
                                {
                                    if (parameter.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        yield return this.AddSearchResultToCache(SearchResultType.ParameterDefinition, type, parameter.Name, parameter);
                                    }
                                    else if (parameter.ParameterType.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        yield return this.AddSearchResultToCache(SearchResultType.ParameterDefinitionType, type, parameter.ParameterType.Name, parameter);
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
                            else if ((operand as string) != null && ((string)operand).Contains(searchString, StringComparison.InvariantCultureIgnoreCase))
                            {
                                yield return this.AddSearchResultToCache(SearchResultType.Instruction, type, operand as string, instruction);
                            }
                        }
                    }
                }
            }
        }

        private SearchResult AddSearchResultToCache(SearchResultType type, TypeDefinition declaringType, string matchedString, object objectReference)
        {
            if (this.decompilationContext.TryGetTypeFilePathFromCache(this.GetTopTypeDefinition(declaringType), out string typeFilePath))
            {
                SearchResult searchResult = new SearchResult(type, typeFilePath, matchedString, objectReference);

                this.cachedSearchResults[this.cachedSearchResults.Count] = searchResult;

                return searchResult;
            }
            else
            {
                throw new ArgumentException("Could not find type file path");
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
