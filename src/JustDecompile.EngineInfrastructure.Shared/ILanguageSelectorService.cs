using Telerik.JustDecompiler.Languages;

namespace JustDecompile.EngineInfrastructure
{
	public interface ILanguageSelectorService
	{
		ILanguageSelector GetLanguageSelector();

        ILanguage GetLanguage();

        string GetMemberDeclartionForSelectedLanguage(object memberReference, bool renameInvalidMembers);

        string GetMemberEscapedOnlyNameForSelectedLanguage(object memberReference);

		string GetMemberNavigationPathName(object memberReference);

        string GetNamespaceForSelectedLanguage(string @namespace, bool renameInvalidMembers);

        ILanguage GetLanguageFromString(string language);
	}
}