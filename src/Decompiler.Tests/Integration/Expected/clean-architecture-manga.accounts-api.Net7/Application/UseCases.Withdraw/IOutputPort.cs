using Domain;
using Domain.Debits;
using System;
using System.Runtime.CompilerServices;

namespace Application.UseCases.Withdraw
{
	[NullableContext(1)]
	public interface IOutputPort
	{
		void Invalid();

		void NotFound();

		void Ok(Debit debit, Account account);

		void OutOfFunds();
	}
}