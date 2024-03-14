using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Squidex.Areas.Api.Controllers.UI;
using Squidex.Domain.Apps.Entities.History;
using Squidex.Web;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Squidex.Areas.Frontend.Middlewares
{
	[Nullable(0)]
	[NullableContext(1)]
	public static class IndexExtensions
	{
		private readonly static ConcurrentDictionary<string, string> Texts;

		static IndexExtensions()
		{
			IndexExtensions.Texts = new ConcurrentDictionary<string, string>();
		}

		public static string AddOptions(this string html, HttpContext httpContext)
		{
			string str;
			object obj;
			MyUIOptions value;
			NotifoOptions notifoOption;
			if (!html.Contains("/* INJECT OPTIONS */", StringComparison.Ordinal))
			{
				return html;
			}
			List<string> strs = new List<string>()
			{
				string.Concat("var texts = ", IndexExtensions.GetText(CultureInfo.CurrentUICulture.Name), ";")
			};
			IOptions<MyUIOptions> service = ServiceProviderServiceExtensions.GetService<IOptions<MyUIOptions>>(httpContext.get_RequestServices());
			if (service != null)
			{
				value = service.get_Value();
			}
			else
			{
				value = null;
			}
			MyUIOptions myUIOption = value;
			if (myUIOption != null)
			{
				MyUIOptions myUIOption1 = myUIOption.u003cCloneu003eu0024();
				Dictionary<string, object> name = new Dictionary<string, object>();
				name["culture"] = CultureInfo.CurrentUICulture.Name;
				myUIOption1.More = name;
				MyUIOptions apiUrl = myUIOption1;
				JsonSerializerOptions requiredService = ServiceProviderServiceExtensions.GetRequiredService<JsonSerializerOptions>(httpContext.get_RequestServices());
				using (JsonDocument document = JsonSerializer.SerializeToDocument<MyUIOptions>(myUIOption, requiredService))
				{
					ExposedValues exposedValue = ServiceProviderServiceExtensions.GetService<ExposedValues>(httpContext.get_RequestServices());
					if (exposedValue != null)
					{
						apiUrl.More["info"] = exposedValue.ToString();
					}
					IOptions<NotifoOptions> option = ServiceProviderServiceExtensions.GetService<IOptions<NotifoOptions>>(httpContext.get_RequestServices());
					if (option != null)
					{
						notifoOption = option.get_Value();
					}
					else
					{
						notifoOption = null;
					}
					NotifoOptions notifoOption1 = notifoOption;
					if (notifoOption1 != null && notifoOption1.IsConfigured())
					{
						apiUrl.More["notifoApi"] = notifoOption1.get_ApiUrl();
					}
					OptionsFeature optionsFeature = httpContext.get_Features().Get<OptionsFeature>();
					if (optionsFeature != null)
					{
						foreach (KeyValuePair<string, object> keyValuePair in optionsFeature.Options)
						{
							keyValuePair.Deconstruct(out str, out obj);
							apiUrl.More[str] = obj;
						}
					}
					strs.Add(string.Concat("var options = ", JsonSerializer.Serialize<MyUIOptions>(apiUrl, (JsonSerializerOptions)null), ";"));
				}
			}
			html = html.Replace("/* INJECT OPTIONS */", string.Join(Environment.NewLine, strs), StringComparison.OrdinalIgnoreCase);
			return html;
		}

		private static string GetText(string culture)
		{
			string end;
			if (!IndexExtensions.Texts.TryGetValue(culture, out end))
			{
				Assembly assembly = typeof(IndexExtensions).Assembly;
				string str = string.Concat("Squidex.Areas.Frontend.Resources.frontend_", culture, ".json");
				Stream manifestResourceStream = assembly.GetManifestResourceStream(str);
				if (manifestResourceStream == null)
				{
					return IndexExtensions.GetText("en");
				}
				using (StreamReader streamReader = new StreamReader(manifestResourceStream))
				{
					end = streamReader.ReadToEnd();
					IndexExtensions.Texts[culture] = end;
				}
			}
			return end;
		}
	}
}