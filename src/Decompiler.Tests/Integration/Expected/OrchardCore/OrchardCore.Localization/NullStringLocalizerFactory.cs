using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace OrchardCore.Localization
{
	public class NullStringLocalizerFactory : IStringLocalizerFactory
	{
		public NullStringLocalizerFactory()
		{
		}

		public IStringLocalizer Create(Type resourceSource)
		{
			return NullStringLocalizerFactory.NullLocalizer.Instance;
		}

		public IStringLocalizer Create(string baseName, string location)
		{
			return NullStringLocalizerFactory.NullLocalizer.Instance;
		}

		internal class NullLocalizer : IStringLocalizer
		{
			private readonly static PluralizationRuleDelegate _defaultPluralRule;

			public static NullStringLocalizerFactory.NullLocalizer Instance
			{
				get;
			}

			public LocalizedString this[string name]
			{
				get
				{
					return new LocalizedString(name, name, false);
				}
			}

			public LocalizedString this[string name, params object[] arguments]
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
							forms = pluralizationArgument.get_Forms()[NullStringLocalizerFactory.NullLocalizer._defaultPluralRule.Invoke(pluralizationArgument.get_Count())];
							arguments = new object[(int)pluralizationArgument.get_Arguments().Length + 1];
							arguments[0] = pluralizationArgument.get_Count();
							Array.Copy(pluralizationArgument.get_Arguments(), 0, arguments, 1, (int)pluralizationArgument.get_Arguments().Length);
						}
					}
					forms = string.Format(forms, arguments);
					return new LocalizedString(name, forms, false);
				}
			}

			static NullLocalizer()
			{
				NullStringLocalizerFactory.NullLocalizer._defaultPluralRule = new PluralizationRuleDelegate(NullStringLocalizerFactory.NullLocalizer.u003cu003ec.u003cu003e9, (int n) => {
					if (n != 1)
					{
						return 1;
					}
					return 0;
				});
				NullStringLocalizerFactory.NullLocalizer.Instance = new NullStringLocalizerFactory.NullLocalizer();
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
				return this[name];
			}

			public LocalizedString GetString(string name, params object[] arguments)
			{
				return this[name, arguments];
			}

			public IStringLocalizer WithCulture(CultureInfo culture)
			{
				return NullStringLocalizerFactory.NullLocalizer.Instance;
			}
		}
	}
}