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

using CodemerxDecompile.Providers;

namespace CodemerxDecompile.ViewModels;

public class AboutWindowViewModel : IAboutWindowViewModel
{
    private readonly IAppInformationProvider appInformationProvider;

    public AboutWindowViewModel(IAppInformationProvider appInformationProvider)
    {
        this.appInformationProvider = appInformationProvider;
    }

    public string Name => appInformationProvider.Name;

    public string Version => appInformationProvider.Version;

    public string Copyright => appInformationProvider.Copyright;

    public AdditionalInfo AdditionalInfo => appInformationProvider.AdditionalInfo;
}
