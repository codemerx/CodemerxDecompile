using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Common.Helper;
using Mix.Domain.Core.Models;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixLanguages
{
	public class SystemLanguageViewModel : ViewModelBase<MixCmsContext, MixLanguage, SystemLanguageViewModel>
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

		public SystemLanguageViewModel()
		{
		}

		public SystemLanguageViewModel(MixLanguage model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public static async Task<RepositoryResponse<bool>> ImportLanguages(List<MixLanguage> arrLanguage, string destCulture)
		{
			bool flag;
			MixCmsContext mixCmsContext = null;
			IDbContextTransaction dbContextTransaction = null;
			bool flag1 = false;
			UnitOfWorkHelper<MixCmsContext>.InitTransaction(null, null, ref mixCmsContext, ref dbContextTransaction, ref flag1);
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			try
			{
				try
				{
					foreach (MixLanguage mixLanguage in arrLanguage)
					{
						SystemLanguageViewModel systemLanguageViewModel = new SystemLanguageViewModel(mixLanguage, mixCmsContext, dbContextTransaction)
						{
							Specificulture = destCulture
						};
						RepositoryResponse<SystemLanguageViewModel> repositoryResponse2 = await ((ViewModelBase<MixCmsContext, MixLanguage, SystemLanguageViewModel>)systemLanguageViewModel).SaveModelAsync(false, mixCmsContext, dbContextTransaction);
						RepositoryResponse<bool> repositoryResponse3 = repositoryResponse1;
						flag = (!repositoryResponse1.get_IsSucceed() ? false : repositoryResponse2.get_IsSucceed());
						repositoryResponse3.set_IsSucceed(flag);
						if (repositoryResponse1.get_IsSucceed())
						{
							continue;
						}
						repositoryResponse1.set_Exception(repositoryResponse2.get_Exception());
						repositoryResponse1.set_Errors(repositoryResponse2.get_Errors());
						break;
					}
					UnitOfWorkHelper<MixCmsContext>.HandleTransaction(repositoryResponse1.get_IsSucceed(), true, dbContextTransaction);
				}
				catch (Exception exception)
				{
					RepositoryResponse<SystemLanguageViewModel> repositoryResponse4 = UnitOfWorkHelper<MixCmsContext>.HandleException<SystemLanguageViewModel>(exception, true, dbContextTransaction);
					repositoryResponse1.set_IsSucceed(false);
					repositoryResponse1.set_Errors(repositoryResponse4.get_Errors());
					repositoryResponse1.set_Exception(repositoryResponse4.get_Exception());
				}
			}
			finally
			{
				if (flag1)
				{
					RelationalDatabaseFacadeExtensions.CloseConnection(mixCmsContext.get_Database());
					dbContextTransaction.Dispose();
					mixCmsContext.Dispose();
				}
			}
			return repositoryResponse1;
		}
	}
}