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
			MixCmsContext mixCmsContext = null;
			IDbContextTransaction dbContextTransaction = null;
			bool flag = false;
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			UnitOfWorkHelper<MixCmsContext>.InitTransaction(_context, _transaction, ref mixCmsContext, ref dbContextTransaction, ref flag);
			try
			{
				try
				{
					if (this.Pages != null)
					{
						repositoryResponse1 = await this.ImportPagesAsync(destCulture, mixCmsContext, dbContextTransaction);
					}
					if (repositoryResponse1.get_IsSucceed() && this.Modules != null)
					{
						repositoryResponse1 = await this.ImportModulesAsync(destCulture, mixCmsContext, dbContextTransaction);
					}
					if (repositoryResponse1.get_IsSucceed() && this.AttributeSets != null)
					{
						repositoryResponse1 = await this.ImportAttributeSetsAsync(mixCmsContext, dbContextTransaction);
					}
					if (repositoryResponse1.get_IsSucceed() && this.AttributeSetDatas.Count > 0)
					{
						repositoryResponse1 = await this.ImportAttributeSetDatas(destCulture, mixCmsContext, dbContextTransaction);
					}
					if (repositoryResponse1.get_IsSucceed() && this.RelatedData.Count > 0)
					{
						repositoryResponse1 = await this.ImportRelatedDatas(destCulture, mixCmsContext, dbContextTransaction);
					}
					UnitOfWorkHelper<MixCmsContext>.HandleTransaction(repositoryResponse1.get_IsSucceed(), flag, dbContextTransaction);
				}
				catch (Exception exception)
				{
					RepositoryResponse<Mix.Cms.Lib.ViewModels.MixPages.ImportViewModel> repositoryResponse2 = UnitOfWorkHelper<MixCmsContext>.HandleException<Mix.Cms.Lib.ViewModels.MixPages.ImportViewModel>(exception, flag, dbContextTransaction);
					repositoryResponse1.set_IsSucceed(false);
					repositoryResponse1.set_Errors(repositoryResponse2.get_Errors());
					repositoryResponse1.set_Exception(repositoryResponse2.get_Exception());
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
			return repositoryResponse1;
		}

		private async Task<RepositoryResponse<bool>> ImportAttributeSetDatas(string destCulture, MixCmsContext context, IDbContextTransaction transaction)
		{
			Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel updateViewModel;
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			foreach (Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel attributeSetData in this.AttributeSetDatas)
			{
				if (!repositoryResponse1.get_IsSucceed())
				{
					break;
				}
				DbSet<MixAttributeSetData> mixAttributeSetData = context.MixAttributeSetData;
				if (mixAttributeSetData.Any<MixAttributeSetData>((MixAttributeSetData m) => m.Id == attributeSetData.Id && m.Specificulture == attributeSetData.Specificulture))
				{
					continue;
				}
				attributeSetData.Specificulture = destCulture;
				if (attributeSetData.AttributeSetName.IndexOf("sys_") != 0 && this.dicAttributeSetIds.ContainsKey(attributeSetData.AttributeSetId))
				{
					attributeSetData.AttributeSetId = this.dicAttributeSetIds[attributeSetData.AttributeSetId];
				}
				Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel importViewModel = attributeSetData;
				List<Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel> fields = attributeSetData.Fields;
				if (fields == null)
				{
					DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixAttributeField, Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel>.Repository;
					fields = repository.GetModelListBy((MixAttributeField m) => m.AttributeSetId == attributeSetData.AttributeSetId, context, transaction).get_Data();
				}
				importViewModel.Fields = fields;
				foreach (Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel field in attributeSetData.Fields)
				{
					field.Specificulture = destCulture;
					Mix.Cms.Lib.ViewModels.MixAttributeSets.ImportViewModel importViewModel1 = this.AttributeSets.FirstOrDefault<Mix.Cms.Lib.ViewModels.MixAttributeSets.ImportViewModel>((Mix.Cms.Lib.ViewModels.MixAttributeSets.ImportViewModel m) => m.Name == field.AttributeSetName);
					if (importViewModel1 != null)
					{
						updateViewModel = importViewModel1.Fields.FirstOrDefault<Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel>((Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel m) => m.Name == field.Name);
					}
					else
					{
						updateViewModel = null;
					}
					Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel updateViewModel1 = updateViewModel;
					if (updateViewModel1 == null)
					{
						continue;
					}
					field.Id = updateViewModel1.Id;
					field.AttributeSetId = importViewModel1.Id;
					field.AttributeSetName = importViewModel1.Name;
				}
				ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel>(await attributeSetData.SaveModelAsync(true, context, transaction), ref repositoryResponse1);
			}
			return repositoryResponse1;
		}

		private async Task<RepositoryResponse<bool>> ImportAttributeSetsAsync(MixCmsContext context, IDbContextTransaction transaction)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			if (this.AttributeSets != null)
			{
				DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixAttributeSet, Mix.Cms.Lib.ViewModels.MixAttributeSets.ImportViewModel>.Repository;
				int data = repository.Max((MixAttributeSet m) => m.Id, null, null).get_Data();
				DefaultRepository<!0, !1, !2> defaultRepository = ViewModelBase<MixCmsContext, MixAttributeField, Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel>.Repository;
				int num = defaultRepository.Max((MixAttributeField m) => m.Id, null, null).get_Data();
				foreach (Mix.Cms.Lib.ViewModels.MixAttributeSets.ImportViewModel attributeSet in this.AttributeSets)
				{
					if (!repositoryResponse1.get_IsSucceed())
					{
						break;
					}
					data++;
					this.dicAttributeSetIds.Add(attributeSet.Id, data);
					DbSet<MixAttributeSet> mixAttributeSet = context.MixAttributeSet;
					if (mixAttributeSet.Any<MixAttributeSet>((MixAttributeSet m) => m.Name == attributeSet.Name))
					{
						continue;
					}
					attributeSet.Id = data;
					attributeSet.CreatedDateTime = DateTime.UtcNow;
					foreach (Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel field in attributeSet.Fields)
					{
						num++;
						this.dicFieldIds.Add(field.Id, num);
						field.Id = num;
						field.CreatedDateTime = DateTime.UtcNow;
					}
					ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixAttributeSets.ImportViewModel>(await attributeSet.SaveModelAsync(true, context, transaction), ref repositoryResponse1);
				}
			}
			return repositoryResponse1;
		}

		private async Task<RepositoryResponse<bool>> ImportModulesAsync(string destCulture, MixCmsContext context, IDbContextTransaction transaction)
		{
			SiteStructureViewModel.u003cu003ec__DisplayClass58_0 variable = null;
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			foreach (Mix.Cms.Lib.ViewModels.MixModules.ImportViewModel module in this.Modules)
			{
				int id = module.Id;
				DbSet<MixModule> mixModule = context.MixModule;
				int num = mixModule.Max<MixModule, int>((MixModule m) => m.Id);
				if (!repositoryResponse1.get_IsSucceed())
				{
					break;
				}
				DbSet<MixModule> dbSet = context.MixModule;
				if (!dbSet.Any<MixModule>((MixModule m) => m.Name == module.Name && m.Specificulture == variable.destCulture))
				{
					num++;
					module.Id = num;
					module.Specificulture = destCulture;
					if (!string.IsNullOrEmpty(module.Image))
					{
						module.Image = module.Image.Replace(string.Concat("content/templates/", this.ThemeName), string.Concat("content/templates/", MixService.GetConfig<string>("ThemeFolder", destCulture)));
					}
					module.CreatedDateTime = DateTime.UtcNow;
					ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixModules.ImportViewModel>(await module.SaveModelAsync(true, context, transaction), ref repositoryResponse1);
				}
				this.dicModuleIds.Add(id, module.Id);
			}
			return repositoryResponse1;
		}

		private async Task<RepositoryResponse<bool>> ImportPagesAsync(string destCulture, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			MixCmsContext mixCmsContext = null;
			IDbContextTransaction dbContextTransaction = null;
			bool flag = false;
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			UnitOfWorkHelper<MixCmsContext>.InitTransaction(_context, _transaction, ref mixCmsContext, ref dbContextTransaction, ref flag);
			try
			{
				try
				{
					DefaultModelRepository<!0, !1> modelRepository = ViewModelBase<MixCmsContext, MixPage, Mix.Cms.Lib.ViewModels.MixPages.UpdateViewModel>.ModelRepository;
					int data = modelRepository.Max((MixPage m) => m.Id, mixCmsContext, dbContextTransaction).get_Data();
					DefaultModelRepository<!0, !1> defaultModelRepository = ViewModelBase<MixCmsContext, MixModule, Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel>.ModelRepository;
					int num = defaultModelRepository.Max((MixModule m) => m.Id, mixCmsContext, dbContextTransaction).get_Data();
					foreach (Mix.Cms.Lib.ViewModels.MixPages.ImportViewModel page in this.Pages)
					{
						DbSet<MixPage> mixPage = mixCmsContext.MixPage;
						if (mixPage.Any<MixPage>((MixPage p) => p.SeoName == page.SeoName))
						{
							continue;
						}
						int id = page.Id;
						data++;
						this.dicPageIds.Add(id, data);
						page.Id = data;
						page.CreatedDateTime = DateTime.UtcNow;
						page.ThemeName = this.ThemeName;
						if (page.ModuleNavs != null)
						{
							foreach (Mix.Cms.Lib.ViewModels.MixPageModules.ImportViewModel moduleNav in page.ModuleNavs)
							{
								num++;
								this.dicModuleIds.Add(moduleNav.Module.Id, num);
								moduleNav.Module.Id = num;
								moduleNav.PageId = data;
								moduleNav.ModuleId = num;
							}
						}
						if (!string.IsNullOrEmpty(page.Image))
						{
							page.Image = page.Image.Replace(string.Concat("content/templates/", this.ThemeName), string.Concat("content/templates/", MixService.GetConfig<string>("ThemeFolder", destCulture)));
						}
						if (!string.IsNullOrEmpty(page.Thumbnail))
						{
							page.Thumbnail = page.Thumbnail.Replace(string.Concat("content/templates/", this.ThemeName), string.Concat("content/templates/", MixService.GetConfig<string>("ThemeFolder", destCulture)));
						}
						page.Specificulture = destCulture;
						RepositoryResponse<Mix.Cms.Lib.ViewModels.MixPages.ImportViewModel> repositoryResponse2 = await page.SaveModelAsync(true, mixCmsContext, dbContextTransaction);
						if (repositoryResponse2.get_IsSucceed())
						{
							continue;
						}
						repositoryResponse1.set_IsSucceed(false);
						repositoryResponse1.set_Exception(repositoryResponse2.get_Exception());
						repositoryResponse1.set_Errors(repositoryResponse2.get_Errors());
						break;
					}
					UnitOfWorkHelper<MixCmsContext>.HandleTransaction(repositoryResponse1.get_IsSucceed(), flag, dbContextTransaction);
				}
				catch (Exception exception)
				{
					RepositoryResponse<Mix.Cms.Lib.ViewModels.MixPages.ImportViewModel> repositoryResponse3 = UnitOfWorkHelper<MixCmsContext>.HandleException<Mix.Cms.Lib.ViewModels.MixPages.ImportViewModel>(exception, flag, dbContextTransaction);
					repositoryResponse1.set_IsSucceed(false);
					repositoryResponse1.set_Errors(repositoryResponse3.get_Errors());
					repositoryResponse1.set_Exception(repositoryResponse3.get_Exception());
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
			return repositoryResponse1;
		}

		private async Task<RepositoryResponse<bool>> ImportRelatedDatas(string desCulture, MixCmsContext context, IDbContextTransaction transaction)
		{
			int num;
			int num1;
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.ImportViewModel>.Enumerator enumerator = this.RelatedData.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.ImportViewModel current = enumerator.Current;
					current.Id = Guid.NewGuid().ToString();
					current.Specificulture = desCulture;
					switch (current.ParentType)
					{
						case MixEnums.MixAttributeSetDataType.System:
						case MixEnums.MixAttributeSetDataType.Post:
						case MixEnums.MixAttributeSetDataType.Service:
						{
							if (!repositoryResponse1.get_IsSucceed())
							{
								goto Label0;
							}
							ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.ImportViewModel>(await current.SaveModelAsync(false, context, transaction), ref repositoryResponse1);
							continue;
						}
						case MixEnums.MixAttributeSetDataType.Set:
						{
							current.AttributeSetId = this.dicAttributeSetIds[int.Parse(current.ParentId)];
							goto case MixEnums.MixAttributeSetDataType.Service;
						}
						case MixEnums.MixAttributeSetDataType.Page:
						{
							if (!this.dicPageIds.TryGetValue(int.Parse(current.ParentId), out num))
							{
								continue;
							}
							current.ParentId = num.ToString();
							goto case MixEnums.MixAttributeSetDataType.Service;
						}
						case MixEnums.MixAttributeSetDataType.Module:
						{
							if (!this.dicModuleIds.TryGetValue(int.Parse(current.ParentId), out num1))
							{
								continue;
							}
							current.ParentId = num1.ToString();
							goto case MixEnums.MixAttributeSetDataType.Service;
						}
						default:
						{
							goto case MixEnums.MixAttributeSetDataType.Service;
						}
					}
				}
			Label0:
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			enumerator = new List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.ImportViewModel>.Enumerator();
			return repositoryResponse1;
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