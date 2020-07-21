using Mix.Cms.Lib;
using Mix.Cms.Lib.ViewModels;
using Newtonsoft.Json.Linq;
using System;

namespace Mix.Cms.Lib.ViewModels.MixModuleDatas
{
	public class Helper
	{
		public Helper()
		{
			base();
			return;
		}

		public static JToken ParseValue(JObject JItem, ApiModuleDataValueViewModel item)
		{
			V_0 = null;
			if (JItem.TryGetValue(item.get_Name(), ref V_1))
			{
				switch (item.get_DataType())
				{
					case 0:
					case 1:
					case 2:
					case 3:
					case 4:
					case 5:
					case 6:
					case 7:
					case 8:
					case 9:
					case 10:
					case 11:
					case 12:
					case 13:
					case 14:
					case 15:
					case 17:
					case 18:
					case 19:
					case 20:
					case 21:
					case 22:
					{
					Label0:
						V_0 = V_1;
						break;
					}
					case 16:
					{
						V_2 = V_1.get_Item("value").ToString().TrimStart('/');
						if (V_2.IndexOf("http") >= 0)
						{
							stackVariable21 = V_2;
						}
						else
						{
							stackVariable21 = string.Concat(MixService.GetConfig<string>("Domain"), "/", V_2);
						}
						V_2 = stackVariable21;
						V_1.set_Item("value", JToken.op_Implicit(V_2));
						goto Label0;
					}
					case 23:
					{
						V_1.set_Item("value", new JArray());
						goto Label0;
					}
					default:
					{
						goto Label0;
					}
				}
			}
			return V_0;
		}
	}
}