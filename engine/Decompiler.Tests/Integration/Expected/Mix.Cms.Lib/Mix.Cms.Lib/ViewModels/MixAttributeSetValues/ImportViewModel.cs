using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixAttributeFields;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Mix.Cms.Lib.ViewModels.MixAttributeSetValues
{
	public class ImportViewModel : ViewModelBase<MixCmsContext, MixAttributeSetValue, ImportViewModel>
	{
		[JsonProperty("attributeFieldId")]
		public int AttributeFieldId
		{
			get;
			set;
		}

		[JsonProperty("attributeFieldName")]
		public string AttributeFieldName
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

		[JsonProperty("booleanValue")]
		public bool? BooleanValue
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

		[JsonProperty("dataId")]
		public string DataId
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

		[JsonProperty("dateTimeValue")]
		public DateTime? DateTimeValue
		{
			get;
			set;
		}

		[JsonProperty("doubleValue")]
		public double? DoubleValue
		{
			get;
			set;
		}

		[JsonProperty("encryptKey")]
		public string EncryptKey
		{
			get;
			set;
		}

		[JsonProperty("encryptType")]
		public int EncryptType
		{
			get;
			set;
		}

		[JsonProperty("encryptValue")]
		public string EncryptValue
		{
			get;
			set;
		}

		[JsonProperty("field")]
		public Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel Field
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

		[JsonProperty("integerValue")]
		public int? IntegerValue
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

		[JsonProperty("stringValue")]
		public string StringValue
		{
			get;
			set;
		}

		public ImportViewModel()
		{
		}

		public ImportViewModel(MixAttributeSetValue model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			string name;
			if (this.AttributeFieldId <= 0)
			{
				this.Field = new Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel()
				{
					DataType = this.DataType,
					Id = this.AttributeFieldId,
					Title = this.AttributeFieldName,
					Name = this.AttributeFieldName,
					Priority = this.Priority
				};
			}
			else
			{
				this.Field = ViewModelBase<MixCmsContext, MixAttributeField, Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel>.Repository.GetSingleModel((MixAttributeField f) => f.Id == this.AttributeFieldId, null, null).get_Data();
				if (this.Field != null && this.DataType == MixEnums.MixDataType.Reference)
				{
					MixAttributeSet mixAttributeSet = _context.MixAttributeSet.FirstOrDefault<MixAttributeSet>((MixAttributeSet m) => (int?)m.Id == this.Field.ReferenceId);
					if (mixAttributeSet != null)
					{
						name = mixAttributeSet.Name;
					}
					else
					{
						name = null;
					}
					this.AttributeSetName = name;
					return;
				}
			}
		}

		private void ParseDefaultValue(string defaultValue)
		{
			double num;
			bool flag;
			int num1;
			this.StringValue = defaultValue;
			MixEnums.MixDataType dataType = this.DataType;
			switch (dataType)
			{
				case MixEnums.MixDataType.DateTime:
				case MixEnums.MixDataType.Date:
				case MixEnums.MixDataType.Time:
				case MixEnums.MixDataType.Duration:
				case MixEnums.MixDataType.PhoneNumber:
				{
					return;
				}
				case MixEnums.MixDataType.Double:
				{
					double.TryParse(defaultValue, out num);
					this.DoubleValue = this.DoubleValue;
					return;
				}
				default:
				{
					if (dataType == MixEnums.MixDataType.Boolean)
					{
						bool.TryParse(defaultValue, out flag);
						this.BooleanValue = new bool?(flag);
						return;
					}
					if (dataType != MixEnums.MixDataType.Integer)
					{
						return;
					}
					int.TryParse(defaultValue, out num1);
					this.IntegerValue = new int?(num1);
					return;
				}
			}
		}

		public override MixAttributeSetValue ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			int priority;
			MixEnums.MixDataType dataType;
			string name;
			int id;
			string defaultValue;
			if (string.IsNullOrEmpty(this.Id))
			{
				this.Id = Guid.NewGuid().ToString();
				this.CreatedDateTime = DateTime.UtcNow;
			}
			Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel field = this.Field;
			if (field != null)
			{
				priority = field.Priority;
			}
			else
			{
				priority = this.Priority;
			}
			this.Priority = priority;
			Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel updateViewModel = this.Field;
			if (updateViewModel != null)
			{
				dataType = updateViewModel.DataType;
			}
			else
			{
				dataType = this.DataType;
			}
			this.DataType = dataType;
			Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel field1 = this.Field;
			if (field1 != null)
			{
				name = field1.Name;
			}
			else
			{
				name = null;
			}
			this.AttributeFieldName = name;
			Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel updateViewModel1 = this.Field;
			if (updateViewModel1 != null)
			{
				id = updateViewModel1.Id;
			}
			else
			{
				id = 0;
			}
			this.AttributeFieldId = id;
			if (string.IsNullOrEmpty(this.StringValue))
			{
				Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel field2 = this.Field;
				if (field2 != null)
				{
					defaultValue = field2.DefaultValue;
				}
				else
				{
					defaultValue = null;
				}
				if (!string.IsNullOrEmpty(defaultValue))
				{
					this.ParseDefaultValue(this.Field.DefaultValue);
				}
			}
			return base.ParseModel(_context, _transaction);
		}

		public override void Validate(MixCmsContext _context, IDbContextTransaction _transaction)
		{
			base.Validate(_context, _transaction);
			if (base.get_IsValid() && this.Field != null)
			{
				if (this.Field.IsUnique)
				{
					if (_context.MixAttributeSetValue.Any<MixAttributeSetValue>((MixAttributeSetValue d) => d.Specificulture == this.Specificulture && d.StringValue == this.StringValue && d.Id != this.Id && d.DataId != this.DataId))
					{
						base.set_IsValid(false);
						base.get_Errors().Add(string.Concat(this.Field.Title, " = ", this.StringValue, " is existed"));
					}
				}
				if (this.Field.IsRequire && string.IsNullOrEmpty(this.StringValue))
				{
					base.set_IsValid(false);
					base.get_Errors().Add(string.Concat(this.Field.Title, " is required"));
				}
				if (!string.IsNullOrEmpty(this.Field.Regex) && !(new System.Text.RegularExpressions.Regex(this.Field.Regex, RegexOptions.IgnoreCase)).Match(this.StringValue).Success)
				{
					base.set_IsValid(false);
					base.get_Errors().Add(string.Concat(this.Field.Title, " is invalid"));
				}
			}
		}
	}
}