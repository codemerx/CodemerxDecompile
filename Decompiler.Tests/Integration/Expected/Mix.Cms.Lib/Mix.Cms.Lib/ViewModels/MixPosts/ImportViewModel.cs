using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixAttributeSetDatas;
using Mix.Cms.Lib.ViewModels.MixAttributeSets;
using Mix.Cms.Lib.ViewModels.MixModulePosts;
using Mix.Cms.Lib.ViewModels.MixPagePosts;
using Mix.Cms.Lib.ViewModels.MixPostMedias;
using Mix.Cms.Lib.ViewModels.MixPostPosts;
using Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas;
using Mix.Cms.Lib.ViewModels.MixUrlAliases;
using Mix.Domain.Core.Models;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixPosts
{
	public class ImportViewModel : ViewModelBase<MixCmsContext, MixPost, Mix.Cms.Lib.ViewModels.MixPosts.ImportViewModel>
	{
		[JsonProperty("attributeData")]
		public Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel AttributeData
		{
			get;
			set;
		}

		[JsonProperty("attributes")]
		public Mix.Cms.Lib.ViewModels.MixAttributeSets.ImportViewModel Attributes
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

		[JsonProperty("cultures")]
		public List<SupportedCulture> Cultures
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

		[JsonProperty("lastModified")]
		public DateTime? LastModified
		{
			get;
			set;
		}

		[JsonProperty("listTag")]
		public JArray ListTag
		{
			get;
			set;
		}

		[JsonProperty("mediaNavs")]
		public List<Mix.Cms.Lib.ViewModels.MixPostMedias.ReadViewModel> MediaNavs
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

		[JsonProperty("modules")]
		public List<Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel> Modules
		{
			get;
			set;
		}

		[JsonProperty("categories")]
		public List<Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel> Pages
		{
			get;
			set;
		}

		[JsonProperty("postNavs")]
		public List<Mix.Cms.Lib.ViewModels.MixPostPosts.ReadViewModel> PostNavs
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

		[JsonProperty("publishedDateTime")]
		public DateTime? PublishedDateTime
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

		[JsonProperty("status")]
		public MixEnums.MixContentStatus Status
		{
			get;
			set;
		}

		[JsonProperty("sysCategories")]
		public List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.FormViewModel> SysCategories
		{
			get;
			set;
		}

		[JsonProperty("sysTags")]
		public List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.FormViewModel> SysTags
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

		[JsonProperty("title")]
		[Required]
		public string Title
		{
			get;
			set;
		}

		[JsonProperty("type")]
		public MixEnums.MixContentStatus Type
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

		[JsonProperty("views")]
		public int? Views
		{
			get;
			set;
		}

		public ImportViewModel()
		{
			this.u003cTagsu003ek__BackingField = "[]";
			this.u003cListTagu003ek__BackingField = new JArray();
			base();
			return;
		}

		public ImportViewModel(MixPost model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.u003cTagsu003ek__BackingField = "[]";
			this.u003cListTagu003ek__BackingField = new JArray();
			base(model, _context, _transaction);
			return;
		}

		private async Task<RepositoryResponse<bool>> SaveAttributeAsync(int parentId, MixCmsContext context, IDbContextTransaction transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.parentId = parentId;
			V_0.context = context;
			V_0.transaction = transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixPosts.ImportViewModel.u003cSaveAttributeAsyncu003ed__145>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private async Task<RepositoryResponse<bool>> SaveMediasAsync(int id, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixPosts.ImportViewModel.u003cSaveMediasAsyncu003ed__149>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private async Task<RepositoryResponse<bool>> SaveParentModulesAsync(int id, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixPosts.ImportViewModel.u003cSaveParentModulesAsyncu003ed__146>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private async Task<RepositoryResponse<bool>> SaveParentPagesAsync(int id, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixPosts.ImportViewModel.u003cSaveParentPagesAsyncu003ed__147>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private async Task<RepositoryResponse<bool>> SaveRelatedPostAsync(int id, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixPosts.ImportViewModel.u003cSaveRelatedPostAsyncu003ed__148>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public override async Task<RepositoryResponse<bool>> SaveSubModelsAsync(MixPost parent, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0.u003cu003e4__this = this;
			V_0.parent = parent;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixPosts.ImportViewModel.u003cSaveSubModelsAsyncu003ed__144>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private async Task<RepositoryResponse<bool>> SaveUrlAliasAsync(int parentId, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.parentId = parentId;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixPosts.ImportViewModel.u003cSaveUrlAliasAsyncu003ed__150>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}