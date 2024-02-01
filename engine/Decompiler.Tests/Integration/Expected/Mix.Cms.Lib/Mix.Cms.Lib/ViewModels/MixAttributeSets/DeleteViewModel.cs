using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixAttributeFields;
using Mix.Cms.Lib.ViewModels.MixAttributeSetDatas;
using Mix.Cms.Lib.ViewModels.MixRelatedAttributeSets;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixAttributeSets
{
	public class DeleteViewModel : ViewModelBase<MixCmsContext, MixAttributeSet, Mix.Cms.Lib.ViewModels.MixAttributeSets.DeleteViewModel>
	{
		[JsonProperty("id")]
		public int Id
		{
			get;
			set;
		}

		[JsonProperty("ReferenceId")]
		public int? ReferenceId
		{
			get;
			set;
		}

		public DeleteViewModel()
		{
		}

		public DeleteViewModel(MixAttributeSet model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override RepositoryResponse<bool> RemoveRelatedModels(Mix.Cms.Lib.ViewModels.MixAttributeSets.DeleteViewModel view, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			ViewModelHelper.HandleResult<List<MixAttributeSetData>>(ViewModelBase<MixCmsContext, MixAttributeSetData, Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.DeleteViewModel>.Repository.RemoveListModel(false, (MixAttributeSetData f) => f.AttributeSetId == this.Id, _context, _transaction), ref repositoryResponse1);
			if (repositoryResponse1.get_IsSucceed())
			{
				ViewModelHelper.HandleResult<List<MixAttributeField>>(ViewModelBase<MixCmsContext, MixAttributeField, Mix.Cms.Lib.ViewModels.MixAttributeFields.DeleteViewModel>.Repository.RemoveListModel(false, (MixAttributeField f) => f.AttributeSetId == this.Id, _context, _transaction), ref repositoryResponse1);
			}
			return repositoryResponse1;
		}

		public override async Task<RepositoryResponse<bool>> RemoveRelatedModelsAsync(Mix.Cms.Lib.ViewModels.MixAttributeSets.DeleteViewModel view, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixAttributeSetData, Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.DeleteViewModel>.Repository;
			ParameterExpression parameterExpression = Expression.Parameter(typeof(MixAttributeSetData), "f");
			BinaryExpression binaryExpression = Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeSetData).GetMethod("get_AttributeSetId").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSets.DeleteViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSets.DeleteViewModel).GetMethod("get_Id").MethodHandle)));
			ParameterExpression[] parameterExpressionArray = new ParameterExpression[] { parameterExpression };
			ViewModelHelper.HandleResult<List<MixAttributeSetData>>(await repository.RemoveListModelAsync(false, Expression.Lambda<Func<MixAttributeSetData, bool>>(binaryExpression, parameterExpressionArray), _context, _transaction), ref repositoryResponse1);
			if (repositoryResponse1.get_IsSucceed())
			{
				DefaultRepository<!0, !1, !2> defaultRepository = ViewModelBase<MixCmsContext, MixAttributeField, Mix.Cms.Lib.ViewModels.MixAttributeFields.DeleteViewModel>.Repository;
				parameterExpression = Expression.Parameter(typeof(MixAttributeField), "f");
				BinaryExpression binaryExpression1 = Expression.OrElse(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeField).GetMethod("get_AttributeSetId").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSets.DeleteViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSets.DeleteViewModel).GetMethod("get_Id").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixAttributeField).GetMethod("get_ReferenceId").MethodHandle)), Expression.Convert(Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSets.DeleteViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSets.DeleteViewModel).GetMethod("get_Id").MethodHandle)), typeof(int?))));
				ParameterExpression[] parameterExpressionArray1 = new ParameterExpression[] { parameterExpression };
				ViewModelHelper.HandleResult<List<MixAttributeField>>(await defaultRepository.RemoveListModelAsync(false, Expression.Lambda<Func<MixAttributeField, bool>>(binaryExpression1, parameterExpressionArray1), _context, _transaction), ref repositoryResponse1);
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				DefaultRepository<!0, !1, !2> repository1 = ViewModelBase<MixCmsContext, MixRelatedAttributeSet, Mix.Cms.Lib.ViewModels.MixRelatedAttributeSets.DeleteViewModel>.Repository;
				parameterExpression = Expression.Parameter(typeof(MixRelatedAttributeSet), "f");
				BinaryExpression binaryExpression2 = Expression.OrElse(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeSet).GetMethod("get_Id").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSets.DeleteViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSets.DeleteViewModel).GetMethod("get_Id").MethodHandle))), Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeSet).GetMethod("get_ParentId").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(Mix.Cms.Lib.ViewModels.MixAttributeSets.DeleteViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixAttributeSets.DeleteViewModel).GetMethod("get_Id").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixRelatedAttributeSet).GetMethod("get_ParentType").MethodHandle)), Expression.Call(Expression.Constant(MixEnums.MixAttributeSetDataType.Service, typeof(MixEnums.MixAttributeSetDataType)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(object).GetMethod("ToString").MethodHandle), Array.Empty<Expression>()))));
				ParameterExpression[] parameterExpressionArray2 = new ParameterExpression[] { parameterExpression };
				ViewModelHelper.HandleResult<List<MixRelatedAttributeSet>>(await repository1.RemoveListModelAsync(false, Expression.Lambda<Func<MixRelatedAttributeSet, bool>>(binaryExpression2, parameterExpressionArray2), _context, _transaction), ref repositoryResponse1);
			}
			return repositoryResponse1;
		}
	}
}