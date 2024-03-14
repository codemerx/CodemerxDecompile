using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.Services;
using Mix.Cms.Lib.ViewModels.MixAttributeSetDatas;
using Mix.Cms.Lib.ViewModels.MixAttributeSets;
using Mix.Cms.Lib.ViewModels.MixModules;
using Mix.Cms.Lib.ViewModels.MixPostMedias;
using Mix.Cms.Lib.ViewModels.MixPostModules;
using Mix.Cms.Lib.ViewModels.MixPostPosts;
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

namespace Mix.Cms.Lib.ViewModels.MixPosts
{
	public class ReadMvcViewModel : ViewModelBase<MixCmsContext, MixPost, Mix.Cms.Lib.ViewModels.MixPosts.ReadMvcViewModel>
	{
		[JsonProperty("attributeData")]
		public Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.ReadMvcViewModel AttributeData
		{
			get;
			set;
		}

		[JsonProperty("attributeSets")]
		public List<Mix.Cms.Lib.ViewModels.MixAttributeSets.ReadViewModel> AttributeSets { get; set; } = new List<Mix.Cms.Lib.ViewModels.MixAttributeSets.ReadViewModel>();

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
		public JArray ListTag
		{
			get
			{
				return JArray.Parse(this.Tags ?? "[]");
			}
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
		public List<Mix.Cms.Lib.ViewModels.MixModules.ReadMvcViewModel> Modules
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
		public MixEnums.MixContentStatus Type
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

		public ReadMvcViewModel(MixPost model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.View = Mix.Cms.Lib.ViewModels.MixTemplates.ReadListItemViewModel.GetTemplateByPath(this.Template, this.Specificulture, _context, _transaction).get_Data();
			this.LoadAttributes(_context, _transaction);
			this.LoadTags(_context, _transaction);
			RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixPostMedias.ReadViewModel>> modelListBy = ViewModelBase<MixCmsContext, MixPostMedia, Mix.Cms.Lib.ViewModels.MixPostMedias.ReadViewModel>.Repository.GetModelListBy((MixPostMedia n) => n.PostId == this.Id && n.Specificulture == this.Specificulture, _context, _transaction);
			if (modelListBy.get_IsSucceed())
			{
				this.MediaNavs = (
					from p in modelListBy.get_Data()
					orderby p.Priority
					select p).ToList<Mix.Cms.Lib.ViewModels.MixPostMedias.ReadViewModel>();
				this.MediaNavs.ForEach((Mix.Cms.Lib.ViewModels.MixPostMedias.ReadViewModel n) => n.IsActived = true);
			}
			RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixPostModules.ReadViewModel>> repositoryResponse = ViewModelBase<MixCmsContext, MixPostModule, Mix.Cms.Lib.ViewModels.MixPostModules.ReadViewModel>.Repository.GetModelListBy((MixPostModule n) => n.PostId == this.Id && n.Specificulture == this.Specificulture, _context, _transaction);
			if (repositoryResponse.get_IsSucceed())
			{
				this.ModuleNavs = (
					from p in repositoryResponse.get_Data()
					orderby p.Priority
					select p).ToList<Mix.Cms.Lib.ViewModels.MixPostModules.ReadViewModel>();
				foreach (Mix.Cms.Lib.ViewModels.MixPostModules.ReadViewModel moduleNav in this.ModuleNavs)
				{
					moduleNav.IsActived = true;
					int? nullable = null;
					int? nullable1 = nullable;
					nullable = null;
					int? nullable2 = nullable;
					nullable = null;
					moduleNav.Module.LoadData(new int?(this.Id), nullable1, nullable2, nullable, new int?(0), _context, _transaction);
				}
			}
			this.PostNavs = ViewModelBase<MixCmsContext, MixRelatedPost, Mix.Cms.Lib.ViewModels.MixPostPosts.ReadViewModel>.Repository.GetModelListBy((MixRelatedPost n) => n.SourceId == this.Id && n.Specificulture == this.Specificulture, _context, _transaction).get_Data();
		}

		public Mix.Cms.Lib.ViewModels.MixAttributeSets.ReadViewModel GetAttributeSet(string name)
		{
			return this.AttributeSets.FirstOrDefault<Mix.Cms.Lib.ViewModels.MixAttributeSets.ReadViewModel>((Mix.Cms.Lib.ViewModels.MixAttributeSets.ReadViewModel m) => m.Name == name);
		}

		public Mix.Cms.Lib.ViewModels.MixModules.ReadMvcViewModel GetModule(string name)
		{
			Mix.Cms.Lib.ViewModels.MixPostModules.ReadViewModel readViewModel = this.ModuleNavs.FirstOrDefault<Mix.Cms.Lib.ViewModels.MixPostModules.ReadViewModel>((Mix.Cms.Lib.ViewModels.MixPostModules.ReadViewModel m) => m.Module.Name == name);
			if (readViewModel != null)
			{
				return readViewModel.Module;
			}
			return null;
		}

		private void LoadAttributes(MixCmsContext _context, IDbContextTransaction _transaction)
		{
			Mix.Cms.Lib.ViewModels.MixPosts.ReadMvcViewModel.u003cu003ec__DisplayClass158_0 variable = null;
			DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixAttributeSet, Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel>.Repository;
			ParameterExpression parameterExpression = Expression.Parameter(typeof(MixAttributeSet), "m");
			RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel> singleModel = repository.GetSingleModel(Expression.Lambda<Func<MixAttributeSet, bool>>(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSet).GetMethod("get_Name").MethodHandle)), Expression.Constant("sys_additional_field_post", typeof(string))), new ParameterExpression[] { parameterExpression }), _context, _transaction);
			if (singleModel.get_IsSucceed())
			{
				DefaultRepository<!0, !1, !2> defaultRepository = ViewModelBase<MixCmsContext, MixRelatedAttributeData, Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.ReadMvcViewModel>.Repository;
				parameterExpression = Expression.Parameter(typeof(MixRelatedAttributeData), "a");
				this.AttributeData = defaultRepository.GetFirstModel(Expression.Lambda<Func<MixRelatedAttributeData, bool>>(Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_ParentId").MethodHandle)), Expression.Call(Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPosts.ReadMvcViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.ReadMvcViewModel).GetMethod("get_Id").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(int).GetMethod("ToString").MethodHandle), Array.Empty<Expression>())), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPosts.ReadMvcViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.ReadMvcViewModel).GetMethod("get_Specificulture").MethodHandle)))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_AttributeSetId").MethodHandle)), Expression.Property(Expression.Property(Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.MixPosts.ReadMvcViewModel.u003cu003ec__DisplayClass158_0)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.ReadMvcViewModel.u003cu003ec__DisplayClass158_0).GetField("getAttrs").FieldHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel>).GetMethod("get_Data").MethodHandle, typeof(RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel>).TypeHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel).GetMethod("get_Id").MethodHandle)))), new ParameterExpression[] { parameterExpression }), _context, _transaction).get_Data();
			}
		}

		private void LoadTags(MixCmsContext context, IDbContextTransaction transaction)
		{
			DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixRelatedAttributeData, Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.FormViewModel>.Repository;
			ParameterExpression parameterExpression = Expression.Parameter(typeof(MixRelatedAttributeData), "m");
			RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.FormViewModel>> modelListBy = repository.GetModelListBy(Expression.Lambda<Func<MixRelatedAttributeData, bool>>(Expression.AndAlso(Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPosts.ReadMvcViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.ReadMvcViewModel).GetMethod("get_Specificulture").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_ParentId").MethodHandle)), Expression.Call(Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPosts.ReadMvcViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPosts.ReadMvcViewModel).GetMethod("get_Id").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(int).GetMethod("ToString").MethodHandle), Array.Empty<Expression>()))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_ParentType").MethodHandle)), Expression.Call(Expression.Constant(MixEnums.MixAttributeSetDataType.Post, typeof(MixEnums.MixAttributeSetDataType)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(object).GetMethod("ToString").MethodHandle), Array.Empty<Expression>()))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_AttributeSetName").MethodHandle)), Expression.Constant("sys_tag", typeof(string)))), new ParameterExpression[] { parameterExpression }), context, transaction);
			if (modelListBy.get_IsSucceed())
			{
				this.SysTags = modelListBy.get_Data();
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