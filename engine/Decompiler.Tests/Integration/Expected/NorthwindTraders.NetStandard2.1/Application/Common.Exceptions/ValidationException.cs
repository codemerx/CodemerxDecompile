using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Northwind.Application.Common.Exceptions
{
	public class ValidationException : Exception
	{
		public IDictionary<string, string[]> Failures
		{
			get;
		}

		public ValidationException() : base("One or more validation failures have occurred.")
		{
			this.Failures = new Dictionary<string, string[]>();
		}

		public ValidationException(List<ValidationFailure> failures) : this()
		{
			IEnumerable<string> strs = (
				from e in failures
				select e.get_PropertyName()).Distinct<string>();
			foreach (string str in strs)
			{
				string[] array = (
					from e in failures
					where e.get_PropertyName() == str
					select e.get_ErrorMessage()).ToArray<string>();
				this.Failures.Add(str, array);
			}
		}
	}
}