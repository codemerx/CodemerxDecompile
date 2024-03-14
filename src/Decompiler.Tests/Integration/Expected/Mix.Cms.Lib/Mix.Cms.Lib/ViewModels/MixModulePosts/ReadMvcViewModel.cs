using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixPosts;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels.MixModulePosts
{
	public class ReadMvcViewModel : ViewModelBase<MixCmsContext, MixModulePost, Mix.Cms.Lib.ViewModels.MixModulePosts.ReadMvcViewModel>
	{
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

		[JsonProperty("image")]
		public string Image
		{
			get;
			set;
		}

		[JsonProperty("isActived")]
		public bool IsActived
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

		[JsonProperty("modifiedBy")]
		public string ModifiedBy
		{
			get;
			set;
		}

		[JsonProperty("moduleId")]
		public int ModuleId
		{
			get;
			set;
		}

		[JsonProperty("post")]
		public Mix.Cms.Lib.ViewModels.MixPosts.ReadMvcViewModel Post
		{
			get;
			set;
		}

		[JsonProperty("postId")]
		public int PostId
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

		public ReadMvcViewModel(MixModulePost model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public ReadMvcViewModel()
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<Mix.Cms.Lib.ViewModels.MixPosts.ReadMvcViewModel> singleModel = ViewModelBase<MixCmsContext, MixPost, Mix.Cms.Lib.ViewModels.MixPosts.ReadMvcViewModel>.Repository.GetSingleModel((MixPost p) => p.Id == this.PostId && p.Specificulture == this.Specificulture, _context, _transaction);
			if (singleModel.get_IsSucceed())
			{
				this.Post = singleModel.get_Data();
			}
		}

		public static RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel>> GetModulePostNavAsync(int postId, string specificulture, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			Mix.Cms.Lib.ViewModels.MixModulePosts.ReadMvcViewModel.u003cu003ec__DisplayClass55_0 variable = null;
			RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel>> repositoryResponse;
			MixCmsContext mixCmsContext = _context ?? new MixCmsContext();
			IDbContextTransaction dbContextTransaction = _transaction ?? mixCmsContext.get_Database().BeginTransaction();
			try
			{
				try
				{
					DbSet<MixModule> mixModule = mixCmsContext.MixModule;
					ParameterExpression parameterExpression = Expression.Parameter(typeof(MixModule), "cp");
					IIncludableQueryable<MixModule, ICollection<MixModulePost>> includableQueryable = EntityFrameworkQueryableExtensions.Include<MixModule, ICollection<MixModulePost>>(mixModule, Expression.Lambda<Func<MixModule, ICollection<MixModulePost>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixModule).GetMethod("get_MixModulePost").MethodHandle)), new ParameterExpression[] { parameterExpression }));
					parameterExpression = Expression.Parameter(typeof(MixModule), "a");
					IQueryable<MixModule> mixModules = includableQueryable.Where<MixModule>(Expression.Lambda<Func<MixModule, bool>>(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixModule).GetMethod("get_Specificulture").MethodHandle)), Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.MixModulePosts.ReadMvcViewModel.u003cu003ec__DisplayClass55_0)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixModulePosts.ReadMvcViewModel.u003cu003ec__DisplayClass55_0).GetField("specificulture").FieldHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixModule).GetMethod("get_Type").MethodHandle)), Expression.Constant(2, typeof(int)))), new ParameterExpression[] { parameterExpression }));
					parameterExpression = Expression.Parameter(typeof(MixModule), "p");
					NewExpression newExpression = Expression.New((ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel).GetMethod(".ctor", new Type[] { typeof(MixModulePost), typeof(MixCmsContext), typeof(IDbContextTransaction) }).MethodHandle), (IEnumerable<Expression>)(new Expression[] { Expression.MemberInit(Expression.New(typeof(MixModulePost)), new MemberBinding[] { Expression.Bind((MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixModulePost).GetMethod("set_PostId", new Type[] { typeof(int) }).MethodHandle), Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.MixModulePosts.ReadMvcViewModel.u003cu003ec__DisplayClass55_0)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixModulePosts.ReadMvcViewModel.u003cu003ec__DisplayClass55_0).GetField("postId").FieldHandle))), Expression.Bind((MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixModulePost).GetMethod("set_ModuleId", new Type[] { typeof(int) }).MethodHandle), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixModule).GetMethod("get_Id").MethodHandle))), Expression.Bind((MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixModulePost).GetMethod("set_Specificulture", new Type[] { typeof(string) }).MethodHandle), Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.MixModulePosts.ReadMvcViewModel.u003cu003ec__DisplayClass55_0)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixModulePosts.ReadMvcViewModel.u003cu003ec__DisplayClass55_0).GetField("specificulture").FieldHandle))) }), Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.MixModulePosts.ReadMvcViewModel.u003cu003ec__DisplayClass55_0)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixModulePosts.ReadMvcViewModel.u003cu003ec__DisplayClass55_0).GetField("_context").FieldHandle)), Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.MixModulePosts.ReadMvcViewModel.u003cu003ec__DisplayClass55_0)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixModulePosts.ReadMvcViewModel.u003cu003ec__DisplayClass55_0).GetField("_transaction").FieldHandle)) }));
					MemberBinding[] memberBindingArray = new MemberBinding[2];
					MethodInfo methodFromHandle = (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel).GetMethod("set_IsActived", new Type[] { typeof(bool) }).MethodHandle);
					MethodInfo methodInfo = (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Enumerable).GetMethod("Any", new Type[] { typeof(IEnumerable<MixModulePost>), typeof(Func<MixModulePost, bool>) }).MethodHandle);
					Expression[] expressionArray = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixModule).GetMethod("get_MixModulePost").MethodHandle)), null };
					ParameterExpression parameterExpression1 = Expression.Parameter(typeof(MixModulePost), "cp");
					expressionArray[1] = Expression.Lambda<Func<MixModulePost, bool>>(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression1, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixModulePost).GetMethod("get_PostId").MethodHandle)), Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.MixModulePosts.ReadMvcViewModel.u003cu003ec__DisplayClass55_0)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixModulePosts.ReadMvcViewModel.u003cu003ec__DisplayClass55_0).GetField("postId").FieldHandle))), Expression.Equal(Expression.Property(parameterExpression1, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixModulePost).GetMethod("get_Specificulture").MethodHandle)), Expression.Field(Expression.Constant(variable, typeof(Mix.Cms.Lib.ViewModels.MixModulePosts.ReadMvcViewModel.u003cu003ec__DisplayClass55_0)), FieldInfo.GetFieldFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixModulePosts.ReadMvcViewModel.u003cu003ec__DisplayClass55_0).GetField("specificulture").FieldHandle)))), new ParameterExpression[] { parameterExpression1 });
					memberBindingArray[0] = Expression.Bind(methodFromHandle, Expression.Call(null, methodInfo, expressionArray));
					memberBindingArray[1] = Expression.Bind((MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel).GetMethod("set_Description", new Type[] { typeof(string) }).MethodHandle), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixModule).GetMethod("get_Title").MethodHandle)));
					IQueryable<Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel> readViewModels = mixModules.Select<MixModule, Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel>(Expression.Lambda<Func<MixModule, Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel>>(Expression.MemberInit(newExpression, memberBindingArray), new ParameterExpression[] { parameterExpression }));
					RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel>> repositoryResponse1 = new RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel>>();
					repositoryResponse1.set_IsSucceed(true);
					repositoryResponse1.set_Data(readViewModels.ToList<Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel>());
					repositoryResponse = repositoryResponse1;
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					if (_transaction == null)
					{
						dbContextTransaction.Rollback();
					}
					RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel>> repositoryResponse2 = new RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel>>();
					repositoryResponse2.set_IsSucceed(true);
					repositoryResponse2.set_Data(null);
					repositoryResponse2.set_Exception(exception);
					repositoryResponse = repositoryResponse2;
				}
			}
			finally
			{
				if (_context == null)
				{
					dbContextTransaction.Dispose();
					RelationalDatabaseFacadeExtensions.CloseConnection(mixCmsContext.get_Database());
					dbContextTransaction.Dispose();
					mixCmsContext.Dispose();
				}
			}
			return repositoryResponse;
		}
	}
}