using FluentValidation;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Northwind.Application.Customers.Commands.CreateCustomer
{
	public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
	{
		public CreateCustomerCommandValidator()
		{
			DefaultValidatorExtensions.NotEmpty<CreateCustomerCommand, string>(DefaultValidatorExtensions.Length<CreateCustomerCommand>(base.RuleFor<string>((CreateCustomerCommand x) => x.Id), 5));
			DefaultValidatorExtensions.MaximumLength<CreateCustomerCommand>(base.RuleFor<string>((CreateCustomerCommand x) => x.Address), 60);
			DefaultValidatorExtensions.MaximumLength<CreateCustomerCommand>(base.RuleFor<string>((CreateCustomerCommand x) => x.City), 15);
			DefaultValidatorExtensions.NotEmpty<CreateCustomerCommand, string>(DefaultValidatorExtensions.MaximumLength<CreateCustomerCommand>(base.RuleFor<string>((CreateCustomerCommand x) => x.CompanyName), 40));
			DefaultValidatorExtensions.MaximumLength<CreateCustomerCommand>(base.RuleFor<string>((CreateCustomerCommand x) => x.ContactName), 30);
			DefaultValidatorExtensions.MaximumLength<CreateCustomerCommand>(base.RuleFor<string>((CreateCustomerCommand x) => x.ContactTitle), 30);
			DefaultValidatorExtensions.MaximumLength<CreateCustomerCommand>(base.RuleFor<string>((CreateCustomerCommand x) => x.Country), 15);
			DefaultValidatorExtensions.MaximumLength<CreateCustomerCommand>(base.RuleFor<string>((CreateCustomerCommand x) => x.Fax), 24);
			DefaultValidatorExtensions.MaximumLength<CreateCustomerCommand>(base.RuleFor<string>((CreateCustomerCommand x) => x.Phone), 24);
			DefaultValidatorExtensions.NotEmpty<CreateCustomerCommand, string>(DefaultValidatorExtensions.MaximumLength<CreateCustomerCommand>(base.RuleFor<string>((CreateCustomerCommand x) => x.PostalCode), 10));
			DefaultValidatorExtensions.MaximumLength<CreateCustomerCommand>(base.RuleFor<string>((CreateCustomerCommand x) => x.Region), 15);
		}
	}
}