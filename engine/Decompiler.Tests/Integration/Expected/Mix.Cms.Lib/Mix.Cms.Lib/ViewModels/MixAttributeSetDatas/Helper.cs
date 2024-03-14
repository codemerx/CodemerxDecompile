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
			RepositoryResponse<PaginationModel<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>> repositoryResponse;
			Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView> variable = null;
			Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_1<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView> variable1 = null;
			Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_2<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView> variable2 = null;
			MixCmsContext mixCmsContext = null;
			IDbContextTransaction dbContextTransaction = null;
			bool flag = false;
			UnitOfWorkHelper<MixCmsContext>.InitTransaction(_context, _transaction, ref mixCmsContext, ref dbContextTransaction, ref flag);
			try
			{
				try
				{
					ParameterExpression parameterExpression = Expression.Parameter(typeof(MixAttributeSetValue), "m");
					BinaryExpression binaryExpression = Expression.AndAlso(Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSetValue).GetMethod("get_Specificulture").MethodHandle)), Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("culture").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSetValue).GetMethod("get_AttributeSetName").MethodHandle)), Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("attributeSetName").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle)))), Expression.OrElse(Expression.Not(Expression.Property(Expression.Property(Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("request").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(RequestPaging).GetMethod("get_FromDate").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(DateTime?).GetMethod("get_HasValue").MethodHandle, typeof(DateTime?).TypeHandle))), Expression.GreaterThanOrEqual(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSetValue).GetMethod("get_CreatedDateTime").MethodHandle)), Expression.Property(Expression.Property(Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("request").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(RequestPaging).GetMethod("get_FromDate").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(DateTime?).GetMethod("get_Value").MethodHandle, typeof(DateTime?).TypeHandle)), false, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(DateTime).GetMethod("op_GreaterThanOrEqual", new Type[] { typeof(DateTime), typeof(DateTime) }).MethodHandle)))), Expression.OrElse(Expression.Not(Expression.Property(Expression.Property(Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("request").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(RequestPaging).GetMethod("get_ToDate").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(DateTime?).GetMethod("get_HasValue").MethodHandle, typeof(DateTime?).TypeHandle))), Expression.LessThanOrEqual(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSetValue).GetMethod("get_CreatedDateTime").MethodHandle)), Expression.Property(Expression.Property(Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("request").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(RequestPaging).GetMethod("get_ToDate").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(DateTime?).GetMethod("get_Value").MethodHandle, typeof(DateTime?).TypeHandle)), false, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(DateTime).GetMethod("op_LessThanOrEqual", new Type[] { typeof(DateTime), typeof(DateTime) }).MethodHandle))));
					ParameterExpression[] parameterExpressionArray = new ParameterExpression[] { parameterExpression };
					Expression<Func<MixAttributeSetValue, bool>> expression = Expression.Lambda<Func<MixAttributeSetValue, bool>>(binaryExpression, parameterExpressionArray);
					Expression<Func<MixAttributeSetValue, bool>> expression1 = null;
					RepositoryResponse<PaginationModel<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>> repositoryResponse1 = new RepositoryResponse<PaginationModel<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>>();
					repositoryResponse1.set_IsSucceed(true);
					repositoryResponse1.set_Data(new PaginationModel<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>());
					RepositoryResponse<PaginationModel<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>> modelListByAsync = repositoryResponse1;
					Dictionary<string, StringValues> strs = queryDictionary;
					KeyValuePair<string, StringValues> keyValuePair = strs.FirstOrDefault<KeyValuePair<string, StringValues>>((KeyValuePair<string, StringValues> q) => q.Key == "filterType");
					List<Task<RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>>> tasks = new List<Task<RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>>>();
					if (queryDictionary != null)
					{
						foreach (KeyValuePair<string, StringValues> keyValuePair1 in queryDictionary)
						{
							if (string.IsNullOrEmpty(keyValuePair1.Key) || !(keyValuePair1.Key != "attributeSetId") || !(keyValuePair1.Key != "attributeSetName") || !(keyValuePair1.Key != "filterType") || string.IsNullOrEmpty(keyValuePair1.Value))
							{
								continue;
							}
							if (string.IsNullOrEmpty(keyValuePair.Value) || !(keyValuePair.Value == "equal"))
							{
								parameterExpression = Expression.Parameter(typeof(MixAttributeSetValue), "m");
								BinaryExpression binaryExpression1 = Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSetValue).GetMethod("get_AttributeFieldName").MethodHandle)), Expression.Property(Expression.Field(Expression.Constant(variable2, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_2<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_2<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("q").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_2<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(KeyValuePair<string, StringValues>).GetMethod("get_Key").MethodHandle, typeof(KeyValuePair<string, StringValues>).TypeHandle)));
								MethodInfo methodFromHandle = (MethodInfo)MethodBase.GetMethodFromHandle(typeof(DbFunctionsExtensions).GetMethod("Like", new Type[] { typeof(DbFunctions), typeof(string), typeof(string) }).MethodHandle);
								Expression[] expressionArray = new Expression[] { Expression.Property(null, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(EF).GetMethod("get_Functions").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSetValue).GetMethod("get_StringValue").MethodHandle)), null };
								MethodInfo methodInfo = (MethodInfo)MethodBase.GetMethodFromHandle(typeof(string).GetMethod("Format", new Type[] { typeof(string), typeof(object) }).MethodHandle);
								Expression[] expressionArray1 = new Expression[] { Expression.Constant("%{0}%", typeof(string)), Expression.Call(Expression.Property(Expression.Field(Expression.Constant(variable2, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_2<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_2<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("q").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_2<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(KeyValuePair<string, StringValues>).GetMethod("get_Value").MethodHandle, typeof(KeyValuePair<string, StringValues>).TypeHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(object).GetMethod("ToString").MethodHandle), Array.Empty<Expression>()) };
								expressionArray[2] = Expression.Call(null, methodInfo, expressionArray1);
								BinaryExpression binaryExpression2 = Expression.AndAlso(binaryExpression1, Expression.Call(null, methodFromHandle, expressionArray));
								ParameterExpression[] parameterExpressionArray1 = new ParameterExpression[] { parameterExpression };
								Expression<Func<MixAttributeSetValue, bool>> expression2 = Expression.Lambda<Func<MixAttributeSetValue, bool>>(binaryExpression2, parameterExpressionArray1);
								expression1 = (expression1 == null ? expression2 : ReflectionHelper.CombineExpression<MixAttributeSetValue>(expression1, expression2, 6, "model"));
							}
							else
							{
								parameterExpression = Expression.Parameter(typeof(MixAttributeSetValue), "m");
								BinaryExpression binaryExpression3 = Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSetValue).GetMethod("get_AttributeFieldName").MethodHandle)), Expression.Property(Expression.Field(Expression.Constant(variable2, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_2<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_2<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("q").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_2<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(KeyValuePair<string, StringValues>).GetMethod("get_Key").MethodHandle, typeof(KeyValuePair<string, StringValues>).TypeHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSetValue).GetMethod("get_StringValue").MethodHandle)), Expression.Call(Expression.Property(Expression.Field(Expression.Constant(variable2, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_2<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_2<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("q").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_2<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(KeyValuePair<string, StringValues>).GetMethod("get_Value").MethodHandle, typeof(KeyValuePair<string, StringValues>).TypeHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(object).GetMethod("ToString").MethodHandle), Array.Empty<Expression>())));
								ParameterExpression[] parameterExpressionArray2 = new ParameterExpression[] { parameterExpression };
								Expression<Func<MixAttributeSetValue, bool>> expression3 = Expression.Lambda<Func<MixAttributeSetValue, bool>>(binaryExpression3, parameterExpressionArray2);
								expression1 = (expression1 == null ? expression3 : ReflectionHelper.CombineExpression<MixAttributeSetValue>(expression1, expression3, 6, "model"));
							}
						}
						if (expression1 != null)
						{
							expression = ReflectionHelper.CombineExpression<MixAttributeSetValue>(expression1, expression, 6, "model");
						}
					}
					if (!string.IsNullOrEmpty(keyword))
					{
						parameterExpression = Expression.Parameter(typeof(MixAttributeSetValue), "m");
						BinaryExpression binaryExpression4 = Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSetValue).GetMethod("get_AttributeSetName").MethodHandle)), Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("attributeSetName").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSetValue).GetMethod("get_Specificulture").MethodHandle)), Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("culture").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle))));
						MemberExpression memberExpression = Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSetValue).GetMethod("get_StringValue").MethodHandle));
						MethodInfo methodFromHandle1 = (MethodInfo)MethodBase.GetMethodFromHandle(typeof(string).GetMethod("Contains", new Type[] { typeof(string) }).MethodHandle);
						Expression[] expressionArray2 = new Expression[] { Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("keyword").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle)) };
						BinaryExpression binaryExpression5 = Expression.AndAlso(binaryExpression4, Expression.Call(memberExpression, methodFromHandle1, expressionArray2));
						ParameterExpression[] parameterExpressionArray3 = new ParameterExpression[] { parameterExpression };
						Expression<Func<MixAttributeSetValue, bool>> expression4 = Expression.Lambda<Func<MixAttributeSetValue, bool>>(binaryExpression5, parameterExpressionArray3);
						expression = ReflectionHelper.CombineExpression<MixAttributeSetValue>(expression, expression4, 6, "model");
					}
					IQueryable<MixAttributeSetValue> mixAttributeSetValues = mixCmsContext.MixAttributeSetValue.Where<MixAttributeSetValue>(expression);
					parameterExpression = Expression.Parameter(typeof(MixAttributeSetValue), "m");
					MemberExpression memberExpression1 = Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSetValue).GetMethod("get_DataId").MethodHandle));
					ParameterExpression[] parameterExpressionArray4 = new ParameterExpression[] { parameterExpression };
					IQueryable<string> strs1 = mixAttributeSetValues.Select<MixAttributeSetValue, string>(Expression.Lambda<Func<MixAttributeSetValue, string>>(memberExpression1, parameterExpressionArray4)).Distinct<string>();
					strs1.ToList<string>();
					if (strs1 != null)
					{
						parameterExpression = Expression.Parameter(typeof(MixAttributeSetData), "m");
						MethodInfo methodInfo1 = (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Enumerable).GetMethod("Any", new Type[] { typeof(IEnumerable<string>), typeof(Func<string, bool>) }).MethodHandle);
						Expression[] expressionArray3 = new Expression[] { Expression.Field(Expression.Constant(variable1, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_1<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_1<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("dataIds").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass3_1<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle)), null };
						ParameterExpression parameterExpression1 = Expression.Parameter(typeof(string), "id");
						BinaryExpression binaryExpression6 = Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSetData).GetMethod("get_Id").MethodHandle)), parameterExpression1);
						ParameterExpression[] parameterExpressionArray5 = new ParameterExpression[] { parameterExpression1 };
						expressionArray3[1] = Expression.Lambda<Func<string, bool>>(binaryExpression6, parameterExpressionArray5);
						MethodCallExpression methodCallExpression = Expression.Call(null, methodInfo1, expressionArray3);
						ParameterExpression[] parameterExpressionArray6 = new ParameterExpression[] { parameterExpression };
						Expression<Func<MixAttributeSetData, bool>> expression5 = Expression.Lambda<Func<MixAttributeSetData, bool>>(methodCallExpression, parameterExpressionArray6);
						int? nullable = null;
						int? nullable1 = nullable;
						nullable = null;
						modelListByAsync = await DefaultRepository<MixCmsContext, MixAttributeSetData, Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>.get_Instance().GetModelListByAsync(expression5, request.get_OrderBy(), request.get_Direction(), request.get_PageSize(), new int?(request.get_PageIndex()), nullable1, nullable, mixCmsContext, dbContextTransaction);
					}
					repositoryResponse = modelListByAsync;
				}
				catch (Exception exception)
				{
					repositoryResponse = UnitOfWorkHelper<MixCmsContext>.HandleException<PaginationModel<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>>(exception, flag, dbContextTransaction);
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

		public static async Task<RepositoryResponse<PaginationModel<TView>>> FilterByKeywordAsync<TView>(string culture, HttpRequest request, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		where TView : ViewModelBase<MixCmsContext, MixAttributeSetData, TView>
		{
			RepositoryResponse<PaginationModel<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>> modelListByAsync;
			Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_1<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView> variable = null;
			Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_2<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView> variable1 = null;
			Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_3<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView> variable2 = null;
			List<Mix.Cms.Lib.ViewModels.MixAttributeFields.ReadViewModel> readViewModels;
			string str;
			Expression<Func<MixAttributeSetValue, bool>> expression;
			Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView> variable3 = null;
			MixCmsContext mixCmsContext = null;
			IDbContextTransaction dbContextTransaction = null;
			bool flag = false;
			MixHeartEnums.DisplayDirection displayDirection;
			int num;
			int num1;
			int num2;
			MixEnums.MixContentStatus mixContentStatu;
			DateTime dateTime;
			DateTime dateTime1;
			UnitOfWorkHelper<MixCmsContext>.InitTransaction(_context, _transaction, ref mixCmsContext, ref dbContextTransaction, ref flag);
			try
			{
				try
				{
					request.get_Query().ToList<KeyValuePair<string, StringValues>>();
					StringValues item = request.get_Query().get_Item("attributeSetName");
					item.ToString();
					item = request.get_Query().get_Item("keyword");
					string str1 = item.ToString();
					item = request.get_Query().get_Item("filterType");
					item.ToString();
					item = request.get_Query().get_Item("orderBy");
					string str2 = item.ToString();
					int.TryParse(request.get_Query().get_Item("attributeSetId"), out num2);
					Enum.TryParse<MixHeartEnums.DisplayDirection>(request.get_Query().get_Item("direction"), out displayDirection);
					int.TryParse(request.get_Query().get_Item("pageIndex"), out num);
					int.TryParse(request.get_Query().get_Item("pageSize"), out num1);
					DateTime.TryParse(request.get_Query().get_Item("fromDate"), out dateTime);
					DateTime.TryParse(request.get_Query().get_Item("toDate"), out dateTime1);
					Enum.TryParse<MixEnums.MixContentStatus>(request.get_Query().get_Item("status"), out mixContentStatu);
					List<Task<RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>>> tasks = new List<Task<RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>>>();
					DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixAttributeField, Mix.Cms.Lib.ViewModels.MixAttributeFields.ReadViewModel>.Repository;
					ParameterExpression parameterExpression = Expression.Parameter(typeof(MixAttributeField), "m");
					BinaryExpression binaryExpression = Expression.OrElse(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeField).GetMethod("get_AttributeSetId").MethodHandle)), Expression.Field(Expression.Constant(variable3, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("attributeSetId").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeField).GetMethod("get_AttributeSetName").MethodHandle)), Expression.Field(Expression.Constant(variable3, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("attributeSetName").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle))));
					ParameterExpression[] parameterExpressionArray = new ParameterExpression[] { parameterExpression };
					RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixAttributeFields.ReadViewModel>> repositoryResponse = await repository.GetModelListByAsync(Expression.Lambda<Func<MixAttributeField, bool>>(binaryExpression, parameterExpressionArray), mixCmsContext, dbContextTransaction);
					readViewModels = (repositoryResponse.get_IsSucceed() ? repositoryResponse.get_Data() : new List<Mix.Cms.Lib.ViewModels.MixAttributeFields.ReadViewModel>());
					List<Mix.Cms.Lib.ViewModels.MixAttributeFields.ReadViewModel> readViewModels1 = readViewModels;
					if (!string.IsNullOrEmpty(request.get_Query().get_Item("query")))
					{
						item = request.get_Query().get_Item("query");
						str = item.ToString();
					}
					else
					{
						str = "{}";
					}
					JObject jObject = JObject.Parse(str);
					Expression<Func<MixAttributeSetValue, bool>> expression1 = null;
					Expression<Func<MixAttributeSetValue, bool>> expression2 = null;
					parameterExpression = Expression.Parameter(typeof(MixAttributeSetData), "m");
					BinaryExpression binaryExpression1 = Expression.AndAlso(Expression.AndAlso(Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSetData).GetMethod("get_Specificulture").MethodHandle)), Expression.Field(Expression.Constant(variable3, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("culture").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle))), Expression.OrElse(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSetData).GetMethod("get_AttributeSetId").MethodHandle)), Expression.Field(Expression.Constant(variable3, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("attributeSetId").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSetData).GetMethod("get_AttributeSetName").MethodHandle)), Expression.Field(Expression.Constant(variable3, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("attributeSetName").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle))))), Expression.OrElse(Expression.Not(Expression.Field(Expression.Constant(variable3, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("isStatus").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSetData).GetMethod("get_Status").MethodHandle)), Expression.Call(Expression.Field(Expression.Constant(variable3, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("status").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(object).GetMethod("ToString").MethodHandle), Array.Empty<Expression>())))), Expression.OrElse(Expression.Not(Expression.Field(Expression.Constant(variable3, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("isFromDate").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle))), Expression.GreaterThanOrEqual(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSetData).GetMethod("get_CreatedDateTime").MethodHandle)), Expression.Field(Expression.Constant(variable3, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("fromDate").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle)), false, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(DateTime).GetMethod("op_GreaterThanOrEqual", new Type[] { typeof(DateTime), typeof(DateTime) }).MethodHandle)))), Expression.OrElse(Expression.Not(Expression.Field(Expression.Constant(variable3, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("isToDate").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle))), Expression.LessThanOrEqual(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSetData).GetMethod("get_CreatedDateTime").MethodHandle)), Expression.Field(Expression.Constant(variable3, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("toDate").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle)), false, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(DateTime).GetMethod("op_LessThanOrEqual", new Type[] { typeof(DateTime), typeof(DateTime) }).MethodHandle))));
					ParameterExpression[] parameterExpressionArray1 = new ParameterExpression[] { parameterExpression };
					Expression<Func<MixAttributeSetData, bool>> expression3 = Expression.Lambda<Func<MixAttributeSetData, bool>>(binaryExpression1, parameterExpressionArray1);
					RepositoryResponse<PaginationModel<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>> repositoryResponse1 = new RepositoryResponse<PaginationModel<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>>();
					repositoryResponse1.set_IsSucceed(true);
					repositoryResponse1.set_Data(new PaginationModel<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>());
					if (jObject.get_Count() > 0 || !string.IsNullOrEmpty(str1))
					{
						if (!string.IsNullOrEmpty(str1))
						{
							foreach (Mix.Cms.Lib.ViewModels.MixAttributeFields.ReadViewModel readViewModel in readViewModels1)
							{
								parameterExpression = Expression.Parameter(typeof(MixAttributeSetValue), "m");
								BinaryExpression binaryExpression2 = Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSetValue).GetMethod("get_AttributeFieldName").MethodHandle)), Expression.Property(Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_1<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_1<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("field").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_1<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeFields.ReadViewModel).GetMethod("get_Name").MethodHandle))), Expression.AndAlso(Expression.Equal(Expression.Field(Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_1<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_1<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("CS$<>8__locals1").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_1<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("filterType").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle)), Expression.Constant("equal", typeof(string))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSetValue).GetMethod("get_StringValue").MethodHandle)), Expression.Field(Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_1<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_1<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("CS$<>8__locals1").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_1<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("keyword").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle)))));
								BinaryExpression binaryExpression3 = Expression.Equal(Expression.Field(Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_1<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_1<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("CS$<>8__locals1").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_1<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("filterType").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle)), Expression.Constant("contain", typeof(string)));
								MethodInfo methodFromHandle = (MethodInfo)MethodBase.GetMethodFromHandle(typeof(DbFunctionsExtensions).GetMethod("Like", new Type[] { typeof(DbFunctions), typeof(string), typeof(string) }).MethodHandle);
								Expression[] expressionArray = new Expression[] { Expression.Property(null, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(EF).GetMethod("get_Functions").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSetValue).GetMethod("get_StringValue").MethodHandle)), null };
								MethodInfo methodInfo = (MethodInfo)MethodBase.GetMethodFromHandle(typeof(string).GetMethod("Format", new Type[] { typeof(string), typeof(object) }).MethodHandle);
								Expression[] expressionArray1 = new Expression[] { Expression.Constant("%{0}%", typeof(string)), Expression.Field(Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_1<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_1<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("CS$<>8__locals1").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_1<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("keyword").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle)) };
								expressionArray[2] = Expression.Call(null, methodInfo, expressionArray1);
								BinaryExpression binaryExpression4 = Expression.OrElse(binaryExpression2, Expression.AndAlso(binaryExpression3, Expression.Call(null, methodFromHandle, expressionArray)));
								ParameterExpression[] parameterExpressionArray2 = new ParameterExpression[] { parameterExpression };
								Expression<Func<MixAttributeSetValue, bool>> expression4 = Expression.Lambda<Func<MixAttributeSetValue, bool>>(binaryExpression4, parameterExpressionArray2);
								expression2 = (expression2 == null ? expression4 : ReflectionHelper.CombineExpression<MixAttributeSetValue>(expression2, expression4, 6, "model"));
							}
						}
						if (jObject != null && jObject.Properties().Count<JProperty>() > 0)
						{
							foreach (KeyValuePair<string, JToken> keyValuePair in jObject)
							{
								if (!readViewModels1.Any<Mix.Cms.Lib.ViewModels.MixAttributeFields.ReadViewModel>((Mix.Cms.Lib.ViewModels.MixAttributeFields.ReadViewModel f) => f.Name == keyValuePair.Key) || string.IsNullOrEmpty(keyValuePair.Value.ToString()))
								{
									continue;
								}
								parameterExpression = Expression.Parameter(typeof(MixAttributeSetValue), "m");
								BinaryExpression binaryExpression5 = Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSetValue).GetMethod("get_AttributeFieldName").MethodHandle)), Expression.Property(Expression.Field(Expression.Constant(variable1, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_2<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_2<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("q").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_2<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(KeyValuePair<string, JToken>).GetMethod("get_Key").MethodHandle, typeof(KeyValuePair<string, JToken>).TypeHandle))), Expression.AndAlso(Expression.Equal(Expression.Field(Expression.Field(Expression.Constant(variable1, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_2<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_2<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("CS$<>8__locals2").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_2<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("filterType").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle)), Expression.Constant("equal", typeof(string))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSetValue).GetMethod("get_StringValue").MethodHandle)), Expression.Call(Expression.Property(Expression.Field(Expression.Constant(variable1, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_2<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_2<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("q").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_2<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(KeyValuePair<string, JToken>).GetMethod("get_Value").MethodHandle, typeof(KeyValuePair<string, JToken>).TypeHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(object).GetMethod("ToString").MethodHandle), Array.Empty<Expression>()))));
								BinaryExpression binaryExpression6 = Expression.Equal(Expression.Field(Expression.Field(Expression.Constant(variable1, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_2<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_2<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("CS$<>8__locals2").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_2<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("filterType").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_0<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle)), Expression.Constant("contain", typeof(string)));
								MethodInfo methodFromHandle1 = (MethodInfo)MethodBase.GetMethodFromHandle(typeof(DbFunctionsExtensions).GetMethod("Like", new Type[] { typeof(DbFunctions), typeof(string), typeof(string) }).MethodHandle);
								Expression[] expressionArray2 = new Expression[] { Expression.Property(null, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(EF).GetMethod("get_Functions").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSetValue).GetMethod("get_StringValue").MethodHandle)), null };
								MethodInfo methodInfo1 = (MethodInfo)MethodBase.GetMethodFromHandle(typeof(string).GetMethod("Format", new Type[] { typeof(string), typeof(object) }).MethodHandle);
								Expression[] expressionArray3 = new Expression[] { Expression.Constant("%{0}%", typeof(string)), Expression.Call(Expression.Property(Expression.Field(Expression.Constant(variable1, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_2<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_2<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("q").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_2<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(KeyValuePair<string, JToken>).GetMethod("get_Value").MethodHandle, typeof(KeyValuePair<string, JToken>).TypeHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(object).GetMethod("ToString").MethodHandle), Array.Empty<Expression>()) };
								expressionArray2[2] = Expression.Call(null, methodInfo1, expressionArray3);
								BinaryExpression binaryExpression7 = Expression.OrElse(binaryExpression5, Expression.AndAlso(binaryExpression6, Expression.Call(null, methodFromHandle1, expressionArray2)));
								ParameterExpression[] parameterExpressionArray3 = new ParameterExpression[] { parameterExpression };
								Expression<Func<MixAttributeSetValue, bool>> expression5 = Expression.Lambda<Func<MixAttributeSetValue, bool>>(binaryExpression7, parameterExpressionArray3);
								expression2 = (expression2 == null ? expression5 : ReflectionHelper.CombineExpression<MixAttributeSetValue>(expression2, expression5, 7, "model"));
							}
						}
						if (expression2 != null)
						{
							expression = (expression1 == null ? expression2 : ReflectionHelper.CombineExpression<MixAttributeSetValue>(expression2, expression1, 6, "model"));
							expression1 = expression;
						}
						if (expression1 != null)
						{
							IQueryable<MixAttributeSetValue> mixAttributeSetValues = mixCmsContext.MixAttributeSetValue.Where<MixAttributeSetValue>(expression1);
							parameterExpression = Expression.Parameter(typeof(MixAttributeSetValue), "m");
							MemberExpression memberExpression = Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSetValue).GetMethod("get_DataId").MethodHandle));
							ParameterExpression[] parameterExpressionArray4 = new ParameterExpression[] { parameterExpression };
							IQueryable<string> strs = mixAttributeSetValues.Select<MixAttributeSetValue, string>(Expression.Lambda<Func<MixAttributeSetValue, string>>(memberExpression, parameterExpressionArray4)).Distinct<string>();
							strs.ToList<string>();
							if (strs != null)
							{
								parameterExpression = Expression.Parameter(typeof(MixAttributeSetData), "m");
								MethodInfo methodFromHandle2 = (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Enumerable).GetMethod("Any", new Type[] { typeof(IEnumerable<string>), typeof(Func<string, bool>) }).MethodHandle);
								Expression[] expressionArray4 = new Expression[] { Expression.Field(Expression.Constant(variable2, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_3<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_3<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).GetField("dataIds").FieldHandle, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.u003cu003ec__DisplayClass4_3<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>).TypeHandle)), null };
								ParameterExpression parameterExpression1 = Expression.Parameter(typeof(string), "id");
								BinaryExpression binaryExpression8 = Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSetData).GetMethod("get_Id").MethodHandle)), parameterExpression1);
								ParameterExpression[] parameterExpressionArray5 = new ParameterExpression[] { parameterExpression1 };
								expressionArray4[1] = Expression.Lambda<Func<string, bool>>(binaryExpression8, parameterExpressionArray5);
								MethodCallExpression methodCallExpression = Expression.Call(null, methodFromHandle2, expressionArray4);
								ParameterExpression[] parameterExpressionArray6 = new ParameterExpression[] { parameterExpression };
								expression3 = ReflectionHelper.CombineExpression<MixAttributeSetData>(Expression.Lambda<Func<MixAttributeSetData, bool>>(methodCallExpression, parameterExpressionArray6), expression3, 6, "model");
							}
						}
					}
					int? nullable = null;
					int? nullable1 = nullable;
					nullable = null;
					modelListByAsync = await DefaultRepository<MixCmsContext, MixAttributeSetData, Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>.get_Instance().GetModelListByAsync(expression3, str2, displayDirection, new int?(num1), new int?(num), nullable1, nullable, mixCmsContext, dbContextTransaction);
				}
				catch (Exception exception)
				{
					modelListByAsync = UnitOfWorkHelper<MixCmsContext>.HandleException<PaginationModel<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>>(exception, flag, dbContextTransaction);
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
			return modelListByAsync;
		}

		public static async Task<RepositoryResponse<List<TView>>> FilterByKeywordAsync<TView>(string culture, string attributeSetName, string filterType, string fieldName, string keyword, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		where TView : ViewModelBase<MixCmsContext, MixAttributeSetData, TView>
		{
			RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>> repositoryResponse;
			MixCmsContext mixCmsContext = null;
			IDbContextTransaction dbContextTransaction = null;
			bool flag = false;
			UnitOfWorkHelper<MixCmsContext>.InitTransaction(_context, _transaction, ref mixCmsContext, ref dbContextTransaction, ref flag);
			try
			{
				try
				{
					Expression<Func<MixAttributeSetValue, bool>> specificulture = (MixAttributeSetValue m) => m.Specificulture == culture && m.AttributeSetName == attributeSetName;
					Expression<Func<MixAttributeSetValue, bool>> expression = null;
					RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>> repositoryResponse1 = new RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>>();
					repositoryResponse1.set_IsSucceed(true);
					repositoryResponse1.set_Data(new List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>());
					RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>> modelListByAsync = repositoryResponse1;
					if (filterType != "equal")
					{
						Expression<Func<MixAttributeSetValue, bool>> attributeFieldName = (MixAttributeSetValue m) => m.AttributeFieldName == fieldName && m.StringValue.Contains(keyword);
						expression = (expression == null ? attributeFieldName : ReflectionHelper.CombineExpression<MixAttributeSetValue>(expression, attributeFieldName, 6, "model"));
					}
					else
					{
						Expression<Func<MixAttributeSetValue, bool>> attributeFieldName1 = (MixAttributeSetValue m) => m.AttributeFieldName == fieldName && m.StringValue == keyword;
						expression = (expression == null ? attributeFieldName1 : ReflectionHelper.CombineExpression<MixAttributeSetValue>(expression, attributeFieldName1, 6, "model"));
					}
					if (expression != null)
					{
						specificulture = ReflectionHelper.CombineExpression<MixAttributeSetValue>(expression, specificulture, 6, "model");
					}
					IQueryable<MixAttributeSetValue> mixAttributeSetValues = mixCmsContext.MixAttributeSetValue.Where<MixAttributeSetValue>(specificulture);
					IQueryable<string> strs = (
						from m in mixAttributeSetValues
						select m.DataId).Distinct<string>();
					List<string> list = strs.ToList<string>();
					if (strs != null)
					{
						Expression<Func<MixAttributeSetData, bool>> expression1 = (MixAttributeSetData m) => list.Any<string>((string id) => m.Id == id);
						modelListByAsync = await DefaultRepository<MixCmsContext, MixAttributeSetData, Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>.get_Instance().GetModelListByAsync(expression1, mixCmsContext, dbContextTransaction);
					}
					repositoryResponse = modelListByAsync;
				}
				catch (Exception exception)
				{
					repositoryResponse = UnitOfWorkHelper<MixCmsContext>.HandleException<List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.TView>>(exception, flag, dbContextTransaction);
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
			RepositoryResponse<bool> repositoryResponse;
			using (MixCmsContext mixCmsContext = new MixCmsContext())
			{
				RepositoryResponse<bool> repositoryResponse1 = new RepositoryResponse<bool>();
				repositoryResponse1.set_IsSucceed(true);
				RepositoryResponse<bool> repositoryResponse2 = repositoryResponse1;
				try
				{
					List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel> importViewModels = Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.Helper.LoadFileData(culture, attributeSet, file);
					DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixAttributeField, Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel>.Repository;
					List<Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel> data = repository.GetModelListBy((MixAttributeField f) => f.AttributeSetId == attributeSet.Id, null, null).get_Data();
					DefaultRepository<!0, !1, !2> defaultRepository = ViewModelBase<MixCmsContext, MixAttributeSetData, Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel>.Repository;
					int num = defaultRepository.Count((MixAttributeSetData m) => m.AttributeSetName == attributeSet.Name && m.Specificulture == culture, null, null).get_Data();
					foreach (Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel name in importViewModels)
					{
						num++;
						name.Priority = num;
						name.Fields = data;
						name.AttributeSetName = attributeSet.Name;
						name.ParseModel(null, null);
						mixCmsContext.Entry<MixAttributeSetData>(name.get_Model()).set_State(4);
						foreach (Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ImportViewModel value in name.Values)
						{
							value.DataId = name.Id;
							value.Specificulture = culture;
							value.ParseModel(null, null);
							mixCmsContext.Entry<MixAttributeSetValue>(value.get_Model()).set_State(4);
						}
					}
					MixCmsContext mixCmsContext1 = mixCmsContext;
					CancellationToken cancellationToken = new CancellationToken();
					await ((DbContext)mixCmsContext1).SaveChangesAsync(cancellationToken);
					repositoryResponse = repositoryResponse2;
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					repositoryResponse2.set_IsSucceed(false);
					repositoryResponse2.set_Exception(exception);
					repositoryResponse2.get_Errors().Add(exception.Message);
					repositoryResponse = repositoryResponse2;
				}
			}
			return repositoryResponse;
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