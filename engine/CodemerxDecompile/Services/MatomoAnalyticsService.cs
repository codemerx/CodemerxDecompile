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
    
    public async Task TrackEventAsync(string @event)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"{options.Value.ServerUrl}{CreateQueryString(@event)}");
        using var response = await httpClient.SendAsync(request);
    }

    private string CreateQueryString(string @event)
    {
        var deviceIdentifier = deviceIdentifierProvider.GetDeviceIdentifier();
        var systemInformation = systemInformationProvider.GetSystemInformation();

        var queryParams = new Dictionary<string, string>
        {
            { "idsite", options.Value.SiteId.ToString() },
            { "rec", 1.ToString() },
            { "action_name", @event },
            { "e_c", "CodemerxDecompile" },
            { "e_a", @event },
            { "cid", ConvertVisitorId(deviceIdentifier) },
            { "dimension1", systemInformation.AppVersion },
            { "dimension2", systemInformation.OsPlatform },
            { "dimension3", systemInformation.OsArchitecture },
            { "dimension4", systemInformation.OsRelease }
        };
        
        return $"?{string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"))}";
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
