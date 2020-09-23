using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels;
using Mix.Cms.Lib.ViewModels.MixAttributeSetDatas;
using Mix.Cms.Lib.ViewModels.MixModulePosts;
using Mix.Cms.Lib.ViewModels.MixModules;
using Mix.Cms.Lib.ViewModels.MixPages;
using Mix.Cms.Lib.ViewModels.MixTemplates;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.ViewModels;
using Mix.Heart.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mix.Cms.Lib
{
	public class MixCmsHelper
	{
		public MixCmsHelper()
		{
			base();
			return;
		}

		public static bool CheckIsPrice(string number)
		{
			if (number == null)
			{
				return false;
			}
			number = number.Replace(",", "");
			return double.TryParse(number, out V_0);
		}

		public static Expression<Func<TModel, bool>> FilterObjectSet<TModel, T>(string propName, T data2, MixEnums.CompareType filterType)
		{
			V_0 = Type.GetTypeFromHandle(// 
			// Current member / type: System.Linq.Expressions.Expression`1<System.Func`2<TModel,System.Boolean>> Mix.Cms.Lib.MixCmsHelper::FilterObjectSet(System.String,T,Mix.Cms.Lib.MixEnums/CompareType)
			// Exception in: System.Linq.Expressions.Expression<System.Func<TModel,System.Boolean>> FilterObjectSet(System.String,T,Mix.Cms.Lib.MixEnums/CompareType)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public static string FormatPrice(double? price, string oldPrice = "0")
		{
			if (price.get_HasValue())
			{
				V_2 = price.GetValueOrDefault();
				stackVariable5 = V_2.ToString();
			}
			else
			{
				stackVariable5 = null;
			}
			V_0 = stackVariable5;
			if (string.IsNullOrEmpty(V_0))
			{
				return "0";
			}
			V_1 = V_0.Replace(",", string.Empty);
			if (!MixCmsHelper.CheckIsPrice(V_1))
			{
				return oldPrice;
			}
			V_3 = new Regex("(\\d+)(\\d{3})");
			while (V_3.IsMatch(V_1))
			{
				V_1 = V_3.Replace(V_1, "$1,$2");
			}
			return V_1;
		}

		public static string GetAssetFolder(string culture)
		{
			return string.Concat(MixService.GetConfig<string>("Domain"), "/content/templates/", MixService.GetConfig<string>("ThemeFolder", culture), "/assets");
		}

		public static List<Mix.Cms.Lib.ViewModels.MixPages.ReadListItemViewModel> GetCategory(IUrlHelper Url, string culture, MixEnums.MixPageType cateType, string activePath = "")
		{
			V_0 = new MixCmsHelper.u003cu003ec__DisplayClass3_0();
			V_0.culture = culture;
			V_0.cateType = cateType;
			stackVariable5 = ViewModelBase<MixCmsContext, MixPage, Mix.Cms.Lib.ViewModels.MixPages.ReadListItemViewModel>.Repository;
			V_2 = Expression.Parameter(Type.GetTypeFromHandle(// 
			// Current member / type: System.Collections.Generic.List`1<Mix.Cms.Lib.ViewModels.MixPages.ReadListItemViewModel> Mix.Cms.Lib.MixCmsHelper::GetCategory(Microsoft.AspNetCore.Mvc.IUrlHelper,System.String,Mix.Cms.Lib.MixEnums/MixPageType,System.String)
			// Exception in: System.Collections.Generic.List<Mix.Cms.Lib.ViewModels.MixPages.ReadListItemViewModel> GetCategory(Microsoft.AspNetCore.Mvc.IUrlHelper,System.String,Mix.Cms.Lib.MixEnums/MixPageType,System.String)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public static async Task<RepositoryResponse<PaginationModel<TView>>> GetListPostByAddictionalField<TView>(string fieldName, object fieldValue, string culture, MixEnums.MixDataType dataType, MixEnums.CompareType filterType = 1, string orderByPropertyName = null, MixHeartEnums.DisplayDirection direction = 0, int? pageSize = null, int? pageIndex = null, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		where TView : ViewModelBase<MixCmsContext, MixPost, TView>
		{
			V_0.fieldName = fieldName;
			V_0.fieldValue = fieldValue;
			V_0.culture = culture;
			V_0.dataType = dataType;
			V_0.filterType = filterType;
			V_0.orderByPropertyName = orderByPropertyName;
			V_0.direction = direction;
			V_0.pageSize = pageSize;
			V_0.pageIndex = pageIndex;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<PaginationModel<TView>>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<MixCmsHelper.u003cGetListPostByAddictionalFieldu003ed__15<TView>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public static Mix.Cms.Lib.ViewModels.MixModules.ReadMvcViewModel GetModule(string name, string culture)
		{
			V_0 = new MixCmsHelper.u003cu003ec__DisplayClass11_0();
			V_0.name = name;
			V_0.culture = culture;
			V_1 = Expression.Parameter(Type.GetTypeFromHandle(// 
			// Current member / type: Mix.Cms.Lib.ViewModels.MixModules.ReadMvcViewModel Mix.Cms.Lib.MixCmsHelper::GetModule(System.String,System.String)
			// Exception in: Mix.Cms.Lib.ViewModels.MixModules.ReadMvcViewModel GetModule(System.String,System.String)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public static Task<Mix.Cms.Lib.ViewModels.MixModules.ReadMvcViewModel> GetModuleAsync(string name, string culture, IUrlHelper url = null)
		{
			V_0 = new MixCmsHelper.u003cu003ec__DisplayClass9_0();
			V_0.name = name;
			V_0.culture = culture;
			V_0.url = url;
			stackVariable8 = new string[5];
			stackVariable8[0] = "vm_";
			stackVariable8[1] = V_0.culture;
			stackVariable8[2] = "_module_";
			stackVariable8[3] = V_0.name;
			stackVariable8[4] = "_mvc";
			dummyVar0 = string.Concat(stackVariable8);
			V_1 = new RepositoryResponse<Mix.Cms.Lib.ViewModels.MixModules.ReadMvcViewModel>();
			if (V_1 == null || !V_1.get_IsSucceed())
			{
				V_2 = Expression.Parameter(Type.GetTypeFromHandle(// 
				// Current member / type: System.Threading.Tasks.Task`1<Mix.Cms.Lib.ViewModels.MixModules.ReadMvcViewModel> Mix.Cms.Lib.MixCmsHelper::GetModuleAsync(System.String,System.String,Microsoft.AspNetCore.Mvc.IUrlHelper)
				// Exception in: System.Threading.Tasks.Task<Mix.Cms.Lib.ViewModels.MixModules.ReadMvcViewModel> GetModuleAsync(System.String,System.String,Microsoft.AspNetCore.Mvc.IUrlHelper)
				// Specified method is not supported.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com


		public static async Task<Navigation> GetNavigation(string name, string culture, IUrlHelper Url)
		{
			V_0.name = name;
			V_0.culture = culture;
			V_0.Url = Url;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<Navigation>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<MixCmsHelper.u003cGetNavigationu003ed__14>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public static Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel GetPage(int id, string culture)
		{
			V_0 = new MixCmsHelper.u003cu003ec__DisplayClass12_0();
			V_0.id = id;
			V_0.culture = culture;
			stackVariable5 = ViewModelBase<MixCmsContext, MixPage, Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel>.Repository;
			V_1 = Expression.Parameter(Type.GetTypeFromHandle(// 
			// Current member / type: Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel Mix.Cms.Lib.MixCmsHelper::GetPage(System.Int32,System.String)
			// Exception in: Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel GetPage(System.Int32,System.String)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public static async Task<Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel> GetPageAsync(int id, string culture)
		{
			V_0.id = id;
			V_0.culture = culture;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<MixCmsHelper.u003cGetPageAsyncu003ed__10>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public static string GetRouterUrl(object routeValues, HttpRequest request, IUrlHelper Url)
		{
			V_0 = "";
			V_1 = routeValues.GetType().GetProperties();
			V_2 = 0;
			while (V_2 < (int)V_1.Length)
			{
				stackVariable11 = V_1[V_2];
				dummyVar0 = stackVariable11.get_Name();
				V_3 = stackVariable11.GetValue(routeValues, null).ToString();
				V_0 = string.Concat(V_0, "/", V_3);
				V_2 = V_2 + 1;
			}
			return string.Format("{0}://{1}{2}", request.get_Scheme(), request.get_Host(), V_0);
		}

		public static async Task<Mix.Cms.Lib.ViewModels.MixTemplates.ReadViewModel> GetTemplateByPath(string themeName, string templatePath)
		{
			V_0.themeName = themeName;
			V_0.templatePath = templatePath;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<Mix.Cms.Lib.ViewModels.MixTemplates.ReadViewModel>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<MixCmsHelper.u003cGetTemplateByPathu003ed__13>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public static string GetTemplateFolder(string culture)
		{
			return string.Concat("/Views/Shared/Templates/", MixService.GetConfig<string>("ThemeFolder", culture));
		}

		private static Expression<Func<MixAttributeSetValue, bool>> GetValuePredicate(string fieldValue, MixEnums.CompareType filterType, MixEnums.MixDataType dataType)
		{
			V_0 = null;
			switch (dataType)
			{
				case 0:
				case 1:
				case 4:
				case 5:
				case 7:
				case 8:
				case 9:
				case 10:
				case 11:
				case 12:
				case 13:
				case 14:
				case 15:
				case 16:
				case 17:
				case 19:
				case 20:
				case 21:
				case 24:
				{
				Label1:
					V_0 = MixCmsHelper.FilterObjectSet<MixAttributeSetValue, string>("StringValue", fieldValue, filterType);
					goto Label0;
				}
				case 2:
				case 3:
				{
					if (!DateTime.TryParse(fieldValue, out V_1))
					{
						goto Label0;
					}
					V_0 = MixCmsHelper.FilterObjectSet<MixAttributeSetValue, DateTime>("DateTimeValue", V_1, filterType);
					goto Label0;
				}
				case 6:
				{
					if (!double.TryParse(fieldValue, out V_2))
					{
						goto Label0;
					}
					V_0 = MixCmsHelper.FilterObjectSet<MixAttributeSetValue, double>("DoubleValue", V_2, filterType);
					goto Label0;
				}
				case 18:
				{
					if (!bool.TryParse(fieldValue, out V_3))
					{
						goto Label0;
					}
					V_0 = MixCmsHelper.FilterObjectSet<MixAttributeSetValue, bool>("BooleanValue", V_3, filterType);
					goto Label0;
				}
				case 22:
				{
					if (!int.TryParse(fieldValue, out V_4))
					{
						goto Label0;
					}
					V_0 = MixCmsHelper.FilterObjectSet<MixAttributeSetValue, int>("IntegerValue", V_4, filterType);
					goto Label0;
				}
				case 23:
				{
				Label0:
					return V_0;
				}
				default:
				{
					goto Label1;
				}
			}
		}

		public static FileViewModel LoadDataFile(string folder, string name)
		{
			return FileRepository.get_Instance().GetFile(name, folder, true, "[]");
		}

		public static void LogException(Exception ex)
		{
			V_0 = string.Format(string.Concat(Environment.get_CurrentDirectory(), "/logs"), Array.Empty<object>());
			if (!string.IsNullOrEmpty(V_0) && !Directory.Exists(V_0))
			{
				dummyVar0 = Directory.CreateDirectory(V_0);
			}
			V_2 = DateTime.get_Now();
			V_1 = string.Concat(V_0, "/", V_2.ToString("YYYYMMDD"), "/log_exceptions.json");
			try
			{
				V_3 = new FileInfo(V_1);
				V_4 = "[]";
				if (V_3.get_Exists())
				{
					V_6 = V_3.OpenText();
					try
					{
						V_4 = V_6.ReadToEnd();
					}
					finally
					{
						if (V_6 != null)
						{
							((IDisposable)V_6).Dispose();
						}
					}
					File.Delete(V_1);
				}
				stackVariable21 = JArray.Parse(V_4);
				stackVariable22 = new JObject();
				stackVariable22.Add(new JProperty("CreatedDateTime", (object)DateTime.get_UtcNow()));
				stackVariable22.Add(new JProperty("Details", JObject.FromObject(ex)));
				stackVariable21.Add(stackVariable22);
				V_4 = stackVariable21.ToString();
				V_7 = File.CreateText(V_1);
				try
				{
					V_7.WriteLine(V_4);
				}
				finally
				{
					if (V_7 != null)
					{
						((IDisposable)V_7).Dispose();
					}
				}
			}
			catch
			{
				dummyVar1 = exception_0;
			}
			return;
		}

		public static double ReversePrice(string formatedPrice)
		{
			try
			{
				if (!string.IsNullOrEmpty(formatedPrice))
				{
					V_0 = double.Parse(formatedPrice.Replace(",", string.Empty));
				}
				else
				{
					V_0 = 0;
				}
			}
			catch
			{
				dummyVar0 = exception_0;
				V_0 = 0;
			}
			return V_0;
		}
	}
}