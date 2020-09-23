using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Primitives;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixAttributeFields;
using Mix.Cms.Lib.ViewModels.MixAttributeSets;
using Mix.Cms.Lib.ViewModels.MixAttributeSetValues;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.ViewModels;
using Mix.Heart.Enums;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixAttributeSetDatas
{
	public class Helper
	{
		public Helper()
		{
			base();
			return;
		}

		public static RepositoryResponse<string> ExportAttributeToExcel(List<JObject> lstData, string sheetName, string folderPath, string fileName, List<string> headers = null)
		{
			V_0 = new RepositoryResponse<string>();
			try
			{
				if (lstData.get_Count() <= 0)
				{
					V_0.get_Errors().Add("Can not export data of empty list");
					V_16 = V_0;
				}
				else
				{
					V_3 = DateTime.get_Now();
					V_1 = string.Concat(fileName, "-", V_3.ToString("yyyyMMdd"), ".xlsx");
					V_2 = new DataTable();
					if (headers != null)
					{
						V_6 = headers.GetEnumerator();
						try
						{
							while (V_6.MoveNext())
							{
								V_7 = V_6.get_Current();
								dummyVar1 = V_2.get_Columns().Add(V_7, Type.GetTypeFromHandle(// 
								// Current member / type: Mix.Domain.Core.ViewModels.RepositoryResponse`1<System.String> Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper::ExportAttributeToExcel(System.Collections.Generic.List`1<Newtonsoft.Json.Linq.JObject>,System.String,System.String,System.String,System.Collections.Generic.List`1<System.String>)
								// Exception in: Mix.Domain.Core.ViewModels.RepositoryResponse<System.String> ExportAttributeToExcel(System.Collections.Generic.List<Newtonsoft.Json.Linq.JObject>,System.String,System.String,System.String,System.Collections.Generic.List<System.String>)
								// Specified method is not supported.
								// 
								// mailto: JustDecompilePublicFeedback@telerik.com


		public static async Task<RepositoryResponse<PaginationModel<TView>>> FilterByKeywordAsync<TView>(string culture, string attributeSetName, RequestPaging request, string keyword, Dictionary<string, StringValues> queryDictionary = null, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		where TView : ViewModelBase<MixCmsContext, MixAttributeSetData, TView>
		{
			V_0.culture = culture;
			V_0.attributeSetName = attributeSetName;
			V_0.request = request;
			V_0.keyword = keyword;
			V_0.queryDictionary = queryDictionary;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<PaginationModel<TView>>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cFilterByKeywordAsyncu003ed__3<TView>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public static async Task<RepositoryResponse<PaginationModel<TView>>> FilterByKeywordAsync<TView>(string culture, HttpRequest request, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		where TView : ViewModelBase<MixCmsContext, MixAttributeSetData, TView>
		{
			V_0.culture = culture;
			V_0.request = request;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<PaginationModel<TView>>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cFilterByKeywordAsyncu003ed__4<TView>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public static async Task<RepositoryResponse<List<TView>>> FilterByKeywordAsync<TView>(string culture, string attributeSetName, string filterType, string fieldName, string keyword, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		where TView : ViewModelBase<MixCmsContext, MixAttributeSetData, TView>
		{
			V_0.culture = culture;
			V_0.attributeSetName = attributeSetName;
			V_0.filterType = filterType;
			V_0.fieldName = fieldName;
			V_0.keyword = keyword;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<List<TView>>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cFilterByKeywordAsyncu003ed__5<TView>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public static Task<RepositoryResponse<List<TView>>> FilterByValueAsync<TView>(string culture, string attributeSetName, Dictionary<string, StringValues> queryDictionary, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		where TView : ODataViewModelBase<MixCmsContext, MixAttributeSetData, TView>
		{
			V_0 = new Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass2_0<TView>();
			V_0.culture = culture;
			V_0.attributeSetName = attributeSetName;
			UnitOfWorkHelper<MixCmsContext>.InitTransaction(_context, _transaction, ref V_1, ref V_2, ref V_3);
			try
			{
				try
				{
					V_8 = Expression.Parameter(Type.GetTypeFromHandle(// 
					// Current member / type: System.Threading.Tasks.Task`1<Mix.Domain.Core.ViewModels.RepositoryResponse`1<System.Collections.Generic.List`1<TView>>> Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper::FilterByValueAsync(System.String,System.String,System.Collections.Generic.Dictionary`2<System.String,Microsoft.Extensions.Primitives.StringValues>,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
					// Exception in: System.Threading.Tasks.Task<Mix.Domain.Core.ViewModels.RepositoryResponse<System.Collections.Generic.List<TView>>> FilterByValueAsync(System.String,System.String,System.Collections.Generic.Dictionary<System.String,Microsoft.Extensions.Primitives.StringValues>,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
					// Specified method is not supported.
					// 
					// mailto: JustDecompilePublicFeedback@telerik.com


		public static async Task<RepositoryResponse<bool>> ImportData(string culture, Mix.Cms.Lib.ViewModels.MixAttributeSets.ReadViewModel attributeSet, IFormFile file)
		{
			V_0.culture = culture;
			V_0.attributeSet = attributeSet;
			V_0.file = file;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cImportDatau003ed__0>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private static List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel> LoadFileData(string culture, Mix.Cms.Lib.ViewModels.MixAttributeSets.ReadViewModel attributeSet, IFormFile file)
		{
			V_0 = new List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel>();
			V_1 = file.OpenReadStream();
			try
			{
				V_2 = new ExcelPackage(V_1);
				try
				{
					V_3 = V_2.get_Workbook().get_Worksheets().GetEnumerator();
					try
					{
						while (V_3.MoveNext())
						{
							V_4 = V_3.get_Current();
							V_5 = 2;
							while (V_5 <= V_4.get_Dimension().get_End().get_Row())
							{
								V_6 = new JObject();
								V_8 = V_4.get_Dimension().get_Start().get_Column();
								while (V_8 <= V_4.get_Dimension().get_End().get_Column())
								{
									V_6.Add(new JProperty(V_4.get_Cells().get_Item(1, V_8).get_Value().ToString(), V_4.get_Cells().get_Item(V_5, V_8).get_Value()));
									V_8 = V_8 + 1;
								}
								stackVariable47 = new Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel();
								stackVariable47.set_AttributeSetId(attributeSet.get_Id());
								stackVariable47.set_AttributeSetName(attributeSet.get_Name());
								stackVariable47.set_Specificulture(culture);
								stackVariable47.set_Data(V_6);
								V_0.Add(stackVariable47);
								V_5 = V_5 + 1;
							}
						}
					}
					finally
					{
						if (V_3 != null)
						{
							V_3.Dispose();
						}
					}
					V_9 = V_0;
				}
				finally
				{
					if (V_2 != null)
					{
						V_2.Dispose();
					}
				}
			}
			finally
			{
				if (V_1 != null)
				{
					((IDisposable)V_1).Dispose();
				}
			}
			return V_9;
		}
	}
}