using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib.Models.Cms;
using Mix.Domain.Core.Models;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels.MixAttributeSetValues
{
	public class DeleteViewModel : ViewModelBase<MixCmsContext, MixAttributeSetValue, DeleteViewModel>
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

		[JsonProperty("dataType")]
		public int DataType
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
		public string Status
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

		public DeleteViewModel()
		{
			base.set_IsCache(false);
		}

		public DeleteViewModel(MixAttributeSetValue model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
			base.set_IsCache(false);
		}
	}
}