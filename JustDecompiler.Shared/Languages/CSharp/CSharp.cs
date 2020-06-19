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

using Mono.Cecil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Languages.CSharp;
using Telerik.JustDecompiler.Steps;
using Telerik.JustDecompiler.Decompiler.Inlining;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Languages
{
    public static partial class LanguageFactory
    {
        private class CSharp : BaseLanguage, ICSharp
        {
            private static CSharp instance;

            private Dictionary<string, string> operators;

            static CSharp()
            {
                instance = new CSharp();
            }

            protected CSharp()
            {
                this.operators = new Dictionary<string, string>();
                InitializeOperators();
            }

            public static CSharp Instance
            {
                get
                {
                    return instance;
                }
            }

            private void InitializeOperators()
            {
                //taken from ECMA-355 documentation
                //TODO: test all of them
                string[,] operatorPairs = { 
                                              //unary operators
                                              { "Decrement", "--" }, { "Increment", "++" }, { "UnaryNegation", "-" }, { "UnaryPlus", "+" }, { "LogicalNot", "!" },
                                              {"OnesComplement","~"}, {"True", "true"}, {"False", "false"},
                                              //{ "AddressOf", "&" },{"PointerDereference","*"}, to test those we will need to run unsafe code
                                              //binary operators
                                              {"Addition","+"},{"Subtraction","-"},{"Multiply","*"},{"Division","/"},{"Modulus","%"},{"ExclusiveOr","^"},
                                              {"BitwiseAnd","&"},{"BitwiseOr","|"},
                                              //{"LogicalAnd","&&"},{"LogicalOr","||"},
                                              {"LeftShift","<<"},{"RightShift",">>"},
                                              {"Equality","=="},{"GreaterThan",">"},{"LessThan","<"},{"Inequality","!="},{"GreaterThanOrEqual",">="},{"LessThanOrEqual","<="},
                                              {"MemberSelection","->"},{"PointerToMemberSelection","->*"},{"Comma",","},
                                              //those can't be redefined, so no point looking for them
                                              //{"RightShiftAssignment",">>="},{"MultiplicationAssignment","*="},
                                              //{"SubtractionAssignment","-="},{"ExclusiveOrAssignment","^="},{"LeftShiftAssignment","<<="},{"ModulusAssignment","%="},
                                              //{"AdditionAssignment","+="},{"BitwiseAndAssignment","&="},{"BitwiseOrAssignment","|="},{"DivisionAssignment","/="},
                                              //other
                                              {"Implicit",""},{"Explicit",""}, //those are for imlicit/explicit type casts
                                          };
                for (int row = 0; row < operatorPairs.GetLength(0); row++)
                {
                    this.operators.Add(operatorPairs[row, 0], operatorPairs[row, 1]);
                }
            }

            public override string FloatingLiteralsConstant
            {
                get
                {
                    return "f";
                }
            }

            public override string Name
            {
                get
                {
                    return "C#" + this.Version;
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
                get
                {
                    return ".cs";
                }
            }

            public override string VSProjectFileExtension
            {
                get
                {
                    return ".csproj";
                }
            }

		    public override string EscapeSymbolBeforeKeyword
		    {
			    get
			    {
				    return "@";
			    }
		    }

		    public override string EscapeSymbolAfterKeyword
		    {
			    get
			    {
				    return "";
			    }
		    }

            public override string CommentLineSymbol
            {
                get { return "//"; }
            }

            public override string DocumentationLineStarter
            {
                get { return "///"; }
            }

            public override ILanguageWriter GetWriter(IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings)
            {
                return new CSharpWriter(this, formatter, exceptionFormatter, settings);
            }

            public override IAssemblyAttributeWriter GetAssemblyAttributeWriter(IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings)
            {
                return new CSharpAssemblyAttributeWriter(this, formatter, exceptionFormatter, settings);
            }

            protected override bool IsLanguageKeyword(string word, HashSet<string> globalKeywords, HashSet<string> contextKeywords)
            {
                    bool result = globalKeywords.Contains(word) || contextKeywords.Contains(word);
                    return result;
            }

            protected override bool IsGlobalKeyword(string word, HashSet<string> globalKeywords)
            {
                return globalKeywords.Contains(word);
            }

		    public override bool IsOperatorKeyword(string @operator)
		    {
			    return false;
		    }

            protected override string GetCommentLine()
            {
                return @"//";
            }

            public override bool TryGetOperatorName(string operatorName, out string languageOperator)
            {
                bool result = this.operators.TryGetValue(operatorName, out languageOperator);
                return result;
            }

            public override bool IsValidLineStarter(CodeNodeType nodeType)
            {
                // As far as we know, basically all nodes can be line starters.
                return true;
            }

            public override IVariablesToNotInlineFinder VariablesToNotInlineFinder
            {
                get
                {
                    return new EmptyVariablesToNotInlineFinder();
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

            public override bool SupportsExceptionFilters
            {
                get
                {
                    return false;
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
															   "Windows.Foundation.Metadata.ActivatableAttribute",
															   "System.Runtime.CompilerServices.DynamicAttribute",
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