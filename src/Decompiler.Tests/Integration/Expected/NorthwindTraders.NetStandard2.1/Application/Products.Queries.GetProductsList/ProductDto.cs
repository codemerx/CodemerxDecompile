using AutoMapper;
using Northwind.Application.Common.Mappings;
using Northwind.Domain.Entities;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Northwind.Application.Products.Queries.GetProductsList
{
	public class ProductDto : IMapFrom<Product>
	{
		public int? CategoryId
		{
			get;
			set;
		}

		public string CategoryName
		{
			get;
			set;
		}

		public bool Discontinued
		{
			get;
			set;
		}

		public int ProductId
		{
			get;
			set;
		}

		public string ProductName
		{
			get;
			set;
		}

		public string SupplierCompanyName
		{
			get;
			set;
		}

		public int? SupplierId
		{
			get;
			set;
		}

		public Decimal? UnitPrice
		{
			get;
			set;
		}

		public ProductDto()
		{
		}

		public void Mapping(Profile profile)
		{
			IMappingExpression<Product, ProductDto> mappingExpression = profile.CreateMap<Product, ProductDto>();
			ParameterExpression parameterExpression1 = Expression.Parameter(typeof(ProductDto), "d");
			IMappingExpression<Product, ProductDto> mappingExpression1 = mappingExpression.ForMember<string>(Expression.Lambda<Func<ProductDto, string>>(Expression.Property(parameterExpression1, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(ProductDto).GetMethod("get_SupplierCompanyName").MethodHandle)), new ParameterExpression[] { parameterExpression1 }), (IMemberConfigurationExpression<Product, ProductDto, string> opt) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Product), "s");
				opt.MapFrom<string>(Expression.Lambda<Func<Product, string>>(Expression.Condition(Expression.NotEqual(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Product).GetMethod("get_Supplier").MethodHandle)), Expression.Constant(null, typeof(Object))), Expression.Property(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Product).GetMethod("get_Supplier").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Supplier).GetMethod("get_CompanyName").MethodHandle)), Expression.Field(null, FieldInfo.GetFieldFromHandle(typeof(String).GetField("Empty").FieldHandle))), new ParameterExpression[] { parameterExpression }));
			});
			parameterExpression1 = Expression.Parameter(typeof(ProductDto), "d");
			mappingExpression1.ForMember<string>(Expression.Lambda<Func<ProductDto, string>>(Expression.Property(parameterExpression1, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(ProductDto).GetMethod("get_CategoryName").MethodHandle)), new ParameterExpression[] { parameterExpression1 }), (IMemberConfigurationExpression<Product, ProductDto, string> opt) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Product), "s");
				opt.MapFrom<string>(Expression.Lambda<Func<Product, string>>(Expression.Condition(Expression.NotEqual(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Product).GetMethod("get_Category").MethodHandle)), Expression.Constant(null, typeof(Object))), Expression.Property(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Product).GetMethod("get_Category").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Category).GetMethod("get_CategoryName").MethodHandle)), Expression.Field(null, FieldInfo.GetFieldFromHandle(typeof(String).GetField("Empty").FieldHandle))), new ParameterExpression[] { parameterExpression }));
			});
		}
	}
}