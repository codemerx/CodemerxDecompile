using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.Services;
using Mix.Cms.Lib.ViewModels;
using Mix.Common.Helper;
using Mix.Domain.Core.Models;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
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
using System.Reflection;
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
				return string.Format("/post/{0}/{1}/{2}", this.Specificulture, this.Id, this.SeoName);
			}
		}

		[JsonProperty("domain")]
		public string Domain
		{
			get
			{
				return MixService.GetConfig<string>("Domain", this.Specificulture);
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
		public string ExtraFields { get; set; } = "[]";

		[JsonIgnore]
		[JsonProperty("extraProperties")]
		public string ExtraProperties { get; set; } = "[]";

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
		}

		public ReadViewModel(MixPost model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.Properties = new List<ExtraProperty>();
			if (!string.IsNullOrEmpty(this.ExtraProperties))
			{
				foreach (JToken jToken in JArray.Parse(this.ExtraProperties))
				{
					this.Properties.Add(jToken.ToObject<ExtraProperty>());
				}
			}
		}

		public static RepositoryResponse<PaginationModel<ReadViewModel>> GetModelListByCategory(int pageId, string specificulture, string orderByPropertyName, MixHeartEnums.DisplayDirection direction, int? pageSize = 1, int? pageIndex = 0, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			ReadViewModel.u003cu003ec__DisplayClass124_0 variable = null;
			RepositoryResponse<PaginationModel<ReadViewModel>> repositoryResponse;
			MixCmsContext mixCmsContext = _context ?? new MixCmsContext();
			IDbContextTransaction dbContextTransaction = _transaction ?? mixCmsContext.get_Database().BeginTransaction();
			try
			{
				try
				{
					DbSet<MixPagePost> mixPagePost = mixCmsContext.MixPagePost;
					ParameterExpression parameterExpression = Expression.Parameter(typeof(MixPagePost), "ac");
					IIncludableQueryable<MixPagePost, MixPost> includableQueryable = EntityFrameworkQueryableExtensions.Include<MixPagePost, MixPost>(mixPagePost, Expression.Lambda<Func<MixPagePost, MixPost>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPagePost).GetMethod("get_MixPost").MethodHandle)), new ParameterExpression[] { parameterExpression }));
					parameterExpression = Expression.Parameter(typeof(MixPagePost), "ac");
					IQueryable<MixPagePost> mixPagePosts = includableQueryable.Where<MixPagePost>(Expression.Lambda<Func<MixPagePost, bool>>(Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPagePost).GetMethod("get_PageId").MethodHandle)), Expression.Field(Expression.Constant(variable, typeof(ReadViewModel.u003cu003ec__DisplayClass124_0)), FieldInfo.GetFieldFromHandle(typeof(ReadViewModel.u003cu003ec__DisplayClass124_0).GetField("pageId").FieldHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPagePost).GetMethod("get_Specificulture").MethodHandle)), Expression.Field(Expression.Constant(variable, typeof(ReadViewModel.u003cu003ec__DisplayClass124_0)), FieldInfo.GetFieldFromHandle(typeof(ReadViewModel.u003cu003ec__DisplayClass124_0).GetField("specificulture").FieldHandle)))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPagePost).GetMethod("get_Status").MethodHandle)), Expression.Call(Expression.Constant(MixEnums.MixContentStatus.Published, typeof(MixEnums.MixContentStatus)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(object).GetMethod("ToString").MethodHandle), Array.Empty<Expression>()))), new ParameterExpression[] { parameterExpression }));
					parameterExpression = Expression.Parameter(typeof(MixPagePost), "ac");
					IQueryable<MixPost> mixPosts = mixPagePosts.Select<MixPagePost, MixPost>(Expression.Lambda<Func<MixPagePost, MixPost>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPagePost).GetMethod("get_MixPost").MethodHandle)), new ParameterExpression[] { parameterExpression }));
					PaginationModel<ReadViewModel> paginationModel = ViewModelBase<MixCmsContext, MixPost, ReadViewModel>.Repository.ParsePagingQuery(mixPosts, orderByPropertyName, direction, pageSize, pageIndex, mixCmsContext, dbContextTransaction);
					RepositoryResponse<PaginationModel<ReadViewModel>> repositoryResponse1 = new RepositoryResponse<PaginationModel<ReadViewModel>>();
					repositoryResponse1.set_IsSucceed(true);
					repositoryResponse1.set_Data(paginationModel);
					repositoryResponse = repositoryResponse1;
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					ViewModelBase<MixCmsContext, MixPost, ReadViewModel>.Repository.LogErrorMessage(exception);
					if (_transaction == null)
					{
						dbContextTransaction.Rollback();
					}
					RepositoryResponse<PaginationModel<ReadViewModel>> repositoryResponse2 = new RepositoryResponse<PaginationModel<ReadViewModel>>();
					repositoryResponse2.set_IsSucceed(false);
					repositoryResponse2.set_Data(null);
					repositoryResponse2.set_Exception(exception);
					repositoryResponse = repositoryResponse2;
				}
			}
			finally
			{
				if (_context == null)
				{
					RelationalDatabaseFacadeExtensions.CloseConnection(mixCmsContext.get_Database());
					dbContextTransaction.Dispose();
					mixCmsContext.Dispose();
				}
			}
			return repositoryResponse;
		}

		public static async Task<RepositoryResponse<PaginationModel<ReadViewModel>>> GetModelListByCategoryAsync(int pageId, string specificulture, string orderByPropertyName, MixHeartEnums.DisplayDirection direction, int? pageSize = 1, int? pageIndex = 0, int? skip = null, int? top = null, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			ReadViewModel.u003cGetModelListByCategoryAsyncu003ed__123 variable = new ReadViewModel.u003cGetModelListByCategoryAsyncu003ed__123();
			variable.pageId = pageId;
			variable.specificulture = specificulture;
			variable.orderByPropertyName = orderByPropertyName;
			variable.direction = direction;
			variable.pageSize = pageSize;
			variable.pageIndex = pageIndex;
			variable.skip = skip;
			variable.top = top;
			variable._context = _context;
			variable._transaction = _transaction;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<PaginationModel<ReadViewModel>>>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<ReadViewModel.u003cGetModelListByCategoryAsyncu003ed__123>(ref variable);
			return variable.u003cu003et__builder.Task;
		}

		public static RepositoryResponse<PaginationModel<ReadViewModel>> GetModelListByModule(int ModuleId, string specificulture, string orderByPropertyName, MixHeartEnums.DisplayDirection direction, int? pageSize = 1, int? pageIndex = 0, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			ReadViewModel.u003cu003ec__DisplayClass125_0 variable = null;
			RepositoryResponse<PaginationModel<ReadViewModel>> repositoryResponse;
			MixCmsContext mixCmsContext = _context ?? new MixCmsContext();
			IDbContextTransaction dbContextTransaction = _transaction ?? mixCmsContext.get_Database().BeginTransaction();
			try
			{
				try
				{
					DbSet<MixModulePost> mixModulePost = mixCmsContext.MixModulePost;
					ParameterExpression parameterExpression = Expression.Parameter(typeof(MixModulePost), "ac");
					IIncludableQueryable<MixModulePost, MixPost> includableQueryable = EntityFrameworkQueryableExtensions.Include<MixModulePost, MixPost>(mixModulePost, Expression.Lambda<Func<MixModulePost, MixPost>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixModulePost).GetMethod("get_MixPost").MethodHandle)), new ParameterExpression[] { parameterExpression }));
					parameterExpression = Expression.Parameter(typeof(MixModulePost), "ac");
					IQueryable<MixModulePost> mixModulePosts = includableQueryable.Where<MixModulePost>(Expression.Lambda<Func<MixModulePost, bool>>(Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixModulePost).GetMethod("get_ModuleId").MethodHandle)), Expression.Field(Expression.Constant(variable, typeof(ReadViewModel.u003cu003ec__DisplayClass125_0)), FieldInfo.GetFieldFromHandle(typeof(ReadViewModel.u003cu003ec__DisplayClass125_0).GetField("ModuleId").FieldHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixModulePost).GetMethod("get_Specificulture").MethodHandle)), Expression.Field(Expression.Constant(variable, typeof(ReadViewModel.u003cu003ec__DisplayClass125_0)), FieldInfo.GetFieldFromHandle(typeof(ReadViewModel.u003cu003ec__DisplayClass125_0).GetField("specificulture").FieldHandle)))), Expression.OrElse(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixModulePost).GetMethod("get_Status").MethodHandle)), Expression.Call(Expression.Constant(MixEnums.MixContentStatus.Published, typeof(MixEnums.MixContentStatus)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(object).GetMethod("ToString").MethodHandle), Array.Empty<Expression>())), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixModulePost).GetMethod("get_Status").MethodHandle)), Expression.Call(Expression.Constant(MixEnums.MixContentStatus.Preview, typeof(MixEnums.MixContentStatus)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(object).GetMethod("ToString").MethodHandle), Array.Empty<Expression>())))), new ParameterExpression[] { parameterExpression }));
					parameterExpression = Expression.Parameter(typeof(MixModulePost), "ac");
					IQueryable<MixPost> mixPosts = mixModulePosts.Select<MixModulePost, MixPost>(Expression.Lambda<Func<MixModulePost, MixPost>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixModulePost).GetMethod("get_MixPost").MethodHandle)), new ParameterExpression[] { parameterExpression }));
					PaginationModel<ReadViewModel> paginationModel = ViewModelBase<MixCmsContext, MixPost, ReadViewModel>.Repository.ParsePagingQuery(mixPosts, orderByPropertyName, direction, pageSize, pageIndex, mixCmsContext, dbContextTransaction);
					RepositoryResponse<PaginationModel<ReadViewModel>> repositoryResponse1 = new RepositoryResponse<PaginationModel<ReadViewModel>>();
					repositoryResponse1.set_IsSucceed(true);
					repositoryResponse1.set_Data(paginationModel);
					repositoryResponse = repositoryResponse1;
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					ViewModelBase<MixCmsContext, MixPost, ReadViewModel>.Repository.LogErrorMessage(exception);
					if (_transaction == null)
					{
						dbContextTransaction.Rollback();
					}
					RepositoryResponse<PaginationModel<ReadViewModel>> repositoryResponse2 = new RepositoryResponse<PaginationModel<ReadViewModel>>();
					repositoryResponse2.set_IsSucceed(false);
					repositoryResponse2.set_Data(null);
					repositoryResponse2.set_Exception(exception);
					repositoryResponse = repositoryResponse2;
				}
			}
			finally
			{
				if (_context == null)
				{
					RelationalDatabaseFacadeExtensions.CloseConnection(mixCmsContext.get_Database());
					dbContextTransaction.Dispose();
					mixCmsContext.Dispose();
				}
			}
			return repositoryResponse;
		}

		public string Property(string name)
		{
			ExtraProperty extraProperty = this.Properties.FirstOrDefault<ExtraProperty>((ExtraProperty p) => p.Name.ToLower() == name.ToLower());
			if (extraProperty != null)
			{
				return extraProperty.Value;
			}
			return null;
		}
	}
}