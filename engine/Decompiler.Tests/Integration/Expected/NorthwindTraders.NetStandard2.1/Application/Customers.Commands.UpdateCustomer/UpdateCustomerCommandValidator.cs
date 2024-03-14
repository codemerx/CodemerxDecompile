using FluentValidation;
using FluentValidation.Validators;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Northwind.Application.Customers.Commands.UpdateCustomer
{
	public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
	{
		public UpdateCustomerCommandValidator()
		{
			DefaultValidatorExtensions.NotEmpty<UpdateCustomerCommand, string>(DefaultValidatorExtensions.MaximumLength<UpdateCustomerCommand>(base.RuleFor<string>((UpdateCustomerCommand x) => x.Id), 5));
			DefaultValidatorExtensions.MaximumLength<UpdateCustomerCommand>(base.RuleFor<string>((UpdateCustomerCommand x) => x.Address), 60);
			DefaultValidatorExtensions.MaximumLength<UpdateCustomerCommand>(base.RuleFor<string>((UpdateCustomerCommand x) => x.City), 15);
			DefaultValidatorExtensions.NotEmpty<UpdateCustomerCommand, string>(DefaultValidatorExtensions.MaximumLength<UpdateCustomerCommand>(base.RuleFor<string>((UpdateCustomerCommand x) => x.CompanyName), 40));
			DefaultValidatorExtensions.MaximumLength<UpdateCustomerCommand>(base.RuleFor<string>((UpdateCustomerCommand x) => x.ContactName), 30);
			DefaultValidatorExtensions.MaximumLength<UpdateCustomerCommand>(base.RuleFor<string>((UpdateCustomerCommand x) => x.ContactTitle), 30);
			DefaultValidatorExtensions.MaximumLength<UpdateCustomerCommand>(base.RuleFor<string>((UpdateCustomerCommand x) => x.Country), 15);
			DefaultValidatorExtensions.NotEmpty<UpdateCustomerCommand, string>(DefaultValidatorExtensions.MaximumLength<UpdateCustomerCommand>(base.RuleFor<string>((UpdateCustomerCommand x) => x.Fax), 24));
			DefaultValidatorExtensions.NotEmpty<UpdateCustomerCommand, string>(DefaultValidatorExtensions.MaximumLength<UpdateCustomerCommand>(base.RuleFor<string>((UpdateCustomerCommand x) => x.Phone), 24));
			DefaultValidatorExtensions.MaximumLength<UpdateCustomerCommand>(base.RuleFor<string>((UpdateCustomerCommand x) => x.PostalCode), 10);
			DefaultValidatorExtensions.MaximumLength<UpdateCustomerCommand>(base.RuleFor<string>((UpdateCustomerCommand x) => x.Region), 15);
			DefaultValidatorOptions.WithMessage<UpdateCustomerCommand, string>(DefaultValidatorOptions.When<UpdateCustomerCommand, string>(DefaultValidatorExtensions.Matches<UpdateCustomerCommand>(base.RuleFor<string>((UpdateCustomerCommand c) => c.PostalCode), "^\\d{4}$"), (UpdateCustomerCommand c) => c.Country == "Australia", 0), "Australian Postcodes have 4 digits");
			DefaultValidatorOptions.WithMessage<UpdateCustomerCommand, string>(DefaultValidatorOptions.When<UpdateCustomerCommand, string>(DefaultValidatorExtensions.Must<UpdateCustomerCommand, string>(base.RuleFor<string>((UpdateCustomerCommand c) => c.Phone), new Func<UpdateCustomerCommand, string, PropertyValidatorContext, bool>(UpdateCustomerCommandValidator.HaveQueenslandLandLine)), (UpdateCustomerCommand c) => (c.Country != "Australia" ? false : c.PostalCode.StartsWith("4")), 0), "Customers in QLD require at least one QLD landline.");
		}

		private static bool HaveQueenslandLandLine(UpdateCustomerCommand model, string phoneValue, PropertyValidatorContext ctx)
		{
			return (model.Phone.StartsWith("07") ? true : model.Fax.StartsWith("07"));
		}
	}
}