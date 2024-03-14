using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Northwind.Application.Common.Models
{
	public class Result
	{
		public string[] Errors
		{
			get;
			set;
		}

		public bool Succeeded
		{
			get;
			set;
		}

		internal Result(bool succeeded, IEnumerable<string> errors)
		{
			this.Succeeded = succeeded;
			this.Errors = errors.ToArray<string>();
		}

		public static Result Failure(IEnumerable<string> errors)
		{
			return new Result(false, errors);
		}

		public static Result Success()
		{
			return new Result(true, new String[0]);
		}
	}
}