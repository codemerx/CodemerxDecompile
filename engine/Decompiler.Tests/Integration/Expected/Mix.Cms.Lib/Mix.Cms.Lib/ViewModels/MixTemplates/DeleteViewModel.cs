using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib.Models.Cms;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixTemplates
{
	public class DeleteViewModel : ViewModelBase<MixCmsContext, MixTemplate, DeleteViewModel>
	{
		[JsonIgnore]
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

		[JsonIgnore]
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

		[JsonIgnore]
		[JsonProperty("lastModified")]
		public DateTime? LastModified
		{
			get;
			set;
		}

		[JsonIgnore]
		[JsonProperty("modifiedBy")]
		public string ModifiedBy
		{
			get;
			set;
		}

		[JsonIgnore]
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

		public DeleteViewModel()
		{
			base();
			return;
		}

		public DeleteViewModel(MixTemplate model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
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
			V_0.u003cu003et__builder.Start<DeleteViewModel.u003cRemoveModelAsyncu003ed__50>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}