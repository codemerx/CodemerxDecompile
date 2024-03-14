using Application.Services;
using Domain;
using Domain.ValueObjects;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Application.UseCases.CloseAccount
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class CloseAccountUseCase : ICloseAccountUseCase
	{
		private readonly IAccountRepository _accountRepository;

		private readonly IUnitOfWork _unitOfWork;

		private readonly IUserService _userService;

		private IOutputPort _outputPort;

		public CloseAccountUseCase(IAccountRepository accountRepository, IUserService userService, IUnitOfWork unitOfWork)
		{
			this._accountRepository = accountRepository;
			this._userService = userService;
			this._unitOfWork = unitOfWork;
			this._outputPort = new CloseAccountPresenter();
		}

		private async Task Close(Account closeAccount)
		{
			ConfiguredTaskAwaitable configuredTaskAwaitable = this._accountRepository.Delete(closeAccount.get_AccountId()).ConfigureAwait(false);
			await configuredTaskAwaitable;
			ConfiguredTaskAwaitable<int> configuredTaskAwaitable1 = this._unitOfWork.Save().ConfigureAwait(false);
			await configuredTaskAwaitable1;
		}

		private async Task CloseAccountInternal(AccountId accountId, string externalUserId)
		{
			ConfiguredTaskAwaitable<IAccount> configuredTaskAwaitable = this._accountRepository.Find(accountId, externalUserId).ConfigureAwait(false);
			IAccount account = await configuredTaskAwaitable;
			IAccount account1 = account;
			account = null;
			Account account2 = account1 as Account;
			if ((object)account2 == (object)null)
			{
				this._outputPort.NotFound();
			}
			else if (account2.IsClosingAllowed())
			{
				await this.Close(account2).ConfigureAwait(false);
				this._outputPort.Ok(account2);
			}
			else
			{
				this._outputPort.HasFunds();
			}
			account1 = null;
			account2 = null;
		}

		public Task Execute(Guid accountId)
		{
			string currentUserId = this._userService.GetCurrentUserId();
			return this.CloseAccountInternal(new AccountId(accountId), currentUserId);
		}

		public void SetOutputPort(IOutputPort outputPort)
		{
			this._outputPort = outputPort;
		}
	}
}