using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace SystemInformationHelpers
{
    public class MachineInformation
    {
        public static string ProcessorCount
        {
            get
            {
                return Environment.ProcessorCount.ToString();
            }
        }

        public static string CurrentDotNetVersion
        {
            get
            {
                return Framework4VersionResolver.GetInstalledFramework4Version().ToString();
            }
        }

        public static string OSVersion
        {
            get
            {
                return GetOSVersion();
            }
        }

        private static string GetOSVersion()
        {
            string osVersion = string.Empty;

            try
            {
				ProcessStartInfo getOsVersionProcess = new ProcessStartInfo("cmd.exe", "/c ver") { RedirectStandardOutput = true, UseShellExecute = false, CreateNoWindow = true, WindowStyle = ProcessWindowStyle.Hidden };

                osVersion = Process
                     .Start(getOsVersionProcess)
                     .StandardOutput
                     .ReadToEnd();

                return FormatOSVersionString(osVersion);
            }
            catch (Exception ex)
            {
                string osVersionExceptionFormat = "ERROR: " + ex.GetType().Name + " - " + ex.Message;
                osVersion = osVersionExceptionFormat;
            }

            return osVersion;
        }

        private static string FormatOSVersionString(string osVersion)
        {
            string formattedOSVersion = osVersion.Trim();
            string osVersionRegexPattern = "^Microsoft Windows \\[Version ([0-9\\.]+)\\]$";

            Regex regex = new Regex(osVersionRegexPattern);
            Match regexMatch = regex.Match(formattedOSVersion);

            if (regexMatch.Success)
            {
                formattedOSVersion = regexMatch.Groups[1].Value;
            }

            return formattedOSVersion;
        }
    }
}
