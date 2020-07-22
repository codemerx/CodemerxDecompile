using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib.Models.Account;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels.Account
{
	public class UserRoleViewModel : ViewModelBase<MixCmsAccountContext, AspNetUserRoles, UserRoleViewModel>
	{
		[JsonProperty("applicationUserId")]
		public string ApplicationUserId
		{
			get;
			set;
		}

		[JsonProperty("role")]
		public RoleViewModel Role
		{
			get;
			set;
		}

		[JsonProperty("roleId")]
		public string RoleId
		{
			get;
			set;
		}

		[JsonProperty("userId")]
		public string UserId
		{
			get;
			set;
		}

		public UserRoleViewModel()
		{
			base();
			return;
		}

		public UserRoleViewModel(AspNetUserRoles model, MixCmsAccountContext _context = null, IDbContextTransaction _transaction = null)
		{
			base(model, _context, _transaction);
			return;
		}

		public override void ExpandView(MixCmsAccountContext _context = null, IDbContextTransaction _transaction = null)
		{
			stackVariable1 = ViewModelBase<MixCmsAccountContext, AspNetRoles, RoleViewModel>.Repository;
			V_0 = Expression.Parameter(Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Mix.Cms.Lib.ViewModels.Account.UserRoleViewModel::ExpandView(Mix.Cms.Lib.Models.Account.MixCmsAccountContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: System.Void ExpandView(Mix.Cms.Lib.Models.Account.MixCmsAccountContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

	}
}