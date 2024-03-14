using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.Services;
using Mix.Cms.Lib.ViewModels.MixAttributeFields;
using Mix.Cms.Lib.ViewModels.MixAttributeSetDatas;
using Mix.Cms.Lib.ViewModels.MixAttributeSets;
using Mix.Cms.Lib.ViewModels.MixConfigurations;
using Mix.Cms.Lib.ViewModels.MixModuleDatas;
using Mix.Cms.Lib.ViewModels.MixModules;
using Mix.Cms.Lib.ViewModels.MixPageModules;
using Mix.Cms.Lib.ViewModels.MixPages;
using Mix.Cms.Lib.ViewModels.MixPosts;
using Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas;
using Mix.Common.Helper;
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
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels
{
	public class SiteStructureViewModel
	{
		private Dictionary<int, int> dicModuleIds = new Dictionary<int, int>();

		private Dictionary<int, int> dicPageIds = new Dictionary<int, int>();

		private Dictionary<int, int> dicFieldIds = new Dictionary<int, int>();

		private Dictionary<int, int> dicAttributeSetIds = new Dictionary<int, int>();

		[JsonProperty("attributeSetDatas")]
		public List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel> AttributeSetDatas { get; set; } = new List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel>();

		[JsonProperty("attributeSets")]
		public List<Mix.Cms.Lib.ViewModels.MixAttributeSets.ImportViewModel> AttributeSets
		{
			get;
			set;
		}

		[JsonProperty("configurations")]
		public List<Mix.Cms.Lib.ViewModels.MixConfigurations.ReadViewModel> Configurations
		{
			get;
			set;
		}

		[JsonProperty("moduleDatas")]
		public List<Mix.Cms.Lib.ViewModels.MixModuleDatas.UpdateViewModel> ModuleDatas { get; set; } = new List<Mix.Cms.Lib.ViewModels.MixModuleDatas.UpdateViewModel>();

		[JsonProperty("modules")]
		public List<Mix.Cms.Lib.ViewModels.MixModules.ImportViewModel> Modules
		{
			get;
			set;
		}

		[JsonProperty("pages")]
		public List<Mix.Cms.Lib.ViewModels.MixPages.ImportViewModel> Pages
		{
			get;
			set;
		}

		[JsonProperty("posts")]
		public List<Mix.Cms.Lib.ViewModels.MixPosts.ImportViewModel> Posts { get; set; } = new List<Mix.Cms.Lib.ViewModels.MixPosts.ImportViewModel>();

		[JsonProperty("relatedData")]
		public List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.ImportViewModel> RelatedData { get; set; } = new List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.ImportViewModel>();

		[JsonProperty("specificulture")]
		public string Specificulture
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

		public SiteStructureViewModel()
		{
		}

		private void GetAdditionalData(string id, MixEnums.MixAttributeSetDataType type, MixCmsContext context, IDbContextTransaction transaction)
		{
			SiteStructureViewModel.u003cu003ec__DisplayClass51_0 variable = null;
			if (!this.RelatedData.Any<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.ImportViewModel>((Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.ImportViewModel m) => {
				if (m.ParentId != id)
				{
					return false;
				}
				return m.ParentType == type;
			}))
			{
				DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixRelatedAttributeData, Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.ImportViewModel>.Repository;
				ParameterExpression parameterExpression = Expression.Parameter(typeof(MixRelatedAttributeData), "m");
				RepositoryResponse<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.ImportViewModel> singleModel = repository.GetSingleModel(Expression.Lambda<Func<MixRelatedAttributeData, bool>>(Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(SiteStructureViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(SiteStructureViewModel).GetMethod("get_Specificulture").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_ParentType").MethodHandle)), Expression.Call(Expression.Field(Expression.Constant(variable, typeof(SiteStructureViewModel.u003cu003ec__DisplayClass51_0)), FieldInfo.GetFieldFromHandle(typeof(SiteStructureViewModel.u003cu003ec__DisplayClass51_0).GetField("type").FieldHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(object).GetMethod("ToString").MethodHandle), Array.Empty<Expression>()))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_ParentId").MethodHandle)), Expression.Field(Expression.Constant(variable, typeof(SiteStructureViewModel.u003cu003ec__DisplayClass51_0)), FieldInfo.GetFieldFromHandle(typeof(SiteStructureViewModel.u003cu003ec__DisplayClass51_0).GetField("id").FieldHandle)))), new ParameterExpression[] { parameterExpression }), context, transaction);
				if (singleModel.get_IsSucceed())
				{
					this.RelatedData.Add(singleModel.get_Data());
				}
			}
		}

		public async Task<RepositoryResponse<bool>> ImportAsync(string destCulture, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			SiteStructureViewModel.u003cImportAsyncu003ed__57 variable = new SiteStructureViewModel.u003cImportAsyncu003ed__57();
			variable.u003cu003e4__this = this;
			variable.destCulture = destCulture;
			variable._context = _context;
			variable._transaction = _transaction;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<SiteStructureViewModel.u003cImportAsyncu003ed__57>(ref variable);
			return variable.u003cu003et__builder.Task;
		}

		private async Task<RepositoryResponse<bool>> ImportAttributeSetDatas(string destCulture, MixCmsContext context, IDbContextTransaction transaction)
		{
			SiteStructureViewModel.u003cImportAttributeSetDatasu003ed__61 variable = new SiteStructureViewModel.u003cImportAttributeSetDatasu003ed__61();
			variable.u003cu003e4__this = this;
			variable.destCulture = destCulture;
			variable.context = context;
			variable.transaction = transaction;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<SiteStructureViewModel.u003cImportAttributeSetDatasu003ed__61>(ref variable);
			return variable.u003cu003et__builder.Task;
		}

		private async Task<RepositoryResponse<bool>> ImportAttributeSetsAsync(MixCmsContext context, IDbContextTransaction transaction)
		{
			SiteStructureViewModel.u003cImportAttributeSetsAsyncu003ed__59 variable = new SiteStructureViewModel.u003cImportAttributeSetsAsyncu003ed__59();
			variable.u003cu003e4__this = this;
			variable.context = context;
			variable.transaction = transaction;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<SiteStructureViewModel.u003cImportAttributeSetsAsyncu003ed__59>(ref variable);
			return variable.u003cu003et__builder.Task;
		}

		private async Task<RepositoryResponse<bool>> ImportModulesAsync(string destCulture, MixCmsContext context, IDbContextTransaction transaction)
		{
			SiteStructureViewModel.u003cImportModulesAsyncu003ed__58 variable = new SiteStructureViewModel.u003cImportModulesAsyncu003ed__58();
			variable.u003cu003e4__this = this;
			variable.destCulture = destCulture;
			variable.context = context;
			variable.transaction = transaction;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<SiteStructureViewModel.u003cImportModulesAsyncu003ed__58>(ref variable);
			return variable.u003cu003et__builder.Task;
		}

		private async Task<RepositoryResponse<bool>> ImportPagesAsync(string destCulture, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			SiteStructureViewModel.u003cImportPagesAsyncu003ed__60 variable = new SiteStructureViewModel.u003cImportPagesAsyncu003ed__60();
			variable.u003cu003e4__this = this;
			variable.destCulture = destCulture;
			variable._context = _context;
			variable._transaction = _transaction;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<SiteStructureViewModel.u003cImportPagesAsyncu003ed__60>(ref variable);
			return variable.u003cu003et__builder.Task;
		}

		private async Task<RepositoryResponse<bool>> ImportRelatedDatas(string desCulture, MixCmsContext context, IDbContextTransaction transaction)
		{
			SiteStructureViewModel.u003cImportRelatedDatasu003ed__62 variable = new SiteStructureViewModel.u003cImportRelatedDatasu003ed__62();
			variable.u003cu003e4__this = this;
			variable.desCulture = desCulture;
			variable.context = context;
			variable.transaction = transaction;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<SiteStructureViewModel.u003cImportRelatedDatasu003ed__62>(ref variable);
			return variable.u003cu003et__builder.Task;
		}

		public async Task InitAsync(string culture)
		{
			DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixPage, Mix.Cms.Lib.ViewModels.MixPages.ImportViewModel>.Repository;
			this.Pages = await repository.GetModelListByAsync((MixPage p) => p.Specificulture == culture, null, null).get_Data();
			DefaultRepository<!0, !1, !2> defaultRepository = ViewModelBase<MixCmsContext, MixModule, Mix.Cms.Lib.ViewModels.MixModules.ImportViewModel>.Repository;
			this.Modules = await defaultRepository.GetModelListByAsync((MixModule p) => p.Specificulture == culture, null, null).get_Data();
			this.AttributeSets = await ViewModelBase<MixCmsContext, MixAttributeSet, Mix.Cms.Lib.ViewModels.MixAttributeSets.ImportViewModel>.Repository.GetModelListAsync(null, null).get_Data();
		}

		private void ProcessAttributeDatas(MixCmsContext context, IDbContextTransaction transaction)
		{
		}

		private void ProcessAttributeSetData(MixCmsContext context, IDbContextTransaction transaction)
		{
			List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel> list;
			this.AttributeSetDatas = new List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel>();
			foreach (Mix.Cms.Lib.ViewModels.MixAttributeSets.ImportViewModel attributeSet in this.AttributeSets)
			{
				if (!attributeSet.IsExportData)
				{
					continue;
				}
				List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel> data = ViewModelBase<MixCmsContext, MixAttributeSetData, Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel>.Repository.GetModelListBy((MixAttributeSetData a) => a.Specificulture == this.Specificulture && a.AttributeSetId == attributeSet.Id, context, transaction).get_Data();
				if (data != null)
				{
					list = (
						from a in data
						orderby a.Priority
						select a).ToList<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel>();
				}
				else
				{
					list = null;
				}
				List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel> importViewModels = list;
				if (importViewModels == null)
				{
					continue;
				}
				this.AttributeSetDatas.AddRange(importViewModels);
			}
			foreach (Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.ImportViewModel relatedDatum in this.RelatedData)
			{
				if (this.AttributeSetDatas.Any<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel>((Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel m) => m.Id == relatedDatum.Id))
				{
					continue;
				}
				RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel> singleModel = ViewModelBase<MixCmsContext, MixAttributeSetData, Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel>.Repository.GetSingleModel((MixAttributeSetData m) => m.Id == relatedDatum.Id, context, transaction);
				if (!singleModel.get_IsSucceed())
				{
					continue;
				}
				this.AttributeSetDatas.Add(singleModel.get_Data());
			}
		}

		private void ProcessAttributeSetsAsync(MixCmsContext context, IDbContextTransaction transaction)
		{
			List<Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel> list;
			foreach (Mix.Cms.Lib.ViewModels.MixAttributeSets.ImportViewModel attributeSet in this.AttributeSets)
			{
				Mix.Cms.Lib.ViewModels.MixAttributeSets.ImportViewModel importViewModel = attributeSet;
				List<Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel> data = ViewModelBase<MixCmsContext, MixAttributeField, Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel>.Repository.GetModelListBy((MixAttributeField a) => a.AttributeSetId == attributeSet.Id, context, transaction).get_Data();
				if (data != null)
				{
					list = (
						from a in data
						orderby a.Priority
						select a).ToList<Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel>();
				}
				else
				{
					list = null;
				}
				importViewModel.Fields = list;
				foreach (Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel updateViewModel in 
					from f in attributeSet.Fields
					where f.DataType == MixEnums.MixDataType.Reference
					select f)
				{
					Mix.Cms.Lib.ViewModels.MixAttributeSets.ImportViewModel data1 = this.AttributeSets.FirstOrDefault<Mix.Cms.Lib.ViewModels.MixAttributeSets.ImportViewModel>((Mix.Cms.Lib.ViewModels.MixAttributeSets.ImportViewModel m) => m.Name == updateViewModel.AttributeSetName);
					if (data1 != null)
					{
						data1.IsExportData = (data1.IsExportData ? true : attributeSet.IsExportData);
					}
					else
					{
						RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSets.ImportViewModel> singleModel = ViewModelBase<MixCmsContext, MixAttributeSet, Mix.Cms.Lib.ViewModels.MixAttributeSets.ImportViewModel>.Repository.GetSingleModel((MixAttributeSet m) => m.Name == updateViewModel.AttributeSetName, context, transaction);
						if (!singleModel.get_IsSucceed())
						{
							continue;
						}
						data1 = singleModel.get_Data();
						data1.IsExportData = (data1.IsExportData ? true : attributeSet.IsExportData);
						this.AttributeSets.Add(data1);
					}
				}
				if (!attributeSet.IsExportData)
				{
					continue;
				}
				List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel> importViewModels = attributeSet.Data;
			}
		}

		private void ProcessDatas(MixCmsContext context, IDbContextTransaction transaction)
		{
			this.ProcessPosts(context, transaction);
			this.ProcessAttributeDatas(context, transaction);
			this.ProcessModuleDatas(context, transaction);
		}

		private void ProcessModuleData(Mix.Cms.Lib.ViewModels.MixModules.ImportViewModel item, MixCmsContext context, IDbContextTransaction transaction)
		{
			DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixModuleData, Mix.Cms.Lib.ViewModels.MixModuleDatas.ReadViewModel>.Repository;
			int? nullable = null;
			int? nullable1 = nullable;
			nullable = null;
			RepositoryResponse<PaginationModel<Mix.Cms.Lib.ViewModels.MixModuleDatas.ReadViewModel>> modelListBy = repository.GetModelListBy((MixModuleData m) => m.ModuleId == item.Id && m.Specificulture == item.Specificulture, "Priority", 0, nullable1, nullable, context, transaction);
			if (modelListBy.get_IsSucceed())
			{
				item.Data = modelListBy.get_Data();
			}
			int id = item.Id;
			this.GetAdditionalData(id.ToString(), MixEnums.MixAttributeSetDataType.Module, context, transaction);
		}

		private void ProcessModuleDatas(MixCmsContext context, IDbContextTransaction transaction)
		{
		}

		private void ProcessModules(MixCmsContext context, IDbContextTransaction transaction)
		{
			foreach (Mix.Cms.Lib.ViewModels.MixModules.ImportViewModel module in this.Modules)
			{
				if (!module.IsExportData)
				{
					continue;
				}
				this.ProcessModuleData(module, context, transaction);
			}
		}

		private void ProcessPages(MixCmsContext context, IDbContextTransaction transaction)
		{
			foreach (Mix.Cms.Lib.ViewModels.MixPages.ImportViewModel page in this.Pages)
			{
				if (!page.IsExportData)
				{
					continue;
				}
				page.ModuleNavs = page.GetModuleNavs(context, transaction);
				foreach (Mix.Cms.Lib.ViewModels.MixPageModules.ImportViewModel moduleNav in page.ModuleNavs)
				{
					Mix.Cms.Lib.ViewModels.MixModules.ImportViewModel importViewModel = this.Modules.FirstOrDefault<Mix.Cms.Lib.ViewModels.MixModules.ImportViewModel>((Mix.Cms.Lib.ViewModels.MixModules.ImportViewModel m) => {
						if (m.Id != moduleNav.ModuleId)
						{
							return false;
						}
						return m.Specificulture == this.Specificulture;
					});
					if (importViewModel == null)
					{
						moduleNav.Module.IsExportData = true;
					}
					else
					{
						this.Modules.Remove(importViewModel);
					}
					this.ProcessModuleData(moduleNav.Module, context, transaction);
				}
				page.UrlAliases = page.GetAliases(context, transaction);
				int id = page.Id;
				this.GetAdditionalData(id.ToString(), MixEnums.MixAttributeSetDataType.Page, context, transaction);
			}
		}

		private void ProcessPosts(MixCmsContext context, IDbContextTransaction transaction)
		{
		}

		public RepositoryResponse<string> ProcessSelectedExportDataAsync()
		{
			MixCmsContext mixCmsContext = null;
			IDbContextTransaction dbContextTransaction = null;
			bool flag = false;
			RepositoryResponse<string> repositoryResponse;
			UnitOfWorkHelper<MixCmsContext>.InitTransaction(null, null, ref mixCmsContext, ref dbContextTransaction, ref flag);
			RepositoryResponse<string> repositoryResponse1 = new RepositoryResponse<string>();
			repositoryResponse1.set_IsSucceed(true);
			RepositoryResponse<string> repositoryResponse2 = repositoryResponse1;
			try
			{
				try
				{
					this.ProcessPages(mixCmsContext, dbContextTransaction);
					this.ProcessModules(mixCmsContext, dbContextTransaction);
					this.ProcessAttributeSetsAsync(mixCmsContext, dbContextTransaction);
					this.ProcessAttributeSetData(mixCmsContext, dbContextTransaction);
					this.ProcessDatas(mixCmsContext, dbContextTransaction);
					repositoryResponse = repositoryResponse2;
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					RepositoryResponse<Mix.Cms.Lib.ViewModels.MixPages.ImportViewModel> repositoryResponse3 = UnitOfWorkHelper<MixCmsContext>.HandleException<Mix.Cms.Lib.ViewModels.MixPages.ImportViewModel>(exception, flag, dbContextTransaction);
					repositoryResponse2.set_IsSucceed(false);
					repositoryResponse2.set_Errors(repositoryResponse3.get_Errors());
					repositoryResponse2.set_Exception(exception);
					repositoryResponse = repositoryResponse2;
				}
			}
			finally
			{
				if (flag)
				{
					RelationalDatabaseFacadeExtensions.CloseConnection(mixCmsContext.get_Database());
					dbContextTransaction.Dispose();
					mixCmsContext.Dispose();
				}
			}
			return repositoryResponse;
		}
	}
}