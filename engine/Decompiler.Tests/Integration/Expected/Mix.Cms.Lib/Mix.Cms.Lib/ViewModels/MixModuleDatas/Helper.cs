using Mix.Cms.Lib;
using Mix.Cms.Lib.Services;
using Mix.Cms.Lib.ViewModels;
using Newtonsoft.Json.Linq;
using System;

namespace Mix.Cms.Lib.ViewModels.MixModuleDatas
{
	public class Helper
	{
		public Helper()
		{
		}

		public static JToken ParseValue(JObject JItem, ApiModuleDataValueViewModel item)
		{
			JToken jToken = null;
			JToken jToken1 = null;
			if (JItem.TryGetValue(item.Name, ref jToken))
			{
				switch (item.DataType)
				{
					case MixEnums.MixDataType.Custom:
					case MixEnums.MixDataType.DateTime:
					case MixEnums.MixDataType.Date:
					case MixEnums.MixDataType.Time:
					case MixEnums.MixDataType.Duration:
					case MixEnums.MixDataType.PhoneNumber:
					case MixEnums.MixDataType.Double:
					case MixEnums.MixDataType.Text:
					case MixEnums.MixDataType.Html:
					case MixEnums.MixDataType.MultilineText:
					case MixEnums.MixDataType.EmailAddress:
					case MixEnums.MixDataType.Password:
					case MixEnums.MixDataType.Url:
					case MixEnums.MixDataType.ImageUrl:
					case MixEnums.MixDataType.CreditCard:
					case MixEnums.MixDataType.PostalCode:
					case MixEnums.MixDataType.Color:
					case MixEnums.MixDataType.Boolean:
					case MixEnums.MixDataType.Icon:
					case MixEnums.MixDataType.VideoYoutube:
					case MixEnums.MixDataType.TuiEditor:
					case MixEnums.MixDataType.Integer:
					{
						jToken1 = jToken;
						break;
					}
					case MixEnums.MixDataType.Upload:
					{
						string str = jToken.get_Item("value").ToString().TrimStart('/');
						str = (str.IndexOf("http") >= 0 ? str : string.Concat(MixService.GetConfig<string>("Domain"), "/", str));
						jToken.set_Item("value", str);
						goto case MixEnums.MixDataType.Integer;
					}
					case MixEnums.MixDataType.Reference:
					{
						jToken.set_Item("value", new JArray());
						goto case MixEnums.MixDataType.Integer;
					}
					default:
					{
						goto case MixEnums.MixDataType.Integer;
					}
				}
			}
			return jToken1;
		}
	}
}