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
using Mono.Cecil;
using Telerik.JustDecompiler.Decompiler;
using System.Collections.Generic;
using Telerik.JustDecompiler.Steps;
using Telerik.JustDecompiler.Decompiler.Inlining;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Languages
{
	public interface ILanguage : IEquatable<ILanguage>
	{
		bool IsValidIdentifierCharacter(char currentChar);

		bool IsValidIdentifierFirstCharacter(char firstCharacter);

		string Name { get; }

        int Version { get; }

        string EscapeSymbolBeforeKeyword { get; }

		string EscapeSymbolAfterKeyword { get; }

		string CommentLineSymbol { get; }

		string DocumentationLineStarter { get; }

		StringComparer IdentifierComparer { get; }

        ILanguageWriter GetWriter(IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings);
		
		IAssemblyAttributeWriter GetAssemblyAttributeWriter(IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings);

        bool IsLanguageKeyword(string word);

        bool IsGlobalKeyword(string word);

		bool IsOperatorKeyword(string @operator);

		bool IsValidIdentifier(string identifier);

        string EscapeWord(string word);

		bool IsEscapedWord(string escapedWord);

		bool IsEscapedWord(string escapedWord, string originalWord);
        
        DecompilationPipeline CreatePipeline();

        DecompilationPipeline CreatePipeline(DecompilationContext context);

        DecompilationPipeline CreateLambdaPipeline(DecompilationContext context);

        BlockDecompilationPipeline CreateFilterMethodPipeline(DecompilationContext context);

        // This pipeline is used by the PropertyDecompiler to finish the decompilation of properties, which are partially decompiled
        // using the step from the IntermediateRepresenationPipeline.
        BlockDecompilationPipeline CreatePropertyPipeline(DecompilationContext context);

		string GetExplicitName(IMemberDefinition member);

		string ReplaceInvalidCharactersInIdentifier(string identifier);

        string CommentLines(string text);

		string VSCodeFileExtension { get; }

		string VSProjectFileExtension { get; }

        bool TryGetOperatorName(string operatorName, out string languageOperator);

        string FloatingLiteralsConstant { get; }

		bool HasOutKeyword { get; }

        bool SupportsGetterOnlyAutoProperties { get; }

        bool SupportsInlineInitializationOfAutoProperties { get; }

        bool SupportsExceptionFilters { get; }

        IVariablesToNotInlineFinder VariablesToNotInlineFinder { get; }

        bool IsValidLineStarter(CodeNodeType nodeType);

        bool HasDelegateSpecificSyntax { get; }

		HashSet<string> AttributesToHide { get; }
    }
}
