using Domain;
using Domain.Credits;
using Domain.Debits;
using System;
using System.Runtime.CompilerServices;

namespace Application.UseCases.Transfer
{
	[NullableContext(1)]
	public interface IOutputPort
	{
		void Invalid();

		void NotFound();

		void Ok(Account originAccount, Debit debit, Account destinationAccount, Credit credit);

		void OutOfFunds();
	}
}