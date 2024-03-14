using System;
using System.Globalization;
using System.Threading.Tasks;

namespace OrchardCore.Localization
{
	public class DefaultLocalizationService : ILocalizationService
	{
		private readonly static Task<string> DefaultCulture;

		private readonly static Task<string[]> SupportedCultures;

		static DefaultLocalizationService()
		{
			DefaultLocalizationService.DefaultCulture = Task.FromResult<string>(CultureInfo.InstalledUICulture.Name);
			DefaultLocalizationService.SupportedCultures = Task.FromResult<string[]>(new string[] { CultureInfo.InstalledUICulture.Name });
		}

		public DefaultLocalizationService()
		{
		}

		public Task<string> GetDefaultCultureAsync()
		{
			return DefaultLocalizationService.DefaultCulture;
		}

		public Task<string[]> GetSupportedCulturesAsync()
		{
			return DefaultLocalizationService.SupportedCultures;
		}
	}
}