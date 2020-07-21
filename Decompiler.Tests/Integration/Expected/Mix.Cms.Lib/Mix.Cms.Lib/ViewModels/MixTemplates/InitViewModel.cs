using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixTemplates
{
	public class InitViewModel : ViewModelBase<MixCmsContext, MixTemplate, InitViewModel>
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
				stackVariable1[1] = this.get_ThemeName();
				return CommonHelper.GetFullPath(stackVariable1);
			}
		}

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

		public InitViewModel()
		{
			this.u003cMobileContentu003ek__BackingField = "{}";
			this.u003cSpaContentu003ek__BackingField = "";
			base();
			return;
		}

		public InitViewModel(MixTemplate model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.u003cMobileContentu003ek__BackingField = "{}";
			this.u003cSpaContentu003ek__BackingField = "";
			base(model, _context, _transaction);
			return;
		}

		public override MixTemplate ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (this.get_Id() == 0)
			{
				this.set_CreatedDateTime(DateTime.get_UtcNow());
			}
			stackVariable4 = new string[3];
			stackVariable4[0] = "Views/Shared/Templates";
			stackVariable4[1] = this.get_ThemeName();
			stackVariable4[2] = this.get_FolderType();
			this.set_FileFolder(CommonHelper.GetFullPath(stackVariable4));
			stackVariable16 = this.get_Content();
			if (stackVariable16 != null)
			{
				stackVariable17 = stackVariable16.Trim();
			}
			else
			{
				dummyVar0 = stackVariable16;
				stackVariable17 = null;
			}
			this.set_Content(stackVariable17);
			stackVariable20 = this.get_Scripts();
			if (stackVariable20 != null)
			{
				stackVariable21 = stackVariable20.Trim();
			}
			else
			{
				dummyVar1 = stackVariable20;
				stackVariable21 = null;
			}
			this.set_Scripts(stackVariable21);
			stackVariable24 = this.get_Styles();
			if (stackVariable24 != null)
			{
				stackVariable25 = stackVariable24.Trim();
			}
			else
			{
				dummyVar2 = stackVariable24;
				stackVariable25 = null;
			}
			this.set_Styles(stackVariable25);
			return this.ParseModel(_context, _transaction);
		}

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
			V_0.u003cu003et__builder.Start<InitViewModel.u003cRemoveModelAsyncu003ed__87>(ref V_0);
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
	}
}