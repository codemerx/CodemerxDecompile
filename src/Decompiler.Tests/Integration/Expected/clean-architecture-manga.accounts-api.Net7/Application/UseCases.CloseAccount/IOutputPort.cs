using Domain;
using System;
using System.Runtime.CompilerServices;

namespace Application.UseCases.CloseAccount
{
	[NullableContext(1)]
	public interface IOutputPort
	{
		void HasFunds();

		void Invalid();

		void NotFound();

		void Ok(Account account);
	}
}