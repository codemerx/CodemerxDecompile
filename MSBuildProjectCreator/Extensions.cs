namespace JustDecompile.Tools.MSBuildProjectBuilder
{
    public static class Extensions
    {
        public static string ToFriendlyString(this VisualStudioVersion self)
        {
            if (self == VisualStudioVersion.Unknown)
            {
                return self.ToString();
            }

            return self.ToString().Substring(2);
        }
    }
}
