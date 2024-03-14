using Domain;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Application.UseCases.GetAccounts
{
	[NullableContext(1)]
	public interface IOutputPort
	{
		void Ok(IList<Account> accounts);
	}
}