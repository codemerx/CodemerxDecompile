using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.Services;
using Mix.Cms.Lib.ViewModels.MixModules;
using Mix.Cms.Lib.ViewModels.MixPageModules;
using Mix.Cms.Lib.ViewModels.MixUrlAliases;
using Mix.Domain.Core.Models;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixPages
{
	public class ImportViewModel : ViewModelBase<MixCmsContext, MixPage, Mix.Cms.Lib.ViewModels.MixPages.ImportViewModel>
	{
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

		[JsonProperty("isExportData")]
		public bool IsExportData
		{
			get;
			set;
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

		[JsonProperty("moduleNavs")]
		public List<Mix.Cms.Lib.ViewModels.MixPageModules.ImportViewModel> ModuleNavs
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

		[JsonProperty("themeName")]
		public string ThemeName { get; set; } = "default";

		[JsonProperty("thumbnail")]
		public string Thumbnail
		{
			get;
			set;
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

		[JsonProperty("views")]
		public int? Views
		{
			get;
			set;
		}

		public ImportViewModel()
		{
		}

		public ImportViewModel(MixPage model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
		}

		public List<Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel> GetAliases(MixCmsContext context, IDbContextTransaction transaction)
		{
			DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixUrlAlias, Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel>.Repository;
			ParameterExpression parameterExpression = Expression.Parameter(typeof(MixUrlAlias), "p");
			RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel>> modelListBy = repository.GetModelListBy(Expression.Lambda<Func<MixUrlAlias, bool>>(Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixUrlAlias).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPages.ImportViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPages.ImportViewModel).GetMethod("get_Specificulture").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixUrlAlias).GetMethod("get_SourceId").MethodHandle)), Expression.Call(Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixPages.ImportViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixPages.ImportViewModel).GetMethod("get_Id").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(int).GetMethod("ToString").MethodHandle), Array.Empty<Expression>()))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixUrlAlias).GetMethod("get_Type").MethodHandle)), Expression.Constant(0, typeof(int)))), new ParameterExpression[] { parameterExpression }), context, transaction);
			if (!modelListBy.get_IsSucceed())
			{
				return new List<Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel>();
			}
			return modelListBy.get_Data();
		}

		public List<Mix.Cms.Lib.ViewModels.MixPageModules.ImportViewModel> GetModuleNavs(MixCmsContext context, IDbContextTransaction transaction)
		{
			return ViewModelBase<MixCmsContext, MixPageModule, Mix.Cms.Lib.ViewModels.MixPageModules.ImportViewModel>.Repository.GetModelListBy((MixPageModule module) => module.Specificulture == this.Specificulture && module.PageId == this.Id, context, transaction).get_Data();
		}

		public override async Task<RepositoryResponse<bool>> SaveSubModelsAsync(MixPage parent, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			if (repositoryResponse1.get_IsSucceed() && this.ModuleNavs != null)
			{
				DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixPageModule, Mix.Cms.Lib.ViewModels.MixPageModules.ImportViewModel>.Repository;
				int data = repository.Max((MixPageModule m) => m.Id, null, null).get_Data();
				foreach (Mix.Cms.Lib.ViewModels.MixPageModules.ImportViewModel moduleNav in this.ModuleNavs)
				{
					if (!ViewModelBase<MixCmsContext, MixModule, Mix.Cms.Lib.ViewModels.MixModules.ImportViewModel>.Repository.CheckIsExists((MixModule m) => {
						if (m.Name != moduleNav.Module.Name)
						{
							return false;
						}
						return m.Specificulture == parent.Specificulture;
					}, _context, _transaction))
					{
						moduleNav.Module.Specificulture = parent.Specificulture;
						if (!string.IsNullOrEmpty(moduleNav.Image))
						{
							moduleNav.Image = moduleNav.Image.Replace(string.Concat("content/templates/", this.ThemeName), string.Concat("content/templates/", MixService.GetConfig<string>("ThemeFolder", parent.Specificulture)));
						}
						if (!string.IsNullOrEmpty(moduleNav.Module.Image))
						{
							moduleNav.Module.Image = moduleNav.Module.Image.Replace(string.Concat("content/templates/", this.ThemeName), string.Concat("content/templates/", MixService.GetConfig<string>("ThemeFolder", parent.Specificulture)));
						}
						if (!string.IsNullOrEmpty(moduleNav.Module.Thumbnail))
						{
							moduleNav.Module.Thumbnail = moduleNav.Module.Thumbnail.Replace("content/templates/default", string.Concat("content/templates/", MixService.GetConfig<string>("ThemeFolder", parent.Specificulture)));
						}
						RepositoryResponse<Mix.Cms.Lib.ViewModels.MixModules.ImportViewModel> repositoryResponse2 = await moduleNav.Module.SaveModelAsync(true, _context, _transaction);
						ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixModules.ImportViewModel>(repositoryResponse2, ref repositoryResponse1);
						if (!repositoryResponse1.get_IsSucceed())
						{
							break;
						}
						data++;
						moduleNav.Id = data;
						moduleNav.PageId = parent.Id;
						moduleNav.ModuleId = repositoryResponse2.get_Data().Id;
						moduleNav.Specificulture = parent.Specificulture;
						moduleNav.Description = repositoryResponse2.get_Data().Title;
						moduleNav.CreatedDateTime = DateTime.UtcNow;
						moduleNav.CreatedBy = this.CreatedBy;
						ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixPageModules.ImportViewModel>(await moduleNav.SaveModelAsync(false, _context, _transaction), ref repositoryResponse1);
						if (!repositoryResponse1.get_IsSucceed())
						{
							break;
						}
					}
				}
			}
			return repositoryResponse1;
		}
	}
}