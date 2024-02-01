using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.Services;
using Mix.Cms.Lib.ViewModels;
using Mix.Cms.Lib.ViewModels.MixAttributeFields;
using Mix.Cms.Lib.ViewModels.MixAttributeSetDatas;
using Mix.Cms.Lib.ViewModels.MixAttributeSets;
using Mix.Cms.Lib.ViewModels.MixAttributeSetValues;
using Mix.Cms.Lib.ViewModels.MixCultures;
using Mix.Cms.Lib.ViewModels.MixModulePosts;
using Mix.Cms.Lib.ViewModels.MixModules;
using Mix.Cms.Lib.ViewModels.MixPagePosts;
using Mix.Cms.Lib.ViewModels.MixPages;
using Mix.Cms.Lib.ViewModels.MixPostMedias;
using Mix.Cms.Lib.ViewModels.MixPostModules;
using Mix.Cms.Lib.ViewModels.MixPostPosts;
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
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
		public string ExtraFields { get; set; } = "[]";

		[JsonIgnore]
		[JsonProperty("extraProperties")]
		public string ExtraProperties { get; set; } = "[]";

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

		[JsonProperty("listTag")]
		public JArray ListTag { get; set; } = new JArray();

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
		public string Tags { get; set; } = "[]";

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
				return CommonHelper.GetFullPath(new string[] { "Views/Shared/Templates", MixService.GetConfig<string>("ThemeName", this.Specificulture), this.TemplateFolderType });
			}
		}

		[JsonIgnore]
		public string TemplateFolderType
		{
			get
			{
				return MixEnums.EnumTemplateFolder.Posts.ToString();
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
		}

		public UpdateViewModel(MixPost model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override Task<RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel>>> CloneAsync(MixPost model, List<SupportedCulture> cloneCultures, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			return base.CloneAsync(model, cloneCultures, _context, _transaction);
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (this.Id == 0)
			{
				this.ExtraFields = MixService.GetConfig<string>("DefaultPostAttr");
			}
			this.Cultures = this.LoadCultures(this.Specificulture, _context, _transaction);
			this.UrlAliases = this.GetAliases(_context, _transaction);
			if (!string.IsNullOrEmpty(this.Tags))
			{
				this.ListTag = JArray.Parse(this.Tags);
			}
			this.LoadAttributes(_context, _transaction);
			this.LoadTemplates(_context, _transaction);
			this.LoadParentPage(_context, _transaction);
			this.LoadParentModules(_context, _transaction);
			this.LoadMedias(_context, _transaction);
			this.LoadRelatedPost(_context, _transaction);
		}

		private void GenerateSEO()
		{
			if (string.IsNullOrEmpty(this.SeoName))
			{
				this.SeoName = SeoHelper.GetSEOString(this.Title, '-');
			}
			int num = 1;
			string seoName = this.SeoName;
			while (ViewModelBase<MixCmsContext, MixPost, Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel>.Repository.CheckIsExists((MixPost a) => {
				if (!(a.SeoName == seoName) || !(a.Specificulture == this.Specificulture))
				{
					return false;
				}
				return a.Id != this.Id;
			}, null, null))
			{
				seoName = string.Concat(this.SeoName, "_", num.ToString());
				num++;
			}
			this.SeoName = seoName;
			if (string.IsNullOrEmpty(this.SeoTitle))
			{
				this.SeoTitle = SeoHelper.GetSEOString(this.Title, '-');
			}
			if (string.IsNullOrEmpty(this.SeoDescription))
			{
				this.SeoDescription = SeoHelper.GetSEOString(this.Title, '-');
			}
			if (string.IsNullOrEmpty(this.SeoKeywords))
			{
				this.SeoKeywords = SeoHelper.GetSEOString(this.Title, '-');
			}
		}

		public List<Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel> GetAliases(MixCmsContext context, IDbContextTransaction transaction)
		{
			DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixUrlAlias, Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel>.Repository;
			ParameterExpression parameterExpression = Expression.Parameter(typeof(MixUrlAlias), "p");
			RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel>> modelListBy = repository.GetModelListBy(Expression.Lambda<Func<MixUrlAlias, bool>>(Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixUrlAlias).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel).GetMethod("get_Specificulture").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixUrlAlias).GetMethod("get_SourceId").MethodHandle)), Expression.Call(Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel).GetMethod("get_Id").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(int).GetMethod("ToString").MethodHandle), Array.Empty<Expression>()))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixUrlAlias).GetMethod("get_Type").MethodHandle)), Expression.Constant(1, typeof(int)))), new ParameterExpression[] { parameterExpression }), context, transaction);
			if (!modelListBy.get_IsSucceed())
			{
				return new List<Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel>();
			}
			return modelListBy.get_Data();
		}

		public List<Mix.Cms.Lib.ViewModels.MixPostPosts.ReadViewModel> GetRelated(MixCmsContext context, IDbContextTransaction transaction)
		{
			List<Mix.Cms.Lib.ViewModels.MixPostPosts.ReadViewModel> data = ViewModelBase<MixCmsContext, MixRelatedPost, Mix.Cms.Lib.ViewModels.MixPostPosts.ReadViewModel>.Repository.GetModelListBy((MixRelatedPost n) => n.SourceId == this.Id && n.Specificulture == this.Specificulture, context, transaction).get_Data();
			data.ForEach((Mix.Cms.Lib.ViewModels.MixPostPosts.ReadViewModel n) => {
				n.Specificulture = this.Specificulture;
				n.IsActived = true;
			});
			return (
				from p in data
				orderby p.Priority
				select p).ToList<Mix.Cms.Lib.ViewModels.MixPostPosts.ReadViewModel>();
		}

		private void LoadAttributes(MixCmsContext _context, IDbContextTransaction _transaction)
		{
			DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixAttributeSet, Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel>.Repository;
			ParameterExpression parameterExpression = Expression.Parameter(typeof(MixAttributeSet), "m");
			RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel> singleModel = repository.GetSingleModel(Expression.Lambda<Func<MixAttributeSet, bool>>(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSet).GetMethod("get_Name").MethodHandle)), Expression.Constant("sys_additional_field_post", typeof(string))), new ParameterExpression[] { parameterExpression }), _context, _transaction);
			if (singleModel.get_IsSucceed())
			{
				this.Attributes = singleModel.get_Data();
				DefaultRepository<!0, !1, !2> defaultRepository = ViewModelBase<MixCmsContext, MixRelatedAttributeData, Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel>.Repository;
				parameterExpression = Expression.Parameter(typeof(MixRelatedAttributeData), "a");
				this.AttributeData = defaultRepository.GetFirstModel(Expression.Lambda<Func<MixRelatedAttributeData, bool>>(Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_ParentId").MethodHandle)), Expression.Call(Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel).GetMethod("get_Id").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(int).GetMethod("ToString").MethodHandle), Array.Empty<Expression>())), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel).GetMethod("get_Specificulture").MethodHandle)))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_AttributeSetId").MethodHandle)), Expression.Property(Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel).GetMethod("get_Attributes").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel).GetMethod("get_Id").MethodHandle)))), new ParameterExpression[] { parameterExpression }), _context, _transaction).get_Data();
				if (this.AttributeData == null)
				{
					this.AttributeData = new Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel(new MixRelatedAttributeData()
					{
						Specificulture = this.Specificulture,
						ParentType = MixEnums.MixAttributeSetDataType.Post.ToString(),
						ParentId = this.Id.ToString(),
						AttributeSetId = this.Attributes.Id,
						AttributeSetName = this.Attributes.Name
					}, null, null)
					{
						Data = new Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel(new MixAttributeSetData()
						{
							Specificulture = this.Specificulture,
							AttributeSetId = this.Attributes.Id,
							AttributeSetName = this.Attributes.Name
						}, null, null)
					};
				}
				foreach (Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel updateViewModel in 
					from f in this.Attributes.Fields
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
							Field = updateViewModel,
							AttributeFieldName = updateViewModel.Name,
							Priority = updateViewModel.Priority,
							StringValue = updateViewModel.DefaultValue
						};
						this.AttributeData.Data.Values.Add(priority);
					}
					priority.Priority = updateViewModel.Priority;
					priority.Field = updateViewModel;
				}
				DefaultRepository<!0, !1, !2> repository1 = ViewModelBase<MixCmsContext, MixRelatedAttributeData, Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.FormViewModel>.Repository;
				parameterExpression = Expression.Parameter(typeof(MixRelatedAttributeData), "m");
				RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.FormViewModel>> modelListBy = repository1.GetModelListBy(Expression.Lambda<Func<MixRelatedAttributeData, bool>>(Expression.AndAlso(Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel).GetMethod("get_Specificulture").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_ParentId").MethodHandle)), Expression.Call(Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel).GetMethod("get_Id").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(int).GetMethod("ToString").MethodHandle), Array.Empty<Expression>()))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_ParentType").MethodHandle)), Expression.Call(Expression.Constant(MixEnums.MixAttributeSetDataType.Post, typeof(MixEnums.MixAttributeSetDataType)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(object).GetMethod("ToString").MethodHandle), Array.Empty<Expression>()))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_AttributeSetName").MethodHandle)), Expression.Constant("sys_category", typeof(string)))), new ParameterExpression[] { parameterExpression }), _context, _transaction);
				if (modelListBy.get_IsSucceed())
				{
					this.SysCategories = modelListBy.get_Data();
				}
				DefaultRepository<!0, !1, !2> defaultRepository1 = ViewModelBase<MixCmsContext, MixRelatedAttributeData, Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.FormViewModel>.Repository;
				parameterExpression = Expression.Parameter(typeof(MixRelatedAttributeData), "m");
				RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.FormViewModel>> repositoryResponse = defaultRepository1.GetModelListBy(Expression.Lambda<Func<MixRelatedAttributeData, bool>>(Expression.AndAlso(Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel).GetMethod("get_Specificulture").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_ParentId").MethodHandle)), Expression.Call(Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel).GetMethod("get_Id").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(int).GetMethod("ToString").MethodHandle), Array.Empty<Expression>()))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_ParentType").MethodHandle)), Expression.Call(Expression.Constant(MixEnums.MixAttributeSetDataType.Post, typeof(MixEnums.MixAttributeSetDataType)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(object).GetMethod("ToString").MethodHandle), Array.Empty<Expression>()))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_AttributeSetName").MethodHandle)), Expression.Constant("sys_tag", typeof(string)))), new ParameterExpression[] { parameterExpression }), _context, _transaction);
				if (repositoryResponse.get_IsSucceed())
				{
					this.SysTags = repositoryResponse.get_Data();
				}
			}
		}

		private List<SupportedCulture> LoadCultures(string initCulture = null, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<List<SystemCultureViewModel>> modelList = ViewModelBase<MixCmsContext, MixCulture, SystemCultureViewModel>.Repository.GetModelList(_context, _transaction);
			List<SupportedCulture> supportedCultures = new List<SupportedCulture>();
			if (modelList.get_IsSucceed())
			{
				foreach (SystemCultureViewModel datum in modelList.get_Data())
				{
					List<SupportedCulture> supportedCultures1 = supportedCultures;
					SupportedCulture supportedCulture = new SupportedCulture();
					supportedCulture.set_Icon(datum.Icon);
					supportedCulture.set_Specificulture(datum.Specificulture);
					supportedCulture.set_Alias(datum.Alias);
					supportedCulture.set_FullName(datum.FullName);
					supportedCulture.set_Description(datum.FullName);
					supportedCulture.set_Id(datum.Id);
					supportedCulture.set_Lcid(datum.Lcid);
					supportedCulture.set_IsSupported((datum.Specificulture == initCulture ? true : _context.MixPost.Any<MixPost>((MixPost p) => p.Id == this.Id && p.Specificulture == datum.Specificulture)));
					supportedCultures1.Add(supportedCulture);
				}
			}
			return supportedCultures;
		}

		private void LoadMedias(MixCmsContext _context, IDbContextTransaction _transaction)
		{
			RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixPostMedias.ReadViewModel>> modelListBy = ViewModelBase<MixCmsContext, MixPostMedia, Mix.Cms.Lib.ViewModels.MixPostMedias.ReadViewModel>.Repository.GetModelListBy((MixPostMedia n) => n.PostId == this.Id && n.Specificulture == this.Specificulture, _context, _transaction);
			if (modelListBy.get_IsSucceed())
			{
				this.MediaNavs = (
					from p in modelListBy.get_Data()
					orderby p.Priority
					select p).ToList<Mix.Cms.Lib.ViewModels.MixPostMedias.ReadViewModel>();
				this.MediaNavs.ForEach((Mix.Cms.Lib.ViewModels.MixPostMedias.ReadViewModel n) => {
					n.Specificulture = this.Specificulture;
					n.IsActived = true;
				});
			}
		}

		private void LoadParentModules(MixCmsContext _context, IDbContextTransaction _transaction)
		{
			object title;
			object imageUrl;
			Func<Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel, bool> func = null;
			RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel>> modulePostNavAsync = Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel.GetModulePostNavAsync(this.Id, this.Specificulture, _context, _transaction);
			if (modulePostNavAsync.get_IsSucceed())
			{
				foreach (Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel datum in modulePostNavAsync.get_Data())
				{
					Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel readViewModel = datum;
					Mix.Cms.Lib.ViewModels.MixModules.ReadListItemViewModel module = datum.Module;
					if (module != null)
					{
						title = module.Title;
					}
					else
					{
						title = null;
					}
					if (title == null)
					{
						title = datum.Description;
					}
					readViewModel.Description = (string)title;
					datum.Specificulture = this.Specificulture;
					Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel readViewModel1 = datum;
					Mix.Cms.Lib.ViewModels.MixModules.ReadListItemViewModel readListItemViewModel = datum.Module;
					if (readListItemViewModel != null)
					{
						imageUrl = readListItemViewModel.ImageUrl;
					}
					else
					{
						imageUrl = null;
					}
					if (imageUrl == null)
					{
						imageUrl = datum.Image;
					}
					readViewModel1.Image = (string)imageUrl;
				}
				this.Modules = modulePostNavAsync.get_Data();
				this.Modules.ForEach((Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel c) => c.IsActived = ViewModelBase<MixCmsContext, MixModulePost, Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel>.Repository.CheckIsExists((MixModulePost n) => {
					if (n.ModuleId != c.ModuleId)
					{
						return false;
					}
					return n.PostId == this.Id;
				}, _context, _transaction));
			}
			DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixModule, Mix.Cms.Lib.ViewModels.MixModules.ReadListItemViewModel>.Repository;
			int? nullable = null;
			RepositoryResponse<PaginationModel<Mix.Cms.Lib.ViewModels.MixModules.ReadListItemViewModel>> modelListBy = repository.GetModelListBy((MixModule m) => (m.Type == 0 || m.Type == 2) && m.Specificulture == this.Specificulture, "CreatedDateTime", 1, nullable, new int?(0), _context, _transaction);
			if (modelListBy.get_Data() != null)
			{
				foreach (Mix.Cms.Lib.ViewModels.MixModules.ReadListItemViewModel item in modelListBy.get_Data().get_Items())
				{
					List<Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel> modules = this.Modules;
					Func<Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel, bool> func1 = func;
					if (func1 == null)
					{
						Func<Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel, bool> moduleId = (Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel m) => {
							if (m.ModuleId != this.Id)
							{
								return false;
							}
							return m.Specificulture == this.Specificulture;
						};
						Func<Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel, bool> func2 = moduleId;
						func = moduleId;
						func1 = func2;
					}
					if (modules.Any<Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel>(func1))
					{
						continue;
					}
					this.Modules.Add(new Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel()
					{
						ModuleId = item.Id,
						Image = item.Image,
						PostId = this.Id,
						Description = item.Title
					});
				}
			}
		}

		private void LoadParentPage(MixCmsContext _context, IDbContextTransaction _transaction)
		{
			RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel>> pagePostNavAsync = Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel.GetPagePostNavAsync(this.Id, this.Specificulture, _context, _transaction);
			if (pagePostNavAsync.get_IsSucceed())
			{
				this.Pages = pagePostNavAsync.get_Data();
				this.Pages.ForEach((Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel c) => {
					object title;
					c.Specificulture = this.Specificulture;
					Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel readViewModel = c;
					Mix.Cms.Lib.ViewModels.MixPages.ReadViewModel page = c.Page;
					if (page != null)
					{
						title = page.Title;
					}
					else
					{
						title = null;
					}
					if (title == null)
					{
						title = c.Description;
					}
					readViewModel.Description = (string)title;
					c.IsActived = ViewModelBase<MixCmsContext, MixPagePost, Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel>.Repository.CheckIsExists((MixPagePost n) => {
						if (n.PageId != c.PageId)
						{
							return false;
						}
						return n.PostId == this.Id;
					}, _context, _transaction);
				});
			}
		}

		private void LoadRelatedPost(MixCmsContext _context, IDbContextTransaction _transaction)
		{
			this.PostNavs = this.GetRelated(_context, _transaction);
		}

		private void LoadTemplates(MixCmsContext _context, IDbContextTransaction _transaction)
		{
			string fileFolder;
			string fileName;
			this.Templates = this.Templates ?? ViewModelBase<MixCmsContext, MixTemplate, Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel>.Repository.GetModelListBy((MixTemplate t) => t.Theme.Id == this.ActivedTheme && t.FolderType == this.TemplateFolderType, null, null).get_Data();
			this.View = Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel.GetTemplateByPath(this.Template, this.Specificulture, MixEnums.EnumTemplateFolder.Posts, _context, _transaction);
			string[] strArrays = new string[2];
			Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel view = this.View;
			if (view != null)
			{
				fileFolder = view.FileFolder;
			}
			else
			{
				fileFolder = null;
			}
			strArrays[0] = fileFolder;
			Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel updateViewModel = this.View;
			if (updateViewModel != null)
			{
				fileName = updateViewModel.FileName;
			}
			else
			{
				fileName = null;
			}
			strArrays[1] = fileName;
			this.Template = CommonHelper.GetFullPath(strArrays);
		}

		public override MixPost ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			DateTime valueOrDefault;
			DateTime? nullable;
			string str;
			if (this.Id == 0)
			{
				this.Id = ViewModelBase<MixCmsContext, MixPost, Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel>.Repository.Max((MixPost c) => c.Id, _context, _transaction).get_Data() + 1;
				this.CreatedDateTime = DateTime.UtcNow;
			}
			this.LastModified = new DateTime?(DateTime.UtcNow);
			DateTime? publishedDateTime = this.PublishedDateTime;
			if (publishedDateTime.HasValue)
			{
				valueOrDefault = publishedDateTime.GetValueOrDefault();
				nullable = new DateTime?(valueOrDefault.ToUniversalTime());
			}
			else
			{
				nullable = null;
			}
			this.PublishedDateTime = nullable;
			this.ExtraFields = ((this.Columns != null ? JArray.Parse(JsonConvert.SerializeObject(
				from c in this.Columns
				orderby c.Priority
				where !string.IsNullOrEmpty(c.Name)
				select c)) : new JArray())).ToString(0, Array.Empty<JsonConverter>());
			if (this.Properties != null && this.Properties.Count > 0)
			{
				JArray jArray = new JArray();
				foreach (ExtraProperty extraProperty in this.Properties.Where<ExtraProperty>((ExtraProperty p) => {
					if (string.IsNullOrEmpty(p.Value))
					{
						return false;
					}
					return !string.IsNullOrEmpty(p.Name);
				}))
				{
					jArray.Add(JObject.FromObject(extraProperty));
				}
				string str1 = jArray.ToString(0, Array.Empty<JsonConverter>());
				if (str1 != null)
				{
					str = str1.Trim();
				}
				else
				{
					str = null;
				}
				this.ExtraProperties = str;
			}
			this.Template = (this.View != null ? string.Format("{0}/{1}{2}", this.View.FolderType, this.View.FileName, this.View.Extension) : this.Template);
			if (this.ThumbnailFileStream != null)
			{
				string[] strArrays = new string[] { "Content/Uploads", "Posts", null };
				valueOrDefault = DateTime.UtcNow;
				strArrays[2] = valueOrDefault.ToString("dd-MM-yyyy");
				string fullPath = CommonHelper.GetFullPath(strArrays);
				string randomName = CommonHelper.GetRandomName(this.ThumbnailFileStream.get_Name());
				if (CommonHelper.SaveFileBase64(fullPath, randomName, this.ThumbnailFileStream.get_Base64()))
				{
					CommonHelper.RemoveFile(this.Thumbnail);
					this.Thumbnail = CommonHelper.GetFullPath(new string[] { fullPath, randomName });
				}
			}
			if (this.ImageFileStream != null)
			{
				string[] strArrays1 = new string[] { "Content/Uploads", "Posts", null };
				valueOrDefault = DateTime.UtcNow;
				strArrays1[2] = valueOrDefault.ToString("dd-MM-yyyy");
				string fullPath1 = CommonHelper.GetFullPath(strArrays1);
				string randomName1 = CommonHelper.GetRandomName(this.ImageFileStream.get_Name());
				if (CommonHelper.SaveFileBase64(fullPath1, randomName1, this.ImageFileStream.get_Base64()))
				{
					CommonHelper.RemoveFile(this.Image);
					this.Image = CommonHelper.GetFullPath(new string[] { fullPath1, randomName1 });
				}
			}
			if (!string.IsNullOrEmpty(this.Image) && this.Image[0] == '/')
			{
				this.Image = this.Image.Substring(1);
			}
			if (!string.IsNullOrEmpty(this.Thumbnail) && this.Thumbnail[0] == '/')
			{
				this.Thumbnail = this.Thumbnail.Substring(1);
			}
			this.Tags = this.ListTag.ToString(0, Array.Empty<JsonConverter>());
			this.GenerateSEO();
			return base.ParseModel(_context, _transaction);
		}

		public override RepositoryResponse<bool> RemoveRelatedModels(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel view, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			ParameterExpression parameterExpression;
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			if (repositoryResponse1.get_IsSucceed())
			{
				DbSet<MixPagePost> mixPagePost = _context.MixPagePost;
				parameterExpression = Expression.Parameter(typeof(MixPagePost), "n");
				foreach (MixPagePost list in mixPagePost.Where<MixPagePost>(Expression.Lambda<Func<MixPagePost, bool>>(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPagePost).GetMethod("get_PostId").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel).GetMethod("get_Id").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPagePost).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel).GetMethod("get_Specificulture").MethodHandle)))), new ParameterExpression[] { parameterExpression })).ToList<MixPagePost>())
				{
					_context.Entry<MixPagePost>(list).set_State(2);
				}
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				DbSet<MixModulePost> mixModulePost = _context.MixModulePost;
				parameterExpression = Expression.Parameter(typeof(MixModulePost), "n");
				foreach (MixModulePost list1 in mixModulePost.Where<MixModulePost>(Expression.Lambda<Func<MixModulePost, bool>>(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixModulePost).GetMethod("get_PostId").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel).GetMethod("get_Id").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixModulePost).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel).GetMethod("get_Specificulture").MethodHandle)))), new ParameterExpression[] { parameterExpression })).ToList<MixModulePost>())
				{
					_context.Entry<MixModulePost>(list1).set_State(2);
				}
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				DbSet<MixPostMedia> mixPostMedia = _context.MixPostMedia;
				parameterExpression = Expression.Parameter(typeof(MixPostMedia), "n");
				foreach (MixPostMedia mixPostMedium in mixPostMedia.Where<MixPostMedia>(Expression.Lambda<Func<MixPostMedia, bool>>(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPostMedia).GetMethod("get_PostId").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel).GetMethod("get_Id").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPostMedia).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel).GetMethod("get_Specificulture").MethodHandle)))), new ParameterExpression[] { parameterExpression })).ToList<MixPostMedia>())
				{
					_context.Entry<MixPostMedia>(mixPostMedium).set_State(2);
				}
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				DbSet<MixPostModule> mixPostModule = _context.MixPostModule;
				parameterExpression = Expression.Parameter(typeof(MixPostModule), "n");
				foreach (MixPostModule mixPostModule1 in mixPostModule.Where<MixPostModule>(Expression.Lambda<Func<MixPostModule, bool>>(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPostModule).GetMethod("get_PostId").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel).GetMethod("get_Id").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPostModule).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel).GetMethod("get_Specificulture").MethodHandle)))), new ParameterExpression[] { parameterExpression })).ToList<MixPostModule>())
				{
					_context.Entry<MixPostModule>(mixPostModule1).set_State(2);
				}
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				DbSet<MixPostMedia> dbSet = _context.MixPostMedia;
				parameterExpression = Expression.Parameter(typeof(MixPostMedia), "n");
				foreach (MixPostMedia mixPostMedium1 in dbSet.Where<MixPostMedia>(Expression.Lambda<Func<MixPostMedia, bool>>(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPostMedia).GetMethod("get_PostId").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel).GetMethod("get_Id").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPostMedia).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel).GetMethod("get_Specificulture").MethodHandle)))), new ParameterExpression[] { parameterExpression })).ToList<MixPostMedia>())
				{
					_context.Entry<MixPostMedia>(mixPostMedium1).set_State(2);
				}
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				DbSet<MixUrlAlias> mixUrlAlias = _context.MixUrlAlias;
				parameterExpression = Expression.Parameter(typeof(MixUrlAlias), "n");
				foreach (MixUrlAlias mixUrlAlia in mixUrlAlias.Where<MixUrlAlias>(Expression.Lambda<Func<MixUrlAlias, bool>>(Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixUrlAlias).GetMethod("get_SourceId").MethodHandle)), Expression.Call(Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel).GetMethod("get_Id").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(int).GetMethod("ToString").MethodHandle), Array.Empty<Expression>())), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixUrlAlias).GetMethod("get_Type").MethodHandle)), Expression.Constant(1, typeof(int)))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixUrlAlias).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel).GetMethod("get_Specificulture").MethodHandle)))), new ParameterExpression[] { parameterExpression })).ToList<MixUrlAlias>())
				{
					_context.Entry<MixUrlAlias>(mixUrlAlia).set_State(2);
				}
			}
			repositoryResponse1.set_IsSucceed(_context.SaveChanges() > 0);
			return repositoryResponse1;
		}

		public override async Task<RepositoryResponse<bool>> RemoveRelatedModelsAsync(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel view, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			ParameterExpression parameterExpression;
			CancellationToken cancellationToken;
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			if (repositoryResponse1.get_IsSucceed())
			{
				DbSet<MixPagePost> mixPagePost = _context.MixPagePost;
				parameterExpression = Expression.Parameter(typeof(MixPagePost), "n");
				BinaryExpression binaryExpression = Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPagePost).GetMethod("get_PostId").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel).GetMethod("get_Id").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPagePost).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel).GetMethod("get_Specificulture").MethodHandle))));
				ParameterExpression[] parameterExpressionArray = new ParameterExpression[] { parameterExpression };
				IQueryable<MixPagePost> mixPagePosts = mixPagePost.Where<MixPagePost>(Expression.Lambda<Func<MixPagePost, bool>>(binaryExpression, parameterExpressionArray));
				cancellationToken = new CancellationToken();
				foreach (MixPagePost listAsync in await EntityFrameworkQueryableExtensions.ToListAsync<MixPagePost>(mixPagePosts, cancellationToken))
				{
					_context.Entry<MixPagePost>(listAsync).set_State(2);
				}
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				DbSet<MixModulePost> mixModulePost = _context.MixModulePost;
				parameterExpression = Expression.Parameter(typeof(MixModulePost), "n");
				BinaryExpression binaryExpression1 = Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixModulePost).GetMethod("get_PostId").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel).GetMethod("get_Id").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixModulePost).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel).GetMethod("get_Specificulture").MethodHandle))));
				ParameterExpression[] parameterExpressionArray1 = new ParameterExpression[] { parameterExpression };
				IQueryable<MixModulePost> mixModulePosts = mixModulePost.Where<MixModulePost>(Expression.Lambda<Func<MixModulePost, bool>>(binaryExpression1, parameterExpressionArray1));
				cancellationToken = new CancellationToken();
				foreach (MixModulePost listAsync1 in await EntityFrameworkQueryableExtensions.ToListAsync<MixModulePost>(mixModulePosts, cancellationToken))
				{
					_context.Entry<MixModulePost>(listAsync1).set_State(2);
				}
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				DbSet<MixPostMedia> mixPostMedia = _context.MixPostMedia;
				parameterExpression = Expression.Parameter(typeof(MixPostMedia), "n");
				BinaryExpression binaryExpression2 = Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPostMedia).GetMethod("get_PostId").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel).GetMethod("get_Id").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPostMedia).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel).GetMethod("get_Specificulture").MethodHandle))));
				ParameterExpression[] parameterExpressionArray2 = new ParameterExpression[] { parameterExpression };
				IQueryable<MixPostMedia> mixPostMedias = mixPostMedia.Where<MixPostMedia>(Expression.Lambda<Func<MixPostMedia, bool>>(binaryExpression2, parameterExpressionArray2));
				cancellationToken = new CancellationToken();
				foreach (MixPostMedia mixPostMedium in await EntityFrameworkQueryableExtensions.ToListAsync<MixPostMedia>(mixPostMedias, cancellationToken))
				{
					_context.Entry<MixPostMedia>(mixPostMedium).set_State(2);
				}
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				DbSet<MixPostModule> mixPostModule = _context.MixPostModule;
				parameterExpression = Expression.Parameter(typeof(MixPostModule), "n");
				BinaryExpression binaryExpression3 = Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPostModule).GetMethod("get_PostId").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel).GetMethod("get_Id").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPostModule).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel).GetMethod("get_Specificulture").MethodHandle))));
				ParameterExpression[] parameterExpressionArray3 = new ParameterExpression[] { parameterExpression };
				IQueryable<MixPostModule> mixPostModules = mixPostModule.Where<MixPostModule>(Expression.Lambda<Func<MixPostModule, bool>>(binaryExpression3, parameterExpressionArray3));
				cancellationToken = new CancellationToken();
				foreach (MixPostModule mixPostModule1 in await EntityFrameworkQueryableExtensions.ToListAsync<MixPostModule>(mixPostModules, cancellationToken))
				{
					_context.Entry<MixPostModule>(mixPostModule1).set_State(2);
				}
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				DbSet<MixPostMedia> dbSet = _context.MixPostMedia;
				parameterExpression = Expression.Parameter(typeof(MixPostMedia), "n");
				BinaryExpression binaryExpression4 = Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPostMedia).GetMethod("get_PostId").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel).GetMethod("get_Id").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPostMedia).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel).GetMethod("get_Specificulture").MethodHandle))));
				ParameterExpression[] parameterExpressionArray4 = new ParameterExpression[] { parameterExpression };
				IQueryable<MixPostMedia> mixPostMedias1 = dbSet.Where<MixPostMedia>(Expression.Lambda<Func<MixPostMedia, bool>>(binaryExpression4, parameterExpressionArray4));
				cancellationToken = new CancellationToken();
				foreach (MixPostMedia mixPostMedium1 in await EntityFrameworkQueryableExtensions.ToListAsync<MixPostMedia>(mixPostMedias1, cancellationToken))
				{
					_context.Entry<MixPostMedia>(mixPostMedium1).set_State(2);
				}
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				DbSet<MixUrlAlias> mixUrlAlias = _context.MixUrlAlias;
				parameterExpression = Expression.Parameter(typeof(MixUrlAlias), "n");
				BinaryExpression binaryExpression5 = Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixUrlAlias).GetMethod("get_SourceId").MethodHandle)), Expression.Call(Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel).GetMethod("get_Id").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(int).GetMethod("ToString").MethodHandle), Array.Empty<Expression>())), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixUrlAlias).GetMethod("get_Type").MethodHandle)), Expression.Constant(1, typeof(int)))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixUrlAlias).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel).GetMethod("get_Specificulture").MethodHandle))));
				ParameterExpression[] parameterExpressionArray5 = new ParameterExpression[] { parameterExpression };
				IQueryable<MixUrlAlias> mixUrlAliases = mixUrlAlias.Where<MixUrlAlias>(Expression.Lambda<Func<MixUrlAlias, bool>>(binaryExpression5, parameterExpressionArray5));
				cancellationToken = new CancellationToken();
				foreach (MixUrlAlias mixUrlAlia in await EntityFrameworkQueryableExtensions.ToListAsync<MixUrlAlias>(mixUrlAliases, cancellationToken))
				{
					_context.Entry<MixUrlAlias>(mixUrlAlia).set_State(2);
				}
			}
			MixCmsContext mixCmsContext = _context;
			cancellationToken = new CancellationToken();
			await ((DbContext)mixCmsContext).SaveChangesAsync(cancellationToken);
			return repositoryResponse1;
		}

		private RepositoryResponse<bool> SaveAttribute(int parentId, MixCmsContext context, IDbContextTransaction transaction)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			this.AttributeData.ParentId = parentId.ToString();
			this.AttributeData.ParentType = MixEnums.MixAttributeSetDataType.Post;
			ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel>(this.AttributeData.SaveModel(true, context, transaction), ref repositoryResponse1);
			foreach (Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.FormViewModel sysCategory in this.SysCategories)
			{
				if (!repositoryResponse1.get_IsSucceed())
				{
					continue;
				}
				sysCategory.ParentId = parentId.ToString();
				sysCategory.ParentType = MixEnums.MixAttributeSetDataType.Post;
				sysCategory.Specificulture = this.Specificulture;
				ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.FormViewModel>(sysCategory.SaveModel(false, context, transaction), ref repositoryResponse1);
			}
			foreach (Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.FormViewModel sysTag in this.SysTags)
			{
				if (!repositoryResponse1.get_IsSucceed())
				{
					continue;
				}
				sysTag.ParentId = parentId.ToString();
				sysTag.ParentType = MixEnums.MixAttributeSetDataType.Post;
				sysTag.Specificulture = this.Specificulture;
				ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.FormViewModel>(sysTag.SaveModel(false, context, transaction), ref repositoryResponse1);
			}
			return repositoryResponse1;
		}

		private async Task<RepositoryResponse<bool>> SaveAttributeAsync(int parentId, MixCmsContext context, IDbContextTransaction transaction)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			this.AttributeData.ParentId = parentId.ToString();
			this.AttributeData.ParentType = MixEnums.MixAttributeSetDataType.Post;
			RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel> repositoryResponse2 = await this.AttributeData.Data.SaveModelAsync(true, context, transaction);
			ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel>(repositoryResponse2, ref repositoryResponse1);
			if (repositoryResponse1.get_IsSucceed())
			{
				this.AttributeData.DataId = repositoryResponse2.get_Data().Id;
				ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel>(await this.AttributeData.SaveModelAsync(true, context, transaction), ref repositoryResponse1);
			}
			foreach (Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.FormViewModel sysCategory in this.SysCategories)
			{
				if (!repositoryResponse1.get_IsSucceed())
				{
					continue;
				}
				sysCategory.ParentId = parentId.ToString();
				sysCategory.ParentType = MixEnums.MixAttributeSetDataType.Post;
				sysCategory.Specificulture = this.Specificulture;
				ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.FormViewModel>(await sysCategory.SaveModelAsync(false, context, transaction), ref repositoryResponse1);
			}
			foreach (Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.FormViewModel sysTag in this.SysTags)
			{
				if (!repositoryResponse1.get_IsSucceed())
				{
					continue;
				}
				sysTag.ParentId = parentId.ToString();
				sysTag.ParentType = MixEnums.MixAttributeSetDataType.Post;
				sysTag.Specificulture = this.Specificulture;
				ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.FormViewModel>(await sysTag.SaveModelAsync(false, context, transaction), ref repositoryResponse1);
			}
			return repositoryResponse1;
		}

		private RepositoryResponse<bool> SaveMedias(int id, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			foreach (Mix.Cms.Lib.ViewModels.MixPostMedias.ReadViewModel mediaNav in this.MediaNavs)
			{
				mediaNav.PostId = id;
				mediaNav.Specificulture = this.Specificulture;
				if (!mediaNav.IsActived)
				{
					ViewModelHelper.HandleResult<MixPostMedia>(mediaNav.RemoveModel(false, _context, _transaction), ref repositoryResponse1);
				}
				else
				{
					ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixPostMedias.ReadViewModel>(mediaNav.SaveModel(false, _context, _transaction), ref repositoryResponse1);
				}
			}
			return repositoryResponse1;
		}

		private async Task<RepositoryResponse<bool>> SaveMediasAsync(int id, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel.u003cSaveMediasAsyncu003ed__201 variable = new Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel.u003cSaveMediasAsyncu003ed__201();
			variable.u003cu003e4__this = this;
			variable.id = id;
			variable._context = _context;
			variable._transaction = _transaction;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel.u003cSaveMediasAsyncu003ed__201>(ref variable);
			return variable.u003cu003et__builder.Task;
		}

		private RepositoryResponse<bool> SaveParentModules(int id, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			foreach (Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel module in this.Modules)
			{
				module.PostId = id;
				module.Description = this.Title;
				module.Image = this.ThumbnailUrl;
				module.Status = MixEnums.MixContentStatus.Published;
				if (!module.IsActived)
				{
					RepositoryResponse<MixModulePost> repositoryResponse2 = module.RemoveModel(false, _context, _transaction);
					repositoryResponse1.set_IsSucceed(repositoryResponse2.get_IsSucceed());
					if (repositoryResponse1.get_IsSucceed())
					{
						ViewModelBase<MixCmsContext, MixModule, Mix.Cms.Lib.ViewModels.MixModules.ReadListItemViewModel>.Repository.RemoveCache(module.Module.get_Model(), _context, _transaction);
					}
					else
					{
						repositoryResponse1.set_Exception(repositoryResponse2.get_Exception());
						base.get_Errors().AddRange(repositoryResponse2.get_Errors());
					}
				}
				else
				{
					RepositoryResponse<Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel> repositoryResponse3 = module.SaveModel(false, _context, _transaction);
					repositoryResponse1.set_IsSucceed(repositoryResponse3.get_IsSucceed());
					if (repositoryResponse1.get_IsSucceed())
					{
						ViewModelBase<MixCmsContext, MixModule, Mix.Cms.Lib.ViewModels.MixModules.ReadListItemViewModel>.Repository.RemoveCache(module.Module.get_Model(), _context, _transaction);
					}
					else
					{
						repositoryResponse1.set_Exception(repositoryResponse3.get_Exception());
						base.get_Errors().AddRange(repositoryResponse3.get_Errors());
					}
				}
			}
			return repositoryResponse1;
		}

		private async Task<RepositoryResponse<bool>> SaveParentModulesAsync(int id, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel.u003cSaveParentModulesAsyncu003ed__198 variable = new Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel.u003cSaveParentModulesAsyncu003ed__198();
			variable.u003cu003e4__this = this;
			variable.id = id;
			variable._context = _context;
			variable._transaction = _transaction;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel.u003cSaveParentModulesAsyncu003ed__198>(ref variable);
			return variable.u003cu003et__builder.Task;
		}

		private RepositoryResponse<bool> SaveParentPages(int id, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			foreach (Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel page in this.Pages)
			{
				page.PostId = id;
				page.Description = this.Title;
				page.Image = this.ThumbnailUrl;
				page.Status = MixEnums.MixContentStatus.Published;
				if (!page.IsActived)
				{
					RepositoryResponse<MixPagePost> repositoryResponse2 = page.RemoveModel(false, _context, _transaction);
					repositoryResponse1.set_IsSucceed(repositoryResponse2.get_IsSucceed());
					if (repositoryResponse1.get_IsSucceed())
					{
						ViewModelBase<MixCmsContext, MixPage, Mix.Cms.Lib.ViewModels.MixPages.ReadListItemViewModel>.Repository.RemoveCache(page.Page.get_Model(), _context, _transaction);
					}
					else
					{
						repositoryResponse1.set_Exception(repositoryResponse2.get_Exception());
						base.get_Errors().AddRange(repositoryResponse2.get_Errors());
					}
				}
				else
				{
					RepositoryResponse<Mix.Cms.Lib.ViewModels.MixPagePosts.ReadViewModel> repositoryResponse3 = page.SaveModel(false, _context, _transaction);
					repositoryResponse1.set_IsSucceed(repositoryResponse3.get_IsSucceed());
					if (repositoryResponse1.get_IsSucceed())
					{
						ViewModelBase<MixCmsContext, MixPage, Mix.Cms.Lib.ViewModels.MixPages.ReadListItemViewModel>.Repository.RemoveCache(page.Page.get_Model(), _context, _transaction);
					}
					else
					{
						repositoryResponse1.set_Exception(repositoryResponse3.get_Exception());
						base.get_Errors().AddRange(repositoryResponse3.get_Errors());
					}
				}
			}
			return repositoryResponse1;
		}

		private async Task<RepositoryResponse<bool>> SaveParentPagesAsync(int id, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel.u003cSaveParentPagesAsyncu003ed__199 variable = new Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel.u003cSaveParentPagesAsyncu003ed__199();
			variable.u003cu003e4__this = this;
			variable.id = id;
			variable._context = _context;
			variable._transaction = _transaction;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel.u003cSaveParentPagesAsyncu003ed__199>(ref variable);
			return variable.u003cu003et__builder.Task;
		}

		private RepositoryResponse<bool> SaveRelatedPost(int id, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			foreach (Mix.Cms.Lib.ViewModels.MixPostPosts.ReadViewModel postNav in this.PostNavs)
			{
				postNav.SourceId = id;
				postNav.Status = MixEnums.MixContentStatus.Published;
				postNav.Specificulture = this.Specificulture;
				if (!postNav.IsActived)
				{
					RepositoryResponse<MixRelatedPost> repositoryResponse2 = postNav.RemoveModel(false, _context, _transaction);
					repositoryResponse1.set_IsSucceed(repositoryResponse2.get_IsSucceed());
					if (repositoryResponse1.get_IsSucceed())
					{
						ViewModelBase<MixCmsContext, MixPost, Mix.Cms.Lib.ViewModels.MixPosts.ReadViewModel>.Repository.RemoveCache(postNav.RelatedPost.get_Model(), null, null);
					}
					else
					{
						repositoryResponse1.set_Exception(repositoryResponse2.get_Exception());
						base.get_Errors().AddRange(repositoryResponse2.get_Errors());
					}
				}
				else
				{
					RepositoryResponse<Mix.Cms.Lib.ViewModels.MixPostPosts.ReadViewModel> repositoryResponse3 = postNav.SaveModel(false, _context, _transaction);
					repositoryResponse1.set_IsSucceed(repositoryResponse3.get_IsSucceed());
					if (repositoryResponse1.get_IsSucceed())
					{
						ViewModelBase<MixCmsContext, MixPost, Mix.Cms.Lib.ViewModels.MixPosts.ReadViewModel>.Repository.RemoveCache(postNav.RelatedPost.get_Model(), null, null);
					}
					else
					{
						repositoryResponse1.set_Exception(repositoryResponse3.get_Exception());
						base.get_Errors().AddRange(repositoryResponse3.get_Errors());
					}
				}
			}
			return repositoryResponse1;
		}

		private async Task<RepositoryResponse<bool>> SaveRelatedPostAsync(int id, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel.u003cSaveRelatedPostAsyncu003ed__200 variable = new Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel.u003cSaveRelatedPostAsyncu003ed__200();
			variable.u003cu003e4__this = this;
			variable.id = id;
			variable._context = _context;
			variable._transaction = _transaction;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel.u003cSaveRelatedPostAsyncu003ed__200>(ref variable);
			return variable.u003cu003et__builder.Task;
		}

		public override RepositoryResponse<bool> SaveSubModels(MixPost parent, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<bool> repositoryResponse;
			RepositoryResponse<bool> repositoryResponse1 = new RepositoryResponse<bool>();
			repositoryResponse1.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse2 = repositoryResponse1;
			try
			{
				RepositoryResponse<Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel> repositoryResponse3 = this.View.SaveModel(true, _context, _transaction);
				repositoryResponse2.set_IsSucceed((!repositoryResponse2.get_IsSucceed() ? false : repositoryResponse3.get_IsSucceed()));
				ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel>(repositoryResponse3, ref repositoryResponse2);
				if (repositoryResponse2.get_IsSucceed())
				{
					repositoryResponse2 = this.SaveUrlAlias(parent.Id, _context, _transaction);
				}
				if (repositoryResponse2.get_IsSucceed())
				{
					repositoryResponse2 = this.SaveMedias(parent.Id, _context, _transaction);
				}
				if (repositoryResponse2.get_IsSucceed())
				{
					repositoryResponse2 = this.SaveAttribute(parent.Id, _context, _transaction);
				}
				if (repositoryResponse2.get_IsSucceed())
				{
					repositoryResponse2 = this.SaveRelatedPost(parent.Id, _context, _transaction);
				}
				if (repositoryResponse2.get_IsSucceed())
				{
					repositoryResponse2 = this.SaveParentPages(parent.Id, _context, _transaction);
				}
				if (repositoryResponse2.get_IsSucceed())
				{
					repositoryResponse2 = this.SaveParentModules(parent.Id, _context, _transaction);
				}
				repositoryResponse = repositoryResponse2;
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				repositoryResponse2.set_IsSucceed(false);
				repositoryResponse2.set_Exception(exception);
				repositoryResponse = repositoryResponse2;
			}
			return repositoryResponse;
		}

		public override async Task<RepositoryResponse<bool>> SaveSubModelsAsync(MixPost parent, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel.u003cSaveSubModelsAsyncu003ed__196 variable = new Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel.u003cSaveSubModelsAsyncu003ed__196();
			variable.u003cu003e4__this = this;
			variable.parent = parent;
			variable._context = _context;
			variable._transaction = _transaction;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel.u003cSaveSubModelsAsyncu003ed__196>(ref variable);
			return variable.u003cu003et__builder.Task;
		}

		private RepositoryResponse<bool> SaveUrlAlias(int parentId, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			foreach (Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel urlAlias in this.UrlAliases)
			{
				urlAlias.SourceId = parentId.ToString();
				urlAlias.Type = MixEnums.UrlAliasType.Post;
				urlAlias.Specificulture = this.Specificulture;
				ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel>(((ViewModelBase<MixCmsContext, MixUrlAlias, Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel>)urlAlias).SaveModel(false, _context, _transaction), ref repositoryResponse1);
				if (repositoryResponse1.get_IsSucceed())
				{
					continue;
				}
				return repositoryResponse1;
			}
			return repositoryResponse1;
		}

		private async Task<RepositoryResponse<bool>> SaveUrlAliasAsync(int parentId, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel.u003cSaveUrlAliasAsyncu003ed__202 variable = new Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel.u003cSaveUrlAliasAsyncu003ed__202();
			variable.u003cu003e4__this = this;
			variable.parentId = parentId;
			variable._context = _context;
			variable._transaction = _transaction;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixPosts.UpdateViewModel.u003cSaveUrlAliasAsyncu003ed__202>(ref variable);
			return variable.u003cu003et__builder.Task;
		}
	}
}