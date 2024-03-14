using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels;
using Mix.Domain.Core.Models;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixLanguages
{
	public class ReadMvcViewModel : ViewModelBase<MixCmsContext, MixLanguage, ReadMvcViewModel>
	{
		[JsonProperty("category")]
		public string Category
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

		[JsonProperty("cultures")]
		public List<SupportedCulture> Cultures
		{
			get;
			set;
		}

		[JsonProperty("dataType")]
		public MixEnums.MixDataType DataType
		{
			get;
			set;
		}

		[JsonProperty("defaultValue")]
		public string DefaultValue
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

		[JsonProperty("id")]
		public int Id
		{
			get;
			set;
		}

		[JsonProperty("keyword")]
		[Required]
		public string Keyword
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

		[JsonProperty("priority")]
		public int Priority
		{
			get;
			set;
		}

		[JsonProperty("property")]
		public DataValueViewModel Property
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

		[JsonProperty("value")]
		public string Value
		{
			get;
			set;
		}

		public ReadMvcViewModel()
		{
		}

		public ReadMvcViewModel(MixLanguage model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.Property = new DataValueViewModel()
			{
				DataType = this.DataType,
				Value = this.Value,
				Name = this.Keyword
			};
		}

		public override RepositoryResponse<bool> RemoveRelatedModels(ReadMvcViewModel view, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			ReadMvcViewModel.u003cu003ec__DisplayClass67_0 variable = null;
			foreach (SupportedCulture supportedCulture in 
				from c in this.Cultures
				where c.get_Specificulture() != this.Specificulture
				select c)
			{
				DbSet<MixLanguage> mixLanguage = _context.MixLanguage;
				ParameterExpression parameterExpression = Expression.Parameter(typeof(MixLanguage), "c");
				MixLanguage mixLanguage1 = mixLanguage.First<MixLanguage>(Expression.Lambda<Func<MixLanguage, bool>>(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixLanguage).GetMethod("get_Keyword").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(ReadMvcViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(ReadMvcViewModel).GetMethod("get_Keyword").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixLanguage).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Field(Expression.Constant(variable, typeof(ReadMvcViewModel.u003cu003ec__DisplayClass67_0)), FieldInfo.GetFieldFromHandle(typeof(ReadMvcViewModel.u003cu003ec__DisplayClass67_0).GetField("culture").FieldHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(SupportedCulture).GetMethod("get_Specificulture").MethodHandle)))), new ParameterExpression[] { parameterExpression }));
				if (mixLanguage1 == null)
				{
					continue;
				}
				_context.MixLanguage.Remove(mixLanguage1);
			}
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(_context.SaveChanges() > 0);
			return repositoryResponse;
		}

		public override async Task<RepositoryResponse<bool>> RemoveRelatedModelsAsync(ReadMvcViewModel view, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			ReadMvcViewModel.u003cu003ec__DisplayClass68_0 variable = null;
			foreach (SupportedCulture supportedCulture in 
				from c in this.Cultures
				where c.get_Specificulture() != this.Specificulture
				select c)
			{
				DbSet<MixLanguage> mixLanguage = _context.MixLanguage;
				ParameterExpression parameterExpression = Expression.Parameter(typeof(MixLanguage), "c");
				BinaryExpression binaryExpression = Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixLanguage).GetMethod("get_Keyword").MethodHandle)), Expression.Property(Expression.Constant(this, typeof(ReadMvcViewModel)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(ReadMvcViewModel).GetMethod("get_Keyword").MethodHandle))), Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixLanguage).GetMethod("get_Specificulture").MethodHandle)), Expression.Property(Expression.Field(Expression.Constant(variable, typeof(ReadMvcViewModel.u003cu003ec__DisplayClass68_0)), FieldInfo.GetFieldFromHandle(typeof(ReadMvcViewModel.u003cu003ec__DisplayClass68_0).GetField("culture").FieldHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(SupportedCulture).GetMethod("get_Specificulture").MethodHandle))));
				ParameterExpression[] parameterExpressionArray = new ParameterExpression[] { parameterExpression };
				MixLanguage mixLanguage1 = mixLanguage.First<MixLanguage>(Expression.Lambda<Func<MixLanguage, bool>>(binaryExpression, parameterExpressionArray));
				if (mixLanguage1 == null)
				{
					continue;
				}
				_context.MixLanguage.Remove(mixLanguage1);
			}
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			MixCmsContext mixCmsContext = _context;
			CancellationToken cancellationToken = new CancellationToken();
			int num = await ((DbContext)mixCmsContext).SaveChangesAsync(cancellationToken);
			repositoryResponse1.set_IsSucceed(num > 0);
			return repositoryResponse;
		}
	}
}