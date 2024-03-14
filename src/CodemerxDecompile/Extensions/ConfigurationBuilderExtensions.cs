using Microsoft.Extensions.Configuration;

namespace CodemerxDecompile.Extensions;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddEmbeddedResource(this IConfigurationBuilder builder, string resource)
    {
        var assembly = AssemblyProvider.Assembly;
        var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{resource}");
        if (stream != null)
            // AddJsonStream will dispose the stream. Unfortunately, this is not documented.
            // Source code: https://github.com/dotnet/runtime/blob/210a7a9feec64b7fac5147656cddb199ec90cf75/src/libraries/Microsoft.Extensions.Configuration.Json/src/JsonConfigurationFileParser.cs#L30
            builder.AddJsonStream(stream);

        return builder;
    }
}
