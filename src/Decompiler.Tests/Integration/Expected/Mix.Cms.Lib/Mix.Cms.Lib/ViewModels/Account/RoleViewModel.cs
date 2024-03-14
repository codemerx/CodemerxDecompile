using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib.Models.Account;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.Account
{
	public class RoleViewModel : ViewModelBase<MixCmsAccountContext, AspNetRoles, RoleViewModel>
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

		public RoleViewModel()
		{
		}

		public RoleViewModel(AspNetRoles model, MixCmsAccountContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override AspNetRoles ParseModel(MixCmsAccountContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (string.IsNullOrEmpty(this.Id))
			{
				this.Id = Guid.NewGuid().ToString();
			}
			return base.ParseModel(_context, _transaction);
		}

		public override async Task<RepositoryResponse<bool>> RemoveRelatedModelsAsync(RoleViewModel view, MixCmsAccountContext _context = null, IDbContextTransaction _transaction = null)
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