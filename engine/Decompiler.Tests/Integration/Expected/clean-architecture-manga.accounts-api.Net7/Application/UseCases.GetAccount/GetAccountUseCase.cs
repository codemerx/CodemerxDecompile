using Domain;
using Domain.ValueObjects;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Application.UseCases.GetAccount
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class GetAccountUseCase : IGetAccountUseCase
	{
		private readonly IAccountRepository _accountRepository;

		private IOutputPort _outputPort;

		public GetAccountUseCase(IAccountRepository accountRepository)
		{
			this._accountRepository = accountRepository;
			this._outputPort = new GetAccountPresenter();
		}

		public Task Execute(Guid accountId)
		{
			return this.GetAccountInternal(new AccountId(accountId));
		}

		private async Task GetAccountInternal(AccountId accountId)
		{
			ConfiguredTaskAwaitable<IAccount> configuredTaskAwaitable = this._accountRepository.GetAccount(accountId).ConfigureAwait(false);
			IAccount account = await configuredTaskAwaitable;
			IAccount account1 = account;
			account = null;
			Account account2 = account1 as Account;
			if ((object)account2 == (object)null)
			{
				this._outputPort.NotFound();
			}
			else
			{
				this._outputPort.Ok(account2);
			}
			account1 = null;
			account2 = null;
		}

		public void SetOutputPort(IOutputPort outputPort)
		{
			this._outputPort = outputPort;
		}
	}
}