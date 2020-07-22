using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixAttributeFields;
using Mix.Cms.Lib.ViewModels.MixAttributeSetDatas;
using Mix.Cms.Lib.ViewModels.MixAttributeSets;
using Mix.Cms.Lib.ViewModels.MixAttributeSetValues;
using Mix.Cms.Lib.ViewModels.MixModules;
using Mix.Cms.Lib.ViewModels.MixPageModules;
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
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixPages
{
	public class UpdateViewModel : ViewModelBase<MixCmsContext, MixPage, Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel>
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
		public Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel Attributes
		{
			get;
			set;
		}

		[JsonProperty("content")]
		public string Content
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

		[JsonProperty("cssClass")]
		public string CssClass
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

		[JsonProperty("excerpt")]
		public string Excerpt
		{
			get;
			set;
		}

		[JsonProperty("icon")]
		public string Icon
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

		[JsonProperty("imageFileStream")]
		public FileStreamViewModel ImageFileStream
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

		[JsonProperty("layout")]
		public string Layout
		{
			get;
			set;
		}

		[JsonProperty("level")]
		public int? Level
		{
			get;
			set;
		}

		[JsonProperty("listTag")]
		public JArray ListTag
		{
			get;
			set;
		}

		[JsonProperty("master")]
		public Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel Master
		{
			get;
			set;
		}

		[JsonProperty("masters")]
		public List<Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel> Masters
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

		[JsonProperty("moduleNavs")]
		public List<Mix.Cms.Lib.ViewModels.MixPageModules.ReadMvcViewModel> ModuleNavs
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

		[JsonProperty("priority")]
		public int Priority
		{
			get;
			set;
		}

		[JsonProperty("seoDescription")]
		public string SeoDescription
		{
			get;
			set;
		}

		[JsonProperty("seoKeywords")]
		public string SeoKeywords
		{
			get;
			set;
		}

		[JsonProperty("seoName")]
		public string SeoName
		{
			get;
			set;
		}

		[JsonProperty("seoTitle")]
		public string SeoTitle
		{
			get;
			set;
		}

		[JsonProperty("source")]
		public string Source
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

		[JsonProperty("staticUrl")]
		public string StaticUrl
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

		[JsonProperty("tags")]
		public string Tags
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
				stackVariable1[2] = this.get_TemplateFolderType();
				return CommonHelper.GetFullPath(stackVariable1);
			}
		}

		[JsonIgnore]
		public string TemplateFolderType
		{
			get
			{
				return 1.ToString();
			}
		}

		[JsonProperty("templates")]
		public List<Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel> Templates
		{
			get;
			set;
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
		[Required]
		public string Title
		{
			get;
			set;
		}

		[JsonProperty("type")]
		public MixEnums.MixPageType Type
		{
			get;
			set;
		}

		[JsonProperty("urlAliases")]
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

		[JsonProperty("views")]
		public int? Views
		{
			get;
			set;
		}

		public UpdateViewModel()
		{
			this.u003cListTagu003ek__BackingField = new JArray();
			base();
			return;
		}

		public UpdateViewModel(MixPage model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.u003cListTagu003ek__BackingField = new JArray();
			base(model, _context, _transaction);
			return;
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0 = new Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel.u003cu003ec__DisplayClass183_0();
			V_0.u003cu003e4__this = this;
			this.set_Cultures(Mix.Cms.Lib.ViewModels.MixPages.Helper.LoadCultures(this.get_Id(), this.get_Specificulture(), _context, _transaction));
			if (!string.IsNullOrEmpty(this.get_Tags()))
			{
				this.set_ListTag(JArray.Parse(this.get_Tags()));
			}
			this.LoadAttributes(_context, _transaction);
			stackVariable18 = ViewModelBase<MixCmsContext, MixTemplate, Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel>.Repository;
			V_1 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel::ExpandView(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: System.Void ExpandView(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		private void GenerateSEO()
		{
			V_0 = new Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel.u003cu003ec__DisplayClass188_0();
			V_0.u003cu003e4__this = this;
			if (string.IsNullOrEmpty(this.get_SeoName()))
			{
				this.set_SeoName(SeoHelper.GetSEOString(this.get_Title(), '-'));
			}
			V_1 = 1;
			V_0.name = this.get_SeoName();
			while (ViewModelBase<MixCmsContext, MixPage, Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel>.Repository.CheckIsExists(new Func<MixPage, bool>(V_0, Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel.u003cu003ec__DisplayClass188_0.u003cGenerateSEOu003eb__0), null, null))
			{
				V_0.name = string.Concat(this.get_SeoName(), "_", V_1.ToString());
				V_1 = V_1 + 1;
			}
			this.set_SeoName(V_0.name);
			if (string.IsNullOrEmpty(this.get_SeoTitle()))
			{
				this.set_SeoTitle(SeoHelper.GetSEOString(this.get_Title(), '-'));
			}
			if (string.IsNullOrEmpty(this.get_SeoDescription()))
			{
				this.set_SeoDescription(SeoHelper.GetSEOString(this.get_Title(), '-'));
			}
			if (string.IsNullOrEmpty(this.get_SeoKeywords()))
			{
				this.set_SeoKeywords(SeoHelper.GetSEOString(this.get_Title(), '-'));
			}
			return;
		}

		public List<Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel> GetAliases(MixCmsContext context, IDbContextTransaction transaction)
		{
			stackVariable0 = ViewModelBase<MixCmsContext, MixUrlAlias, Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel>.Repository;
			V_1 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
			// Current member / type: System.Collections.Generic.List`1<Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel> Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel::GetAliases(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: System.Collections.Generic.List<Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel> GetAliases(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public List<Mix.Cms.Lib.ViewModels.MixPageModules.ReadMvcViewModel> GetModuleNavs(MixCmsContext context, IDbContextTransaction transaction)
		{
			V_0 = new Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel.u003cu003ec__DisplayClass190_0();
			V_0.u003cu003e4__this = this;
			stackVariable3 = ViewModelBase<MixCmsContext, MixPageModule, Mix.Cms.Lib.ViewModels.MixPageModules.ReadMvcViewModel>.Repository;
			V_2 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
			// Current member / type: System.Collections.Generic.List`1<Mix.Cms.Lib.ViewModels.MixPageModules.ReadMvcViewModel> Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel::GetModuleNavs(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: System.Collections.Generic.List<Mix.Cms.Lib.ViewModels.MixPageModules.ReadMvcViewModel> GetModuleNavs(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		private void LoadAttributes(MixCmsContext _context, IDbContextTransaction _transaction)
		{
			stackVariable0 = ViewModelBase<MixCmsContext, MixAttributeSet, Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel>.Repository;
			V_1 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel::LoadAttributes(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: System.Void LoadAttributes(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public override MixPage ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.GenerateSEO();
			if (this.get_View() != null)
			{
				stackVariable14 = string.Concat(this.get_View().get_FolderType(), "/", this.get_View().get_FileName(), this.get_View().get_Extension());
			}
			else
			{
				stackVariable14 = this.get_Template();
			}
			this.set_Template(stackVariable14);
			if (this.get_Master() != null)
			{
				stackVariable28 = string.Concat(this.get_Master().get_FolderType(), "/", this.get_Master().get_FileName(), this.get_Master().get_Extension());
			}
			else
			{
				stackVariable28 = null;
			}
			this.set_Layout(stackVariable28);
			if (this.get_Id() == 0)
			{
				stackVariable65 = ViewModelBase<MixCmsContext, MixPage, Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel>.Repository;
				V_0 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
				// Current member / type: Mix.Cms.Lib.Models.Cms.MixPage Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel::ParseModel(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Exception in: Mix.Cms.Lib.Models.Cms.MixPage ParseModel(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Specified method is not supported.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com


		private async Task<RepositoryResponse<bool>> SaveAttributeAsync(int parentId, MixCmsContext context, IDbContextTransaction transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.parentId = parentId;
			V_0.context = context;
			V_0.transaction = transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel.u003cSaveAttributeAsyncu003ed__186>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public override RepositoryResponse<bool> SaveSubModels(MixPage parent, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			stackVariable0 = new RepositoryResponse<bool>();
			stackVariable0.set_IsSucceed(true);
			V_0 = stackVariable0;
			ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel>(this.get_View().SaveModel(true, _context, _transaction), ref V_0);
			if (V_0.get_IsSucceed() && this.get_Master() != null)
			{
				ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel>(this.get_Master().SaveModel(true, _context, _transaction), ref V_0);
			}
			if (V_0.get_IsSucceed() && this.get_UrlAliases() != null)
			{
				V_1 = this.get_UrlAliases().GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_2 = V_1.get_Current();
						if (!V_0.get_IsSucceed())
						{
							break;
						}
						V_2.set_SourceId(parent.get_Id().ToString());
						V_2.set_Type(0);
						V_2.set_Specificulture(this.get_Specificulture());
						ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel>(V_2.SaveModel(false, _context, _transaction), ref V_0);
					}
				}
				finally
				{
					V_1.Dispose();
				}
			}
			if (V_0.get_IsSucceed())
			{
				V_4 = this.get_ModuleNavs().GetEnumerator();
				try
				{
					while (V_4.MoveNext())
					{
						V_5 = V_4.get_Current();
						V_5.set_PageId(parent.get_Id());
						if (!V_5.get_IsActived())
						{
							ViewModelHelper.HandleResult<MixPageModule>(V_5.RemoveModel(false, _context, _transaction), ref V_0);
						}
						else
						{
							ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixPageModules.ReadMvcViewModel>(V_5.SaveModel(false, _context, _transaction), ref V_0);
						}
					}
				}
				finally
				{
					V_4.Dispose();
				}
			}
			return V_0;
		}

		public override async Task<RepositoryResponse<bool>> SaveSubModelsAsync(MixPage parent, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0.u003cu003e4__this = this;
			V_0.parent = parent;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel.u003cSaveSubModelsAsyncu003ed__185>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}