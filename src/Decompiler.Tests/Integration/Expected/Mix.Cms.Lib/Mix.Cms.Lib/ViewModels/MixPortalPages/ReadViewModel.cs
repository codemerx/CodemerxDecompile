using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages;
using Mix.Common.Helper;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixPortalPages
{
	public class ReadViewModel : ViewModelBase<MixCmsContext, MixPortalPage, Mix.Cms.Lib.ViewModels.MixPortalPages.ReadViewModel>
	{
		[JsonProperty("childNavs")]
		public List<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.ReadViewModel> ChildNavs
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

		[JsonProperty("description")]
		public string Descriotion
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

		[JsonProperty("lastModified")]
		public DateTime? LastModified
		{
			get;
			set;
		}

		[JsonProperty("level")]
		public int Level
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

		[JsonProperty("textDefault")]
		public string TextDefault
		{
			get;
			set;
		}

		[JsonProperty("textKeyword")]
		public string TextKeyword
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

		[JsonProperty("url")]
		public string Url
		{
			get;
			set;
		}

		public ReadViewModel()
		{
		}

		public ReadViewModel(MixPortalPage model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.ReadViewModel>> modelListBy = ViewModelBase<MixCmsContext, MixPortalPageNavigation, Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.ReadViewModel>.Repository.GetModelListBy((MixPortalPageNavigation n) => n.ParentId == this.Id, _context, _transaction);
			if (modelListBy.get_IsSucceed())
			{
				this.ChildNavs = (
					from c in modelListBy.get_Data()
					orderby c.Priority
					select c).ToList<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.ReadViewModel>();
			}
		}

		public override MixPortalPage ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (this.CreatedDateTime == new DateTime())
			{
				this.CreatedDateTime = DateTime.UtcNow;
			}
			return base.ParseModel(_context, _transaction);
		}

		public override async Task<RepositoryResponse<bool>> RemoveRelatedModelsAsync(Mix.Cms.Lib.ViewModels.MixPortalPages.ReadViewModel view, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			DbSet<MixPortalPageNavigation> mixPortalPageNavigation = _context.MixPortalPageNavigation;
			IQueryable<MixPortalPageNavigation> parentId = 
				from p in mixPortalPageNavigation
				where p.ParentId == this.Id || p.Id == this.Id
				select p;
			IQueryable<MixPortalPageNavigation> mixPortalPageNavigations = parentId;
			Action<MixPortalPageNavigation> action = (MixPortalPageNavigation n) => _context.Entry<MixPortalPageNavigation>(n).set_State(2);
			CancellationToken cancellationToken = new CancellationToken();
			await EntityFrameworkQueryableExtensions.ForEachAsync<MixPortalPageNavigation>(mixPortalPageNavigations, action, cancellationToken);
			DbSet<MixPortalPageRole> mixPortalPageRole = _context.MixPortalPageRole;
			IQueryable<MixPortalPageNavigation> mixPortalPageNavigations1 = parentId;
			Action<MixPortalPageNavigation> action1 = (MixPortalPageNavigation n) => _context.Entry<MixPortalPageNavigation>(n).set_State(2);
			cancellationToken = new CancellationToken();
			await EntityFrameworkQueryableExtensions.ForEachAsync<MixPortalPageNavigation>(mixPortalPageNavigations1, action1, cancellationToken);
			MixCmsContext mixCmsContext = _context;
			cancellationToken = new CancellationToken();
			await ((DbContext)mixCmsContext).SaveChangesAsync(cancellationToken);
			return repositoryResponse1;
		}

		public static async Task<RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixPortalPages.ReadViewModel>>> UpdateInfosAsync(List<Mix.Cms.Lib.ViewModels.MixPortalPages.ReadViewModel> cates)
		{
			RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixPortalPages.ReadViewModel>> repositoryResponse;
			MixCmsContext mixCmsContext = new MixCmsContext();
			IDbContextTransaction dbContextTransaction = mixCmsContext.get_Database().BeginTransaction();
			RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixPortalPages.ReadViewModel>> repositoryResponse1 = new RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixPortalPages.ReadViewModel>>();
			try
			{
				try
				{
					foreach (Mix.Cms.Lib.ViewModels.MixPortalPages.ReadViewModel cate in cates)
					{
						RepositoryResponse<Mix.Cms.Lib.ViewModels.MixPortalPages.ReadViewModel> repositoryResponse2 = await cate.SaveModelAsync(false, mixCmsContext, dbContextTransaction);
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
					UnitOfWorkHelper<MixCmsContext>.HandleException<Mix.Cms.Lib.ViewModels.MixPortalPages.ReadViewModel>(exception, true, dbContextTransaction);
					RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixPortalPages.ReadViewModel>> repositoryResponse3 = new RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixPortalPages.ReadViewModel>>();
					repositoryResponse3.set_IsSucceed(false);
					repositoryResponse3.set_Data(null);
					repositoryResponse3.set_Exception(exception);
					repositoryResponse = repositoryResponse3;
				}
			}
			finally
			{
				dbContextTransaction.Dispose();
				RelationalDatabaseFacadeExtensions.CloseConnection(mixCmsContext.get_Database());
				dbContextTransaction.Dispose();
				mixCmsContext.Dispose();
			}
			return repositoryResponse;
		}
	}
}