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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CodemerxDecompile.Options;
using CodemerxDecompile.Providers;
using Microsoft.Extensions.Options;

namespace CodemerxDecompile.Services;

public class MatomoAnalyticsService : IAnalyticsService
{
    private readonly IOptions<MatomoAnalyticsOptions> options;
    private readonly HttpClient httpClient;
    private readonly IDeviceIdentifierProvider deviceIdentifierProvider;
    private readonly ISystemInformationProvider systemInformationProvider;

    public MatomoAnalyticsService(IOptions<MatomoAnalyticsOptions> options, HttpClient httpClient,
        IDeviceIdentifierProvider deviceIdentifierProvider, ISystemInformationProvider systemInformationProvider)
    {
        this.options = options;
        this.httpClient = httpClient;
        this.deviceIdentifierProvider = deviceIdentifierProvider;
        this.systemInformationProvider = systemInformationProvider;
    }

    public void TrackEvent(AnalyticsEvent @event)
    {
        httpClient.Send(new HttpRequestMessage(HttpMethod.Get, CreateRequestUri(@event)));
    }

    public Task TrackEventAsync(AnalyticsEvent @event) => Task.Run(async () =>
    {
        await httpClient.GetAsync(CreateRequestUri(@event));
    });

    private string CreateRequestUri(AnalyticsEvent @event) => $"{options.Value.Endpoint}{CreateQueryString(@event)}";

    private string CreateQueryString(AnalyticsEvent @event)
    {
        var deviceIdentifier = deviceIdentifierProvider.GetDeviceIdentifier();
        var systemInformation = systemInformationProvider.GetSystemInformation();

        var queryParams = new Dictionary<string, object>
        {
            { "idsite", options.Value.SiteId },
            { "rec", 1 },
            { "cid", ConvertVisitorId(deviceIdentifier) },
            { "dimension1", systemInformation.AppVersion },
            { "dimension2", systemInformation.OsPlatform },
            { "dimension3", systemInformation.OsArchitecture },
            { "dimension4", systemInformation.OsRelease },
            { "e_c", @event.Category },
            { "e_a", @event.Action }
        };
        
        if (@event == AnalyticsEvents.Startup)
            queryParams.Add("new_visit", 1);
        
        return $"?{string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value.ToString()!)}"))}";
    }

    private static string ConvertVisitorId(string input)
    {
        var result = SHA256.HashData(Encoding.UTF8.GetBytes(input));

        // Converting 8 bytes, which will result in 16 byte Hex string. This requirement is by Matomo for visitor id.
        // The increased probability for collisions shouldn't be critical for the analytics.
        return ToHexString(result, length: 8);
        
        static string ToHexString(byte[] array, int length)
        {
            var hex = new StringBuilder(length * 2);

            for (var i = 0; i < length; i++)
            {
                var b = array[i];
                hex.Append($"{b:x2}");
            }

            return hex.ToString();
        }
    }
}
