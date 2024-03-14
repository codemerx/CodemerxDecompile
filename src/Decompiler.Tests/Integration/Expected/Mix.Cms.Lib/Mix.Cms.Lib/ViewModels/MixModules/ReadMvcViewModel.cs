using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.Services;
using Mix.Cms.Lib.ViewModels;
using Mix.Cms.Lib.ViewModels.MixAttributeSetDatas;
using Mix.Cms.Lib.ViewModels.MixAttributeSets;
using Mix.Cms.Lib.ViewModels.MixModuleDatas;
using Mix.Cms.Lib.ViewModels.MixModulePosts;
using Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas;
using Mix.Cms.Lib.ViewModels.MixTemplates;
using Mix.Common.Helper;
using Mix.Domain.Core.Models;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels.MixModules
{
	public class ReadMvcViewModel : ViewModelBase<MixCmsContext, MixModule, Mix.Cms.Lib.ViewModels.MixModules.ReadMvcViewModel>
	{
		[JsonProperty("attributeData")]
		public Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.ReadMvcViewModel AttributeData
		{
			get;
			set;
		}

		[JsonProperty("columns")]
		public List<ModuleFieldViewModel> Columns
		{
			get
			{
				if (this.Fields == null)
				{
					return null;
				}
				return JsonConvert.DeserializeObject<List<ModuleFieldViewModel>>(this.Fields);
			}
			set
			{
				this.Fields = JsonConvert.SerializeObject(value);
			}
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
		public PaginationModel<Mix.Cms.Lib.ViewModels.MixModuleDatas.ReadViewModel> Data { get; set; } = new PaginationModel<Mix.Cms.Lib.ViewModels.MixModuleDatas.ReadViewModel>();

		[JsonProperty("description")]
		public string Description
		{
			get;
			set;
		}

		[JsonProperty("detailsUrl")]
		public string DetailsUrl
		{
			get;
			set;
		}

		[JsonProperty("domain")]
		public string Domain
		{
			get
			{
				return MixService.GetConfig<string>("Domain");
			}
		}

		[JsonProperty("edmTemplate")]
		public string EdmTemplate
		{
			get;
			set;
		}

		public string EdmTemplatePath
		{
			get
			{
				string[] config = new string[] { "", "Views/Shared/Templates", null, null };
				config[2] = MixService.GetConfig<string>("ThemeFolder", this.Specificulture) ?? "Default";
				config[3] = this.EdmTemplate;
				return CommonHelper.GetFullPath(config);
			}
		}

		[JsonProperty("edmView")]
		public Mix.Cms.Lib.ViewModels.MixTemplates.ReadListItemViewModel EdmView
		{
			get;
			set;
		}

		[JsonProperty("fields")]
		public string Fields
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

		public string FormTemplatePath
		{
			get
			{
				string[] config = new string[] { "", "Views/Shared/Templates", null, null };
				config[2] = MixService.GetConfig<string>("ThemeFolder", this.Specificulture) ?? "Default";
				config[3] = this.FormTemplate;
				return CommonHelper.GetFullPath(config);
			}
		}

		[JsonProperty("formView")]
		public Mix.Cms.Lib.ViewModels.MixTemplates.ReadListItemViewModel FormView
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

		[JsonProperty("imageUrl")]
		public string ImageUrl
		{
			get
			{
				if (string.IsNullOrEmpty(this.Image) || this.Image.IndexOf("http") != -1 || this.Image[0] == '/')
				{
					return this.Image;
				}
				return CommonHelper.GetFullPath(new string[] { this.Domain, this.Image });
			}
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

		public int? PageId
		{
			get;
			set;
		}

		[JsonProperty("pageSize")]
		public int? PageSize
		{
			get;
			set;
		}

		public int? PostId
		{
			get;
			set;
		}

		[JsonProperty("posts")]
		public PaginationModel<Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel> Posts { get; set; } = new PaginationModel<Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel>();

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

		[JsonProperty("template")]
		public string Template
		{
			get;
			set;
		}

		public string TemplatePath
		{
			get
			{
				string[] config = new string[] { "", "Views/Shared/Templates", null, null };
				config[2] = MixService.GetConfig<string>("ThemeFolder", this.Specificulture) ?? "Default";
				config[3] = this.Template;
				return CommonHelper.GetFullPath(config);
			}
		}

		[JsonProperty("thumbnail")]
		public string Thumbnail
		{
			get;
			set;
		}

		[JsonProperty("thumbnailUrl")]
		public string ThumbnailUrl
		{
			get
			{
				if (this.Thumbnail == null || this.Thumbnail.IndexOf("http") != -1 || this.Thumbnail[0] == '/')
				{
					if (!string.IsNullOrEmpty(this.Thumbnail))
					{
						return this.Thumbnail;
					}
					return this.ImageUrl;
				}
				return CommonHelper.GetFullPath(new string[] { this.Domain, this.Thumbnail });
			}
		}

		[JsonProperty("title")]
		public string Title
		{
			get;
			set;
		}

		[JsonProperty("type")]
		public MixEnums.MixModuleType Type
		{
			get;
			set;
		}

		[JsonProperty("view")]
		public Mix.Cms.Lib.ViewModels.MixTemplates.ReadListItemViewModel View
		{
			get;
			set;
		}

		public ReadMvcViewModel()
		{
		}

		public ReadMvcViewModel(MixModule model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.View = Mix.Cms.Lib.ViewModels.MixTemplates.ReadListItemViewModel.GetTemplateByPath(this.Template, this.Specificulture, _context, _transaction).get_Data();
			this.FormView = Mix.Cms.Lib.ViewModels.MixTemplates.ReadListItemViewModel.GetTemplateByPath(this.FormTemplate, this.Specificulture, _context, _transaction).get_Data();
			this.EdmView = Mix.Cms.Lib.ViewModels.MixTemplates.ReadListItemViewModel.GetTemplateByPath(this.EdmTemplate, this.Specificulture, _context, _transaction).get_Data();
			this.LoadAttributes(_context, _transaction);
		}

		public static RepositoryResponse<Mix.Cms.Lib.ViewModels.MixModules.ReadMvcViewModel> GetBy(Expression<Func<MixModule, bool>> predicate, int? postId = null, int? productid = null, int pageId = 0, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<Mix.Cms.Lib.ViewModels.MixModules.ReadMvcViewModel> singleModel = ViewModelBase<MixCmsContext, MixModule, Mix.Cms.Lib.ViewModels.MixModules.ReadMvcViewModel>.Repository.GetSingleModel(predicate, _context, _transaction);
			if (singleModel.get_IsSucceed())
			{
				singleModel.get_Data().PostId = postId;
				singleModel.get_Data().PageId = new int?(pageId);
				int? nullable = null;
				int? nullable1 = nullable;
				nullable = null;
				int? nullable2 = nullable;
				nullable = null;
				int? nullable3 = nullable;
				nullable = null;
				singleModel.get_Data().LoadData(nullable1, nullable2, nullable3, nullable, new int?(0), null, null);
			}
			return singleModel;
		}

		private void LoadAttributes(MixCmsContext _context, IDbContextTransaction _transaction)
		{
			Mix.Cms.Lib.ViewModels.MixModules.ReadMvcViewModel.u003cu003ec__DisplayClass134_0 variable = null;
			DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixAttributeSet, Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel>.Repository;
			ParameterExpression parameterExpression = Expression.Parameter(typeof(MixAttributeSet), "m");
			RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel> singleModel = repository.GetSingleModel(Expression.Lambda<Func<MixAttributeSet, bool>>(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSet).GetMethod("get_Name").MethodHandle)), Expression.Constant("sys_additional_field_module", typeof(string))), new ParameterExpression[] { parameterExpression }), _context, _transaction);
			if (singleModel.get_IsSucceed())
			{
				DefaultRepository<!0, !1, !2> defaultRepository = ViewModelBase<MixCmsContext, MixRelatedAttributeData, Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.ReadMvcViewModel>.Repository;
				parameterExpression = Expression.Parameter(typeof(MixRelatedAttributeData), "a");
				this.AttributeData = defaultRepository.GetFirstModel(Expression.Lambda<Func<MixRelatedAttributeData, bool>>(Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_ParentId").MethodHandle)), Expression.Call(Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixModules.ReadMvcViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixModules.ReadMvcViewModel).GetMethod("get_Id").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(int).GetMethod("ToString").MethodHandle), Array.Empty<Expression>())), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixModules.ReadMvcViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixModules.ReadMvcViewModel).GetMethod("get_Specificulture").MethodHandle)))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_AttributeSetId").MethodHandle)), Expression.Property(Expression.Property(Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.MixModules.ReadMvcViewModel.u003cu003ec__DisplayClass134_0)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixModules.ReadMvcViewModel.u003cu003ec__DisplayClass134_0).GetField("getAttrs").FieldHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel>).GetMethod("get_Data").MethodHandle, typeof(RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel>).TypeHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel).GetMethod("get_Id").MethodHandle)))), new ParameterExpression[] { parameterExpression }), _context, _transaction).get_Data();
			}
		}

		public void LoadData(int? postId = null, int? productId = null, int? pageId = null, int? pageSize = null, int? pageIndex = 0, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			MixCmsContext mixCmsContext = null;
			IDbContextTransaction dbContextTransaction = null;
			bool flag = false;
			UnitOfWorkHelper<MixCmsContext>.InitTransaction(_context, _transaction, ref mixCmsContext, ref dbContextTransaction, ref flag);
			try
			{
				try
				{
					int? nullable = pageSize;
					pageSize = (nullable.GetValueOrDefault() > 0 & nullable.HasValue ? pageSize : this.PageSize);
					nullable = pageIndex;
					pageIndex = (nullable.GetValueOrDefault() > 0 & nullable.HasValue ? pageIndex : new int?(0));
					Expression<Func<MixModuleData, bool>> moduleId = null;
					Expression<Func<MixModulePost, bool>> expression = null;
					switch (this.Type)
					{
						case MixEnums.MixModuleType.Content:
						case MixEnums.MixModuleType.Data:
						{
							moduleId = (MixModuleData m) => m.ModuleId == this.Id && m.Specificulture == this.Specificulture;
							break;
						}
						case MixEnums.MixModuleType.ListPost:
						{
							expression = (MixModulePost n) => n.ModuleId == this.Id && n.Specificulture == this.Specificulture;
							break;
						}
						case MixEnums.MixModuleType.SubPage:
						{
							moduleId = (MixModuleData m) => m.ModuleId == this.Id && m.Specificulture == this.Specificulture && m.PageId == pageId;
							expression = (MixModulePost n) => n.ModuleId == this.Id && n.Specificulture == this.Specificulture;
							break;
						}
						case MixEnums.MixModuleType.SubPost:
						{
							moduleId = (MixModuleData m) => m.ModuleId == this.Id && m.Specificulture == this.Specificulture && m.PostId == postId;
							break;
						}
						default:
						{
							moduleId = (MixModuleData m) => m.ModuleId == this.Id && m.Specificulture == this.Specificulture;
							expression = (MixModulePost n) => n.ModuleId == this.Id && n.Specificulture == this.Specificulture;
							break;
						}
					}
					if (moduleId != null)
					{
						RepositoryResponse<PaginationModel<Mix.Cms.Lib.ViewModels.MixModuleDatas.ReadViewModel>> modelListBy = ViewModelBase<MixCmsContext, MixModuleData, Mix.Cms.Lib.ViewModels.MixModuleDatas.ReadViewModel>.Repository.GetModelListBy(moduleId, MixService.GetConfig<string>("OrderBy"), 0, pageSize, pageIndex, mixCmsContext, dbContextTransaction);
						if (modelListBy.get_IsSucceed())
						{
							this.Data = modelListBy.get_Data();
						}
					}
					if (expression != null)
					{
						RepositoryResponse<PaginationModel<Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel>> repositoryResponse = ViewModelBase<MixCmsContext, MixModulePost, Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel>.Repository.GetModelListBy(expression, MixService.GetConfig<string>("OrderBy"), 0, pageSize, pageIndex, mixCmsContext, dbContextTransaction);
						if (repositoryResponse.get_IsSucceed())
						{
							this.Posts = repositoryResponse.get_Data();
						}
					}
				}
				catch (Exception exception)
				{
					UnitOfWorkHelper<MixCmsContext>.HandleException<PaginationModel<Mix.Cms.Lib.ViewModels.MixModules.ReadMvcViewModel>>(exception, flag, dbContextTransaction);
				}
			}
			finally
			{
				if (flag)
				{
					RelationalDatabaseFacadeExtensions.CloseConnection(mixCmsContext.get_Database());
					dbContextTransaction.Dispose();
					mixCmsContext.Dispose();
				}
			}
		}

		public T Property<T>(string fieldName)
		{
			T t;
			if (this.AttributeData == null)
			{
				t = default(T);
				return t;
			}
			JToken value = this.AttributeData.Data.Data.GetValue(fieldName);
			if (value != null)
			{
				return Newtonsoft.Json.Linq.Extensions.Value<T>(value);
			}
			t = default(T);
			return t;
		}
	}
}