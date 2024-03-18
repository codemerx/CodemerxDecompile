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
    public static readonly AnalyticsEvent About = new("application", "open-about-dialog");
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
