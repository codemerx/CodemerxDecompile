using Domain;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Application.UseCases.GetAccounts
{
	public sealed class GetAccountPresenter : IOutputPort
	{
		[Nullable(new byte[] { 2, 1 })]
		public IList<Account> Accounts
		{
			[return: Nullable(new byte[] { 2, 1 })]
			get;
			private set;
		}

		public GetAccountPresenter()
		{
		}

		[NullableContext(1)]
		public void Ok(IList<Account> accounts)
		{
			this.Accounts = accounts;
		}
	}
}