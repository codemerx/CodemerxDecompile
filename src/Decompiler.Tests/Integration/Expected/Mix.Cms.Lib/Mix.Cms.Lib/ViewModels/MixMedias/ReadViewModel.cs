using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.Services;
using Mix.Cms.Lib.ViewModels;
using Mix.Cms.Lib.ViewModels.MixCultures;
using Mix.Domain.Core.Models;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels.MixMedias
{
	public class ReadViewModel : ViewModelBase<MixCmsContext, MixMedia, Mix.Cms.Lib.ViewModels.MixMedias.ReadViewModel>
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
		[Required(ErrorMessage="Please choose File")]
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

		public ReadViewModel()
		{
		}

		public ReadViewModel(MixMedia model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
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
	}
}