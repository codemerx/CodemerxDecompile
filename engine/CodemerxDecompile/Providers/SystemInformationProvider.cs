using System;
using System.Runtime.InteropServices;

namespace CodemerxDecompile.Providers;

public class SystemInformationProvider : ISystemInformationProvider
{
    private readonly Lazy<SystemInformation> systemInformationHolder = new(() =>
    {
        return new SystemInformation
        {
            AppVersion = GetAppVersion(),
            OsPlatform = GetOsPlatform(),
            OsArchitecture = GetOsArchitecture(),
            OsRelease = GetOsRelease()
        };

        string GetAppVersion() => AssemblyProvider.Assembly.GetName().Version?.ToString() ?? "unknown";

        string GetOsPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return "windows";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return "linux";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return "macos";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
                return "freebsd";

            return "unknown";
        }

        string GetOsArchitecture() => RuntimeInformation.OSArchitecture.ToString();

        string GetOsRelease() => RuntimeInformation.OSDescription;
    });

    public SystemInformation GetSystemInformation() => systemInformationHolder.Value;
}
