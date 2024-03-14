using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixAttributeSetValues;
using Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas;
using Mix.Common.Helper;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Mix.Heart.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixPosts
{
	public class Helper
	{
		public Helper()
		{
		}

		public static async Task<RepositoryResponse<PaginationModel<TView>>> GetModelistByMeta<TView>(string metaName, string metaValue, string culture, string orderByPropertyName, MixHeartEnums.DisplayDirection direction, int? pageSize, int? pageIndex, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		where TView : ViewModelBase<MixCmsContext, MixPost, TView>
		{
			RepositoryResponse<PaginationModel<TView>> repositoryResponse;
			Helper.u003cu003ec__DisplayClass0_2<TView> variable = null;
			int num;
			MixCmsContext mixCmsContext = null;
			IDbContextTransaction dbContextTransaction = null;
			bool flag = false;
			Helper.u003cu003ec__DisplayClass0_1<TView> variable1 = null;
			int num1;
			UnitOfWorkHelper<MixCmsContext>.InitTransaction(_context, _transaction, ref mixCmsContext, ref dbContextTransaction, ref flag);
			try
			{
				try
				{
					RepositoryResponse<PaginationModel<TView>> repositoryResponse1 = new RepositoryResponse<PaginationModel<TView>>();
					repositoryResponse1.set_IsSucceed(true);
					PaginationModel<TView> paginationModel = new PaginationModel<TView>();
					num = (pageIndex.HasValue ? pageIndex.Value : 0);
					paginationModel.set_PageIndex(num);
					paginationModel.set_PageSize(pageSize);
					repositoryResponse1.set_Data(paginationModel);
					RepositoryResponse<PaginationModel<TView>> repositoryResponse2 = repositoryResponse1;
					List<Task<RepositoryResponse<TView>>> tasks = new List<Task<RepositoryResponse<TView>>>();
					DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixAttributeSetValue, Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ReadViewModel>.Repository;
					ParameterExpression parameterExpression = Expression.Parameter(typeof(MixAttributeSetValue), "m");
					BinaryExpression binaryExpression = Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSetValue).GetMethod("get_AttributeSetName").MethodHandle)), Expression.Field(Expression.Field(Expression.Constant(variable1, typeof(Helper.u003cu003ec__DisplayClass0_1<TView>)), FieldInfo.GetFieldFromHandle(typeof(Helper.u003cu003ec__DisplayClass0_1<TView>).GetField("CS$<>8__locals1").FieldHandle, typeof(Helper.u003cu003ec__DisplayClass0_1<TView>).TypeHandle)), FieldInfo.GetFieldFromHandle(typeof(Helper.u003cu003ec__DisplayClass0_0<TView>).GetField("metaName").FieldHandle, typeof(Helper.u003cu003ec__DisplayClass0_0<TView>).TypeHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSetValue).GetMethod("get_StringValue").MethodHandle)), Expression.Field(Expression.Field(Expression.Constant(variable1, typeof(Helper.u003cu003ec__DisplayClass0_1<TView>)), FieldInfo.GetFieldFromHandle(typeof(Helper.u003cu003ec__DisplayClass0_1<TView>).GetField("CS$<>8__locals1").FieldHandle, typeof(Helper.u003cu003ec__DisplayClass0_1<TView>).TypeHandle)), FieldInfo.GetFieldFromHandle(typeof(Helper.u003cu003ec__DisplayClass0_0<TView>).GetField("metaValue").FieldHandle, typeof(Helper.u003cu003ec__DisplayClass0_0<TView>).TypeHandle))));
					ParameterExpression[] parameterExpressionArray = new ParameterExpression[] { parameterExpression };
					RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ReadViewModel> singleModelAsync = await repository.GetSingleModelAsync(Expression.Lambda<Func<MixAttributeSetValue, bool>>(binaryExpression, parameterExpressionArray), mixCmsContext, dbContextTransaction);
					if (singleModelAsync.get_IsSucceed())
					{
						DefaultRepository<!0, !1, !2> defaultRepository = ViewModelBase<MixCmsContext, MixRelatedAttributeData, Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.ReadViewModel>.Repository;
						parameterExpression = Expression.Parameter(typeof(MixRelatedAttributeData), "m");
						BinaryExpression binaryExpression1 = Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_Specificulture").MethodHandle)), Expression.Field(Expression.Field(Expression.Constant(variable1, typeof(Helper.u003cu003ec__DisplayClass0_1<TView>)), FieldInfo.GetFieldFromHandle(typeof(Helper.u003cu003ec__DisplayClass0_1<TView>).GetField("CS$<>8__locals1").FieldHandle, typeof(Helper.u003cu003ec__DisplayClass0_1<TView>).TypeHandle)), FieldInfo.GetFieldFromHandle(typeof(Helper.u003cu003ec__DisplayClass0_0<TView>).GetField("culture").FieldHandle, typeof(Helper.u003cu003ec__DisplayClass0_0<TView>).TypeHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_DataId").MethodHandle)), Expression.Property(Expression.Property(Expression.Field(Expression.Constant(variable1, typeof(Helper.u003cu003ec__DisplayClass0_1<TView>)), FieldInfo.GetFieldFromHandle(typeof(Helper.u003cu003ec__DisplayClass0_1<TView>).GetField("getVal").FieldHandle, typeof(Helper.u003cu003ec__DisplayClass0_1<TView>).TypeHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ReadViewModel>).GetMethod("get_Data").MethodHandle, typeof(RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ReadViewModel>).TypeHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ReadViewModel).GetMethod("get_DataId").MethodHandle)))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeData).GetMethod("get_ParentType").MethodHandle)), Expression.Call(Expression.Constant(MixEnums.MixAttributeSetDataType.Post, typeof(MixEnums.MixAttributeSetDataType)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(object).GetMethod("ToString").MethodHandle), Array.Empty<Expression>())));
						ParameterExpression[] parameterExpressionArray1 = new ParameterExpression[] { parameterExpression };
						int? nullable = null;
						int? nullable1 = nullable;
						nullable = null;
						RepositoryResponse<PaginationModel<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.ReadViewModel>> modelListByAsync = await defaultRepository.GetModelListByAsync(Expression.Lambda<Func<MixRelatedAttributeData, bool>>(binaryExpression1, parameterExpressionArray1), orderByPropertyName, direction, pageIndex, pageSize, nullable1, nullable, mixCmsContext, dbContextTransaction);
						if (modelListByAsync.get_IsSucceed())
						{
							foreach (Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.ReadViewModel item in modelListByAsync.get_Data().get_Items())
							{
								if (!int.TryParse(item.ParentId, out num1))
								{
									continue;
								}
								DefaultRepository<MixCmsContext, MixPost, TView> instance = DefaultRepository<MixCmsContext, MixPost, TView>.get_Instance();
								parameterExpression = Expression.Parameter(typeof(MixPost), "m");
								BinaryExpression binaryExpression2 = Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPost).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Field(Expression.Constant(variable, typeof(Helper.u003cu003ec__DisplayClass0_2<TView>)), FieldInfo.GetFieldFromHandle(typeof(Helper.u003cu003ec__DisplayClass0_2<TView>).GetField("item").FieldHandle, typeof(Helper.u003cu003ec__DisplayClass0_2<TView>).TypeHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.ReadViewModel).GetMethod("get_Specificulture").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixPost).GetMethod("get_Id").MethodHandle)), Expression.Field(Expression.Constant(variable, typeof(Helper.u003cu003ec__DisplayClass0_2<TView>)), FieldInfo.GetFieldFromHandle(typeof(Helper.u003cu003ec__DisplayClass0_2<TView>).GetField("postId").FieldHandle, typeof(Helper.u003cu003ec__DisplayClass0_2<TView>).TypeHandle))));
								ParameterExpression[] parameterExpressionArray2 = new ParameterExpression[] { parameterExpression };
								RepositoryResponse<TView> singleModelAsync1 = await instance.GetSingleModelAsync(Expression.Lambda<Func<MixPost, bool>>(binaryExpression2, parameterExpressionArray2), mixCmsContext, dbContextTransaction);
								if (!singleModelAsync1.get_IsSucceed())
								{
									continue;
								}
								repositoryResponse2.get_Data().get_Items().Add(singleModelAsync1.get_Data());
							}
							repositoryResponse2.get_Data().set_TotalItems(modelListByAsync.get_Data().get_TotalItems());
							repositoryResponse2.get_Data().set_TotalPage(modelListByAsync.get_Data().get_TotalPage());
						}
						modelListByAsync = null;
					}
					parameterExpression = Expression.Parameter(typeof(MixAttributeSetValue), "m");
					BinaryExpression binaryExpression3 = Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSetValue).GetMethod("get_Specificulture").MethodHandle)), Expression.Field(Expression.Field(Expression.Constant(variable1, typeof(Helper.u003cu003ec__DisplayClass0_1<TView>)), FieldInfo.GetFieldFromHandle(typeof(Helper.u003cu003ec__DisplayClass0_1<TView>).GetField("CS$<>8__locals1").FieldHandle, typeof(Helper.u003cu003ec__DisplayClass0_1<TView>).TypeHandle)), FieldInfo.GetFieldFromHandle(typeof(Helper.u003cu003ec__DisplayClass0_0<TView>).GetField("culture").FieldHandle, typeof(Helper.u003cu003ec__DisplayClass0_0<TView>).TypeHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSetValue).GetMethod("get_AttributeSetName").MethodHandle)), Expression.Field(Expression.Field(Expression.Constant(variable1, typeof(Helper.u003cu003ec__DisplayClass0_1<TView>)), FieldInfo.GetFieldFromHandle(typeof(Helper.u003cu003ec__DisplayClass0_1<TView>).GetField("CS$<>8__locals1").FieldHandle, typeof(Helper.u003cu003ec__DisplayClass0_1<TView>).TypeHandle)), FieldInfo.GetFieldFromHandle(typeof(Helper.u003cu003ec__DisplayClass0_0<TView>).GetField("metaName").FieldHandle, typeof(Helper.u003cu003ec__DisplayClass0_0<TView>).TypeHandle)))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSetValue).GetMethod("get_StringValue").MethodHandle)), Expression.Field(Expression.Field(Expression.Constant(variable1, typeof(Helper.u003cu003ec__DisplayClass0_1<TView>)), FieldInfo.GetFieldFromHandle(typeof(Helper.u003cu003ec__DisplayClass0_1<TView>).GetField("CS$<>8__locals1").FieldHandle, typeof(Helper.u003cu003ec__DisplayClass0_1<TView>).TypeHandle)), FieldInfo.GetFieldFromHandle(typeof(Helper.u003cu003ec__DisplayClass0_0<TView>).GetField("metaValue").FieldHandle, typeof(Helper.u003cu003ec__DisplayClass0_0<TView>).TypeHandle))));
					ParameterExpression[] parameterExpressionArray3 = new ParameterExpression[] { parameterExpression };
					Expression.Lambda<Func<MixAttributeSetValue, bool>>(binaryExpression3, parameterExpressionArray3);
					repositoryResponse = repositoryResponse2;
				}
				catch (Exception exception)
				{
					repositoryResponse = UnitOfWorkHelper<MixCmsContext>.HandleException<PaginationModel<TView>>(exception, flag, dbContextTransaction);
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