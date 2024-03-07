using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.Services;
using Mix.Cms.Lib.ViewModels.MixAttributeSetDatas;
using Mix.Common.Helper;
using Mix.Domain.Core.Models;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas
{
	public class FormViewModel : ViewModelBase<MixCmsContext, MixRelatedAttributeData, Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.FormViewModel>
	{
		[JsonProperty("attributeData")]
		public Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel AttributeData
		{
			get;
			set;
		}

		[JsonProperty("attributeSetId")]
		public int AttributeSetId
		{
			get;
			set;
		}

		[JsonProperty("attributeSetName")]
		public string AttributeSetName
		{
			get;
			set;
		}

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

		[JsonProperty("cultures")]
		public List<SupportedCulture> Cultures
		{
			get;
			set;
		}

		[JsonProperty("dataId")]
		public string DataId
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

		[JsonProperty("id")]
		public string Id
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

		[JsonProperty("modifiedBy")]
		public string ModifiedBy
		{
			get;
			set;
		}

		[JsonProperty("parentId")]
		public string ParentId
		{
			get;
			set;
		}

		[JsonProperty("parentType")]
		public MixEnums.MixAttributeSetDataType ParentType
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

		public FormViewModel(MixRelatedAttributeData model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public FormViewModel()
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			string name;
			RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel> singleModel = ViewModelBase<MixCmsContext, MixAttributeSetData, Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel>.Repository.GetSingleModel((MixAttributeSetData p) => p.Id == this.DataId && p.Specificulture == this.Specificulture, _context, _transaction);
			if (singleModel.get_IsSucceed())
			{
				this.AttributeData = singleModel.get_Data();
			}
			MixAttributeSet mixAttributeSet = _context.MixAttributeSet.FirstOrDefault<MixAttributeSet>((MixAttributeSet m) => m.Id == this.AttributeSetId);
			if (mixAttributeSet != null)
			{
				name = mixAttributeSet.Name;
			}
			else
			{
				name = null;
			}
			this.AttributeSetName = name;
		}

		public override MixRelatedAttributeData ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (string.IsNullOrEmpty(this.Id))
			{
				this.Id = Guid.NewGuid().ToString();
				this.CreatedDateTime = DateTime.UtcNow;
				this.Status = (this.Status == MixEnums.MixContentStatus.Deleted ? Enum.Parse<MixEnums.MixContentStatus>(MixService.GetConfig<string>("DefaultContentStatus")) : this.Status);
			}
			return base.ParseModel(_context, _transaction);
		}

		public override async Task<RepositoryResponse<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.FormViewModel>> SaveModelAsync(bool isSaveSubModels = false, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.FormViewModel> repositoryResponse;
			MixCmsContext mixCmsContext = null;
			IDbContextTransaction dbContextTransaction = null;
			bool flag = false;
			RepositoryResponse<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.FormViewModel> repositoryResponse1 = new RepositoryResponse<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.FormViewModel>();
			repositoryResponse1.set_IsSucceed(true);
			RepositoryResponse<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.FormViewModel> repositoryResponse2 = repositoryResponse1;
			UnitOfWorkHelper<MixCmsContext>.InitTransaction(_context, _transaction, ref mixCmsContext, ref dbContextTransaction, ref flag);
			try
			{
				try
				{
					if (this.AttributeData == null || !string.IsNullOrEmpty(this.AttributeData.Id))
					{
						this.DataId = this.AttributeData.Id;
					}
					else
					{
						RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel> repositoryResponse3 = await this.AttributeData.SaveModelAsync(true, mixCmsContext, dbContextTransaction);
						if (repositoryResponse3.get_IsSucceed())
						{
							this.DataId = repositoryResponse3.get_Data().Id;
						}
						else
						{
							repositoryResponse2.set_IsSucceed(false);
							repositoryResponse2.set_Errors(repositoryResponse3.get_Errors());
							repositoryResponse2.set_Exception(repositoryResponse3.get_Exception());
						}
					}
					if (repositoryResponse2.get_IsSucceed())
					{
						repositoryResponse2 = await this.u003cu003en__0(true, mixCmsContext, dbContextTransaction);
					}
					UnitOfWorkHelper<MixCmsContext>.HandleTransaction(repositoryResponse2.get_IsSucceed(), flag, dbContextTransaction);
					repositoryResponse = repositoryResponse2;
				}
				catch (Exception exception)
				{
					repositoryResponse = UnitOfWorkHelper<MixCmsContext>.HandleException<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.FormViewModel>(exception, flag, dbContextTransaction);
				}
			}
			finally
			{
				if (flag)
				{
					mixCmsContext.Dispose();
				}
			}
			return repositoryResponse;
		}
	}
}