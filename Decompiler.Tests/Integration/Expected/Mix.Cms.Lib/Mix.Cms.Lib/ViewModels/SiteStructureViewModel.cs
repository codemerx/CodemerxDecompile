using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
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
using Mix.Domain.Core.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels
{
	public class SiteStructureViewModel
	{
		private Dictionary<int, int> dicModuleIds;

		private Dictionary<int, int> dicPageIds;

		private Dictionary<int, int> dicFieldIds;

		private Dictionary<int, int> dicAttributeSetIds;

		[JsonProperty("attributeSetDatas")]
		public List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel> AttributeSetDatas
		{
			get;
			set;
		}

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
		public List<Mix.Cms.Lib.ViewModels.MixModuleDatas.UpdateViewModel> ModuleDatas
		{
			get;
			set;
		}

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
		public List<Mix.Cms.Lib.ViewModels.MixPosts.ImportViewModel> Posts
		{
			get;
			set;
		}

		[JsonProperty("relatedData")]
		public List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.ImportViewModel> RelatedData
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

		[JsonProperty("themeName")]
		public string ThemeName
		{
			get;
			set;
		}

		public SiteStructureViewModel()
		{
			this.u003cRelatedDatau003ek__BackingField = new List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.ImportViewModel>();
			this.u003cPostsu003ek__BackingField = new List<Mix.Cms.Lib.ViewModels.MixPosts.ImportViewModel>();
			this.u003cModuleDatasu003ek__BackingField = new List<Mix.Cms.Lib.ViewModels.MixModuleDatas.UpdateViewModel>();
			this.u003cAttributeSetDatasu003ek__BackingField = new List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel>();
			this.dicModuleIds = new Dictionary<int, int>();
			this.dicPageIds = new Dictionary<int, int>();
			this.dicFieldIds = new Dictionary<int, int>();
			this.dicAttributeSetIds = new Dictionary<int, int>();
			base();
			return;
		}

		private void GetAdditionalData(string id, MixEnums.MixAttributeSetDataType type, MixCmsContext context, IDbContextTransaction transaction)
		{
			V_0 = new SiteStructureViewModel.u003cu003ec__DisplayClass51_0();
			V_0.id = id;
			V_0.type = type;
			V_0.u003cu003e4__this = this;
			if (!Enumerable.Any<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.ImportViewModel>(this.get_RelatedData(), new Func<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.ImportViewModel, bool>(V_0, SiteStructureViewModel.u003cu003ec__DisplayClass51_0.u003cGetAdditionalDatau003eb__0)))
			{
				stackVariable13 = ViewModelBase<MixCmsContext, MixRelatedAttributeData, Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.ImportViewModel>.Repository;
				V_2 = Expression.Parameter(Type.GetTypeFromHandle(// 
				// Current member / type: System.Void Mix.Cms.Lib.ViewModels.SiteStructureViewModel::GetAdditionalData(System.String,Mix.Cms.Lib.MixEnums/MixAttributeSetDataType,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Exception in: System.Void GetAdditionalData(System.String,Mix.Cms.Lib.MixEnums/MixAttributeSetDataType,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Specified method is not supported.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com


		public async Task<RepositoryResponse<bool>> ImportAsync(string destCulture, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0.u003cu003e4__this = this;
			V_0.destCulture = destCulture;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<SiteStructureViewModel.u003cImportAsyncu003ed__57>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private async Task<RepositoryResponse<bool>> ImportAttributeSetDatas(string destCulture, MixCmsContext context, IDbContextTransaction transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.destCulture = destCulture;
			V_0.context = context;
			V_0.transaction = transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<SiteStructureViewModel.u003cImportAttributeSetDatasu003ed__61>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private async Task<RepositoryResponse<bool>> ImportAttributeSetsAsync(MixCmsContext context, IDbContextTransaction transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.context = context;
			V_0.transaction = transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<SiteStructureViewModel.u003cImportAttributeSetsAsyncu003ed__59>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private async Task<RepositoryResponse<bool>> ImportModulesAsync(string destCulture, MixCmsContext context, IDbContextTransaction transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.destCulture = destCulture;
			V_0.context = context;
			V_0.transaction = transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<SiteStructureViewModel.u003cImportModulesAsyncu003ed__58>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private async Task<RepositoryResponse<bool>> ImportPagesAsync(string destCulture, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0.u003cu003e4__this = this;
			V_0.destCulture = destCulture;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<SiteStructureViewModel.u003cImportPagesAsyncu003ed__60>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private async Task<RepositoryResponse<bool>> ImportRelatedDatas(string desCulture, MixCmsContext context, IDbContextTransaction transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.desCulture = desCulture;
			V_0.context = context;
			V_0.transaction = transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<SiteStructureViewModel.u003cImportRelatedDatasu003ed__62>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task InitAsync(string culture)
		{
			V_0.u003cu003e4__this = this;
			V_0.culture = culture;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<SiteStructureViewModel.u003cInitAsyncu003ed__41>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private void ProcessAttributeDatas(MixCmsContext context, IDbContextTransaction transaction)
		{
			return;
		}

		private void ProcessAttributeSetData(MixCmsContext context, IDbContextTransaction transaction)
		{
			this.set_AttributeSetDatas(new List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel>());
			V_0 = this.get_AttributeSets().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = new SiteStructureViewModel.u003cu003ec__DisplayClass52_0();
					V_1.u003cu003e4__this = this;
					V_1.item = V_0.get_Current();
					if (!V_1.item.get_IsExportData())
					{
						continue;
					}
					stackVariable16 = ViewModelBase<MixCmsContext, MixAttributeSetData, Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel>.Repository;
					V_3 = Expression.Parameter(Type.GetTypeFromHandle(// 
					// Current member / type: System.Void Mix.Cms.Lib.ViewModels.SiteStructureViewModel::ProcessAttributeSetData(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
					// Exception in: System.Void ProcessAttributeSetData(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
					// Specified method is not supported.
					// 
					// mailto: JustDecompilePublicFeedback@telerik.com


		private void ProcessAttributeSetsAsync(MixCmsContext context, IDbContextTransaction transaction)
		{
			V_0 = this.get_AttributeSets().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = new SiteStructureViewModel.u003cu003ec__DisplayClass47_0();
					V_1.item = V_0.get_Current();
					stackVariable10 = V_1.item;
					stackVariable11 = ViewModelBase<MixCmsContext, MixAttributeField, Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel>.Repository;
					V_2 = Expression.Parameter(Type.GetTypeFromHandle(// 
					// Current member / type: System.Void Mix.Cms.Lib.ViewModels.SiteStructureViewModel::ProcessAttributeSetsAsync(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
					// Exception in: System.Void ProcessAttributeSetsAsync(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
					// Specified method is not supported.
					// 
					// mailto: JustDecompilePublicFeedback@telerik.com


		private void ProcessDatas(MixCmsContext context, IDbContextTransaction transaction)
		{
			this.ProcessPosts(context, transaction);
			this.ProcessAttributeDatas(context, transaction);
			this.ProcessModuleDatas(context, transaction);
			return;
		}

		private void ProcessModuleData(Mix.Cms.Lib.ViewModels.MixModules.ImportViewModel item, MixCmsContext context, IDbContextTransaction transaction)
		{
			V_0 = new SiteStructureViewModel.u003cu003ec__DisplayClass50_0();
			V_0.item = item;
			stackVariable3 = ViewModelBase<MixCmsContext, MixModuleData, Mix.Cms.Lib.ViewModels.MixModuleDatas.ReadViewModel>.Repository;
			V_2 = Expression.Parameter(Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Mix.Cms.Lib.ViewModels.SiteStructureViewModel::ProcessModuleData(Mix.Cms.Lib.ViewModels.MixModules.ImportViewModel,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: System.Void ProcessModuleData(Mix.Cms.Lib.ViewModels.MixModules.ImportViewModel,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		private void ProcessModuleDatas(MixCmsContext context, IDbContextTransaction transaction)
		{
			return;
		}

		private void ProcessModules(MixCmsContext context, IDbContextTransaction transaction)
		{
			V_0 = this.get_Modules().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (!V_1.get_IsExportData())
					{
						continue;
					}
					this.ProcessModuleData(V_1, context, transaction);
				}
			}
			finally
			{
				V_0.Dispose();
			}
			return;
		}

		private void ProcessPages(MixCmsContext context, IDbContextTransaction transaction)
		{
			V_0 = this.get_Pages().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (!V_1.get_IsExportData())
					{
						continue;
					}
					V_1.set_ModuleNavs(V_1.GetModuleNavs(context, transaction));
					V_2 = V_1.get_ModuleNavs().GetEnumerator();
					try
					{
						while (V_2.MoveNext())
						{
							V_3 = new SiteStructureViewModel.u003cu003ec__DisplayClass49_0();
							V_3.u003cu003e4__this = this;
							V_3.nav = V_2.get_Current();
							V_4 = Enumerable.FirstOrDefault<Mix.Cms.Lib.ViewModels.MixModules.ImportViewModel>(this.get_Modules(), new Func<Mix.Cms.Lib.ViewModels.MixModules.ImportViewModel, bool>(V_3, SiteStructureViewModel.u003cu003ec__DisplayClass49_0.u003cProcessPagesu003eb__0));
							if (V_4 == null)
							{
								V_3.nav.get_Module().set_IsExportData(true);
							}
							else
							{
								dummyVar0 = this.get_Modules().Remove(V_4);
							}
							this.ProcessModuleData(V_3.nav.get_Module(), context, transaction);
						}
					}
					finally
					{
						V_2.Dispose();
					}
					V_1.set_UrlAliases(V_1.GetAliases(context, transaction));
					V_5 = V_1.get_Id();
					this.GetAdditionalData(V_5.ToString(), 3, context, transaction);
				}
			}
			finally
			{
				V_0.Dispose();
			}
			return;
		}

		private void ProcessPosts(MixCmsContext context, IDbContextTransaction transaction)
		{
			return;
		}

		public RepositoryResponse<string> ProcessSelectedExportDataAsync()
		{
			UnitOfWorkHelper<MixCmsContext>.InitTransaction(null, null, ref V_0, ref V_1, ref V_2);
			stackVariable5 = new RepositoryResponse<string>();
			stackVariable5.set_IsSucceed(true);
			V_3 = stackVariable5;
			try
			{
				try
				{
					this.ProcessPages(V_0, V_1);
					this.ProcessModules(V_0, V_1);
					this.ProcessAttributeSetsAsync(V_0, V_1);
					this.ProcessAttributeSetData(V_0, V_1);
					this.ProcessDatas(V_0, V_1);
					V_4 = V_3;
				}
				catch (Exception exception_0)
				{
					V_5 = exception_0;
					V_6 = UnitOfWorkHelper<MixCmsContext>.HandleException<Mix.Cms.Lib.ViewModels.MixPages.ImportViewModel>(V_5, V_2, V_1);
					V_3.set_IsSucceed(false);
					V_3.set_Errors(V_6.get_Errors());
					V_3.set_Exception(V_5);
					V_4 = V_3;
				}
			}
			finally
			{
				if (V_2)
				{
					RelationalDatabaseFacadeExtensions.CloseConnection(V_0.get_Database());
					V_1.Dispose();
					V_0.Dispose();
				}
			}
			return V_4;
		}
	}
}