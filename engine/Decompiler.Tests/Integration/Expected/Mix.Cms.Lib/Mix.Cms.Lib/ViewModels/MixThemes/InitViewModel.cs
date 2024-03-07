using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.Repositories;
using Mix.Cms.Lib.Services;
using Mix.Cms.Lib.ViewModels;
using Mix.Cms.Lib.ViewModels.MixConfigurations;
using Mix.Cms.Lib.ViewModels.MixTemplates;
using Mix.Common.Helper;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixThemes
{
	public class InitViewModel : ViewModelBase<MixCmsContext, MixTheme, Mix.Cms.Lib.ViewModels.MixThemes.InitViewModel>
	{
		[JsonProperty("asset")]
		public FileViewModel Asset
		{
			get;
			set;
		}

		[JsonProperty("assetFolder")]
		public string AssetFolder
		{
			get
			{
				return string.Concat("wwwroot/content/templates/", this.Name, "/assets");
			}
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

		[JsonProperty("domain")]
		public string Domain
		{
			get
			{
				return MixService.GetConfig<string>("Domain");
			}
		}

		[JsonProperty("id")]
		public int Id
		{
			get;
			set;
		}

		[JsonProperty("image")]
		public string Image
		{
			get;
			set;
		}

		[JsonProperty("imageUrl")]
		public string ImageUrl
		{
			get
			{
				if (string.IsNullOrEmpty(this.Image) || this.Image.IndexOf("http") != -1 || this.Image[0] == '/')
				{
					return this.Image;
				}
				return CommonHelper.GetFullPath(new string[] { this.Domain, this.Image });
			}
		}

		[JsonProperty("isActived")]
		public bool IsActived
		{
			get;
			set;
		}

		[JsonProperty("isCreateDefault")]
		public bool IsCreateDefault
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

		[JsonProperty("modifiedBy")]
		public string ModifiedBy
		{
			get;
			set;
		}

		[JsonProperty("name")]
		public string Name
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

		[JsonProperty("specificulture")]
		public string Specificulture
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

		[JsonProperty("templateAsset")]
		public FileViewModel TemplateAsset
		{
			get;
			set;
		}

		[JsonProperty("templateFolder")]
		public string TemplateFolder
		{
			get
			{
				return string.Concat("Views/Shared/Templates/", this.Name);
			}
		}

		public List<Mix.Cms.Lib.ViewModels.MixTemplates.InitViewModel> Templates
		{
			get;
			set;
		}

		[JsonProperty("title")]
		[Required]
		public string Title
		{
			get;
			set;
		}

		public string UploadsFolder
		{
			get
			{
				return string.Concat("wwwroot/content/templates/", this.Name, "/uploads");
			}
		}

		public InitViewModel()
		{
		}

		public InitViewModel(MixTheme model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		private async Task<RepositoryResponse<bool>> ActivedThemeAsync(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			int id;
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixConfiguration, SystemConfigurationViewModel>.Repository;
			SystemConfigurationViewModel data = await repository.GetSingleModelAsync((MixConfiguration c) => c.Keyword == "ThemeName" && c.Specificulture == this.Specificulture, _context, _transaction).get_Data();
			if (data != null)
			{
				data.Value = this.Name;
			}
			else
			{
				SystemConfigurationViewModel systemConfigurationViewModel = new SystemConfigurationViewModel()
				{
					Keyword = "ThemeName",
					Specificulture = this.Specificulture,
					Category = "Site",
					DataType = MixEnums.MixDataType.Text,
					Description = "Cms Theme",
					Value = this.Name
				};
				data = systemConfigurationViewModel;
			}
			RepositoryResponse<SystemConfigurationViewModel> repositoryResponse2 = await data.SaveModelAsync(false, _context, _transaction);
			if (repositoryResponse2.get_IsSucceed())
			{
				DefaultRepository<!0, !1, !2> defaultRepository = ViewModelBase<MixCmsContext, MixConfiguration, SystemConfigurationViewModel>.Repository;
				SystemConfigurationViewModel name = await defaultRepository.GetSingleModelAsync((MixConfiguration c) => c.Keyword == "ThemeFolder" && c.Specificulture == this.Specificulture, _context, _transaction).get_Data();
				name.Value = this.Name;
				repositoryResponse2 = await ((ViewModelBase<MixCmsContext, MixConfiguration, SystemConfigurationViewModel>)name).SaveModelAsync(false, _context, _transaction);
			}
			ViewModelHelper.HandleResult<SystemConfigurationViewModel>(repositoryResponse2, ref repositoryResponse1);
			if (repositoryResponse1.get_IsSucceed())
			{
				DefaultRepository<!0, !1, !2> repository1 = ViewModelBase<MixCmsContext, MixConfiguration, SystemConfigurationViewModel>.Repository;
				SystemConfigurationViewModel str = await repository1.GetSingleModelAsync((MixConfiguration c) => c.Keyword == "ThemeId" && c.Specificulture == this.Specificulture, _context, _transaction).get_Data();
				if (str != null)
				{
					id = base.get_Model().Id;
					str.Value = id.ToString();
				}
				else
				{
					SystemConfigurationViewModel systemConfigurationViewModel1 = new SystemConfigurationViewModel()
					{
						Keyword = "ThemeId",
						Specificulture = this.Specificulture,
						Category = "Site",
						DataType = MixEnums.MixDataType.Text,
						Description = "Cms Theme Id"
					};
					id = base.get_Model().Id;
					systemConfigurationViewModel1.Value = id.ToString();
					str = systemConfigurationViewModel1;
				}
				ViewModelHelper.HandleResult<SystemConfigurationViewModel>(await str.SaveModelAsync(false, _context, _transaction), ref repositoryResponse1);
			}
			return repositoryResponse1;
		}

		private async Task<RepositoryResponse<bool>> CreateDefaultThemeTemplatesAsync(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			string config = MixService.GetConfig<string>("DefaultTemplateFolder");
			if (config == null)
			{
				config = "";
			}
			FileRepository.Instance.CopyDirectory(config, this.TemplateFolder);
			List<FileViewModel> filesWithContent = FileRepository.Instance.GetFilesWithContent(this.TemplateFolder);
			int num = _context.MixTemplate.Count<MixTemplate>() + 1;
			foreach (FileViewModel fileViewModel in filesWithContent)
			{
				MixTemplate mixTemplate = new MixTemplate()
				{
					Id = num,
					FileFolder = fileViewModel.FileFolder,
					FileName = fileViewModel.Filename,
					Content = fileViewModel.Content,
					Extension = fileViewModel.Extension,
					CreatedDateTime = DateTime.UtcNow,
					LastModified = new DateTime?(DateTime.UtcNow),
					ThemeId = base.get_Model().Id,
					ThemeName = base.get_Model().Name,
					FolderType = fileViewModel.FolderName,
					ModifiedBy = this.CreatedBy
				};
				ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixTemplates.InitViewModel>(await (new Mix.Cms.Lib.ViewModels.MixTemplates.InitViewModel(mixTemplate, _context, _transaction)).SaveModelAsync(true, _context, _transaction), ref repositoryResponse1);
				if (!repositoryResponse1.get_IsSucceed())
				{
					break;
				}
				num++;
			}
			return repositoryResponse1;
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.Templates = ViewModelBase<MixCmsContext, MixTemplate, Mix.Cms.Lib.ViewModels.MixTemplates.InitViewModel>.Repository.GetModelListBy((MixTemplate t) => t.ThemeId == this.Id, _context, _transaction).get_Data();
			FileViewModel fileViewModel = new FileViewModel();
			DateTime utcNow = DateTime.UtcNow;
			fileViewModel.FileFolder = string.Concat("Import/Themes/", utcNow.ToShortDateString(), "/", this.Name);
			this.TemplateAsset = fileViewModel;
			this.Asset = new FileViewModel()
			{
				FileFolder = this.AssetFolder
			};
		}

		private async Task<RepositoryResponse<bool>> ImportThemeAsync(MixTheme parent, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			bool flag;
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			string str = string.Concat(this.TemplateAsset.FileFolder, "/", this.TemplateAsset.Filename, this.TemplateAsset.Extension);
			if (File.Exists(str))
			{
				string str1 = string.Concat(this.TemplateAsset.FileFolder, "/Extract");
				FileRepository.Instance.DeleteFolder(str1);
				FileRepository.Instance.CreateDirectoryIfNotExist(str1);
				FileRepository.Instance.UnZipFile(str, str1);
				FileRepository instance = FileRepository.Instance;
				string str2 = string.Concat(str1, "/Assets");
				string assetFolder = this.AssetFolder;
				if (assetFolder == null)
				{
					assetFolder = "";
				}
				instance.CopyDirectory(str2, assetFolder);
				FileRepository.Instance.CopyDirectory(string.Concat(str1, "/Templates"), this.TemplateFolder);
				FileRepository fileRepository = FileRepository.Instance;
				string str3 = string.Concat(str1, "/Uploads");
				string uploadsFolder = this.UploadsFolder;
				if (uploadsFolder == null)
				{
					uploadsFolder = "";
				}
				fileRepository.CopyDirectory(str3, uploadsFolder);
				FileViewModel file = FileRepository.Instance.GetFile("schema.json", string.Concat(str1, "/Data"), false, "");
				SiteStructureViewModel obj = JObject.Parse(file.Content).ToObject<SiteStructureViewModel>();
				FileRepository.Instance.DeleteFolder(str1);
				repositoryResponse1 = await obj.ImportAsync(this.Specificulture, null, null);
				if (repositoryResponse1.get_IsSucceed())
				{
					foreach (FileViewModel filesWithContent in FileRepository.Instance.GetFilesWithContent(this.TemplateFolder))
					{
						filesWithContent.Content.Replace(string.Concat("/Content/Templates/", obj.ThemeName, "/"), string.Concat("/Content/Templates/", this.Name, "/"));
						MixTemplate mixTemplate = new MixTemplate()
						{
							FileFolder = filesWithContent.FileFolder,
							FileName = filesWithContent.Filename,
							Content = filesWithContent.Content,
							Extension = filesWithContent.Extension,
							CreatedDateTime = DateTime.UtcNow,
							LastModified = new DateTime?(DateTime.UtcNow),
							ThemeId = parent.Id,
							ThemeName = parent.Name,
							FolderType = filesWithContent.FolderName,
							ModifiedBy = this.CreatedBy
						};
						RepositoryResponse<Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel> repositoryResponse2 = await (new Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel(mixTemplate, null, null)).SaveModelAsync(true, _context, _transaction);
						RepositoryResponse<bool> repositoryResponse3 = repositoryResponse1;
						flag = (!repositoryResponse1.get_IsSucceed() ? false : repositoryResponse2.get_IsSucceed());
						repositoryResponse3.set_IsSucceed(flag);
						if (repositoryResponse2.get_IsSucceed())
						{
							continue;
						}
						repositoryResponse1.set_IsSucceed(false);
						repositoryResponse1.set_Exception(repositoryResponse2.get_Exception());
						repositoryResponse1.get_Errors().AddRange(repositoryResponse2.get_Errors());
						break;
					}
				}
				obj = null;
			}
			return repositoryResponse1;
		}

		public override MixTheme ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.Id = 1;
			this.Name = SeoHelper.GetSEOString(this.Title, '-');
			this.CreatedDateTime = DateTime.UtcNow;
			return base.ParseModel(_context, _transaction);
		}

		public override async Task<RepositoryResponse<bool>> SaveSubModelsAsync(MixTheme parent, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			if (!string.IsNullOrEmpty(this.TemplateAsset.Filename))
			{
				repositoryResponse1 = await this.ImportThemeAsync(parent, _context, _transaction);
			}
			if (repositoryResponse1.get_IsSucceed() && !Directory.Exists(this.TemplateFolder) && string.IsNullOrEmpty(this.TemplateAsset.Filename))
			{
				repositoryResponse1 = await this.CreateDefaultThemeTemplatesAsync(_context, _transaction);
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				repositoryResponse1 = await this.ActivedThemeAsync(_context, _transaction);
			}
			return repositoryResponse1;
		}
	}
}