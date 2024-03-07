using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Account;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.Repositories;
using Mix.Cms.Lib.ViewModels;
using Mix.Cms.Lib.ViewModels.MixAttributeSets;
using Mix.Cms.Lib.ViewModels.MixConfigurations;
using Mix.Cms.Lib.ViewModels.MixLanguages;
using Mix.Cms.Lib.ViewModels.MixThemes;
using Mix.Cms.Messenger.Models.Data;
using Mix.Common.Helper;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.Services
{
	public class InitCmsService
	{
		public InitCmsService()
		{
		}

		public static async Task<RepositoryResponse<bool>> InitAttributeSetsAsync(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			MixCmsContext mixCmsContext = null;
			IDbContextTransaction dbContextTransaction = null;
			bool flag = false;
			UnitOfWorkHelper<MixCmsContext>.InitTransaction(_context, _transaction, ref mixCmsContext, ref dbContextTransaction, ref flag);
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			List<Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel> obj = JObject.Parse(FileRepository.Instance.GetFile("attribute_sets.json", "data", true, "{}").Content).get_Item("data").ToObject<List<Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel>>();
			foreach (Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel utcNow in obj)
			{
				if (!repositoryResponse1.get_IsSucceed())
				{
					break;
				}
				utcNow.CreatedDateTime = DateTime.UtcNow;
				ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel>(await utcNow.SaveModelAsync(true, mixCmsContext, dbContextTransaction), ref repositoryResponse1);
			}
			UnitOfWorkHelper<MixCmsContext>.HandleTransaction(repositoryResponse1.get_IsSucceed(), flag, dbContextTransaction);
			return repositoryResponse1;
		}

		public static async Task<RepositoryResponse<bool>> InitCms(string siteName, InitCulture culture)
		{
			RepositoryResponse<bool> repositoryResponse;
			RepositoryResponse<bool> repositoryResponse1 = new RepositoryResponse<bool>();
			MixCmsContext mixCmsContext = null;
			MixCmsAccountContext mixCmsAccountContext = null;
			IDbContextTransaction dbContextTransaction = null;
			IDbContextTransaction dbContextTransaction1 = null;
			bool isSucceed = true;
			try
			{
				try
				{
					if (!string.IsNullOrEmpty(MixService.GetConnectionString("MixCmsConnection")))
					{
						mixCmsContext = new MixCmsContext();
						mixCmsAccountContext = new MixCmsAccountContext();
						MixChatServiceContext mixChatServiceContext = new MixChatServiceContext();
						DatabaseFacade database = mixCmsContext.get_Database();
						CancellationToken cancellationToken = new CancellationToken();
						await RelationalDatabaseFacadeExtensions.MigrateAsync(database, cancellationToken);
						DatabaseFacade databaseFacade = mixCmsAccountContext.get_Database();
						cancellationToken = new CancellationToken();
						await RelationalDatabaseFacadeExtensions.MigrateAsync(databaseFacade, cancellationToken);
						DatabaseFacade database1 = mixChatServiceContext.get_Database();
						cancellationToken = new CancellationToken();
						await RelationalDatabaseFacadeExtensions.MigrateAsync(database1, cancellationToken);
						dbContextTransaction = mixCmsContext.get_Database().BeginTransaction();
						mixCmsContext.MixCulture.Count<MixCulture>();
						if (MixService.GetConfig<bool>("IsInit"))
						{
							isSucceed = InitCmsService.InitCultures(culture, mixCmsContext, dbContextTransaction);
							if (!isSucceed || mixCmsContext.MixConfiguration.Count<MixConfiguration>() != 0)
							{
								repositoryResponse1.get_Errors().Add("Cannot init Configurations");
							}
							else
							{
								isSucceed = await InitCmsService.InitConfigurationsAsync(siteName, culture.Specificulture, mixCmsContext, dbContextTransaction).get_IsSucceed();
							}
						}
						if (!isSucceed)
						{
							dbContextTransaction.Rollback();
						}
						else
						{
							dbContextTransaction.Commit();
						}
					}
					repositoryResponse1.set_IsSucceed(isSucceed);
					repositoryResponse = repositoryResponse1;
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					IDbContextTransaction dbContextTransaction2 = dbContextTransaction;
					if (dbContextTransaction2 != null)
					{
						dbContextTransaction2.Rollback();
					}
					else
					{
					}
					IDbContextTransaction dbContextTransaction3 = dbContextTransaction1;
					if (dbContextTransaction3 != null)
					{
						dbContextTransaction3.Rollback();
					}
					else
					{
					}
					repositoryResponse1.set_IsSucceed(false);
					repositoryResponse1.set_Exception(exception);
					repositoryResponse = repositoryResponse1;
				}
			}
			finally
			{
				MixCmsContext mixCmsContext1 = mixCmsContext;
				if (mixCmsContext1 != null)
				{
					RelationalDatabaseFacadeExtensions.CloseConnection(((DbContext)mixCmsContext1).get_Database());
				}
				else
				{
				}
				MixCmsContext mixCmsContext2 = mixCmsContext;
				if (mixCmsContext2 != null)
				{
					((DbContext)mixCmsContext2).Dispose();
				}
				else
				{
				}
				MixCmsAccountContext mixCmsAccountContext1 = mixCmsAccountContext;
				if (mixCmsAccountContext1 != null)
				{
					RelationalDatabaseFacadeExtensions.CloseConnection(((DbContext)mixCmsAccountContext1).get_Database());
				}
				else
				{
				}
				MixCmsAccountContext mixCmsAccountContext2 = mixCmsAccountContext;
				if (mixCmsAccountContext2 != null)
				{
					((DbContext)mixCmsAccountContext2).Dispose();
				}
				else
				{
				}
			}
			return repositoryResponse;
		}

		public static async Task<RepositoryResponse<bool>> InitConfigurationsAsync(string siteName, string specifiCulture, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			MixCmsContext mixCmsContext = null;
			IDbContextTransaction dbContextTransaction = null;
			bool flag = false;
			UnitOfWorkHelper<MixCmsContext>.InitTransaction(_context, _transaction, ref mixCmsContext, ref dbContextTransaction, ref flag);
			List<MixConfiguration> obj = JObject.Parse(FileRepository.Instance.GetFile("configurations.json", "data", true, "{}").Content).get_Item("data").ToObject<List<MixConfiguration>>();
			List<MixConfiguration> mixConfigurations = obj;
			MixConfiguration mixConfiguration = mixConfigurations.Find((MixConfiguration c) => c.Keyword == "SiteName");
			mixConfiguration.Value = siteName;
			if (!string.IsNullOrEmpty(mixConfiguration.Value))
			{
				List<MixConfiguration> sEOString = obj;
				sEOString.Find((MixConfiguration c) => c.Keyword == "ThemeName").Value = SeoHelper.GetSEOString(mixConfiguration.Value, '-');
				List<MixConfiguration> sEOString1 = obj;
				sEOString1.Find((MixConfiguration c) => c.Keyword == "ThemeFolder").Value = SeoHelper.GetSEOString(mixConfiguration.Value, '-');
			}
			RepositoryResponse<bool> repositoryResponse = await Mix.Cms.Lib.ViewModels.MixConfigurations.UpdateViewModel.ImportConfigurations(obj, specifiCulture, mixCmsContext, dbContextTransaction);
			UnitOfWorkHelper<MixCmsContext>.HandleTransaction(repositoryResponse.get_IsSucceed(), flag, dbContextTransaction);
			return repositoryResponse;
		}

		protected static bool InitCultures(InitCulture culture, MixCmsContext context, IDbContextTransaction transaction)
		{
			bool flag = true;
			try
			{
				if (context.MixCulture.Count<MixCulture>() == 0)
				{
					context.Entry<MixCulture>(new MixCulture()
					{
						Id = 1,
						Specificulture = culture.Specificulture,
						FullName = culture.FullName,
						Description = culture.Description,
						Icon = culture.Icon,
						Alias = culture.Alias,
						Status = MixEnums.MixContentStatus.Published.ToString(),
						CreatedDateTime = DateTime.UtcNow
					}).set_State(4);
					context.SaveChanges();
				}
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		public async Task<RepositoryResponse<bool>> InitLanguagesAsync(string specificulture, List<MixLanguage> languages, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			MixCmsContext mixCmsContext = null;
			IDbContextTransaction dbContextTransaction = null;
			bool flag = false;
			UnitOfWorkHelper<MixCmsContext>.InitTransaction(_context, _transaction, ref mixCmsContext, ref dbContextTransaction, ref flag);
			RepositoryResponse<bool> repositoryResponse = await Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel.ImportLanguages(languages, specificulture, mixCmsContext, dbContextTransaction);
			UnitOfWorkHelper<MixCmsContext>.HandleTransaction(repositoryResponse.get_IsSucceed(), flag, dbContextTransaction);
			return repositoryResponse;
		}

		protected static void InitPages(string culture, MixCmsContext context, IDbContextTransaction transaction)
		{
			foreach (MixPage obj in JObject.Parse(FileRepository.Instance.GetFile("pages.json", "data", true, "{}").Content).get_Item("data").ToObject<List<MixPage>>())
			{
				obj.Specificulture = culture;
				obj.SeoTitle = obj.Title.ToLower();
				obj.SeoName = SeoHelper.GetSEOString(obj.Title, '-');
				obj.SeoDescription = obj.Title.ToLower();
				obj.SeoKeywords = obj.Title.ToLower();
				obj.CreatedDateTime = DateTime.UtcNow;
				obj.CreatedBy = "SuperAdmin";
				context.Entry<MixPage>(obj).set_State(4);
				context.Entry<MixUrlAlias>(new MixUrlAlias()
				{
					Id = obj.Id,
					SourceId = obj.Id.ToString(),
					Type = 0,
					Specificulture = culture,
					CreatedDateTime = DateTime.UtcNow,
					Alias = obj.Title.ToLower(),
					Status = MixEnums.MixContentStatus.Published.ToString()
				}).set_State(4);
			}
		}

		public async Task<RepositoryResponse<bool>> InitThemesAsync(string siteName, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			MixCmsContext mixCmsContext = null;
			IDbContextTransaction dbContextTransaction = null;
			bool flag = false;
			UnitOfWorkHelper<MixCmsContext>.InitTransaction(_context, _transaction, ref mixCmsContext, ref dbContextTransaction, ref flag);
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			if (!mixCmsContext.MixTheme.Any<MixTheme>())
			{
				InitViewModel initViewModel = new InitViewModel()
				{
					Id = 1,
					Title = siteName,
					Name = SeoHelper.GetSEOString(siteName, '-'),
					CreatedDateTime = DateTime.UtcNow,
					CreatedBy = "Admin",
					Status = MixEnums.MixContentStatus.Published
				};
				((ViewModelBase<MixCmsContext, MixTheme, InitViewModel>)initViewModel).ExpandView(mixCmsContext, dbContextTransaction);
				ViewModelHelper.HandleResult<InitViewModel>(await ((ViewModelBase<MixCmsContext, MixTheme, InitViewModel>)initViewModel).SaveModelAsync(true, mixCmsContext, dbContextTransaction), ref repositoryResponse1);
			}
			UnitOfWorkHelper<MixCmsContext>.HandleTransaction(repositoryResponse1.get_IsSucceed(), flag, dbContextTransaction);
			RepositoryResponse<bool> repositoryResponse2 = new RepositoryResponse<bool>();
			repositoryResponse2.set_IsSucceed(repositoryResponse1.get_IsSucceed());
			return repositoryResponse2;
		}
	}
}