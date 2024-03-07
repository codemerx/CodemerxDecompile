using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.Services;
using Mix.Cms.Lib.ViewModels.MixConfigurations;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixCultures
{
	public class UpdateViewModel : ViewModelBase<MixCmsContext, MixCulture, Mix.Cms.Lib.ViewModels.MixCultures.UpdateViewModel>
	{
		[JsonProperty("alias")]
		public string Alias
		{
			get;
			set;
		}

		[JsonProperty("configurations")]
		public List<ReadMvcViewModel> Configurations
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

		public UpdateViewModel()
		{
		}

		public UpdateViewModel(MixCulture model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public async Task<RepositoryResponse<bool>> CloneConfigurationsAsync(MixCulture parent, MixCmsContext context, IDbContextTransaction transaction)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			try
			{
				DefaultModelRepository<MixCmsContext, MixConfiguration> instance = DefaultModelRepository<MixCmsContext, MixConfiguration>.get_Instance();
				RepositoryResponse<List<MixConfiguration>> modelListByAsync = await instance.GetModelListByAsync((MixConfiguration c) => c.Specificulture == MixService.GetConfig<string>("DefaultCulture"), null, null);
				if (modelListByAsync.get_IsSucceed())
				{
					foreach (MixConfiguration datum in modelListByAsync.get_Data())
					{
						datum.Specificulture = this.Specificulture;
						datum.CreatedDateTime = DateTime.UtcNow;
						context.Entry<MixConfiguration>(datum).set_State(4);
					}
					MixCmsContext mixCmsContext = context;
					CancellationToken cancellationToken = new CancellationToken();
					await ((DbContext)mixCmsContext).SaveChangesAsync(cancellationToken);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				repositoryResponse1.set_IsSucceed(false);
				repositoryResponse1.set_Exception(exception);
				repositoryResponse1.get_Errors().Add(exception.Message);
			}
			return repositoryResponse1;
		}

		public async Task<RepositoryResponse<bool>> CloneLanguagesAsync(MixCulture parent, MixCmsContext context, IDbContextTransaction transaction)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			try
			{
				DefaultModelRepository<MixCmsContext, MixLanguage> instance = DefaultModelRepository<MixCmsContext, MixLanguage>.get_Instance();
				RepositoryResponse<List<MixLanguage>> modelListByAsync = await instance.GetModelListByAsync((MixLanguage c) => c.Specificulture == MixService.GetConfig<string>("DefaultCulture"), null, null);
				if (modelListByAsync.get_IsSucceed())
				{
					foreach (MixLanguage datum in modelListByAsync.get_Data())
					{
						datum.Specificulture = this.Specificulture;
						datum.CreatedDateTime = DateTime.UtcNow;
						context.Entry<MixLanguage>(datum).set_State(4);
					}
					MixCmsContext mixCmsContext = context;
					CancellationToken cancellationToken = new CancellationToken();
					await ((DbContext)mixCmsContext).SaveChangesAsync(cancellationToken);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				repositoryResponse1.set_IsSucceed(false);
				repositoryResponse1.set_Exception(exception);
				repositoryResponse1.get_Errors().Add(exception.Message);
			}
			return repositoryResponse1;
		}

		public async Task<RepositoryResponse<bool>> CloneMediasAsync(MixCulture parent, MixCmsContext context, IDbContextTransaction transaction)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			try
			{
				DefaultModelRepository<MixCmsContext, MixMedia> instance = DefaultModelRepository<MixCmsContext, MixMedia>.get_Instance();
				RepositoryResponse<List<MixMedia>> modelListByAsync = await instance.GetModelListByAsync((MixMedia c) => c.Specificulture == MixService.GetConfig<string>("DefaultCulture"), null, null);
				if (modelListByAsync.get_IsSucceed())
				{
					foreach (MixMedia datum in modelListByAsync.get_Data())
					{
						datum.Specificulture = this.Specificulture;
						context.Entry<MixMedia>(datum).set_State(4);
					}
					MixCmsContext mixCmsContext = context;
					CancellationToken cancellationToken = new CancellationToken();
					await ((DbContext)mixCmsContext).SaveChangesAsync(cancellationToken);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				repositoryResponse1.set_IsSucceed(false);
				repositoryResponse1.set_Exception(exception);
				repositoryResponse1.get_Errors().Add(exception.Message);
			}
			return repositoryResponse1;
		}

		public async Task<RepositoryResponse<bool>> CloneModuleDatasAsync(MixCulture parent, MixCmsContext context, IDbContextTransaction transaction)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			try
			{
				DefaultModelRepository<MixCmsContext, MixModulePost> instance = DefaultModelRepository<MixCmsContext, MixModulePost>.get_Instance();
				RepositoryResponse<List<MixModulePost>> modelListByAsync = await instance.GetModelListByAsync((MixModulePost c) => c.Specificulture == MixService.GetConfig<string>("DefaultCulture"), null, null);
				if (modelListByAsync.get_IsSucceed())
				{
					foreach (MixModulePost datum in modelListByAsync.get_Data())
					{
						datum.Specificulture = this.Specificulture;
						context.Entry<MixModulePost>(datum).set_State(4);
					}
					MixCmsContext mixCmsContext = context;
					CancellationToken cancellationToken = new CancellationToken();
					await ((DbContext)mixCmsContext).SaveChangesAsync(cancellationToken);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				repositoryResponse1.set_IsSucceed(false);
				repositoryResponse1.set_Exception(exception);
				repositoryResponse1.get_Errors().Add(exception.Message);
			}
			return repositoryResponse1;
		}

		public async Task<RepositoryResponse<bool>> CloneModulePostsAsync(MixCulture parent, MixCmsContext context, IDbContextTransaction transaction)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			try
			{
				DefaultModelRepository<MixCmsContext, MixModulePost> instance = DefaultModelRepository<MixCmsContext, MixModulePost>.get_Instance();
				RepositoryResponse<List<MixModulePost>> modelListByAsync = await instance.GetModelListByAsync((MixModulePost c) => c.Specificulture == MixService.GetConfig<string>("DefaultCulture"), null, null);
				if (modelListByAsync.get_IsSucceed())
				{
					foreach (MixModulePost datum in modelListByAsync.get_Data())
					{
						datum.Specificulture = this.Specificulture;
						context.Entry<MixModulePost>(datum).set_State(4);
					}
					MixCmsContext mixCmsContext = context;
					CancellationToken cancellationToken = new CancellationToken();
					await ((DbContext)mixCmsContext).SaveChangesAsync(cancellationToken);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				repositoryResponse1.set_IsSucceed(false);
				repositoryResponse1.set_Exception(exception);
				repositoryResponse1.get_Errors().Add(exception.Message);
			}
			return repositoryResponse1;
		}

		public async Task<RepositoryResponse<bool>> CloneModulesAsync(MixCulture parent, MixCmsContext context, IDbContextTransaction transaction)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			try
			{
				DefaultModelRepository<MixCmsContext, MixModule> instance = DefaultModelRepository<MixCmsContext, MixModule>.get_Instance();
				RepositoryResponse<List<MixModule>> modelListByAsync = await instance.GetModelListByAsync((MixModule c) => c.Specificulture == MixService.GetConfig<string>("DefaultCulture"), null, null);
				if (modelListByAsync.get_IsSucceed())
				{
					foreach (MixModule datum in modelListByAsync.get_Data())
					{
						datum.Specificulture = this.Specificulture;
						context.Entry<MixModule>(datum).set_State(4);
					}
					MixCmsContext mixCmsContext = context;
					CancellationToken cancellationToken = new CancellationToken();
					await ((DbContext)mixCmsContext).SaveChangesAsync(cancellationToken);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				repositoryResponse1.set_IsSucceed(false);
				repositoryResponse1.set_Exception(exception);
				repositoryResponse1.get_Errors().Add(exception.Message);
			}
			return repositoryResponse1;
		}

		public async Task<RepositoryResponse<bool>> ClonePageModulesAsync(MixCulture parent, MixCmsContext context, IDbContextTransaction transaction)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			try
			{
				DefaultModelRepository<MixCmsContext, MixPageModule> instance = DefaultModelRepository<MixCmsContext, MixPageModule>.get_Instance();
				RepositoryResponse<List<MixPageModule>> modelListByAsync = await instance.GetModelListByAsync((MixPageModule c) => c.Specificulture == MixService.GetConfig<string>("DefaultCulture"), null, null);
				if (modelListByAsync.get_IsSucceed())
				{
					foreach (MixPageModule datum in modelListByAsync.get_Data())
					{
						datum.Specificulture = this.Specificulture;
						context.Entry<MixPageModule>(datum).set_State(4);
					}
					MixCmsContext mixCmsContext = context;
					CancellationToken cancellationToken = new CancellationToken();
					await ((DbContext)mixCmsContext).SaveChangesAsync(cancellationToken);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				repositoryResponse1.set_IsSucceed(false);
				repositoryResponse1.set_Exception(exception);
				repositoryResponse1.get_Errors().Add(exception.Message);
			}
			return repositoryResponse1;
		}

		public async Task<RepositoryResponse<bool>> ClonePagePostsAsync(MixCulture parent, MixCmsContext context, IDbContextTransaction transaction)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			try
			{
				DefaultModelRepository<MixCmsContext, MixPagePost> instance = DefaultModelRepository<MixCmsContext, MixPagePost>.get_Instance();
				RepositoryResponse<List<MixPagePost>> modelListByAsync = await instance.GetModelListByAsync((MixPagePost c) => c.Specificulture == MixService.GetConfig<string>("DefaultCulture"), null, null);
				if (modelListByAsync.get_IsSucceed())
				{
					foreach (MixPagePost datum in modelListByAsync.get_Data())
					{
						datum.Specificulture = this.Specificulture;
						context.Entry<MixPagePost>(datum).set_State(4);
					}
					MixCmsContext mixCmsContext = context;
					CancellationToken cancellationToken = new CancellationToken();
					await ((DbContext)mixCmsContext).SaveChangesAsync(cancellationToken);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				repositoryResponse1.set_IsSucceed(false);
				repositoryResponse1.set_Exception(exception);
				repositoryResponse1.get_Errors().Add(exception.Message);
			}
			return repositoryResponse1;
		}

		public async Task<RepositoryResponse<bool>> ClonePagesAsync(MixCulture parent, MixCmsContext context, IDbContextTransaction transaction)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			try
			{
				DefaultModelRepository<MixCmsContext, MixPage> instance = DefaultModelRepository<MixCmsContext, MixPage>.get_Instance();
				RepositoryResponse<List<MixPage>> modelListByAsync = await instance.GetModelListByAsync((MixPage c) => c.Specificulture == MixService.GetConfig<string>("DefaultCulture"), null, null);
				if (modelListByAsync.get_IsSucceed())
				{
					foreach (MixPage datum in modelListByAsync.get_Data())
					{
						datum.Specificulture = this.Specificulture;
						datum.CreatedDateTime = DateTime.UtcNow;
						datum.LastModified = new DateTime?(DateTime.UtcNow);
						context.Entry<MixPage>(datum).set_State(4);
					}
					MixCmsContext mixCmsContext = context;
					CancellationToken cancellationToken = new CancellationToken();
					await ((DbContext)mixCmsContext).SaveChangesAsync(cancellationToken);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				repositoryResponse1.set_IsSucceed(false);
				repositoryResponse1.set_Exception(exception);
				repositoryResponse1.get_Errors().Add(exception.Message);
			}
			return repositoryResponse1;
		}

		public async Task<RepositoryResponse<bool>> ClonePostMediasAsync(MixCulture parent, MixCmsContext context, IDbContextTransaction transaction)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			try
			{
				DefaultModelRepository<MixCmsContext, MixPostMedia> instance = DefaultModelRepository<MixCmsContext, MixPostMedia>.get_Instance();
				RepositoryResponse<List<MixPostMedia>> modelListByAsync = await instance.GetModelListByAsync((MixPostMedia c) => c.Specificulture == MixService.GetConfig<string>("DefaultCulture"), null, null);
				if (modelListByAsync.get_IsSucceed())
				{
					foreach (MixPostMedia datum in modelListByAsync.get_Data())
					{
						datum.Specificulture = this.Specificulture;
						context.Entry<MixPostMedia>(datum).set_State(4);
					}
					MixCmsContext mixCmsContext = context;
					CancellationToken cancellationToken = new CancellationToken();
					await ((DbContext)mixCmsContext).SaveChangesAsync(cancellationToken);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				repositoryResponse1.set_IsSucceed(false);
				repositoryResponse1.set_Exception(exception);
				repositoryResponse1.get_Errors().Add(exception.Message);
			}
			return repositoryResponse1;
		}

		public async Task<RepositoryResponse<bool>> ClonePostPostsAsync(MixCulture parent, MixCmsContext context, IDbContextTransaction transaction)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			try
			{
				DefaultModelRepository<MixCmsContext, MixRelatedPost> instance = DefaultModelRepository<MixCmsContext, MixRelatedPost>.get_Instance();
				RepositoryResponse<List<MixRelatedPost>> modelListByAsync = await instance.GetModelListByAsync((MixRelatedPost c) => c.Specificulture == MixService.GetConfig<string>("DefaultCulture"), null, null);
				if (modelListByAsync.get_IsSucceed())
				{
					foreach (MixRelatedPost datum in modelListByAsync.get_Data())
					{
						datum.Specificulture = this.Specificulture;
						datum.CreatedDateTime = DateTime.UtcNow;
						context.Entry<MixRelatedPost>(datum).set_State(4);
					}
					MixCmsContext mixCmsContext = context;
					CancellationToken cancellationToken = new CancellationToken();
					await ((DbContext)mixCmsContext).SaveChangesAsync(cancellationToken);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				repositoryResponse1.set_IsSucceed(false);
				repositoryResponse1.set_Exception(exception);
				repositoryResponse1.get_Errors().Add(exception.Message);
			}
			return repositoryResponse1;
		}

		public async Task<RepositoryResponse<bool>> ClonePostsAsync(MixCulture parent, MixCmsContext context, IDbContextTransaction transaction)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			try
			{
				DefaultModelRepository<MixCmsContext, MixPost> instance = DefaultModelRepository<MixCmsContext, MixPost>.get_Instance();
				RepositoryResponse<List<MixPost>> modelListByAsync = await instance.GetModelListByAsync((MixPost c) => c.Specificulture == MixService.GetConfig<string>("DefaultCulture"), null, null);
				if (modelListByAsync.get_IsSucceed())
				{
					foreach (MixPost datum in modelListByAsync.get_Data())
					{
						datum.Specificulture = this.Specificulture;
						context.Entry<MixPost>(datum).set_State(4);
					}
					MixCmsContext mixCmsContext = context;
					CancellationToken cancellationToken = new CancellationToken();
					await ((DbContext)mixCmsContext).SaveChangesAsync(cancellationToken);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				repositoryResponse1.set_IsSucceed(false);
				repositoryResponse1.set_Exception(exception);
				repositoryResponse1.get_Errors().Add(exception.Message);
			}
			return repositoryResponse1;
		}

		public async Task<RepositoryResponse<bool>> CloneUrlAliasAsync(MixCulture parent, MixCmsContext context, IDbContextTransaction transaction)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			try
			{
				DefaultModelRepository<MixCmsContext, MixUrlAlias> instance = DefaultModelRepository<MixCmsContext, MixUrlAlias>.get_Instance();
				RepositoryResponse<List<MixUrlAlias>> modelListByAsync = await instance.GetModelListByAsync((MixUrlAlias c) => c.Specificulture == MixService.GetConfig<string>("DefaultCulture"), null, null);
				if (modelListByAsync.get_IsSucceed())
				{
					foreach (MixUrlAlias datum in modelListByAsync.get_Data())
					{
						datum.Specificulture = this.Specificulture;
						datum.CreatedDateTime = DateTime.UtcNow;
						context.Entry<MixUrlAlias>(datum).set_State(4);
					}
					MixCmsContext mixCmsContext = context;
					CancellationToken cancellationToken = new CancellationToken();
					await ((DbContext)mixCmsContext).SaveChangesAsync(cancellationToken);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				repositoryResponse1.set_IsSucceed(false);
				repositoryResponse1.set_Exception(exception);
				repositoryResponse1.get_Errors().Add(exception.Message);
			}
			return repositoryResponse1;
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<List<ReadMvcViewModel>> modelListBy = ViewModelBase<MixCmsContext, MixConfiguration, ReadMvcViewModel>.Repository.GetModelListBy((MixConfiguration c) => c.Specificulture == this.Specificulture, _context, _transaction);
			if (modelListBy.get_IsSucceed())
			{
				this.Configurations = modelListBy.get_Data();
			}
		}

		public override MixCulture ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (this.Id == 0)
			{
				this.Id = ViewModelBase<MixCmsContext, MixCulture, Mix.Cms.Lib.ViewModels.MixCultures.UpdateViewModel>.Repository.Max((MixCulture m) => m.Id, null, null).get_Data() + 1;
				this.CreatedDateTime = DateTime.UtcNow;
			}
			return base.ParseModel(_context, _transaction);
		}

		public override async Task<RepositoryResponse<MixCulture>> RemoveModelAsync(bool isRemoveRelatedModels = false, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<MixCulture> repositoryResponse = await this.u003cu003en__1(isRemoveRelatedModels, _context, _transaction);
			if (repositoryResponse.get_IsSucceed() && repositoryResponse.get_IsSucceed())
			{
				MixService.LoadFromDatabase(null, null);
				MixService.SaveSettings();
			}
			return repositoryResponse;
		}

		public override async Task<RepositoryResponse<bool>> RemoveRelatedModelsAsync(Mix.Cms.Lib.ViewModels.MixCultures.UpdateViewModel view, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			DbSet<MixConfiguration> mixConfiguration = _context.MixConfiguration;
			IQueryable<MixConfiguration> specificulture = 
				from c in mixConfiguration
				where c.Specificulture == this.Specificulture
				select c;
			CancellationToken cancellationToken = new CancellationToken();
			await EntityFrameworkQueryableExtensions.ToListAsync<MixConfiguration>(specificulture, cancellationToken).ForEach((MixConfiguration c) => _context.Entry<MixConfiguration>(c).set_State(2));
			DbSet<MixLanguage> mixLanguage = _context.MixLanguage;
			IQueryable<MixLanguage> mixLanguages = 
				from l in mixLanguage
				where l.Specificulture == this.Specificulture
				select l;
			cancellationToken = new CancellationToken();
			await EntityFrameworkQueryableExtensions.ToListAsync<MixLanguage>(mixLanguages, cancellationToken).ForEach((MixLanguage l) => _context.Entry<MixLanguage>(l).set_State(2));
			DbSet<MixPageModule> mixPageModule = _context.MixPageModule;
			IQueryable<MixPageModule> mixPageModules = 
				from l in mixPageModule
				where l.Specificulture == this.Specificulture
				select l;
			cancellationToken = new CancellationToken();
			await EntityFrameworkQueryableExtensions.ToListAsync<MixPageModule>(mixPageModules, cancellationToken).ForEach((MixPageModule l) => _context.Entry<MixPageModule>(l).set_State(2));
			DbSet<MixPagePost> mixPagePost = _context.MixPagePost;
			IQueryable<MixPagePost> mixPagePosts = 
				from l in mixPagePost
				where l.Specificulture == this.Specificulture
				select l;
			cancellationToken = new CancellationToken();
			await EntityFrameworkQueryableExtensions.ToListAsync<MixPagePost>(mixPagePosts, cancellationToken).ForEach((MixPagePost l) => _context.Entry<MixPagePost>(l).set_State(2));
			DbSet<MixModulePost> mixModulePost = _context.MixModulePost;
			IQueryable<MixModulePost> mixModulePosts = 
				from l in mixModulePost
				where l.Specificulture == this.Specificulture
				select l;
			cancellationToken = new CancellationToken();
			await EntityFrameworkQueryableExtensions.ToListAsync<MixModulePost>(mixModulePosts, cancellationToken).ForEach((MixModulePost l) => _context.Entry<MixModulePost>(l).set_State(2));
			DbSet<MixPostMedia> mixPostMedia = _context.MixPostMedia;
			IQueryable<MixPostMedia> mixPostMedias = 
				from l in mixPostMedia
				where l.Specificulture == this.Specificulture
				select l;
			cancellationToken = new CancellationToken();
			await EntityFrameworkQueryableExtensions.ToListAsync<MixPostMedia>(mixPostMedias, cancellationToken).ForEach((MixPostMedia l) => _context.Entry<MixPostMedia>(l).set_State(2));
			DbSet<MixModuleData> mixModuleData = _context.MixModuleData;
			IQueryable<MixModuleData> mixModuleDatas = 
				from l in mixModuleData
				where l.Specificulture == this.Specificulture
				select l;
			cancellationToken = new CancellationToken();
			await EntityFrameworkQueryableExtensions.ToListAsync<MixModuleData>(mixModuleDatas, cancellationToken).ForEach((MixModuleData l) => _context.Entry<MixModuleData>(l).set_State(2));
			DbSet<MixRelatedPost> mixRelatedPost = _context.MixRelatedPost;
			IQueryable<MixRelatedPost> mixRelatedPosts = 
				from l in mixRelatedPost
				where l.Specificulture == this.Specificulture
				select l;
			cancellationToken = new CancellationToken();
			await EntityFrameworkQueryableExtensions.ToListAsync<MixRelatedPost>(mixRelatedPosts, cancellationToken).ForEach((MixRelatedPost l) => _context.Entry<MixRelatedPost>(l).set_State(2));
			DbSet<MixMedia> mixMedia = _context.MixMedia;
			IQueryable<MixMedia> mixMedias = 
				from c in mixMedia
				where c.Specificulture == this.Specificulture
				select c;
			cancellationToken = new CancellationToken();
			await EntityFrameworkQueryableExtensions.ToListAsync<MixMedia>(mixMedias, cancellationToken).ForEach((MixMedia c) => _context.Entry<MixMedia>(c).set_State(2));
			DbSet<MixPage> mixPage = _context.MixPage;
			IQueryable<MixPage> mixPages = 
				from c in mixPage
				where c.Specificulture == this.Specificulture
				select c;
			cancellationToken = new CancellationToken();
			await EntityFrameworkQueryableExtensions.ToListAsync<MixPage>(mixPages, cancellationToken).ForEach((MixPage c) => _context.Entry<MixPage>(c).set_State(2));
			DbSet<MixModule> mixModule = _context.MixModule;
			IQueryable<MixModule> mixModules = 
				from c in mixModule
				where c.Specificulture == this.Specificulture
				select c;
			cancellationToken = new CancellationToken();
			await EntityFrameworkQueryableExtensions.ToListAsync<MixModule>(mixModules, cancellationToken).ForEach((MixModule c) => _context.Entry<MixModule>(c).set_State(2));
			DbSet<MixPost> mixPost = _context.MixPost;
			IQueryable<MixPost> mixPosts = 
				from c in mixPost
				where c.Specificulture == this.Specificulture
				select c;
			cancellationToken = new CancellationToken();
			await EntityFrameworkQueryableExtensions.ToListAsync<MixPost>(mixPosts, cancellationToken).ForEach((MixPost c) => _context.Entry<MixPost>(c).set_State(2));
			DbSet<MixUrlAlias> mixUrlAlias = _context.MixUrlAlias;
			IQueryable<MixUrlAlias> mixUrlAliases = 
				from c in mixUrlAlias
				where c.Specificulture == this.Specificulture
				select c;
			cancellationToken = new CancellationToken();
			await EntityFrameworkQueryableExtensions.ToListAsync<MixUrlAlias>(mixUrlAliases, cancellationToken).ForEach((MixUrlAlias c) => _context.Entry<MixUrlAlias>(c).set_State(2));
			RepositoryResponse<bool> repositoryResponse2 = repositoryResponse1;
			MixCmsContext mixCmsContext = _context;
			cancellationToken = new CancellationToken();
			int num = await ((DbContext)mixCmsContext).SaveChangesAsync(cancellationToken);
			repositoryResponse2.set_IsSucceed(num > 0);
			repositoryResponse2 = null;
			return repositoryResponse1;
		}

		public override async Task<RepositoryResponse<Mix.Cms.Lib.ViewModels.MixCultures.UpdateViewModel>> SaveModelAsync(bool isSaveSubModels = false, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<Mix.Cms.Lib.ViewModels.MixCultures.UpdateViewModel> repositoryResponse = await this.u003cu003en__0(isSaveSubModels, _context, _transaction);
			if (repositoryResponse.get_IsSucceed())
			{
				MixService.LoadFromDatabase(null, null);
				MixService.SaveSettings();
			}
			return repositoryResponse;
		}

		public override async Task<RepositoryResponse<bool>> SaveSubModelsAsync(MixCulture parent, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			if (repositoryResponse1.get_IsSucceed())
			{
				ViewModelHelper.HandleResult<bool>(await this.CloneConfigurationsAsync(parent, _context, _transaction), ref repositoryResponse1);
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				ViewModelHelper.HandleResult<bool>(await this.CloneLanguagesAsync(parent, _context, _transaction), ref repositoryResponse1);
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				ViewModelHelper.HandleResult<bool>(await this.CloneMediasAsync(parent, _context, _transaction), ref repositoryResponse1);
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				ViewModelHelper.HandleResult<bool>(await this.CloneModulesAsync(parent, _context, _transaction), ref repositoryResponse1);
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				ViewModelHelper.HandleResult<bool>(await this.ClonePagesAsync(parent, _context, _transaction), ref repositoryResponse1);
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				ViewModelHelper.HandleResult<bool>(await this.ClonePostsAsync(parent, _context, _transaction), ref repositoryResponse1);
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				ViewModelHelper.HandleResult<bool>(await this.CloneModuleDatasAsync(parent, _context, _transaction), ref repositoryResponse1);
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				ViewModelHelper.HandleResult<bool>(await this.ClonePageModulesAsync(parent, _context, _transaction), ref repositoryResponse1);
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				ViewModelHelper.HandleResult<bool>(await this.ClonePagePostsAsync(parent, _context, _transaction), ref repositoryResponse1);
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				ViewModelHelper.HandleResult<bool>(await this.CloneModulePostsAsync(parent, _context, _transaction), ref repositoryResponse1);
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				ViewModelHelper.HandleResult<bool>(await this.ClonePostPostsAsync(parent, _context, _transaction), ref repositoryResponse1);
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				ViewModelHelper.HandleResult<bool>(await this.ClonePostMediasAsync(parent, _context, _transaction), ref repositoryResponse1);
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				ViewModelHelper.HandleResult<bool>(await this.CloneUrlAliasAsync(parent, _context, _transaction), ref repositoryResponse1);
			}
			return repositoryResponse1;
		}
	}
}