using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels.MixAttributeFields
{
	public class UpdateViewModel : ViewModelBase<MixCmsContext, MixAttributeField, UpdateViewModel>
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

		[JsonProperty("dataType")]
		public MixEnums.MixDataType DataType
		{
			get;
			set;
		}

		[JsonProperty("defaultValue")]
		public string DefaultValue
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

		[JsonProperty("isEncrypt")]
		public bool IsEncrypt
		{
			get;
			set;
		}

		[JsonProperty("isMultiple")]
		public bool IsMultiple
		{
			get;
			set;
		}

		[JsonProperty("isRegex")]
		public bool IsRegex
		{
			get
			{
				return !string.IsNullOrEmpty(this.Regex);
			}
		}

		[JsonProperty("isRequire")]
		public bool IsRequire
		{
			get;
			set;
		}

		[JsonProperty("isSelect")]
		public bool IsSelect
		{
			get;
			set;
		}

		[JsonProperty("isUnique")]
		public bool IsUnique
		{
			get;
			set;
		}

		[JsonProperty("options")]
		public JArray JOptions
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

		[JsonProperty("name")]
		public string Name
		{
			get;
			set;
		}

		[JsonProperty("strOptions")]
		public string Options { get; set; } = "[]";

		[JsonProperty("priority")]
		public int Priority
		{
			get;
			set;
		}

		[JsonProperty("referenceId")]
		public int? ReferenceId
		{
			get;
			set;
		}

		[JsonProperty("regex")]
		public string Regex
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

		[JsonProperty("title")]
		public string Title
		{
			get;
			set;
		}

		public UpdateViewModel()
		{
		}

		public UpdateViewModel(MixAttributeField model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (!string.IsNullOrEmpty(this.Options))
			{
				this.JOptions = JArray.Parse(this.Options);
			}
		}

		public override MixAttributeField ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			string str;
			if (this.Id == 0)
			{
				this.Id = ViewModelBase<MixCmsContext, MixAttributeField, UpdateViewModel>.Repository.Max((MixAttributeField s) => s.Id, _context, _transaction).get_Data() + 1;
				this.CreatedDateTime = DateTime.UtcNow;
			}
			JArray jOptions = this.JOptions;
			if (jOptions != null)
			{
				str = jOptions.ToString();
			}
			else
			{
				str = null;
			}
			this.Options = str;
			return base.ParseModel(_context, _transaction);
		}

		public override void Validate(MixCmsContext _context, IDbContextTransaction _transaction)
		{
			base.Validate(_context, _transaction);
			if (base.get_IsValid())
			{
				if (this.AttributeSetName == "sys_additional_field")
				{
					RepositoryResponse<UpdateViewModel> singleModel = ViewModelBase<MixCmsContext, MixAttributeField, UpdateViewModel>.Repository.GetSingleModel((MixAttributeField m) => m.Name == this.Name && m.DataType == (int)this.DataType, _context, _transaction);
					if (singleModel.get_IsSucceed())
					{
						this.Id = singleModel.get_Data().Id;
						this.CreatedDateTime = singleModel.get_Data().CreatedDateTime;
					}
				}
				else
				{
					base.set_IsValid(!ViewModelBase<MixCmsContext, MixAttributeField, UpdateViewModel>.Repository.CheckIsExists((MixAttributeField f) => {
						if (f.Id == this.Id || !(f.Name == this.Name))
						{
							return false;
						}
						return f.AttributeSetId == this.AttributeSetId;
					}, null, null));
					if (!base.get_IsValid())
					{
						base.get_Errors().Add(string.Concat("Field ", this.Name, " Existed"));
						return;
					}
				}
			}
		}
	}
}