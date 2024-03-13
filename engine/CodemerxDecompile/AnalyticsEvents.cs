namespace CodemerxDecompile;

public static class AnalyticsEvents
{
    public static readonly AnalyticsEvent Startup = new("application", "startup");
    public static readonly AnalyticsEvent Shutdown = new("application", "shutdown");
    public static readonly AnalyticsEvent OpenFile = new("assemblies", "open-file");
    public static readonly AnalyticsEvent CloseAll = new("assemblies", "close-all");
    public static readonly AnalyticsEvent OpenViaDragDrop = new("assemblies", "open-via-drag-drop");
    public static readonly AnalyticsEvent ResolveReference = new("assemblies", "resolve-reference");
    public static readonly AnalyticsEvent GoBack = new("navigation", "go-back");
    public static readonly AnalyticsEvent GoForward = new("navigation", "go-forward");
    public static readonly AnalyticsEvent GoToDefinition = new("navigation", "go-to-definition");
    public static readonly AnalyticsEvent CreateProject2019 = new("create-project", "2019");
    public static readonly AnalyticsEvent CreateProject2017 = new("create-project", "2017");
    public static readonly AnalyticsEvent CreateProject2015 = new("create-project", "2015");
    public static readonly AnalyticsEvent ChangeLanguageToCSharp = new("change-language", "C#");
    public static readonly AnalyticsEvent ChangeLanguageToVisualBasic = new("change-language", "VB.NET");
    public static readonly AnalyticsEvent ChangeLanguageToIntermediateLanguage = new("change-language", "IL");
    public static readonly AnalyticsEvent Search = new("search", "run");
    public static readonly AnalyticsEvent NavigateToSearchResult = new("search", "navigate-to-result");
    public static readonly AnalyticsEvent DownloadNewVersion = new("new-version", "download");
    // TODO: Report the following events to monitor usage of shortcuts
    // public static readonly Event OpenFileShortcut = new("shortcut", "open-file");
    // public static readonly Event GoBackViaKeyboardShortcut = new("shortcut", "go-back-via-keyboard");
    // public static readonly Event GoForwardViaKeyboardShortcut = new("shortcut", "go-forward-via-keyboard");
    // public static readonly Event GoBackViaMouseShortcut = new("shortcut", "go-back-via-mouse");
    // public static readonly Event GoForwardViaMouseShortcut = new("shortcut", "go-forward-via-mouse");
    // public static readonly Event SearchShortcut = new("shortcut", "search");
    // public static readonly Event GoToDefinitionViaKeyboardShortcut = new("shortcut", "go-to-definition-via-keyboard");
    // public static readonly Event GoToDefinitionViaMouseShortcut = new("shortcut", "go-to-definition-via-mouse");
}
