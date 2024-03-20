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

using System.Threading.Tasks;
using CodemerxDecompile.Providers;

namespace CodemerxDecompile.ViewModels.Design;

public class DesignAboutWindowViewModel : AboutWindowViewModel
{
    public DesignAboutWindowViewModel()
        : base(new DesignAppInformationProvider())
    {
    }
}

file class DesignAppInformationProvider : IAppInformationProvider
{
    public string Name { get; } = "CodemerxDecompile";
    public string Version { get; } = "1.69.0";
    public string Copyright { get; } = "Copyright \u00a9 CodeMerx Ltd. All rights reserved.";

    public AdditionalInfo AdditionalInfo { get; } = new()
    {
        Title = "Additional info",
        Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
    };

    public Task TryLoadRemoteAdditionalInfoAsync()
    {
        return Task.CompletedTask;
    }
}
