//    Copyright CodeMerx 2020
//    This file is part of CodemerxDecompile.

//    CodemerxDecompile is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.

//    CodemerxDecompile is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//    GNU Affero General Public License for more details.

//    You should have received a copy of the GNU Affero General Public License
//    along with CodemerxDecompile.  If not, see<https://www.gnu.org/licenses/>.

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
