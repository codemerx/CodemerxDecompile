using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Linq.Expressions;
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
				stackVariable1 = new string[3];
				stackVariable1[0] = "content";
				stackVariable1[1] = "templates";
				stackVariable1[2] = this.get_ThemeName();
				return CommonHelper.GetFullPath(stackVariable1);
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
		public string MobileContent
		{
			get;
			set;
		}

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
		public string SpaContent
		{
			get;
			set;
		}

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
				stackVariable1 = new string[2];
				stackVariable1[0] = "Views/Shared/Templates";
				stackVariable1[1] = this.get_ThemeName();
				return CommonHelper.GetFullPath(stackVariable1);
			}
		}

		[JsonProperty("templatePath")]
		public string TemplatePath
		{
			get
			{
				stackVariable1 = new string[5];
				stackVariable1[0] = "/";
				stackVariable1[1] = this.get_FileFolder();
				stackVariable1[2] = "/";
				stackVariable1[3] = this.get_FileName();
				stackVariable1[4] = this.get_Extension();
				return string.Concat(stackVariable1);
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
			this.u003cMobileContentu003ek__BackingField = "{}";
			this.u003cSpaContentu003ek__BackingField = "";
			base();
			return;
		}

		public ReadListItemViewModel(MixTemplate model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.u003cMobileContentu003ek__BackingField = "{}";
			this.u003cSpaContentu003ek__BackingField = "";
			base(model, _context, _transaction);
			return;
		}

		public static ReadListItemViewModel GetDefault(string activedTemplate, string folderType, string folder, string specificulture)
		{
			stackVariable0 = new MixTemplate();
			stackVariable0.set_Extension(MixService.GetConfig<string>("TemplateExtension"));
			stackVariable0.set_ThemeId(MixService.GetConfig<int>("ThemeId", specificulture));
			stackVariable0.set_ThemeName(activedTemplate);
			stackVariable0.set_FolderType(folderType);
			stackVariable0.set_FileFolder(folder);
			stackVariable0.set_FileName(MixService.GetConfig<string>("DefaultTemplate"));
			stackVariable0.set_Content("<div></div>");
			return new ReadListItemViewModel(stackVariable0, null, null);
		}

		public static RepositoryResponse<ReadListItemViewModel> GetTemplateByPath(string path, string culture, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0 = new ReadListItemViewModel.u003cu003ec__DisplayClass80_0();
			V_1 = new RepositoryResponse<ReadListItemViewModel>();
			V_0.temp = path.Split('/', 0);
			if ((int)V_0.temp.Length >= 2)
			{
				V_0.activeThemeId = MixService.GetConfig<int>("ThemeId", culture);
				V_0.name = V_0.temp[1].Split('.', 0)[0];
				stackVariable26 = ViewModelBase<MixCmsContext, MixTemplate, ReadListItemViewModel>.Repository;
				V_2 = Expression.Parameter(Type.GetTypeFromHandle(// 
				// Current member / type: Mix.Domain.Core.ViewModels.RepositoryResponse`1<Mix.Cms.Lib.ViewModels.MixTemplates.ReadListItemViewModel> Mix.Cms.Lib.ViewModels.MixTemplates.ReadListItemViewModel::GetTemplateByPath(System.String,System.String,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Exception in: Mix.Domain.Core.ViewModels.RepositoryResponse<Mix.Cms.Lib.ViewModels.MixTemplates.ReadListItemViewModel> GetTemplateByPath(System.String,System.String,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Specified method is not supported.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com


		public static ReadListItemViewModel GetTemplateByPath(int themeId, string path, string type, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0 = new ReadListItemViewModel.u003cu003ec__DisplayClass81_0();
			V_0.themeId = themeId;
			V_0.type = type;
			stackVariable5 = V_0;
			if (path != null)
			{
				stackVariable12 = path.Split('/', 0)[1];
			}
			else
			{
				stackVariable12 = null;
			}
			stackVariable5.templateName = stackVariable12;
			stackVariable13 = ViewModelBase<MixCmsContext, MixTemplate, ReadListItemViewModel>.Repository;
			V_1 = Expression.Parameter(Type.GetTypeFromHandle(// 
			// Current member / type: Mix.Cms.Lib.ViewModels.MixTemplates.ReadListItemViewModel Mix.Cms.Lib.ViewModels.MixTemplates.ReadListItemViewModel::GetTemplateByPath(System.Int32,System.String,System.String,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: Mix.Cms.Lib.ViewModels.MixTemplates.ReadListItemViewModel GetTemplateByPath(System.Int32,System.String,System.String,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

	}
}