using System;
using System.Collections.Generic;
using System.IO;

using Mono.Cecil;
using Telerik.JustDecompiler.Languages;

namespace CodemerxDecompile.Service.Services
{
    public class CodeFormatter : PlainTextFormatter
    {
        public CodeFormatter(StringWriter writer) : base(writer) 
        {
            this.CodeSpanToMemberReference = new Dictionary<CodeSpan, MemberReference>();
        }

        public Dictionary<CodeSpan, MemberReference> CodeSpanToMemberReference { get; }

        public override void WriteReference(string value, object reference)
        {
            CodePosition startPosition = new CodePosition(this.CurrentLineNumber, this.CurrentColumnIndex);
            base.WriteReference(value, reference);
            CodePosition endPosition = new CodePosition(this.CurrentLineNumber, this.CurrentColumnIndex);

            this.CodeSpanToMemberReference.Add(new CodeSpan(startPosition, endPosition), reference as MemberReference);
        }
    }
}
