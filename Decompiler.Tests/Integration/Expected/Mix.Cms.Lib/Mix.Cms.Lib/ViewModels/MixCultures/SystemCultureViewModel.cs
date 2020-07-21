using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixConfigurations;
using Mix.Cms.Lib.ViewModels.MixLanguages;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixCultures
{
	public class SystemCultureViewModel : ViewModelBase<MixCmsContext, MixCulture, SystemCultureViewModel>
	{
		[JsonProperty("alias")]
		public string Alias
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
		public string Description
		{
			get;
			set;
		}

		[JsonProperty("fullName")]
		public string FullName
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

		[JsonProperty("lcid")]
		public string Lcid
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

		public SystemCultureViewModel()
		{
			base();
			return;
		}

		public SystemCultureViewModel(MixCulture model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			base(model, _context, _transaction);
			return;
		}

		public override async Task<RepositoryResponse<bool>> RemoveRelatedModelsAsync(SystemCultureViewModel view, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0.view = view;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<SystemCultureViewModel.u003cRemoveRelatedModelsAsyncu003ed__54>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}