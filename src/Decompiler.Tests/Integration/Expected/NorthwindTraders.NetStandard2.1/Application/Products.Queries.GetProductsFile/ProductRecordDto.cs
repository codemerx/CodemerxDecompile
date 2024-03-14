using AutoMapper;
using Northwind.Application.Common.Mappings;
using Northwind.Domain.Entities;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Northwind.Application.Products.Queries.GetProductsFile
{
	public class ProductRecordDto : IMapFrom<Product>
	{
		public string Category
		{
			get;
			set;
		}

		public bool Discontinued
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public Decimal? UnitPrice
		{
			get;
			set;
		}

		public ProductRecordDto()
		{
		}

		public void Mapping(Profile profile)
		{
			IMappingExpression<Product, ProductRecordDto> mappingExpression = profile.CreateMap<Product, ProductRecordDto>();
			ParameterExpression parameterExpression1 = Expression.Parameter(typeof(ProductRecordDto), "d");
			IMappingExpression<Product, ProductRecordDto> mappingExpression1 = mappingExpression.ForMember<string>(Expression.Lambda<Func<ProductRecordDto, string>>(Expression.Property(parameterExpression1, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(ProductRecordDto).GetMethod("get_Name").MethodHandle)), new ParameterExpression[] { parameterExpression1 }), (IMemberConfigurationExpression<Product, ProductRecordDto, string> opt) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Product), "s");
				opt.MapFrom<string>(Expression.Lambda<Func<Product, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Product).GetMethod("get_ProductName").MethodHandle)), new ParameterExpression[] { parameterExpression }));
			});
			parameterExpression1 = Expression.Parameter(typeof(ProductRecordDto), "d");
			mappingExpression1.ForMember<string>(Expression.Lambda<Func<ProductRecordDto, string>>(Expression.Property(parameterExpression1, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(ProductRecordDto).GetMethod("get_Category").MethodHandle)), new ParameterExpression[] { parameterExpression1 }), (IMemberConfigurationExpression<Product, ProductRecordDto, string> opt) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Product), "s");
				opt.MapFrom<string>(Expression.Lambda<Func<Product, string>>(Expression.Condition(Expression.NotEqual(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Product).GetMethod("get_Category").MethodHandle)), Expression.Constant(null, typeof(Object))), Expression.Property(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Product).GetMethod("get_Category").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Northwind.Domain.Entities.Category).GetMethod("get_CategoryName").MethodHandle)), Expression.Field(null, FieldInfo.GetFieldFromHandle(typeof(String).GetField("Empty").FieldHandle))), new ParameterExpression[] { parameterExpression }));
			});
		}
	}
}