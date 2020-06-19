using Mono.Cecil;
using Telerik.JustDecompiler.Steps;
using Telerik.JustDecompiler.Decompiler.GotoElimination;
using Telerik.JustDecompiler.Steps.SwitchByString;
using Telerik.JustDecompiler.Languages.CSharp;

namespace Telerik.JustDecompiler.Languages
{
    public static partial class LanguageFactory
    {
        private class CSharpV5 : CSharp, ICSharp
        {
            private static CSharpV5 instance;

            static CSharpV5()
            {
                instance = new CSharpV5();
            }

            protected CSharpV5()
            {
                //list taken from http://msdn.microsoft.com/en-us/library/x53a06bb.aspx -> MSDN list of C# keywords

                string[] GlobalKeywords =
                {
                    "abstract","as","base","bool","break","byte","case","catch","char","checked","class","const","continue","decimal",
                    "default","delegate","do","double","else","enum","event","explicit","extern","false","finally","fixed","float",
                    "for","foreach","goto","if","implicit","in","int","interface","internal","is","lock","long","namespace","new",
                    "null","object","operator","out","override","params","private","protected","public","readonly","ref","return",
                    "sbyte","sealed","short","sizeof","stackalloc","static","string","struct","switch","this","throw","true","try",
                    "typeof","uint","ulong","unchecked","unsafe","ushort","using","virtual","void","volatile","while"
                };
                foreach (string word in GlobalKeywords)
                {
                    this.languageSpecificGlobalKeywords.Add(word);
                }

                //list taken from http://msdn.microsoft.com/en-us/library/x53a06bb.aspx -> MSDN list of C# contextual keywords

                string[] contextualKeywords =
                {
                    "add","alias","ascending","async","await","descending","dynamic","from","get","global","group","into","join",
                    "let","orderby","partial","remove","select","set","value","var","where","yield"
                };
                foreach (string word in contextualKeywords)
                {
                    this.languageSpecificContextualKeywords.Add(word);
                }
            }

            new public static CSharpV5 Instance
            {
                get
                {
                    return instance;
                }
            }

            public override int Version
            {
                get
                {
                    return 5;
                }
            }
            
            internal override IDecompilationStep[] LanguageDecompilationSteps(bool inlineAggressively)
            {
                return new IDecompilationStep[]
                {
                    new OutParameterAssignmentAnalysisStep(),
                    new RebuildAsyncStatementsStep(),
                    new RebuildYieldStatementsStep(),
                    new RemoveDelegateCachingStep(),
                    // RebuildAnonymousDelegatesStep needs to be executed before the RebuildLambdaExpressions step
                    new RebuildAnonymousDelegatesStep(),
                    new RebuildLambdaExpressions(),
                    new ResolveDynamicVariablesStep(),
                    new GotoCancelation(),
                    new CombinedTransformerStep(),
                    new MergeUnaryAndBinaryExpression(),
                    new RemoveLastReturn(),
                    new RebuildSwitchByString(),
                    new RebuildForeachStatements(),
                    new RebuildForeachArrayStatements(),
                    new RebuildForStatements(),
                    new RebuildLockStatements(),
                    new RebuildFixedStatements(),
                    new RebuildUsingStatements(),
                    new RenameEnumValues(),
                    new FixMethodOverloadsStep(),
                    new RebuildExpressionTreesStep(),
                    new TransformMemberHandlersStep(),
                    // There were a lot of issues when trying to merge the SelfAssignment step with the CombinedTransformerStep.
                    // The SelfAssignment step is moved before CodePatternsStep in order to enable the VariableInliningPattern
                    // to try to inline expressions composed in the SelfAssignment step.
                    new SelfAssignment(),
                    new CodePatternsStep(inlineAggressively),
                    new DetermineCtorInvocationStep(),
                    new DeduceImplicitDelegates(),
                    new RebuildLinqQueriesStep(),
                    new CreateIfElseIfStatementsStep(),
                    new CreateCompilerOptimizedSwitchByStringStatementsStep(),
                    new ParenthesizeExpressionsStep(),
                    new RemoveUnusedVariablesStep(),
                    new DeclareVariablesOnFirstAssignment(),
                    new DeclareTopLevelVariables(),
                    new AssignOutParametersStep(),
                    new RenameSplitPropertiesMethodsAndBackingFields(),
                    new RenameVariables(),
                    new CastEnumsToIntegersStep(),
                    new CastIntegersStep(),
                    new ArrayVariablesStep(),
                    new CaseGotoTransformerStep(),
                    new UnsafeMethodBodyStep(),
                    new DetermineDestructorStep(),
                    // DependsOnAnalysisStep must be always last step, because it make analysis on the final decompilation result.
				    new DependsOnAnalysisStep(),
                };
            }
        }
    }
}
