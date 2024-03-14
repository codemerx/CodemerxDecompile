using Domain;
using System;
using System.Runtime.CompilerServices;

namespace Application.UseCases.GetAccount
{
	[NullableContext(1)]
	public interface IOutputPort
	{
		void Invalid();

		void NotFound();

		void Ok(Account account);
	}
}