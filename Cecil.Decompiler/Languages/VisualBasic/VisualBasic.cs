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
using System.Linq;
using Telerik.JustDecompiler.Steps;
using Mono.Cecil;
using Telerik.JustDecompiler.Decompiler;
using System.Collections.Generic;
using Telerik.JustDecompiler.Decompiler.GotoElimination;
using Telerik.JustDecompiler.Steps.SwitchByString;
using Telerik.JustDecompiler.Languages.VisualBasic;
using Telerik.JustDecompiler.Decompiler.Inlining;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Languages
{
    public static partial class LanguageFactory
    {
        private class VisualBasic : BaseLanguage, IVisualBasic
        {
            private static VisualBasic instance;

            private Dictionary<string, string> operators;
		    private HashSet<string> operatorKeywords;
        
            static VisualBasic()
            {
                instance = new VisualBasic();
            }

            public static VisualBasic Instance
            {
                get
                {
                    return instance;
                }
            }

		    protected override bool IsLanguageKeyword(string word, HashSet<string> globalKeywords, HashSet<string> contextKeywords)
		    {
			    foreach (string globalKeyword in globalKeywords)
			    {
				    if (globalKeyword.Equals(word, StringComparison.OrdinalIgnoreCase))
				    {
					    return true;
				    }
			    }

			    foreach (string contextKeyword in contextKeywords)
			    {
				    if (contextKeyword.Equals(word, StringComparison.OrdinalIgnoreCase))
				    {
					    return true;
				    }
			    }

			    return false;
		    }

		    protected override bool IsGlobalKeyword(string word, HashSet<string> globalKeywords)
		    {
			    foreach (string globalKeyword in globalKeywords)
			    {
				    if (globalKeyword.Equals(word, StringComparison.OrdinalIgnoreCase))
				    {
					    return true;
				    }
			    }

			    return false;
		    }

		    public override bool IsOperatorKeyword(string @operator)
		    {
			    return this.operatorKeywords.Contains(@operator);
		    }

            protected VisualBasic()
            {
                this.operators = new Dictionary<string, string>();
			    this.operatorKeywords = new HashSet<string>();
                InitializeOperators();
            }

            private void InitializeOperators()
            {
                //find reliable source for this operators
                //TODO: test all of them
                string[,] operatorPairs = { 
                                              //unary operators
                                              //{ "Decrement", "--" }, { "Increment", "++" }, unavailable
                                              { "UnaryNegation", "-" }, { "UnaryPlus", "+" }, { "LogicalNot", "Not" }, {"True", "IsTrue"}, {"False", "IsFalse"},
                                              { "AddressOf", "&" },{"OnesComplement","Not"},{"PointerDereference","*"},
                                              //binary operators
                                              {"Addition","+"},{"Subtraction","-"},{"Multiply","*"},{"Division","/"},{"Modulus","Mod"},{"ExclusiveOr","Xor"},
                                              {"BitwiseAnd","And"},{"BitwiseOr","Or"},{"LogicalAnd","AndAlso"},{"LogicalOr","OrElse"},{"LeftShift","<<"},{"RightShift",">>"},
                                              {"Equality","="},{"GreaterThan",">"},{"LessThan","<"},{"Inequality","<>"},{"GreaterThanOrEqual",">="},{"LessThanOrEqual","<="},
                                              {"MemberSelection","->"},{"PointerToMemberSelection","->*"},{"Comma",","},//not sure if all these exist in VB
                                              //those can't be redefined, so no point looking for them
                                              //{"RightShiftAssignment",">>="},{"MultiplicationAssignment","*="},
                                              //{"SubtractionAssignment","-="},{"ExclusiveOrAssignment","^="},{"LeftShiftAssignment","<<="},{"ModulusAssignment","%="},
                                              //{"AdditionAssignment","+="},{"BitwiseAndAssignment","&="},{"BitwiseOrAssignment","|="},{"DivisionAssignment","/="},
                                              //other
                                              {"Implicit","CType"},{"Explicit","CType"}, //those are for imlicit/explicit type casts
                                          };
                for (int row = 0; row < operatorPairs.GetLength(0); row++)
                {
                    this.operators.Add(operatorPairs[row, 0], operatorPairs[row, 1]);
                }

			    string[] operatorKeywordsArray = { "And", "Or", "Xor", "AndAlso", "OrElse", "Mod", "Is", "IsNot", "Not" };
			    for (int operatorKeywordIndex = 0; operatorKeywordIndex < operatorKeywordsArray.Length; operatorKeywordIndex++)
			    {
				    this.operatorKeywords.Add(operatorKeywordsArray[operatorKeywordIndex]);
			    }
            }

            public override string FloatingLiteralsConstant 
            { 
                get 
                {
                    return "!";
                }
            }

            public override string Name
            {
                get
                {
                    return "VB.NET" + this.Version;
                }
            }

            public override int Version
            {
                get
                {
                    return 0;
                }
            }

            public override string VSCodeFileExtension
            {
                get { return ".vb"; }
            }

            public override string VSProjectFileExtension
            {
                get { return ".vbproj"; }
            }

		    public override string CommentLineSymbol
		    {
			    get { return "'"; }
		    }

		    public override string DocumentationLineStarter
		    {
			    get { return "'''"; }
		    }

		    public override StringComparer IdentifierComparer
		    {
			    get
			    {
				    return StringComparer.OrdinalIgnoreCase;
			    }
		    }

		    public override ILanguageWriter GetWriter(IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings)
            {
			    return new VisualBasicWriter(this, formatter, exceptionFormatter, settings);
            }

		    public override IAssemblyAttributeWriter GetAssemblyAttributeWriter(IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings)
		    {
			    return new VisualBasicAssemblyAttributeWriter(this, formatter, exceptionFormatter, settings);
		    }

		    //public override DecompilationPipeline CreatePipeline(MethodDefinition method)
		    //{
		    //    return new DecompilationPipeline(
		    //        new StatementDecompiler(BlockOptimization.Basic),
		    //        CombinedTransformerStep.Instance,
		    //        DeclareTopLevelVariables.Instance);
		    //}

		    private bool ExistsNonExplicitMember(IMemberDefinition explicitMember, string nonExplicitName)
		    {
			    if (explicitMember is MethodDefinition)
			    {
				    return explicitMember.DeclaringType.Methods.Where(t => GetMemberNonExplicitName(t) == nonExplicitName).Count() > 1;
			    }

			    if (explicitMember is PropertyDefinition)
			    {
				    return explicitMember.DeclaringType.Properties.Where(t => GetMemberNonExplicitName(t) == nonExplicitName).Count() > 1;
			    }

			    if (explicitMember is EventDefinition)
			    {
				    return explicitMember.DeclaringType.Events.Where(t => GetMemberNonExplicitName(t) == nonExplicitName).Count() > 1;
			    }

			    return false;
		    }

		    private string GetMemberSpecialExplicitName(IMemberDefinition member)
		    {
			    string nonExplicitName = GetMemberNonExplicitName(member);

			    if (ExistsNonExplicitMember(member, nonExplicitName))
			    {
				    return "Explicit" + nonExplicitName;
			    }
			    else
			    {
				    return nonExplicitName;
			    }
		    }

		    private string GetMemberNonExplicitName(IMemberDefinition member)
		    {
			    string memberName = member.Name;

			    int lastIndex = memberName.LastIndexOf('.');
			    if (lastIndex != -1)
			    {
				    memberName = memberName.Substring(lastIndex + 1);
			    }

			    return memberName;
		    }

		    public override string GetExplicitName(IMemberDefinition member)
		    {
			    string nonExplicitName = GetMemberNonExplicitName(member);

			    if (ExistsNonExplicitMember(member, nonExplicitName))
			    {
				    return GetMemberSpecialExplicitName(member);
			    }
			    else
			    {
				    return nonExplicitName;
			    }
		    }
        

		    public override string EscapeSymbolBeforeKeyword
		    {
			    get
			    {
				    return "[";
			    }
		    }

		    public override string EscapeSymbolAfterKeyword
		    {
			    get
			    {
				    return "]";
			    }
		    }

            protected override string GetCommentLine()
            {
                return @"'";
            }

            public override bool TryGetOperatorName(string operatorName, out string languageOperator)
            {
                bool result = this.operators.TryGetValue(operatorName, out languageOperator);
                return result;
            }

            public override bool IsValidLineStarter(CodeNodeType nodeType)
            {
                return nodeType == CodeNodeType.FieldReferenceExpression ||
                       nodeType == CodeNodeType.PropertyReferenceExpression ||
                       nodeType == CodeNodeType.MethodInvocationExpression ||
                       nodeType == CodeNodeType.SafeCastExpression ||
                       nodeType == CodeNodeType.ExplicitCastExpression ||
                       nodeType == CodeNodeType.ThisReferenceExpression ||
                       nodeType == CodeNodeType.BaseReferenceExpression ||
                       nodeType == CodeNodeType.VariableReferenceExpression ||
                       nodeType == CodeNodeType.ArgumentReferenceExpression ||
                       nodeType == CodeNodeType.TypeOfExpression ||
                       nodeType == CodeNodeType.ArrayIndexerExpression;
            }

            public override IVariablesToNotInlineFinder VariablesToNotInlineFinder
            {
                get
                {
                    return new VisualBasicVariablesToNotInlineFinder(this);
                }
            }

            public override bool HasOutKeyword
		    {
			    get
			    {
				    return false;
			    }
		    }

            public override bool SupportsGetterOnlyAutoProperties
            {
                get
                {
                    return false;
                }
            }

            public override bool SupportsInlineInitializationOfAutoProperties
            {
                get
                {
                    return false;
                }
            }

            // Exception filters are supported since the first version of VB.NET
            public override bool SupportsExceptionFilters
            {
                get
                {
                    return true;
                }
            }

            public override bool HasDelegateSpecificSyntax
            {
                get
                {
                    return true;
                }
            }

			public override HashSet<string> AttributesToHide
			{
				get
				{
					string[] attributesToHide = new string[] { "System.ParamArrayAttribute",
															   "System.Runtime.CompilerServices.IteratorStateMachineAttribute",
															   "Microsoft.VisualBasic.CompilerServices.StandardModuleAttribute",
															   "System.Runtime.CompilerServices.ExtensionAttribute",
															   "System.Diagnostics.DebuggerStepThroughAttribute",
															   "System.Runtime.CompilerServices.AsyncStateMachineAttribute",
															   "System.Runtime.CompilerServices.CompilerGeneratedAttribute" };

					return new HashSet<string>(attributesToHide);
				}
			}
        }
    }
}