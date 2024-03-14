using Microsoft.EntityFrameworkCore;
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
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixConfigurations
{
	public class UpdateViewModel : ViewModelBase<MixCmsContext, MixConfiguration, Mix.Cms.Lib.ViewModels.MixConfigurations.UpdateViewModel>
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

		public UpdateViewModel()
		{
		}

		public UpdateViewModel(MixConfiguration model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.Cultures = this.LoadCultures(this.Specificulture, _context, _transaction);
			this.Cultures.ForEach((SupportedCulture c) => c.set_IsSupported(true));
			this.Property = new DataValueViewModel()
			{
				DataType = this.DataType,
				Value = this.Value,
				Name = this.Keyword
			};
		}

		public static async Task<RepositoryResponse<bool>> ImportConfigurations(List<MixConfiguration> arrConfiguration, string destCulture, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			bool flag;
			MixCmsContext mixCmsContext = null;
			IDbContextTransaction dbContextTransaction = null;
			bool flag1 = false;
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			UnitOfWorkHelper<MixCmsContext>.InitTransaction(_context, _transaction, ref mixCmsContext, ref dbContextTransaction, ref flag1);
			try
			{
				try
				{
					foreach (MixConfiguration mixConfiguration in arrConfiguration)
					{
						Mix.Cms.Lib.ViewModels.MixConfigurations.UpdateViewModel updateViewModel = new Mix.Cms.Lib.ViewModels.MixConfigurations.UpdateViewModel(mixConfiguration, mixCmsContext, dbContextTransaction)
						{
							CreatedDateTime = DateTime.UtcNow,
							Specificulture = destCulture
						};
						RepositoryResponse<Mix.Cms.Lib.ViewModels.MixConfigurations.UpdateViewModel> repositoryResponse2 = await ((ViewModelBase<MixCmsContext, MixConfiguration, Mix.Cms.Lib.ViewModels.MixConfigurations.UpdateViewModel>)updateViewModel).SaveModelAsync(false, mixCmsContext, dbContextTransaction);
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
					repositoryResponse1.set_Data(true);
					UnitOfWorkHelper<MixCmsContext>.HandleTransaction(repositoryResponse1.get_IsSucceed(), flag1, dbContextTransaction);
				}
				catch (Exception exception)
				{
					RepositoryResponse<ReadMvcViewModel> repositoryResponse4 = UnitOfWorkHelper<MixCmsContext>.HandleException<ReadMvcViewModel>(exception, flag1, dbContextTransaction);
					repositoryResponse1.set_IsSucceed(false);
					repositoryResponse1.set_Errors(repositoryResponse4.get_Errors());
					repositoryResponse1.set_Exception(repositoryResponse4.get_Exception());
				}
			}
			finally
			{
				if (flag1)
				{
					mixCmsContext.Dispose();
				}
			}
			return repositoryResponse1;
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
					supportedCulture.set_IsSupported((datum.Specificulture == initCulture ? true : _context.MixConfiguration.Any<MixConfiguration>((MixConfiguration p) => p.Keyword == this.Keyword && p.Specificulture == datum.Specificulture)));
					supportedCultures1.Add(supportedCulture);
				}
			}
			return supportedCultures;
		}

		public override MixConfiguration ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (this.Id == 0)
			{
				this.Id = ViewModelBase<MixCmsContext, MixConfiguration, Mix.Cms.Lib.ViewModels.MixConfigurations.UpdateViewModel>.Repository.Max((MixConfiguration s) => s.Id, _context, _transaction).get_Data() + 1;
				this.CreatedDateTime = DateTime.UtcNow;
			}
			this.Value = this.Property.Value;
			this.DataType = this.Property.DataType;
			if (this.CreatedDateTime == new DateTime())
			{
				this.CreatedDateTime = DateTime.UtcNow;
			}
			return base.ParseModel(_context, _transaction);
		}
	}
}