using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.Services;
using Mix.Cms.Lib.ViewModels;
using Mix.Cms.Lib.ViewModels.MixAttributeFields;
using Mix.Cms.Lib.ViewModels.MixAttributeSetDatas;
using Mix.Cms.Lib.ViewModels.MixAttributeSets;
using Mix.Cms.Lib.ViewModels.MixAttributeSetValues;
using Mix.Cms.Lib.ViewModels.MixModuleDatas;
using Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas;
using Mix.Cms.Lib.ViewModels.MixTemplates;
using Mix.Cms.Lib.ViewModels.MixUrlAliases;
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

namespace Mix.Cms.Lib.ViewModels.MixModules
{
	public class UpdateViewModel : ViewModelBase<MixCmsContext, MixModule, Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel>
	{
		[JsonIgnore]
		public int ActivedTheme
		{
			get
			{
				return MixService.GetConfig<int>("ThemeId", this.Specificulture);
			}
		}

		[JsonProperty("attributeData")]
		public Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel AttributeData
		{
			get;
			set;
		}

		[JsonProperty("attributes")]
		public List<Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel> Attributes
		{
			get;
			set;
		}

		[JsonProperty("attributeSet")]
		public Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel AttributeSet
		{
			get;
			set;
		}

		[JsonProperty("columns")]
		public List<ModuleFieldViewModel> Columns
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
		public PaginationModel<Mix.Cms.Lib.ViewModels.MixModuleDatas.ReadViewModel> Data { get; set; } = new PaginationModel<Mix.Cms.Lib.ViewModels.MixModuleDatas.ReadViewModel>();

		[JsonProperty("description")]
		public string Description
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

		[JsonProperty("edmFolder")]
		public string EdmFolder
		{
			get
			{
				return CommonHelper.GetFullPath(new string[] { "Views/Shared/Templates", MixService.GetConfig<string>("ThemeName", this.Specificulture), MixEnums.EnumTemplateFolder.Edms.ToString() });
			}
		}

		[JsonIgnore]
		public string EdmFolderType
		{
			get
			{
				return MixEnums.EnumTemplateFolder.Edms.ToString();
			}
		}

		[JsonProperty("edms")]
		public List<Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel> Edms
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

		[JsonProperty("edmView")]
		public Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel EdmView
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

		[JsonProperty("formFolder")]
		public string FormFolder
		{
			get
			{
				return CommonHelper.GetFullPath(new string[] { "Views/Shared/Templates", MixService.GetConfig<string>("ThemeName", this.Specificulture), MixEnums.EnumTemplateFolder.Forms.ToString() });
			}
		}

		[JsonIgnore]
		public string FormFolderType
		{
			get
			{
				return MixEnums.EnumTemplateFolder.Forms.ToString();
			}
		}

		[JsonProperty("forms")]
		public List<Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel> Forms
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

		[JsonProperty("formView")]
		public Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel FormView
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

		[JsonProperty("pageId")]
		public int PageId
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

		[JsonProperty("postId")]
		public string PostId
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

		[JsonProperty("sysCategories")]
		public List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel> SysCategories
		{
			get;
			set;
		}

		[JsonProperty("sysTags")]
		public List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel> SysTags
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

		[JsonProperty("templateFolder")]
		public string TemplateFolder
		{
			get
			{
				return CommonHelper.GetFullPath(new string[] { "Views/Shared/Templates", MixService.GetConfig<string>("ThemeName", this.Specificulture), this.ThemeFolderType });
			}
		}

		[JsonIgnore]
		public string TemplateFolderType
		{
			get
			{
				return MixEnums.EnumTemplateFolder.Modules.ToString();
			}
		}

		[JsonProperty("templates")]
		public List<Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel> Templates
		{
			get;
			set;
		}

		[JsonIgnore]
		public string ThemeFolderType
		{
			get
			{
				return MixEnums.EnumTemplateFolder.Modules.ToString();
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

		public List<Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel> UrlAliases
		{
			get;
			set;
		}

		[JsonProperty("view")]
		public Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel View
		{
			get;
			set;
		}

		public UpdateViewModel()
		{
		}

		public UpdateViewModel(MixModule model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			string str;
			object obj;
			string fileFolder;
			string fileName;
			string fileFolder1;
			string fileName1;
			string str1;
			string fileName2;
			this.Cultures = Mix.Cms.Lib.ViewModels.MixModules.Helper.LoadCultures(this.Id, this.Specificulture, _context, _transaction);
			this.Cultures.ForEach((SupportedCulture c) => {
				Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel.u003cu003ec__DisplayClass170_1 variable = null;
				SupportedCulture supportedCulture = c;
				DbSet<MixModule> mixModule = _context.MixModule;
				ParameterExpression parameterExpression = Expression.Parameter(typeof(MixModule), "m");
				supportedCulture.set_IsSupported(mixModule.Any<MixModule>(Expression.Lambda<Func<MixModule, bool>>(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixModule).GetMethod("get_Id").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel).GetMethod("get_Id").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixModule).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel.u003cu003ec__DisplayClass170_1)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel.u003cu003ec__DisplayClass170_1).GetField("c").FieldHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(SupportedCulture).GetMethod("get_Specificulture").MethodHandle)))), new ParameterExpression[] { parameterExpression })));
			});
			this.Columns = new List<ModuleFieldViewModel>();
			foreach (JToken jToken in (!string.IsNullOrEmpty(this.Fields) ? JArray.Parse(this.Fields) : new JArray()))
			{
				ModuleFieldViewModel moduleFieldViewModel = new ModuleFieldViewModel()
				{
					Name = CommonHelper.ParseJsonPropertyName(jToken.get_Item("name").ToString())
				};
				JToken item = jToken.get_Item("title");
				if (item != null)
				{
					str = item.ToString();
				}
				else
				{
					str = null;
				}
				moduleFieldViewModel.Title = str;
				moduleFieldViewModel.Options = (jToken.get_Item("options") != null ? Newtonsoft.Json.Linq.Extensions.Value<JArray>(jToken.get_Item("options")) : new JArray());
				moduleFieldViewModel.Priority = (jToken.get_Item("priority") != null ? Newtonsoft.Json.Linq.Extensions.Value<int>(jToken.get_Item("priority")) : 0);
				moduleFieldViewModel.DataType = (MixEnums.MixDataType)((int)jToken.get_Item("dataType"));
				moduleFieldViewModel.Width = (jToken.get_Item("width") != null ? Newtonsoft.Json.Linq.Extensions.Value<int>(jToken.get_Item("width")) : 3);
				moduleFieldViewModel.IsUnique = (jToken.get_Item("isUnique") != null ? Newtonsoft.Json.Linq.Extensions.Value<bool>(jToken.get_Item("isUnique")) : true);
				moduleFieldViewModel.IsRequired = (jToken.get_Item("isRequired") != null ? Newtonsoft.Json.Linq.Extensions.Value<bool>(jToken.get_Item("isRequired")) : true);
				moduleFieldViewModel.IsDisplay = (jToken.get_Item("isDisplay") != null ? Newtonsoft.Json.Linq.Extensions.Value<bool>(jToken.get_Item("isDisplay")) : true);
				moduleFieldViewModel.IsSelect = (jToken.get_Item("isSelect") != null ? Newtonsoft.Json.Linq.Extensions.Value<bool>(jToken.get_Item("isSelect")) : false);
				moduleFieldViewModel.IsGroupBy = (jToken.get_Item("isGroupBy") != null ? Newtonsoft.Json.Linq.Extensions.Value<bool>(jToken.get_Item("isGroupBy")) : false);
				this.Columns.Add(moduleFieldViewModel);
			}
			this.LoadAttributes(_context, _transaction);
			DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixTemplate, Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel>.Repository;
			ParameterExpression parameterExpression1 = Expression.Parameter(typeof(MixTemplate), "t");
			this.Templates = repository.GetModelListBy(Expression.Lambda<Func<MixTemplate, bool>>(Expression.AndAlso(Expression.Equal(Expression.Property(Expression.Property(parameterExpression1, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixTemplate).GetMethod("get_Theme").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixTheme).GetMethod("get_Id").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel).GetMethod("get_ActivedTheme").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression1, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixTemplate).GetMethod("get_FolderType").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel).GetMethod("get_TemplateFolderType").MethodHandle)))), new ParameterExpression[] { parameterExpression1 }), _context, _transaction).get_Data();
			string template = this.Template;
			if (template != null)
			{
				obj = template.Substring(this.Template.LastIndexOf('/') + 1);
			}
			else
			{
				obj = null;
			}
			if (obj == null)
			{
				obj = "_Blank.cshtml";
			}
			string str2 = (string)obj;
			this.View = this.Templates.FirstOrDefault<Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel>((Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel t) => {
				if (string.IsNullOrEmpty(str2))
				{
					return false;
				}
				return str2.Equals(string.Concat(t.FileName, t.Extension));
			});
			if (this.View == null)
			{
				this.View = this.Templates.FirstOrDefault<Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel>((Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel t) => "_Blank.cshtml".Equals(string.Concat(t.FileName, t.Extension)));
			}
			Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel view = this.View;
			if (view != null)
			{
				fileFolder = view.FileFolder;
			}
			else
			{
				fileFolder = null;
			}
			Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel updateViewModel = this.View;
			if (updateViewModel != null)
			{
				fileName = updateViewModel.FileName;
			}
			else
			{
				fileName = null;
			}
			this.Template = string.Concat(fileFolder, "/", fileName, this.View.Extension);
			DefaultRepository<!0, !1, !2> defaultRepository = ViewModelBase<MixCmsContext, MixTemplate, Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel>.Repository;
			parameterExpression1 = Expression.Parameter(typeof(MixTemplate), "t");
			this.Forms = defaultRepository.GetModelListBy(Expression.Lambda<Func<MixTemplate, bool>>(Expression.AndAlso(Expression.Equal(Expression.Property(Expression.Property(parameterExpression1, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixTemplate).GetMethod("get_Theme").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixTheme).GetMethod("get_Id").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel).GetMethod("get_ActivedTheme").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression1, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixTemplate).GetMethod("get_FolderType").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel).GetMethod("get_FormFolderType").MethodHandle)))), new ParameterExpression[] { parameterExpression1 }), null, null).get_Data();
			this.FormView = Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel.GetTemplateByPath(this.FormTemplate, this.Specificulture, MixEnums.EnumTemplateFolder.Forms, _context, _transaction);
			Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel formView = this.FormView;
			if (formView != null)
			{
				fileFolder1 = formView.FileFolder;
			}
			else
			{
				fileFolder1 = null;
			}
			Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel formView1 = this.FormView;
			if (formView1 != null)
			{
				fileName1 = formView1.FileName;
			}
			else
			{
				fileName1 = null;
			}
			this.FormTemplate = string.Concat(fileFolder1, "/", fileName1, this.View.Extension);
			DefaultRepository<!0, !1, !2> repository1 = ViewModelBase<MixCmsContext, MixTemplate, Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel>.Repository;
			parameterExpression1 = Expression.Parameter(typeof(MixTemplate), "t");
			this.Edms = repository1.GetModelListBy(Expression.Lambda<Func<MixTemplate, bool>>(Expression.AndAlso(Expression.Equal(Expression.Property(Expression.Property(parameterExpression1, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixTemplate).GetMethod("get_Theme").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixTheme).GetMethod("get_Id").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel).GetMethod("get_ActivedTheme").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression1, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixTemplate).GetMethod("get_FolderType").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel).GetMethod("get_EdmFolderType").MethodHandle)))), new ParameterExpression[] { parameterExpression1 }), null, null).get_Data();
			this.EdmView = Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel.GetTemplateByPath(this.EdmTemplate, this.Specificulture, MixEnums.EnumTemplateFolder.Edms, _context, _transaction);
			Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel edmView = this.EdmView;
			if (edmView != null)
			{
				str1 = edmView.FileFolder;
			}
			else
			{
				str1 = null;
			}
			Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel edmView1 = this.EdmView;
			if (edmView1 != null)
			{
				fileName2 = edmView1.FileName;
			}
			else
			{
				fileName2 = null;
			}
			this.EdmTemplate = string.Concat(str1, "/", fileName2, this.View.Extension);
		}

		private void LoadAttributeData(MixCmsContext context, IDbContextTransaction transaction)
		{
			DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixRelatedAttributeData, Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel>.Repository;
			ParameterExpression parameterExpression = Expression.Parameter(typeof(MixRelatedAttributeData), "a");
			this.AttributeData = repository.GetFirstModel(Expression.Lambda<Func<MixRelatedAttributeData, bool>>(Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_ParentId").MethodHandle)), Expression.Call(Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel).GetMethod("get_Id").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(int).GetMethod("ToString").MethodHandle), Array.Empty<Expression>())), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel).GetMethod("get_Specificulture").MethodHandle)))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_ParentType").MethodHandle)), Expression.Call(Expression.Constant(MixEnums.MixAttributeSetDataType.Module, typeof(MixEnums.MixAttributeSetDataType)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(object).GetMethod("ToString").MethodHandle), Array.Empty<Expression>()))), new ParameterExpression[] { parameterExpression }), context, transaction).get_Data();
			if (this.AttributeData == null)
			{
				this.AttributeData = new Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel(new MixRelatedAttributeData()
				{
					Specificulture = this.Specificulture,
					ParentType = MixEnums.MixAttributeSetDataType.Module.ToString(),
					ParentId = this.Id.ToString()
				}, null, null)
				{
					Data = new Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel(new MixAttributeSetData()
					{
						Specificulture = this.Specificulture
					}, null, null)
				};
			}
		}

		private void LoadAttributeFields(MixCmsContext context, IDbContextTransaction transaction)
		{
			if (!string.IsNullOrEmpty(this.AttributeData.Id))
			{
				this.Attributes = new List<Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel>();
				foreach (Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel value in this.AttributeData.Data.Values)
				{
					if (value.Field == null)
					{
						continue;
					}
					this.Attributes.Add(value.Field);
				}
			}
			else
			{
				RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel> singleModel = ViewModelBase<MixCmsContext, MixAttributeSet, Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel>.Repository.GetSingleModel((MixAttributeSet m) => m.Name == "sys_additional_field_module", context, transaction);
				if (singleModel.get_IsSucceed())
				{
					this.Attributes = singleModel.get_Data().Fields;
					return;
				}
			}
		}

		private void LoadAttributes(MixCmsContext _context, IDbContextTransaction _transaction)
		{
			this.LoadAttributeData(_context, _transaction);
			this.LoadAttributeFields(_context, _transaction);
			foreach (Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel updateViewModel in 
				from f in this.Attributes
				orderby f.Priority
				select f)
			{
				Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel priority = this.AttributeData.Data.Values.FirstOrDefault<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel>((Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel v) => v.AttributeFieldId == updateViewModel.Id);
				if (priority == null)
				{
					priority = new Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel(new MixAttributeSetValue()
					{
						AttributeFieldId = updateViewModel.Id
					}, _context, _transaction)
					{
						DataType = updateViewModel.DataType,
						AttributeFieldName = updateViewModel.Name,
						Priority = updateViewModel.Priority
					};
					this.AttributeData.Data.Values.Add(priority);
				}
				priority.Priority = updateViewModel.Priority;
				priority.Field = updateViewModel;
			}
			DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixRelatedAttributeData, Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel>.Repository;
			ParameterExpression parameterExpression = Expression.Parameter(typeof(MixRelatedAttributeData), "m");
			RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel>> modelListBy = repository.GetModelListBy(Expression.Lambda<Func<MixRelatedAttributeData, bool>>(Expression.AndAlso(Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel).GetMethod("get_Specificulture").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_ParentId").MethodHandle)), Expression.Call(Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel).GetMethod("get_Id").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(int).GetMethod("ToString").MethodHandle), Array.Empty<Expression>()))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_ParentType").MethodHandle)), Expression.Call(Expression.Constant(MixEnums.MixAttributeSetDataType.Module, typeof(MixEnums.MixAttributeSetDataType)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(object).GetMethod("ToString").MethodHandle), Array.Empty<Expression>()))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_AttributeSetName").MethodHandle)), Expression.Constant("sys_category", typeof(string)))), new ParameterExpression[] { parameterExpression }), _context, _transaction);
			if (modelListBy.get_IsSucceed())
			{
				this.SysCategories = modelListBy.get_Data();
			}
			DefaultRepository<!0, !1, !2> defaultRepository = ViewModelBase<MixCmsContext, MixRelatedAttributeData, Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel>.Repository;
			parameterExpression = Expression.Parameter(typeof(MixRelatedAttributeData), "m");
			RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel>> repositoryResponse = defaultRepository.GetModelListBy(Expression.Lambda<Func<MixRelatedAttributeData, bool>>(Expression.AndAlso(Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel).GetMethod("get_Specificulture").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_ParentId").MethodHandle)), Expression.Call(Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel).GetMethod("get_Id").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(int).GetMethod("ToString").MethodHandle), Array.Empty<Expression>()))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_ParentType").MethodHandle)), Expression.Call(Expression.Constant(MixEnums.MixAttributeSetDataType.Module, typeof(MixEnums.MixAttributeSetDataType)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(object).GetMethod("ToString").MethodHandle), Array.Empty<Expression>()))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_AttributeSetName").MethodHandle)), Expression.Constant("sys_tag", typeof(string)))), new ParameterExpression[] { parameterExpression }), _context, _transaction);
			if (repositoryResponse.get_IsSucceed())
			{
				this.SysTags = repositoryResponse.get_Data();
			}
		}

		public void LoadData(int? postId = null, int? productId = null, int? pageId = null, int? pageSize = null, int? pageIndex = 0, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<PaginationModel<Mix.Cms.Lib.ViewModels.MixModuleDatas.ReadViewModel>> repositoryResponse = new RepositoryResponse<PaginationModel<Mix.Cms.Lib.ViewModels.MixModuleDatas.ReadViewModel>>();
			switch (this.Type)
			{
				case MixEnums.MixModuleType.Content:
				{
					repositoryResponse = ViewModelBase<MixCmsContext, MixModuleData, Mix.Cms.Lib.ViewModels.MixModuleDatas.ReadViewModel>.Repository.GetModelListBy((MixModuleData m) => m.ModuleId == this.Id && m.Specificulture == this.Specificulture, "Priority", 0, pageSize, pageIndex, _context, _transaction);
					if (repositoryResponse.get_IsSucceed())
					{
						this.Data = repositoryResponse.get_Data();
					}
					return;
				}
				case MixEnums.MixModuleType.Data:
				case MixEnums.MixModuleType.ListPost:
				{
					if (repositoryResponse.get_IsSucceed())
					{
						this.Data = repositoryResponse.get_Data();
					}
					return;
				}
				case MixEnums.MixModuleType.SubPage:
				{
					repositoryResponse = ViewModelBase<MixCmsContext, MixModuleData, Mix.Cms.Lib.ViewModels.MixModuleDatas.ReadViewModel>.Repository.GetModelListBy((MixModuleData m) => m.ModuleId == this.Id && m.Specificulture == this.Specificulture && m.PageId == pageId, "Priority", 0, pageSize, pageIndex, _context, _transaction);
					if (repositoryResponse.get_IsSucceed())
					{
						this.Data = repositoryResponse.get_Data();
					}
					return;
				}
				case MixEnums.MixModuleType.SubPost:
				{
					repositoryResponse = ViewModelBase<MixCmsContext, MixModuleData, Mix.Cms.Lib.ViewModels.MixModuleDatas.ReadViewModel>.Repository.GetModelListBy((MixModuleData m) => m.ModuleId == this.Id && m.Specificulture == this.Specificulture && m.PostId == postId, "Priority", 0, pageSize, pageIndex, _context, _transaction);
					if (repositoryResponse.get_IsSucceed())
					{
						this.Data = repositoryResponse.get_Data();
					}
					return;
				}
				default:
				{
					if (repositoryResponse.get_IsSucceed())
					{
						this.Data = repositoryResponse.get_Data();
					}
					return;
				}
			}
		}

		public override MixModule ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (this.Id == 0)
			{
				this.Id = ViewModelBase<MixCmsContext, MixModule, Mix.Cms.Lib.ViewModels.MixModules.ReadListItemViewModel>.Repository.Max((MixModule m) => m.Id, _context, _transaction).get_Data() + 1;
				this.LastModified = new DateTime?(DateTime.UtcNow);
				this.CreatedDateTime = DateTime.UtcNow;
			}
			this.Template = (this.View != null ? string.Format("{0}/{1}{2}", this.View.FolderType, this.View.FileName, this.View.Extension) : this.Template);
			this.FormTemplate = (this.FormView != null ? string.Format("{0}/{1}{2}", this.FormView.FolderType, this.FormView.FileName, this.FormView.Extension) : this.FormTemplate);
			this.EdmTemplate = (this.EdmView != null ? string.Format("{0}/{1}{2}", this.EdmView.FolderType, this.EdmView.FileName, this.EdmView.Extension) : this.EdmTemplate);
			this.Fields = ((this.Columns != null ? JArray.Parse(JsonConvert.SerializeObject(
				from c in this.Columns
				orderby c.Priority
				where !string.IsNullOrEmpty(c.Name)
				select c)) : new JArray())).ToString(0, Array.Empty<JsonConverter>());
			if (!string.IsNullOrEmpty(this.Image) && this.Image[0] == '/')
			{
				this.Image = this.Image.Substring(1);
			}
			return base.ParseModel(_context, _transaction);
		}

		public override Task<RepositoryResponse<MixModule>> RemoveModelAsync(bool isRemoveRelatedModels = false, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			return base.RemoveModelAsync(isRemoveRelatedModels, _context, _transaction);
		}

		private async Task<RepositoryResponse<bool>> SaveAttributeAsync(int parentId, MixCmsContext context, IDbContextTransaction transaction)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixAttributeSet, Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel>.Repository;
			RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel> singleModel = repository.GetSingleModel((MixAttributeSet m) => m.Name == "sys_additional_field_module", context, transaction);
			if (singleModel.get_IsSucceed())
			{
				this.Attributes = singleModel.get_Data().Fields;
				this.AttributeData.AttributeSetId = singleModel.get_Data().Id;
				this.AttributeData.AttributeSetName = singleModel.get_Data().Name;
				this.AttributeData.Data.AttributeSetId = singleModel.get_Data().Id;
				this.AttributeData.Data.AttributeSetName = singleModel.get_Data().Name;
				this.AttributeData.ParentId = parentId.ToString();
				this.AttributeData.ParentType = MixEnums.MixAttributeSetDataType.Module;
				RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel> repositoryResponse2 = await this.AttributeData.Data.SaveModelAsync(true, context, transaction);
				ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel>(repositoryResponse2, ref repositoryResponse1);
				if (repositoryResponse1.get_IsSucceed())
				{
					this.AttributeData.DataId = repositoryResponse2.get_Data().Id;
					ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel>(await this.AttributeData.SaveModelAsync(true, context, transaction), ref repositoryResponse1);
				}
			}
			foreach (Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel sysCategory in this.SysCategories)
			{
				if (!repositoryResponse1.get_IsSucceed())
				{
					continue;
				}
				sysCategory.ParentId = parentId.ToString();
				sysCategory.ParentType = MixEnums.MixAttributeSetDataType.Module;
				sysCategory.Specificulture = this.Specificulture;
				ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel>(await sysCategory.SaveModelAsync(false, context, transaction), ref repositoryResponse1);
			}
			foreach (Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel sysTag in this.SysTags)
			{
				if (!repositoryResponse1.get_IsSucceed())
				{
					continue;
				}
				sysTag.ParentId = parentId.ToString();
				sysTag.ParentType = MixEnums.MixAttributeSetDataType.Module;
				sysTag.Specificulture = this.Specificulture;
				ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel>(await sysTag.SaveModelAsync(false, context, transaction), ref repositoryResponse1);
			}
			return repositoryResponse1;
		}

		public static async Task<RepositoryResponse<JObject>> SaveByModuleName(string culture, string createdBy, string name, string formName, JObject obj, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<JObject> repositoryResponse;
			string str;
			JObject data;
			MixCmsContext mixCmsContext = null;
			IDbContextTransaction dbContextTransaction = null;
			bool flag = false;
			Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel.u003cu003ec__DisplayClass174_1 variable = null;
			UnitOfWorkHelper<MixCmsContext>.InitTransaction(_context, _transaction, ref mixCmsContext, ref dbContextTransaction, ref flag);
			try
			{
				try
				{
					DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixModule, Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel>.Repository;
					ParameterExpression parameterExpression = Expression.Parameter(typeof(MixModule), "m");
					BinaryExpression binaryExpression = Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixModule).GetMethod("get_Specificulture").MethodHandle)), Expression.Field(Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel.u003cu003ec__DisplayClass174_1)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel.u003cu003ec__DisplayClass174_1).GetField("CS$<>8__locals1").FieldHandle)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel.u003cu003ec__DisplayClass174_0).GetField("culture").FieldHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixModule).GetMethod("get_Name").MethodHandle)), Expression.Field(Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel.u003cu003ec__DisplayClass174_1)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel.u003cu003ec__DisplayClass174_1).GetField("CS$<>8__locals1").FieldHandle)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel.u003cu003ec__DisplayClass174_0).GetField("name").FieldHandle))));
					ParameterExpression[] parameterExpressionArray = new ParameterExpression[] { parameterExpression };
					RepositoryResponse<Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel> singleModelAsync = await repository.GetSingleModelAsync(Expression.Lambda<Func<MixModule, bool>>(binaryExpression, parameterExpressionArray), mixCmsContext, dbContextTransaction);
					RepositoryResponse<Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel> repositoryResponse1 = singleModelAsync;
					JToken item = obj.get_Item("id");
					if (item != null)
					{
						str = Newtonsoft.Json.Linq.Extensions.Value<string>(item);
					}
					else
					{
						str = null;
					}
					string str1 = str;
					if (!repositoryResponse1.get_IsSucceed())
					{
						RepositoryResponse<JObject> repositoryResponse2 = new RepositoryResponse<JObject>();
						repositoryResponse2.set_IsSucceed(false);
						repositoryResponse2.set_Status(0x190);
						repositoryResponse = repositoryResponse2;
					}
					else
					{
						DefaultRepository<!0, !1, !2> defaultRepository = ViewModelBase<MixCmsContext, MixAttributeSet, Mix.Cms.Lib.ViewModels.MixAttributeSets.ReadViewModel>.Repository;
						parameterExpression = Expression.Parameter(typeof(MixAttributeSet), "m");
						BinaryExpression binaryExpression1 = Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSet).GetMethod("get_Name").MethodHandle)), Expression.Field(Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel.u003cu003ec__DisplayClass174_1)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel.u003cu003ec__DisplayClass174_1).GetField("CS$<>8__locals1").FieldHandle)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel.u003cu003ec__DisplayClass174_0).GetField("formName").FieldHandle)));
						ParameterExpression[] parameterExpressionArray1 = new ParameterExpression[] { parameterExpression };
						RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSets.ReadViewModel> singleModelAsync1 = await defaultRepository.GetSingleModelAsync(Expression.Lambda<Func<MixAttributeSet, bool>>(binaryExpression1, parameterExpressionArray1), mixCmsContext, dbContextTransaction);
						if (!singleModelAsync1.get_IsSucceed())
						{
							RepositoryResponse<JObject> repositoryResponse3 = new RepositoryResponse<JObject>();
							repositoryResponse3.set_IsSucceed(false);
							repositoryResponse3.set_Status(0x190);
							repositoryResponse = repositoryResponse3;
						}
						else
						{
							Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel updateViewModel = new Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel()
							{
								Id = str1,
								CreatedBy = createdBy,
								AttributeSetId = singleModelAsync1.get_Data().Id,
								AttributeSetName = singleModelAsync1.get_Data().Name,
								Specificulture = culture,
								Data = obj
							};
							Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel updateViewModel1 = updateViewModel;
							DefaultRepository<!0, !1, !2> repository1 = ViewModelBase<MixCmsContext, MixRelatedAttributeData, Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.ReadViewModel>.Repository;
							parameterExpression = Expression.Parameter(typeof(MixRelatedAttributeData), "m");
							BinaryExpression binaryExpression2 = Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_ParentId").MethodHandle)), Expression.Call(Expression.Property(Expression.Property(Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel.u003cu003ec__DisplayClass174_1)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel.u003cu003ec__DisplayClass174_1).GetField("getModule").FieldHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(RepositoryResponse<Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel>).GetMethod("get_Data").MethodHandle, typeof(RepositoryResponse<Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel>).TypeHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel).GetMethod("get_Id").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(int).GetMethod("ToString").MethodHandle), Array.Empty<Expression>())), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_ParentType").MethodHandle)), Expression.Call(Expression.Constant(MixEnums.MixAttributeSetDataType.Module, typeof(MixEnums.MixAttributeSetDataType)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(object).GetMethod("ToString").MethodHandle), Array.Empty<Expression>()))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_Specificulture").MethodHandle)), Expression.Field(Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel.u003cu003ec__DisplayClass174_1)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel.u003cu003ec__DisplayClass174_1).GetField("CS$<>8__locals1").FieldHandle)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel.u003cu003ec__DisplayClass174_0).GetField("culture").FieldHandle))));
							ParameterExpression[] parameterExpressionArray2 = new ParameterExpression[] { parameterExpression };
							if (!await repository1.GetSingleModelAsync(Expression.Lambda<Func<MixRelatedAttributeData, bool>>(binaryExpression2, parameterExpressionArray2), mixCmsContext, dbContextTransaction).get_IsSucceed())
							{
								List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel> relatedData = updateViewModel1.RelatedData;
								Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel updateViewModel2 = new Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel()
								{
									ParentId = repositoryResponse1.get_Data().Id.ToString(),
									Specificulture = culture,
									ParentType = MixEnums.MixAttributeSetDataType.Module
								};
								relatedData.Add(updateViewModel2);
							}
							RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel> repositoryResponse4 = await updateViewModel1.SaveModelAsync(true, mixCmsContext, dbContextTransaction);
							UnitOfWorkHelper<MixCmsContext>.HandleTransaction(repositoryResponse4.get_IsSucceed(), flag, dbContextTransaction);
							RepositoryResponse<JObject> repositoryResponse5 = new RepositoryResponse<JObject>();
							repositoryResponse5.set_IsSucceed(repositoryResponse4.get_IsSucceed());
							Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel data1 = repositoryResponse4.get_Data();
							if (data1 != null)
							{
								data = data1.Data;
							}
							else
							{
								data = null;
							}
							repositoryResponse5.set_Data(data);
							repositoryResponse5.set_Exception(repositoryResponse4.get_Exception());
							repositoryResponse5.set_Errors(repositoryResponse4.get_Errors());
							repositoryResponse = repositoryResponse5;
						}
					}
				}
				catch (Exception exception)
				{
					repositoryResponse = UnitOfWorkHelper<MixCmsContext>.HandleException<JObject>(exception, flag, dbContextTransaction);
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
			return repositoryResponse;
		}

		public override async Task<RepositoryResponse<bool>> SaveSubModelsAsync(MixModule parent, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel>(await this.View.SaveModelAsync(true, _context, _transaction), ref repositoryResponse1);
			if (repositoryResponse1.get_IsSucceed() && !string.IsNullOrEmpty(this.FormView.Content))
			{
				ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel>(await this.FormView.SaveModelAsync(true, _context, _transaction), ref repositoryResponse1);
			}
			if (repositoryResponse1.get_IsSucceed() && !string.IsNullOrEmpty(this.EdmView.Content))
			{
				ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel>(await this.EdmView.SaveModelAsync(true, _context, _transaction), ref repositoryResponse1);
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				ViewModelHelper.HandleResult<bool>(await this.SaveAttributeAsync(parent.Id, _context, _transaction), ref repositoryResponse1);
			}
			return repositoryResponse1;
		}

		public override void Validate(MixCmsContext _context, IDbContextTransaction _transaction)
		{
			base.Validate(_context, _transaction);
			if (base.get_IsValid() && this.Id == 0)
			{
				base.set_IsValid(!ViewModelBase<MixCmsContext, MixModule, Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel>.Repository.CheckIsExists((MixModule m) => {
					if (m.Name != this.Name)
					{
						return false;
					}
					return m.Specificulture == this.Specificulture;
				}, _context, _transaction));
				if (!base.get_IsValid())
				{
					base.get_Errors().Add("Module Name Existed");
				}
			}
		}
	}
}