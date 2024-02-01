using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Primitives;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.Services;
using Mix.Cms.Lib.ViewModels.MixAttributeFields;
using Mix.Cms.Lib.ViewModels.MixAttributeSets;
using Mix.Cms.Lib.ViewModels.MixAttributeSetValues;
using Mix.Common.Helper;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Mix.Heart.Enums;
using Mix.Heart.Helpers;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixAttributeSetDatas
{
	public class Helper
	{
		public Helper()
		{
		}

		public static RepositoryResponse<string> ExportAttributeToExcel(List<JObject> lstData, string sheetName, string folderPath, string fileName, List<string> headers = null)
		{
			JToken jToken = null;
			RepositoryResponse<string> repositoryResponse;
			RepositoryResponse<string> repositoryResponse1 = new RepositoryResponse<string>();
			try
			{
				if (lstData.Count <= 0)
				{
					repositoryResponse1.get_Errors().Add("Can not export data of empty list");
					repositoryResponse = repositoryResponse1;
				}
				else
				{
					DateTime now = DateTime.Now;
					string str = string.Concat(fileName, "-", now.ToString("yyyyMMdd"), ".xlsx");
					DataTable dataTable = new DataTable();
					if (headers != null)
					{
						foreach (string header in headers)
						{
							dataTable.Columns.Add(header, typeof(string));
						}
					}
					else
					{
						foreach (JProperty jProperty in lstData[0].Properties())
						{
							dataTable.Columns.Add(jProperty.get_Name(), typeof(string));
						}
					}
					foreach (JObject lstDatum in lstData)
					{
						DataRow dataRow = dataTable.NewRow();
						foreach (JProperty str1 in lstDatum.Properties())
						{
							if (!lstDatum.TryGetValue(str1.get_Name(), ref jToken))
							{
								continue;
							}
							dataRow[str1.get_Name()] = jToken.ToString();
						}
						dataTable.Rows.Add(dataRow);
					}
					using (ExcelPackage excelPackage = new ExcelPackage())
					{
						ExcelWorksheet excelWorksheet = excelPackage.get_Workbook().get_Worksheets().Add((sheetName != string.Empty ? sheetName : "Report"));
						excelWorksheet.get_Cells().get_Item("A1").LoadFromDataTable(dataTable, true, 0);
						excelWorksheet.get_Cells().get_Item(excelWorksheet.get_Dimension().get_Address()).AutoFitColumns();
						CommonHelper.SaveFileBytes(folderPath, str, excelPackage.GetAsByteArray());
						repositoryResponse1.set_IsSucceed(true);
						repositoryResponse1.set_Data(string.Concat(new string[] { MixService.GetConfig<string>("Domain"), "/", folderPath, "/", str }));
						repositoryResponse = repositoryResponse1;
					}
				}
			}
			catch (Exception exception)
			{
				repositoryResponse1.get_Errors().Add(exception.Message);
				repositoryResponse = repositoryResponse1;
			}
			return repositoryResponse;
		}

		public static async Task<RepositoryResponse<PaginationModel<TView>>> FilterByKeywordAsync<TView>(string culture, string attributeSetName, RequestPaging request, string keyword, Dictionary<string, StringValues> queryDictionary = null, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		where TView : ViewModelBase<MixCmsContext, MixAttributeSetData, TView>
		{
			Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cFilterByKeywordAsyncu003ed__3<TView> variable = new Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cFilterByKeywordAsyncu003ed__3<TView>();
			variable.culture = culture;
			variable.attributeSetName = attributeSetName;
			variable.request = request;
			variable.keyword = keyword;
			variable.queryDictionary = queryDictionary;
			variable._context = _context;
			variable._transaction = _transaction;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<PaginationModel<TView>>>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cFilterByKeywordAsyncu003ed__3<TView>>(ref variable);
			return variable.u003cu003et__builder.Task;
		}

		public static async Task<RepositoryResponse<PaginationModel<TView>>> FilterByKeywordAsync<TView>(string culture, HttpRequest request, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		where TView : ViewModelBase<MixCmsContext, MixAttributeSetData, TView>
		{
			Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cFilterByKeywordAsyncu003ed__4<TView> variable = new Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cFilterByKeywordAsyncu003ed__4<TView>();
			variable.culture = culture;
			variable.request = request;
			variable._context = _context;
			variable._transaction = _transaction;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<PaginationModel<TView>>>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cFilterByKeywordAsyncu003ed__4<TView>>(ref variable);
			return variable.u003cu003et__builder.Task;
		}

		public static async Task<RepositoryResponse<List<TView>>> FilterByKeywordAsync<TView>(string culture, string attributeSetName, string filterType, string fieldName, string keyword, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		where TView : ViewModelBase<MixCmsContext, MixAttributeSetData, TView>
		{
			Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cFilterByKeywordAsyncu003ed__5<TView> variable = new Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cFilterByKeywordAsyncu003ed__5<TView>();
			variable.culture = culture;
			variable.attributeSetName = attributeSetName;
			variable.filterType = filterType;
			variable.fieldName = fieldName;
			variable.keyword = keyword;
			variable._context = _context;
			variable._transaction = _transaction;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<List<TView>>>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cFilterByKeywordAsyncu003ed__5<TView>>(ref variable);
			return variable.u003cu003et__builder.Task;
		}

		public static Task<RepositoryResponse<List<TView>>> FilterByValueAsync<TView>(string culture, string attributeSetName, Dictionary<string, StringValues> queryDictionary, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		where TView : ODataViewModelBase<MixCmsContext, MixAttributeSetData, TView>
		{
			MixCmsContext mixCmsContext = null;
			IDbContextTransaction dbContextTransaction = null;
			bool flag = false;
			Task<RepositoryResponse<List<TView>>> task;
			UnitOfWorkHelper<MixCmsContext>.InitTransaction(_context, _transaction, ref mixCmsContext, ref dbContextTransaction, ref flag);
			try
			{
				try
				{
					Expression<Func<MixAttributeSetValue, bool>> specificulture = (MixAttributeSetValue m) => m.Specificulture == culture && m.AttributeSetName == attributeSetName;
					RepositoryResponse<List<TView>> repositoryResponse = new RepositoryResponse<List<TView>>();
					repositoryResponse.set_IsSucceed(true);
					repositoryResponse.set_Data(new List<TView>());
					RepositoryResponse<List<TView>> repositoryResponse1 = repositoryResponse;
					List<Task<RepositoryResponse<TView>>> tasks = new List<Task<RepositoryResponse<TView>>>();
					foreach (KeyValuePair<string, StringValues> keyValuePair in queryDictionary)
					{
						if (string.IsNullOrEmpty(keyValuePair.Key) || string.IsNullOrEmpty(keyValuePair.Value))
						{
							continue;
						}
						Expression<Func<MixAttributeSetValue, bool>> attributeFieldName = (MixAttributeSetValue m) => m.AttributeFieldName == keyValuePair.Key && m.StringValue.Contains((string)keyValuePair.Value);
						specificulture = ReflectionHelper.CombineExpression<MixAttributeSetValue>(specificulture, attributeFieldName, 6, "model");
					}
					List<string> list = mixCmsContext.MixAttributeSetValue.Where<MixAttributeSetValue>(specificulture).Select<MixAttributeSetValue, string>((MixAttributeSetValue m) => m.DataId).Distinct<string>().ToList<string>();
					if (list != null)
					{
						foreach (string str in list)
						{
							tasks.Add(Task.Run<RepositoryResponse<TView>>(async () => await ODataDefaultRepository<MixCmsContext, MixAttributeSetData, Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>.get_Instance().GetSingleModelAsync((MixAttributeSetData m) => m.Id == str && m.Specificulture == culture, null, null)));
						}
						Task<RepositoryResponse<TView>[]> task1 = Task.WhenAll<RepositoryResponse<TView>>(tasks);
						task1.Wait();
						if (task1.Status != TaskStatus.RanToCompletion)
						{
							foreach (Task<RepositoryResponse<TView>> task2 in tasks)
							{
								repositoryResponse1.get_Errors().Add(string.Format("Task {0}: {1}", task2.Id, task2.Status));
							}
						}
						else
						{
							RepositoryResponse<TView>[] result = task1.Result;
							for (int i = 0; i < (int)result.Length; i++)
							{
								RepositoryResponse<TView> repositoryResponse2 = result[i];
								if (!repositoryResponse2.get_IsSucceed())
								{
									repositoryResponse1.get_Errors().AddRange(repositoryResponse2.get_Errors());
								}
								else
								{
									repositoryResponse1.get_Data().Add(repositoryResponse2.get_Data());
								}
							}
						}
					}
					task = Task.FromResult<RepositoryResponse<List<TView>>>(repositoryResponse1);
				}
				catch (Exception exception)
				{
					task = Task.FromResult<RepositoryResponse<List<TView>>>(UnitOfWorkHelper<MixCmsContext>.HandleException<List<TView>>(exception, flag, dbContextTransaction));
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
			return task;
		}

		public static async Task<RepositoryResponse<bool>> ImportData(string culture, Mix.Cms.Lib.ViewModels.MixAttributeSets.ReadViewModel attributeSet, IFormFile file)
		{
			Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cImportDatau003ed__0 variable = new Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cImportDatau003ed__0();
			variable.culture = culture;
			variable.attributeSet = attributeSet;
			variable.file = file;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cImportDatau003ed__0>(ref variable);
			return variable.u003cu003et__builder.Task;
		}

		private static List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel> LoadFileData(string culture, Mix.Cms.Lib.ViewModels.MixAttributeSets.ReadViewModel attributeSet, IFormFile file)
		{
			List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel> importViewModels;
			List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel> importViewModels1 = new List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel>();
			using (Stream stream = file.OpenReadStream())
			{
				using (ExcelPackage excelPackage = new ExcelPackage(stream))
				{
					foreach (ExcelWorksheet worksheet in excelPackage.get_Workbook().get_Worksheets())
					{
						for (int i = 2; i <= worksheet.get_Dimension().get_End().get_Row(); i++)
						{
							JObject jObject = new JObject();
							for (int j = worksheet.get_Dimension().get_Start().get_Column(); j <= worksheet.get_Dimension().get_End().get_Column(); j++)
							{
								jObject.Add(new JProperty(worksheet.get_Cells().get_Item(1, j).get_Value().ToString(), worksheet.get_Cells().get_Item(i, j).get_Value()));
							}
							importViewModels1.Add(new Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel()
							{
								AttributeSetId = attributeSet.Id,
								AttributeSetName = attributeSet.Name,
								Specificulture = culture,
								Data = jObject
							});
						}
					}
					importViewModels = importViewModels1;
				}
			}
			return importViewModels;
		}
	}
}