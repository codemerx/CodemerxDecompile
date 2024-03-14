using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixConfigurations;
using Mix.Cms.Lib.ViewModels.MixLanguages;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixCultures
{
	public class SystemCultureViewModel : ViewModelBase<MixCmsContext, MixCulture, SystemCultureViewModel>
	{
		[JsonProperty("alias")]
		public string Alias
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

		[JsonProperty("description")]
		public string Description
		{
			get;
			set;
		}

		[JsonProperty("fullName")]
		public string FullName
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

		[JsonProperty("lastModified")]
		public DateTime? LastModified
		{
			get;
			set;
		}

		[JsonProperty("lcid")]
		public string Lcid
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

		public SystemCultureViewModel()
		{
		}

		public SystemCultureViewModel(MixCulture model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override async Task<RepositoryResponse<bool>> RemoveRelatedModelsAsync(SystemCultureViewModel view, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			bool flag;
			bool flag1;
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixConfiguration, SystemConfigurationViewModel>.Repository;
			RepositoryResponse<List<SystemConfigurationViewModel>> modelListByAsync = await repository.GetModelListByAsync((MixConfiguration c) => c.Specificulture == view.Specificulture, _context, _transaction);
			if (modelListByAsync.get_IsSucceed())
			{
				foreach (SystemConfigurationViewModel datum in modelListByAsync.get_Data())
				{
					RepositoryResponse<MixConfiguration> repositoryResponse2 = await datum.RemoveModelAsync(false, _context, _transaction);
					RepositoryResponse<bool> repositoryResponse3 = repositoryResponse1;
					flag1 = (!repositoryResponse1.get_IsSucceed() ? false : repositoryResponse2.get_IsSucceed());
					repositoryResponse3.set_IsSucceed(flag1);
					if (repositoryResponse1.get_IsSucceed())
					{
						continue;
					}
					repositoryResponse1.get_Errors().AddRange(repositoryResponse2.get_Errors());
					repositoryResponse1.set_Exception(repositoryResponse2.get_Exception());
					break;
				}
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				DefaultRepository<!0, !1, !2> defaultRepository = ViewModelBase<MixCmsContext, MixLanguage, SystemLanguageViewModel>.Repository;
				RepositoryResponse<List<SystemLanguageViewModel>> modelListByAsync1 = await defaultRepository.GetModelListByAsync((MixLanguage c) => c.Specificulture == view.Specificulture, _context, _transaction);
				if (modelListByAsync1.get_IsSucceed())
				{
					foreach (SystemLanguageViewModel systemLanguageViewModel in modelListByAsync1.get_Data())
					{
						RepositoryResponse<MixLanguage> repositoryResponse4 = await systemLanguageViewModel.RemoveModelAsync(false, _context, _transaction);
						RepositoryResponse<bool> repositoryResponse5 = repositoryResponse1;
						flag = (!repositoryResponse1.get_IsSucceed() ? false : repositoryResponse4.get_IsSucceed());
						repositoryResponse5.set_IsSucceed(flag);
						if (repositoryResponse1.get_IsSucceed())
						{
							continue;
						}
						repositoryResponse1.get_Errors().AddRange(repositoryResponse4.get_Errors());
						repositoryResponse1.set_Exception(repositoryResponse4.get_Exception());
						break;
					}
				}
			}
			return repositoryResponse1;
		}
	}
}