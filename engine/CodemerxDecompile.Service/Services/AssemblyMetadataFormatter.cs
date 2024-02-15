//    Copyright CodeMerx 2020
//    This file is part of CodemerxDecompile.

//    CodemerxDecompile is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.

//    CodemerxDecompile is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//    GNU Affero General Public License for more details.

//    You should have received a copy of the GNU Affero General Public License
//    along with CodemerxDecompile.  If not, see<https://www.gnu.org/licenses/>.

using Mono.Cecil;
using Mono.Cecil.AssemblyResolver;

namespace CodemerxDecompile.Service.Services
{
    public static class AssemblyMetadataFormatter
    {
        public static string FormatTargetPlatform(TargetPlatform targetPlatform, FrameworkVersion frameworkVersion, bool shortFormat = false)
        {
            switch (targetPlatform)
            {
                case TargetPlatform.CLR_1:
                    {
                        if (!shortFormat)
                        {
                            return ".NET Framework 1.x";
                        }
                        return "(NET 1.x)";
                    }
                case TargetPlatform.CLR_2:
                    {
                        if (!shortFormat)
                        {
                            return ".NET Framework 2.0";
                        }
                        return "(NET 2.0)";
                    }
                case TargetPlatform.CLR_2_3:
                    {
                        if (!shortFormat)
                        {
                            return ".NET Framework 2.0/3.5";
                        }
                        return "(NET 2.0/3.5)";
                    }
                case TargetPlatform.CLR_3_5:
                    {
                        if (!shortFormat)
                        {
                            return ".NET Framework 3.5";
                        }
                        return "(NET 3.5)";
                    }
                case TargetPlatform.CLR_4:
                    {
                        if (frameworkVersion == FrameworkVersion.Unknown)
                        {
                            return string.Format((shortFormat ? "(NET {0})" : ".NET Framework {0}"), "4.x");
                        }
                        else if (frameworkVersion == FrameworkVersion.v4_8)
                        {
                            if (!shortFormat)
                            {
                                return ".NET Framework 4.8";
                            }
                            return "(NET 4.8)";
                        }
                        else if (frameworkVersion == FrameworkVersion.v4_7_2)
                        {
                            if (!shortFormat)
                            {
                                return ".NET Framework 4.7.2";
                            }
                            return "(NET 4.7.2)";
                        }
                        else if (frameworkVersion == FrameworkVersion.v4_7_1)
                        {
                            if (!shortFormat)
                            {
                                return ".NET Framework 4.7.1";
                            }
                            return "(NET 4.7.1)";
                        }
                        else if (frameworkVersion == FrameworkVersion.v4_7)
                        {
                            if (!shortFormat)
                            {
                                return ".NET Framework 4.7";
                            }
                            return "(NET 4.7)";
                        }
                        else if (frameworkVersion == FrameworkVersion.v4_6_2)
                        {
                            if (!shortFormat)
                            {
                                return ".NET Framework 4.6.2";
                            }
                            return "(NET 4.6.2)";
                        }
                        else if (frameworkVersion == FrameworkVersion.v4_6_1)
                        {
                            if (!shortFormat)
                            {
                                return ".NET Framework 4.6.1";
                            }
                            return "(NET 4.6.1)";
                        }
                        else if (frameworkVersion == FrameworkVersion.v4_6)
                        {
                            if (!shortFormat)
                            {
                                return ".NET Framework 4.6";
                            }
                            return "(NET 4.6)";
                        }
                        else if (frameworkVersion == FrameworkVersion.v4_5_2)
                        {
                            if (!shortFormat)
                            {
                                return ".NET Framework 4.5.2";
                            }
                            return "(NET 4.5.2)";
                        }
                        else if (frameworkVersion == FrameworkVersion.v4_5_1)
                        {
                            if (!shortFormat)
                            {
                                return ".NET Framework 4.5.1";
                            }
                            return "(NET 4.5.1)";
                        }
                        else if (frameworkVersion == FrameworkVersion.v4_5)
                        {
                            if (!shortFormat)
                            {
                                return ".NET Framework 4.5";
                            }
                            return "(NET 4.5)";
                        }
                        else if (!shortFormat)
                        {
                            return ".NET Framework 4.0";
                        }
                        return "(NET 4.0)";
                    }
                case TargetPlatform.Silverlight:
                case TargetPlatform.WindowsPhone:
                    {
                        if (!shortFormat)
                        {
                            return "Silverlight";
                        }
                        return "(SL)";
                    }
                case TargetPlatform.WindowsCE:
                    {
                        if (!shortFormat)
                        {
                            return "Windows CE";
                        }
                        return "(CE)";
                    }
                case TargetPlatform.WinRT:
                    {
                        if (frameworkVersion == FrameworkVersion.NetPortableV4_0)
                        {
                            if (!shortFormat)
                            {
                                return ".NET Portable 4.0";
                            }
                            return "(NETPortable 4.0)";
                        }
                        else if (frameworkVersion == FrameworkVersion.NetPortableV4_5)
                        {
                            if (!shortFormat)
                            {
                                return ".NET Portable 4.5";
                            }
                            return "(NETPortable 4.5)";
                        }
                        else if (frameworkVersion == FrameworkVersion.NetPortableV4_6)
                        {
                            if (!shortFormat)
                            {
                                return ".NET Portable 4.6";
                            }
                            return "(NETPortable 4.6)";
                        }
                        else if (frameworkVersion == FrameworkVersion.NetPortableV5_0)
                        {
                            if (!shortFormat)
                            {
                                return ".NET Portable 5.0";
                            }
                            return "(NETPortable 5.0)";
                        }
                        else if (frameworkVersion == FrameworkVersion.WinRT_4_5)
                        {
                            if (!shortFormat)
                            {
                                return "WinRT - 4.5";
                            }
                            return "(WinRT - 4.5)";
                        }
                        else if (frameworkVersion == FrameworkVersion.WinRT_4_5_1)
                        {
                            if (!shortFormat)
                            {
                                return "WinRT - 4.5.1";
                            }
                            return "(WinRT - 4.5.1)";
                        }
                        else if (frameworkVersion == FrameworkVersion.UWP)
                        {
                            if (!shortFormat)
                            {
                                return "UWP";
                            }
                            return "(UWP)";
                        }
                        else if (frameworkVersion == FrameworkVersion.WinRT_System)
                        {
                            if (!shortFormat)
                            {
                                return "WinRT System";
                            }
                            return "(WinRT System)";
                        }
                        else if (frameworkVersion == FrameworkVersion.WindowsPhone)
                        {
                            if (!shortFormat)
                            {
                                return "Windows Phone";
                            }
                            return "(WP)";
                        }
                        else if (!shortFormat)
                        {
                            return "Unknown";
                        }
                        return "(Unknown)";
                    }
                case TargetPlatform.NetCore:
                    {
                        if (frameworkVersion == FrameworkVersion.NetCoreV5_0)
                        {
                            if (!shortFormat)
                            {
                                return ".NET 5";
                            }
                            return "(NET 5)";
                        }
                        else if (frameworkVersion == FrameworkVersion.NetCoreV3_1)
                        {
                            if (!shortFormat)
                            {
                                return ".NET Core 3.1";
                            }
                            return "(NETCore 3.1)";
                        }
                        else if (frameworkVersion == FrameworkVersion.NetCoreV3_0)
                        {
                            if (!shortFormat)
                            {
                                return ".NET Core 3.0";
                            }
                            return "(NETCore 3.0)";
                        }
                        else if (frameworkVersion == FrameworkVersion.NetCoreV2_2)
                        {
                            if (!shortFormat)
                            {
                                return ".NET Core 2.2";
                            }
                            return "(NETCore 2.2)";
                        }
                        else if (frameworkVersion == FrameworkVersion.NetCoreV2_1)
                        {
                            if (!shortFormat)
                            {
                                return ".NET Core 2.1";
                            }
                            return "(NETCore 2.1)";
                        }
                        else if (frameworkVersion == FrameworkVersion.NetCoreV2_0)
                        {
                            if (!shortFormat)
                            {
                                return ".NET Core 2.0";
                            }
                            return "(NETCore 2.0)";
                        }
                        else if (frameworkVersion == FrameworkVersion.NetCoreV1_1)
                        {
                            if (!shortFormat)
                            {
                                return ".NET Core 1.1";
                            }
                            return "(NETCore 1.1)";
                        }
                        else if (frameworkVersion == FrameworkVersion.NetCoreV1_0)
                        {
                            if (!shortFormat)
                            {
                                return ".NET Core 1.0";
                            }
                            return "(NETCore 1.0)";
                        }
                        else if (!shortFormat)
                        {
                            return "Unknown";
                        }
                        return "(Unknown)";
                    }
                case TargetPlatform.NetStandard:
                    {
                        if (frameworkVersion == FrameworkVersion.NetStandardV2_1)
                        {
                            if (!shortFormat)
                            {
                                return ".NET Standard 2.1";
                            }
                            return "(NETStandard 2.1)";
                        }
                        else if (frameworkVersion == FrameworkVersion.NetStandardV2_0)
                        {
                            if (!shortFormat)
                            {
                                return ".NET Standard 2.0";
                            }
                            return "(NETStandard 2.0)";
                        }
                        else if (frameworkVersion == FrameworkVersion.NetStandardV1_6)
                        {
                            if (!shortFormat)
                            {
                                return ".NET Standard 1.6";
                            }
                            return "(NETStandard 1.6)";
                        }
                        else if (frameworkVersion == FrameworkVersion.NetStandardV1_5)
                        {
                            if (!shortFormat)
                            {
                                return ".NET Standard 1.5";
                            }
                            return "(NETStandard 1.5)";
                        }
                        else if (frameworkVersion == FrameworkVersion.NetStandardV1_4)
                        {
                            if (!shortFormat)
                            {
                                return ".NET Standard 1.4";
                            }
                            return "(NETStandard 1.4)";
                        }
                        else if (frameworkVersion == FrameworkVersion.NetStandardV1_3)
                        {
                            if (!shortFormat)
                            {
                                return ".NET Standard 1.3";
                            }
                            return "(NETStandard 1.3)";
                        }
                        else if (frameworkVersion == FrameworkVersion.NetStandardV1_2)
                        {
                            if (!shortFormat)
                            {
                                return ".NET Standard 1.2";
                            }
                            return "(NETStandard 1.2)";
                        }
                        else if (frameworkVersion == FrameworkVersion.NetStandardV1_1)
                        {
                            if (!shortFormat)
                            {
                                return ".NET Standard 1.1";
                            }
                            return "(NETStandard 1.1)";
                        }
                        else if (frameworkVersion == FrameworkVersion.NetStandardV1_0)
                        {
                            if (!shortFormat)
                            {
                                return ".NET Standard 1.0";
                            }
                            return "(NETStandard 1.0)";
                        }
                        else if (!shortFormat)
                        {
                            return "Unknown";
                        }
                        return "(Unknown)";
                    }
                case TargetPlatform.Xamarin:
                    {
                        if (frameworkVersion == FrameworkVersion.XamarinAndroid)
                        {
                            if (!shortFormat)
                            {
                                return "Xamarin Android";
                            }
                            return "(XamarinAndroid)";
                        }
                        if (frameworkVersion == FrameworkVersion.XamarinIOS)
                        {
                            if (!shortFormat)
                            {
                                return "Xamarin IOS";
                            }
                            return "(XamarinIOS)";
                        }
                        if (!shortFormat)
                        {
                            return "Unknown";
                        }
                        return "(Unknown)";
                    }
                case TargetPlatform.WPF:
                default:
                    {
                        if (!shortFormat)
                        {
                            return "Unknown";
                        }
                        return "(Unknown)";
                    }
            }
        }

        public static string GetFormattedArchitecture(TargetArchitecture targetArchitecture, bool shortFormat = false)
        {
            switch (targetArchitecture)
            {
                case TargetArchitecture.I386:
                    {
                        return "x86";
                    }
                case TargetArchitecture.AMD64:
                    {
                        return "x64";
                    }
                case TargetArchitecture.IA64:
                    {
                        return "Itanium";
                    }
                case TargetArchitecture.ARMv7:
                    {
                        return "ARM";
                    }
                case TargetArchitecture.AnyCPU:
                default:
                    {
                        if (!shortFormat)
                        {
                            return "Any CPU";
                        }
                        return "AnyCPU";
                    }
            }
        }
    }
}