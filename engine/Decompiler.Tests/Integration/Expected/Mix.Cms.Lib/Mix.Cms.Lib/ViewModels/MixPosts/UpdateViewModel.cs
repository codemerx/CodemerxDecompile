using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels;
using Mix.Cms.Lib.ViewModels.MixAttributeFields;
using Mix.Cms.Lib.ViewModels.MixAttributeSetDatas;
using Mix.Cms.Lib.ViewModels.MixAttributeSets;
using Mix.Cms.Lib.ViewModels.MixAttributeSetValues;
using Mix.Cms.Lib.ViewModels.MixCultures;
using Mix.Cms.Lib.ViewModels.MixModulePosts;
using Mix.Cms.Lib.ViewModels.MixModules;
using Mix.Cms.Lib.ViewModels.MixPagePosts;
using Mix.Cms.Lib.ViewModels.MixPostMedias;
using Mix.Cms.Lib.ViewModels.MixPostModules;
using Mix.Cms.Lib.ViewModels.MixPostPosts;
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
using System.Threading;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixPosts
{
	public class UpdateViewModel : ViewModelBase<MixCmsContext, MixPost, Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel>
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

		[JsonProperty("columns")]
		public List<ModuleFieldViewModel> Columns
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

		[JsonIgnore]
		[JsonProperty("extraFields")]
		public string ExtraFields
		{
			get;
			set;
		}

		[JsonIgnore]
		[JsonProperty("extraProperties")]
		public string ExtraProperties
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

		[JsonProperty("listTag")]
		public JArray ListTag
		{
			get;
			set;
		}

		[JsonProperty("mediaNavs")]
		public List<Mix.Cms.Lib.ViewModels.MixPostMedias.ReadViewModel> MediaNavs
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
		public List<Mix.Cms.Lib.ViewModels.MixPostModules.ReadViewModel> ModuleNavs
		{
			get;
			set;
		}

		[JsonProperty("modules")]
		public List<Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel> Modules
		{
			get;
			set;
		}

		[JsonProperty("categories")]
		public List<Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel> Pages
		{
			get;
			set;
		}

		[JsonProperty("postNavs")]
		public List<Mix.Cms.Lib.ViewModels.MixPostPosts.ReadViewModel> PostNavs
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

		[JsonProperty("properties")]
		public List<ExtraProperty> Properties
		{
			get;
			set;
		}

		[JsonProperty("publishedDateTime")]
		public DateTime? PublishedDateTime
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

		[JsonProperty("status")]
		public MixEnums.MixContentStatus Status
		{
			get;
			set;
		}

		[JsonProperty("sysCategories")]
		public List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.FormViewModel> SysCategories
		{
			get;
			set;
		}

		[JsonProperty("sysTags")]
		public List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.FormViewModel> SysTags
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
				return 5.ToString();
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

		[JsonProperty("thumbnailFileStream")]
		public FileStreamViewModel ThumbnailFileStream
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
		public MixEnums.MixContentStatus Type
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
			this.u003cExtraFieldsu003ek__BackingField = "[]";
			this.u003cExtraPropertiesu003ek__BackingField = "[]";
			this.u003cTagsu003ek__BackingField = "[]";
			this.u003cListTagu003ek__BackingField = new JArray();
			base();
			return;
		}

		public UpdateViewModel(MixPost model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.u003cExtraFieldsu003ek__BackingField = "[]";
			this.u003cExtraPropertiesu003ek__BackingField = "[]";
			this.u003cTagsu003ek__BackingField = "[]";
			this.u003cListTagu003ek__BackingField = new JArray();
			base(model, _context, _transaction);
			return;
		}

		public override Task<RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel>>> CloneAsync(MixPost model, List<SupportedCulture> cloneCultures, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			return this.CloneAsync(model, cloneCultures, _context, _transaction);
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (this.get_Id() == 0)
			{
				this.set_ExtraFields(MixService.GetConfig<string>("DefaultPostAttr"));
			}
			this.set_Cultures(this.LoadCultures(this.get_Specificulture(), _context, _transaction));
			this.set_UrlAliases(this.GetAliases(_context, _transaction));
			if (!string.IsNullOrEmpty(this.get_Tags()))
			{
				this.set_ListTag(JArray.Parse(this.get_Tags()));
			}
			this.LoadAttributes(_context, _transaction);
			this.LoadTemplates(_context, _transaction);
			this.LoadParentPage(_context, _transaction);
			this.LoadParentModules(_context, _transaction);
			this.LoadMedias(_context, _transaction);
			this.LoadRelatedPost(_context, _transaction);
			return;
		}

		private void GenerateSEO()
		{
			V_0 = new Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel.u003cu003ec__DisplayClass220_0();
			V_0.u003cu003e4__this = this;
			if (string.IsNullOrEmpty(this.get_SeoName()))
			{
				this.set_SeoName(SeoHelper.GetSEOString(this.get_Title(), '-'));
			}
			V_1 = 1;
			V_0.name = this.get_SeoName();
			while (ViewModelBase<MixCmsContext, MixPost, Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel>.Repository.CheckIsExists(new Func<MixPost, bool>(V_0.u003cGenerateSEOu003eb__0), null, null))
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
			// Current member / type: System.Collections.Generic.List`1<Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel> Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel::GetAliases(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: System.Collections.Generic.List<Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel> GetAliases(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public List<Mix.Cms.Lib.ViewModels.MixPostPosts.ReadViewModel> GetRelated(MixCmsContext context, IDbContextTransaction transaction)
		{
			stackVariable0 = ViewModelBase<MixCmsContext, MixRelatedPost, Mix.Cms.Lib.ViewModels.MixPostPosts.ReadViewModel>.Repository;
			V_0 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
			// Current member / type: System.Collections.Generic.List`1<Mix.Cms.Lib.ViewModels.MixPostPosts.ReadViewModel> Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel::GetRelated(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: System.Collections.Generic.List<Mix.Cms.Lib.ViewModels.MixPostPosts.ReadViewModel> GetRelated(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		private void LoadAttributes(MixCmsContext _context, IDbContextTransaction _transaction)
		{
			stackVariable0 = ViewModelBase<MixCmsContext, MixAttributeSet, Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel>.Repository;
			V_1 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel::LoadAttributes(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: System.Void LoadAttributes(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		private List<SupportedCulture> LoadCultures(string initCulture = null, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0 = ViewModelBase<MixCmsContext, MixCulture, SystemCultureViewModel>.Repository.GetModelList(_context, _transaction);
			V_1 = new List<SupportedCulture>();
			if (V_0.get_IsSucceed())
			{
				V_2 = V_0.get_Data().GetEnumerator();
				try
				{
					while (V_2.MoveNext())
					{
						V_3 = new Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel.u003cu003ec__DisplayClass219_0();
						V_3.u003cu003e4__this = this;
						V_3.culture = V_2.get_Current();
						stackVariable19 = V_1;
						V_4 = new SupportedCulture();
						V_4.set_Icon(V_3.culture.get_Icon());
						V_4.set_Specificulture(V_3.culture.get_Specificulture());
						V_4.set_Alias(V_3.culture.get_Alias());
						V_4.set_FullName(V_3.culture.get_FullName());
						V_4.set_Description(V_3.culture.get_FullName());
						V_4.set_Id(V_3.culture.get_Id());
						V_4.set_Lcid(V_3.culture.get_Lcid());
						stackVariable49 = V_4;
						if (string.op_Equality(V_3.culture.get_Specificulture(), initCulture))
						{
							stackVariable55 = true;
						}
						else
						{
							stackVariable58 = _context.get_MixPost();
							V_5 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
							// Current member / type: System.Collections.Generic.List`1<Mix.Domain.Core.Models.SupportedCulture> Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel::LoadCultures(System.String,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
							// Exception in: System.Collections.Generic.List<Mix.Domain.Core.Models.SupportedCulture> LoadCultures(System.String,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
							// Specified method is not supported.
							// 
							// mailto: JustDecompilePublicFeedback@telerik.com


		private void LoadMedias(MixCmsContext _context, IDbContextTransaction _transaction)
		{
			stackVariable0 = ViewModelBase<MixCmsContext, MixPostMedia, Mix.Cms.Lib.ViewModels.MixPostMedias.ReadViewModel>.Repository;
			V_1 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel::LoadMedias(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: System.Void LoadMedias(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		private void LoadParentModules(MixCmsContext _context, IDbContextTransaction _transaction)
		{
			V_0 = new Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel.u003cu003ec__DisplayClass215_0();
			V_0.u003cu003e4__this = this;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_1 = Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel.GetModulePostNavAsync(this.get_Id(), this.get_Specificulture(), V_0._context, V_0._transaction);
			if (V_1.get_IsSucceed())
			{
				V_3 = V_1.get_Data().GetEnumerator();
				try
				{
					while (V_3.MoveNext())
					{
						V_4 = V_3.get_Current();
						stackVariable114 = V_4;
						stackVariable116 = V_4.get_Module();
						if (stackVariable116 != null)
						{
							stackVariable117 = stackVariable116.get_Title();
						}
						else
						{
							dummyVar0 = stackVariable116;
							stackVariable117 = null;
						}
						if (stackVariable117 == null)
						{
							dummyVar1 = stackVariable117;
							stackVariable117 = V_4.get_Description();
						}
						stackVariable114.set_Description((string)stackVariable117);
						V_4.set_Specificulture(this.get_Specificulture());
						stackVariable121 = V_4;
						stackVariable123 = V_4.get_Module();
						if (stackVariable123 != null)
						{
							stackVariable124 = stackVariable123.get_ImageUrl();
						}
						else
						{
							dummyVar2 = stackVariable123;
							stackVariable124 = null;
						}
						if (stackVariable124 == null)
						{
							dummyVar3 = stackVariable124;
							stackVariable124 = V_4.get_Image();
						}
						stackVariable121.set_Image((string)stackVariable124);
					}
				}
				finally
				{
					((IDisposable)V_3).Dispose();
				}
				this.set_Modules(V_1.get_Data());
				this.get_Modules().ForEach(new Action<Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel>(V_0.u003cLoadParentModulesu003eb__0));
			}
			stackVariable18 = ViewModelBase<MixCmsContext, MixModule, Mix.Cms.Lib.ViewModels.MixModules.ReadListItemViewModel>.Repository;
			V_5 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel::LoadParentModules(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: System.Void LoadParentModules(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		private void LoadParentPage(MixCmsContext _context, IDbContextTransaction _transaction)
		{
			V_0 = new Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel.u003cu003ec__DisplayClass216_0();
			V_0.u003cu003e4__this = this;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_1 = Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel.GetPagePostNavAsync(this.get_Id(), this.get_Specificulture(), V_0._context, V_0._transaction);
			if (V_1.get_IsSucceed())
			{
				this.set_Pages(V_1.get_Data());
				this.get_Pages().ForEach(new Action<Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel>(V_0.u003cLoadParentPageu003eb__0));
			}
			return;
		}

		private void LoadRelatedPost(MixCmsContext _context, IDbContextTransaction _transaction)
		{
			this.set_PostNavs(this.GetRelated(_context, _transaction));
			return;
		}

		private void LoadTemplates(MixCmsContext _context, IDbContextTransaction _transaction)
		{
			stackVariable2 = this.get_Templates();
			if (stackVariable2 == null)
			{
				dummyVar0 = stackVariable2;
				stackVariable24 = ViewModelBase<MixCmsContext, MixTemplate, Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel>.Repository;
				V_0 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
				// Current member / type: System.Void Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel::LoadTemplates(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Exception in: System.Void LoadTemplates(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Specified method is not supported.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com


		public override MixPost ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (this.get_Id() == 0)
			{
				stackVariable184 = ViewModelBase<MixCmsContext, MixPost, Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel>.Repository;
				V_1 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
				// Current member / type: Mix.Cms.Lib.Models.Cms.MixPost Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel::ParseModel(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Exception in: Mix.Cms.Lib.Models.Cms.MixPost ParseModel(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Specified method is not supported.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com


		public override RepositoryResponse<bool> RemoveRelatedModels(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel view, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			stackVariable0 = new RepositoryResponse<bool>();
			stackVariable0.set_IsSucceed(true);
			V_0 = stackVariable0;
			if (V_0.get_IsSucceed())
			{
				stackVariable293 = _context.get_MixPagePost();
				V_1 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
				// Current member / type: Mix.Domain.Core.ViewModels.RepositoryResponse`1<System.Boolean> Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel::RemoveRelatedModels(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Exception in: Mix.Domain.Core.ViewModels.RepositoryResponse<System.Boolean> RemoveRelatedModels(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Specified method is not supported.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com


		public override async Task<RepositoryResponse<bool>> RemoveRelatedModelsAsync(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel view, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0.u003cu003e4__this = this;
			V_0._context = _context;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel.u003cRemoveRelatedModelsAsyncu003ed__203>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private RepositoryResponse<bool> SaveAttribute(int parentId, MixCmsContext context, IDbContextTransaction transaction)
		{
			stackVariable0 = new RepositoryResponse<bool>();
			stackVariable0.set_IsSucceed(true);
			V_0 = stackVariable0;
			this.get_AttributeData().set_ParentId(parentId.ToString());
			this.get_AttributeData().set_ParentType(2);
			ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel>(this.get_AttributeData().SaveModel(true, context, transaction), ref V_0);
			V_1 = this.get_SysCategories().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (!V_0.get_IsSucceed())
					{
						continue;
					}
					V_2.set_ParentId(parentId.ToString());
					V_2.set_ParentType(2);
					V_2.set_Specificulture(this.get_Specificulture());
					ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.FormViewModel>(V_2.SaveModel(false, context, transaction), ref V_0);
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			V_1 = this.get_SysTags().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_3 = V_1.get_Current();
					if (!V_0.get_IsSucceed())
					{
						continue;
					}
					V_3.set_ParentId(parentId.ToString());
					V_3.set_ParentType(2);
					V_3.set_Specificulture(this.get_Specificulture());
					ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.FormViewModel>(V_3.SaveModel(false, context, transaction), ref V_0);
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			return V_0;
		}

		private async Task<RepositoryResponse<bool>> SaveAttributeAsync(int parentId, MixCmsContext context, IDbContextTransaction transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.parentId = parentId;
			V_0.context = context;
			V_0.transaction = transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel.u003cSaveAttributeAsyncu003ed__197>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private RepositoryResponse<bool> SaveMedias(int id, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			stackVariable0 = new RepositoryResponse<bool>();
			stackVariable0.set_IsSucceed(true);
			V_0 = stackVariable0;
			V_1 = this.get_MediaNavs().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_2.set_PostId(id);
					V_2.set_Specificulture(this.get_Specificulture());
					if (!V_2.get_IsActived())
					{
						ViewModelHelper.HandleResult<MixPostMedia>(V_2.RemoveModel(false, _context, _transaction), ref V_0);
					}
					else
					{
						ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixPostMedias.ReadViewModel>(V_2.SaveModel(false, _context, _transaction), ref V_0);
					}
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			return V_0;
		}

		private async Task<RepositoryResponse<bool>> SaveMediasAsync(int id, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel.u003cSaveMediasAsyncu003ed__201>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private RepositoryResponse<bool> SaveParentModules(int id, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			stackVariable0 = new RepositoryResponse<bool>();
			stackVariable0.set_IsSucceed(true);
			V_0 = stackVariable0;
			V_1 = this.get_Modules().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_2.set_PostId(id);
					V_2.set_Description(this.get_Title());
					V_2.set_Image(this.get_ThumbnailUrl());
					V_2.set_Status(2);
					if (!V_2.get_IsActived())
					{
						V_4 = V_2.RemoveModel(false, _context, _transaction);
						V_0.set_IsSucceed(V_4.get_IsSucceed());
						if (V_0.get_IsSucceed())
						{
							dummyVar1 = ViewModelBase<MixCmsContext, MixModule, Mix.Cms.Lib.ViewModels.MixModules.ReadListItemViewModel>.Repository.RemoveCache(V_2.get_Module().get_Model(), _context, _transaction);
						}
						else
						{
							V_0.set_Exception(V_4.get_Exception());
							this.get_Errors().AddRange(V_4.get_Errors());
						}
					}
					else
					{
						V_3 = V_2.SaveModel(false, _context, _transaction);
						V_0.set_IsSucceed(V_3.get_IsSucceed());
						if (V_0.get_IsSucceed())
						{
							dummyVar0 = ViewModelBase<MixCmsContext, MixModule, Mix.Cms.Lib.ViewModels.MixModules.ReadListItemViewModel>.Repository.RemoveCache(V_2.get_Module().get_Model(), _context, _transaction);
						}
						else
						{
							V_0.set_Exception(V_3.get_Exception());
							this.get_Errors().AddRange(V_3.get_Errors());
						}
					}
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			return V_0;
		}

		private async Task<RepositoryResponse<bool>> SaveParentModulesAsync(int id, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel.u003cSaveParentModulesAsyncu003ed__198>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private RepositoryResponse<bool> SaveParentPages(int id, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			stackVariable0 = new RepositoryResponse<bool>();
			stackVariable0.set_IsSucceed(true);
			V_0 = stackVariable0;
			V_1 = this.get_Pages().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_2.set_PostId(id);
					V_2.set_Description(this.get_Title());
					V_2.set_Image(this.get_ThumbnailUrl());
					V_2.set_Status(2);
					if (!V_2.get_IsActived())
					{
						V_4 = V_2.RemoveModel(false, _context, _transaction);
						V_0.set_IsSucceed(V_4.get_IsSucceed());
						if (V_0.get_IsSucceed())
						{
							dummyVar1 = ViewModelBase<MixCmsContext, MixPage, Mix.Cms.Lib.ViewModels.MixPages.ReadListItemViewModel>.Repository.RemoveCache(V_2.get_Page().get_Model(), _context, _transaction);
						}
						else
						{
							V_0.set_Exception(V_4.get_Exception());
							this.get_Errors().AddRange(V_4.get_Errors());
						}
					}
					else
					{
						V_3 = V_2.SaveModel(false, _context, _transaction);
						V_0.set_IsSucceed(V_3.get_IsSucceed());
						if (V_0.get_IsSucceed())
						{
							dummyVar0 = ViewModelBase<MixCmsContext, MixPage, Mix.Cms.Lib.ViewModels.MixPages.ReadListItemViewModel>.Repository.RemoveCache(V_2.get_Page().get_Model(), _context, _transaction);
						}
						else
						{
							V_0.set_Exception(V_3.get_Exception());
							this.get_Errors().AddRange(V_3.get_Errors());
						}
					}
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			return V_0;
		}

		private async Task<RepositoryResponse<bool>> SaveParentPagesAsync(int id, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel.u003cSaveParentPagesAsyncu003ed__199>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private RepositoryResponse<bool> SaveRelatedPost(int id, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			stackVariable0 = new RepositoryResponse<bool>();
			stackVariable0.set_IsSucceed(true);
			V_0 = stackVariable0;
			V_1 = this.get_PostNavs().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_2.set_SourceId(id);
					V_2.set_Status(2);
					V_2.set_Specificulture(this.get_Specificulture());
					if (!V_2.get_IsActived())
					{
						V_4 = V_2.RemoveModel(false, _context, _transaction);
						V_0.set_IsSucceed(V_4.get_IsSucceed());
						if (V_0.get_IsSucceed())
						{
							dummyVar1 = ViewModelBase<MixCmsContext, MixPost, Mix.Cms.Lib.ViewModels.MixPosts.ReadViewModel>.Repository.RemoveCache(V_2.get_RelatedPost().get_Model(), null, null);
						}
						else
						{
							V_0.set_Exception(V_4.get_Exception());
							this.get_Errors().AddRange(V_4.get_Errors());
						}
					}
					else
					{
						V_3 = V_2.SaveModel(false, _context, _transaction);
						V_0.set_IsSucceed(V_3.get_IsSucceed());
						if (V_0.get_IsSucceed())
						{
							dummyVar0 = ViewModelBase<MixCmsContext, MixPost, Mix.Cms.Lib.ViewModels.MixPosts.ReadViewModel>.Repository.RemoveCache(V_2.get_RelatedPost().get_Model(), null, null);
						}
						else
						{
							V_0.set_Exception(V_3.get_Exception());
							this.get_Errors().AddRange(V_3.get_Errors());
						}
					}
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			return V_0;
		}

		private async Task<RepositoryResponse<bool>> SaveRelatedPostAsync(int id, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel.u003cSaveRelatedPostAsyncu003ed__200>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public override RepositoryResponse<bool> SaveSubModels(MixPost parent, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			stackVariable0 = new RepositoryResponse<bool>();
			stackVariable0.set_IsSucceed(true);
			V_0 = stackVariable0;
			try
			{
				V_1 = this.get_View().SaveModel(true, _context, _transaction);
				stackVariable8 = V_0;
				if (!V_0.get_IsSucceed())
				{
					stackVariable11 = false;
				}
				else
				{
					stackVariable11 = V_1.get_IsSucceed();
				}
				stackVariable8.set_IsSucceed(stackVariable11);
				ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel>(V_1, ref V_0);
				if (V_0.get_IsSucceed())
				{
					V_0 = this.SaveUrlAlias(parent.get_Id(), _context, _transaction);
				}
				if (V_0.get_IsSucceed())
				{
					V_0 = this.SaveMedias(parent.get_Id(), _context, _transaction);
				}
				if (V_0.get_IsSucceed())
				{
					V_0 = this.SaveAttribute(parent.get_Id(), _context, _transaction);
				}
				if (V_0.get_IsSucceed())
				{
					V_0 = this.SaveRelatedPost(parent.get_Id(), _context, _transaction);
				}
				if (V_0.get_IsSucceed())
				{
					V_0 = this.SaveParentPages(parent.get_Id(), _context, _transaction);
				}
				if (V_0.get_IsSucceed())
				{
					V_0 = this.SaveParentModules(parent.get_Id(), _context, _transaction);
				}
				V_2 = V_0;
			}
			catch (Exception exception_0)
			{
				V_3 = exception_0;
				V_0.set_IsSucceed(false);
				V_0.set_Exception(V_3);
				V_2 = V_0;
			}
			return V_2;
		}

		public override async Task<RepositoryResponse<bool>> SaveSubModelsAsync(MixPost parent, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0.u003cu003e4__this = this;
			V_0.parent = parent;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel.u003cSaveSubModelsAsyncu003ed__196>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private RepositoryResponse<bool> SaveUrlAlias(int parentId, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			stackVariable0 = new RepositoryResponse<bool>();
			stackVariable0.set_IsSucceed(true);
			V_0 = stackVariable0;
			V_1 = this.get_UrlAliases().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					stackVariable8 = V_1.get_Current();
					stackVariable8.set_SourceId(parentId.ToString());
					stackVariable8.set_Type(1);
					stackVariable8.set_Specificulture(this.get_Specificulture());
					ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel>(((ViewModelBase<MixCmsContext, MixUrlAlias, Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel>)stackVariable8).SaveModel(false, _context, _transaction), ref V_0);
					if (V_0.get_IsSucceed())
					{
						continue;
					}
					goto Label0;
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
		Label0:
			return V_0;
		}

		private async Task<RepositoryResponse<bool>> SaveUrlAliasAsync(int parentId, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.parentId = parentId;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel.u003cSaveUrlAliasAsyncu003ed__202>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}