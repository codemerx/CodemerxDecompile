using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixAttributeSetValues;
using Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixAttributeSetDatas
{
	public class DeleteViewModel : ViewModelBase<MixCmsContext, MixAttributeSetData, Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.DeleteViewModel>
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
		}

		public DeleteViewModel(MixAttributeSetData model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override RepositoryResponse<bool> RemoveRelatedModels(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.DeleteViewModel view, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			ViewModelHelper.HandleResult<List<MixAttributeSetValue>>(ViewModelBase<MixCmsContext, MixAttributeSetValue, Mix.Cms.Lib.ViewModels.MixAttributeSetValues.DeleteViewModel>.Repository.RemoveListModel(false, (MixAttributeSetValue f) => f.DataId == this.Id && f.Specificulture == this.Specificulture, _context, _transaction), ref repositoryResponse1);
			if (repositoryResponse1.get_IsSucceed())
			{
				ViewModelHelper.HandleResult<List<MixRelatedAttributeData>>(ViewModelBase<MixCmsContext, MixRelatedAttributeData, Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.DeleteViewModel>.Repository.RemoveListModel(true, (MixRelatedAttributeData d) => (d.DataId == this.Id || d.ParentId == this.Id) && d.Specificulture == this.Specificulture, null, null), ref repositoryResponse1);
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				ViewModelHelper.HandleResult<List<MixAttributeSetValue>>(ViewModelBase<MixCmsContext, MixAttributeSetValue, Mix.Cms.Lib.ViewModels.MixAttributeSetValues.DeleteViewModel>.Repository.RemoveListModel(false, (MixAttributeSetValue f) => f.DataId == this.Id && f.Specificulture == this.Specificulture, _context, _transaction), ref repositoryResponse1);
				ViewModelHelper.HandleResult<List<MixAttributeSetData>>(ViewModelBase<MixCmsContext, MixAttributeSetData, Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.DeleteViewModel>.Repository.RemoveListModel(false, (MixAttributeSetData f) => f.Id == this.Id && f.Specificulture == this.Specificulture, _context, _transaction), ref repositoryResponse1);
			}
			return repositoryResponse1;
		}

		public override async Task<RepositoryResponse<bool>> RemoveRelatedModelsAsync(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.DeleteViewModel view, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixAttributeSetValue, Mix.Cms.Lib.ViewModels.MixAttributeSetValues.DeleteViewModel>.Repository;
			ViewModelHelper.HandleResult<List<MixAttributeSetValue>>(await repository.RemoveListModelAsync(false, (MixAttributeSetValue f) => f.DataId == this.Id && f.Specificulture == this.Specificulture, _context, _transaction), ref repositoryResponse1);
			if (repositoryResponse1.get_IsSucceed())
			{
				DefaultRepository<!0, !1, !2> defaultRepository = ViewModelBase<MixCmsContext, MixRelatedAttributeData, Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.DeleteViewModel>.Repository;
				ViewModelHelper.HandleResult<List<MixRelatedAttributeData>>(await defaultRepository.RemoveListModelAsync(true, (MixRelatedAttributeData d) => (d.DataId == this.Id || d.ParentId == this.Id) && d.Specificulture == this.Specificulture, _context, _transaction), ref repositoryResponse1);
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				DefaultRepository<!0, !1, !2> repository1 = ViewModelBase<MixCmsContext, MixAttributeSetValue, Mix.Cms.Lib.ViewModels.MixAttributeSetValues.DeleteViewModel>.Repository;
				ViewModelHelper.HandleResult<List<MixAttributeSetValue>>(await repository1.RemoveListModelAsync(false, (MixAttributeSetValue f) => f.DataId == this.Id && f.Specificulture == this.Specificulture, _context, _transaction), ref repositoryResponse1);
				DefaultRepository<!0, !1, !2> defaultRepository1 = ViewModelBase<MixCmsContext, MixAttributeSetData, Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.DeleteViewModel>.Repository;
				ViewModelHelper.HandleResult<List<MixAttributeSetData>>(await defaultRepository1.RemoveListModelAsync(false, (MixAttributeSetData f) => f.Id == this.Id && f.Specificulture == this.Specificulture, _context, _transaction), ref repositoryResponse1);
			}
			return repositoryResponse1;
		}
	}
}