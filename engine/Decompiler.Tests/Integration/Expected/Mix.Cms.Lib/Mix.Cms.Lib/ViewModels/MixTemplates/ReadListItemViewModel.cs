using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.Services;
using Mix.Common.Helper;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels.MixTemplates
{
	public class ReadListItemViewModel : ViewModelBase<MixCmsContext, MixTemplate, ReadListItemViewModel>
	{
		[JsonProperty("assetFolder")]
		public string AssetFolder
		{
			get
			{
				return CommonHelper.GetFullPath(new string[] { "content", "templates", this.ThemeName });
			}
		}

		[JsonProperty("content")]
		public string Content
		{
			get;
			set;
		}

		[JsonProperty("createdBy")]
		public string CreatedBy
		{
			get;
			set;
		}

		[JsonProperty("createdDateTime")]
		public DateTime CreatedDateTime
		{
			get;
			set;
		}

		[JsonProperty("extension")]
		public string Extension
		{
			get;
			set;
		}

		[JsonProperty("fileFolder")]
		public string FileFolder
		{
			get;
			set;
		}

		[JsonProperty("fileName")]
		public string FileName
		{
			get;
			set;
		}

		[JsonProperty("folderType")]
		public string FolderType
		{
			get;
			set;
		}

		[JsonProperty("id")]
		public int Id
		{
			get;
			set;
		}

		[JsonProperty("lastModified")]
		public DateTime? LastModified
		{
			get;
			set;
		}

		[JsonProperty("mobileContent")]
		public string MobileContent { get; set; } = "{}";

		[JsonProperty("modifiedBy")]
		public string ModifiedBy
		{
			get;
			set;
		}

		[JsonProperty("priority")]
		public int Priority
		{
			get;
			set;
		}

		[JsonProperty("scripts")]
		public string Scripts
		{
			get;
			set;
		}

		[JsonProperty("spaContent")]
		public string SpaContent { get; set; } = "";

		[JsonProperty("status")]
		public MixEnums.MixContentStatus Status
		{
			get;
			set;
		}

		[JsonProperty("styles")]
		public string Styles
		{
			get;
			set;
		}

		[JsonProperty("templateFolder")]
		public string TemplateFolder
		{
			get
			{
				return CommonHelper.GetFullPath(new string[] { "Views/Shared/Templates", this.ThemeName });
			}
		}

		[JsonProperty("templatePath")]
		public string TemplatePath
		{
			get
			{
				return string.Concat(new string[] { "/", this.FileFolder, "/", this.FileName, this.Extension });
			}
		}

		[JsonProperty("themeId")]
		public int ThemeId
		{
			get;
			set;
		}

		[JsonProperty("themeName")]
		public string ThemeName
		{
			get;
			set;
		}

		public ReadListItemViewModel()
		{
		}

		public ReadListItemViewModel(MixTemplate model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public static ReadListItemViewModel GetDefault(string activedTemplate, string folderType, string folder, string specificulture)
		{
			return new ReadListItemViewModel(new MixTemplate()
			{
				Extension = MixService.GetConfig<string>("TemplateExtension"),
				ThemeId = MixService.GetConfig<int>("ThemeId", specificulture),
				ThemeName = activedTemplate,
				FolderType = folderType,
				FileFolder = folder,
				FileName = MixService.GetConfig<string>("DefaultTemplate"),
				Content = "<div></div>"
			}, null, null);
		}

		public static RepositoryResponse<ReadListItemViewModel> GetTemplateByPath(string path, string culture, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			ReadListItemViewModel.u003cu003ec__DisplayClass80_0 variable = null;
			RepositoryResponse<ReadListItemViewModel> repositoryResponse = new RepositoryResponse<ReadListItemViewModel>();
			string[] strArrays = path.Split('/', StringSplitOptions.None);
			if ((int)strArrays.Length >= 2)
			{
				MixService.GetConfig<int>("ThemeId", culture);
				string str = strArrays[1].Split('.', StringSplitOptions.None)[0];
				DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixTemplate, ReadListItemViewModel>.Repository;
				ParameterExpression parameterExpression = Expression.Parameter(typeof(MixTemplate), "t");
				repositoryResponse = repository.GetSingleModel(Expression.Lambda<Func<MixTemplate, bool>>(Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixTemplate).GetMethod("get_FolderType").MethodHandle)), Expression.ArrayIndex(Expression.Field(Expression.Constant(variable, typeof(ReadListItemViewModel.u003cu003ec__DisplayClass80_0)), FieldInfo.GetFieldFromHandle(typeof(ReadListItemViewModel.u003cu003ec__DisplayClass80_0).GetField("temp").FieldHandle)), Expression.Constant(0, typeof(int)))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixTemplate).GetMethod("get_FileName").MethodHandle)), Expression.Field(Expression.Constant(variable, typeof(ReadListItemViewModel.u003cu003ec__DisplayClass80_0)), FieldInfo.GetFieldFromHandle(typeof(ReadListItemViewModel.u003cu003ec__DisplayClass80_0).GetField("name").FieldHandle)))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixTemplate).GetMethod("get_ThemeId").MethodHandle)), Expression.Field(Expression.Constant(variable, typeof(ReadListItemViewModel.u003cu003ec__DisplayClass80_0)), FieldInfo.GetFieldFromHandle(typeof(ReadListItemViewModel.u003cu003ec__DisplayClass80_0).GetField("activeThemeId").FieldHandle)))), new ParameterExpression[] { parameterExpression }), _context, _transaction);
			}
			else
			{
				repositoryResponse.set_IsSucceed(false);
				repositoryResponse.get_Errors().Add("Template Not Found");
			}
			return repositoryResponse;
		}

		public static ReadListItemViewModel GetTemplateByPath(int themeId, string path, string type, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			string str;
			if (path != null)
			{
				str = path.Split('/', StringSplitOptions.None)[1];
			}
			else
			{
				str = null;
			}
			string str1 = str;
			return ViewModelBase<MixCmsContext, MixTemplate, ReadListItemViewModel>.Repository.GetSingleModel((MixTemplate t) => t.ThemeId == themeId && t.FolderType == type && !string.IsNullOrEmpty(str1) && str1.Equals(string.Format("{0}{1}", t.FileName, t.Extension)), _context, _transaction).get_Data();
		}
	}
}