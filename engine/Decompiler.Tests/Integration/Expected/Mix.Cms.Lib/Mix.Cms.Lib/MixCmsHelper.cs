using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.Repositories;
using Mix.Cms.Lib.Services;
using Mix.Cms.Lib.ViewModels;
using Mix.Cms.Lib.ViewModels.MixAttributeSetDatas;
using Mix.Cms.Lib.ViewModels.MixModulePosts;
using Mix.Cms.Lib.ViewModels.MixModules;
using Mix.Cms.Lib.ViewModels.MixPages;
using Mix.Cms.Lib.ViewModels.MixPosts;
using Mix.Cms.Lib.ViewModels.MixTemplates;
using Mix.Common.Helper;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Mix.Heart.Enums;
using Mix.Heart.Helpers;
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
		}

		public static bool CheckIsPrice(string number)
		{
			double num;
			if (number == null)
			{
				return false;
			}
			number = number.Replace(",", "");
			return double.TryParse(number, out num);
		}

		public static Expression<Func<TModel, bool>> FilterObjectSet<TModel, T>(string propName, T data2, MixEnums.CompareType filterType)
		{
			Type fieldType;
			Expression expression;
			Type type = typeof(TModel);
			ParameterExpression parameterExpression = Expression.Parameter(type, "model");
			FieldInfo field = type.GetField(propName);
			if (field != null)
			{
				fieldType = field.FieldType;
				expression = Expression.Field(parameterExpression, field);
			}
			else
			{
				PropertyInfo property = type.GetProperty(propName);
				if (property == null)
				{
					throw new Exception();
				}
				fieldType = property.PropertyType;
				expression = Expression.Property(parameterExpression, property);
			}
			BinaryExpression binaryExpression = null;
			switch (filterType)
			{
				case MixEnums.CompareType.Eq:
				{
					binaryExpression = Expression.Equal(expression, Expression.Constant(data2, fieldType));
					break;
				}
				case MixEnums.CompareType.Lt:
				{
					binaryExpression = Expression.LessThan(expression, Expression.Constant(data2, fieldType));
					break;
				}
				case MixEnums.CompareType.Gt:
				{
					binaryExpression = Expression.GreaterThan(expression, Expression.Constant(data2, fieldType));
					break;
				}
				case MixEnums.CompareType.Lte:
				{
					binaryExpression = Expression.LessThanOrEqual(expression, Expression.Constant(data2, fieldType));
					break;
				}
				case MixEnums.CompareType.Gte:
				{
					binaryExpression = Expression.GreaterThanOrEqual(expression, Expression.Constant(data2, fieldType));
					break;
				}
				case MixEnums.CompareType.In:
				{
					MethodInfo method = typeof(string).GetMethod("Contains");
					return Expression.Lambda<Func<TModel, bool>>(Expression.Call(parameterExpression, method, new Expression[] { Expression.Constant(data2, typeof(string)) }), new ParameterExpression[] { parameterExpression });
				}
			}
			return Expression.Lambda<Func<TModel, bool>>(binaryExpression, new ParameterExpression[] { parameterExpression });
		}

		public static string FormatPrice(double? price, string oldPrice = "0")
		{
			string str;
			if (price.HasValue)
			{
				str = price.GetValueOrDefault().ToString();
			}
			else
			{
				str = null;
			}
			string str1 = str;
			if (string.IsNullOrEmpty(str1))
			{
				return "0";
			}
			string str2 = str1.Replace(",", string.Empty);
			if (!MixCmsHelper.CheckIsPrice(str2))
			{
				return oldPrice;
			}
			Regex regex = new Regex("(\\d+)(\\d{3})");
			while (regex.IsMatch(str2))
			{
				str2 = regex.Replace(str2, "$1,$2");
			}
			return str2;
		}

		public static string GetAssetFolder(string culture)
		{
			return string.Concat(MixService.GetConfig<string>("Domain"), "/content/templates/", MixService.GetConfig<string>("ThemeFolder", culture), "/assets");
		}

		public static List<Mix.Cms.Lib.ViewModels.MixPages.ReadListItemViewModel> GetCategory(IUrlHelper Url, string culture, MixEnums.MixPageType cateType, string activePath = "")
		{
			MixCmsHelper.u003cu003ec__DisplayClass3_0 variable = null;
			bool flag;
			DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixPage, Mix.Cms.Lib.ViewModels.MixPages.ReadListItemViewModel>.Repository;
			ParameterExpression parameterExpression = Expression.Parameter(typeof(MixPage), "c");
			List<Mix.Cms.Lib.ViewModels.MixPages.ReadListItemViewModel> data = repository.GetModelListBy(Expression.Lambda<Func<MixPage, bool>>(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPage).GetMethod("get_Specificulture").MethodHandle)), Expression.Field(Expression.Constant(variable, typeof(MixCmsHelper.u003cu003ec__DisplayClass3_0)), FieldInfo.GetFieldFromHandle(typeof(MixCmsHelper.u003cu003ec__DisplayClass3_0).GetField("culture").FieldHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPage).GetMethod("get_Type").MethodHandle)), Expression.Call(Expression.Field(Expression.Constant(variable, typeof(MixCmsHelper.u003cu003ec__DisplayClass3_0)), FieldInfo.GetFieldFromHandle(typeof(MixCmsHelper.u003cu003ec__DisplayClass3_0).GetField("cateType").FieldHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(object).GetMethod("ToString").MethodHandle), Array.Empty<Expression>()))), new ParameterExpression[] { parameterExpression }), null, null).get_Data() ?? new List<Mix.Cms.Lib.ViewModels.MixPages.ReadListItemViewModel>();
			activePath = activePath.ToLower();
			foreach (Mix.Cms.Lib.ViewModels.MixPages.ReadListItemViewModel datum in data)
			{
				MixEnums.MixPageType type = datum.Type;
				datum.DetailsUrl = UrlHelperExtensions.RouteUrl(Url, "Alias", new { culture = culture, seoName = datum.SeoName });
				Mix.Cms.Lib.ViewModels.MixPages.ReadListItemViewModel readListItemViewModel = datum;
				if (datum.DetailsUrl == activePath)
				{
					flag = true;
				}
				else
				{
					flag = (datum.Type != MixEnums.MixPageType.Home ? false : activePath == string.Format("/{0}/home", culture));
				}
				readListItemViewModel.IsActived = flag;
			}
			return data;
		}

		public static async Task<RepositoryResponse<PaginationModel<TView>>> GetListPostByAddictionalField<TView>(string fieldName, object fieldValue, string culture, MixEnums.MixDataType dataType, MixEnums.CompareType filterType = 1, string orderByPropertyName = null, MixHeartEnums.DisplayDirection direction = 0, int? pageSize = null, int? pageIndex = null, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		where TView : ViewModelBase<MixCmsContext, MixPost, TView>
		{
			MixCmsHelper.u003cGetListPostByAddictionalFieldu003ed__15<TView> variable = new MixCmsHelper.u003cGetListPostByAddictionalFieldu003ed__15<TView>();
			variable.fieldName = fieldName;
			variable.fieldValue = fieldValue;
			variable.culture = culture;
			variable.dataType = dataType;
			variable.filterType = filterType;
			variable.orderByPropertyName = orderByPropertyName;
			variable.direction = direction;
			variable.pageSize = pageSize;
			variable.pageIndex = pageIndex;
			variable._context = _context;
			variable._transaction = _transaction;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<PaginationModel<TView>>>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<MixCmsHelper.u003cGetListPostByAddictionalFieldu003ed__15<TView>>(ref variable);
			return variable.u003cu003et__builder.Task;
		}

		public static Mix.Cms.Lib.ViewModels.MixModules.ReadMvcViewModel GetModule(string name, string culture)
		{
			int? nullable = null;
			int? nullable1 = nullable;
			nullable = null;
			return Mix.Cms.Lib.ViewModels.MixModules.ReadMvcViewModel.GetBy((MixModule m) => m.Name == name && m.Specificulture == culture, nullable1, nullable, 0, null, null).get_Data();
		}

		public static Task<Mix.Cms.Lib.ViewModels.MixModules.ReadMvcViewModel> GetModuleAsync(string name, string culture, IUrlHelper url = null)
		{
			string.Concat(new string[] { "vm_", culture, "_module_", name, "_mvc" });
			RepositoryResponse<Mix.Cms.Lib.ViewModels.MixModules.ReadMvcViewModel> repositoryResponse = new RepositoryResponse<Mix.Cms.Lib.ViewModels.MixModules.ReadMvcViewModel>();
			if (repositoryResponse == null || !repositoryResponse.get_IsSucceed())
			{
				int? nullable = null;
				int? nullable1 = nullable;
				nullable = null;
				repositoryResponse = Mix.Cms.Lib.ViewModels.MixModules.ReadMvcViewModel.GetBy((MixModule m) => m.Name == name && m.Specificulture == culture, nullable1, nullable, 0, null, null);
			}
			if (repositoryResponse.get_IsSucceed() && url != null && repositoryResponse.get_Data().Posts != null)
			{
				repositoryResponse.get_Data().Posts.get_Items().ForEach((Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel a) => a.Post.DetailsUrl = UrlHelperExtensions.RouteUrl(url, "Post", new { id = a.PostId, seoName = a.Post.SeoName }));
			}
			return Task.FromResult<Mix.Cms.Lib.ViewModels.MixModules.ReadMvcViewModel>(repositoryResponse.get_Data());
		}

		public static async Task<Navigation> GetNavigation(string name, string culture, IUrlHelper Url)
		{
			Navigation nav;
			bool flag;
			NavigationViewModel navigationViewModel = await Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.FilterByKeywordAsync<NavigationViewModel>(culture, "sys_navigation", "equal", "name", name, null, null).get_Data().FirstOrDefault<NavigationViewModel>();
			if (navigationViewModel != null)
			{
				nav = navigationViewModel.Nav;
			}
			else
			{
				nav = null;
			}
			Navigation navigation = nav;
			string path = Url.get_ActionContext().get_HttpContext().get_Request().get_Path();
			if (navigation != null && !string.IsNullOrEmpty(path))
			{
				foreach (MenuItem menuItem in navigation.MenuItems)
				{
					menuItem.IsActive = menuItem.Uri == path;
					foreach (MenuItem uri in menuItem.MenuItems)
					{
						uri.IsActive = uri.Uri == path;
						MenuItem menuItem1 = menuItem;
						flag = (menuItem.IsActive ? true : uri.IsActive);
						menuItem1.IsActive = flag;
					}
				}
			}
			return navigation;
		}

		public static Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel GetPage(int id, string culture)
		{
			return ViewModelBase<MixCmsContext, MixPage, Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel>.Repository.GetSingleModel((MixPage m) => m.Id == id && m.Specificulture == culture, null, null).get_Data();
		}

		public static async Task<Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel> GetPageAsync(int id, string culture)
		{
			RepositoryResponse<Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel> singleModelAsync = null;
			if (singleModelAsync == null)
			{
				DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixPage, Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel>.Repository;
				singleModelAsync = await repository.GetSingleModelAsync((MixPage m) => m.Id == id && m.Specificulture == culture, null, null);
			}
			return singleModelAsync.get_Data();
		}

		public static string GetRouterUrl(object routeValues, HttpRequest request, IUrlHelper Url)
		{
			string str = "";
			PropertyInfo[] properties = routeValues.GetType().GetProperties();
			for (int i = 0; i < (int)properties.Length; i++)
			{
				PropertyInfo propertyInfo = properties[i];
				string name = propertyInfo.Name;
				string str1 = propertyInfo.GetValue(routeValues, null).ToString();
				str = string.Concat(str, "/", str1);
			}
			return string.Format("{0}://{1}{2}", request.get_Scheme(), request.get_Host(), str);
		}

		public static async Task<Mix.Cms.Lib.ViewModels.MixTemplates.ReadViewModel> GetTemplateByPath(string themeName, string templatePath)
		{
			MixCmsHelper.u003cu003ec__DisplayClass13_0 variable = null;
			string[] strArrays = templatePath.Split('/', StringSplitOptions.None);
			if (strArrays[1].IndexOf('.') > 0)
			{
				strArrays[1] = strArrays[1].Substring(0, strArrays[1].IndexOf('.'));
			}
			DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixTemplate, Mix.Cms.Lib.ViewModels.MixTemplates.ReadViewModel>.Repository;
			ParameterExpression parameterExpression = Expression.Parameter(typeof(MixTemplate), "m");
			BinaryExpression binaryExpression = Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixTemplate).GetMethod("get_ThemeName").MethodHandle)), Expression.Field(Expression.Constant(variable, typeof(MixCmsHelper.u003cu003ec__DisplayClass13_0)), FieldInfo.GetFieldFromHandle(typeof(MixCmsHelper.u003cu003ec__DisplayClass13_0).GetField("themeName").FieldHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixTemplate).GetMethod("get_FolderType").MethodHandle)), Expression.ArrayIndex(Expression.Field(Expression.Constant(variable, typeof(MixCmsHelper.u003cu003ec__DisplayClass13_0)), FieldInfo.GetFieldFromHandle(typeof(MixCmsHelper.u003cu003ec__DisplayClass13_0).GetField("tmp").FieldHandle)), Expression.Constant(0, typeof(int))))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixTemplate).GetMethod("get_FileName").MethodHandle)), Expression.ArrayIndex(Expression.Field(Expression.Constant(variable, typeof(MixCmsHelper.u003cu003ec__DisplayClass13_0)), FieldInfo.GetFieldFromHandle(typeof(MixCmsHelper.u003cu003ec__DisplayClass13_0).GetField("tmp").FieldHandle)), Expression.Constant(1, typeof(int)))));
			ParameterExpression[] parameterExpressionArray = new ParameterExpression[] { parameterExpression };
			Mix.Cms.Lib.ViewModels.MixTemplates.ReadViewModel data = await repository.GetFirstModelAsync(Expression.Lambda<Func<MixTemplate, bool>>(binaryExpression, parameterExpressionArray), null, null).get_Data();
			return data;
		}

		public static string GetTemplateFolder(string culture)
		{
			return string.Concat("/Views/Shared/Templates/", MixService.GetConfig<string>("ThemeFolder", culture));
		}

		private static Expression<Func<MixAttributeSetValue, bool>> GetValuePredicate(string fieldValue, MixEnums.CompareType filterType, MixEnums.MixDataType dataType)
		{
			DateTime dateTime;
			double num;
			bool flag;
			int num1;
			Expression<Func<MixAttributeSetValue, bool>> expression = null;
			switch (dataType)
			{
				case MixEnums.MixDataType.Custom:
				case MixEnums.MixDataType.DateTime:
				case MixEnums.MixDataType.Duration:
				case MixEnums.MixDataType.PhoneNumber:
				case MixEnums.MixDataType.Text:
				case MixEnums.MixDataType.Html:
				case MixEnums.MixDataType.MultilineText:
				case MixEnums.MixDataType.EmailAddress:
				case MixEnums.MixDataType.Password:
				case MixEnums.MixDataType.Url:
				case MixEnums.MixDataType.ImageUrl:
				case MixEnums.MixDataType.CreditCard:
				case MixEnums.MixDataType.PostalCode:
				case MixEnums.MixDataType.Upload:
				case MixEnums.MixDataType.Color:
				case MixEnums.MixDataType.Icon:
				case MixEnums.MixDataType.VideoYoutube:
				case MixEnums.MixDataType.TuiEditor:
				case MixEnums.MixDataType.QRCode:
				{
					expression = MixCmsHelper.FilterObjectSet<MixAttributeSetValue, string>("StringValue", fieldValue, filterType);
					return expression;
				}
				case MixEnums.MixDataType.Date:
				case MixEnums.MixDataType.Time:
				{
					if (!DateTime.TryParse(fieldValue, out dateTime))
					{
						return expression;
					}
					expression = MixCmsHelper.FilterObjectSet<MixAttributeSetValue, DateTime>("DateTimeValue", dateTime, filterType);
					return expression;
				}
				case MixEnums.MixDataType.Double:
				{
					if (!double.TryParse(fieldValue, out num))
					{
						return expression;
					}
					expression = MixCmsHelper.FilterObjectSet<MixAttributeSetValue, double>("DoubleValue", num, filterType);
					return expression;
				}
				case MixEnums.MixDataType.Boolean:
				{
					if (!bool.TryParse(fieldValue, out flag))
					{
						return expression;
					}
					expression = MixCmsHelper.FilterObjectSet<MixAttributeSetValue, bool>("BooleanValue", flag, filterType);
					return expression;
				}
				case MixEnums.MixDataType.Integer:
				{
					if (!int.TryParse(fieldValue, out num1))
					{
						return expression;
					}
					expression = MixCmsHelper.FilterObjectSet<MixAttributeSetValue, int>("IntegerValue", num1, filterType);
					return expression;
				}
				case MixEnums.MixDataType.Reference:
				{
					return expression;
				}
				default:
				{
					expression = MixCmsHelper.FilterObjectSet<MixAttributeSetValue, string>("StringValue", fieldValue, filterType);
					return expression;
				}
			}
		}

		public static FileViewModel LoadDataFile(string folder, string name)
		{
			return FileRepository.Instance.GetFile(name, folder, true, "[]");
		}

		public static void LogException(Exception ex)
		{
			string str = string.Format(string.Concat(Environment.CurrentDirectory, "/logs"), Array.Empty<object>());
			if (!string.IsNullOrEmpty(str) && !Directory.Exists(str))
			{
				Directory.CreateDirectory(str);
			}
			DateTime now = DateTime.Now;
			string str1 = string.Concat(str, "/", now.ToString("YYYYMMDD"), "/log_exceptions.json");
			try
			{
				FileInfo fileInfo = new FileInfo(str1);
				string end = "[]";
				if (fileInfo.Exists)
				{
					using (StreamReader streamReader = fileInfo.OpenText())
					{
						end = streamReader.ReadToEnd();
					}
					File.Delete(str1);
				}
				JArray jArray = JArray.Parse(end);
				JObject jObject = new JObject();
				jObject.Add(new JProperty("CreatedDateTime", (object)DateTime.UtcNow));
				jObject.Add(new JProperty("Details", JObject.FromObject(ex)));
				jArray.Add(jObject);
				end = jArray.ToString();
				using (StreamWriter streamWriter = File.CreateText(str1))
				{
					streamWriter.WriteLine(end);
				}
			}
			catch
			{
			}
		}

		public static double ReversePrice(string formatedPrice)
		{
			double num;
			try
			{
				num = (!string.IsNullOrEmpty(formatedPrice) ? double.Parse(formatedPrice.Replace(",", string.Empty)) : 0);
			}
			catch
			{
				num = 0;
			}
			return num;
		}
	}
}