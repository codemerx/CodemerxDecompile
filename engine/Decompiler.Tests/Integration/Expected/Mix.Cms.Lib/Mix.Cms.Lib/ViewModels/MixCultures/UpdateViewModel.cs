using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixConfigurations;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixCultures
{
	public class UpdateViewModel : ViewModelBase<MixCmsContext, MixCulture, Mix.Cms.Lib.ViewModels.MixCultures.UpdateViewModel>
	{
		[JsonProperty("alias")]
		public string Alias
		{
			get;
			set;
		}

		[JsonProperty("configurations")]
		public List<ReadMvcViewModel> Configurations
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

		public UpdateViewModel()
		{
			base();
			return;
		}

		public UpdateViewModel(MixCulture model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			base(model, _context, _transaction);
			return;
		}

		public async Task<RepositoryResponse<bool>> CloneConfigurationsAsync(MixCulture parent, MixCmsContext context, IDbContextTransaction transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.context = context;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixCultures.UpdateViewModel.u003cCloneConfigurationsAsyncu003ed__73>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<RepositoryResponse<bool>> CloneLanguagesAsync(MixCulture parent, MixCmsContext context, IDbContextTransaction transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.context = context;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixCultures.UpdateViewModel.u003cCloneLanguagesAsyncu003ed__72>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<RepositoryResponse<bool>> CloneMediasAsync(MixCulture parent, MixCmsContext context, IDbContextTransaction transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.context = context;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixCultures.UpdateViewModel.u003cCloneMediasAsyncu003ed__64>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<RepositoryResponse<bool>> CloneModuleDatasAsync(MixCulture parent, MixCmsContext context, IDbContextTransaction transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.context = context;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixCultures.UpdateViewModel.u003cCloneModuleDatasAsyncu003ed__71>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<RepositoryResponse<bool>> CloneModulePostsAsync(MixCulture parent, MixCmsContext context, IDbContextTransaction transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.context = context;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixCultures.UpdateViewModel.u003cCloneModulePostsAsyncu003ed__67>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<RepositoryResponse<bool>> CloneModulesAsync(MixCulture parent, MixCmsContext context, IDbContextTransaction transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.context = context;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixCultures.UpdateViewModel.u003cCloneModulesAsyncu003ed__63>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<RepositoryResponse<bool>> ClonePageModulesAsync(MixCulture parent, MixCmsContext context, IDbContextTransaction transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.context = context;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixCultures.UpdateViewModel.u003cClonePageModulesAsyncu003ed__69>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<RepositoryResponse<bool>> ClonePagePostsAsync(MixCulture parent, MixCmsContext context, IDbContextTransaction transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.context = context;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixCultures.UpdateViewModel.u003cClonePagePostsAsyncu003ed__68>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<RepositoryResponse<bool>> ClonePagesAsync(MixCulture parent, MixCmsContext context, IDbContextTransaction transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.context = context;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixCultures.UpdateViewModel.u003cClonePagesAsyncu003ed__74>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<RepositoryResponse<bool>> ClonePostMediasAsync(MixCulture parent, MixCmsContext context, IDbContextTransaction transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.context = context;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixCultures.UpdateViewModel.u003cClonePostMediasAsyncu003ed__65>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<RepositoryResponse<bool>> ClonePostPostsAsync(MixCulture parent, MixCmsContext context, IDbContextTransaction transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.context = context;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixCultures.UpdateViewModel.u003cClonePostPostsAsyncu003ed__66>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<RepositoryResponse<bool>> ClonePostsAsync(MixCulture parent, MixCmsContext context, IDbContextTransaction transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.context = context;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixCultures.UpdateViewModel.u003cClonePostsAsyncu003ed__70>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<RepositoryResponse<bool>> CloneUrlAliasAsync(MixCulture parent, MixCmsContext context, IDbContextTransaction transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.context = context;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixCultures.UpdateViewModel.u003cCloneUrlAliasAsyncu003ed__62>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			stackVariable0 = ViewModelBase<MixCmsContext, MixConfiguration, ReadMvcViewModel>.Repository;
			V_1 = Expression.Parameter(Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Mix.Cms.Lib.ViewModels.MixCultures.UpdateViewModel::ExpandView(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: System.Void ExpandView(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public override MixCulture ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (this.get_Id() == 0)
			{
				stackVariable7 = ViewModelBase<MixCmsContext, MixCulture, Mix.Cms.Lib.ViewModels.MixCultures.UpdateViewModel>.Repository;
				V_0 = Expression.Parameter(Type.GetTypeFromHandle(// 
				// Current member / type: Mix.Cms.Lib.Models.Cms.MixCulture Mix.Cms.Lib.ViewModels.MixCultures.UpdateViewModel::ParseModel(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Exception in: Mix.Cms.Lib.Models.Cms.MixCulture ParseModel(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Specified method is not supported.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com


		public override async Task<RepositoryResponse<MixCulture>> RemoveModelAsync(bool isRemoveRelatedModels = false, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0.u003cu003e4__this = this;
			V_0.isRemoveRelatedModels = isRemoveRelatedModels;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<MixCulture>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixCultures.UpdateViewModel.u003cRemoveModelAsyncu003ed__76>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public override async Task<RepositoryResponse<bool>> RemoveRelatedModelsAsync(Mix.Cms.Lib.ViewModels.MixCultures.UpdateViewModel view, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0.u003cu003e4__this = this;
			V_0._context = _context;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixCultures.UpdateViewModel.u003cRemoveRelatedModelsAsyncu003ed__75>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public override async Task<RepositoryResponse<Mix.Cms.Lib.ViewModels.MixCultures.UpdateViewModel>> SaveModelAsync(bool isSaveSubModels = false, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0.u003cu003e4__this = this;
			V_0.isSaveSubModels = isSaveSubModels;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<Mix.Cms.Lib.ViewModels.MixCultures.UpdateViewModel>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixCultures.UpdateViewModel.u003cSaveModelAsyncu003ed__60>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public override async Task<RepositoryResponse<bool>> SaveSubModelsAsync(MixCulture parent, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0.u003cu003e4__this = this;
			V_0.parent = parent;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixCultures.UpdateViewModel.u003cSaveSubModelsAsyncu003ed__61>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}