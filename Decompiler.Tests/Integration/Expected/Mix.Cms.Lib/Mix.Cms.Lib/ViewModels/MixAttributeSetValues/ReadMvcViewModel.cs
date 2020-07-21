using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels.MixAttributeSetValues
{
	public class ReadMvcViewModel : ViewModelBase<MixCmsContext, MixAttributeSetValue, Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ReadMvcViewModel>
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

		[JsonProperty("dataNavs")]
		public List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.ReadMvcViewModel> DataNavs
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

		public ReadMvcViewModel()
		{
			base();
			this.set_IsCache(false);
			return;
		}

		public ReadMvcViewModel(MixAttributeSetValue model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			base(model, _context, _transaction);
			this.set_IsCache(false);
			return;
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (this.get_DataType() == 23)
			{
				stackVariable4 = ViewModelBase<MixCmsContext, MixRelatedAttributeData, Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.ReadMvcViewModel>.Repository;
				V_0 = Expression.Parameter(Type.GetTypeFromHandle(// 
				// Current member / type: System.Void Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ReadMvcViewModel::ExpandView(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Exception in: System.Void ExpandView(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Specified method is not supported.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com

	}
}