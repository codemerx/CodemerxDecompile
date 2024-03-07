using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.Services;
using Mix.Cms.Lib.ViewModels;
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

namespace Mix.Cms.Lib.ViewModels.MixConfigurations
{
	public class SystemConfigurationViewModel : ViewModelBase<MixCmsContext, MixConfiguration, SystemConfigurationViewModel>
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

		public SystemConfigurationViewModel()
		{
		}

		public SystemConfigurationViewModel(MixConfiguration model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.Property = new DataValueViewModel()
			{
				DataType = this.DataType,
				Value = this.Value,
				Name = this.Keyword
			};
		}

		public static async Task<RepositoryResponse<bool>> ImportConfigurations(List<MixConfiguration> arrConfiguration, string destCulture)
		{
			bool flag;
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			MixCmsContext mixCmsContext = new MixCmsContext();
			IDbContextTransaction dbContextTransaction = mixCmsContext.get_Database().BeginTransaction();
			try
			{
				try
				{
					foreach (MixConfiguration mixConfiguration in arrConfiguration)
					{
						SystemConfigurationViewModel systemConfigurationViewModel = new SystemConfigurationViewModel(mixConfiguration, mixCmsContext, dbContextTransaction)
						{
							Specificulture = destCulture
						};
						RepositoryResponse<SystemConfigurationViewModel> repositoryResponse2 = await ((ViewModelBase<MixCmsContext, MixConfiguration, SystemConfigurationViewModel>)systemConfigurationViewModel).SaveModelAsync(false, mixCmsContext, dbContextTransaction);
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
					RepositoryResponse<SystemConfigurationViewModel> repositoryResponse4 = UnitOfWorkHelper<MixCmsContext>.HandleException<SystemConfigurationViewModel>(exception, true, dbContextTransaction);
					repositoryResponse1.set_IsSucceed(false);
					repositoryResponse1.set_Errors(repositoryResponse4.get_Errors());
					repositoryResponse1.set_Exception(repositoryResponse4.get_Exception());
				}
			}
			finally
			{
				RelationalDatabaseFacadeExtensions.CloseConnection(mixCmsContext.get_Database());
				dbContextTransaction.Dispose();
				mixCmsContext.Dispose();
			}
			return repositoryResponse1;
		}

		public override MixConfiguration ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (this.CreatedDateTime == new DateTime())
			{
				this.CreatedDateTime = DateTime.UtcNow;
			}
			return base.ParseModel(_context, _transaction);
		}
	}
}