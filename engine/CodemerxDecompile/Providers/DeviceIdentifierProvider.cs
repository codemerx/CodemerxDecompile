using System;
using DeviceId;

namespace CodemerxDecompile.Providers;

public class DeviceIdentifierProvider : IDeviceIdentifierProvider
{
    private readonly Lazy<string> deviceIdentifierHolder = new(() =>
        new DeviceIdBuilder()
            .AddMachineName()
            .AddUserName()
            .ToString());


    public string GetDeviceIdentifier() => deviceIdentifierHolder.Value;
}
