using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixAttributeSets;
using Mix.Cms.Lib.ViewModels.MixModules;
using Mix.Cms.Lib.ViewModels.MixPageModules;
using Mix.Cms.Lib.ViewModels.MixPagePosts;
using Mix.Cms.Lib.ViewModels.MixPosts;
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
using System.Text;

namespace Mix.Cms.Lib.ViewModels.MixPages
{
	public class ReadMvcViewModel : ViewModelBase<MixCmsContext, MixPage, Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel>
	{
		[JsonProperty("attributeData")]
		public Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.ReadMvcViewModel AttributeData
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

		[JsonProperty("details")]
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

		[JsonProperty("modifiedBy")]
		public string ModifiedBy
		{
			get;
			set;
		}

		[JsonProperty("modules")]
		public List<Mix.Cms.Lib.ViewModels.MixPageModules.ReadMvcViewModel> Modules
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

		[JsonProperty("posts")]
		public PaginationModel<Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel> Posts
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

		public string TemplatePath
		{
			get
			{
				return string.Concat("/Views/Shared/Templates/", MixService.GetConfig<string>("ThemeFolder", this.get_Specificulture()), "/", this.get_Template());
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
		public MixEnums.MixPageType Type
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

		[JsonProperty("views")]
		public int? Views
		{
			get;
			set;
		}

		public ReadMvcViewModel()
		{
			this.u003cPostsu003ek__BackingField = new PaginationModel<Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel>();
			this.u003cModulesu003ek__BackingField = new List<Mix.Cms.Lib.ViewModels.MixPageModules.ReadMvcViewModel>();
			base();
			return;
		}

		public ReadMvcViewModel(MixPage model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.u003cPostsu003ek__BackingField = new PaginationModel<Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel>();
			this.u003cModulesu003ek__BackingField = new List<Mix.Cms.Lib.ViewModels.MixPageModules.ReadMvcViewModel>();
			base(model, _context, _transaction);
			return;
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.set_View(Mix.Cms.Lib.ViewModels.MixTemplates.ReadListItemViewModel.GetTemplateByPath(this.get_Template(), this.get_Specificulture(), _context, _transaction).get_Data());
			if (this.get_View() != null)
			{
				this.GetSubModules(_context, _transaction);
			}
			this.LoadAttributes(_context, _transaction);
			return;
		}

		public Mix.Cms.Lib.ViewModels.MixModules.ReadMvcViewModel GetModule(string name)
		{
			V_0 = new Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel.u003cu003ec__DisplayClass153_0();
			V_0.name = name;
			stackVariable8 = Enumerable.FirstOrDefault<Mix.Cms.Lib.ViewModels.MixPageModules.ReadMvcViewModel>(this.get_Modules(), new Func<Mix.Cms.Lib.ViewModels.MixPageModules.ReadMvcViewModel, bool>(V_0, Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel.u003cu003ec__DisplayClass153_0.u003cGetModuleu003eb__0));
			if (stackVariable8 != null)
			{
				return stackVariable8.get_Module();
			}
			dummyVar0 = stackVariable8;
			return null;
		}

		private void GetSubModules(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			stackVariable0 = ViewModelBase<MixCmsContext, MixPageModule, Mix.Cms.Lib.ViewModels.MixPageModules.ReadMvcViewModel>.Repository;
			V_1 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel::GetSubModules(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: System.Void GetSubModules(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		private void GetSubPosts(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			stackVariable0 = ViewModelBase<MixCmsContext, MixPagePost, Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel>.Repository;
			V_1 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel::GetSubPosts(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: System.Void GetSubPosts(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		private void LoadAttributes(MixCmsContext _context, IDbContextTransaction _transaction)
		{
			V_0 = new Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel.u003cu003ec__DisplayClass152_0();
			V_0.u003cu003e4__this = this;
			stackVariable4 = ViewModelBase<MixCmsContext, MixAttributeSet, Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel>.Repository;
			V_1 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel::LoadAttributes(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: System.Void LoadAttributes(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public void LoadData(int? pageSize = null, int? pageIndex = null, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			UnitOfWorkHelper<MixCmsContext>.InitTransaction(_context, _transaction, ref V_0, ref V_1, ref V_2);
			try
			{
				try
				{
					V_4 = pageSize;
					if (V_4.GetValueOrDefault() > 0 & V_4.get_HasValue())
					{
						stackVariable14 = pageSize;
					}
					else
					{
						stackVariable14 = this.get_PageSize();
					}
					pageSize = stackVariable14;
					V_4 = pageIndex;
					if (V_4.GetValueOrDefault() > 0 & V_4.get_HasValue())
					{
						stackVariable24 = pageIndex;
					}
					else
					{
						stackVariable24 = new int?(0);
					}
					pageIndex = stackVariable24;
					V_3 = null;
					V_6 = this.get_Modules().GetEnumerator();
					try
					{
						while (V_6.MoveNext())
						{
							V_4 = null;
							stackVariable35 = V_4;
							V_4 = null;
							stackVariable37 = V_4;
							V_4 = null;
							V_6.get_Current().get_Module().LoadData(stackVariable35, stackVariable37, V_4, pageSize, pageIndex, V_0, V_1);
						}
					}
					finally
					{
						V_6.Dispose();
					}
					if (this.get_Type() != 2)
					{
						V_7 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
						// Current member / type: System.Void Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel::LoadData(System.Nullable`1<System.Int32>,System.Nullable`1<System.Int32>,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
						// Exception in: System.Void LoadData(System.Nullable<System.Int32>,System.Nullable<System.Int32>,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
						// Specified method is not supported.
						// 
						// mailto: JustDecompilePublicFeedback@telerik.com


		public void LoadDataByKeyword(string keyword, string orderBy, int orderDirection, int? pageSize = null, int? pageIndex = null, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0 = new Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel.u003cu003ec__DisplayClass149_0();
			V_0.keyword = keyword;
			V_0.u003cu003e4__this = this;
			UnitOfWorkHelper<MixCmsContext>.InitTransaction(_context, _transaction, ref V_1, ref V_2, ref V_3);
			try
			{
				try
				{
					V_5 = pageSize;
					if (V_5.GetValueOrDefault() > 0 & V_5.get_HasValue())
					{
						stackVariable19 = pageSize;
					}
					else
					{
						stackVariable19 = this.get_PageSize();
					}
					pageSize = stackVariable19;
					pageIndex = new int?(pageIndex.GetValueOrDefault());
					V_4 = null;
					V_7 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
					// Current member / type: System.Void Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel::LoadDataByKeyword(System.String,System.String,System.Int32,System.Nullable`1<System.Int32>,System.Nullable`1<System.Int32>,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
					// Exception in: System.Void LoadDataByKeyword(System.String,System.String,System.Int32,System.Nullable<System.Int32>,System.Nullable<System.Int32>,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
					// Specified method is not supported.
					// 
					// mailto: JustDecompilePublicFeedback@telerik.com


		public void LoadDataByTag(string tagName, string orderBy, int orderDirection, int? pageSize = null, int? pageIndex = null, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0 = new Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel.u003cu003ec__DisplayClass148_0();
			V_0.u003cu003e4__this = this;
			UnitOfWorkHelper<MixCmsContext>.InitTransaction(_context, _transaction, ref V_1, ref V_2, ref V_3);
			try
			{
				try
				{
					V_5 = pageSize;
					if (V_5.GetValueOrDefault() > 0 & V_5.get_HasValue())
					{
						stackVariable17 = pageSize;
					}
					else
					{
						stackVariable17 = this.get_PageSize();
					}
					pageSize = stackVariable17;
					pageIndex = new int?(pageIndex.GetValueOrDefault());
					V_4 = null;
					V_0.obj = new JObject(new JProperty("text", tagName));
					V_7 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
					// Current member / type: System.Void Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel::LoadDataByTag(System.String,System.String,System.Int32,System.Nullable`1<System.Int32>,System.Nullable`1<System.Int32>,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
					// Exception in: System.Void LoadDataByTag(System.String,System.String,System.Int32,System.Nullable<System.Int32>,System.Nullable<System.Int32>,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
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