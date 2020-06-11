namespace JustDecompile.Tools.MSBuildProjectBuilder
{
    public class ProjectGenerationSettings
    {
        public ProjectGenerationSettings(bool visualStudioSupportedProjectType)
            : this(visualStudioSupportedProjectType, null, true)
        {
        }

        public ProjectGenerationSettings(bool visualStudioSupportedProjectType, string errorMessage)
            : this(visualStudioSupportedProjectType, errorMessage, true)
        {
        }

        public ProjectGenerationSettings(bool visualStudioSupportedProjectType, string errorMessage, bool justDecompileSupportedProjectType)
        {
            this.VisualStudioSupportedProjectType = visualStudioSupportedProjectType;
            this.ErrorMessage = errorMessage;
            this.JustDecompileSupportedProjectType = justDecompileSupportedProjectType;
        }

        public bool VisualStudioSupportedProjectType { get; private set; }

        public string ErrorMessage { get; private set; }

        public bool JustDecompileSupportedProjectType { get; private set; }
    }
}