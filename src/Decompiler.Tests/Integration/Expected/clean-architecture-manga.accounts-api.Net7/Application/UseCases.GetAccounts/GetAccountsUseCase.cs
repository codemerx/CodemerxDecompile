using Application.Services;
using Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Application.UseCases.GetAccounts
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class GetAccountsUseCase : IGetAccountsUseCase
	{
		private readonly IAccountRepository _accountRepository;

		private readonly IUserService _userService;

		private IOutputPort _outputPort;

		public GetAccountsUseCase(IUserService userService, IAccountRepository accountRepository)
		{
			this._userService = userService;
			this._accountRepository = accountRepository;
			this._outputPort = new GetAccountPresenter();
		}

		public Task Execute()
		{
			return this.GetAccounts(this._userService.GetCurrentUserId());
		}

		private async Task GetAccounts(string externalUserId)
		{
			ConfiguredTaskAwaitable<IList<Account>> configuredTaskAwaitable = this._accountRepository.GetAccounts(externalUserId).ConfigureAwait(false);
			IList<Account> list = await configuredTaskAwaitable;
			IList<Account> list1 = list;
			list = null;
			this._outputPort.Ok(list1);
			list1 = null;
		}

		public void SetOutputPort(IOutputPort outputPort)
		{
			this._outputPort = outputPort;
		}
	}
}