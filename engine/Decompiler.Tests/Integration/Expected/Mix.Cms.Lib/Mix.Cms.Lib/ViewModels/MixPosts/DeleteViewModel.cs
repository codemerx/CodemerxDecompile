using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Domain.Core.Models;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixPosts
{
	public class DeleteViewModel : ViewModelBase<MixCmsContext, MixPost, DeleteViewModel>
	{
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

		[JsonProperty("excerpt")]
		public string Excerpt
		{
			get;
			set;
		}

		[JsonIgnore]
		[JsonProperty("extraFields")]
		public string ExtraFields
		{
			get;
			set;
		}

		[JsonIgnore]
		[JsonProperty("extraProperties")]
		public string ExtraProperties
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
		public int Type
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
			this.u003cExtraFieldsu003ek__BackingField = "[]";
			this.u003cExtraPropertiesu003ek__BackingField = "[]";
			this.u003cTagsu003ek__BackingField = "[]";
			base();
			return;
		}

		public DeleteViewModel(MixPost model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.u003cExtraFieldsu003ek__BackingField = "[]";
			this.u003cExtraPropertiesu003ek__BackingField = "[]";
			this.u003cTagsu003ek__BackingField = "[]";
			base(model, _context, _transaction);
			return;
		}

		public override RepositoryResponse<bool> RemoveRelatedModels(DeleteViewModel view, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			stackVariable0 = new RepositoryResponse<bool>();
			stackVariable0.set_IsSucceed(true);
			V_0 = stackVariable0;
			if (V_0.get_IsSucceed())
			{
				stackVariable290 = _context.get_MixPagePost();
				V_1 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
				// Current member / type: Mix.Domain.Core.ViewModels.RepositoryResponse`1<System.Boolean> Mix.Cms.Lib.ViewModels.MixPosts.DeleteViewModel::RemoveRelatedModels(Mix.Cms.Lib.ViewModels.MixPosts.DeleteViewModel,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Exception in: Mix.Domain.Core.ViewModels.RepositoryResponse<System.Boolean> RemoveRelatedModels(Mix.Cms.Lib.ViewModels.MixPosts.DeleteViewModel,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Specified method is not supported.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com


		public override async Task<RepositoryResponse<bool>> RemoveRelatedModelsAsync(DeleteViewModel view, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0.u003cu003e4__this = this;
			V_0._context = _context;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<DeleteViewModel.u003cRemoveRelatedModelsAsyncu003ed__110>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}