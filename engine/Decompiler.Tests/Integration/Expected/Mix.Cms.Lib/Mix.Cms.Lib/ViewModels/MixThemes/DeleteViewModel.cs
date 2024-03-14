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
using System.Text;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixThemes
{
	public class DeleteViewModel : ViewModelBase<MixCmsContext, MixTheme, Mix.Cms.Lib.ViewModels.MixThemes.DeleteViewModel>
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

		public List<Mix.Cms.Lib.ViewModels.MixTemplates.DeleteViewModel> Templates
		{
			get;
			set;
		}

		[JsonProperty("thumbnail")]
		public string Thumbnail
		{
			get;
			set;
		}

		[JsonProperty("thumbnailUrl")]
		public string ThumbnailUrl
		{
			get
			{
				if (this.Thumbnail == null || this.Thumbnail.IndexOf("http") != -1 || this.Thumbnail[0] == '/')
				{
					if (!string.IsNullOrEmpty(this.Thumbnail))
					{
						return this.Thumbnail;
					}
					return this.ImageUrl;
				}
				return CommonHelper.GetFullPath(new string[] { this.Domain, this.Thumbnail });
			}
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

		public DeleteViewModel()
		{
		}

		public DeleteViewModel(MixTheme model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
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
			Mix.Cms.Lib.ViewModels.MixThemes.DeleteViewModel.u003cCreateDefaultThemeTemplatesAsyncu003ed__74 variable = new Mix.Cms.Lib.ViewModels.MixThemes.DeleteViewModel.u003cCreateDefaultThemeTemplatesAsyncu003ed__74();
			variable.u003cu003e4__this = this;
			variable._context = _context;
			variable._transaction = _transaction;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixThemes.DeleteViewModel.u003cCreateDefaultThemeTemplatesAsyncu003ed__74>(ref variable);
			return variable.u003cu003et__builder.Task;
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.Templates = ViewModelBase<MixCmsContext, MixTemplate, Mix.Cms.Lib.ViewModels.MixTemplates.DeleteViewModel>.Repository.GetModelListBy((MixTemplate t) => t.ThemeId == this.Id, _context, _transaction).get_Data();
			FileViewModel fileViewModel = new FileViewModel();
			DateTime utcNow = DateTime.UtcNow;
			fileViewModel.FileFolder = string.Concat("wwwroot/import/themes/", utcNow.ToShortDateString(), "/", this.Name);
			this.TemplateAsset = fileViewModel;
			this.Asset = new FileViewModel()
			{
				FileFolder = this.AssetFolder
			};
		}

		private RepositoryResponse<bool> ImportAssetsAsync(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			string str = string.Concat(this.Asset.FileFolder, "/", this.Asset.Filename, this.Asset.Extension);
			if (!File.Exists(str))
			{
				repositoryResponse.get_Errors().Add("Cannot saved asset file");
			}
			else
			{
				FileRepository.Instance.UnZipFile(str, this.Asset.FileFolder);
				repositoryResponse.set_IsSucceed(true);
			}
			return repositoryResponse;
		}

		private async Task<RepositoryResponse<bool>> ImportThemeAsync(MixTheme parent, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			bool flag;
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			string[] fileFolder = new string[] { "wwwroot/", this.TemplateAsset.FileFolder, "/", this.TemplateAsset.Filename, this.TemplateAsset.Extension };
			string str = string.Concat(fileFolder);
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
				FileRepository.Instance.DeleteFile(str);
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
						RepositoryResponse<Mix.Cms.Lib.ViewModels.MixTemplates.DeleteViewModel> repositoryResponse2 = await (new Mix.Cms.Lib.ViewModels.MixTemplates.DeleteViewModel(mixTemplate, null, null)).SaveModelAsync(true, _context, _transaction);
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

		private async Task InitAssetStyleAsync(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			List<FileViewModel> webFiles = FileRepository.Instance.GetWebFiles(this.AssetFolder);
			StringBuilder stringBuilder = new StringBuilder();
			List<FileViewModel> fileViewModels = webFiles;
			foreach (FileViewModel fileViewModel in 
				from f in fileViewModels
				where f.Extension == ".css"
				select f)
			{
				string[] fileFolder = new string[] { "   <link href='", fileViewModel.FileFolder, "/", fileViewModel.Filename, fileViewModel.Extension, "' rel='stylesheet'/>" };
				stringBuilder.Append(string.Concat(fileFolder));
			}
			StringBuilder stringBuilder1 = new StringBuilder();
			List<FileViewModel> fileViewModels1 = webFiles;
			foreach (FileViewModel fileViewModel1 in 
				from f in fileViewModels1
				where f.Extension == ".js"
				select f)
			{
				string[] strArrays = new string[] { "  <script src='", fileViewModel1.FileFolder, "/", fileViewModel1.Filename, fileViewModel1.Extension, "'></script>" };
				stringBuilder1.Append(string.Concat(strArrays));
			}
			DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixTemplate, Mix.Cms.Lib.ViewModels.MixTemplates.InitViewModel>.Repository;
			ParameterExpression parameterExpression = Expression.Parameter(typeof(MixTemplate), "t");
			BinaryExpression binaryExpression = Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixTemplate).GetMethod("get_FileName").MethodHandle)), Expression.Constant("_Layout", typeof(string))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixTemplate).GetMethod("get_ThemeId").MethodHandle)), Expression.Property(Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixThemes.DeleteViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(ViewModelBase<MixCmsContext, MixTheme, Mix.Cms.Lib.ViewModels.MixThemes.DeleteViewModel>).GetMethod("get_Model").MethodHandle, typeof(ViewModelBase<MixCmsContext, MixTheme, Mix.Cms.Lib.ViewModels.MixThemes.DeleteViewModel>).TypeHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixTheme).GetMethod("get_Id").MethodHandle))));
			ParameterExpression[] parameterExpressionArray = new ParameterExpression[] { parameterExpression };
			RepositoryResponse<Mix.Cms.Lib.ViewModels.MixTemplates.InitViewModel> singleModel = repository.GetSingleModel(Expression.Lambda<Func<MixTemplate, bool>>(binaryExpression, parameterExpressionArray), _context, _transaction);
			singleModel.get_Data().Content = singleModel.get_Data().Content.Replace("<!--[STYLES]-->", string.Format("{0}", stringBuilder));
			singleModel.get_Data().Content = singleModel.get_Data().Content.Replace("<!--[SCRIPTS]-->", string.Format("{0}", stringBuilder1));
			await singleModel.get_Data().SaveModelAsync(true, _context, _transaction);
		}

		public override async Task<RepositoryResponse<bool>> RemoveRelatedModelsAsync(Mix.Cms.Lib.ViewModels.MixThemes.DeleteViewModel view, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			Mix.Cms.Lib.ViewModels.MixThemes.DeleteViewModel.u003cRemoveRelatedModelsAsyncu003ed__71 variable = new Mix.Cms.Lib.ViewModels.MixThemes.DeleteViewModel.u003cRemoveRelatedModelsAsyncu003ed__71();
			variable.u003cu003e4__this = this;
			variable._context = _context;
			variable._transaction = _transaction;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixThemes.DeleteViewModel.u003cRemoveRelatedModelsAsyncu003ed__71>(ref variable);
			return variable.u003cu003et__builder.Task;
		}
	}
}