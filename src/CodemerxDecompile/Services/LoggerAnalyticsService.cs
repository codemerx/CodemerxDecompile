/*
    Copyright CodeMerx 2024
    This file is part of CodemerxDecompile.

    CodemerxDecompile is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    CodemerxDecompile is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with CodemerxDecompile.  If not, see<https://www.gnu.org/licenses/>.
*/

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
