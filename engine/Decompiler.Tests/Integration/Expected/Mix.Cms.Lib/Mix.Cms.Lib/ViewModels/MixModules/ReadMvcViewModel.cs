using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels;
using Mix.Cms.Lib.ViewModels.MixAttributeSets;
using Mix.Cms.Lib.ViewModels.MixModuleDatas;
using Mix.Cms.Lib.ViewModels.MixModulePosts;
using Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas;
using Mix.Cms.Lib.ViewModels.MixTemplates;
using Mix.Domain.Core.Models;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
				if (this.get_Fields() == null)
				{
					return null;
				}
				return JsonConvert.DeserializeObject<List<ModuleFieldViewModel>>(this.get_Fields());
			}
			set
			{
				this.set_Fields(JsonConvert.SerializeObject(value));
				return;
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
				stackVariable1 = new string[4];
				stackVariable1[0] = "";
				stackVariable1[1] = "Views/Shared/Templates";
				stackVariable10 = MixService.GetConfig<string>("ThemeFolder", this.get_Specificulture());
				if (stackVariable10 == null)
				{
					dummyVar0 = stackVariable10;
					stackVariable10 = "Default";
				}
				stackVariable1[2] = stackVariable10;
				stackVariable1[3] = this.get_EdmTemplate();
				return CommonHelper.GetFullPath(stackVariable1);
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
				stackVariable1 = new string[4];
				stackVariable1[0] = "";
				stackVariable1[1] = "Views/Shared/Templates";
				stackVariable10 = MixService.GetConfig<string>("ThemeFolder", this.get_Specificulture());
				if (stackVariable10 == null)
				{
					dummyVar0 = stackVariable10;
					stackVariable10 = "Default";
				}
				stackVariable1[2] = stackVariable10;
				stackVariable1[3] = this.get_FormTemplate();
				return CommonHelper.GetFullPath(stackVariable1);
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
		public PaginationModel<Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel> Posts
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
				stackVariable1 = new string[4];
				stackVariable1[0] = "";
				stackVariable1[1] = "Views/Shared/Templates";
				stackVariable10 = MixService.GetConfig<string>("ThemeFolder", this.get_Specificulture());
				if (stackVariable10 == null)
				{
					dummyVar0 = stackVariable10;
					stackVariable10 = "Default";
				}
				stackVariable1[2] = stackVariable10;
				stackVariable1[3] = this.get_Template();
				return CommonHelper.GetFullPath(stackVariable1);
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

		[JsonProperty("view")]
		public Mix.Cms.Lib.ViewModels.MixTemplates.ReadListItemViewModel View
		{
			get;
			set;
		}

		public ReadMvcViewModel()
		{
			this.u003cDatau003ek__BackingField = new PaginationModel<Mix.Cms.Lib.ViewModels.MixModuleDatas.ReadViewModel>();
			this.u003cPostsu003ek__BackingField = new PaginationModel<Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel>();
			base();
			return;
		}

		public ReadMvcViewModel(MixModule model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.u003cDatau003ek__BackingField = new PaginationModel<Mix.Cms.Lib.ViewModels.MixModuleDatas.ReadViewModel>();
			this.u003cPostsu003ek__BackingField = new PaginationModel<Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel>();
			base(model, _context, _transaction);
			return;
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.set_View(Mix.Cms.Lib.ViewModels.MixTemplates.ReadListItemViewModel.GetTemplateByPath(this.get_Template(), this.get_Specificulture(), _context, _transaction).get_Data());
			this.set_FormView(Mix.Cms.Lib.ViewModels.MixTemplates.ReadListItemViewModel.GetTemplateByPath(this.get_FormTemplate(), this.get_Specificulture(), _context, _transaction).get_Data());
			this.set_EdmView(Mix.Cms.Lib.ViewModels.MixTemplates.ReadListItemViewModel.GetTemplateByPath(this.get_EdmTemplate(), this.get_Specificulture(), _context, _transaction).get_Data());
			this.LoadAttributes(_context, _transaction);
			return;
		}

		public static RepositoryResponse<Mix.Cms.Lib.ViewModels.MixModules.ReadMvcViewModel> GetBy(Expression<Func<MixModule, bool>> predicate, int? postId = null, int? productid = null, int pageId = 0, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0 = ViewModelBase<MixCmsContext, MixModule, Mix.Cms.Lib.ViewModels.MixModules.ReadMvcViewModel>.Repository.GetSingleModel(predicate, _context, _transaction);
			if (V_0.get_IsSucceed())
			{
				V_0.get_Data().set_PostId(postId);
				V_0.get_Data().set_PageId(new int?(pageId));
				V_1 = null;
				stackVariable18 = V_1;
				V_1 = null;
				stackVariable20 = V_1;
				V_1 = null;
				stackVariable22 = V_1;
				V_1 = null;
				V_0.get_Data().LoadData(stackVariable18, stackVariable20, stackVariable22, V_1, new int?(0), null, null);
			}
			return V_0;
		}

		private void LoadAttributes(MixCmsContext _context, IDbContextTransaction _transaction)
		{
			V_0 = new Mix.Cms.Lib.ViewModels.MixModules.ReadMvcViewModel.u003cu003ec__DisplayClass134_0();
			V_0.u003cu003e4__this = this;
			stackVariable4 = ViewModelBase<MixCmsContext, MixAttributeSet, Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel>.Repository;
			V_1 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Mix.Cms.Lib.ViewModels.MixModules.ReadMvcViewModel::LoadAttributes(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: System.Void LoadAttributes(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public void LoadData(int? postId = null, int? productId = null, int? pageId = null, int? pageSize = null, int? pageIndex = 0, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0 = new Mix.Cms.Lib.ViewModels.MixModules.ReadMvcViewModel.u003cu003ec__DisplayClass136_0();
			V_0.u003cu003e4__this = this;
			V_0.pageId = pageId;
			V_0.postId = postId;
			UnitOfWorkHelper<MixCmsContext>.InitTransaction(_context, _transaction, ref V_1, ref V_2, ref V_3);
			try
			{
				try
				{
					V_6 = pageSize;
					if (V_6.GetValueOrDefault() > 0 & V_6.get_HasValue())
					{
						stackVariable21 = pageSize;
					}
					else
					{
						stackVariable21 = this.get_PageSize();
					}
					pageSize = stackVariable21;
					V_6 = pageIndex;
					if (V_6.GetValueOrDefault() > 0 & V_6.get_HasValue())
					{
						stackVariable31 = pageIndex;
					}
					else
					{
						stackVariable31 = new int?(0);
					}
					pageIndex = stackVariable31;
					V_4 = null;
					V_5 = null;
					switch (this.get_Type())
					{
						case 0:
						case 1:
						{
							V_9 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
							// Current member / type: System.Void Mix.Cms.Lib.ViewModels.MixModules.ReadMvcViewModel::LoadData(System.Nullable`1<System.Int32>,System.Nullable`1<System.Int32>,System.Nullable`1<System.Int32>,System.Nullable`1<System.Int32>,System.Nullable`1<System.Int32>,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
							// Exception in: System.Void LoadData(System.Nullable<System.Int32>,System.Nullable<System.Int32>,System.Nullable<System.Int32>,System.Nullable<System.Int32>,System.Nullable<System.Int32>,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
							// Specified method is not supported.
							// 
							// mailto: JustDecompilePublicFeedback@telerik.com


		public T Property<T>(string fieldName)
		{
			if (this.get_AttributeData() == null)
			{
				V_1 = default(T);
				return V_1;
			}
			V_0 = this.get_AttributeData().get_Data().get_Data().GetValue(fieldName);
			if (V_0 != null)
			{
				return Newtonsoft.Json.Linq.Extensions.Value<T>(V_0);
			}
			V_1 = default(T);
			return V_1;
		}
	}
}