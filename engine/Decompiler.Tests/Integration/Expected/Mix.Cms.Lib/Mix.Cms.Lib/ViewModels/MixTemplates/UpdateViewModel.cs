using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixTemplates
{
	public class UpdateViewModel : ViewModelBase<MixCmsContext, MixTemplate, UpdateViewModel>
	{
		[JsonProperty("assetFolder")]
		public string AssetFolder
		{
			get
			{
				stackVariable1 = new string[3];
				stackVariable1[0] = "content";
				stackVariable1[1] = "templates";
				stackVariable1[2] = SeoHelper.GetSEOString(this.get_ThemeName(), '-');
				return CommonHelper.GetFullPath(stackVariable1);
			}
		}

		[JsonProperty("content")]
		[Required]
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

		[JsonProperty("layout")]
		public string Layout
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
				stackVariable1[1] = SeoHelper.GetSEOString(this.get_ThemeName(), '-');
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

		public UpdateViewModel()
		{
			this.u003cExtensionu003ek__BackingField = ".cshtml";
			this.u003cMobileContentu003ek__BackingField = "{}";
			this.u003cSpaContentu003ek__BackingField = "";
			base();
			return;
		}

		public UpdateViewModel(MixTemplate model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.u003cExtensionu003ek__BackingField = ".cshtml";
			this.u003cMobileContentu003ek__BackingField = "{}";
			this.u003cSpaContentu003ek__BackingField = "";
			base(model, _context, _transaction);
			return;
		}

		public async Task<RepositoryResponse<UpdateViewModel>> CopyAsync()
		{
			V_0.u003cu003e4__this = this;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<UpdateViewModel>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<UpdateViewModel.u003cCopyAsyncu003ed__95>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (!string.IsNullOrEmpty(this.get_FileName()))
			{
				V_0 = FileRepository.get_Instance().GetFile(this.get_FileName(), this.get_Extension(), this.get_FileFolder(), false, "");
				if (V_0 != null)
				{
					stackVariable15 = V_0.get_Content();
				}
				else
				{
					stackVariable15 = null;
				}
				if (!string.IsNullOrWhiteSpace(stackVariable15))
				{
					this.set_Content(V_0.get_Content());
				}
			}
			return;
		}

		public static UpdateViewModel GetDefault(MixEnums.EnumTemplateFolder folderType, string specificulture)
		{
			// 
			// Current member / type: Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel::GetDefault(Mix.Cms.Lib.MixEnums/EnumTemplateFolder,System.String)
			// Exception in: Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel GetDefault(Mix.Cms.Lib.MixEnums/EnumTemplateFolder,System.String)
			// Object reference not set to an instance of an object.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		public static RepositoryResponse<UpdateViewModel> GetTemplateByPath(string path, string culture, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0 = new UpdateViewModel.u003cu003ec__DisplayClass92_0();
			V_1 = new RepositoryResponse<UpdateViewModel>();
			stackVariable2 = V_0;
			if (path != null)
			{
				stackVariable7 = path.Split('/', 0);
			}
			else
			{
				stackVariable7 = null;
			}
			stackVariable2.temp = stackVariable7;
			if (V_0.temp == null || (int)V_0.temp.Length < 2)
			{
				V_1.set_IsSucceed(false);
				V_1.get_Errors().Add("Template Not Found");
			}
			else
			{
				V_0.activeThemeId = MixService.GetConfig<int>("ThemeId", culture);
				V_0.name = V_0.temp[1].Split('.', 0)[0];
				stackVariable35 = ViewModelBase<MixCmsContext, MixTemplate, UpdateViewModel>.Repository;
				V_2 = Expression.Parameter(Type.GetTypeFromHandle(// 
				// Current member / type: Mix.Domain.Core.ViewModels.RepositoryResponse`1<Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel> Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel::GetTemplateByPath(System.String,System.String,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Exception in: Mix.Domain.Core.ViewModels.RepositoryResponse<Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel> GetTemplateByPath(System.String,System.String,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Specified method is not supported.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com


		public static UpdateViewModel GetTemplateByPath(string path, string specificulture, MixEnums.EnumTemplateFolder folderType, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0 = new UpdateViewModel.u003cu003ec__DisplayClass93_0();
			V_0.folderType = folderType;
			stackVariable3 = V_0;
			if (path != null)
			{
				stackVariable10 = path.Split('/', 0)[1];
			}
			else
			{
				stackVariable10 = null;
			}
			stackVariable3.templateName = stackVariable10;
			V_0.themeId = MixService.GetConfig<int>("ThemeId", specificulture);
			dummyVar0 = MixService.GetConfig<string>("ThemeName", specificulture);
			stackVariable18 = ViewModelBase<MixCmsContext, MixTemplate, UpdateViewModel>.Repository;
			V_1 = Expression.Parameter(Type.GetTypeFromHandle(// 
			// Current member / type: Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel::GetTemplateByPath(System.String,System.String,Mix.Cms.Lib.MixEnums/EnumTemplateFolder,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel GetTemplateByPath(System.String,System.String,Mix.Cms.Lib.MixEnums/EnumTemplateFolder,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public override MixTemplate ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (this.get_Id() == 0)
			{
				stackVariable31 = ViewModelBase<MixCmsContext, MixTemplate, UpdateViewModel>.Repository;
				V_0 = Expression.Parameter(Type.GetTypeFromHandle(// 
				// Current member / type: Mix.Cms.Lib.Models.Cms.MixTemplate Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel::ParseModel(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Exception in: Mix.Cms.Lib.Models.Cms.MixTemplate ParseModel(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Specified method is not supported.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com


		public override RepositoryResponse<MixTemplate> RemoveModel(bool isRemoveRelatedModels = false, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			stackVariable4 = this.RemoveModel(isRemoveRelatedModels, _context, _transaction);
			if (stackVariable4.get_IsSucceed())
			{
				dummyVar0 = TemplateRepository.get_Instance().DeleteTemplate(this.get_FileName(), this.get_FileFolder());
			}
			return stackVariable4;
		}

		public override async Task<RepositoryResponse<MixTemplate>> RemoveModelAsync(bool isRemoveRelatedModels = false, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0.u003cu003e4__this = this;
			V_0.isRemoveRelatedModels = isRemoveRelatedModels;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<MixTemplate>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<UpdateViewModel.u003cRemoveModelAsyncu003ed__90>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public override async Task<RepositoryResponse<UpdateViewModel>> SaveModelAsync(bool isSaveSubModels = false, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0.u003cu003e4__this = this;
			V_0.isSaveSubModels = isSaveSubModels;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<UpdateViewModel>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<UpdateViewModel.u003cSaveModelAsyncu003ed__87>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public override RepositoryResponse<bool> SaveSubModels(MixTemplate parent, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			stackVariable0 = TemplateRepository.get_Instance();
			stackVariable1 = new TemplateViewModel();
			stackVariable1.set_Filename(this.get_FileName());
			stackVariable1.set_Extension(this.get_Extension());
			stackVariable1.set_Content(this.get_Content());
			stackVariable1.set_FileFolder(this.get_FileFolder());
			dummyVar0 = stackVariable0.SaveTemplate(stackVariable1);
			return this.SaveSubModels(parent, _context, _transaction);
		}

		public override Task<RepositoryResponse<bool>> SaveSubModelsAsync(MixTemplate parent, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			stackVariable0 = TemplateRepository.get_Instance();
			stackVariable1 = new TemplateViewModel();
			stackVariable1.set_Filename(this.get_FileName());
			stackVariable1.set_Extension(this.get_Extension());
			stackVariable1.set_Content(this.get_Content());
			stackVariable1.set_FileFolder(this.get_FileFolder());
			dummyVar0 = stackVariable0.SaveTemplate(stackVariable1);
			return this.SaveSubModelsAsync(parent, _context, _transaction);
		}

		public override void Validate(MixCmsContext _context, IDbContextTransaction _transaction)
		{
			this.Validate(_context, _transaction);
			if (this.get_IsValid())
			{
				if (this.get_Id() == 0)
				{
					stackVariable42 = _context.get_MixTemplate();
					V_0 = Expression.Parameter(Type.GetTypeFromHandle(// 
					// Current member / type: System.Void Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel::Validate(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
					// Exception in: System.Void Validate(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
					// Specified method is not supported.
					// 
					// mailto: JustDecompilePublicFeedback@telerik.com

	}
}