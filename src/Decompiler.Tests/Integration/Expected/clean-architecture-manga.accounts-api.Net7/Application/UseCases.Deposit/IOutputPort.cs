using Domain;
using Domain.Credits;
using System;
using System.Runtime.CompilerServices;

namespace Application.UseCases.Deposit
{
	[NullableContext(1)]
	public interface IOutputPort
	{
		void Invalid();

		void NotFound();

		void Ok(Credit credit, Account account);
	}
}