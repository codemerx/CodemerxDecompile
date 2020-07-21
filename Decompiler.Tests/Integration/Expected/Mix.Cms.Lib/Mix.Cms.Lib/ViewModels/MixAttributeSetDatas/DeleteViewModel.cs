using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib.Models.Cms;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixAttributeSetDatas
{
	public class DeleteViewModel : ViewModelBase<MixCmsContext, MixAttributeSetData, DeleteViewModel>
	{
		[JsonProperty("id")]
		public string Id
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

		public DeleteViewModel()
		{
			base();
			return;
		}

		public DeleteViewModel(MixAttributeSetData model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			base(model, _context, _transaction);
			return;
		}

		public override RepositoryResponse<bool> RemoveRelatedModels(DeleteViewModel view, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			stackVariable0 = new RepositoryResponse<bool>();
			stackVariable0.set_IsSucceed(true);
			V_0 = stackVariable0;
			stackVariable2 = ViewModelBase<MixCmsContext, MixAttributeSetValue, DeleteViewModel>.Repository;
			V_1 = Expression.Parameter(Type.GetTypeFromHandle(// 
			// Current member / type: Mix.Domain.Core.ViewModels.RepositoryResponse`1<System.Boolean> Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.DeleteViewModel::RemoveRelatedModels(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.DeleteViewModel,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: Mix.Domain.Core.ViewModels.RepositoryResponse<System.Boolean> RemoveRelatedModels(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.DeleteViewModel,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public override async Task<RepositoryResponse<bool>> RemoveRelatedModelsAsync(DeleteViewModel view, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0.u003cu003e4__this = this;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<DeleteViewModel.u003cRemoveRelatedModelsAsyncu003ed__11>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}