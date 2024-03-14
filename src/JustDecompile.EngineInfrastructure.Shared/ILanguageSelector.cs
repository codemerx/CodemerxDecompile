using Telerik.JustDecompiler.Languages;

namespace JustDecompile.EngineInfrastructure
{
	public interface ILanguageSelector
	{
		string Name { get; }

		ILanguage GetLanguage();

        bool ContainsLanguage(string language);
    }
}