/*
    Copyright 2024 CodeMerx
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

using System.Net.Http;
using System.Threading.Tasks;
using CodemerxDecompile.Options;
using CodemerxDecompile.Providers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CodemerxDecompile.Services;

public class MatomoDevelopmentAnalyticsService : MatomoAnalyticsService
{
    private readonly ILogger<MatomoDevelopmentAnalyticsService> logger;

    public MatomoDevelopmentAnalyticsService(IOptions<MatomoAnalyticsOptions> options, HttpClient httpClient,
        IDeviceIdentifierProvider deviceIdentifierProvider, ISystemInformationProvider systemInformationProvider,
        IAppInformationProvider appInformationProvider, ILogger<MatomoDevelopmentAnalyticsService> logger)
        : base(options, httpClient, deviceIdentifierProvider, systemInformationProvider, appInformationProvider)
    {
        this.logger = logger;
    }

    public override void TrackEvent(AnalyticsEvent @event)
    {
        logger.LogInformation("Request: {Request}", CreateRequestUri(@event));
    }

    public override Task TrackEventAsync(AnalyticsEvent @event)
    {
        TrackEvent(@event);
        
        return Task.CompletedTask;
    }
}
