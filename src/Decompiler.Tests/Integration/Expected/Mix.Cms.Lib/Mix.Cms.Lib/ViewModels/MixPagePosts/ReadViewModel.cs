using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixPages;
using Mix.Cms.Lib.ViewModels.MixPosts;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels.MixPagePosts
{
	public class ReadViewModel : ViewModelBase<MixCmsContext, MixPagePost, Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel>
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

		[JsonProperty("modifiedBy")]
		public string ModifiedBy
		{
			get;
			set;
		}

		[JsonProperty("page")]
		public Mix.Cms.Lib.ViewModels.MixPages.ReadViewModel Page
		{
			get;
			set;
		}

		[JsonProperty("pageId")]
		public int PageId
		{
			get;
			set;
		}

		[JsonProperty("post")]
		public Mix.Cms.Lib.ViewModels.MixPosts.ReadListItemViewModel Post
		{
			get;
			set;
		}

		[JsonProperty("postId")]
		public int PostId
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

		public ReadViewModel(MixPagePost model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public ReadViewModel()
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<Mix.Cms.Lib.ViewModels.MixPosts.ReadListItemViewModel> singleModel = ViewModelBase<MixCmsContext, MixPost, Mix.Cms.Lib.ViewModels.MixPosts.ReadListItemViewModel>.Repository.GetSingleModel((MixPost p) => p.Id == this.PostId && p.Specificulture == this.Specificulture, _context, _transaction);
			RepositoryResponse<Mix.Cms.Lib.ViewModels.MixPages.ReadViewModel> repositoryResponse = ViewModelBase<MixCmsContext, MixPage, Mix.Cms.Lib.ViewModels.MixPages.ReadViewModel>.Repository.GetSingleModel((MixPage p) => p.Id == this.PageId && p.Specificulture == this.Specificulture, _context, _transaction);
			if (singleModel.get_IsSucceed())
			{
				this.Post = singleModel.get_Data();
			}
			if (repositoryResponse.get_IsSucceed())
			{
				this.Page = repositoryResponse.get_Data();
			}
		}

		public static RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel>> GetPagePostNavAsync(int postId, string specificulture, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel.u003cu003ec__DisplayClass64_0 variable = null;
			RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel>> repositoryResponse;
			Func<MixPagePost, bool> func1 = null;
			MixCmsContext mixCmsContext = _context ?? new MixCmsContext();
			IDbContextTransaction dbContextTransaction = _transaction ?? mixCmsContext.get_Database().BeginTransaction();
			try
			{
				try
				{
					DbSet<MixPage> mixPage = mixCmsContext.MixPage;
					ParameterExpression parameterExpression = Expression.Parameter(typeof(MixPage), "cp");
					IIncludableQueryable<MixPage, ICollection<MixPagePost>> includableQueryable = EntityFrameworkQueryableExtensions.Include<MixPage, ICollection<MixPagePost>>(mixPage, Expression.Lambda<Func<MixPage, ICollection<MixPagePost>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPage).GetMethod("get_MixPagePost").MethodHandle)), new ParameterExpression[] { parameterExpression }));
					parameterExpression = Expression.Parameter(typeof(MixPage), "a");
					IEnumerable<Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel> readViewModels = includableQueryable.Where<MixPage>(Expression.Lambda<Func<MixPage, bool>>(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPage).GetMethod("get_Specificulture").MethodHandle)), Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel.u003cu003ec__DisplayClass64_0)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel.u003cu003ec__DisplayClass64_0).GetField("specificulture").FieldHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPage).GetMethod("get_Type").MethodHandle)), Expression.Call(Expression.Constant(MixEnums.MixPageType.ListPost, typeof(MixEnums.MixPageType)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(object).GetMethod("ToString").MethodHandle), Array.Empty<Expression>()))), new ParameterExpression[] { parameterExpression })).AsEnumerable<MixPage>().Select<MixPage, Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel>((MixPage p) => {
						Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel readViewModel = new Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel(new MixPagePost()
						{
							PostId = postId,
							PageId = p.Id,
							Specificulture = specificulture
						}, _context, _transaction);
						ICollection<MixPagePost> mixPagePost = p.MixPagePost;
						Func<MixPagePost, bool> u003cu003e9_3 = func1;
						if (u003cu003e9_3 == null)
						{
							Func<MixPagePost, bool> func2 = (MixPagePost cp) => {
								if (cp.PostId != postId)
								{
									return false;
								}
								return cp.Specificulture == specificulture;
							};
							Func<MixPagePost, bool> func = func2;
							func1 = func2;
							u003cu003e9_3 = func;
						}
						readViewModel.IsActived = mixPagePost.Any<MixPagePost>(u003cu003e9_3);
						readViewModel.Description = p.Title;
						return readViewModel;
					});
					RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel>> repositoryResponse1 = new RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel>>();
					repositoryResponse1.set_IsSucceed(true);
					repositoryResponse1.set_Data(readViewModels.ToList<Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel>());
					repositoryResponse = repositoryResponse1;
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					if (_transaction == null)
					{
						dbContextTransaction.Rollback();
					}
					RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel>> repositoryResponse2 = new RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel>>();
					repositoryResponse2.set_IsSucceed(true);
					repositoryResponse2.set_Data(null);
					repositoryResponse2.set_Exception(exception);
					repositoryResponse = repositoryResponse2;
				}
			}
			finally
			{
				if (_context == null)
				{
					dbContextTransaction.Dispose();
					RelationalDatabaseFacadeExtensions.CloseConnection(mixCmsContext.get_Database());
					dbContextTransaction.Dispose();
					mixCmsContext.Dispose();
				}
			}
			return repositoryResponse;
		}

		public override MixPagePost ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (this.Id == 0)
			{
				this.Id = ViewModelBase<MixCmsContext, MixPagePost, Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel>.Repository.Max((MixPagePost m) => m.Id, _context, _transaction).get_Data() + 1;
			}
			return base.ParseModel(_context, _transaction);
		}
	}
}