using AutoMapper;
using Northwind.Application.Common.Mappings;
using Northwind.Domain.Entities;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Northwind.Application.Customers.Queries.GetCustomersList
{
	public class CustomerLookupDto : IMapFrom<Customer>
	{
		public string Id
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public CustomerLookupDto()
		{
		}

		public void Mapping(Profile profile)
		{
			IMappingExpression<Customer, CustomerLookupDto> mappingExpression = profile.CreateMap<Customer, CustomerLookupDto>();
			ParameterExpression parameterExpression1 = Expression.Parameter(typeof(CustomerLookupDto), "d");
			IMappingExpression<Customer, CustomerLookupDto> mappingExpression1 = mappingExpression.ForMember<string>(Expression.Lambda<Func<CustomerLookupDto, string>>(Expression.Property(parameterExpression1, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(CustomerLookupDto).GetMethod("get_Id").MethodHandle)), new ParameterExpression[] { parameterExpression1 }), (IMemberConfigurationExpression<Customer, CustomerLookupDto, string> opt) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Customer), "s");
				opt.MapFrom<string>(Expression.Lambda<Func<Customer, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Customer).GetMethod("get_CustomerId").MethodHandle)), new ParameterExpression[] { parameterExpression }));
			});
			parameterExpression1 = Expression.Parameter(typeof(CustomerLookupDto), "d");
			mappingExpression1.ForMember<string>(Expression.Lambda<Func<CustomerLookupDto, string>>(Expression.Property(parameterExpression1, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(CustomerLookupDto).GetMethod("get_Name").MethodHandle)), new ParameterExpression[] { parameterExpression1 }), (IMemberConfigurationExpression<Customer, CustomerLookupDto, string> opt) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Customer), "s");
				opt.MapFrom<string>(Expression.Lambda<Func<Customer, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Customer).GetMethod("get_CompanyName").MethodHandle)), new ParameterExpression[] { parameterExpression }));
			});
		}
	}
}