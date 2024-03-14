using AutoMapper;
using Northwind.Application.Common.Mappings;
using Northwind.Domain.Entities;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Northwind.Application.Products.Queries.GetProductDetail
{
	public class ProductDetailVm : IMapFrom<Product>
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

		public bool DeleteEnabled
		{
			get;
			set;
		}

		public bool Discontinued
		{
			get;
			set;
		}

		public bool EditEnabled
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

		public ProductDetailVm()
		{
		}

		public void Mapping(Profile profile)
		{
			IMappingExpression<Product, ProductDetailVm> mappingExpression = profile.CreateMap<Product, ProductDetailVm>();
			ParameterExpression parameterExpression1 = Expression.Parameter(typeof(ProductDetailVm), "d");
			IMappingExpression<Product, ProductDetailVm> mappingExpression1 = mappingExpression.ForMember<bool>(Expression.Lambda<Func<ProductDetailVm, bool>>(Expression.Property(parameterExpression1, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(ProductDetailVm).GetMethod("get_EditEnabled").MethodHandle)), new ParameterExpression[] { parameterExpression1 }), (IMemberConfigurationExpression<Product, ProductDetailVm, bool> opt) => opt.Ignore());
			parameterExpression1 = Expression.Parameter(typeof(ProductDetailVm), "d");
			IMappingExpression<Product, ProductDetailVm> mappingExpression2 = mappingExpression1.ForMember<bool>(Expression.Lambda<Func<ProductDetailVm, bool>>(Expression.Property(parameterExpression1, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(ProductDetailVm).GetMethod("get_DeleteEnabled").MethodHandle)), new ParameterExpression[] { parameterExpression1 }), (IMemberConfigurationExpression<Product, ProductDetailVm, bool> opt) => opt.Ignore());
			parameterExpression1 = Expression.Parameter(typeof(ProductDetailVm), "d");
			IMappingExpression<Product, ProductDetailVm> mappingExpression3 = mappingExpression2.ForMember<string>(Expression.Lambda<Func<ProductDetailVm, string>>(Expression.Property(parameterExpression1, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(ProductDetailVm).GetMethod("get_SupplierCompanyName").MethodHandle)), new ParameterExpression[] { parameterExpression1 }), (IMemberConfigurationExpression<Product, ProductDetailVm, string> opt) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Product), "s");
				opt.MapFrom<string>(Expression.Lambda<Func<Product, string>>(Expression.Condition(Expression.NotEqual(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Product).GetMethod("get_Supplier").MethodHandle)), Expression.Constant(null, typeof(Object))), Expression.Property(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Product).GetMethod("get_Supplier").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Supplier).GetMethod("get_CompanyName").MethodHandle)), Expression.Field(null, FieldInfo.GetFieldFromHandle(typeof(String).GetField("Empty").FieldHandle))), new ParameterExpression[] { parameterExpression }));
			});
			parameterExpression1 = Expression.Parameter(typeof(ProductDetailVm), "d");
			mappingExpression3.ForMember<string>(Expression.Lambda<Func<ProductDetailVm, string>>(Expression.Property(parameterExpression1, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(ProductDetailVm).GetMethod("get_CategoryName").MethodHandle)), new ParameterExpression[] { parameterExpression1 }), (IMemberConfigurationExpression<Product, ProductDetailVm, string> opt) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Product), "s");
				opt.MapFrom<string>(Expression.Lambda<Func<Product, string>>(Expression.Condition(Expression.NotEqual(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Product).GetMethod("get_Category").MethodHandle)), Expression.Constant(null, typeof(Object))), Expression.Property(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Product).GetMethod("get_Category").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Category).GetMethod("get_CategoryName").MethodHandle)), Expression.Field(null, FieldInfo.GetFieldFromHandle(typeof(String).GetField("Empty").FieldHandle))), new ParameterExpression[] { parameterExpression }));
			});
		}
	}
}