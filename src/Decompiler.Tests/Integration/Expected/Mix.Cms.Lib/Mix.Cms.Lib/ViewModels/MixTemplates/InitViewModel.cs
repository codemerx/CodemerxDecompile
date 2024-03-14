using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.Repositories;
using Mix.Cms.Lib.ViewModels;
using Mix.Common.Helper;
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
				return CommonHelper.GetFullPath(new string[] { "content", "templates", this.ThemeName });
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

		public InitViewModel()
		{
		}

		public InitViewModel(MixTemplate model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override MixTemplate ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			string str;
			string str1;
			string str2;
			if (this.Id == 0)
			{
				this.CreatedDateTime = DateTime.UtcNow;
			}
			this.FileFolder = CommonHelper.GetFullPath(new string[] { "Views/Shared/Templates", this.ThemeName, this.FolderType });
			string content = this.Content;
			if (content != null)
			{
				str = content.Trim();
			}
			else
			{
				str = null;
			}
			this.Content = str;
			string scripts = this.Scripts;
			if (scripts != null)
			{
				str1 = scripts.Trim();
			}
			else
			{
				str1 = null;
			}
			this.Scripts = str1;
			string styles = this.Styles;
			if (styles != null)
			{
				str2 = styles.Trim();
			}
			else
			{
				str2 = null;
			}
			this.Styles = str2;
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

		public override RepositoryResponse<bool> SaveSubModels(MixTemplate parent, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			TemplateRepository.Instance.SaveTemplate(new TemplateViewModel()
			{
				Filename = this.FileName,
				Extension = this.Extension,
				Content = this.Content,
				FileFolder = this.FileFolder
			});
			return base.SaveSubModels(parent, _context, _transaction);
		}

		public override Task<RepositoryResponse<bool>> SaveSubModelsAsync(MixTemplate parent, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			TemplateRepository.Instance.SaveTemplate(new TemplateViewModel()
			{
				Filename = this.FileName,
				Extension = this.Extension,
				Content = this.Content,
				FileFolder = this.FileFolder
			});
			return base.SaveSubModelsAsync(parent, _context, _transaction);
		}
	}
}