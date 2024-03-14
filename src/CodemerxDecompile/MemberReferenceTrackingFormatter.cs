using System.Collections.Generic;
using System.IO;
using Mono.Cecil;
using Telerik.JustDecompiler.Languages;

namespace CodemerxDecompile;

internal class MemberReferenceTrackingFormatter : PlainTextFormatter
{
    public MemberReferenceTrackingFormatter(StringWriter writer)
        : base(writer)
    {
    }

    public Dictionary<OffsetSpan, MemberReference> CodeSpanToMemberReference { get; } = new();

    public override void WriteReference(string value, object reference)
    {
        if (reference is not MemberReference memberReference)
        {
            base.WriteReference(value, reference);
            return;
        }
        
        var startOffset = CurrentPosition;
        base.WriteReference(value, reference);
        var endOffset = CurrentPosition;

        CodeSpanToMemberReference.Add(new OffsetSpan(startOffset, endOffset), memberReference);
    }
}
