using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixAttributeFields;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

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
			base();
			return;
		}

		public ImportViewModel(MixAttributeSetValue model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			base(model, _context, _transaction);
			return;
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (this.get_AttributeFieldId() <= 0)
			{
				stackVariable4 = new Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel();
				stackVariable4.set_DataType(this.get_DataType());
				stackVariable4.set_Id(this.get_AttributeFieldId());
				stackVariable4.set_Title(this.get_AttributeFieldName());
				stackVariable4.set_Name(this.get_AttributeFieldName());
				stackVariable4.set_Priority(this.get_Priority());
				this.set_Field(stackVariable4);
			}
			else
			{
				stackVariable16 = ViewModelBase<MixCmsContext, MixAttributeField, Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel>.Repository;
				V_0 = Expression.Parameter(Type.GetTypeFromHandle(// 
				// Current member / type: System.Void Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ImportViewModel::ExpandView(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Exception in: System.Void ExpandView(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Specified method is not supported.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com


		private void ParseDefaultValue(string defaultValue)
		{
			this.set_StringValue(defaultValue);
			V_3 = this.get_DataType();
			switch (V_3 - 1)
			{
				case 0:
				case 1:
				case 2:
				case 3:
				case 4:
				{
				Label0:
					return;
				}
				case 5:
				{
					dummyVar0 = double.TryParse(defaultValue, out V_0);
					this.set_DoubleValue(this.get_DoubleValue());
					return;
				}
				default:
				{
					if (V_3 == 18)
					{
						dummyVar1 = bool.TryParse(defaultValue, out V_1);
						this.set_BooleanValue(new bool?(V_1));
						return;
					}
					if (V_3 != 22)
					{
						return;
					}
					dummyVar2 = int.TryParse(defaultValue, out V_2);
					this.set_IntegerValue(new int?(V_2));
					goto Label0;
				}
			}
		}

		public override MixAttributeSetValue ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (string.IsNullOrEmpty(this.get_Id()))
			{
				this.set_Id(Guid.NewGuid().ToString());
				this.set_CreatedDateTime(DateTime.get_UtcNow());
			}
			stackVariable5 = this.get_Field();
			if (stackVariable5 != null)
			{
				stackVariable6 = stackVariable5.get_Priority();
			}
			else
			{
				dummyVar0 = stackVariable5;
				stackVariable6 = this.get_Priority();
			}
			this.set_Priority(stackVariable6);
			stackVariable9 = this.get_Field();
			if (stackVariable9 != null)
			{
				stackVariable10 = stackVariable9.get_DataType();
			}
			else
			{
				dummyVar1 = stackVariable9;
				stackVariable10 = this.get_DataType();
			}
			this.set_DataType(stackVariable10);
			stackVariable13 = this.get_Field();
			if (stackVariable13 != null)
			{
				stackVariable14 = stackVariable13.get_Name();
			}
			else
			{
				dummyVar2 = stackVariable13;
				stackVariable14 = null;
			}
			this.set_AttributeFieldName(stackVariable14);
			stackVariable17 = this.get_Field();
			if (stackVariable17 != null)
			{
				stackVariable18 = stackVariable17.get_Id();
			}
			else
			{
				dummyVar3 = stackVariable17;
				stackVariable18 = 0;
			}
			this.set_AttributeFieldId(stackVariable18);
			if (string.IsNullOrEmpty(this.get_StringValue()))
			{
				stackVariable27 = this.get_Field();
				if (stackVariable27 != null)
				{
					stackVariable28 = stackVariable27.get_DefaultValue();
				}
				else
				{
					dummyVar4 = stackVariable27;
					stackVariable28 = null;
				}
				if (!string.IsNullOrEmpty(stackVariable28))
				{
					this.ParseDefaultValue(this.get_Field().get_DefaultValue());
				}
			}
			return this.ParseModel(_context, _transaction);
		}

		public override void Validate(MixCmsContext _context, IDbContextTransaction _transaction)
		{
			this.Validate(_context, _transaction);
			if (this.get_IsValid() && this.get_Field() != null)
			{
				if (this.get_Field().get_IsUnique())
				{
					stackVariable48 = _context.get_MixAttributeSetValue();
					V_0 = Expression.Parameter(Type.GetTypeFromHandle(// 
					// Current member / type: System.Void Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ImportViewModel::Validate(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
					// Exception in: System.Void Validate(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
					// Specified method is not supported.
					// 
					// mailto: JustDecompilePublicFeedback@telerik.com

	}
}