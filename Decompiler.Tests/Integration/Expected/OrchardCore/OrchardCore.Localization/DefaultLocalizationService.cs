using System;
using System.Threading.Tasks;

namespace OrchardCore.Localization
{
	public class DefaultLocalizationService : ILocalizationService
	{
		private readonly static Task<string> DefaultCulture;

		private readonly static Task<string[]> SupportedCultures;

		static DefaultLocalizationService()
		{
			DefaultLocalizationService.DefaultCulture = Task.FromResult<string>(CultureInfo.get_InstalledUICulture().get_Name());
			stackVariable4 = new string[1];
			stackVariable4[0] = CultureInfo.get_InstalledUICulture().get_Name();
			DefaultLocalizationService.SupportedCultures = Task.FromResult<string[]>(stackVariable4);
			return;
		}

		public DefaultLocalizationService()
		{
			base();
			return;
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