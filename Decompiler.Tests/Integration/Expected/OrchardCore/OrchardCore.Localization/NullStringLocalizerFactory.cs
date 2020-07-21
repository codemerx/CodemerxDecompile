using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace OrchardCore.Localization
{
	public class NullStringLocalizerFactory : IStringLocalizerFactory
	{
		public NullStringLocalizerFactory()
		{
			base();
			return;
		}

		public IStringLocalizer Create(Type resourceSource)
		{
			return NullStringLocalizerFactory.NullLocalizer.get_Instance();
		}

		public IStringLocalizer Create(string baseName, string location)
		{
			return NullStringLocalizerFactory.NullLocalizer.get_Instance();
		}

		internal class NullLocalizer : IStringLocalizer
		{
			private readonly static PluralizationRuleDelegate _defaultPluralRule;

			public static NullStringLocalizerFactory.NullLocalizer Instance
			{
				get
				{
					return NullStringLocalizerFactory.NullLocalizer.u003cInstanceu003ek__BackingField;
				}
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
					V_0 = name;
					if ((int)arguments.Length == 1)
					{
						V_2 = arguments[0];
						if (V_2 as PluralizationArgument != null)
						{
							V_1 = (PluralizationArgument)V_2;
							V_0 = V_1.get_Forms()[NullStringLocalizerFactory.NullLocalizer._defaultPluralRule.Invoke(V_1.get_Count())];
							arguments = new object[(int)V_1.get_Arguments().Length + 1];
							arguments[0] = V_1.get_Count();
							Array.Copy(V_1.get_Arguments(), 0, arguments, 1, (int)V_1.get_Arguments().Length);
						}
					}
					V_0 = string.Format(V_0, arguments);
					return new LocalizedString(name, V_0, false);
				}
			}

			static NullLocalizer()
			{
				NullStringLocalizerFactory.NullLocalizer._defaultPluralRule = new PluralizationRuleDelegate(NullStringLocalizerFactory.NullLocalizer.u003cu003ec.u003cu003e9, NullStringLocalizerFactory.NullLocalizer.u003cu003ec.u003cu002ecctoru003eb__13_0);
				NullStringLocalizerFactory.NullLocalizer.u003cInstanceu003ek__BackingField = new NullStringLocalizerFactory.NullLocalizer();
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
				return this.get_Item(name);
			}

			public LocalizedString GetString(string name, params object[] arguments)
			{
				return this.get_Item(name, arguments);
			}

			public IStringLocalizer WithCulture(CultureInfo culture)
			{
				return NullStringLocalizerFactory.NullLocalizer.get_Instance();
			}
		}
	}
}