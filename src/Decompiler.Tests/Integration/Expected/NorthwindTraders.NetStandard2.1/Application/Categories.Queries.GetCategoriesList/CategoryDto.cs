using AutoMapper;
using Northwind.Application.Common.Mappings;
using Northwind.Domain.Entities;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Northwind.Application.Categories.Queries.GetCategoriesList
{
	public class CategoryDto : IMapFrom<Category>
	{
		public string Description
		{
			get;
			set;
		}

		public int Id
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public byte[] Picture
		{
			get;
			set;
		}

		public CategoryDto()
		{
		}

		public void Mapping(Profile profile)
		{
			IMappingExpression<Category, CategoryDto> mappingExpression = profile.CreateMap<Category, CategoryDto>();
			ParameterExpression parameterExpression1 = Expression.Parameter(typeof(CategoryDto), "d");
			IMappingExpression<Category, CategoryDto> mappingExpression1 = mappingExpression.ForMember<int>(Expression.Lambda<Func<CategoryDto, int>>(Expression.Property(parameterExpression1, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(CategoryDto).GetMethod("get_Id").MethodHandle)), new ParameterExpression[] { parameterExpression1 }), (IMemberConfigurationExpression<Category, CategoryDto, int> opt) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Category), "s");
				opt.MapFrom<int>(Expression.Lambda<Func<Category, int>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Category).GetMethod("get_CategoryId").MethodHandle)), new ParameterExpression[] { parameterExpression }));
			});
			parameterExpression1 = Expression.Parameter(typeof(CategoryDto), "d");
			mappingExpression1.ForMember<string>(Expression.Lambda<Func<CategoryDto, string>>(Expression.Property(parameterExpression1, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(CategoryDto).GetMethod("get_Name").MethodHandle)), new ParameterExpression[] { parameterExpression1 }), (IMemberConfigurationExpression<Category, CategoryDto, string> opt) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Category), "s");
				opt.MapFrom<string>(Expression.Lambda<Func<Category, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Category).GetMethod("get_CategoryName").MethodHandle)), new ParameterExpression[] { parameterExpression }));
			});
		}
	}
}