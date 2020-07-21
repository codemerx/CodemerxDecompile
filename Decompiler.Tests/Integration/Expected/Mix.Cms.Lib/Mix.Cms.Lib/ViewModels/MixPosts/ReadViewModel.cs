using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels;
using Mix.Domain.Core.Models;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.ViewModels;
using Mix.Heart.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixPosts
{
	public class ReadViewModel : ViewModelBase<MixCmsContext, MixPost, ReadViewModel>
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

		[JsonProperty("detailsUrl")]
		public string DetailsUrl
		{
			get
			{
				return string.Format("/post/{0}/{1}/{2}", this.get_Specificulture(), this.get_Id(), this.get_SeoName());
			}
		}

		[JsonProperty("domain")]
		public string Domain
		{
			get
			{
				return MixService.GetConfig<string>("Domain", this.get_Specificulture());
			}
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

		[JsonProperty("imageUrl")]
		public string ImageUrl
		{
			get
			{
				if (string.IsNullOrEmpty(this.get_Image()) || this.get_Image().IndexOf("http") != -1 || this.get_Image().get_Chars(0) == '/')
				{
					return this.get_Image();
				}
				stackVariable16 = new string[2];
				stackVariable16[0] = this.get_Domain();
				stackVariable16[1] = this.get_Image();
				return CommonHelper.GetFullPath(stackVariable16);
			}
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

		[JsonProperty("properties")]
		public List<ExtraProperty> Properties
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

		[JsonProperty("thumbnailUrl")]
		public string ThumbnailUrl
		{
			get
			{
				if (this.get_Thumbnail() == null || this.get_Thumbnail().IndexOf("http") != -1 || this.get_Thumbnail().get_Chars(0) == '/')
				{
					if (!string.IsNullOrEmpty(this.get_Thumbnail()))
					{
						return this.get_Thumbnail();
					}
					return this.get_ImageUrl();
				}
				stackVariable20 = new string[2];
				stackVariable20[0] = this.get_Domain();
				stackVariable20[1] = this.get_Thumbnail();
				return CommonHelper.GetFullPath(stackVariable20);
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
		public MixEnums.MixContentStatus Type
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
			this.u003cExtraFieldsu003ek__BackingField = "[]";
			this.u003cExtraPropertiesu003ek__BackingField = "[]";
			this.u003cTagsu003ek__BackingField = "[]";
			base();
			return;
		}

		public ReadViewModel(MixPost model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.u003cExtraFieldsu003ek__BackingField = "[]";
			this.u003cExtraPropertiesu003ek__BackingField = "[]";
			this.u003cTagsu003ek__BackingField = "[]";
			base(model, _context, _transaction);
			return;
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.set_Properties(new List<ExtraProperty>());
			if (!string.IsNullOrEmpty(this.get_ExtraProperties()))
			{
				V_0 = JArray.Parse(this.get_ExtraProperties()).GetEnumerator();
				try
				{
					while (V_0.MoveNext())
					{
						V_1 = V_0.get_Current();
						this.get_Properties().Add(V_1.ToObject<ExtraProperty>());
					}
				}
				finally
				{
					if (V_0 != null)
					{
						V_0.Dispose();
					}
				}
			}
			return;
		}

		public static RepositoryResponse<PaginationModel<ReadViewModel>> GetModelListByCategory(int pageId, string specificulture, string orderByPropertyName, MixHeartEnums.DisplayDirection direction, int? pageSize = 1, int? pageIndex = 0, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0 = new ReadViewModel.u003cu003ec__DisplayClass124_0();
			V_0.pageId = pageId;
			V_0.specificulture = specificulture;
			stackVariable5 = _context;
			if (stackVariable5 == null)
			{
				dummyVar0 = stackVariable5;
				stackVariable5 = new MixCmsContext();
			}
			V_1 = stackVariable5;
			stackVariable6 = _transaction;
			if (stackVariable6 == null)
			{
				dummyVar1 = stackVariable6;
				stackVariable6 = V_1.get_Database().BeginTransaction();
			}
			V_2 = stackVariable6;
			try
			{
				try
				{
					stackVariable8 = V_1.get_MixPagePost();
					V_5 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
					// Current member / type: Mix.Domain.Core.ViewModels.RepositoryResponse`1<Mix.Domain.Core.ViewModels.PaginationModel`1<Mix.Cms.Lib.ViewModels.MixPosts.ReadViewModel>> Mix.Cms.Lib.ViewModels.MixPosts.ReadViewModel::GetModelListByCategory(System.Int32,System.String,System.String,Mix.Heart.Enums.MixHeartEnums/DisplayDirection,System.Nullable`1<System.Int32>,System.Nullable`1<System.Int32>,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
					// Exception in: Mix.Domain.Core.ViewModels.RepositoryResponse<Mix.Domain.Core.ViewModels.PaginationModel<Mix.Cms.Lib.ViewModels.MixPosts.ReadViewModel>> GetModelListByCategory(System.Int32,System.String,System.String,Mix.Heart.Enums.MixHeartEnums/DisplayDirection,System.Nullable<System.Int32>,System.Nullable<System.Int32>,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
					// Specified method is not supported.
					// 
					// mailto: JustDecompilePublicFeedback@telerik.com


		public static async Task<RepositoryResponse<PaginationModel<ReadViewModel>>> GetModelListByCategoryAsync(int pageId, string specificulture, string orderByPropertyName, MixHeartEnums.DisplayDirection direction, int? pageSize = 1, int? pageIndex = 0, int? skip = null, int? top = null, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0.pageId = pageId;
			V_0.specificulture = specificulture;
			V_0.orderByPropertyName = orderByPropertyName;
			V_0.direction = direction;
			V_0.pageSize = pageSize;
			V_0.pageIndex = pageIndex;
			V_0.skip = skip;
			V_0.top = top;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<PaginationModel<ReadViewModel>>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ReadViewModel.u003cGetModelListByCategoryAsyncu003ed__123>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public static RepositoryResponse<PaginationModel<ReadViewModel>> GetModelListByModule(int ModuleId, string specificulture, string orderByPropertyName, MixHeartEnums.DisplayDirection direction, int? pageSize = 1, int? pageIndex = 0, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0 = new ReadViewModel.u003cu003ec__DisplayClass125_0();
			V_0.ModuleId = ModuleId;
			V_0.specificulture = specificulture;
			stackVariable5 = _context;
			if (stackVariable5 == null)
			{
				dummyVar0 = stackVariable5;
				stackVariable5 = new MixCmsContext();
			}
			V_1 = stackVariable5;
			stackVariable6 = _transaction;
			if (stackVariable6 == null)
			{
				dummyVar1 = stackVariable6;
				stackVariable6 = V_1.get_Database().BeginTransaction();
			}
			V_2 = stackVariable6;
			try
			{
				try
				{
					stackVariable8 = V_1.get_MixModulePost();
					V_5 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
					// Current member / type: Mix.Domain.Core.ViewModels.RepositoryResponse`1<Mix.Domain.Core.ViewModels.PaginationModel`1<Mix.Cms.Lib.ViewModels.MixPosts.ReadViewModel>> Mix.Cms.Lib.ViewModels.MixPosts.ReadViewModel::GetModelListByModule(System.Int32,System.String,System.String,Mix.Heart.Enums.MixHeartEnums/DisplayDirection,System.Nullable`1<System.Int32>,System.Nullable`1<System.Int32>,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
					// Exception in: Mix.Domain.Core.ViewModels.RepositoryResponse<Mix.Domain.Core.ViewModels.PaginationModel<Mix.Cms.Lib.ViewModels.MixPosts.ReadViewModel>> GetModelListByModule(System.Int32,System.String,System.String,Mix.Heart.Enums.MixHeartEnums/DisplayDirection,System.Nullable<System.Int32>,System.Nullable<System.Int32>,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
					// Specified method is not supported.
					// 
					// mailto: JustDecompilePublicFeedback@telerik.com


		public string Property(string name)
		{
			V_0 = new ReadViewModel.u003cu003ec__DisplayClass122_0();
			V_0.name = name;
			stackVariable8 = Enumerable.FirstOrDefault<ExtraProperty>(this.get_Properties(), new Func<ExtraProperty, bool>(V_0, ReadViewModel.u003cu003ec__DisplayClass122_0.u003cPropertyu003eb__0));
			if (stackVariable8 != null)
			{
				return stackVariable8.get_Value();
			}
			dummyVar0 = stackVariable8;
			return null;
		}
	}
}