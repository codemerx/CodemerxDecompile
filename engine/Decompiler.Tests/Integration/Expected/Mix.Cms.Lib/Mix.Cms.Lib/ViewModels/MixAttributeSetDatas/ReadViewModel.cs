using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixAttributeFields;
using Mix.Cms.Lib.ViewModels.MixAttributeSetValues;
using Mix.Domain.Core.Models;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels.MixAttributeSetDatas
{
	public class ReadViewModel : ViewModelBase<MixCmsContext, MixAttributeSetData, Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ReadViewModel>
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

		[JsonProperty("cultures")]
		public List<SupportedCulture> Cultures
		{
			get;
			set;
		}

		[JsonProperty("fields")]
		public List<Mix.Cms.Lib.ViewModels.MixAttributeFields.ReadViewModel> Fields
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

		[JsonProperty("values")]
		public List<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ReadViewModel> Values
		{
			get;
			set;
		}

		public ReadViewModel()
		{
		}

		public ReadViewModel(MixAttributeSetData model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ReadViewModel>> modelListBy = ViewModelBase<MixCmsContext, MixAttributeSetValue, Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ReadViewModel>.Repository.GetModelListBy((MixAttributeSetValue a) => a.DataId == this.Id && a.Specificulture == this.Specificulture, _context, _transaction);
			if (!modelListBy.get_IsSucceed())
			{
				Console.WriteLine(modelListBy.get_Exception());
			}
			else
			{
				this.Values = (
					from a in modelListBy.get_Data()
					orderby a.Priority
					select a).ToList<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ReadViewModel>();
			}
			this.Fields = ViewModelBase<MixCmsContext, MixAttributeField, Mix.Cms.Lib.ViewModels.MixAttributeFields.ReadViewModel>.Repository.GetModelListBy((MixAttributeField f) => f.AttributeSetId == this.AttributeSetId, _context, _transaction).get_Data();
			foreach (Mix.Cms.Lib.ViewModels.MixAttributeFields.ReadViewModel readViewModel in 
				from f in this.Fields
				orderby f.Priority
				select f)
			{
				Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ReadViewModel attributeSetName = this.Values.FirstOrDefault<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ReadViewModel>((Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ReadViewModel v) => v.AttributeFieldId == readViewModel.Id);
				if (attributeSetName == null)
				{
					attributeSetName = new Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ReadViewModel(new MixAttributeSetValue()
					{
						AttributeFieldId = readViewModel.Id
					}, _context, _transaction)
					{
						Field = readViewModel,
						AttributeFieldName = readViewModel.Name,
						StringValue = readViewModel.DefaultValue,
						Priority = readViewModel.Priority
					};
					this.Values.Add(attributeSetName);
				}
				attributeSetName.AttributeSetName = this.AttributeSetName;
				attributeSetName.Priority = readViewModel.Priority;
				attributeSetName.Field = readViewModel;
			}
		}
	}
}