using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.Services;
using Mix.Cms.Lib.ViewModels.MixAttributeSets;
using Mix.Cms.Lib.ViewModels.MixPageModules;
using Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas;
using Mix.Cms.Lib.ViewModels.MixTemplates;
using Mix.Cms.Lib.ViewModels.MixUrlAliases;
using Mix.Common.Helper;
using Mix.Domain.Core.Models;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixPages
{
	public class DeleteViewModel : ViewModelBase<MixCmsContext, MixPage, Mix.Cms.Lib.ViewModels.MixPages.DeleteViewModel>
	{
		[JsonIgnore]
		public int ActivedTheme
		{
			get
			{
				return MixService.GetConfig<int>("ThemeId", this.Specificulture);
			}
		}

		[JsonProperty("attributeData")]
		public Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel AttributeData
		{
			get;
			set;
		}

		[JsonProperty("attributes")]
		public Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel Attributes
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

		[JsonProperty("imageFileStream")]
		public FileStreamViewModel ImageFileStream
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

		[JsonProperty("listTag")]
		public JArray ListTag { get; set; } = new JArray();

		[JsonProperty("master")]
		public Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel Master
		{
			get;
			set;
		}

		[JsonProperty("masters")]
		public List<Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel> Masters
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

		[JsonProperty("moduleNavs")]
		public List<Mix.Cms.Lib.ViewModels.MixPageModules.ReadMvcViewModel> ModuleNavs
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
		public MixEnums.PageStatus Status
		{
			get;
			set;
		}

		[JsonProperty("sysCategories")]
		public List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel> SysCategories
		{
			get;
			set;
		}

		[JsonProperty("sysTags")]
		public List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel> SysTags
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

		[JsonProperty("templateFolder")]
		public string TemplateFolder
		{
			get
			{
				return CommonHelper.GetFullPath(new string[] { "Views/Shared/Templates", MixService.GetConfig<string>("ThemeName", this.Specificulture), this.TemplateFolderType });
			}
		}

		[JsonIgnore]
		public string TemplateFolderType
		{
			get
			{
				return MixEnums.EnumTemplateFolder.Pages.ToString();
			}
		}

		[JsonProperty("templates")]
		public List<Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel> Templates
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

		[JsonProperty("type")]
		public MixEnums.MixPageType Type
		{
			get;
			set;
		}

		[JsonProperty("updatedDateTime")]
		public DateTime? UpdatedDateTime
		{
			get;
			set;
		}

		[JsonProperty("urlAliases")]
		public List<Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel> UrlAliases
		{
			get;
			set;
		}

		[JsonProperty("view")]
		public Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel View
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

		public DeleteViewModel()
		{
		}

		public DeleteViewModel(MixPage model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override async Task<RepositoryResponse<bool>> RemoveRelatedModelsAsync(Mix.Cms.Lib.ViewModels.MixPages.DeleteViewModel view, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			DbSet<MixPagePost> mixPagePost = _context.MixPagePost;
			IQueryable<MixPagePost> pageId = 
				from m in mixPagePost
				where m.PageId == this.Id && m.Specificulture == this.Specificulture
				select m;
			Action<MixPagePost> action = (MixPagePost m) => _context.Entry<MixPagePost>(m).set_State(2);
			CancellationToken cancellationToken = new CancellationToken();
			await EntityFrameworkQueryableExtensions.ForEachAsync<MixPagePost>(pageId, action, cancellationToken);
			DbSet<MixPageModule> mixPageModule = _context.MixPageModule;
			IQueryable<MixPageModule> mixPageModules = 
				from m in mixPageModule
				where m.PageId == this.Id && m.Specificulture == this.Specificulture
				select m;
			Action<MixPageModule> action1 = (MixPageModule m) => _context.Entry<MixPageModule>(m).set_State(2);
			cancellationToken = new CancellationToken();
			await EntityFrameworkQueryableExtensions.ForEachAsync<MixPageModule>(mixPageModules, action1, cancellationToken);
			MixCmsContext mixCmsContext = _context;
			cancellationToken = new CancellationToken();
			await ((DbContext)mixCmsContext).SaveChangesAsync(cancellationToken);
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			return repositoryResponse;
		}
	}
}