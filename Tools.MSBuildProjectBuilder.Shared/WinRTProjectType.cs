namespace JustDecompile.Tools.MSBuildProjectBuilder
{
    public enum WinRTProjectType
    {
        Unknown,
        Component, // Generated with Visual Studio 2012 on 8/8.1
        // Generated with Visual Studio 2013
        ComponentForUniversal,
        ComponentForWindows,
        ComponentForWindowsPhone,
        // Generated with Visual Studio 2015
        UWPComponent,
        UWPLibrary,
        UWPApplication
    }
}