using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.Repositories;
using Mix.Cms.Lib.Services;
using Mix.Cms.Lib.ViewModels;
using Mix.Common.Helper;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixTemplates
{
	public class ImportViewModel : ViewModelBase<MixCmsContext, MixTemplate, ImportViewModel>
	{
		[JsonProperty("assetFolder")]
		public string AssetFolder
		{
			get
			{
				return CommonHelper.GetFullPath(new string[] { "content", "templates", SeoHelper.GetSEOString(this.ThemeName, '-') });
			}
		}

		[JsonProperty("content")]
		[Required]
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

		[JsonProperty("extension")]
		public string Extension { get; set; } = ".cshtml";

		[JsonProperty("fileFolder")]
		public string FileFolder
		{
			get;
			set;
		}

		[JsonProperty("fileName")]
		public string FileName
		{
			get;
			set;
		}

		[JsonProperty("folderType")]
		public string FolderType
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

		[JsonProperty("mobileContent")]
		public string MobileContent { get; set; } = "{}";

		[JsonProperty("modifiedBy")]
		public string ModifiedBy
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

		[JsonProperty("scripts")]
		public string Scripts
		{
			get;
			set;
		}

		[JsonProperty("spaContent")]
		public string SpaContent { get; set; } = "";

		[JsonProperty("status")]
		public MixEnums.MixContentStatus Status
		{
			get;
			set;
		}

		[JsonProperty("styles")]
		public string Styles
		{
			get;
			set;
		}

		[JsonProperty("templateFolder")]
		public string TemplateFolder
		{
			get
			{
				return CommonHelper.GetFullPath(new string[] { "Views/Shared/Templates", SeoHelper.GetSEOString(this.ThemeName, '-') });
			}
		}

		[JsonProperty("templatePath")]
		public string TemplatePath
		{
			get
			{
				return string.Concat(new string[] { "/", this.FileFolder, "/", this.FileName, this.Extension });
			}
		}

		[JsonProperty("themeId")]
		public int ThemeId
		{
			get;
			set;
		}

		[JsonProperty("themeName")]
		public string ThemeName
		{
			get;
			set;
		}

		public ImportViewModel()
		{
		}

		public ImportViewModel(MixTemplate model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public async Task<RepositoryResponse<ImportViewModel>> CopyAsync()
		{
			DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixTemplate, ImportViewModel>.Repository;
			RepositoryResponse<ImportViewModel> singleModelAsync = await repository.GetSingleModelAsync((MixTemplate m) => m.Id == this.Id, null, null);
			singleModelAsync.get_Data().Id = 0;
			singleModelAsync.get_Data().FileName = string.Concat("Copy_", singleModelAsync.get_Data().FileName);
			RepositoryResponse<ImportViewModel> repositoryResponse = await singleModelAsync.get_Data().SaveModelAsync(false, null, null);
			return repositoryResponse;
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			string content;
			if (!string.IsNullOrEmpty(this.FileName))
			{
				FileViewModel file = FileRepository.Instance.GetFile(this.FileName, this.Extension, this.FileFolder, false, "");
				if (file != null)
				{
					content = file.Content;
				}
				else
				{
					content = null;
				}
				if (!string.IsNullOrWhiteSpace(content))
				{
					this.Content = file.Content;
				}
			}
		}

		public static ImportViewModel GetDefault(MixEnums.EnumTemplateFolder folderType, string specificulture)
		{
			// 
			// Current member / type: Mix.Cms.Lib.ViewModels.MixTemplates.ImportViewModel Mix.Cms.Lib.ViewModels.MixTemplates.ImportViewModel::GetDefault(Mix.Cms.Lib.MixEnums/EnumTemplateFolder,System.String)
			// Exception in: Mix.Cms.Lib.ViewModels.MixTemplates.ImportViewModel GetDefault(Mix.Cms.Lib.MixEnums/EnumTemplateFolder,System.String)
			// Object reference not set to an instance of an object.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		public static RepositoryResponse<ImportViewModel> GetTemplateByPath(string path, string culture, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			ImportViewModel.u003cu003ec__DisplayClass92_0 variable = null;
			string[] strArrays;
			RepositoryResponse<ImportViewModel> repositoryResponse = new RepositoryResponse<ImportViewModel>();
			if (path != null)
			{
				strArrays = path.Split('/', StringSplitOptions.None);
			}
			else
			{
				strArrays = null;
			}
			string[] strArrays1 = strArrays;
			if (strArrays1 == null || (int)strArrays1.Length < 2)
			{
				repositoryResponse.set_IsSucceed(false);
				repositoryResponse.get_Errors().Add("Template Not Found");
			}
			else
			{
				MixService.GetConfig<int>("ThemeId", culture);
				string str = strArrays1[1].Split('.', StringSplitOptions.None)[0];
				DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixTemplate, ImportViewModel>.Repository;
				ParameterExpression parameterExpression = Expression.Parameter(typeof(MixTemplate), "t");
				repositoryResponse = repository.GetSingleModel(Expression.Lambda<Func<MixTemplate, bool>>(Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixTemplate).GetMethod("get_FolderType").MethodHandle)), Expression.ArrayIndex(Expression.Field(Expression.Constant(variable, typeof(ImportViewModel.u003cu003ec__DisplayClass92_0)), FieldInfo.GetFieldFromHandle(typeof(ImportViewModel.u003cu003ec__DisplayClass92_0).GetField("temp").FieldHandle)), Expression.Constant(0, typeof(int)))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixTemplate).GetMethod("get_FileName").MethodHandle)), Expression.Field(Expression.Constant(variable, typeof(ImportViewModel.u003cu003ec__DisplayClass92_0)), FieldInfo.GetFieldFromHandle(typeof(ImportViewModel.u003cu003ec__DisplayClass92_0).GetField("name").FieldHandle)))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixTemplate).GetMethod("get_ThemeId").MethodHandle)), Expression.Field(Expression.Constant(variable, typeof(ImportViewModel.u003cu003ec__DisplayClass92_0)), FieldInfo.GetFieldFromHandle(typeof(ImportViewModel.u003cu003ec__DisplayClass92_0).GetField("activeThemeId").FieldHandle)))), new ParameterExpression[] { parameterExpression }), _context, _transaction);
			}
			return repositoryResponse;
		}

		public static ImportViewModel GetTemplateByPath(string path, string specificulture, MixEnums.EnumTemplateFolder folderType, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			ImportViewModel.u003cu003ec__DisplayClass93_0 variable = null;
			string str;
			if (path != null)
			{
				str = path.Split('/', StringSplitOptions.None)[1];
			}
			else
			{
				str = null;
			}
			MixService.GetConfig<int>("ThemeId", specificulture);
			MixService.GetConfig<string>("ThemeName", specificulture);
			DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixTemplate, ImportViewModel>.Repository;
			ParameterExpression parameterExpression = Expression.Parameter(typeof(MixTemplate), "t");
			return repository.GetSingleModel(Expression.Lambda<Func<MixTemplate, bool>>(Expression.AndAlso(Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixTemplate).GetMethod("get_ThemeId").MethodHandle)), Expression.Field(Expression.Constant(variable, typeof(ImportViewModel.u003cu003ec__DisplayClass93_0)), FieldInfo.GetFieldFromHandle(typeof(ImportViewModel.u003cu003ec__DisplayClass93_0).GetField("themeId").FieldHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixTemplate).GetMethod("get_FolderType").MethodHandle)), Expression.Call(Expression.Field(Expression.Constant(variable, typeof(ImportViewModel.u003cu003ec__DisplayClass93_0)), FieldInfo.GetFieldFromHandle(typeof(ImportViewModel.u003cu003ec__DisplayClass93_0).GetField("folderType").FieldHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(object).GetMethod("ToString").MethodHandle), Array.Empty<Expression>()))), Expression.Not(Expression.Call(null, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(string).GetMethod("IsNullOrEmpty", new Type[] { typeof(string) }).MethodHandle), new Expression[] { Expression.Field(Expression.Constant(variable, typeof(ImportViewModel.u003cu003ec__DisplayClass93_0)), FieldInfo.GetFieldFromHandle(typeof(ImportViewModel.u003cu003ec__DisplayClass93_0).GetField("templateName").FieldHandle)) }))), Expression.Call(Expression.Field(Expression.Constant(variable, typeof(ImportViewModel.u003cu003ec__DisplayClass93_0)), FieldInfo.GetFieldFromHandle(typeof(ImportViewModel.u003cu003ec__DisplayClass93_0).GetField("templateName").FieldHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(string).GetMethod("Equals", new Type[] { typeof(string) }).MethodHandle), new Expression[] { Expression.Call(null, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(string).GetMethod("Format", new Type[] { typeof(string), typeof(object), typeof(object) }).MethodHandle), new Expression[] { Expression.Constant("{0}{1}", typeof(string)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixTemplate).GetMethod("get_FileName").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixTemplate).GetMethod("get_Extension").MethodHandle)) }) })), new ParameterExpression[] { parameterExpression }), _context, _transaction).get_Data() ?? ImportViewModel.GetDefault(folderType, specificulture);
		}

		public override MixTemplate ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			string str;
			string str1;
			string str2;
			if (this.Id == 0)
			{
				this.Id = ViewModelBase<MixCmsContext, MixTemplate, ImportViewModel>.Repository.Max((MixTemplate m) => m.Id, _context, _transaction).get_Data() + 1;
				this.CreatedDateTime = DateTime.UtcNow;
			}
			this.FileFolder = CommonHelper.GetFullPath(new string[] { "Views/Shared/Templates", this.ThemeName, this.FolderType });
			string content = this.Content;
			if (content != null)
			{
				str = content.Trim();
			}
			else
			{
				str = null;
			}
			this.Content = str;
			string scripts = this.Scripts;
			if (scripts != null)
			{
				str1 = scripts.Trim();
			}
			else
			{
				str1 = null;
			}
			this.Scripts = str1;
			string styles = this.Styles;
			if (styles != null)
			{
				str2 = styles.Trim();
			}
			else
			{
				str2 = null;
			}
			this.Styles = str2;
			return base.ParseModel(_context, _transaction);
		}

		public override RepositoryResponse<MixTemplate> RemoveModel(bool isRemoveRelatedModels = false, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<MixTemplate> repositoryResponse = base.RemoveModel(isRemoveRelatedModels, _context, _transaction);
			if (repositoryResponse.get_IsSucceed())
			{
				TemplateRepository.Instance.DeleteTemplate(this.FileName, this.FileFolder);
			}
			return repositoryResponse;
		}

		public override async Task<RepositoryResponse<MixTemplate>> RemoveModelAsync(bool isRemoveRelatedModels = false, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<MixTemplate> repositoryResponse = await this.u003cu003en__1(isRemoveRelatedModels, _context, _transaction);
			if (repositoryResponse.get_IsSucceed())
			{
				TemplateRepository.Instance.DeleteTemplate(this.FileName, this.FileFolder);
			}
			return repositoryResponse;
		}

		public override async Task<RepositoryResponse<ImportViewModel>> SaveModelAsync(bool isSaveSubModels = false, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<ImportViewModel> repositoryResponse = await this.u003cu003en__0(isSaveSubModels, _context, _transaction);
			repositoryResponse.get_IsSucceed();
			return repositoryResponse;
		}

		public override RepositoryResponse<bool> SaveSubModels(MixTemplate parent, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			TemplateRepository.Instance.SaveTemplate(new TemplateViewModel()
			{
				Filename = this.FileName,
				Extension = this.Extension,
				Content = this.Content,
				FileFolder = this.FileFolder
			});
			return base.SaveSubModels(parent, _context, _transaction);
		}

		public override Task<RepositoryResponse<bool>> SaveSubModelsAsync(MixTemplate parent, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			TemplateRepository.Instance.SaveTemplate(new TemplateViewModel()
			{
				Filename = this.FileName,
				Extension = this.Extension,
				Content = this.Content,
				FileFolder = this.FileFolder
			});
			return base.SaveSubModelsAsync(parent, _context, _transaction);
		}

		public override void Validate(MixCmsContext _context, IDbContextTransaction _transaction)
		{
			string name;
			base.Validate(_context, _transaction);
			if (base.get_IsValid())
			{
				if (this.Id == 0)
				{
					if (_context.MixTemplate.Any<MixTemplate>((MixTemplate t) => t.FileName == this.FileName && t.FolderType == this.FolderType && t.ThemeId == this.ThemeId))
					{
						base.set_IsValid(false);
						base.get_Errors().Add(string.Concat(this.FileName, " is existed"));
					}
				}
				if (string.IsNullOrEmpty(this.ThemeName) && this.ThemeId > 0)
				{
					MixTheme mixTheme = _context.MixTheme.FirstOrDefault<MixTheme>((MixTheme m) => m.Id == this.ThemeId);
					if (mixTheme != null)
					{
						name = mixTheme.Name;
					}
					else
					{
						name = null;
					}
					this.ThemeName = name;
				}
			}
		}
	}
}