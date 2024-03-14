using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib.Models.Account;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.Account;
using Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages;
using Mix.Cms.Lib.ViewModels.MixPortalPageRoles;
using Mix.Cms.Lib.ViewModels.MixPortalPages;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
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
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.Account.MixRoles
{
	public class ReadViewModel : ViewModelBase<MixCmsAccountContext, AspNetRoles, Mix.Cms.Lib.ViewModels.Account.MixRoles.ReadViewModel>
	{
		[JsonProperty("concurrencyStamp")]
		public string ConcurrencyStamp
		{
			get;
			set;
		}

		[JsonProperty("id")]
		public string Id
		{
			get;
			set;
		}

		[JsonProperty("name")]
		[Required]
		public string Name
		{
			get;
			set;
		}

		[JsonProperty("normalizedName")]
		public string NormalizedName
		{
			get;
			set;
		}

		[JsonProperty("permissions")]
		public List<ReadRolePermissionViewModel> Permissions
		{
			get;
			set;
		}

		public ReadViewModel()
		{
		}

		public ReadViewModel(AspNetRoles model, MixCmsAccountContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override void ExpandView(MixCmsAccountContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.Permissions = ViewModelBase<MixCmsContext, MixPortalPage, ReadRolePermissionViewModel>.Repository.GetModelListBy((MixPortalPage p) => p.Level == 0 && (p.MixPortalPageRole.Any<MixPortalPageRole>((MixPortalPageRole r) => r.RoleId == this.Id) || this.Name == "SuperAdmin"), null, null).get_Data();
			foreach (ReadRolePermissionViewModel permission in this.Permissions)
			{
				permission.NavPermission = ViewModelBase<MixCmsContext, MixPortalPageRole, Mix.Cms.Lib.ViewModels.MixPortalPageRoles.ReadViewModel>.Repository.GetSingleModel((MixPortalPageRole n) => n.PageId == permission.Id && n.RoleId == this.Id, null, null).get_Data();
				foreach (Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.ReadViewModel childPage in permission.ChildPages)
				{
					childPage.Page.NavPermission = ViewModelBase<MixCmsContext, MixPortalPageRole, Mix.Cms.Lib.ViewModels.MixPortalPageRoles.ReadViewModel>.Repository.GetSingleModel((MixPortalPageRole n) => n.PageId == childPage.Page.Id && n.RoleId == this.Id, null, null).get_Data();
				}
			}
		}

		private List<Mix.Cms.Lib.ViewModels.MixPortalPageRoles.ReadViewModel> GetPermission()
		{
			Mix.Cms.Lib.ViewModels.Account.MixRoles.ReadViewModel.u003cu003ec__DisplayClass25_1 variable = null;
			List<Mix.Cms.Lib.ViewModels.MixPortalPageRoles.ReadViewModel> list;
			using (MixCmsContext mixCmsContext = new MixCmsContext())
			{
				IDbContextTransaction dbContextTransaction = mixCmsContext.get_Database().BeginTransaction();
				DbSet<MixPortalPage> mixPortalPage = mixCmsContext.MixPortalPage;
				ParameterExpression parameterExpression = Expression.Parameter(typeof(MixPortalPage), "cp");
				IIncludableQueryable<MixPortalPage, ICollection<MixPortalPageRole>> includableQueryable = EntityFrameworkQueryableExtensions.Include<MixPortalPage, ICollection<MixPortalPageRole>>(mixPortalPage, Expression.Lambda<Func<MixPortalPage, ICollection<MixPortalPageRole>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPortalPage).GetMethod("get_MixPortalPageRole").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(MixPortalPage), "Category");
				List<Mix.Cms.Lib.ViewModels.MixPortalPageRoles.ReadViewModel> readViewModels = includableQueryable.Select<MixPortalPage, Mix.Cms.Lib.ViewModels.MixPortalPageRoles.ReadViewModel>(Expression.Lambda<Func<MixPortalPage, Mix.Cms.Lib.ViewModels.MixPortalPageRoles.ReadViewModel>>(Expression.New((ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPortalPageRoles.ReadViewModel).GetMethod(".ctor", new Type[] { typeof(MixPortalPageRole), typeof(MixCmsContext), typeof(IDbContextTransaction) }).MethodHandle), (IEnumerable<Expression>)(new Expression[] { Expression.MemberInit(Expression.New(typeof(MixPortalPageRole)), new MemberBinding[] { Expression.Bind((MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPortalPageRole).GetMethod("set_RoleId", new Type[] { typeof(string) }).MethodHandle), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.Account.MixRoles.ReadViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.Account.MixRoles.ReadViewModel).GetMethod("get_Id").MethodHandle))), Expression.Bind((MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPortalPageRole).GetMethod("set_PageId", new Type[] { typeof(int) }).MethodHandle), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPortalPage).GetMethod("get_Id").MethodHandle))) }), Expression.Field(Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.Account.MixRoles.ReadViewModel.u003cu003ec__DisplayClass25_1)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.Account.MixRoles.ReadViewModel.u003cu003ec__DisplayClass25_1).GetField("CS$<>8__locals1").FieldHandle)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.Account.MixRoles.ReadViewModel.u003cu003ec__DisplayClass25_0).GetField("context").FieldHandle)), Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.Account.MixRoles.ReadViewModel.u003cu003ec__DisplayClass25_1)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.Account.MixRoles.ReadViewModel.u003cu003ec__DisplayClass25_1).GetField("transaction").FieldHandle)) })), new ParameterExpression[] { parameterExpression })).ToList<Mix.Cms.Lib.ViewModels.MixPortalPageRoles.ReadViewModel>();
				readViewModels.ForEach((Mix.Cms.Lib.ViewModels.MixPortalPageRoles.ReadViewModel nav) => nav.IsActived = this.context.MixPortalPageRole.Any<MixPortalPageRole>((MixPortalPageRole m) => m.PageId == nav.PageId && m.RoleId == this.u003cu003e4__this.Id));
				dbContextTransaction.Commit();
				list = (
					from m in readViewModels
					orderby m.Priority
					select m).ToList<Mix.Cms.Lib.ViewModels.MixPortalPageRoles.ReadViewModel>();
			}
			return list;
		}

		public override AspNetRoles ParseModel(MixCmsAccountContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (string.IsNullOrEmpty(this.Id))
			{
				this.Id = Guid.NewGuid().ToString();
			}
			return base.ParseModel(_context, _transaction);
		}

		public override async Task<RepositoryResponse<bool>> RemoveRelatedModelsAsync(Mix.Cms.Lib.ViewModels.Account.MixRoles.ReadViewModel view, MixCmsAccountContext _context = null, IDbContextTransaction _transaction = null)
		{
			DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsAccountContext, AspNetUserRoles, UserRoleViewModel>.Repository;
			RepositoryResponse<List<AspNetUserRoles>> repositoryResponse = await repository.RemoveListModelAsync(false, (AspNetUserRoles ur) => ur.RoleId == this.Id, _context, _transaction);
			RepositoryResponse<bool> repositoryResponse1 = new RepositoryResponse<bool>();
			repositoryResponse1.set_IsSucceed(repositoryResponse.get_IsSucceed());
			repositoryResponse1.set_Errors(repositoryResponse.get_Errors());
			repositoryResponse1.set_Exception(repositoryResponse.get_Exception());
			return repositoryResponse1;
		}
	}
}