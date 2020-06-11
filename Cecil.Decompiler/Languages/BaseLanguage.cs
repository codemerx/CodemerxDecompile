using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Mono.Cecil;
using Telerik.JustDecompiler.Common;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.DefineUseAnalysis;
using Telerik.JustDecompiler.Decompiler.LogicFlow;
using Telerik.JustDecompiler.Steps;
using Telerik.JustDecompiler.Decompiler.GotoElimination;
using System.Text.RegularExpressions;
using Telerik.JustDecompiler.Decompiler.Inlining;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Languages
{
    public abstract class BaseLanguage : ILanguage
    {
        protected HashSet<string> languageSpecificGlobalKeywords;

        protected HashSet<string> languageSpecificContextualKeywords;

        public BaseLanguage()
        {
            this.languageSpecificGlobalKeywords = new HashSet<string>();
            this.languageSpecificContextualKeywords = new HashSet<string>();
        }

        public virtual string FloatingLiteralsConstant
        {
            get
            {
                return "";
            }
        }

        public static DecompilationPipeline IntermediateRepresenationPipeline
        {
            get
            {
                return new DecompilationPipeline(new RemoveUnreachableBlocksStep(),
                                                 new StackUsageAnalysis(),
                                                 new ExpressionDecompilerStep(),
                                                 new RemoveCompilerOptimizationsStep(),
                                                 new ManagedPointersRemovalStep(),
                                                 new VariableAssignmentAnalysisStep(),
                                                 new LogicalFlowBuilderStep(),
                                                 new FollowNodeLoopCleanUpStep(),
                                                 new StatementDecompilerStep(),
                                                 new MapUnconditionalBranchesStep());
            }
        }
        
        public abstract string Name { get; }

        public abstract int Version { get; }

        public abstract string EscapeSymbolBeforeKeyword { get; }

		public abstract string EscapeSymbolAfterKeyword { get; }

        public abstract string CommentLineSymbol { get; }

        public abstract string DocumentationLineStarter { get; }

		public virtual StringComparer IdentifierComparer
		{
			get
			{
				return StringComparer.Ordinal;
			}
		}

        public string ReplaceInvalidCharactersInIdentifier(string identifier)
        {
            //sanity checks
            if (identifier == null)
            {
                return null;
            }
            if (identifier == string.Empty)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();

            char firstCharacter = identifier[0];
            if (IsValidIdentifierFirstCharacter(firstCharacter))
            {
                sb.Append(firstCharacter);
            }
            else
            {
                sb.Append(string.Format("u{0:x4}", (int)firstCharacter));
            }
            for (int i = 1; i < identifier.Length; i++)
            {
                char currentChar = identifier[i];
                if (IsValidIdentifierCharacter(currentChar))
                {
                    sb.Append(currentChar);
                }
                else
                {
                    sb.Append(string.Format("u{0:x4}", (int)currentChar));
                }
            }
            return sb.ToString();
        }

        public bool IsValidIdentifier(string identifier)
        {
            return identifier.IsValidIdentifier();
        }

        public bool IsValidIdentifierCharacter(char currentChar)
        {
            return currentChar.IsValidIdentifierCharacter();
        }

        public bool IsValidIdentifierFirstCharacter(char firstCharacter)
        {
            return firstCharacter.IsValidIdentifierFirstCharacter();
        }

        public abstract ILanguageWriter GetWriter(IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings);

        public abstract IAssemblyAttributeWriter GetAssemblyAttributeWriter(IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings);

        public virtual DecompilationPipeline CreatePipeline()
        {
            DecompilationPipeline result = new DecompilationPipeline();
            result.AddSteps(BaseLanguage.IntermediateRepresenationPipeline.Steps);
            result.AddSteps(LanguageDecompilationSteps(false));
            return result;
        }

        public virtual DecompilationPipeline CreatePipeline(DecompilationContext context)
        {
            return CreatePipelineInternal(context, false);
        }

        public virtual DecompilationPipeline CreateLambdaPipeline(DecompilationContext context)
        {
            return CreatePipelineInternal(context, true);
        }

        public virtual BlockDecompilationPipeline CreateFilterMethodPipeline(DecompilationContext context)
        {
            return new BlockDecompilationPipeline(LanguageFilterMethodDecompilationSteps(false), context);
        }

        // This pipeline is used by the PropertyDecompiler to finish the decompilation of properties, which are partially decompiled
        // using the steps from the IntermediateRepresenationPipeline.
        public virtual BlockDecompilationPipeline CreatePropertyPipeline(DecompilationContext context)
        {
            BlockDecompilationPipeline result = new BlockDecompilationPipeline(LanguageDecompilationSteps(false), context);
            return result;
        }

        private DecompilationPipeline CreatePipelineInternal(DecompilationContext context, bool inlineAggressively)
        {
            DecompilationPipeline result = new DecompilationPipeline(BaseLanguage.IntermediateRepresenationPipeline.Steps, context);
            result.AddSteps(LanguageDecompilationSteps(inlineAggressively));
            return result;
        }

        internal virtual IDecompilationStep[] LanguageDecompilationSteps(bool inlineAggressively)
        {
            return new IDecompilationStep[0];
        }

        protected virtual IDecompilationStep[] LanguageFilterMethodDecompilationSteps(bool inlineAggressively)
        {
            return new IDecompilationStep[0];
        }

        public virtual bool IsLanguageKeyword(string word)
        {
            return IsLanguageKeyword(word, this.languageSpecificGlobalKeywords, this.languageSpecificContextualKeywords);
        }

        public virtual bool IsGlobalKeyword(string word)
        {
            return IsGlobalKeyword(word, this.languageSpecificGlobalKeywords);
        }

        public abstract bool IsOperatorKeyword(string @operator);

        protected abstract bool IsLanguageKeyword(string word, HashSet<string> globalKeywords, HashSet<string> contextKeywords);

        protected abstract bool IsGlobalKeyword(string word, HashSet<string> globalKeywords);

        public virtual string EscapeWord(string word)
        {
            string escapedWord = EscapeSymbolBeforeKeyword + word + EscapeSymbolAfterKeyword;
            return escapedWord;
        }

		public virtual bool IsEscapedWord(string word)
		{
			return word.StartsWith(EscapeSymbolBeforeKeyword) && word.EndsWith(EscapeSymbolAfterKeyword);
		}

		public virtual bool IsEscapedWord(string escapedWord, string originalWord)
		{
			return escapedWord == EscapeWord(originalWord);
		}

		public virtual string GetExplicitName(IMemberDefinition member)
		{
			return member.Name;
		}

        public abstract string VSCodeFileExtension { get; }

        public abstract string VSProjectFileExtension { get; }

        public string CommentLines(string text)
        {
            string result = CommentLines(text, GetCommentLine());
            return result;
        }

        protected abstract string GetCommentLine();

        private string CommentLines(string text, string lineCommentString)
        {
            StringBuilder sb = new StringBuilder();
            StringReader reader = new StringReader(text);
            using (reader)
            {
                string currentLine = reader.ReadLine();
                while (currentLine != null)
                {
                    sb.AppendLine(string.Format("{0} {1}", lineCommentString, currentLine));
                    currentLine = reader.ReadLine();
                }
            }
            return sb.ToString();
        }

        public virtual bool TryGetOperatorName(string operatorName, out string languageOperator)
        {
            languageOperator = null;
            return false;
        }

        public bool Equals(ILanguage other)
        {
            if (other == null)
            {
                return false;
            }
            return string.Equals(this.Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }

        public abstract bool IsValidLineStarter(CodeNodeType nodeType);

        public virtual bool HasOutKeyword
		{
			get
			{
				return true;
			}
		}

        public abstract bool SupportsGetterOnlyAutoProperties { get; }

        public abstract bool SupportsInlineInitializationOfAutoProperties { get; }

        public abstract bool SupportsExceptionFilters { get; }

        public abstract IVariablesToNotInlineFinder VariablesToNotInlineFinder { get; }

        public abstract bool HasDelegateSpecificSyntax { get; }

		public abstract HashSet<string> AttributesToHide { get; }
    }
}