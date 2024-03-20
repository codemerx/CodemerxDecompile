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
    private readonly IAnalyticsService analyticsService;

    public AutoUpdateService(HttpClient httpClient, INotificationService notificationService,
        IAnalyticsService analyticsService)
    {
        this.httpClient = httpClient;
        this.notificationService = notificationService;
        this.analyticsService = analyticsService;
    }

    public Task CheckForNewerVersionAsync() => Task.Run(async () =>
    {
        var response = await httpClient.GetAsync("/repos/codemerx/CodemerxDecompile/releases/latest");
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
                        Action = () =>
                        {
                            _ = analyticsService.TrackEventAsync(AnalyticsEvents.DownloadNewVersion);
                            Process.Start(new ProcessStartInfo(latestRelease.HtmlUrl) { UseShellExecute = true });
                        }
                    }
                }
            });
        }
    });
}

file class Release
{
    public string? Name { get; set; }

    [JsonPropertyName("html_url")]
    public string? HtmlUrl { get; set; }
}
