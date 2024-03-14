using AutoMapper;
using Northwind.Application.Common.Mappings;
using Northwind.Domain.Entities;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Northwind.Application.Employees.Queries.GetEmployeesList
{
	public class EmployeeLookupDto : IMapFrom<Employee>
	{
		public string Extension
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

		public string Position
		{
			get;
			set;
		}

		public EmployeeLookupDto()
		{
		}

		public void Mapping(Profile profile)
		{
			IMappingExpression<Employee, EmployeeLookupDto> mappingExpression = profile.CreateMap<Employee, EmployeeLookupDto>();
			ParameterExpression parameterExpression1 = Expression.Parameter(typeof(EmployeeLookupDto), "d");
			IMappingExpression<Employee, EmployeeLookupDto> mappingExpression1 = mappingExpression.ForMember<int>(Expression.Lambda<Func<EmployeeLookupDto, int>>(Expression.Property(parameterExpression1, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(EmployeeLookupDto).GetMethod("get_Id").MethodHandle)), new ParameterExpression[] { parameterExpression1 }), (IMemberConfigurationExpression<Employee, EmployeeLookupDto, int> opt) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Employee), "s");
				opt.MapFrom<int>(Expression.Lambda<Func<Employee, int>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Employee).GetMethod("get_EmployeeId").MethodHandle)), new ParameterExpression[] { parameterExpression }));
			});
			parameterExpression1 = Expression.Parameter(typeof(EmployeeLookupDto), "d");
			IMappingExpression<Employee, EmployeeLookupDto> mappingExpression2 = mappingExpression1.ForMember<string>(Expression.Lambda<Func<EmployeeLookupDto, string>>(Expression.Property(parameterExpression1, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(EmployeeLookupDto).GetMethod("get_Name").MethodHandle)), new ParameterExpression[] { parameterExpression1 }), (IMemberConfigurationExpression<Employee, EmployeeLookupDto, string> opt) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Employee), "s");
				opt.MapFrom<string>(Expression.Lambda<Func<Employee, string>>(Expression.Add(Expression.Add(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Employee).GetMethod("get_LastName").MethodHandle)), Expression.Constant(", ", typeof(String)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(String).GetMethod("Concat", new Type[] { typeof(string), typeof(string) }).MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Employee).GetMethod("get_FirstName").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(String).GetMethod("Concat", new Type[] { typeof(string), typeof(string) }).MethodHandle)), new ParameterExpression[] { parameterExpression }));
			});
			parameterExpression1 = Expression.Parameter(typeof(EmployeeLookupDto), "d");
			mappingExpression2.ForMember<string>(Expression.Lambda<Func<EmployeeLookupDto, string>>(Expression.Property(parameterExpression1, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(EmployeeLookupDto).GetMethod("get_Position").MethodHandle)), new ParameterExpression[] { parameterExpression1 }), (IMemberConfigurationExpression<Employee, EmployeeLookupDto, string> opt) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Employee), "s");
				opt.MapFrom<string>(Expression.Lambda<Func<Employee, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Employee).GetMethod("get_Title").MethodHandle)), new ParameterExpression[] { parameterExpression }));
			});
		}
	}
}