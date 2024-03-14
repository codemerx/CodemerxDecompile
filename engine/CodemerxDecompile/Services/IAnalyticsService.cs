using System.Threading.Tasks;

namespace CodemerxDecompile.Services;

public interface IAnalyticsService
{
    void TrackEvent(AnalyticsEvent @event);
    Task TrackEventAsync(AnalyticsEvent @event);
}
