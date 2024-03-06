using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CodemerxDecompile.Notifications;

namespace CodemerxDecompile.Services;

public class AutoUpdateService : IAutoUpdateService
{
    private readonly HttpClient httpClient;
    private readonly INotificationService notificationService;

    public AutoUpdateService(HttpClient httpClient, INotificationService notificationService)
    {
        this.httpClient = httpClient;
        this.notificationService = notificationService;
    }
    
    public async Task CheckForNewerVersionAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/repos/codemerx/CodemerxDecompile/releases/latest")
        {
            Headers =
            {
                { "Accept", "application/vnd.github+json" },
                { "X-GitHub-Api-Version", "2022-11-28" },
                // As per https://docs.github.com/en/rest/using-the-rest-api/troubleshooting-the-rest-api?apiVersion=2022-11-28#user-agent-required
                { "User-Agent", "CodemerxDecompile" }
            }
        };

        var response = await httpClient.SendAsync(request);
        if (response.StatusCode != HttpStatusCode.OK)
            return;

        var latestRelease = await response.Content.ReadFromJsonAsync<Release>();
        if (latestRelease == null || latestRelease.Name == null || latestRelease.HtmlUrl == null)
            return;
        
        var latestReleaseVersion = new Version(latestRelease.Name);
        var currentVersion = AssemblyProvider.Assembly.GetName().Version;
        if (currentVersion == null)
            return;

        if (latestReleaseVersion > currentVersion)
        {
            notificationService.ShowNotification(new Notification
            {
                Message = "A new version of CodemerxDecompile is now available!",
                Level = NotificationLevel.Information,
                Actions = new[]
                {
                    new NotificationAction
                    {
                        Title = "Download",
                        Action = () => Process.Start(new ProcessStartInfo(latestRelease.HtmlUrl) { UseShellExecute = true })
                    }
                }
            });
        }
    }
}

file class Release
{
    public string? Name { get; set; }

    [JsonPropertyName("html_url")]
    public string? HtmlUrl { get; set; }
}
