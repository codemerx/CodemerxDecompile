using Domain;
using System;
using System.Runtime.CompilerServices;

namespace Application.UseCases.OpenAccount
{
	[NullableContext(1)]
	public interface IOutputPort
	{
		void Invalid();

		void NotFound();

		void Ok(Account account);
	}
}