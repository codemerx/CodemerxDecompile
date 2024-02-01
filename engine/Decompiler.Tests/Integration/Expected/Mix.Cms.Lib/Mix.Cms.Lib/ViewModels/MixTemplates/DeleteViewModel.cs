using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.Repositories;
using Mix.Common.Helper;
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
				return CommonHelper.GetFullPath(new string[] { "content", "templates", this.ThemeName });
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

		public DeleteViewModel()
		{
		}

		public DeleteViewModel(MixTemplate model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override MixTemplate ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (this.Id == 0)
			{
				this.CreatedDateTime = DateTime.UtcNow;
			}
			this.FileFolder = CommonHelper.GetFullPath(new string[] { "Views/Shared/Templates", this.ThemeName, this.FolderType });
			return base.ParseModel(_context, _transaction);
		}

		public override RepositoryResponse<MixTemplate> RemoveModel(bool isRemoveRelatedModels = false, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<MixTemplate> repositoryResponse = base.RemoveModel(isRemoveRelatedModels, _context, _transaction);
			if (repositoryResponse.get_IsSucceed())
			{
				TemplateRepository.Instance.DeleteTemplate(this.FileName, this.FileFolder);
			}
			return repositoryResponse;
		}

		public override async Task<RepositoryResponse<MixTemplate>> RemoveModelAsync(bool isRemoveRelatedModels = false, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<MixTemplate> repositoryResponse = await this.u003cu003en__0(isRemoveRelatedModels, _context, _transaction);
			if (repositoryResponse.get_IsSucceed())
			{
				TemplateRepository.Instance.DeleteTemplate(this.FileName, this.FileFolder);
			}
			return repositoryResponse;
		}
	}
}