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
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixTemplates
{
	public class ReadViewModel : ViewModelBase<MixCmsContext, MixTemplate, ReadViewModel>
	{
		[JsonProperty("assetFolder")]
		public string AssetFolder
		{
			get
			{
				return CommonHelper.GetFullPath(new string[] { "content", "templates", this.ThemeName });
			}
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

		[JsonProperty("extension")]
		public string Extension
		{
			get;
			set;
		}

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
				return CommonHelper.GetFullPath(new string[] { "Views/Shared/Templates", this.ThemeName });
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

		public ReadViewModel()
		{
		}

		public ReadViewModel(MixTemplate model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			string content;
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

		public static ReadViewModel GetDefault(string activedTemplate, string folderType, string folder, string specificulture)
		{
			return new ReadViewModel(new MixTemplate()
			{
				Extension = MixService.GetConfig<string>("TemplateExtension"),
				ThemeId = MixService.GetConfig<int>("ThemeId", specificulture),
				ThemeName = activedTemplate,
				FolderType = folderType,
				FileFolder = folder,
				FileName = MixService.GetConfig<string>("DefaultTemplate"),
				Content = "<div></div>"
			}, null, null);
		}

		public static RepositoryResponse<ReadViewModel> GetTemplateByPath(string path, string culture, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			ReadViewModel.u003cu003ec__DisplayClass86_0 variable = null;
			RepositoryResponse<ReadViewModel> repositoryResponse = new RepositoryResponse<ReadViewModel>();
			string[] strArrays = path.Split('/', StringSplitOptions.None);
			if ((int)strArrays.Length >= 2)
			{
				MixService.GetConfig<int>("ThemeId", culture);
				string str = strArrays[1].Split('.', StringSplitOptions.None)[0];
				DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixTemplate, ReadViewModel>.Repository;
				ParameterExpression parameterExpression = Expression.Parameter(typeof(MixTemplate), "t");
				repositoryResponse = repository.GetSingleModel(Expression.Lambda<Func<MixTemplate, bool>>(Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixTemplate).GetMethod("get_FolderType").MethodHandle)), Expression.ArrayIndex(Expression.Field(Expression.Constant(variable, typeof(ReadViewModel.u003cu003ec__DisplayClass86_0)), FieldInfo.GetFieldFromHandle(typeof(ReadViewModel.u003cu003ec__DisplayClass86_0).GetField("temp").FieldHandle)), Expression.Constant(0, typeof(int)))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixTemplate).GetMethod("get_FileName").MethodHandle)), Expression.Field(Expression.Constant(variable, typeof(ReadViewModel.u003cu003ec__DisplayClass86_0)), FieldInfo.GetFieldFromHandle(typeof(ReadViewModel.u003cu003ec__DisplayClass86_0).GetField("name").FieldHandle)))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixTemplate).GetMethod("get_ThemeId").MethodHandle)), Expression.Field(Expression.Constant(variable, typeof(ReadViewModel.u003cu003ec__DisplayClass86_0)), FieldInfo.GetFieldFromHandle(typeof(ReadViewModel.u003cu003ec__DisplayClass86_0).GetField("activeThemeId").FieldHandle)))), new ParameterExpression[] { parameterExpression }), _context, _transaction);
			}
			else
			{
				repositoryResponse.set_IsSucceed(false);
				repositoryResponse.get_Errors().Add("Template Not Found");
			}
			return repositoryResponse;
		}

		public static ReadViewModel GetTemplateByPath(int themeId, string path, string type, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			string str;
			if (path != null)
			{
				str = path.Split('/', StringSplitOptions.None)[1];
			}
			else
			{
				str = null;
			}
			string str1 = str;
			return ViewModelBase<MixCmsContext, MixTemplate, ReadViewModel>.Repository.GetSingleModel((MixTemplate t) => t.ThemeId == themeId && t.FolderType == type && !string.IsNullOrEmpty(str1) && str1.Equals(string.Format("{0}{1}", t.FileName, t.Extension)), _context, _transaction).get_Data();
		}

		public static async Task<RepositoryResponse<ReadViewModel>> GetTemplateByPathAsync(string path, string culture, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			ReadViewModel.u003cu003ec__DisplayClass87_0 variable = null;
			RepositoryResponse<ReadViewModel> repositoryResponse = new RepositoryResponse<ReadViewModel>();
			string[] strArrays = path.Split('/', StringSplitOptions.None);
			if ((int)strArrays.Length >= 2)
			{
				MixService.GetConfig<int>("ThemeId", culture);
				string str = strArrays[1].Split('.', StringSplitOptions.None)[0];
				DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixTemplate, ReadViewModel>.Repository;
				ParameterExpression parameterExpression = Expression.Parameter(typeof(MixTemplate), "t");
				BinaryExpression binaryExpression = Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixTemplate).GetMethod("get_FolderType").MethodHandle)), Expression.ArrayIndex(Expression.Field(Expression.Constant(variable, typeof(ReadViewModel.u003cu003ec__DisplayClass87_0)), FieldInfo.GetFieldFromHandle(typeof(ReadViewModel.u003cu003ec__DisplayClass87_0).GetField("temp").FieldHandle)), Expression.Constant(0, typeof(int)))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixTemplate).GetMethod("get_FileName").MethodHandle)), Expression.Field(Expression.Constant(variable, typeof(ReadViewModel.u003cu003ec__DisplayClass87_0)), FieldInfo.GetFieldFromHandle(typeof(ReadViewModel.u003cu003ec__DisplayClass87_0).GetField("name").FieldHandle)))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixTemplate).GetMethod("get_ThemeId").MethodHandle)), Expression.Field(Expression.Constant(variable, typeof(ReadViewModel.u003cu003ec__DisplayClass87_0)), FieldInfo.GetFieldFromHandle(typeof(ReadViewModel.u003cu003ec__DisplayClass87_0).GetField("activeThemeId").FieldHandle))));
				ParameterExpression[] parameterExpressionArray = new ParameterExpression[] { parameterExpression };
				repositoryResponse = await repository.GetSingleModelAsync(Expression.Lambda<Func<MixTemplate, bool>>(binaryExpression, parameterExpressionArray), _context, _transaction);
			}
			else
			{
				repositoryResponse.set_IsSucceed(false);
				repositoryResponse.get_Errors().Add("Template Not Found");
			}
			return repositoryResponse;
		}

		public override MixTemplate ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			string str;
			string str1;
			string str2;
			if (this.Id == 0)
			{
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
			RepositoryResponse<MixTemplate> repositoryResponse = await this.u003cu003en__0(isRemoveRelatedModels, _context, _transaction);
			if (repositoryResponse.get_IsSucceed())
			{
				TemplateRepository.Instance.DeleteTemplate(this.FileName, this.FileFolder);
			}
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
	}
}