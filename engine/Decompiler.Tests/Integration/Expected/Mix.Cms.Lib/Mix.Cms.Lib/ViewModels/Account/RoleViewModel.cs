using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib.Models.Account;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq.Expressions;
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
			base();
			return;
		}

		public RoleViewModel(AspNetRoles model, MixCmsAccountContext _context = null, IDbContextTransaction _transaction = null)
		{
			base(model, _context, _transaction);
			return;
		}

		public override AspNetRoles ParseModel(MixCmsAccountContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (string.IsNullOrEmpty(this.get_Id()))
			{
				this.set_Id(Guid.NewGuid().ToString());
			}
			return this.ParseModel(_context, _transaction);
		}

		public override async Task<RepositoryResponse<bool>> RemoveRelatedModelsAsync(RoleViewModel view, MixCmsAccountContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0.u003cu003e4__this = this;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<RoleViewModel.u003cRemoveRelatedModelsAsyncu003ed__19>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}