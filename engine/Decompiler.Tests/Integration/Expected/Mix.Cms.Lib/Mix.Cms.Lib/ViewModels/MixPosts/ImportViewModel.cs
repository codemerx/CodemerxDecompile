using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.Services;
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
		public JArray ListTag { get; set; } = new JArray();

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
		public string Tags { get; set; } = "[]";

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
		}

		public ImportViewModel(MixPost model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		private async Task<RepositoryResponse<bool>> SaveAttributeAsync(int parentId, MixCmsContext context, IDbContextTransaction transaction)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			this.AttributeData.ParentId = parentId.ToString();
			this.AttributeData.ParentType = MixEnums.MixAttributeSetDataType.Post;
			RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel> repositoryResponse2 = await this.AttributeData.Data.SaveModelAsync(true, context, transaction);
			ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel>(repositoryResponse2, ref repositoryResponse1);
			if (repositoryResponse1.get_IsSucceed())
			{
				this.AttributeData.DataId = repositoryResponse2.get_Data().Id;
				ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel>(await this.AttributeData.SaveModelAsync(true, context, transaction), ref repositoryResponse1);
			}
			foreach (Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.FormViewModel sysCategory in this.SysCategories)
			{
				if (!repositoryResponse1.get_IsSucceed())
				{
					continue;
				}
				sysCategory.ParentId = parentId.ToString();
				sysCategory.ParentType = MixEnums.MixAttributeSetDataType.Post;
				sysCategory.Specificulture = this.Specificulture;
				ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.FormViewModel>(await sysCategory.SaveModelAsync(false, context, transaction), ref repositoryResponse1);
			}
			foreach (Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.FormViewModel sysTag in this.SysTags)
			{
				if (!repositoryResponse1.get_IsSucceed())
				{
					continue;
				}
				sysTag.ParentId = parentId.ToString();
				sysTag.ParentType = MixEnums.MixAttributeSetDataType.Post;
				sysTag.Specificulture = this.Specificulture;
				ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.FormViewModel>(await sysTag.SaveModelAsync(false, context, transaction), ref repositoryResponse1);
			}
			return repositoryResponse1;
		}

		private async Task<RepositoryResponse<bool>> SaveMediasAsync(int id, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			foreach (Mix.Cms.Lib.ViewModels.MixPostMedias.ReadViewModel mediaNav in this.MediaNavs)
			{
				mediaNav.PostId = id;
				mediaNav.Specificulture = this.Specificulture;
				if (!mediaNav.IsActived)
				{
					continue;
				}
				ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixPostMedias.ReadViewModel>(await mediaNav.SaveModelAsync(false, _context, _transaction), ref repositoryResponse1);
			}
			return repositoryResponse1;
		}

		private async Task<RepositoryResponse<bool>> SaveParentModulesAsync(int id, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			foreach (Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel module in this.Modules)
			{
				module.Specificulture = this.Specificulture;
				module.PostId = id;
				module.Status = MixEnums.MixContentStatus.Published;
				if (!module.IsActived)
				{
					RepositoryResponse<MixModulePost> repositoryResponse2 = await module.RemoveModelAsync(false, _context, _transaction);
					repositoryResponse1.set_IsSucceed(repositoryResponse2.get_IsSucceed());
					if (repositoryResponse1.get_IsSucceed())
					{
						continue;
					}
					repositoryResponse1.set_Exception(repositoryResponse2.get_Exception());
					base.get_Errors().AddRange(repositoryResponse2.get_Errors());
				}
				else
				{
					RepositoryResponse<Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel> repositoryResponse3 = await module.SaveModelAsync(false, _context, _transaction);
					repositoryResponse1.set_IsSucceed(repositoryResponse3.get_IsSucceed());
					if (repositoryResponse1.get_IsSucceed())
					{
						continue;
					}
					repositoryResponse1.set_Exception(repositoryResponse3.get_Exception());
					base.get_Errors().AddRange(repositoryResponse3.get_Errors());
				}
			}
			return repositoryResponse1;
		}

		private async Task<RepositoryResponse<bool>> SaveParentPagesAsync(int id, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			foreach (Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel page in this.Pages)
			{
				page.Specificulture = this.Specificulture;
				page.PostId = id;
				page.Status = MixEnums.MixContentStatus.Published;
				if (!page.IsActived)
				{
					RepositoryResponse<MixPagePost> repositoryResponse2 = await page.RemoveModelAsync(false, _context, _transaction);
					repositoryResponse1.set_IsSucceed(repositoryResponse2.get_IsSucceed());
					if (repositoryResponse1.get_IsSucceed())
					{
						continue;
					}
					repositoryResponse1.set_Exception(repositoryResponse2.get_Exception());
					base.get_Errors().AddRange(repositoryResponse2.get_Errors());
				}
				else
				{
					RepositoryResponse<Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel> repositoryResponse3 = await page.SaveModelAsync(false, _context, _transaction);
					repositoryResponse1.set_IsSucceed(repositoryResponse3.get_IsSucceed());
					if (repositoryResponse1.get_IsSucceed())
					{
						continue;
					}
					repositoryResponse1.set_Exception(repositoryResponse3.get_Exception());
					base.get_Errors().AddRange(repositoryResponse3.get_Errors());
				}
			}
			return repositoryResponse1;
		}

		private async Task<RepositoryResponse<bool>> SaveRelatedPostAsync(int id, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			foreach (Mix.Cms.Lib.ViewModels.MixPostPosts.ReadViewModel postNav in this.PostNavs)
			{
				postNav.SourceId = id;
				postNav.Status = MixEnums.MixContentStatus.Published;
				postNav.Specificulture = this.Specificulture;
				if (!postNav.IsActived)
				{
					RepositoryResponse<MixRelatedPost> repositoryResponse2 = await postNav.RemoveModelAsync(false, _context, _transaction);
					repositoryResponse1.set_IsSucceed(repositoryResponse2.get_IsSucceed());
					if (repositoryResponse1.get_IsSucceed())
					{
						continue;
					}
					repositoryResponse1.set_Exception(repositoryResponse2.get_Exception());
					base.get_Errors().AddRange(repositoryResponse2.get_Errors());
				}
				else
				{
					RepositoryResponse<Mix.Cms.Lib.ViewModels.MixPostPosts.ReadViewModel> repositoryResponse3 = await postNav.SaveModelAsync(false, _context, _transaction);
					repositoryResponse1.set_IsSucceed(repositoryResponse3.get_IsSucceed());
					if (repositoryResponse1.get_IsSucceed())
					{
						continue;
					}
					repositoryResponse1.set_Exception(repositoryResponse3.get_Exception());
					base.get_Errors().AddRange(repositoryResponse3.get_Errors());
				}
			}
			return repositoryResponse1;
		}

		public override async Task<RepositoryResponse<bool>> SaveSubModelsAsync(MixPost parent, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<bool> repositoryResponse;
			RepositoryResponse<bool> repositoryResponse1 = new RepositoryResponse<bool>();
			repositoryResponse1.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse2 = repositoryResponse1;
			try
			{
				if (repositoryResponse2.get_IsSucceed())
				{
					repositoryResponse2 = await this.SaveUrlAliasAsync(parent.Id, _context, _transaction);
				}
				if (repositoryResponse2.get_IsSucceed() && this.MediaNavs != null)
				{
					repositoryResponse2 = await this.SaveMediasAsync(parent.Id, _context, _transaction);
				}
				if (repositoryResponse2.get_IsSucceed())
				{
					repositoryResponse2 = await this.SaveAttributeAsync(parent.Id, _context, _transaction);
				}
				if (repositoryResponse2.get_IsSucceed() && this.PostNavs != null)
				{
					repositoryResponse2 = await this.SaveRelatedPostAsync(parent.Id, _context, _transaction);
				}
				if (repositoryResponse2.get_IsSucceed() && this.Pages != null)
				{
					repositoryResponse2 = await this.SaveParentPagesAsync(parent.Id, _context, _transaction);
				}
				if (repositoryResponse2.get_IsSucceed() && this.Modules != null)
				{
					repositoryResponse2 = await this.SaveParentModulesAsync(parent.Id, _context, _transaction);
				}
				repositoryResponse = repositoryResponse2;
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				repositoryResponse2.set_IsSucceed(false);
				repositoryResponse2.set_Exception(exception);
				repositoryResponse = repositoryResponse2;
			}
			return repositoryResponse;
		}

		private async Task<RepositoryResponse<bool>> SaveUrlAliasAsync(int parentId, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			// 
			// Current member / type: System.Threading.Tasks.Task`1<Mix.Domain.Core.ViewModels.RepositoryResponse`1<System.Boolean>> Mix.Cms.Lib.ViewModels.MixPosts.ImportViewModel::SaveUrlAliasAsync(System.Int32,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: System.Threading.Tasks.Task<Mix.Domain.Core.ViewModels.RepositoryResponse<System.Boolean>> SaveUrlAliasAsync(System.Int32,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Invalid entry of new construct
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}
	}
}