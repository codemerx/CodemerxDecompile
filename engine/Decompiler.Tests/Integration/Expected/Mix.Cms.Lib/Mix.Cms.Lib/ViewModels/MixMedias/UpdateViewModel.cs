using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.Repositories;
using Mix.Cms.Lib.Services;
using Mix.Cms.Lib.ViewModels;
using Mix.Cms.Lib.ViewModels.MixCultures;
using Mix.Common.Helper;
using Mix.Domain.Core.Models;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixMedias
{
	public class UpdateViewModel : ViewModelBase<MixCmsContext, MixMedia, Mix.Cms.Lib.ViewModels.MixMedias.UpdateViewModel>
	{
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

		[JsonProperty("description")]
		public string Description
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

		[JsonProperty("extension")]
		public string Extension
		{
			get;
			set;
		}

		[JsonProperty("file")]
		public IFormFile File
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

		[JsonProperty("filePath")]
		public string FilePath
		{
			get
			{
				if (string.IsNullOrEmpty(this.FileName) || !string.IsNullOrEmpty(this.TargetUrl))
				{
					return this.TargetUrl;
				}
				if (this.FileFolder.IndexOf("http") > 0)
				{
					return string.Concat(this.FileFolder, "/", this.FileName, this.Extension);
				}
				return string.Concat(new string[] { "/", this.FileFolder, "/", this.FileName, this.Extension });
			}
		}

		[JsonProperty("fileSize")]
		public int FileSize
		{
			get;
			set;
		}

		[JsonProperty("fileType")]
		public string FileType
		{
			get;
			set;
		}

		[JsonProperty("fullPath")]
		public string FullPath
		{
			get
			{
				if (string.IsNullOrEmpty(this.FileName) || !string.IsNullOrEmpty(this.TargetUrl))
				{
					return this.TargetUrl;
				}
				if (this.FileFolder.IndexOf("http") > 0)
				{
					return string.Concat(this.FileFolder, "/", this.FileName, this.Extension);
				}
				return string.Concat(new string[] { this.Domain, "/", this.FileFolder, "/", this.FileName, this.Extension });
			}
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

		[JsonProperty("mediaFile")]
		public FileViewModel MediaFile
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

		[JsonProperty("source")]
		public string Source
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

		[JsonProperty("tags")]
		public string Tags
		{
			get;
			set;
		}

		[JsonProperty("targetUrl")]
		public string TargetUrl
		{
			get;
			set;
		}

		[JsonProperty("title")]
		public string Title
		{
			get;
			set;
		}

		public UpdateViewModel()
		{
		}

		public UpdateViewModel(MixMedia model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.MediaFile = new FileViewModel();
		}

		private List<SupportedCulture> LoadCultures(string initCulture = null, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<List<SystemCultureViewModel>> modelList = ViewModelBase<MixCmsContext, MixCulture, SystemCultureViewModel>.Repository.GetModelList(_context, _transaction);
			List<SupportedCulture> supportedCultures = new List<SupportedCulture>();
			if (modelList.get_IsSucceed())
			{
				foreach (SystemCultureViewModel datum in modelList.get_Data())
				{
					List<SupportedCulture> supportedCultures1 = supportedCultures;
					SupportedCulture supportedCulture = new SupportedCulture();
					supportedCulture.set_Icon(datum.Icon);
					supportedCulture.set_Specificulture(datum.Specificulture);
					supportedCulture.set_Alias(datum.Alias);
					supportedCulture.set_FullName(datum.FullName);
					supportedCulture.set_Description(datum.FullName);
					supportedCulture.set_Id(datum.Id);
					supportedCulture.set_Lcid(datum.Lcid);
					supportedCulture.set_IsSupported((datum.Specificulture == initCulture ? true : _context.MixMedia.Any<MixMedia>((MixMedia p) => p.Id == this.Id && p.Specificulture == datum.Specificulture)));
					supportedCultures1.Add(supportedCulture);
				}
			}
			return supportedCultures;
		}

		public override MixMedia ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (this.CreatedDateTime == new DateTime())
			{
				this.Id = (this.Id > 0 ? this.Id : ViewModelBase<MixCmsContext, MixMedia, Mix.Cms.Lib.ViewModels.MixMedias.UpdateViewModel>.Repository.Max((MixMedia c) => c.Id, null, null).get_Data() + 1);
				this.CreatedDateTime = DateTime.UtcNow;
			}
			if (string.IsNullOrEmpty(this.TargetUrl) && this.FileFolder[0] == '/')
			{
				this.FileFolder = this.FileFolder.Substring(1);
			}
			return base.ParseModel(_context, _transaction);
		}

		public override RepositoryResponse<bool> RemoveRelatedModels(Mix.Cms.Lib.ViewModels.MixMedias.UpdateViewModel view, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(FileRepository.Instance.DeleteWebFile(this.FileName, this.Extension, this.FileFolder));
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			repositoryResponse1.set_IsSucceed(ViewModelBase<MixCmsContext, MixMedia, Mix.Cms.Lib.ViewModels.MixMedias.UpdateViewModel>.Repository.RemoveListModel(false, (MixMedia m) => m.Id == this.Id && m.Specificulture != this.Specificulture, _context, _transaction).get_IsSucceed());
			return repositoryResponse1;
		}

		public override async Task<RepositoryResponse<bool>> RemoveRelatedModelsAsync(Mix.Cms.Lib.ViewModels.MixMedias.UpdateViewModel view, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (this.FileFolder.IndexOf("http") < 0)
			{
				FileRepository.Instance.DeleteWebFile(this.FileName, this.Extension, this.FileFolder);
			}
			DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixMedia, Mix.Cms.Lib.ViewModels.MixMedias.UpdateViewModel>.Repository;
			await repository.RemoveListModelAsync(false, (MixMedia m) => m.Id == this.Id && m.Specificulture != this.Specificulture, _context, _transaction);
			return await this.u003cu003en__0(view, _context, _transaction);
		}

		public override void Validate(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			DateTime utcNow;
			bool fileStream;
			FileViewModel mediaFile = this.MediaFile;
			if (mediaFile != null)
			{
				fileStream = mediaFile.FileStream;
			}
			else
			{
				fileStream = false;
			}
			if (fileStream)
			{
				string templateUploadFolder = MixService.GetTemplateUploadFolder(this.Specificulture);
				utcNow = DateTime.UtcNow;
				this.FileFolder = string.Concat(templateUploadFolder, "/", utcNow.ToString("yyyy-MM"));
				FileViewModel fileViewModel = this.MediaFile;
				string lower = SeoHelper.GetSEOString(this.MediaFile.Filename, '-').ToLower();
				Guid guid = Guid.NewGuid();
				fileViewModel.Filename = string.Concat(lower, guid.ToString("N"));
				this.MediaFile.FileFolder = this.FileFolder;
				if (!FileRepository.Instance.SaveWebFile(this.MediaFile))
				{
					base.set_IsValid(false);
				}
				else
				{
					this.Extension = this.MediaFile.Extension.ToLower();
					this.FileName = this.MediaFile.Filename;
					this.FileFolder = this.MediaFile.FileFolder;
					if (string.IsNullOrEmpty(this.Title))
					{
						this.Title = this.FileName;
					}
				}
			}
			else if (this.File != null)
			{
				string str = MixService.GetTemplateUploadFolder(this.Specificulture);
				utcNow = DateTime.UtcNow;
				this.FileFolder = string.Concat(str, "/", utcNow.ToString("yyyy-MM"));
				string sEOString = SeoHelper.GetSEOString(this.File.get_FileName().Substring(0, this.File.get_FileName().LastIndexOf('.')), '-');
				long ticks = DateTime.UtcNow.Ticks;
				this.FileName = string.Concat(sEOString, ticks.ToString());
				this.Extension = this.File.get_FileName().Substring(this.File.get_FileName().LastIndexOf('.'));
				RepositoryResponse<FileViewModel> repositoryResponse = FileRepository.Instance.SaveWebFile(this.File, string.Concat(this.FileName, this.Extension), this.FileFolder);
				if (repositoryResponse.get_IsSucceed())
				{
					base.set_IsValid(false);
					base.get_Errors().AddRange(repositoryResponse.get_Errors());
				}
				if (string.IsNullOrEmpty(this.Title))
				{
					this.Title = this.FileName;
				}
			}
			this.FileType = this.FileType ?? "image";
			base.Validate(_context, _transaction);
		}
	}
}