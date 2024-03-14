using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.Services;
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

namespace Mix.Cms.Lib.ViewModels.MixAttributeSets
{
	public class ReadMvcViewModel : ViewModelBase<MixCmsContext, MixAttributeSet, Mix.Cms.Lib.ViewModels.MixAttributeSets.ReadMvcViewModel>
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

		public PaginationModel<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.ReadMvcViewModel> Data
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

		[JsonProperty("edmAutoSend")]
		public bool? EdmAutoSend
		{
			get;
			set;
		}

		[JsonProperty("edmFrom")]
		public string EdmFrom
		{
			get;
			set;
		}

		[JsonProperty("edmSubject")]
		public string EdmSubject
		{
			get;
			set;
		}

		[JsonProperty("edmTemplate")]
		public string EdmTemplate
		{
			get;
			set;
		}

		[JsonProperty("fields")]
		public List<Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel> Fields
		{
			get;
			set;
		}

		[JsonProperty("formTemplate")]
		public string FormTemplate
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

		[JsonProperty("priority")]
		public int Priority
		{
			get;
			set;
		}

		[JsonProperty("ReferenceId")]
		public int? ReferenceId
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

		[JsonProperty("type")]
		public int? Type
		{
			get;
			set;
		}

		public ReadMvcViewModel()
		{
		}

		public ReadMvcViewModel(MixAttributeSet model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			object fields = this.Fields;
			if (fields == null)
			{
				List<Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel> data = ViewModelBase<MixCmsContext, MixAttributeField, Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel>.Repository.GetModelListBy((MixAttributeField a) => a.AttributeSetId == this.Id, _context, _transaction).get_Data();
				if (data != null)
				{
					fields = (
						from a in data
						orderby a.Priority
						select a).ToList<Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel>();
				}
				else
				{
					fields = null;
				}
			}
			this.Fields = (List<Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel>)fields;
		}

		public void LoadData(string parentId, MixEnums.MixAttributeSetDataType parentType, string specificulture, int? pageSize = null, int? pageIndex = 0, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			Mix.Cms.Lib.ViewModels.MixAttributeSets.ReadMvcViewModel.u003cu003ec__DisplayClass79_0 variable = null;
			DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixRelatedAttributeData, Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.ReadMvcViewModel>.Repository;
			ParameterExpression parameterExpression = Expression.Parameter(typeof(MixRelatedAttributeData), "m");
			RepositoryResponse<PaginationModel<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.ReadMvcViewModel>> modelListBy = repository.GetModelListBy(Expression.Lambda<Func<MixRelatedAttributeData, bool>>(Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_ParentId").MethodHandle)), Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSets.ReadMvcViewModel.u003cu003ec__DisplayClass79_0)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSets.ReadMvcViewModel.u003cu003ec__DisplayClass79_0).GetField("parentId").FieldHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_ParentType").MethodHandle)), Expression.Call(Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSets.ReadMvcViewModel.u003cu003ec__DisplayClass79_0)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSets.ReadMvcViewModel.u003cu003ec__DisplayClass79_0).GetField("parentType").FieldHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(object).GetMethod("ToString").MethodHandle), Array.Empty<Expression>()))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_Specificulture").MethodHandle)), Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSets.ReadMvcViewModel.u003cu003ec__DisplayClass79_0)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSets.ReadMvcViewModel.u003cu003ec__DisplayClass79_0).GetField("specificulture").FieldHandle)))), new ParameterExpression[] { parameterExpression }), MixService.GetConfig<string>("OrderBy"), 0, pageSize, pageIndex, _context, _transaction);
			this.Data = modelListBy.get_Data();
		}
	}
}