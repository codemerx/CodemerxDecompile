using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixAttributeSetDatas;
using Mix.Domain.Core.Models;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas
{
	public class NavigationViewModel : ViewModelBase<MixCmsContext, MixRelatedAttributeData, Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.NavigationViewModel>
	{
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

		[JsonProperty("data")]
		public Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.NavigationViewModel Data
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

		[JsonProperty("parentName")]
		public string ParentName
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

		public NavigationViewModel(MixRelatedAttributeData model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public NavigationViewModel()
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.NavigationViewModel> singleModel = ViewModelBase<MixCmsContext, MixAttributeSetData, Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.NavigationViewModel>.Repository.GetSingleModel((MixAttributeSetData p) => p.Id == this.DataId && p.Specificulture == this.Specificulture, _context, _transaction);
			if (singleModel.get_IsSucceed())
			{
				this.Data = singleModel.get_Data();
			}
		}
	}
}