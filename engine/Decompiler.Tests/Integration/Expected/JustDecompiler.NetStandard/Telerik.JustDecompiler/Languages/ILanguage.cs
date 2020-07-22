using Mono.Cecil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.Inlining;

namespace Telerik.JustDecompiler.Languages
{
	public interface ILanguage : IEquatable<ILanguage>
	{
		HashSet<string> AttributesToHide
		{
			get;
		}

		string CommentLineSymbol
		{
			get;
		}

		string DocumentationLineStarter
		{
			get;
		}

		string EscapeSymbolAfterKeyword
		{
			get;
		}

		string EscapeSymbolBeforeKeyword
		{
			get;
		}

		string FloatingLiteralsConstant
		{
			get;
		}

		bool HasDelegateSpecificSyntax
		{
			get;
		}

		bool HasOutKeyword
		{
			get;
		}

		StringComparer IdentifierComparer
		{
			get;
		}

		string Name
		{
			get;
		}

		bool SupportsExceptionFilters
		{
			get;
		}

		bool SupportsGetterOnlyAutoProperties
		{
			get;
		}

		bool SupportsInlineInitializationOfAutoProperties
		{
			get;
		}

		IVariablesToNotInlineFinder VariablesToNotInlineFinder
		{
			get;
		}

		int Version
		{
			get;
		}

		string VSCodeFileExtension
		{
			get;
		}

		string VSProjectFileExtension
		{
			get;
		}

		string CommentLines(string text);

		BlockDecompilationPipeline CreateFilterMethodPipeline(DecompilationContext context);

		DecompilationPipeline CreateLambdaPipeline(DecompilationContext context);

		DecompilationPipeline CreatePipeline();

		DecompilationPipeline CreatePipeline(DecompilationContext context);

		BlockDecompilationPipeline CreatePropertyPipeline(DecompilationContext context);

		string EscapeWord(string word);

		IAssemblyAttributeWriter GetAssemblyAttributeWriter(IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings);

		string GetExplicitName(IMemberDefinition member);

		ILanguageWriter GetWriter(IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings);

		bool IsEscapedWord(string escapedWord);

		bool IsEscapedWord(string escapedWord, string originalWord);

		bool IsGlobalKeyword(string word);

		bool IsLanguageKeyword(string word);

		bool IsOperatorKeyword(string operator);

		bool IsValidIdentifier(string identifier);

		bool IsValidIdentifierCharacter(char currentChar);

		bool IsValidIdentifierFirstCharacter(char firstCharacter);

		bool IsValidLineStarter(CodeNodeType nodeType);

		string ReplaceInvalidCharactersInIdentifier(string identifier);

		bool TryGetOperatorName(string operatorName, out string languageOperator);
	}
}