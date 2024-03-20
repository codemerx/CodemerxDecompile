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
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Threading.Tasks;
using CodemerxDecompile.Options;
using Microsoft.Extensions.Options;

namespace CodemerxDecompile.Providers;

public class AppInformationProvider : IAppInformationProvider
{
    private readonly IOptions<AppInformationProviderOptions> options;
    private readonly IHttpClientFactory httpClientFactory;

    private readonly Lazy<string> versionHolder = new(() =>
        AssemblyProvider.Assembly.GetName().Version?.ToString(3) ?? "unknown");
    
    private readonly Lazy<string> copyrightHolder = new(() =>
    {
        var customAttributes = AssemblyProvider.Assembly.GetCustomAttributes();
        var copyrightAttribute = customAttributes.OfType<AssemblyCopyrightAttribute>().FirstOrDefault();
        return copyrightAttribute?.Copyright ?? string.Empty;
    });

    public AppInformationProvider(IOptions<AppInformationProviderOptions> options, IHttpClientFactory httpClientFactory)
    {
        this.options = options;
        this.httpClientFactory = httpClientFactory;
    }
    
    public string Name => "CodemerxDecompile";

    public string Version => versionHolder.Value;

    public string Copyright => copyrightHolder.Value;

    public AdditionalInfo AdditionalInfo { get; private set; } = new()
    {
        Title = "About CodeMerx",
        Text = "CodeMerx is an outsourcing company founded by the team that created the fastest .NET decompiler - JustDecompile."
    };

    public async Task TryLoadRemoteAdditionalInfoAsync()
    {
        using var httpClient = httpClientFactory.CreateClient(nameof(AppInformationProvider));
        var remoteAdditionalInfo = await httpClient.GetFromJsonAsync<AdditionalInfo?>(options.Value.AdditionalInfoPath);
        if (remoteAdditionalInfo.HasValue)
            AdditionalInfo = remoteAdditionalInfo.Value;
    }
}
