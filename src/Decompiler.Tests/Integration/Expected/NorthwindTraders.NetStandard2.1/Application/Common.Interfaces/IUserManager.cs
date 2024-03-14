using Northwind.Application.Common.Models;
using System;
using System.Threading.Tasks;

namespace Northwind.Application.Common.Interfaces
{
	public interface IUserManager
	{
		[return: TupleElementNames(new string[] { "Result", "UserId" })]
		Task<ValueTuple<Result, string>> CreateUserAsync(string userName, string password);

		Task<Result> DeleteUserAsync(string userId);
	}
}