using Mono.Cecil;

namespace CodemerxDecompile.Extensions;

public static class MemberReferenceExtensions
{
    public static TypeReference GetTopDeclaringTypeOrSelf(this MemberReference memberReference)
    {
        var result = memberReference as TypeReference;
        var declaringType = memberReference.DeclaringType;
        while (declaringType != null)
        {
            result = declaringType;
            declaringType = declaringType.DeclaringType;
        }

        return result!;
    }
}
