using AutoMapper;
using Northwind.Application.Common.Mappings;
using Northwind.Domain.Entities;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Northwind.Application.Employees.Queries.GetEmployeeDetail
{
	public class EmployeeTerritoryDto : IMapFrom<EmployeeTerritory>
	{
		public string Region
		{
			get;
			set;
		}

		public string Territory
		{
			get;
			set;
		}

		public string TerritoryId
		{
			get;
			set;
		}

		public EmployeeTerritoryDto()
		{
		}

		public void Mapping(Profile profile)
		{
			IMappingExpression<EmployeeTerritory, EmployeeTerritoryDto> mappingExpression = profile.CreateMap<EmployeeTerritory, EmployeeTerritoryDto>();
			ParameterExpression parameterExpression1 = Expression.Parameter(typeof(EmployeeTerritoryDto), "d");
			IMappingExpression<EmployeeTerritory, EmployeeTerritoryDto> mappingExpression1 = mappingExpression.ForMember<string>(Expression.Lambda<Func<EmployeeTerritoryDto, string>>(Expression.Property(parameterExpression1, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(EmployeeTerritoryDto).GetMethod("get_TerritoryId").MethodHandle)), new ParameterExpression[] { parameterExpression1 }), (IMemberConfigurationExpression<EmployeeTerritory, EmployeeTerritoryDto, string> opts) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(EmployeeTerritory), "s");
				opts.MapFrom<string>(Expression.Lambda<Func<EmployeeTerritory, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(EmployeeTerritory).GetMethod("get_TerritoryId").MethodHandle)), new ParameterExpression[] { parameterExpression }));
			});
			parameterExpression1 = Expression.Parameter(typeof(EmployeeTerritoryDto), "d");
			IMappingExpression<EmployeeTerritory, EmployeeTerritoryDto> mappingExpression2 = mappingExpression1.ForMember<string>(Expression.Lambda<Func<EmployeeTerritoryDto, string>>(Expression.Property(parameterExpression1, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(EmployeeTerritoryDto).GetMethod("get_Territory").MethodHandle)), new ParameterExpression[] { parameterExpression1 }), (IMemberConfigurationExpression<EmployeeTerritory, EmployeeTerritoryDto, string> opts) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(EmployeeTerritory), "s");
				opts.MapFrom<string>(Expression.Lambda<Func<EmployeeTerritory, string>>(Expression.Property(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(EmployeeTerritory).GetMethod("get_Territory").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Northwind.Domain.Entities.Territory).GetMethod("get_TerritoryDescription").MethodHandle)), new ParameterExpression[] { parameterExpression }));
			});
			parameterExpression1 = Expression.Parameter(typeof(EmployeeTerritoryDto), "d");
			mappingExpression2.ForMember<string>(Expression.Lambda<Func<EmployeeTerritoryDto, string>>(Expression.Property(parameterExpression1, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(EmployeeTerritoryDto).GetMethod("get_Region").MethodHandle)), new ParameterExpression[] { parameterExpression1 }), (IMemberConfigurationExpression<EmployeeTerritory, EmployeeTerritoryDto, string> opts) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(EmployeeTerritory), "s");
				opts.MapFrom<string>(Expression.Lambda<Func<EmployeeTerritory, string>>(Expression.Property(Expression.Property(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(EmployeeTerritory).GetMethod("get_Territory").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Northwind.Domain.Entities.Territory).GetMethod("get_Region").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Northwind.Domain.Entities.Region).GetMethod("get_RegionDescription").MethodHandle)), new ParameterExpression[] { parameterExpression }));
			});
		}
	}
}