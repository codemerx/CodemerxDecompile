using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
		}

		public DeleteViewModel(MixPost model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override RepositoryResponse<bool> RemoveRelatedModels(DeleteViewModel view, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			ParameterExpression parameterExpression;
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			if (repositoryResponse1.get_IsSucceed())
			{
				DbSet<MixPagePost> mixPagePost = _context.MixPagePost;
				parameterExpression = Expression.Parameter(typeof(MixPagePost), "n");
				foreach (MixPagePost list in mixPagePost.Where<MixPagePost>(Expression.Lambda<Func<MixPagePost, bool>>(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPagePost).GetMethod("get_PostId").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(DeleteViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(DeleteViewModel).GetMethod("get_Id").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPagePost).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(DeleteViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(DeleteViewModel).GetMethod("get_Specificulture").MethodHandle)))), new ParameterExpression[] { parameterExpression })).ToList<MixPagePost>())
				{
					_context.Entry<MixPagePost>(list).set_State(2);
				}
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				DbSet<MixModulePost> mixModulePost = _context.MixModulePost;
				parameterExpression = Expression.Parameter(typeof(MixModulePost), "n");
				foreach (MixModulePost list1 in mixModulePost.Where<MixModulePost>(Expression.Lambda<Func<MixModulePost, bool>>(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixModulePost).GetMethod("get_PostId").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(DeleteViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(DeleteViewModel).GetMethod("get_Id").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixModulePost).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(DeleteViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(DeleteViewModel).GetMethod("get_Specificulture").MethodHandle)))), new ParameterExpression[] { parameterExpression })).ToList<MixModulePost>())
				{
					_context.Entry<MixModulePost>(list1).set_State(2);
				}
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				DbSet<MixPostMedia> mixPostMedia = _context.MixPostMedia;
				parameterExpression = Expression.Parameter(typeof(MixPostMedia), "n");
				foreach (MixPostMedia mixPostMedium in mixPostMedia.Where<MixPostMedia>(Expression.Lambda<Func<MixPostMedia, bool>>(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPostMedia).GetMethod("get_PostId").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(DeleteViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(DeleteViewModel).GetMethod("get_Id").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPostMedia).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(DeleteViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(DeleteViewModel).GetMethod("get_Specificulture").MethodHandle)))), new ParameterExpression[] { parameterExpression })).ToList<MixPostMedia>())
				{
					_context.Entry<MixPostMedia>(mixPostMedium).set_State(2);
				}
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				DbSet<MixPostModule> mixPostModule = _context.MixPostModule;
				parameterExpression = Expression.Parameter(typeof(MixPostModule), "n");
				foreach (MixPostModule mixPostModule1 in mixPostModule.Where<MixPostModule>(Expression.Lambda<Func<MixPostModule, bool>>(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPostModule).GetMethod("get_PostId").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(DeleteViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(DeleteViewModel).GetMethod("get_Id").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPostModule).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(DeleteViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(DeleteViewModel).GetMethod("get_Specificulture").MethodHandle)))), new ParameterExpression[] { parameterExpression })).ToList<MixPostModule>())
				{
					_context.Entry<MixPostModule>(mixPostModule1).set_State(2);
				}
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				DbSet<MixPostMedia> dbSet = _context.MixPostMedia;
				parameterExpression = Expression.Parameter(typeof(MixPostMedia), "n");
				foreach (MixPostMedia mixPostMedium1 in dbSet.Where<MixPostMedia>(Expression.Lambda<Func<MixPostMedia, bool>>(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPostMedia).GetMethod("get_PostId").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(DeleteViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(DeleteViewModel).GetMethod("get_Id").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPostMedia).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(DeleteViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(DeleteViewModel).GetMethod("get_Specificulture").MethodHandle)))), new ParameterExpression[] { parameterExpression })).ToList<MixPostMedia>())
				{
					_context.Entry<MixPostMedia>(mixPostMedium1).set_State(2);
				}
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				DbSet<MixUrlAlias> mixUrlAlias = _context.MixUrlAlias;
				parameterExpression = Expression.Parameter(typeof(MixUrlAlias), "n");
				foreach (MixUrlAlias mixUrlAlia in mixUrlAlias.Where<MixUrlAlias>(Expression.Lambda<Func<MixUrlAlias, bool>>(Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixUrlAlias).GetMethod("get_SourceId").MethodHandle)), Expression.Call(Expression.Property(Expression.Constant(this, typeof(DeleteViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(DeleteViewModel).GetMethod("get_Id").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(int).GetMethod("ToString").MethodHandle), Array.Empty<Expression>())), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixUrlAlias).GetMethod("get_Type").MethodHandle)), Expression.Constant(1, typeof(int)))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixUrlAlias).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(DeleteViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(DeleteViewModel).GetMethod("get_Specificulture").MethodHandle)))), new ParameterExpression[] { parameterExpression })).ToList<MixUrlAlias>())
				{
					_context.Entry<MixUrlAlias>(mixUrlAlia).set_State(2);
				}
			}
			_context.SaveChanges();
			return repositoryResponse1;
		}

		public override async Task<RepositoryResponse<bool>> RemoveRelatedModelsAsync(DeleteViewModel view, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			ParameterExpression parameterExpression;
			CancellationToken cancellationToken;
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			if (repositoryResponse1.get_IsSucceed())
			{
				DbSet<MixPagePost> mixPagePost = _context.MixPagePost;
				parameterExpression = Expression.Parameter(typeof(MixPagePost), "n");
				BinaryExpression binaryExpression = Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPagePost).GetMethod("get_PostId").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(DeleteViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(DeleteViewModel).GetMethod("get_Id").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPagePost).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(DeleteViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(DeleteViewModel).GetMethod("get_Specificulture").MethodHandle))));
				ParameterExpression[] parameterExpressionArray = new ParameterExpression[] { parameterExpression };
				IQueryable<MixPagePost> mixPagePosts = mixPagePost.Where<MixPagePost>(Expression.Lambda<Func<MixPagePost, bool>>(binaryExpression, parameterExpressionArray));
				cancellationToken = new CancellationToken();
				foreach (MixPagePost listAsync in await EntityFrameworkQueryableExtensions.ToListAsync<MixPagePost>(mixPagePosts, cancellationToken))
				{
					_context.Entry<MixPagePost>(listAsync).set_State(2);
				}
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				DbSet<MixModulePost> mixModulePost = _context.MixModulePost;
				parameterExpression = Expression.Parameter(typeof(MixModulePost), "n");
				BinaryExpression binaryExpression1 = Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixModulePost).GetMethod("get_PostId").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(DeleteViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(DeleteViewModel).GetMethod("get_Id").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixModulePost).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(DeleteViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(DeleteViewModel).GetMethod("get_Specificulture").MethodHandle))));
				ParameterExpression[] parameterExpressionArray1 = new ParameterExpression[] { parameterExpression };
				IQueryable<MixModulePost> mixModulePosts = mixModulePost.Where<MixModulePost>(Expression.Lambda<Func<MixModulePost, bool>>(binaryExpression1, parameterExpressionArray1));
				cancellationToken = new CancellationToken();
				foreach (MixModulePost listAsync1 in await EntityFrameworkQueryableExtensions.ToListAsync<MixModulePost>(mixModulePosts, cancellationToken))
				{
					_context.Entry<MixModulePost>(listAsync1).set_State(2);
				}
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				DbSet<MixPostMedia> mixPostMedia = _context.MixPostMedia;
				parameterExpression = Expression.Parameter(typeof(MixPostMedia), "n");
				BinaryExpression binaryExpression2 = Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPostMedia).GetMethod("get_PostId").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(DeleteViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(DeleteViewModel).GetMethod("get_Id").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPostMedia).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(DeleteViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(DeleteViewModel).GetMethod("get_Specificulture").MethodHandle))));
				ParameterExpression[] parameterExpressionArray2 = new ParameterExpression[] { parameterExpression };
				IQueryable<MixPostMedia> mixPostMedias = mixPostMedia.Where<MixPostMedia>(Expression.Lambda<Func<MixPostMedia, bool>>(binaryExpression2, parameterExpressionArray2));
				cancellationToken = new CancellationToken();
				foreach (MixPostMedia mixPostMedium in await EntityFrameworkQueryableExtensions.ToListAsync<MixPostMedia>(mixPostMedias, cancellationToken))
				{
					_context.Entry<MixPostMedia>(mixPostMedium).set_State(2);
				}
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				DbSet<MixPostModule> mixPostModule = _context.MixPostModule;
				parameterExpression = Expression.Parameter(typeof(MixPostModule), "n");
				BinaryExpression binaryExpression3 = Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPostModule).GetMethod("get_PostId").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(DeleteViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(DeleteViewModel).GetMethod("get_Id").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPostModule).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(DeleteViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(DeleteViewModel).GetMethod("get_Specificulture").MethodHandle))));
				ParameterExpression[] parameterExpressionArray3 = new ParameterExpression[] { parameterExpression };
				IQueryable<MixPostModule> mixPostModules = mixPostModule.Where<MixPostModule>(Expression.Lambda<Func<MixPostModule, bool>>(binaryExpression3, parameterExpressionArray3));
				cancellationToken = new CancellationToken();
				foreach (MixPostModule mixPostModule1 in await EntityFrameworkQueryableExtensions.ToListAsync<MixPostModule>(mixPostModules, cancellationToken))
				{
					_context.Entry<MixPostModule>(mixPostModule1).set_State(2);
				}
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				DbSet<MixPostMedia> dbSet = _context.MixPostMedia;
				parameterExpression = Expression.Parameter(typeof(MixPostMedia), "n");
				BinaryExpression binaryExpression4 = Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPostMedia).GetMethod("get_PostId").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(DeleteViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(DeleteViewModel).GetMethod("get_Id").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPostMedia).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(DeleteViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(DeleteViewModel).GetMethod("get_Specificulture").MethodHandle))));
				ParameterExpression[] parameterExpressionArray4 = new ParameterExpression[] { parameterExpression };
				IQueryable<MixPostMedia> mixPostMedias1 = dbSet.Where<MixPostMedia>(Expression.Lambda<Func<MixPostMedia, bool>>(binaryExpression4, parameterExpressionArray4));
				cancellationToken = new CancellationToken();
				foreach (MixPostMedia mixPostMedium1 in await EntityFrameworkQueryableExtensions.ToListAsync<MixPostMedia>(mixPostMedias1, cancellationToken))
				{
					_context.Entry<MixPostMedia>(mixPostMedium1).set_State(2);
				}
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				DbSet<MixUrlAlias> mixUrlAlias = _context.MixUrlAlias;
				parameterExpression = Expression.Parameter(typeof(MixUrlAlias), "n");
				BinaryExpression binaryExpression5 = Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixUrlAlias).GetMethod("get_SourceId").MethodHandle)), Expression.Call(Expression.Property(Expression.Constant(this, typeof(DeleteViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(DeleteViewModel).GetMethod("get_Id").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(int).GetMethod("ToString").MethodHandle), Array.Empty<Expression>())), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixUrlAlias).GetMethod("get_Type").MethodHandle)), Expression.Constant(1, typeof(int)))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixUrlAlias).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(DeleteViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(DeleteViewModel).GetMethod("get_Specificulture").MethodHandle))));
				ParameterExpression[] parameterExpressionArray5 = new ParameterExpression[] { parameterExpression };
				IQueryable<MixUrlAlias> mixUrlAliases = mixUrlAlias.Where<MixUrlAlias>(Expression.Lambda<Func<MixUrlAlias, bool>>(binaryExpression5, parameterExpressionArray5));
				cancellationToken = new CancellationToken();
				foreach (MixUrlAlias mixUrlAlia in await EntityFrameworkQueryableExtensions.ToListAsync<MixUrlAlias>(mixUrlAliases, cancellationToken))
				{
					_context.Entry<MixUrlAlias>(mixUrlAlia).set_State(2);
				}
			}
			MixCmsContext mixCmsContext = _context;
			cancellationToken = new CancellationToken();
			await ((DbContext)mixCmsContext).SaveChangesAsync(cancellationToken);
			return repositoryResponse1;
		}
	}
}