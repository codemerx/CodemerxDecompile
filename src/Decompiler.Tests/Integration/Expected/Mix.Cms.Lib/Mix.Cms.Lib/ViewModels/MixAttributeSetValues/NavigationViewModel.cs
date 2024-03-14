using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixAttributeFields;
using Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels.MixAttributeSetValues
{
	public class NavigationViewModel : ViewModelBase<MixCmsContext, MixAttributeSetValue, Mix.Cms.Lib.ViewModels.MixAttributeSetValues.NavigationViewModel>
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
		public List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.NavigationViewModel> DataNavs
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
		public Mix.Cms.Lib.ViewModels.MixAttributeFields.ReadViewModel Field
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

		public NavigationViewModel()
		{
		}

		public NavigationViewModel(MixAttributeSetValue model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			ParameterExpression parameterExpression;
			List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.NavigationViewModel> list;
			if (this.DataType == MixEnums.MixDataType.Reference)
			{
				DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixRelatedAttributeData, Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.NavigationViewModel>.Repository;
				parameterExpression = Expression.Parameter(typeof(MixRelatedAttributeData), "d");
				List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.NavigationViewModel> data = repository.GetModelListBy(Expression.Lambda<Func<MixRelatedAttributeData, bool>>(Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_ParentId").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetValues.NavigationViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetValues.NavigationViewModel).GetMethod("get_DataId").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_ParentType").MethodHandle)), Expression.Call(Expression.Constant(MixEnums.MixAttributeSetDataType.Set, typeof(MixEnums.MixAttributeSetDataType)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(object).GetMethod("ToString").MethodHandle), Array.Empty<Expression>()))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetValues.NavigationViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetValues.NavigationViewModel).GetMethod("get_Specificulture").MethodHandle)))), new ParameterExpression[] { parameterExpression }), _context, _transaction).get_Data();
				if (data != null)
				{
					list = (
						from m in data
						orderby m.Priority
						select m).ToList<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.NavigationViewModel>();
				}
				else
				{
					list = null;
				}
				this.DataNavs = list;
			}
			DefaultRepository<!0, !1, !2> defaultRepository = ViewModelBase<MixCmsContext, MixAttributeField, Mix.Cms.Lib.ViewModels.MixAttributeFields.ReadViewModel>.Repository;
			parameterExpression = Expression.Parameter(typeof(MixAttributeField), "f");
			this.Field = defaultRepository.GetSingleModel(Expression.Lambda<Func<MixAttributeField, bool>>(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeField).GetMethod("get_Id").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetValues.NavigationViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetValues.NavigationViewModel).GetMethod("get_AttributeFieldId").MethodHandle))), new ParameterExpression[] { parameterExpression }), _context, _transaction).get_Data();
		}

		public override MixAttributeSetValue ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (string.IsNullOrEmpty(this.Id))
			{
				this.Id = Guid.NewGuid().ToString();
				this.CreatedDateTime = DateTime.UtcNow;
			}
			this.Priority = this.Field.Priority;
			this.DataType = this.Field.DataType;
			return base.ParseModel(_context, _transaction);
		}

		public override void Validate(MixCmsContext _context, IDbContextTransaction _transaction)
		{
			base.Validate(_context, _transaction);
			if (base.get_IsValid())
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
			}
		}
	}
}