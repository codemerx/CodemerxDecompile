using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.Services;
using Mix.Cms.Lib.ViewModels.MixAttributeSetDatas;
using Mix.Cms.Lib.ViewModels.MixAttributeSets;
using Mix.Cms.Lib.ViewModels.MixModules;
using Mix.Cms.Lib.ViewModels.MixPageModules;
using Mix.Cms.Lib.ViewModels.MixPagePosts;
using Mix.Cms.Lib.ViewModels.MixPosts;
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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
		public List<Mix.Cms.Lib.ViewModels.MixPageModules.ReadMvcViewModel> Modules { get; set; } = new List<Mix.Cms.Lib.ViewModels.MixPageModules.ReadMvcViewModel>();

		[JsonProperty("pageSize")]
		public int? PageSize
		{
			get;
			set;
		}

		[JsonProperty("posts")]
		public PaginationModel<Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel> Posts { get; set; } = new PaginationModel<Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel>();

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
				return string.Concat("/Views/Shared/Templates/", MixService.GetConfig<string>("ThemeFolder", this.Specificulture), "/", this.Template);
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
		}

		public ReadMvcViewModel(MixPage model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.View = Mix.Cms.Lib.ViewModels.MixTemplates.ReadListItemViewModel.GetTemplateByPath(this.Template, this.Specificulture, _context, _transaction).get_Data();
			if (this.View != null)
			{
				this.GetSubModules(_context, _transaction);
			}
			this.LoadAttributes(_context, _transaction);
		}

		public Mix.Cms.Lib.ViewModels.MixModules.ReadMvcViewModel GetModule(string name)
		{
			Mix.Cms.Lib.ViewModels.MixPageModules.ReadMvcViewModel readMvcViewModel = this.Modules.FirstOrDefault<Mix.Cms.Lib.ViewModels.MixPageModules.ReadMvcViewModel>((Mix.Cms.Lib.ViewModels.MixPageModules.ReadMvcViewModel m) => m.Module.Name == name);
			if (readMvcViewModel != null)
			{
				return readMvcViewModel.Module;
			}
			return null;
		}

		private void GetSubModules(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			string scripts;
			string styles;
			RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixPageModules.ReadMvcViewModel>> modelListBy = ViewModelBase<MixCmsContext, MixPageModule, Mix.Cms.Lib.ViewModels.MixPageModules.ReadMvcViewModel>.Repository.GetModelListBy((MixPageModule m) => m.PageId == this.Id && m.Specificulture == this.Specificulture, _context, _transaction);
			if (modelListBy.get_IsSucceed())
			{
				this.Modules = modelListBy.get_Data();
				StringBuilder stringBuilder = new StringBuilder();
				StringBuilder stringBuilder1 = new StringBuilder();
				foreach (Mix.Cms.Lib.ViewModels.MixPageModules.ReadMvcViewModel list in (
					from n in modelListBy.get_Data()
					orderby n.Priority
					select n).ToList<Mix.Cms.Lib.ViewModels.MixPageModules.ReadMvcViewModel>())
				{
					StringBuilder stringBuilder2 = stringBuilder;
					Mix.Cms.Lib.ViewModels.MixTemplates.ReadListItemViewModel view = list.Module.View;
					if (view != null)
					{
						scripts = view.Scripts;
					}
					else
					{
						scripts = null;
					}
					stringBuilder2.Append(scripts);
					StringBuilder stringBuilder3 = stringBuilder1;
					Mix.Cms.Lib.ViewModels.MixTemplates.ReadListItemViewModel readListItemViewModel = list.Module.View;
					if (readListItemViewModel != null)
					{
						styles = readListItemViewModel.Styles;
					}
					else
					{
						styles = null;
					}
					stringBuilder3.Append(styles);
				}
				Mix.Cms.Lib.ViewModels.MixTemplates.ReadListItemViewModel view1 = this.View;
				view1.Scripts = string.Concat(view1.Scripts, stringBuilder.ToString());
				Mix.Cms.Lib.ViewModels.MixTemplates.ReadListItemViewModel readListItemViewModel1 = this.View;
				readListItemViewModel1.Styles = string.Concat(readListItemViewModel1.Styles, stringBuilder1.ToString());
			}
		}

		private void GetSubPosts(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<PaginationModel<Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel>> modelListBy = ViewModelBase<MixCmsContext, MixPagePost, Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel>.Repository.GetModelListBy((MixPagePost n) => n.PageId == this.Id && n.Specificulture == this.Specificulture, MixService.GetConfig<string>("OrderBy"), 0, new int?(4), new int?(0), _context, _transaction);
			if (modelListBy.get_IsSucceed())
			{
				this.Posts = modelListBy.get_Data();
			}
		}

		private void LoadAttributes(MixCmsContext _context, IDbContextTransaction _transaction)
		{
			Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel.u003cu003ec__DisplayClass152_0 variable = null;
			DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixAttributeSet, Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel>.Repository;
			ParameterExpression parameterExpression = Expression.Parameter(typeof(MixAttributeSet), "m");
			RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel> singleModel = repository.GetSingleModel(Expression.Lambda<Func<MixAttributeSet, bool>>(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSet).GetMethod("get_Name").MethodHandle)), Expression.Constant("sys_additional_field_page", typeof(string))), new ParameterExpression[] { parameterExpression }), _context, _transaction);
			if (singleModel.get_IsSucceed())
			{
				DefaultRepository<!0, !1, !2> defaultRepository = ViewModelBase<MixCmsContext, MixRelatedAttributeData, Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.ReadMvcViewModel>.Repository;
				parameterExpression = Expression.Parameter(typeof(MixRelatedAttributeData), "a");
				this.AttributeData = defaultRepository.GetFirstModel(Expression.Lambda<Func<MixRelatedAttributeData, bool>>(Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_ParentId").MethodHandle)), Expression.Call(Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel).GetMethod("get_Id").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(int).GetMethod("ToString").MethodHandle), Array.Empty<Expression>())), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel).GetMethod("get_Specificulture").MethodHandle)))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_AttributeSetId").MethodHandle)), Expression.Property(Expression.Property(Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel.u003cu003ec__DisplayClass152_0)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel.u003cu003ec__DisplayClass152_0).GetField("getAttrs").FieldHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel>).GetMethod("get_Data").MethodHandle, typeof(RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel>).TypeHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel).GetMethod("get_Id").MethodHandle)))), new ParameterExpression[] { parameterExpression }), _context, _transaction).get_Data();
			}
		}

		public void LoadData(int? pageSize = null, int? pageIndex = null, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
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
					Expression<Func<MixPagePost, bool>> pageId = null;
					foreach (Mix.Cms.Lib.ViewModels.MixPageModules.ReadMvcViewModel module in this.Modules)
					{
						nullable = null;
						int? nullable1 = nullable;
						nullable = null;
						int? nullable2 = nullable;
						nullable = null;
						module.Module.LoadData(nullable1, nullable2, nullable, pageSize, pageIndex, mixCmsContext, dbContextTransaction);
					}
					if (this.Type != MixEnums.MixPageType.ListPost)
					{
						Expression<Func<MixPageModule, bool>> expression = (MixPageModule m) => m.PageId == this.Id && m.Specificulture == this.Specificulture;
						pageId = (MixPagePost n) => n.PageId == this.Id && n.Specificulture == this.Specificulture;
					}
					else
					{
						pageId = (MixPagePost n) => n.PageId == this.Id && n.Specificulture == this.Specificulture;
					}
					if (pageId != null)
					{
						RepositoryResponse<PaginationModel<Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel>> modelListBy = ViewModelBase<MixCmsContext, MixPagePost, Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel>.Repository.GetModelListBy(pageId, MixService.GetConfig<string>("OrderBy"), 0, pageSize, pageIndex, mixCmsContext, dbContextTransaction);
						if (modelListBy.get_IsSucceed())
						{
							this.Posts = modelListBy.get_Data();
						}
					}
				}
				catch (Exception exception)
				{
					UnitOfWorkHelper<MixCmsContext>.HandleException<PaginationModel<Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel>>(exception, flag, dbContextTransaction);
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

		public void LoadDataByKeyword(string keyword, string orderBy, int orderDirection, int? pageSize = null, int? pageIndex = null, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
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
					pageIndex = new int?(pageIndex.GetValueOrDefault());
					Expression<Func<MixPost, bool>> expression = null;
					expression = (MixPost n) => n.Title.Contains(keyword) && n.Specificulture == this.Specificulture;
					if (expression != null)
					{
						RepositoryResponse<PaginationModel<Mix.Cms.Lib.ViewModels.MixPosts.ReadListItemViewModel>> modelListBy = ViewModelBase<MixCmsContext, MixPost, Mix.Cms.Lib.ViewModels.MixPosts.ReadListItemViewModel>.Repository.GetModelListBy(expression, MixService.GetConfig<string>(orderBy), 0, pageSize, pageIndex, mixCmsContext, dbContextTransaction);
						if (modelListBy.get_IsSucceed())
						{
							this.Posts.set_Items(new List<Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel>());
							this.Posts.set_PageIndex(modelListBy.get_Data().get_PageIndex());
							this.Posts.set_PageSize(modelListBy.get_Data().get_PageSize());
							this.Posts.set_TotalItems(modelListBy.get_Data().get_TotalItems());
							this.Posts.set_TotalPage(modelListBy.get_Data().get_TotalPage());
							foreach (Mix.Cms.Lib.ViewModels.MixPosts.ReadListItemViewModel item in modelListBy.get_Data().get_Items())
							{
								this.Posts.get_Items().Add(new Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel()
								{
									PageId = this.Id,
									PostId = item.Id,
									Post = item
								});
							}
						}
					}
				}
				catch (Exception exception)
				{
					UnitOfWorkHelper<MixCmsContext>.HandleException<PaginationModel<Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel>>(exception, flag, dbContextTransaction);
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

		public void LoadDataByTag(string tagName, string orderBy, int orderDirection, int? pageSize = null, int? pageIndex = null, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel.u003cu003ec__DisplayClass148_0 variable = null;
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
					pageIndex = new int?(pageIndex.GetValueOrDefault());
					Expression<Func<MixPost, bool>> expression = null;
					JObject jObject = new JObject(new JProperty("text", tagName));
					ParameterExpression parameterExpression = Expression.Parameter(typeof(MixPost), "n");
					expression = Expression.Lambda<Func<MixPost, bool>>(Expression.AndAlso(Expression.Call(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPost).GetMethod("get_Tags").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(string).GetMethod("Contains", new System.Type[] { typeof(string) }).MethodHandle), new Expression[] { Expression.Call(Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel.u003cu003ec__DisplayClass148_0)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel.u003cu003ec__DisplayClass148_0).GetField("obj").FieldHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(JToken).GetMethod("ToString", new System.Type[] { typeof(Formatting), typeof(JsonConverter[]) }).MethodHandle), new Expression[] { Expression.Constant((Formatting)0, typeof(Formatting)), Expression.NewArrayInit(typeof(JsonConverter), Array.Empty<Expression>()) }) }), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPost).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel).GetMethod("get_Specificulture").MethodHandle)))), new ParameterExpression[] { parameterExpression });
					if (expression != null)
					{
						RepositoryResponse<PaginationModel<Mix.Cms.Lib.ViewModels.MixPosts.ReadListItemViewModel>> modelListBy = ViewModelBase<MixCmsContext, MixPost, Mix.Cms.Lib.ViewModels.MixPosts.ReadListItemViewModel>.Repository.GetModelListBy(expression, MixService.GetConfig<string>(orderBy), 0, pageSize, pageIndex, mixCmsContext, dbContextTransaction);
						if (modelListBy.get_IsSucceed())
						{
							this.Posts.set_Items(new List<Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel>());
							this.Posts.set_PageIndex(modelListBy.get_Data().get_PageIndex());
							this.Posts.set_PageSize(modelListBy.get_Data().get_PageSize());
							this.Posts.set_TotalItems(modelListBy.get_Data().get_TotalItems());
							this.Posts.set_TotalPage(modelListBy.get_Data().get_TotalPage());
							foreach (Mix.Cms.Lib.ViewModels.MixPosts.ReadListItemViewModel item in modelListBy.get_Data().get_Items())
							{
								this.Posts.get_Items().Add(new Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel()
								{
									PageId = this.Id,
									PostId = item.Id,
									Post = item
								});
							}
						}
					}
				}
				catch (Exception exception)
				{
					UnitOfWorkHelper<MixCmsContext>.HandleException<PaginationModel<Mix.Cms.Lib.ViewModels.MixPages.ReadMvcViewModel>>(exception, flag, dbContextTransaction);
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