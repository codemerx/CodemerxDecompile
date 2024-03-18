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
using System.Runtime.InteropServices;

namespace CodemerxDecompile.Providers;

public class SystemInformationProvider : ISystemInformationProvider
{
    private const string Unknown = "unknown";
    
    private readonly Lazy<SystemInformation> systemInformationHolder = new(() =>
    {
        return new SystemInformation
        {
            AppVersion = GetAppVersion(),
            OsPlatform = GetOsPlatform(),
            OsArchitecture = GetOsArchitecture(),
            OsRelease = GetOsRelease()
        };

        string GetAppVersion() => AssemblyProvider.Assembly.GetName().Version?.ToString() ?? Unknown;

        string GetOsPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return "Windows";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return "Linux";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return "MacOS";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
                return "FreeBSD";

            return Unknown;
        }

        string GetOsArchitecture() => RuntimeInformation.OSArchitecture.ToString();

        string GetOsRelease() => RuntimeInformation.OSDescription;
    });

    public SystemInformation GetSystemInformation() => systemInformationHolder.Value;
}
