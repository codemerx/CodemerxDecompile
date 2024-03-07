using System.Threading.Tasks;

namespace CodemerxDecompile.Services;

public interface IAnalyticsService
{
    Task TrackEventAsync(string @event);
}
