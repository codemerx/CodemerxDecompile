using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels;
using Mix.Cms.Lib.ViewModels.MixCultures;
using Mix.Cms.Lib.ViewModels.MixMedias;
using Mix.Cms.Lib.ViewModels.MixModulePosts;
using Mix.Cms.Lib.ViewModels.MixModules;
using Mix.Cms.Lib.ViewModels.MixPagePosts;
using Mix.Cms.Lib.ViewModels.MixPostMedias;
using Mix.Cms.Lib.ViewModels.MixPostModules;
using Mix.Cms.Lib.ViewModels.MixPostPosts;
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

namespace Mix.Cms.Lib.ViewModels.MixPosts
{
	public class SyncViewModel : ViewModelBase<MixCmsContext, MixPost, SyncViewModel>
	{
		[JsonIgnore]
		public int ActivedTheme
		{
			get
			{
				return MixService.GetConfig<int>("ThemeId", this.get_Specificulture());
			}
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

		public SyncViewModel()
		{
			this.u003cExtraFieldsu003ek__BackingField = "[]";
			this.u003cExtraPropertiesu003ek__BackingField = "[]";
			this.u003cTagsu003ek__BackingField = "[]";
			this.u003cListTagu003ek__BackingField = new JArray();
			base();
			return;
		}

		public SyncViewModel(MixPost model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.u003cExtraFieldsu003ek__BackingField = "[]";
			this.u003cExtraPropertiesu003ek__BackingField = "[]";
			this.u003cTagsu003ek__BackingField = "[]";
			this.u003cListTagu003ek__BackingField = new JArray();
			base(model, _context, _transaction);
			return;
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0 = new SyncViewModel.u003cu003ec__DisplayClass178_0();
			V_0.u003cu003e4__this = this;
			V_0._context = _context;
			V_0._transaction = _transaction;
			if (this.get_Id() == 0)
			{
				this.set_ExtraFields(MixService.GetConfig<string>("DefaultPostAttr"));
			}
			this.set_Cultures(this.LoadCultures(this.get_Specificulture(), V_0._context, V_0._transaction));
			this.set_UrlAliases(this.GetAliases(V_0._context, V_0._transaction));
			if (!string.IsNullOrEmpty(this.get_Tags()))
			{
				this.set_ListTag(JArray.Parse(this.get_Tags()));
			}
			this.set_Columns(new List<ModuleFieldViewModel>());
			if (!string.IsNullOrEmpty(this.get_ExtraFields()))
			{
				stackVariable35 = JArray.Parse(this.get_ExtraFields());
			}
			else
			{
				stackVariable35 = new JArray();
			}
			V_5 = stackVariable35.GetEnumerator();
			try
			{
				while (V_5.MoveNext())
				{
					V_6 = V_5.get_Current();
					stackVariable41 = new ModuleFieldViewModel();
					stackVariable41.set_Name(CommonHelper.ParseJsonPropertyName(V_6.get_Item("name").ToString()));
					stackVariable49 = V_6.get_Item("title");
					if (stackVariable49 != null)
					{
						stackVariable50 = stackVariable49.ToString();
					}
					else
					{
						dummyVar0 = stackVariable49;
						stackVariable50 = null;
					}
					stackVariable41.set_Title(stackVariable50);
					if (V_6.get_Item("options") != null)
					{
						stackVariable57 = Newtonsoft.Json.Linq.Extensions.Value<JArray>(V_6.get_Item("options"));
					}
					else
					{
						stackVariable57 = new JArray();
					}
					stackVariable41.set_Options(stackVariable57);
					if (V_6.get_Item("priority") != null)
					{
						stackVariable64 = Newtonsoft.Json.Linq.Extensions.Value<int>(V_6.get_Item("priority"));
					}
					else
					{
						stackVariable64 = 0;
					}
					stackVariable41.set_Priority(stackVariable64);
					stackVariable41.set_DataType(JToken.op_Explicit(V_6.get_Item("dataType")));
					if (V_6.get_Item("width") != null)
					{
						stackVariable75 = Newtonsoft.Json.Linq.Extensions.Value<int>(V_6.get_Item("width"));
					}
					else
					{
						stackVariable75 = 3;
					}
					stackVariable41.set_Width(stackVariable75);
					if (V_6.get_Item("isUnique") != null)
					{
						stackVariable82 = Newtonsoft.Json.Linq.Extensions.Value<bool>(V_6.get_Item("isUnique"));
					}
					else
					{
						stackVariable82 = true;
					}
					stackVariable41.set_IsUnique(stackVariable82);
					if (V_6.get_Item("isRequired") != null)
					{
						stackVariable89 = Newtonsoft.Json.Linq.Extensions.Value<bool>(V_6.get_Item("isRequired"));
					}
					else
					{
						stackVariable89 = true;
					}
					stackVariable41.set_IsRequired(stackVariable89);
					if (V_6.get_Item("isDisplay") != null)
					{
						stackVariable96 = Newtonsoft.Json.Linq.Extensions.Value<bool>(V_6.get_Item("isDisplay"));
					}
					else
					{
						stackVariable96 = true;
					}
					stackVariable41.set_IsDisplay(stackVariable96);
					if (V_6.get_Item("isSelect") != null)
					{
						stackVariable103 = Newtonsoft.Json.Linq.Extensions.Value<bool>(V_6.get_Item("isSelect"));
					}
					else
					{
						stackVariable103 = false;
					}
					stackVariable41.set_IsSelect(stackVariable103);
					if (V_6.get_Item("isGroupBy") != null)
					{
						stackVariable110 = Newtonsoft.Json.Linq.Extensions.Value<bool>(V_6.get_Item("isGroupBy"));
					}
					else
					{
						stackVariable110 = false;
					}
					stackVariable41.set_IsGroupBy(stackVariable110);
					this.get_Columns().Add(stackVariable41);
				}
			}
			finally
			{
				if (V_5 != null)
				{
					V_5.Dispose();
				}
			}
			this.set_Properties(new List<ExtraProperty>());
			if (!string.IsNullOrEmpty(this.get_ExtraProperties()))
			{
				V_5 = JArray.Parse(this.get_ExtraProperties()).GetEnumerator();
				try
				{
					while (V_5.MoveNext())
					{
						V_8 = V_5.get_Current();
						this.get_Properties().Add(V_8.ToObject<ExtraProperty>());
					}
				}
				finally
				{
					if (V_5 != null)
					{
						V_5.Dispose();
					}
				}
			}
			stackVariable121 = this.get_Templates();
			if (stackVariable121 == null)
			{
				dummyVar1 = stackVariable121;
				stackVariable686 = ViewModelBase<MixCmsContext, MixTemplate, Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel>.Repository;
				V_9 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
				// Current member / type: System.Void Mix.Cms.Lib.ViewModels.MixPosts.SyncViewModel::ExpandView(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Exception in: System.Void ExpandView(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Specified method is not supported.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com


		private void GenerateSEO()
		{
			V_0 = new SyncViewModel.u003cu003ec__DisplayClass184_0();
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
			// Current member / type: System.Collections.Generic.List`1<Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel> Mix.Cms.Lib.ViewModels.MixPosts.SyncViewModel::GetAliases(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: System.Collections.Generic.List<Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel> GetAliases(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public List<Mix.Cms.Lib.ViewModels.MixPostPosts.ReadViewModel> GetRelated(MixCmsContext context, IDbContextTransaction transaction)
		{
			stackVariable0 = ViewModelBase<MixCmsContext, MixRelatedPost, Mix.Cms.Lib.ViewModels.MixPostPosts.ReadViewModel>.Repository;
			V_0 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
			// Current member / type: System.Collections.Generic.List`1<Mix.Cms.Lib.ViewModels.MixPostPosts.ReadViewModel> Mix.Cms.Lib.ViewModels.MixPosts.SyncViewModel::GetRelated(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: System.Collections.Generic.List<Mix.Cms.Lib.ViewModels.MixPostPosts.ReadViewModel> GetRelated(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
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
						V_3 = new SyncViewModel.u003cu003ec__DisplayClass183_0();
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
							// Current member / type: System.Collections.Generic.List`1<Mix.Domain.Core.Models.SupportedCulture> Mix.Cms.Lib.ViewModels.MixPosts.SyncViewModel::LoadCultures(System.String,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
							// Exception in: System.Collections.Generic.List<Mix.Domain.Core.Models.SupportedCulture> LoadCultures(System.String,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
							// Specified method is not supported.
							// 
							// mailto: JustDecompilePublicFeedback@telerik.com


		public override MixPost ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (this.get_Id() == 0)
			{
				stackVariable184 = ViewModelBase<MixCmsContext, MixPost, SyncViewModel>.Repository;
				V_1 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
				// Current member / type: Mix.Cms.Lib.Models.Cms.MixPost Mix.Cms.Lib.ViewModels.MixPosts.SyncViewModel::ParseModel(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Exception in: Mix.Cms.Lib.Models.Cms.MixPost ParseModel(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Specified method is not supported.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com


		public override RepositoryResponse<bool> RemoveRelatedModels(SyncViewModel view, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			stackVariable0 = new RepositoryResponse<bool>();
			stackVariable0.set_IsSucceed(true);
			V_0 = stackVariable0;
			if (V_0.get_IsSucceed())
			{
				stackVariable293 = _context.get_MixPagePost();
				V_1 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
				// Current member / type: Mix.Domain.Core.ViewModels.RepositoryResponse`1<System.Boolean> Mix.Cms.Lib.ViewModels.MixPosts.SyncViewModel::RemoveRelatedModels(Mix.Cms.Lib.ViewModels.MixPosts.SyncViewModel,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Exception in: Mix.Domain.Core.ViewModels.RepositoryResponse<System.Boolean> RemoveRelatedModels(Mix.Cms.Lib.ViewModels.MixPosts.SyncViewModel,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Specified method is not supported.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com


		public override RepositoryResponse<bool> SaveSubModels(MixPost parent, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			stackVariable0 = new RepositoryResponse<bool>();
			stackVariable0.set_IsSucceed(true);
			V_0 = stackVariable0;
			try
			{
				if (V_0.get_IsSucceed())
				{
					stackVariable171 = ViewModelBase<MixCmsContext, MixMedia, Mix.Cms.Lib.ViewModels.MixMedias.UpdateViewModel>.Repository;
					V_2 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
					// Current member / type: Mix.Domain.Core.ViewModels.RepositoryResponse`1<System.Boolean> Mix.Cms.Lib.ViewModels.MixPosts.SyncViewModel::SaveSubModels(Mix.Cms.Lib.Models.Cms.MixPost,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
					// Exception in: Mix.Domain.Core.ViewModels.RepositoryResponse<System.Boolean> SaveSubModels(Mix.Cms.Lib.Models.Cms.MixPost,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
					// Specified method is not supported.
					// 
					// mailto: JustDecompilePublicFeedback@telerik.com


		public override async Task<RepositoryResponse<bool>> SaveSubModelsAsync(MixPost parent, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0.u003cu003e4__this = this;
			V_0.parent = parent;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<SyncViewModel.u003cSaveSubModelsAsyncu003ed__180>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}