using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.Services;
using Mix.Cms.Lib.ViewModels;
using Mix.Cms.Lib.ViewModels.MixCultures;
using Mix.Common.Helper;
using Mix.Domain.Core.Models;
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
using System.Threading;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixLanguages
{
	public class UpdateViewModel : ViewModelBase<MixCmsContext, MixLanguage, Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel>
	{
		[JsonProperty("category")]
		public string Category
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

		[JsonProperty("cultures")]
		public List<SupportedCulture> Cultures
		{
			get;
			set;
		}

		[JsonProperty("dataType")]
		public MixEnums.MixDataType DataType
		{
			get;
			set;
		}

		[JsonProperty("defaultValue")]
		public string DefaultValue
		{
			get;
			set;
		}

		[JsonProperty("description")]
		public string Description
		{
			get;
			set;
		}

		[JsonProperty("domain")]
		public string Domain
		{
			get
			{
				return MixService.GetConfig<string>("Domain", this.Specificulture);
			}
		}

		[JsonProperty("id")]
		public int Id
		{
			get;
			set;
		}

		[JsonProperty("keyword")]
		[Required]
		public string Keyword
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

		[JsonProperty("property")]
		public DataValueViewModel Property
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

		[JsonProperty("value")]
		public string Value
		{
			get;
			set;
		}

		public UpdateViewModel()
		{
		}

		public UpdateViewModel(MixLanguage model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.Cultures = this.LoadCultures(this.Specificulture, _context, _transaction);
			this.Property = new DataValueViewModel()
			{
				DataType = this.DataType,
				Value = this.Value,
				Name = this.Keyword
			};
			this.Cultures.ForEach((SupportedCulture c) => c.set_IsSupported(true));
		}

		public override Task<bool> ExpandViewAsync(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.Cultures = this.LoadCultures(this.Specificulture, _context, _transaction);
			this.Property = new DataValueViewModel()
			{
				DataType = this.DataType,
				Value = this.Value,
				Name = this.Keyword
			};
			this.Cultures.ForEach((SupportedCulture c) => c.set_IsSupported(true));
			return base.ExpandViewAsync(_context, _transaction);
		}

		public static async Task<RepositoryResponse<bool>> ImportLanguages(List<MixLanguage> arrLanguage, string destCulture, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel.u003cImportLanguagesu003ed__76 variable = new Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel.u003cImportLanguagesu003ed__76();
			variable.arrLanguage = arrLanguage;
			variable.destCulture = destCulture;
			variable._context = _context;
			variable._transaction = _transaction;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel.u003cImportLanguagesu003ed__76>(ref variable);
			return variable.u003cu003et__builder.Task;
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
					supportedCulture.set_IsSupported((datum.Specificulture == initCulture ? true : _context.MixLanguage.Any<MixLanguage>((MixLanguage p) => p.Keyword == this.Keyword && p.Specificulture == datum.Specificulture)));
					supportedCultures1.Add(supportedCulture);
				}
			}
			return supportedCultures;
		}

		public override MixLanguage ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (this.Id == 0)
			{
				this.Id = ViewModelBase<MixCmsContext, MixLanguage, Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel>.Repository.Max((MixLanguage s) => s.Id, _context, _transaction).get_Data() + 1;
				this.CreatedDateTime = DateTime.UtcNow;
			}
			this.Value = this.Property.Value ?? this.Value;
			if (this.CreatedDateTime == new DateTime())
			{
				this.CreatedDateTime = DateTime.UtcNow;
			}
			if (string.IsNullOrEmpty(this.DefaultValue))
			{
				this.DefaultValue = this.Value;
			}
			return base.ParseModel(_context, _transaction);
		}

		public override Task<Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel> ParseViewAsync(bool isExpand = true, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.Property = new DataValueViewModel()
			{
				DataType = this.DataType,
				Value = this.Value,
				Name = this.Keyword
			};
			return base.ParseViewAsync(isExpand, _context, _transaction);
		}

		public override async Task<RepositoryResponse<MixLanguage>> RemoveModelAsync(bool isRemoveRelatedModels = false, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<MixLanguage> repositoryResponse = await this.u003cu003en__1(isRemoveRelatedModels, _context, _transaction);
			if (repositoryResponse.get_IsSucceed() && repositoryResponse.get_IsSucceed())
			{
				MixService.LoadFromDatabase(null, null);
				MixService.SaveSettings();
			}
			return repositoryResponse;
		}

		public override RepositoryResponse<bool> RemoveRelatedModels(Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel view, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel.u003cu003ec__DisplayClass75_0 variable = null;
			foreach (SupportedCulture supportedCulture in 
				from c in this.Cultures
				where c.get_Specificulture() != this.Specificulture
				select c)
			{
				DbSet<MixLanguage> mixLanguage = _context.MixLanguage;
				ParameterExpression parameterExpression = Expression.Parameter(typeof(MixLanguage), "c");
				MixLanguage mixLanguage1 = mixLanguage.First<MixLanguage>(Expression.Lambda<Func<MixLanguage, bool>>(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixLanguage).GetMethod("get_Keyword").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel).GetMethod("get_Keyword").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixLanguage).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel.u003cu003ec__DisplayClass75_0)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel.u003cu003ec__DisplayClass75_0).GetField("culture").FieldHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(SupportedCulture).GetMethod("get_Specificulture").MethodHandle)))), new ParameterExpression[] { parameterExpression }));
				if (mixLanguage1 == null)
				{
					continue;
				}
				_context.MixLanguage.Remove(mixLanguage1);
			}
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(_context.SaveChanges() > 0);
			return repositoryResponse;
		}

		public override async Task<RepositoryResponse<bool>> RemoveRelatedModelsAsync(Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel view, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel.u003cu003ec__DisplayClass71_0 variable = null;
			foreach (SupportedCulture supportedCulture in 
				from c in this.Cultures
				where c.get_Specificulture() != this.Specificulture
				select c)
			{
				DbSet<MixLanguage> mixLanguage = _context.MixLanguage;
				ParameterExpression parameterExpression = Expression.Parameter(typeof(MixLanguage), "c");
				BinaryExpression binaryExpression = Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixLanguage).GetMethod("get_Keyword").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel).GetMethod("get_Keyword").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixLanguage).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel.u003cu003ec__DisplayClass71_0)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel.u003cu003ec__DisplayClass71_0).GetField("culture").FieldHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(SupportedCulture).GetMethod("get_Specificulture").MethodHandle))));
				ParameterExpression[] parameterExpressionArray = new ParameterExpression[] { parameterExpression };
				MixLanguage mixLanguage1 = mixLanguage.First<MixLanguage>(Expression.Lambda<Func<MixLanguage, bool>>(binaryExpression, parameterExpressionArray));
				if (mixLanguage1 == null)
				{
					continue;
				}
				_context.MixLanguage.Remove(mixLanguage1);
			}
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			MixCmsContext mixCmsContext = _context;
			CancellationToken cancellationToken = new CancellationToken();
			int num = await ((DbContext)mixCmsContext).SaveChangesAsync(cancellationToken);
			repositoryResponse1.set_IsSucceed(num > 0);
			return repositoryResponse;
		}

		public override async Task<RepositoryResponse<Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel>> SaveModelAsync(bool isSaveSubModels = false, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel> repositoryResponse = await this.u003cu003en__0(isSaveSubModels, _context, _transaction);
			if (repositoryResponse.get_IsSucceed())
			{
				MixService.LoadFromDatabase(null, null);
				MixService.SaveSettings();
			}
			return repositoryResponse;
		}
	}
}