using FluentValidation;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Northwind.Application.Customers.Commands.DeleteCustomer
{
	public class DeleteCustomerCommandValidator : AbstractValidator<DeleteCustomerCommand>
	{
		public DeleteCustomerCommandValidator()
		{
			DefaultValidatorExtensions.Length<DeleteCustomerCommand>(DefaultValidatorExtensions.NotEmpty<DeleteCustomerCommand, string>(base.RuleFor<string>((DeleteCustomerCommand v) => v.Id)), 5);
		}
	}
}