using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixAttributeFields;
using Mix.Cms.Lib.ViewModels.MixAttributeSets;
using Mix.Cms.Lib.ViewModels.MixAttributeSetValues;
using Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas;
using Mix.Common.Helper;
using Mix.Domain.Core.Models;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixAttributeSetDatas
{
	public class UpdateViewModel : ViewModelBase<MixCmsContext, MixAttributeSetData, Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel>
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

		[JsonProperty("data")]
		public JObject Data
		{
			get;
			set;
		}

		[JsonProperty("dataNavs")]
		public List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel> DataNavs
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

		[JsonProperty("relatedData")]
		public List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel> RelatedData { get; set; } = new List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel>();

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
		public List<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel> Values
		{
			get;
			set;
		}

		public UpdateViewModel()
		{
		}

		public UpdateViewModel(MixAttributeSetData model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixRelatedAttributeData, Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel>.Repository;
			ParameterExpression parameterExpression = Expression.Parameter(typeof(MixRelatedAttributeData), "n");
			this.DataNavs = repository.GetModelListBy(Expression.Lambda<Func<MixRelatedAttributeData, bool>>(Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_ParentId").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel).GetMethod("get_Id").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_ParentType").MethodHandle)), Expression.Call(Expression.Constant(MixEnums.MixAttributeSetDataType.Set, typeof(MixEnums.MixAttributeSetDataType)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(object).GetMethod("ToString").MethodHandle), Array.Empty<Expression>()))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel).GetMethod("get_Specificulture").MethodHandle)))), new ParameterExpression[] { parameterExpression }), _context, _transaction).get_Data();
			DefaultRepository<!0, !1, !2> defaultRepository = ViewModelBase<MixCmsContext, MixAttributeSetValue, Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel>.Repository;
			parameterExpression = Expression.Parameter(typeof(MixAttributeSetValue), "a");
			this.Values = (
				from a in defaultRepository.GetModelListBy(Expression.Lambda<Func<MixAttributeSetValue, bool>>(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSetValue).GetMethod("get_DataId").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel).GetMethod("get_Id").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSetValue).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel).GetMethod("get_Specificulture").MethodHandle)))), new ParameterExpression[] { parameterExpression }), _context, _transaction).get_Data()
				orderby a.Priority
				select a).ToList<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel>();
			DefaultRepository<!0, !1, !2> repository1 = ViewModelBase<MixCmsContext, MixAttributeField, Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel>.Repository;
			parameterExpression = Expression.Parameter(typeof(MixAttributeField), "f");
			this.Fields = repository1.GetModelListBy(Expression.Lambda<Func<MixAttributeField, bool>>(Expression.OrElse(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeField).GetMethod("get_AttributeSetId").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel).GetMethod("get_AttributeSetId").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeField).GetMethod("get_AttributeSetName").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel).GetMethod("get_AttributeSetName").MethodHandle)))), new ParameterExpression[] { parameterExpression }), _context, _transaction).get_Data();
			foreach (Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel updateViewModel in 
				from f in this.Fields
				orderby f.Priority
				select f)
			{
				Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel attributeSetName = this.Values.FirstOrDefault<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel>((Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel v) => v.AttributeFieldId == updateViewModel.Id);
				if (attributeSetName == null)
				{
					attributeSetName = new Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel(new MixAttributeSetValue()
					{
						AttributeFieldId = updateViewModel.Id
					}, _context, _transaction)
					{
						Field = updateViewModel,
						AttributeFieldName = updateViewModel.Name,
						StringValue = updateViewModel.DefaultValue,
						Priority = updateViewModel.Priority
					};
					this.Values.Add(attributeSetName);
				}
				attributeSetName.AttributeSetName = this.AttributeSetName;
				attributeSetName.Priority = updateViewModel.Priority;
				attributeSetName.Field = updateViewModel;
				attributeSetName.DataType = attributeSetName.Field.DataType;
				Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel updateViewModel1 = attributeSetName;
				object attributeFieldName = attributeSetName.AttributeFieldName;
				if (attributeFieldName == null)
				{
					Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel field = attributeSetName.Field;
					if (field != null)
					{
						attributeFieldName = field.Name;
					}
					else
					{
						attributeFieldName = null;
					}
				}
				updateViewModel1.AttributeFieldName = (string)attributeFieldName;
			}
		}

		private void ParseData()
		{
			JObject jObject = new JObject();
			jObject.Add(new JProperty("id", this.Id));
			jObject.Add(new JProperty("specificulture", this.Specificulture));
			jObject.Add(new JProperty("createdDateTime", (object)this.CreatedDateTime));
			this.Data = jObject;
			foreach (Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel updateViewModel in 
				from v in this.Values
				orderby v.Priority
				select v)
			{
				this.Data.Add(this.ParseValue(updateViewModel));
			}
		}

		public override MixAttributeSetData ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			string name;
			int id;
			if (string.IsNullOrEmpty(this.Id))
			{
				this.Id = Guid.NewGuid().ToString();
				this.CreatedDateTime = DateTime.UtcNow;
				if (this.AttributeSetId == 0 && !string.IsNullOrEmpty(this.AttributeSetName))
				{
					Mix.Cms.Lib.ViewModels.MixAttributeSets.ReadViewModel data = ViewModelBase<MixCmsContext, MixAttributeSet, Mix.Cms.Lib.ViewModels.MixAttributeSets.ReadViewModel>.Repository.GetSingleModel((MixAttributeSet m) => m.Name == this.AttributeSetName, _context, _transaction).get_Data();
					if (data != null)
					{
						id = data.Id;
					}
					else
					{
						id = 0;
					}
					this.AttributeSetId = id;
				}
				else if (this.AttributeSetId > 0 && string.IsNullOrEmpty(this.AttributeSetName))
				{
					Mix.Cms.Lib.ViewModels.MixAttributeSets.ReadViewModel readViewModel = ViewModelBase<MixCmsContext, MixAttributeSet, Mix.Cms.Lib.ViewModels.MixAttributeSets.ReadViewModel>.Repository.GetSingleModel((MixAttributeSet m) => m.Name == this.AttributeSetName, _context, _transaction).get_Data();
					if (readViewModel != null)
					{
						name = readViewModel.Name;
					}
					else
					{
						name = null;
					}
					this.AttributeSetName = name;
				}
			}
			return base.ParseModel(_context, _transaction);
		}

		private JProperty ParseValue(Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel item)
		{
			switch (item.DataType)
			{
				case MixEnums.MixDataType.Custom:
				case MixEnums.MixDataType.Duration:
				case MixEnums.MixDataType.PhoneNumber:
				case MixEnums.MixDataType.Text:
				case MixEnums.MixDataType.Html:
				case MixEnums.MixDataType.MultilineText:
				case MixEnums.MixDataType.EmailAddress:
				case MixEnums.MixDataType.Password:
				case MixEnums.MixDataType.Url:
				case MixEnums.MixDataType.ImageUrl:
				case MixEnums.MixDataType.CreditCard:
				case MixEnums.MixDataType.PostalCode:
				case MixEnums.MixDataType.Upload:
				case MixEnums.MixDataType.Color:
				case MixEnums.MixDataType.Icon:
				case MixEnums.MixDataType.VideoYoutube:
				case MixEnums.MixDataType.TuiEditor:
				{
					return new JProperty(item.AttributeFieldName, item.StringValue);
				}
				case MixEnums.MixDataType.DateTime:
				{
					return new JProperty(item.AttributeFieldName, (object)item.DateTimeValue);
				}
				case MixEnums.MixDataType.Date:
				{
					return new JProperty(item.AttributeFieldName, (object)item.DateTimeValue);
				}
				case MixEnums.MixDataType.Time:
				{
					return new JProperty(item.AttributeFieldName, (object)item.DateTimeValue);
				}
				case MixEnums.MixDataType.Double:
				{
					return new JProperty(item.AttributeFieldName, (object)item.DoubleValue);
				}
				case MixEnums.MixDataType.Boolean:
				{
					return new JProperty(item.AttributeFieldName, (object)item.BooleanValue);
				}
				case MixEnums.MixDataType.Integer:
				{
					return new JProperty(item.AttributeFieldName, (object)item.IntegerValue);
				}
				case MixEnums.MixDataType.Reference:
				{
					return new JProperty(item.AttributeFieldName, new JArray());
				}
				default:
				{
					return new JProperty(item.AttributeFieldName, item.StringValue);
				}
			}
		}

		public override async Task<RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel>> SaveModelAsync(bool isSaveSubModels = false, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel.u003cSaveModelAsyncu003ed__76 variable = new Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel.u003cSaveModelAsyncu003ed__76();
			variable.u003cu003e4__this = this;
			variable.isSaveSubModels = isSaveSubModels;
			variable._context = _context;
			variable._transaction = _transaction;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel>>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel.u003cSaveModelAsyncu003ed__76>(ref variable);
			return variable.u003cu003et__builder.Task;
		}

		private async Task<RepositoryResponse<bool>> SaveRelatedDataAsync(MixAttributeSetData parent, MixCmsContext context, IDbContextTransaction transaction)
		{
			Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel.u003cSaveRelatedDataAsyncu003ed__79 variable = new Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel.u003cSaveRelatedDataAsyncu003ed__79();
			variable.u003cu003e4__this = this;
			variable.parent = parent;
			variable.context = context;
			variable.transaction = transaction;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel.u003cSaveRelatedDataAsyncu003ed__79>(ref variable);
			return variable.u003cu003et__builder.Task;
		}

		public override RepositoryResponse<bool> SaveSubModels(MixAttributeSetData parent, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			int priority;
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			if (repositoryResponse1.get_IsSucceed())
			{
				foreach (Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel value in this.Values)
				{
					if (!repositoryResponse1.get_IsSucceed())
					{
						break;
					}
					value.Field = this.Fields.Find((Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel f) => f.Name == value.AttributeFieldName);
					Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel updateViewModel = value;
					Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel field = value.Field;
					if (field != null)
					{
						priority = field.Priority;
					}
					else
					{
						priority = value.Priority;
					}
					updateViewModel.Priority = priority;
					value.DataId = parent.Id;
					value.Specificulture = parent.Specificulture;
					ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel>(value.SaveModel(false, _context, _transaction), ref repositoryResponse1);
				}
			}
			return repositoryResponse1;
		}

		public override async Task<RepositoryResponse<bool>> SaveSubModelsAsync(MixAttributeSetData parent, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel.u003cSaveSubModelsAsyncu003ed__77 variable = new Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel.u003cSaveSubModelsAsyncu003ed__77();
			variable.u003cu003e4__this = this;
			variable.parent = parent;
			variable._context = _context;
			variable._transaction = _transaction;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel.u003cSaveSubModelsAsyncu003ed__77>(ref variable);
			return variable.u003cu003et__builder.Task;
		}
	}
}