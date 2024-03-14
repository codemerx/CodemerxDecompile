using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Northwind.Application.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Northwind.Application.Common.Behaviours
{
	public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
	{
		private readonly IEnumerable<IValidator<TRequest>> _validators;

		public RequestValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
		{
			this._validators = validators;
		}

		public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
		{
			ValidationContext validationContext = new ValidationContext((object)request);
			List<ValidationFailure> list = (
				from  in this._validators
				select v.Validate(validationContext)).SelectMany<ValidationResult, ValidationFailure>((ValidationResult result) => result.get_Errors()).Where<ValidationFailure>((ValidationFailure f) => (object)f != (object)null).ToList<ValidationFailure>();
			if (list.Count != 0)
			{
				throw new ValidationException(list);
			}
			return next.Invoke();
		}
	}
}