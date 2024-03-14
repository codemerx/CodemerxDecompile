using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace OrchardCore.Localization
{
	public class NullHtmlLocalizerFactory : IHtmlLocalizerFactory
	{
		public NullHtmlLocalizerFactory()
		{
		}

		public IHtmlLocalizer Create(string baseName, string location)
		{
			return NullHtmlLocalizerFactory.NullLocalizer.Instance;
		}

		public IHtmlLocalizer Create(Type resourceSource)
		{
			return NullHtmlLocalizerFactory.NullLocalizer.Instance;
		}

		private class NullLocalizer : IHtmlLocalizer
		{
			private readonly static PluralizationRuleDelegate _defaultPluralRule;

			public static NullHtmlLocalizerFactory.NullLocalizer Instance
			{
				get;
			}

			public LocalizedHtmlString this[string name]
			{
				get
				{
					return new LocalizedHtmlString(name, name, true);
				}
			}

			public LocalizedHtmlString this[string name, params object[] arguments]
			{
				get
				{
					string forms = name;
					if ((int)arguments.Length == 1)
					{
						object obj = arguments[0];
						if (obj is PluralizationArgument)
						{
							PluralizationArgument pluralizationArgument = (PluralizationArgument)obj;
							forms = pluralizationArgument.get_Forms()[NullHtmlLocalizerFactory.NullLocalizer._defaultPluralRule.Invoke(pluralizationArgument.get_Count())];
							arguments = new object[(int)pluralizationArgument.get_Arguments().Length + 1];
							arguments[0] = pluralizationArgument.get_Count();
							Array.Copy(pluralizationArgument.get_Arguments(), 0, arguments, 1, (int)pluralizationArgument.get_Arguments().Length);
						}
					}
					return new LocalizedHtmlString(name, forms, false, arguments);
				}
			}

			static NullLocalizer()
			{
				NullHtmlLocalizerFactory.NullLocalizer._defaultPluralRule = new PluralizationRuleDelegate(NullHtmlLocalizerFactory.NullLocalizer.u003cu003ec.u003cu003e9, (int n) => {
					if (n != 1)
					{
						return 1;
					}
					return 0;
				});
				NullHtmlLocalizerFactory.NullLocalizer.Instance = new NullHtmlLocalizerFactory.NullLocalizer();
			}

			public NullLocalizer()
			{
			}

			public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
			{
				return Enumerable.Empty<LocalizedString>();
			}

			public LocalizedString GetString(string name)
			{
				return NullStringLocalizerFactory.NullLocalizer.Instance.GetString(name);
			}

			public LocalizedString GetString(string name, params object[] arguments)
			{
				return NullStringLocalizerFactory.NullLocalizer.Instance.GetString(name, arguments);
			}

			IHtmlLocalizer Microsoft.AspNetCore.Mvc.Localization.IHtmlLocalizer.WithCulture(CultureInfo culture)
			{
				return NullHtmlLocalizerFactory.NullLocalizer.Instance;
			}
		}
	}
}