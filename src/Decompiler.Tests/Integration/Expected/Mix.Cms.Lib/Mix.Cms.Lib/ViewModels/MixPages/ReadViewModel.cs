using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.Services;
using Mix.Cms.Lib.ViewModels.MixPagePosts;
using Mix.Common.Helper;
using Mix.Domain.Core.Models;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixPages
{
	public class ReadViewModel : ViewModelBase<MixCmsContext, MixPage, Mix.Cms.Lib.ViewModels.MixPages.ReadViewModel>
	{
		[JsonProperty("childs")]
		public List<Mix.Cms.Lib.ViewModels.MixPages.ReadViewModel> Childs
		{
			get;
			set;
		}

		[JsonProperty("content")]
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

		[JsonProperty("cssClass")]
		public string CssClass
		{
			get;
			set;
		}

		[JsonProperty("cultures")]
		public List<SupportedCulture> Cultures
		{
			get;
			set;
		}

		[JsonProperty("detailsUrl")]
		public string DetailsUrl
		{
			get
			{
				return string.Concat("page/", this.Specificulture, "/", this.SeoName);
			}
		}

		[JsonProperty("domain")]
		public string Domain
		{
			get
			{
				return MixService.GetConfig<string>("Domain");
			}
		}

		[JsonProperty("excerpt")]
		public string Excerpt
		{
			get;
			set;
		}

		[JsonProperty("icon")]
		public string Icon
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
				if (string.IsNullOrEmpty(this.Image) || this.Image.IndexOf("http") != -1)
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

		[JsonProperty("level")]
		public int? Level
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

		[JsonProperty("pageSize")]
		public int? PageSize
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

		[JsonProperty("seoDescription")]
		public string SeoDescription
		{
			get;
			set;
		}

		[JsonProperty("seoKeywords")]
		public string SeoKeywords
		{
			get;
			set;
		}

		[JsonProperty("seoName")]
		public string SeoName
		{
			get;
			set;
		}

		[JsonProperty("seoTitle")]
		public string SeoTitle
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

		[JsonProperty("staticUrl")]
		public string StaticUrl
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

		[JsonProperty("template")]
		public string Template
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
		public string Title
		{
			get;
			set;
		}

		[JsonProperty("totalPost")]
		public int TotalPost
		{
			get;
			set;
		}

		[JsonProperty("totalProduct")]
		public int TotalProduct
		{
			get;
			set;
		}

		[JsonProperty("type")]
		public MixEnums.MixPageType Type
		{
			get;
			set;
		}

		[JsonProperty("views")]
		public int? Views
		{
			get;
			set;
		}

		public ReadViewModel()
		{
		}

		public ReadViewModel(MixPage model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<int> repositoryResponse = ViewModelBase<MixCmsContext, MixPagePost, Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel>.Repository.Count((MixPagePost c) => c.PageId == this.Id && c.Specificulture == this.Specificulture, _context, _transaction);
			if (repositoryResponse.get_IsSucceed())
			{
				this.TotalPost = repositoryResponse.get_Data();
			}
		}

		public static async Task<RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixPages.ReadViewModel>>> UpdateInfosAsync(List<Mix.Cms.Lib.ViewModels.MixPages.ReadViewModel> cates)
		{
			RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixPages.ReadViewModel>> repositoryResponse;
			MixCmsContext mixCmsContext = new MixCmsContext();
			IDbContextTransaction dbContextTransaction = mixCmsContext.get_Database().BeginTransaction();
			RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixPages.ReadViewModel>> repositoryResponse1 = new RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixPages.ReadViewModel>>();
			try
			{
				try
				{
					foreach (Mix.Cms.Lib.ViewModels.MixPages.ReadViewModel cate in cates)
					{
						cate.LastModified = new DateTime?(DateTime.UtcNow);
						RepositoryResponse<Mix.Cms.Lib.ViewModels.MixPages.ReadViewModel> repositoryResponse2 = await ((ViewModelBase<MixCmsContext, MixPage, Mix.Cms.Lib.ViewModels.MixPages.ReadViewModel>)cate).SaveModelAsync(false, mixCmsContext, dbContextTransaction);
						repositoryResponse1.set_IsSucceed(repositoryResponse2.get_IsSucceed());
						if (repositoryResponse1.get_IsSucceed())
						{
							continue;
						}
						repositoryResponse1.get_Errors().AddRange(repositoryResponse2.get_Errors());
						repositoryResponse1.set_Exception(repositoryResponse2.get_Exception());
						break;
					}
					UnitOfWorkHelper<MixCmsContext>.HandleTransaction(repositoryResponse1.get_IsSucceed(), true, dbContextTransaction);
					repositoryResponse = repositoryResponse1;
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					UnitOfWorkHelper<MixCmsContext>.HandleException<Mix.Cms.Lib.ViewModels.MixPages.ReadViewModel>(exception, true, dbContextTransaction);
					RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixPages.ReadViewModel>> repositoryResponse3 = new RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixPages.ReadViewModel>>();
					repositoryResponse3.set_IsSucceed(false);
					repositoryResponse3.set_Data(null);
					repositoryResponse3.set_Exception(exception);
					repositoryResponse = repositoryResponse3;
				}
			}
			finally
			{
				RelationalDatabaseFacadeExtensions.CloseConnection(mixCmsContext.get_Database());
				dbContextTransaction.Dispose();
				mixCmsContext.Dispose();
			}
			return repositoryResponse;
		}
	}
}