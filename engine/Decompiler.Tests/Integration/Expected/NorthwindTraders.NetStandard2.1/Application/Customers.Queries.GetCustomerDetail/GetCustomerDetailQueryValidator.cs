using FluentValidation;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Northwind.Application.Customers.Queries.GetCustomerDetail
{
	public class GetCustomerDetailQueryValidator : AbstractValidator<GetCustomerDetailQuery>
	{
		public GetCustomerDetailQueryValidator()
		{
			DefaultValidatorExtensions.Length<GetCustomerDetailQuery>(DefaultValidatorExtensions.NotEmpty<GetCustomerDetailQuery, string>(base.RuleFor<string>((GetCustomerDetailQuery v) => v.Id)), 5);
		}
	}
}