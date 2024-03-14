using System.Threading.Tasks;
using CodemerxDecompile.Providers;
using Microsoft.Extensions.Logging;

namespace CodemerxDecompile.Services;

public class LoggerAnalyticsService : IAnalyticsService
{
    private readonly ILogger<LoggerAnalyticsService> logger;
    private readonly IDeviceIdentifierProvider deviceIdentifierProvider;
    private readonly ISystemInformationProvider systemInformationProvider;
    
    public LoggerAnalyticsService(ILogger<LoggerAnalyticsService> logger,
        IDeviceIdentifierProvider deviceIdentifierProvider, ISystemInformationProvider systemInformationProvider)
    {
        this.logger = logger;
        this.deviceIdentifierProvider = deviceIdentifierProvider;
        this.systemInformationProvider = systemInformationProvider;
    }

    public void TrackEvent(AnalyticsEvent @event)
    {
        logger.LogInformation(
            "Event: {Event}; Device identifier: {DeviceIdentifier}; System information: {SystemInformation}",
            @event, deviceIdentifierProvider.GetDeviceIdentifier(), systemInformationProvider.GetSystemInformation());
    }

    public Task TrackEventAsync(AnalyticsEvent @event)
    {
        TrackEvent(@event);
        
        return Task.CompletedTask;
    }
}
