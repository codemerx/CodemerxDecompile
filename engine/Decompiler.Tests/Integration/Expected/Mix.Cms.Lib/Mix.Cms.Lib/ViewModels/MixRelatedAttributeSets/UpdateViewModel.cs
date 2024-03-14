using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixAttributeSets;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels.MixRelatedAttributeSets
{
	public class UpdateViewModel : ViewModelBase<MixCmsContext, MixRelatedAttributeSet, Mix.Cms.Lib.ViewModels.MixRelatedAttributeSets.UpdateViewModel>
	{
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

		public Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel Data
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
		public int Id
		{
			get;
			set;
		}

		[JsonProperty("image")]
		public string Image
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
		public int ParentId
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

		[JsonProperty("status")]
		public MixEnums.MixContentStatus Status
		{
			get;
			set;
		}

		public UpdateViewModel(MixRelatedAttributeSet model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public UpdateViewModel()
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel> singleModel = ViewModelBase<MixCmsContext, MixAttributeSet, Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel>.Repository.GetSingleModel((MixAttributeSet p) => p.Id == this.Id, _context, _transaction);
			if (singleModel.get_IsSucceed())
			{
				this.Data = singleModel.get_Data();
			}
		}

		public override MixRelatedAttributeSet ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (this.Id == 0)
			{
				this.Id = ViewModelBase<MixCmsContext, MixRelatedAttributeSet, Mix.Cms.Lib.ViewModels.MixRelatedAttributeSets.UpdateViewModel>.Repository.Count(null, null).get_Data() + 1;
				this.CreatedDateTime = DateTime.UtcNow;
			}
			return base.ParseModel(_context, _transaction);
		}
	}
}