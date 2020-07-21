using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels;
using Mix.Cms.Lib.ViewModels.MixAttributeFields;
using Mix.Cms.Lib.ViewModels.MixAttributeSetDatas;
using Mix.Cms.Lib.ViewModels.MixAttributeSets;
using Mix.Cms.Lib.ViewModels.MixAttributeSetValues;
using Mix.Cms.Lib.ViewModels.MixModuleDatas;
using Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas;
using Mix.Cms.Lib.ViewModels.MixTemplates;
using Mix.Cms.Lib.ViewModels.MixUrlAliases;
using Mix.Domain.Core.Models;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
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
				return MixService.GetConfig<int>("ThemeId", this.get_Specificulture());
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
		public PaginationModel<Mix.Cms.Lib.ViewModels.MixModuleDatas.ReadViewModel> Data
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
				stackVariable1 = new string[3];
				stackVariable1[0] = "Views/Shared/Templates";
				stackVariable1[1] = MixService.GetConfig<string>("ThemeName", this.get_Specificulture());
				stackVariable1[2] = 4.ToString();
				return CommonHelper.GetFullPath(stackVariable1);
			}
		}

		[JsonIgnore]
		public string EdmFolderType
		{
			get
			{
				return 4.ToString();
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
				stackVariable1 = new string[3];
				stackVariable1[0] = "Views/Shared/Templates";
				stackVariable1[1] = MixService.GetConfig<string>("ThemeName", this.get_Specificulture());
				stackVariable1[2] = 3.ToString();
				return CommonHelper.GetFullPath(stackVariable1);
			}
		}

		[JsonIgnore]
		public string FormFolderType
		{
			get
			{
				return 3.ToString();
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
				if (string.IsNullOrEmpty(this.get_Image()) || this.get_Image().IndexOf("http") != -1 || this.get_Image().get_Chars(0) == '/')
				{
					return this.get_Image();
				}
				stackVariable16 = new string[2];
				stackVariable16[0] = this.get_Domain();
				stackVariable16[1] = this.get_Image();
				return CommonHelper.GetFullPath(stackVariable16);
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
				stackVariable1 = new string[3];
				stackVariable1[0] = "Views/Shared/Templates";
				stackVariable1[1] = MixService.GetConfig<string>("ThemeName", this.get_Specificulture());
				stackVariable1[2] = this.get_ThemeFolderType();
				return CommonHelper.GetFullPath(stackVariable1);
			}
		}

		[JsonIgnore]
		public string TemplateFolderType
		{
			get
			{
				return 2.ToString();
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
				return 2.ToString();
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
				if (this.get_Thumbnail() == null || this.get_Thumbnail().IndexOf("http") != -1 || this.get_Thumbnail().get_Chars(0) == '/')
				{
					if (!string.IsNullOrEmpty(this.get_Thumbnail()))
					{
						return this.get_Thumbnail();
					}
					return this.get_ImageUrl();
				}
				stackVariable20 = new string[2];
				stackVariable20[0] = this.get_Domain();
				stackVariable20[1] = this.get_Thumbnail();
				return CommonHelper.GetFullPath(stackVariable20);
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
			this.u003cDatau003ek__BackingField = new PaginationModel<Mix.Cms.Lib.ViewModels.MixModuleDatas.ReadViewModel>();
			base();
			return;
		}

		public UpdateViewModel(MixModule model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.u003cDatau003ek__BackingField = new PaginationModel<Mix.Cms.Lib.ViewModels.MixModuleDatas.ReadViewModel>();
			base(model, _context, _transaction);
			return;
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0 = new Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel.u003cu003ec__DisplayClass170_0();
			V_0._context = _context;
			V_0.u003cu003e4__this = this;
			this.set_Cultures(Mix.Cms.Lib.ViewModels.MixModules.Helper.LoadCultures(this.get_Id(), this.get_Specificulture(), V_0._context, _transaction));
			this.get_Cultures().ForEach(new Action<SupportedCulture>(V_0, Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel.u003cu003ec__DisplayClass170_0.u003cExpandViewu003eb__0));
			this.set_Columns(new List<ModuleFieldViewModel>());
			if (!string.IsNullOrEmpty(this.get_Fields()))
			{
				stackVariable26 = JArray.Parse(this.get_Fields());
			}
			else
			{
				stackVariable26 = new JArray();
			}
			V_1 = stackVariable26.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					stackVariable32 = new ModuleFieldViewModel();
					stackVariable32.set_Name(CommonHelper.ParseJsonPropertyName(V_2.get_Item("name").ToString()));
					stackVariable40 = V_2.get_Item("title");
					if (stackVariable40 != null)
					{
						stackVariable41 = stackVariable40.ToString();
					}
					else
					{
						dummyVar0 = stackVariable40;
						stackVariable41 = null;
					}
					stackVariable32.set_Title(stackVariable41);
					if (V_2.get_Item("options") != null)
					{
						stackVariable48 = Newtonsoft.Json.Linq.Extensions.Value<JArray>(V_2.get_Item("options"));
					}
					else
					{
						stackVariable48 = new JArray();
					}
					stackVariable32.set_Options(stackVariable48);
					if (V_2.get_Item("priority") != null)
					{
						stackVariable55 = Newtonsoft.Json.Linq.Extensions.Value<int>(V_2.get_Item("priority"));
					}
					else
					{
						stackVariable55 = 0;
					}
					stackVariable32.set_Priority(stackVariable55);
					stackVariable32.set_DataType(JToken.op_Explicit(V_2.get_Item("dataType")));
					if (V_2.get_Item("width") != null)
					{
						stackVariable66 = Newtonsoft.Json.Linq.Extensions.Value<int>(V_2.get_Item("width"));
					}
					else
					{
						stackVariable66 = 3;
					}
					stackVariable32.set_Width(stackVariable66);
					if (V_2.get_Item("isUnique") != null)
					{
						stackVariable73 = Newtonsoft.Json.Linq.Extensions.Value<bool>(V_2.get_Item("isUnique"));
					}
					else
					{
						stackVariable73 = true;
					}
					stackVariable32.set_IsUnique(stackVariable73);
					if (V_2.get_Item("isRequired") != null)
					{
						stackVariable80 = Newtonsoft.Json.Linq.Extensions.Value<bool>(V_2.get_Item("isRequired"));
					}
					else
					{
						stackVariable80 = true;
					}
					stackVariable32.set_IsRequired(stackVariable80);
					if (V_2.get_Item("isDisplay") != null)
					{
						stackVariable87 = Newtonsoft.Json.Linq.Extensions.Value<bool>(V_2.get_Item("isDisplay"));
					}
					else
					{
						stackVariable87 = true;
					}
					stackVariable32.set_IsDisplay(stackVariable87);
					if (V_2.get_Item("isSelect") != null)
					{
						stackVariable94 = Newtonsoft.Json.Linq.Extensions.Value<bool>(V_2.get_Item("isSelect"));
					}
					else
					{
						stackVariable94 = false;
					}
					stackVariable32.set_IsSelect(stackVariable94);
					if (V_2.get_Item("isGroupBy") != null)
					{
						stackVariable101 = Newtonsoft.Json.Linq.Extensions.Value<bool>(V_2.get_Item("isGroupBy"));
					}
					else
					{
						stackVariable101 = false;
					}
					stackVariable32.set_IsGroupBy(stackVariable101);
					this.get_Columns().Add(stackVariable32);
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			this.LoadAttributes(V_0._context, _transaction);
			stackVariable110 = ViewModelBase<MixCmsContext, MixTemplate, Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel>.Repository;
			V_4 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel::ExpandView(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: System.Void ExpandView(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		private void LoadAttributeData(MixCmsContext context, IDbContextTransaction transaction)
		{
			stackVariable1 = ViewModelBase<MixCmsContext, MixRelatedAttributeData, Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel>.Repository;
			V_0 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel::LoadAttributeData(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: System.Void LoadAttributeData(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		private void LoadAttributeFields(MixCmsContext context, IDbContextTransaction transaction)
		{
			if (!string.IsNullOrEmpty(this.get_AttributeData().get_Id()))
			{
				this.set_Attributes(new List<Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel>());
				V_2 = this.get_AttributeData().get_Data().get_Values().GetEnumerator();
				try
				{
					while (V_2.MoveNext())
					{
						V_3 = V_2.get_Current();
						if (V_3.get_Field() == null)
						{
							continue;
						}
						this.get_Attributes().Add(V_3.get_Field());
					}
				}
				finally
				{
					V_2.Dispose();
				}
			}
			else
			{
				stackVariable21 = ViewModelBase<MixCmsContext, MixAttributeSet, Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel>.Repository;
				V_1 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
				// Current member / type: System.Void Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel::LoadAttributeFields(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Exception in: System.Void LoadAttributeFields(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Specified method is not supported.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com


		private void LoadAttributes(MixCmsContext _context, IDbContextTransaction _transaction)
		{
			this.LoadAttributeData(_context, _transaction);
			this.LoadAttributeFields(_context, _transaction);
			stackVariable7 = this.get_Attributes();
			stackVariable8 = Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel.u003cu003ec.u003cu003e9__175_2;
			if (stackVariable8 == null)
			{
				dummyVar0 = stackVariable8;
				stackVariable8 = new Func<Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel, int>(Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel.u003cu003ec.u003cu003e9, Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel.u003cu003ec.u003cLoadAttributesu003eb__175_2);
				Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel.u003cu003ec.u003cu003e9__175_2 = stackVariable8;
			}
			V_2 = Enumerable.OrderBy<Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel, int>(stackVariable7, stackVariable8).GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = new Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel.u003cu003ec__DisplayClass175_0();
					V_3.field = V_2.get_Current();
					V_4 = Enumerable.FirstOrDefault<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel>(this.get_AttributeData().get_Data().get_Values(), new Func<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel, bool>(V_3, Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel.u003cu003ec__DisplayClass175_0.u003cLoadAttributesu003eb__3));
					if (V_4 == null)
					{
						stackVariable33 = new MixAttributeSetValue();
						stackVariable33.set_AttributeFieldId(V_3.field.get_Id());
						V_4 = new Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel(stackVariable33, _context, _transaction);
						V_4.set_DataType(V_3.field.get_DataType());
						V_4.set_AttributeFieldName(V_3.field.get_Name());
						V_4.set_Priority(V_3.field.get_Priority());
						this.get_AttributeData().get_Data().get_Values().Add(V_4);
					}
					V_4.set_Priority(V_3.field.get_Priority());
					V_4.set_Field(V_3.field);
				}
			}
			finally
			{
				if (V_2 != null)
				{
					V_2.Dispose();
				}
			}
			stackVariable57 = ViewModelBase<MixCmsContext, MixRelatedAttributeData, Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel>.Repository;
			V_5 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel::LoadAttributes(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: System.Void LoadAttributes(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public void LoadData(int? postId = null, int? productId = null, int? pageId = null, int? pageSize = null, int? pageIndex = 0, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0 = new Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel.u003cu003ec__DisplayClass178_0();
			V_0.u003cu003e4__this = this;
			V_0.pageId = pageId;
			V_0.postId = postId;
			V_1 = new RepositoryResponse<PaginationModel<Mix.Cms.Lib.ViewModels.MixModuleDatas.ReadViewModel>>();
			switch (this.get_Type())
			{
				case 0:
				{
					stackVariable11 = ViewModelBase<MixCmsContext, MixModuleData, Mix.Cms.Lib.ViewModels.MixModuleDatas.ReadViewModel>.Repository;
					V_3 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
					// Current member / type: System.Void Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel::LoadData(System.Nullable`1<System.Int32>,System.Nullable`1<System.Int32>,System.Nullable`1<System.Int32>,System.Nullable`1<System.Int32>,System.Nullable`1<System.Int32>,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
					// Exception in: System.Void LoadData(System.Nullable<System.Int32>,System.Nullable<System.Int32>,System.Nullable<System.Int32>,System.Nullable<System.Int32>,System.Nullable<System.Int32>,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
					// Specified method is not supported.
					// 
					// mailto: JustDecompilePublicFeedback@telerik.com


		public override MixModule ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (this.get_Id() == 0)
			{
				stackVariable84 = ViewModelBase<MixCmsContext, MixModule, Mix.Cms.Lib.ViewModels.MixModules.ReadListItemViewModel>.Repository;
				V_1 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
				// Current member / type: Mix.Cms.Lib.Models.Cms.MixModule Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel::ParseModel(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Exception in: Mix.Cms.Lib.Models.Cms.MixModule ParseModel(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Specified method is not supported.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com


		public override Task<RepositoryResponse<MixModule>> RemoveModelAsync(bool isRemoveRelatedModels = false, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			return this.RemoveModelAsync(isRemoveRelatedModels, _context, _transaction);
		}

		private async Task<RepositoryResponse<bool>> SaveAttributeAsync(int parentId, MixCmsContext context, IDbContextTransaction transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.parentId = parentId;
			V_0.context = context;
			V_0.transaction = transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel.u003cSaveAttributeAsyncu003ed__173>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public static async Task<RepositoryResponse<JObject>> SaveByModuleName(string culture, string createdBy, string name, string formName, JObject obj, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0.culture = culture;
			V_0.createdBy = createdBy;
			V_0.name = name;
			V_0.formName = formName;
			V_0.obj = obj;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<JObject>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel.u003cSaveByModuleNameu003ed__174>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public override async Task<RepositoryResponse<bool>> SaveSubModelsAsync(MixModule parent, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0.u003cu003e4__this = this;
			V_0.parent = parent;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel.u003cSaveSubModelsAsyncu003ed__172>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public override void Validate(MixCmsContext _context, IDbContextTransaction _transaction)
		{
			this.Validate(_context, _transaction);
			if (this.get_IsValid() && this.get_Id() == 0)
			{
				this.set_IsValid(!ViewModelBase<MixCmsContext, MixModule, Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel>.Repository.CheckIsExists(new Func<MixModule, bool>(this, Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel.u003cValidateu003eb__168_0), _context, _transaction));
				if (!this.get_IsValid())
				{
					this.get_Errors().Add("Module Name Existed");
				}
			}
			return;
		}
	}
}