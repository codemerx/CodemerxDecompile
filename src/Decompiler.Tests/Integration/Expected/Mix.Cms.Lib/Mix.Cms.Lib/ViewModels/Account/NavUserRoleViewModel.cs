using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib.Models.Account;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels.Account
{
	public class NavUserRoleViewModel : ViewModelBase<MixCmsAccountContext, AspNetUserRoles, NavUserRoleViewModel>
	{
		[JsonProperty("applicationUserId")]
		public string ApplicationUserId
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

		[JsonProperty("isActived")]
		public bool IsActived
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

		[JsonProperty("specificulture")]
		public string Specificulture
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

		public NavUserRoleViewModel()
		{
		}

		public NavUserRoleViewModel(AspNetUserRoles model, MixCmsAccountContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override void ExpandView(MixCmsAccountContext _context = null, IDbContextTransaction _transaction = null)
		{
			string name;
			this.Role = ViewModelBase<MixCmsAccountContext, AspNetRoles, RoleViewModel>.Repository.GetSingleModel((AspNetRoles r) => r.Id == this.RoleId, _context, _transaction).get_Data();
			RoleViewModel role = this.Role;
			if (role != null)
			{
				name = role.Name;
			}
			else
			{
				name = null;
			}
			this.Description = name;
		}
	}
}