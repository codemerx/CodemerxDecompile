using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages;
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
	public class UpdateViewModel : ViewModelBase<MixCmsContext, MixPortalPage, Mix.Cms.Lib.ViewModels.MixPortalPages.UpdateViewModel>
	{
		[JsonProperty("childNavs")]
		public List<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel> ChildNavs
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

		[JsonProperty("parentNavs")]
		public List<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel> ParentNavs
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

		public UpdateViewModel()
		{
		}

		public UpdateViewModel(MixPortalPage model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.ParentNavs = this.GetParentNavs(_context, _transaction);
			this.ChildNavs = this.GetChildNavs(_context, _transaction);
		}

		public List<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel> GetChildNavs(MixCmsContext context, IDbContextTransaction transaction)
		{
			List<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel> list = (
				from PortalPage in (
					from PortalPage in EntityFrameworkQueryableExtensions.Include<MixPortalPage, ICollection<MixPortalPageNavigation>>(context.MixPortalPage, (MixPortalPage cp) => cp.MixPortalPageNavigationParent)
					where PortalPage.Id != this.Id
					select PortalPage).AsEnumerable<MixPortalPage>()
				select new Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel(new MixPortalPageNavigation()
				{
					Id = PortalPage.Id,
					ParentId = this.Id,
					Description = PortalPage.TextDefault
				}, context, transaction)).ToList<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel>();
			list.ForEach((Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel nav) => {
				MixPortalPageNavigation mixPortalPageNavigation = context.MixPortalPageNavigation.FirstOrDefault<MixPortalPageNavigation>((MixPortalPageNavigation m) => m.ParentId == this.Id && m.Id == nav.Id);
				nav.Priority = (mixPortalPageNavigation != null ? mixPortalPageNavigation.Priority : 0);
				nav.IsActived = mixPortalPageNavigation != null;
			});
			return (
				from m in list
				orderby m.Priority
				select m).ToList<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel>();
		}

		public List<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel> GetParentNavs(MixCmsContext context, IDbContextTransaction transaction)
		{
			List<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel> list = (
				from PortalPage in (
					from PortalPage in EntityFrameworkQueryableExtensions.Include<MixPortalPage, ICollection<MixPortalPageNavigation>>(context.MixPortalPage, (MixPortalPage cp) => cp.MixPortalPageNavigationParent)
					where PortalPage.Id != this.Id && PortalPage.Level == 0
					select PortalPage).AsEnumerable<MixPortalPage>()
				select new Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel()
				{
					Id = this.Id,
					ParentId = PortalPage.Id,
					Description = PortalPage.TextDefault,
					Level = PortalPage.Level
				}).ToList<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel>();
			list.ForEach((Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel nav) => nav.IsActived = context.MixPortalPageNavigation.Any<MixPortalPageNavigation>((MixPortalPageNavigation m) => m.ParentId == nav.ParentId && m.Id == this.Id));
			return (
				from m in list
				orderby m.Priority
				select m).ToList<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel>();
		}

		public override MixPortalPage ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel updateViewModel;
			if (this.Id == 0)
			{
				this.Id = ViewModelBase<MixCmsContext, MixPortalPage, Mix.Cms.Lib.ViewModels.MixPortalPages.UpdateViewModel>.Repository.Max((MixPortalPage c) => c.Id, _context, _transaction).get_Data() + 1;
				this.CreatedDateTime = DateTime.UtcNow;
			}
			List<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel> parentNavs = this.ParentNavs;
			if (parentNavs != null)
			{
				updateViewModel = parentNavs.FirstOrDefault<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel>((Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel p) => p.IsActived);
			}
			else
			{
				updateViewModel = null;
			}
			Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel updateViewModel1 = updateViewModel;
			if (updateViewModel1 == null)
			{
				this.Level = 0;
			}
			else
			{
				this.Level = updateViewModel1.Level + 1;
			}
			if (this.ChildNavs != null)
			{
				(
					from c in this.ChildNavs
					where c.IsActived
					select c).ToList<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel>().ForEach((Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel c) => c.Page.Level = this.Level + 1);
			}
			return base.ParseModel(_context, _transaction);
		}

		public override async Task<RepositoryResponse<bool>> RemoveRelatedModelsAsync(Mix.Cms.Lib.ViewModels.MixPortalPages.UpdateViewModel view, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			DbSet<MixPortalPageNavigation> mixPortalPageNavigation = _context.MixPortalPageNavigation;
			foreach (MixPortalPageNavigation mixPortalPageNavigation1 in 
				from p in mixPortalPageNavigation
				where p.Id == this.Id || p.ParentId == this.Id
				select p)
			{
				_context.Entry<MixPortalPageNavigation>(mixPortalPageNavigation1).set_State(2);
				if (mixPortalPageNavigation1.ParentId != this.Id)
				{
					continue;
				}
				MixCmsContext mixCmsContext = _context;
				DbSet<MixPortalPage> mixPortalPage = _context.MixPortalPage;
				((DbContext)mixCmsContext).Entry<MixPortalPage>(mixPortalPage.Single<MixPortalPage>((MixPortalPage sub) => sub.Id == mixPortalPageNavigation1.Id)).set_State(2);
			}
			MixCmsContext mixCmsContext1 = _context;
			CancellationToken cancellationToken = new CancellationToken();
			await ((DbContext)mixCmsContext1).SaveChangesAsync(cancellationToken);
			return repositoryResponse1;
		}

		public override async Task<RepositoryResponse<bool>> SaveSubModelsAsync(MixPortalPage parent, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			Mix.Cms.Lib.ViewModels.MixPortalPages.UpdateViewModel.u003cSaveSubModelsAsyncu003ed__72 variable = new Mix.Cms.Lib.ViewModels.MixPortalPages.UpdateViewModel.u003cSaveSubModelsAsyncu003ed__72();
			variable.u003cu003e4__this = this;
			variable.parent = parent;
			variable._context = _context;
			variable._transaction = _transaction;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixPortalPages.UpdateViewModel.u003cSaveSubModelsAsyncu003ed__72>(ref variable);
			return variable.u003cu003et__builder.Task;
		}
	}
}