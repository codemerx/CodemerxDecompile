using AutoMapper;
using Northwind.Application.Common.Mappings;
using Northwind.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Northwind.Application.Employees.Queries.GetEmployeeDetail
{
	public class EmployeeDetailVm : IMapFrom<Employee>
	{
		public string Address
		{
			get;
			set;
		}

		public DateTime? BirthDate
		{
			get;
			set;
		}

		public string City
		{
			get;
			set;
		}

		public string Country
		{
			get;
			set;
		}

		public string Extension
		{
			get;
			set;
		}

		public string FirstName
		{
			get;
			set;
		}

		public DateTime? HireDate
		{
			get;
			set;
		}

		public string HomePhone
		{
			get;
			set;
		}

		public int Id
		{
			get;
			set;
		}

		public string LastName
		{
			get;
			set;
		}

		public int? ManagerId
		{
			get;
			set;
		}

		public string Notes
		{
			get;
			set;
		}

		public byte[] Photo
		{
			get;
			set;
		}

		public string Position
		{
			get;
			set;
		}

		public string PostalCode
		{
			get;
			set;
		}

		public string Region
		{
			get;
			set;
		}

		public virtual List<EmployeeTerritoryDto> Territories
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public EmployeeDetailVm()
		{
		}

		public void Mapping(Profile profile)
		{
			IMappingExpression<Employee, EmployeeDetailVm> mappingExpression = profile.CreateMap<Employee, EmployeeDetailVm>();
			ParameterExpression parameterExpression1 = Expression.Parameter(typeof(EmployeeDetailVm), "d");
			IMappingExpression<Employee, EmployeeDetailVm> mappingExpression1 = mappingExpression.ForMember<int>(Expression.Lambda<Func<EmployeeDetailVm, int>>(Expression.Property(parameterExpression1, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(EmployeeDetailVm).GetMethod("get_Id").MethodHandle)), new ParameterExpression[] { parameterExpression1 }), (IMemberConfigurationExpression<Employee, EmployeeDetailVm, int> opt) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Employee), "s");
				opt.MapFrom<int>(Expression.Lambda<Func<Employee, int>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Employee).GetMethod("get_EmployeeId").MethodHandle)), new ParameterExpression[] { parameterExpression }));
			});
			parameterExpression1 = Expression.Parameter(typeof(EmployeeDetailVm), "d");
			IMappingExpression<Employee, EmployeeDetailVm> mappingExpression2 = mappingExpression1.ForMember<string>(Expression.Lambda<Func<EmployeeDetailVm, string>>(Expression.Property(parameterExpression1, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(EmployeeDetailVm).GetMethod("get_Title").MethodHandle)), new ParameterExpression[] { parameterExpression1 }), (IMemberConfigurationExpression<Employee, EmployeeDetailVm, string> opt) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Employee), "s");
				opt.MapFrom<string>(Expression.Lambda<Func<Employee, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Employee).GetMethod("get_TitleOfCourtesy").MethodHandle)), new ParameterExpression[] { parameterExpression }));
			});
			parameterExpression1 = Expression.Parameter(typeof(EmployeeDetailVm), "d");
			IMappingExpression<Employee, EmployeeDetailVm> mappingExpression3 = mappingExpression2.ForMember<string>(Expression.Lambda<Func<EmployeeDetailVm, string>>(Expression.Property(parameterExpression1, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(EmployeeDetailVm).GetMethod("get_Position").MethodHandle)), new ParameterExpression[] { parameterExpression1 }), (IMemberConfigurationExpression<Employee, EmployeeDetailVm, string> opt) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Employee), "s");
				opt.MapFrom<string>(Expression.Lambda<Func<Employee, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Employee).GetMethod("get_Title").MethodHandle)), new ParameterExpression[] { parameterExpression }));
			});
			parameterExpression1 = Expression.Parameter(typeof(EmployeeDetailVm), "d");
			IMappingExpression<Employee, EmployeeDetailVm> mappingExpression4 = mappingExpression3.ForMember<int?>(Expression.Lambda<Func<EmployeeDetailVm, int?>>(Expression.Property(parameterExpression1, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(EmployeeDetailVm).GetMethod("get_ManagerId").MethodHandle)), new ParameterExpression[] { parameterExpression1 }), (IMemberConfigurationExpression<Employee, EmployeeDetailVm, int?> opt) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Employee), "s");
				opt.MapFrom<int?>(Expression.Lambda<Func<Employee, int?>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Employee).GetMethod("get_ReportsTo").MethodHandle)), new ParameterExpression[] { parameterExpression }));
			});
			parameterExpression1 = Expression.Parameter(typeof(EmployeeDetailVm), "d");
			mappingExpression4.ForMember<List<EmployeeTerritoryDto>>(Expression.Lambda<Func<EmployeeDetailVm, List<EmployeeTerritoryDto>>>(Expression.Property(parameterExpression1, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(EmployeeDetailVm).GetMethod("get_Territories").MethodHandle)), new ParameterExpression[] { parameterExpression1 }), (IMemberConfigurationExpression<Employee, EmployeeDetailVm, List<EmployeeTerritoryDto>> opts) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Employee), "s");
				opts.MapFrom<ICollection<EmployeeTerritory>>(Expression.Lambda<Func<Employee, ICollection<EmployeeTerritory>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Employee).GetMethod("get_EmployeeTerritories").MethodHandle)), new ParameterExpression[] { parameterExpression }));
			});
		}
	}
}