namespace CodemerxDecompile.Options;

public record MatomoAnalyticsOptions
{
    public const string Key = "MatomoAnalytics";

    public required string ServerUrl { get; init; }
    public required int SiteId { get; init; }
}
