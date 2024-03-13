namespace CodemerxDecompile.Providers;

public readonly record struct SystemInformation
{
    public required string AppVersion { get; init; }
    public required string OsPlatform { get; init; }
    public required string OsArchitecture { get; init; }
    public required string OsRelease { get; init; }
}
