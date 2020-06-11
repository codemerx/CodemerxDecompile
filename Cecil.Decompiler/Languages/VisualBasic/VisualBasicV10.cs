using Mono.Cecil;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.GotoElimination;
using Telerik.JustDecompiler.Languages.VisualBasic;
using Telerik.JustDecompiler.Steps;
using Telerik.JustDecompiler.Steps.SwitchByString;

namespace Telerik.JustDecompiler.Languages
{
    public static partial class LanguageFactory
    {
        private class VisualBasicV10 : VisualBasic, IVisualBasic
        {
            private static VisualBasicV10 instance;

            static VisualBasicV10()
            {
                instance = new VisualBasicV10();
            }

            protected VisualBasicV10()
            {
                // Keywords are taken from https://msdn.microsoft.com/en-us/library/dd409611(v=vs.140).aspx
                // The Out keyword is not added on purpose. That's because, we succeded to use it as identifier and the compiler
                // doesn't display error. If you add it, there will be many changes linked to the use of the OutAttribute.
                string[] GlobalKeywords =
                {
                    "AddHandler","AddressOf","Alias","And","AndAlso","As","Boolean","ByRef","Byte","ByVal","Call","Case","Catch","CBool",
                    "CByte","CChar","CDate","CDbl","CDec","Char","CInt","Class","CLng","CObj","Const","Continue","CSByte","CShort","CSng",
                    "CStr","CType","CUInt","CULng","CUShort","Date","Decimal","Declare","Default","Delegate","Dim","DirectCast","Do",
                    "Double","Each","Else","ElseIf","End","EndIf","Enum","Erase","Error","Event","Exit","False","Finally","For","Friend",
                    "Function","Get","GetType","GetXMLNamespace","Global","GoSub","GoTo","Handles","If","Implements","Imports","In",
                    "Inherits","Integer","Interface","Is","IsNot","Let","Lib","Like","Long","Loop","Me","Mod","Module","MustInherit",
                    "MustOverride","MyBase","MyClass","Namespace","Narrowing","New","Next","Not","Nothing","NotInheritable",
                    "NotOverridable","Object","Of","On","Operator","Option","Optional","Or","OrElse","Overloads","Overridable","Overrides",
                    "ParamArray","Partial","Private","Property","Protected","Public","RaiseEvent","ReadOnly","ReDim","REM","RemoveHandler",
                    "Resume","Return","SByte","Select","Set","Shadows","Shared","Short","Single","Static","Step","Stop","String",
                    "Structure","Sub","SyncLock","Then","Throw","To","True","Try","TryCast","TypeOf","UInteger","ULong","UShort","Using",
                    "Variant","Wend","When","While","Widening","With","WithEvents","WriteOnly","Xor","#Const","#Else","#ElseIf","#End","#If"
                };

                foreach (string word in GlobalKeywords)
                {
                    this.languageSpecificGlobalKeywords.Add(word);
                }
            }

            new public static VisualBasicV10 Instance
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
                    return 10;
                }
            }

            public override bool SupportsGetterOnlyAutoProperties
            {
                get
                {
                    // TODO: Fix when VB14 is added
                    return true;
                }
            }
            
            internal override IDecompilationStep[] LanguageDecompilationSteps(bool inlineAggressively)
            {
                return new IDecompilationStep[]
                {
                    new RebuildAsyncStatementsStep(),
                    new RebuildYieldStatementsStep(),
                    new VisualBasicRemoveDelegateCachingStep(),
                    // RebuildAnonymousDelegatesStep needs to be executed before the RebuildLambdaExpressions step
                    new RebuildAnonymousDelegatesStep(),
                    new RebuildLambdaExpressions(),
                    new GotoCancelation(),
                    new CombinedTransformerStep(),
                    // new RemoveConditionOnlyVariables(),
                    new MergeUnaryAndBinaryExpression(),
                    new RemoveLastReturn(),
                    new RebuildSwitchByString(),
                    new RebuildForeachStatements(),
                    new RebuildForeachArrayStatements(),
                    new RebuildVBForStatements(),
                    new RebuildDoWhileStatements(),
                    new RebuildLockStatements(),
                    new RebuildFixedStatements(),
                    new RebuildUsingStatements(),
                    new RenameEnumValues(),
                    new FixMethodOverloadsStep(),
                    new DetermineCtorInvocationStep(),
                    new RebuildExpressionTreesStep(),
                    new TransformMemberHandlersStep(),
                    // There were a lot of issues when trying to merge the SelfAssignment step with the CombinedTransformerStep.
                    // The SelfAssignment step is moved before VBCodePatternsStep in order to enable the VariableInliningPattern
                    // to try to inline expressions composed in the SelfAssignment step.
                    new VBSelfAssignment(),
                    new VBCodePatternsStep(inlineAggressively),
                    // TransformCatchClausesFilterExpressionStep needs to be after VBCodePatternsStep,
                    // because it works only if the TernaryConditionPattern has been applied.
                    new TransformCatchClausesFilterExpressionStep(),
                    new DeduceImplicitDelegates(),
                    new CreateIfElseIfStatementsStep(),
                    new CreateCompilerOptimizedSwitchByStringStatementsStep(),
                    new ParenthesizeExpressionsStep(),
                    new VisualBasicRemoveUnusedVariablesStep(),
                    // RebuildCatchClausesFilterStep needs to be before DeclareVariablesOnFirstAssignment and after RemoveUnusedVariablesStep.
                    // RebuildCatchClausesFilterStep contains pattern matching and need to be after TransformCatchClausesFilterExpressionStep.
                    new RebuildCatchClausesFilterStep(),
                    new DeclareVariablesOnFirstAssignment(),
                    new DeclareTopLevelVariables(),
                    new RenameSplitPropertiesMethodsAndBackingFields(),
                    new RenameVBVariables(),
                    new CastEnumsToIntegersStep(),
                    new CastIntegersStep(),
                    new ArrayVariablesStep(),
                    new UnsafeMethodBodyStep(),
                    new DetermineDestructorStep(),
                    new DependsOnAnalysisStep(),
                    new DetermineNotSupportedVBCodeStep(),
                };
            }

            protected override IDecompilationStep[] LanguageFilterMethodDecompilationSteps(bool inlineAggressively)
            {
                return new IDecompilationStep[]
                {
                    new DeclareVariablesOnFirstAssignment(),
                    new DeclareTopLevelVariables(),
                    new RenameSplitPropertiesMethodsAndBackingFields(),
                    new RenameVBVariables(),
                    new CastEnumsToIntegersStep(),
                    new CastIntegersStep(),
                    new ArrayVariablesStep(),
                    new UnsafeMethodBodyStep(),
                    new DetermineDestructorStep(),
                    new DependsOnAnalysisStep(),
                    new DetermineNotSupportedVBCodeStep(),
                };
            }
        }
    }
}
