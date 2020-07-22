using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels;
using Mix.Cms.Lib.ViewModels.MixCultures;
using Mix.Domain.Core.Models;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
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
				if (string.IsNullOrEmpty(this.get_FileName()) || !string.IsNullOrEmpty(this.get_TargetUrl()))
				{
					return this.get_TargetUrl();
				}
				if (this.get_FileFolder().IndexOf("http") > 0)
				{
					return string.Concat(this.get_FileFolder(), "/", this.get_FileName(), this.get_Extension());
				}
				stackVariable22 = new string[5];
				stackVariable22[0] = "/";
				stackVariable22[1] = this.get_FileFolder();
				stackVariable22[2] = "/";
				stackVariable22[3] = this.get_FileName();
				stackVariable22[4] = this.get_Extension();
				return string.Concat(stackVariable22);
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
				if (string.IsNullOrEmpty(this.get_FileName()) || !string.IsNullOrEmpty(this.get_TargetUrl()))
				{
					return this.get_TargetUrl();
				}
				if (this.get_FileFolder().IndexOf("http") > 0)
				{
					return string.Concat(this.get_FileFolder(), "/", this.get_FileName(), this.get_Extension());
				}
				stackVariable22 = new string[6];
				stackVariable22[0] = this.get_Domain();
				stackVariable22[1] = "/";
				stackVariable22[2] = this.get_FileFolder();
				stackVariable22[3] = "/";
				stackVariable22[4] = this.get_FileName();
				stackVariable22[5] = this.get_Extension();
				return string.Concat(stackVariable22);
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
			base();
			return;
		}

		public UpdateViewModel(MixMedia model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			base(model, _context, _transaction);
			return;
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.set_MediaFile(new FileViewModel());
			return;
		}

		private List<SupportedCulture> LoadCultures(string initCulture = null, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0 = ViewModelBase<MixCmsContext, MixCulture, SystemCultureViewModel>.Repository.GetModelList(_context, _transaction);
			V_1 = new List<SupportedCulture>();
			if (V_0.get_IsSucceed())
			{
				V_2 = V_0.get_Data().GetEnumerator();
				try
				{
					while (V_2.MoveNext())
					{
						V_3 = new Mix.Cms.Lib.ViewModels.MixMedias.UpdateViewModel.u003cu003ec__DisplayClass93_0();
						V_3.u003cu003e4__this = this;
						V_3.culture = V_2.get_Current();
						stackVariable19 = V_1;
						V_4 = new SupportedCulture();
						V_4.set_Icon(V_3.culture.get_Icon());
						V_4.set_Specificulture(V_3.culture.get_Specificulture());
						V_4.set_Alias(V_3.culture.get_Alias());
						V_4.set_FullName(V_3.culture.get_FullName());
						V_4.set_Description(V_3.culture.get_FullName());
						V_4.set_Id(V_3.culture.get_Id());
						V_4.set_Lcid(V_3.culture.get_Lcid());
						stackVariable49 = V_4;
						if (string.op_Equality(V_3.culture.get_Specificulture(), initCulture))
						{
							stackVariable55 = true;
						}
						else
						{
							stackVariable58 = _context.get_MixMedia();
							V_5 = Expression.Parameter(Type.GetTypeFromHandle(// 
							// Current member / type: System.Collections.Generic.List`1<Mix.Domain.Core.Models.SupportedCulture> Mix.Cms.Lib.ViewModels.MixMedias.UpdateViewModel::LoadCultures(System.String,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
							// Exception in: System.Collections.Generic.List<Mix.Domain.Core.Models.SupportedCulture> LoadCultures(System.String,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
							// Specified method is not supported.
							// 
							// mailto: JustDecompilePublicFeedback@telerik.com


		public override MixMedia ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			stackVariable1 = this.get_CreatedDateTime();
			V_0 = new DateTime();
			if (DateTime.op_Equality(stackVariable1, V_0))
			{
				if (this.get_Id() > 0)
				{
					stackVariable27 = this.get_Id();
				}
				else
				{
					stackVariable30 = ViewModelBase<MixCmsContext, MixMedia, Mix.Cms.Lib.ViewModels.MixMedias.UpdateViewModel>.Repository;
					V_1 = Expression.Parameter(Type.GetTypeFromHandle(// 
					// Current member / type: Mix.Cms.Lib.Models.Cms.MixMedia Mix.Cms.Lib.ViewModels.MixMedias.UpdateViewModel::ParseModel(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
					// Exception in: Mix.Cms.Lib.Models.Cms.MixMedia ParseModel(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
					// Specified method is not supported.
					// 
					// mailto: JustDecompilePublicFeedback@telerik.com


		public override RepositoryResponse<bool> RemoveRelatedModels(Mix.Cms.Lib.ViewModels.MixMedias.UpdateViewModel view, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			stackVariable0 = new RepositoryResponse<bool>();
			stackVariable0.set_IsSucceed(FileRepository.get_Instance().DeleteWebFile(this.get_FileName(), this.get_Extension(), this.get_FileFolder()));
			V_0 = stackVariable0;
			stackVariable10 = ViewModelBase<MixCmsContext, MixMedia, Mix.Cms.Lib.ViewModels.MixMedias.UpdateViewModel>.Repository;
			V_1 = Expression.Parameter(Type.GetTypeFromHandle(// 
			// Current member / type: Mix.Domain.Core.ViewModels.RepositoryResponse`1<System.Boolean> Mix.Cms.Lib.ViewModels.MixMedias.UpdateViewModel::RemoveRelatedModels(Mix.Cms.Lib.ViewModels.MixMedias.UpdateViewModel,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: Mix.Domain.Core.ViewModels.RepositoryResponse<System.Boolean> RemoveRelatedModels(Mix.Cms.Lib.ViewModels.MixMedias.UpdateViewModel,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public override async Task<RepositoryResponse<bool>> RemoveRelatedModelsAsync(Mix.Cms.Lib.ViewModels.MixMedias.UpdateViewModel view, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0.u003cu003e4__this = this;
			V_0.view = view;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixMedias.UpdateViewModel.u003cRemoveRelatedModelsAsyncu003ed__92>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public override void Validate(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			stackVariable1 = this.get_MediaFile();
			if (stackVariable1 != null)
			{
				stackVariable2 = stackVariable1.get_FileStream();
			}
			else
			{
				dummyVar0 = stackVariable1;
				stackVariable2 = false;
			}
			if (!stackVariable2)
			{
				if (this.get_File() != null)
				{
					stackVariable14 = MixService.GetTemplateUploadFolder(this.get_Specificulture());
					V_0 = DateTime.get_UtcNow();
					this.set_FileFolder(string.Concat(stackVariable14, "/", V_0.ToString("yyyy-MM")));
					stackVariable33 = SeoHelper.GetSEOString(this.get_File().get_FileName().Substring(0, this.get_File().get_FileName().LastIndexOf('.')), '-');
					V_3 = DateTime.get_UtcNow().get_Ticks();
					this.set_FileName(string.Concat(stackVariable33, V_3.ToString()));
					this.set_Extension(this.get_File().get_FileName().Substring(this.get_File().get_FileName().LastIndexOf('.')));
					V_2 = FileRepository.get_Instance().SaveWebFile(this.get_File(), string.Concat(this.get_FileName(), this.get_Extension()), this.get_FileFolder());
					if (V_2.get_IsSucceed())
					{
						this.set_IsValid(false);
						this.get_Errors().AddRange(V_2.get_Errors());
					}
					if (string.IsNullOrEmpty(this.get_Title()))
					{
						this.set_Title(this.get_FileName());
					}
				}
			}
			else
			{
				stackVariable78 = MixService.GetTemplateUploadFolder(this.get_Specificulture());
				V_0 = DateTime.get_UtcNow();
				this.set_FileFolder(string.Concat(stackVariable78, "/", V_0.ToString("yyyy-MM")));
				stackVariable86 = this.get_MediaFile();
				stackVariable92 = SeoHelper.GetSEOString(this.get_MediaFile().get_Filename(), '-').ToLower();
				V_1 = Guid.NewGuid();
				stackVariable86.set_Filename(string.Concat(stackVariable92, V_1.ToString("N")));
				this.get_MediaFile().set_FileFolder(this.get_FileFolder());
				if (!FileRepository.get_Instance().SaveWebFile(this.get_MediaFile()))
				{
					this.set_IsValid(false);
				}
				else
				{
					this.set_Extension(this.get_MediaFile().get_Extension().ToLower());
					this.set_FileName(this.get_MediaFile().get_Filename());
					this.set_FileFolder(this.get_MediaFile().get_FileFolder());
					if (string.IsNullOrEmpty(this.get_Title()))
					{
						this.set_Title(this.get_FileName());
					}
				}
			}
			stackVariable7 = this.get_FileType();
			if (stackVariable7 == null)
			{
				dummyVar1 = stackVariable7;
				stackVariable7 = "image";
			}
			this.set_FileType(stackVariable7);
			this.Validate(_context, _transaction);
			return;
		}
	}
}