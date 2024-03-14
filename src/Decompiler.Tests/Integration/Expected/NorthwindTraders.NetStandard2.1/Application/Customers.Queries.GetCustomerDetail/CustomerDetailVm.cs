using AutoMapper;
using Northwind.Application.Common.Mappings;
using Northwind.Domain.Entities;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Northwind.Application.Customers.Queries.GetCustomerDetail
{
	public class CustomerDetailVm : IMapFrom<Customer>
	{
		public string Address
		{
			get;
			set;
		}

		public string City
		{
			get;
			set;
		}

		public string CompanyName
		{
			get;
			set;
		}

		public string ContactName
		{
			get;
			set;
		}

		public string ContactTitle
		{
			get;
			set;
		}

		public string Country
		{
			get;
			set;
		}

		public string Fax
		{
			get;
			set;
		}

		public string Id
		{
			get;
			set;
		}

		public string Phone
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

		public CustomerDetailVm()
		{
		}

		public void Mapping(Profile profile)
		{
			IMappingExpression<Customer, CustomerDetailVm> mappingExpression = profile.CreateMap<Customer, CustomerDetailVm>();
			ParameterExpression parameterExpression1 = Expression.Parameter(typeof(CustomerDetailVm), "d");
			mappingExpression.ForMember<string>(Expression.Lambda<Func<CustomerDetailVm, string>>(Expression.Property(parameterExpression1, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(CustomerDetailVm).GetMethod("get_Id").MethodHandle)), new ParameterExpression[] { parameterExpression1 }), (IMemberConfigurationExpression<Customer, CustomerDetailVm, string> opt) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Customer), "s");
				opt.MapFrom<string>(Expression.Lambda<Func<Customer, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Customer).GetMethod("get_CustomerId").MethodHandle)), new ParameterExpression[] { parameterExpression }));
			});
		}
	}
}