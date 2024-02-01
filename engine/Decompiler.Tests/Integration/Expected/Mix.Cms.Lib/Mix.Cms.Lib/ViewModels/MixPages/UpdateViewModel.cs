using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.Services;
using Mix.Cms.Lib.ViewModels.MixAttributeFields;
using Mix.Cms.Lib.ViewModels.MixAttributeSetDatas;
using Mix.Cms.Lib.ViewModels.MixAttributeSets;
using Mix.Cms.Lib.ViewModels.MixAttributeSetValues;
using Mix.Cms.Lib.ViewModels.MixModules;
using Mix.Cms.Lib.ViewModels.MixPageModules;
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

		[JsonProperty("listTag")]
		public JArray ListTag { get; set; } = new JArray();

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
				return CommonHelper.GetFullPath(new string[] { "Views/Shared/Templates", MixService.GetConfig<string>("ThemeName", this.Specificulture), this.TemplateFolderType });
			}
		}

		[JsonIgnore]
		public string TemplateFolderType
		{
			get
			{
				return MixEnums.EnumTemplateFolder.Pages.ToString();
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
		}

		public UpdateViewModel(MixPage model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			object obj;
			string fileFolder;
			string fileName;
			object obj1;
			string str;
			string fileName1;
			string extension;
			this.Cultures = Mix.Cms.Lib.ViewModels.MixPages.Helper.LoadCultures(this.Id, this.Specificulture, _context, _transaction);
			if (!string.IsNullOrEmpty(this.Tags))
			{
				this.ListTag = JArray.Parse(this.Tags);
			}
			this.LoadAttributes(_context, _transaction);
			DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixTemplate, Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel>.Repository;
			ParameterExpression parameterExpression = Expression.Parameter(typeof(MixTemplate), "t");
			this.Templates = repository.GetModelListBy(Expression.Lambda<Func<MixTemplate, bool>>(Expression.AndAlso(Expression.Equal(Expression.Property(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixTemplate).GetMethod("get_Theme").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixTheme).GetMethod("get_Id").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel).GetMethod("get_ActivedTheme").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixTemplate).GetMethod("get_FolderType").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel).GetMethod("get_TemplateFolderType").MethodHandle)))), new ParameterExpression[] { parameterExpression }), _context, _transaction).get_Data();
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
			string str1 = (string)obj;
			this.View = this.Templates.FirstOrDefault<Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel>((Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel t) => {
				if (string.IsNullOrEmpty(str1))
				{
					return false;
				}
				return str1.Equals(string.Concat(t.FileName, t.Extension));
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
			parameterExpression = Expression.Parameter(typeof(MixTemplate), "t");
			this.Masters = defaultRepository.GetModelListBy(Expression.Lambda<Func<MixTemplate, bool>>(Expression.AndAlso(Expression.Equal(Expression.Property(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixTemplate).GetMethod("get_Theme").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixTheme).GetMethod("get_Id").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel).GetMethod("get_ActivedTheme").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixTemplate).GetMethod("get_FolderType").MethodHandle)), Expression.Call(Expression.Constant(MixEnums.EnumTemplateFolder.Masters, typeof(MixEnums.EnumTemplateFolder)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(object).GetMethod("ToString").MethodHandle), Array.Empty<Expression>()))), new ParameterExpression[] { parameterExpression }), _context, _transaction).get_Data();
			string layout = this.Layout;
			if (layout != null)
			{
				obj1 = layout.Substring(this.Layout.LastIndexOf('/') + 1);
			}
			else
			{
				obj1 = null;
			}
			if (obj1 == null)
			{
				obj1 = "_Layout.cshtml";
			}
			string str2 = (string)obj1;
			this.Master = this.Masters.FirstOrDefault<Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel>((Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel t) => {
				if (string.IsNullOrEmpty(str2))
				{
					return false;
				}
				return str2.Equals(string.Concat(t.FileName, t.Extension));
			});
			if (this.Master == null)
			{
				this.Master = this.Masters.FirstOrDefault<Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel>((Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel t) => "_Layout.cshtml".Equals(string.Concat(t.FileName, t.Extension)));
			}
			Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel master = this.Master;
			if (master != null)
			{
				str = master.FileFolder;
			}
			else
			{
				str = null;
			}
			Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel master1 = this.Master;
			if (master1 != null)
			{
				fileName1 = master1.FileName;
			}
			else
			{
				fileName1 = null;
			}
			Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel updateViewModel1 = this.Master;
			if (updateViewModel1 != null)
			{
				extension = updateViewModel1.Extension;
			}
			else
			{
				extension = null;
			}
			this.Layout = string.Concat(str, "/", fileName1, extension);
			this.ModuleNavs = this.GetModuleNavs(_context, _transaction);
			this.UrlAliases = this.GetAliases(_context, _transaction);
		}

		private void GenerateSEO()
		{
			if (string.IsNullOrEmpty(this.SeoName))
			{
				this.SeoName = SeoHelper.GetSEOString(this.Title, '-');
			}
			int num = 1;
			string seoName = this.SeoName;
			while (ViewModelBase<MixCmsContext, MixPage, Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel>.Repository.CheckIsExists((MixPage a) => {
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
			RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel>> modelListBy = repository.GetModelListBy(Expression.Lambda<Func<MixUrlAlias, bool>>(Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixUrlAlias).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel).GetMethod("get_Specificulture").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixUrlAlias).GetMethod("get_SourceId").MethodHandle)), Expression.Call(Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel).GetMethod("get_Id").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(int).GetMethod("ToString").MethodHandle), Array.Empty<Expression>()))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixUrlAlias).GetMethod("get_Type").MethodHandle)), Expression.Constant(0, typeof(int)))), new ParameterExpression[] { parameterExpression }), context, transaction);
			if (!modelListBy.get_IsSucceed() || modelListBy.get_Data() == null)
			{
				return new List<Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel>();
			}
			return modelListBy.get_Data();
		}

		public List<Mix.Cms.Lib.ViewModels.MixPageModules.ReadMvcViewModel> GetModuleNavs(MixCmsContext context, IDbContextTransaction transaction)
		{
			List<Mix.Cms.Lib.ViewModels.MixPageModules.ReadMvcViewModel> data = ViewModelBase<MixCmsContext, MixPageModule, Mix.Cms.Lib.ViewModels.MixPageModules.ReadMvcViewModel>.Repository.GetModelListBy((MixPageModule m) => m.PageId == this.Id && m.Specificulture == this.Specificulture, null, null).get_Data();
			data.ForEach((Mix.Cms.Lib.ViewModels.MixPageModules.ReadMvcViewModel nav) => nav.IsActived = true);
			IEnumerable<int> moduleId = 
				from m in data
				select m.ModuleId;
			foreach (Mix.Cms.Lib.ViewModels.MixModules.ReadListItemViewModel datum in ViewModelBase<MixCmsContext, MixModule, Mix.Cms.Lib.ViewModels.MixModules.ReadListItemViewModel>.Repository.GetModelListBy((MixModule m) => m.Specificulture == this.Specificulture && !moduleId.Any<int>((int r) => r == m.Id), null, null).get_Data())
			{
				data.Add(new Mix.Cms.Lib.ViewModels.MixPageModules.ReadMvcViewModel()
				{
					Specificulture = this.Specificulture,
					PageId = this.Id,
					ModuleId = datum.Id,
					Image = datum.ImageUrl,
					Description = datum.Title
				});
			}
			return (
				from m in data
				orderby m.Priority
				select m).ToList<Mix.Cms.Lib.ViewModels.MixPageModules.ReadMvcViewModel>();
		}

		private void LoadAttributes(MixCmsContext _context, IDbContextTransaction _transaction)
		{
			DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixAttributeSet, Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel>.Repository;
			ParameterExpression parameterExpression = Expression.Parameter(typeof(MixAttributeSet), "m");
			RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel> singleModel = repository.GetSingleModel(Expression.Lambda<Func<MixAttributeSet, bool>>(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSet).GetMethod("get_Name").MethodHandle)), Expression.Constant("sys_additional_field_page", typeof(string))), new ParameterExpression[] { parameterExpression }), _context, _transaction);
			if (singleModel.get_IsSucceed())
			{
				this.Attributes = singleModel.get_Data();
				DefaultRepository<!0, !1, !2> defaultRepository = ViewModelBase<MixCmsContext, MixRelatedAttributeData, Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel>.Repository;
				parameterExpression = Expression.Parameter(typeof(MixRelatedAttributeData), "a");
				this.AttributeData = defaultRepository.GetFirstModel(Expression.Lambda<Func<MixRelatedAttributeData, bool>>(Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_ParentId").MethodHandle)), Expression.Call(Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel).GetMethod("get_Id").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(int).GetMethod("ToString").MethodHandle), Array.Empty<Expression>())), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel).GetMethod("get_Specificulture").MethodHandle)))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_AttributeSetId").MethodHandle)), Expression.Property(Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel).GetMethod("get_Attributes").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel).GetMethod("get_Id").MethodHandle)))), new ParameterExpression[] { parameterExpression }), _context, _transaction).get_Data();
				if (this.AttributeData == null)
				{
					this.AttributeData = new Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel(new MixRelatedAttributeData()
					{
						Specificulture = this.Specificulture,
						ParentType = MixEnums.MixAttributeSetDataType.Page.ToString(),
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
							Priority = updateViewModel.Priority
						};
						this.AttributeData.Data.Values.Add(priority);
					}
					priority.Priority = updateViewModel.Priority;
					priority.Field = updateViewModel;
				}
				DefaultRepository<!0, !1, !2> repository1 = ViewModelBase<MixCmsContext, MixRelatedAttributeData, Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel>.Repository;
				parameterExpression = Expression.Parameter(typeof(MixRelatedAttributeData), "m");
				RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel>> modelListBy = repository1.GetModelListBy(Expression.Lambda<Func<MixRelatedAttributeData, bool>>(Expression.AndAlso(Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel).GetMethod("get_Specificulture").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_ParentId").MethodHandle)), Expression.Call(Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel).GetMethod("get_Id").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(int).GetMethod("ToString").MethodHandle), Array.Empty<Expression>()))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_ParentType").MethodHandle)), Expression.Call(Expression.Constant(MixEnums.MixAttributeSetDataType.Page, typeof(MixEnums.MixAttributeSetDataType)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(object).GetMethod("ToString").MethodHandle), Array.Empty<Expression>()))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_AttributeSetName").MethodHandle)), Expression.Constant("sys_category", typeof(string)))), new ParameterExpression[] { parameterExpression }), _context, _transaction);
				if (modelListBy.get_IsSucceed())
				{
					this.SysCategories = modelListBy.get_Data();
				}
				DefaultRepository<!0, !1, !2> defaultRepository1 = ViewModelBase<MixCmsContext, MixRelatedAttributeData, Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel>.Repository;
				parameterExpression = Expression.Parameter(typeof(MixRelatedAttributeData), "m");
				RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel>> repositoryResponse = defaultRepository1.GetModelListBy(Expression.Lambda<Func<MixRelatedAttributeData, bool>>(Expression.AndAlso(Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel).GetMethod("get_Specificulture").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_ParentId").MethodHandle)), Expression.Call(Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel).GetMethod("get_Id").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(int).GetMethod("ToString").MethodHandle), Array.Empty<Expression>()))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_ParentType").MethodHandle)), Expression.Call(Expression.Constant(MixEnums.MixAttributeSetDataType.Page, typeof(MixEnums.MixAttributeSetDataType)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(object).GetMethod("ToString").MethodHandle), Array.Empty<Expression>()))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_AttributeSetName").MethodHandle)), Expression.Constant("sys_tag", typeof(string)))), new ParameterExpression[] { parameterExpression }), _context, _transaction);
				if (repositoryResponse.get_IsSucceed())
				{
					this.SysTags = repositoryResponse.get_Data();
				}
			}
		}

		public override MixPage ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			string str;
			this.GenerateSEO();
			this.Template = (this.View != null ? string.Concat(this.View.FolderType, "/", this.View.FileName, this.View.Extension) : this.Template);
			if (this.Master != null)
			{
				str = string.Concat(this.Master.FolderType, "/", this.Master.FileName, this.Master.Extension);
			}
			else
			{
				str = null;
			}
			this.Layout = str;
			if (this.Id == 0)
			{
				this.Id = ViewModelBase<MixCmsContext, MixPage, Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel>.Repository.Max((MixPage c) => c.Id, _context, _transaction).get_Data() + 1;
				this.CreatedDateTime = DateTime.UtcNow;
			}
			this.LastModified = new DateTime?(DateTime.UtcNow);
			if (!string.IsNullOrEmpty(this.Image) && this.Image[0] == '/')
			{
				this.Image = this.Image.Substring(1);
			}
			if (!string.IsNullOrEmpty(this.Thumbnail) && this.Thumbnail[0] == '/')
			{
				this.Thumbnail = this.Thumbnail.Substring(1);
			}
			return base.ParseModel(_context, _transaction);
		}

		private async Task<RepositoryResponse<bool>> SaveAttributeAsync(int parentId, MixCmsContext context, IDbContextTransaction transaction)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			this.AttributeData.ParentId = parentId.ToString();
			this.AttributeData.ParentType = MixEnums.MixAttributeSetDataType.Page;
			RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel> repositoryResponse2 = await this.AttributeData.Data.SaveModelAsync(true, context, transaction);
			ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel>(repositoryResponse2, ref repositoryResponse1);
			if (repositoryResponse1.get_IsSucceed())
			{
				this.AttributeData.DataId = repositoryResponse2.get_Data().Id;
				ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel>(await this.AttributeData.SaveModelAsync(true, context, transaction), ref repositoryResponse1);
			}
			foreach (Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel sysCategory in this.SysCategories)
			{
				if (!repositoryResponse1.get_IsSucceed())
				{
					continue;
				}
				sysCategory.ParentId = parentId.ToString();
				sysCategory.ParentType = MixEnums.MixAttributeSetDataType.Page;
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
				sysTag.ParentType = MixEnums.MixAttributeSetDataType.Page;
				sysTag.Specificulture = this.Specificulture;
				ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel>(await sysTag.SaveModelAsync(false, context, transaction), ref repositoryResponse1);
			}
			return repositoryResponse1;
		}

		public override RepositoryResponse<bool> SaveSubModels(MixPage parent, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel>(this.View.SaveModel(true, _context, _transaction), ref repositoryResponse1);
			if (repositoryResponse1.get_IsSucceed() && this.Master != null)
			{
				ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel>(this.Master.SaveModel(true, _context, _transaction), ref repositoryResponse1);
			}
			if (repositoryResponse1.get_IsSucceed() && this.UrlAliases != null)
			{
				foreach (Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel urlAlias in this.UrlAliases)
				{
					if (!repositoryResponse1.get_IsSucceed())
					{
						break;
					}
					urlAlias.SourceId = parent.Id.ToString();
					urlAlias.Type = MixEnums.UrlAliasType.Page;
					urlAlias.Specificulture = this.Specificulture;
					ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel>(urlAlias.SaveModel(false, _context, _transaction), ref repositoryResponse1);
				}
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				foreach (Mix.Cms.Lib.ViewModels.MixPageModules.ReadMvcViewModel moduleNav in this.ModuleNavs)
				{
					moduleNav.PageId = parent.Id;
					if (!moduleNav.IsActived)
					{
						ViewModelHelper.HandleResult<MixPageModule>(moduleNav.RemoveModel(false, _context, _transaction), ref repositoryResponse1);
					}
					else
					{
						ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixPageModules.ReadMvcViewModel>(moduleNav.SaveModel(false, _context, _transaction), ref repositoryResponse1);
					}
				}
			}
			return repositoryResponse1;
		}

		public override async Task<RepositoryResponse<bool>> SaveSubModelsAsync(MixPage parent, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel>(await this.View.SaveModelAsync(true, _context, _transaction), ref repositoryResponse1);
			if (repositoryResponse1.get_IsSucceed() && this.Master != null)
			{
				ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel>(this.Master.SaveModel(true, _context, _transaction), ref repositoryResponse1);
			}
			if (repositoryResponse1.get_IsSucceed() && this.UrlAliases != null)
			{
				foreach (Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel urlAlias in this.UrlAliases)
				{
					if (!repositoryResponse1.get_IsSucceed())
					{
						break;
					}
					urlAlias.SourceId = parent.Id.ToString();
					urlAlias.Type = MixEnums.UrlAliasType.Page;
					urlAlias.Specificulture = this.Specificulture;
					ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel>(await urlAlias.SaveModelAsync(false, _context, _transaction), ref repositoryResponse1);
				}
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				foreach (Mix.Cms.Lib.ViewModels.MixPageModules.ReadMvcViewModel moduleNav in this.ModuleNavs)
				{
					moduleNav.PageId = parent.Id;
					if (!moduleNav.IsActived)
					{
						ViewModelHelper.HandleResult<MixPageModule>(await moduleNav.RemoveModelAsync(false, _context, _transaction), ref repositoryResponse1);
					}
					else
					{
						ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixPageModules.ReadMvcViewModel>(await moduleNav.SaveModelAsync(false, _context, _transaction), ref repositoryResponse1);
					}
				}
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				repositoryResponse1 = await this.SaveAttributeAsync(parent.Id, _context, _transaction);
			}
			return repositoryResponse1;
		}
	}
}