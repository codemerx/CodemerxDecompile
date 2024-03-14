using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Common;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.DefineUseAnalysis;
using Telerik.JustDecompiler.Decompiler.Inlining;
using Telerik.JustDecompiler.Decompiler.LogicFlow;
using Telerik.JustDecompiler.Steps;

namespace Telerik.JustDecompiler.Languages
{
	public abstract class BaseLanguage : ILanguage, IEquatable<ILanguage>
	{
		protected HashSet<string> languageSpecificGlobalKeywords;

		protected HashSet<string> languageSpecificContextualKeywords;

		public abstract HashSet<string> AttributesToHide
		{
			get;
		}

		public abstract string CommentLineSymbol
		{
			get;
		}

		public abstract string DocumentationLineStarter
		{
			get;
		}

		public abstract string EscapeSymbolAfterKeyword
		{
			get;
		}

		public abstract string EscapeSymbolBeforeKeyword
		{
			get;
		}

		public virtual string FloatingLiteralsConstant
		{
			get
			{
				return "";
			}
		}

		public abstract bool HasDelegateSpecificSyntax
		{
			get;
		}

		public virtual bool HasOutKeyword
		{
			get
			{
				return true;
			}
		}

		public virtual StringComparer IdentifierComparer
		{
			get
			{
				return StringComparer.Ordinal;
			}
		}

		public static DecompilationPipeline IntermediateRepresenationPipeline
		{
			get
			{
				return new DecompilationPipeline(new IDecompilationStep[] { new RemoveUnreachableBlocksStep(), new StackUsageAnalysis(), new ExpressionDecompilerStep(), new RemoveCompilerOptimizationsStep(), new ManagedPointersRemovalStep(), new VariableAssignmentAnalysisStep(), new LogicalFlowBuilderStep(), new FollowNodeLoopCleanUpStep(), new StatementDecompilerStep(), new MapUnconditionalBranchesStep() });
			}
		}

		public abstract string Name
		{
			get;
		}

		public abstract bool SupportsExceptionFilters
		{
			get;
		}

		public abstract bool SupportsGetterOnlyAutoProperties
		{
			get;
		}

		public abstract bool SupportsInlineInitializationOfAutoProperties
		{
			get;
		}

		public abstract IVariablesToNotInlineFinder VariablesToNotInlineFinder
		{
			get;
		}

		public abstract int Version
		{
			get;
		}

		public abstract string VSCodeFileExtension
		{
			get;
		}

		public abstract string VSProjectFileExtension
		{
			get;
		}

		public BaseLanguage()
		{
			this.languageSpecificGlobalKeywords = new HashSet<string>();
			this.languageSpecificContextualKeywords = new HashSet<string>();
		}

		public string CommentLines(string text)
		{
			return this.CommentLines(text, this.GetCommentLine());
		}

		private string CommentLines(string text, string lineCommentString)
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringReader stringReader = new StringReader(text);
			using (stringReader)
			{
				for (string i = stringReader.ReadLine(); i != null; i = stringReader.ReadLine())
				{
					stringBuilder.AppendLine(String.Format("{0} {1}", (object)lineCommentString, i));
				}
			}
			return stringBuilder.ToString();
		}

		public virtual BlockDecompilationPipeline CreateFilterMethodPipeline(DecompilationContext context)
		{
			return new BlockDecompilationPipeline(this.LanguageFilterMethodDecompilationSteps(false), context);
		}

		public virtual DecompilationPipeline CreateLambdaPipeline(DecompilationContext context)
		{
			return this.CreatePipelineInternal(context, true);
		}

		public virtual DecompilationPipeline CreatePipeline()
		{
			DecompilationPipeline decompilationPipeline = new DecompilationPipeline(Array.Empty<IDecompilationStep>());
			decompilationPipeline.AddSteps(BaseLanguage.IntermediateRepresenationPipeline.Steps);
			decompilationPipeline.AddSteps(this.LanguageDecompilationSteps(false));
			return decompilationPipeline;
		}

		public virtual DecompilationPipeline CreatePipeline(DecompilationContext context)
		{
			return this.CreatePipelineInternal(context, false);
		}

		private DecompilationPipeline CreatePipelineInternal(DecompilationContext context, bool inlineAggressively)
		{
			DecompilationPipeline decompilationPipeline = new DecompilationPipeline(BaseLanguage.IntermediateRepresenationPipeline.Steps, context);
			decompilationPipeline.AddSteps(this.LanguageDecompilationSteps(inlineAggressively));
			return decompilationPipeline;
		}

		public virtual BlockDecompilationPipeline CreatePropertyPipeline(DecompilationContext context)
		{
			return new BlockDecompilationPipeline(this.LanguageDecompilationSteps(false), context);
		}

		public bool Equals(ILanguage other)
		{
			if (other == null)
			{
				return false;
			}
			return String.Equals(this.Name, other.Name, StringComparison.OrdinalIgnoreCase);
		}

		public virtual string EscapeWord(string word)
		{
			return String.Concat(this.EscapeSymbolBeforeKeyword, word, this.EscapeSymbolAfterKeyword);
		}

		public abstract IAssemblyAttributeWriter GetAssemblyAttributeWriter(IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings);

		protected abstract string GetCommentLine();

		public virtual string GetExplicitName(IMemberDefinition member)
		{
			return member.get_Name();
		}

		public abstract ILanguageWriter GetWriter(IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings);

		public virtual bool IsEscapedWord(string word)
		{
			if (!word.StartsWith(this.EscapeSymbolBeforeKeyword))
			{
				return false;
			}
			return word.EndsWith(this.EscapeSymbolAfterKeyword);
		}

		public virtual bool IsEscapedWord(string escapedWord, string originalWord)
		{
			return escapedWord == this.EscapeWord(originalWord);
		}

		public virtual bool IsGlobalKeyword(string word)
		{
			return this.IsGlobalKeyword(word, this.languageSpecificGlobalKeywords);
		}

		protected abstract bool IsGlobalKeyword(string word, HashSet<string> globalKeywords);

		public virtual bool IsLanguageKeyword(string word)
		{
			return this.IsLanguageKeyword(word, this.languageSpecificGlobalKeywords, this.languageSpecificContextualKeywords);
		}

		protected abstract bool IsLanguageKeyword(string word, HashSet<string> globalKeywords, HashSet<string> contextKeywords);

		public abstract bool IsOperatorKeyword(string @operator);

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

		public abstract bool IsValidLineStarter(CodeNodeType nodeType);

		internal virtual IDecompilationStep[] LanguageDecompilationSteps(bool inlineAggressively)
		{
			return new IDecompilationStep[0];
		}

		protected virtual IDecompilationStep[] LanguageFilterMethodDecompilationSteps(bool inlineAggressively)
		{
			return new IDecompilationStep[0];
		}

		public string ReplaceInvalidCharactersInIdentifier(string identifier)
		{
			if (identifier == null)
			{
				return null;
			}
			if (identifier == String.Empty)
			{
				return String.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			char chr = identifier[0];
			if (!this.IsValidIdentifierFirstCharacter(chr))
			{
				stringBuilder.Append(String.Format("u{0:x4}", (Int32)chr));
			}
			else
			{
				stringBuilder.Append(chr);
			}
			for (int i = 1; i < identifier.Length; i++)
			{
				char chr1 = identifier[i];
				if (!this.IsValidIdentifierCharacter(chr1))
				{
					stringBuilder.Append(String.Format("u{0:x4}", (Int32)chr1));
				}
				else
				{
					stringBuilder.Append(chr1);
				}
			}
			return stringBuilder.ToString();
		}

		public virtual bool TryGetOperatorName(string operatorName, out string languageOperator)
		{
			languageOperator = null;
			return false;
		}
	}
}