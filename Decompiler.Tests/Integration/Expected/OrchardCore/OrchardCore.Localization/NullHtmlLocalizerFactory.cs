using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace OrchardCore.Localization
{
	public class NullHtmlLocalizerFactory : IHtmlLocalizerFactory
	{
		public NullHtmlLocalizerFactory()
		{
			base();
			return;
		}

		public IHtmlLocalizer Create(string baseName, string location)
		{
			return NullHtmlLocalizerFactory.NullLocalizer.get_Instance();
		}

		public IHtmlLocalizer Create(Type resourceSource)
		{
			return NullHtmlLocalizerFactory.NullLocalizer.get_Instance();
		}

		private class NullLocalizer : IHtmlLocalizer
		{
			private readonly static PluralizationRuleDelegate _defaultPluralRule;

			public static NullHtmlLocalizerFactory.NullLocalizer Instance
			{
				get
				{
					return NullHtmlLocalizerFactory.NullLocalizer.u003cInstanceu003ek__BackingField;
				}
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
					V_0 = name;
					if ((int)arguments.Length == 1)
					{
						V_2 = arguments[0];
						if (V_2 as PluralizationArgument != null)
						{
							V_1 = (PluralizationArgument)V_2;
							V_0 = V_1.get_Forms()[NullHtmlLocalizerFactory.NullLocalizer._defaultPluralRule.Invoke(V_1.get_Count())];
							arguments = new object[(int)V_1.get_Arguments().Length + 1];
							arguments[0] = V_1.get_Count();
							Array.Copy(V_1.get_Arguments(), 0, arguments, 1, (int)V_1.get_Arguments().Length);
						}
					}
					return new LocalizedHtmlString(name, V_0, false, arguments);
				}
			}

			static NullLocalizer()
			{
				NullHtmlLocalizerFactory.NullLocalizer._defaultPluralRule = new PluralizationRuleDelegate(NullHtmlLocalizerFactory.NullLocalizer.u003cu003ec.u003cu003e9, NullHtmlLocalizerFactory.NullLocalizer.u003cu003ec.u003cu002ecctoru003eb__13_0);
				NullHtmlLocalizerFactory.NullLocalizer.u003cInstanceu003ek__BackingField = new NullHtmlLocalizerFactory.NullLocalizer();
				return;
			}

			public NullLocalizer()
			{
				base();
				return;
			}

			public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
			{
				return Enumerable.Empty<LocalizedString>();
			}

			public LocalizedString GetString(string name)
			{
				return NullStringLocalizerFactory.NullLocalizer.get_Instance().GetString(name);
			}

			public LocalizedString GetString(string name, params object[] arguments)
			{
				return NullStringLocalizerFactory.NullLocalizer.get_Instance().GetString(name, arguments);
			}

			IHtmlLocalizer Microsoft.AspNetCore.Mvc.Localization.IHtmlLocalizer.WithCulture(CultureInfo culture)
			{
				return NullHtmlLocalizerFactory.NullLocalizer.get_Instance();
			}
		}
	}
}