using Application.Services;
using Domain;
using Domain.Debits;
using Domain.ValueObjects;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Application.UseCases.Withdraw
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class WithdrawUseCase : IWithdrawUseCase
	{
		private readonly IAccountFactory _accountFactory;

		private readonly IAccountRepository _accountRepository;

		private readonly ICurrencyExchange _currencyExchange;

		private readonly IUnitOfWork _unitOfWork;

		private readonly IUserService _userService;

		private IOutputPort _outputPort;

		public WithdrawUseCase(IAccountRepository accountRepository, IUnitOfWork unitOfWork, IAccountFactory accountFactory, IUserService userService, ICurrencyExchange currencyExchange)
		{
			this._accountRepository = accountRepository;
			this._unitOfWork = unitOfWork;
			this._accountFactory = accountFactory;
			this._userService = userService;
			this._currencyExchange = currencyExchange;
			this._outputPort = new WithdrawPresenter();
		}

		public Task Execute(Guid accountId, decimal amount, string currency)
		{
			return this.Withdraw(new AccountId(accountId), new Money(amount, new Currency(currency)));
		}

		public void SetOutputPort(IOutputPort outputPort)
		{
			this._outputPort = outputPort;
		}

		private async Task Withdraw(AccountId accountId, Money withdrawAmount)
		{
			string currentUserId = this._userService.GetCurrentUserId();
			ConfiguredTaskAwaitable<IAccount> configuredTaskAwaitable = this._accountRepository.Find(accountId, currentUserId).ConfigureAwait(false);
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
				ConfiguredTaskAwaitable<Money> configuredTaskAwaitable1 = this._currencyExchange.Convert(withdrawAmount, account2.get_Currency()).ConfigureAwait(false);
				Money money = await configuredTaskAwaitable1;
				Money money1 = money;
				money = new Money();
				Debit debit = this._accountFactory.NewDebit(account2, money1, DateTime.get_Now());
				if (account2.GetCurrentBalance().Subtract(debit.get_Amount()).get_Amount() >= decimal.Zero)
				{
					await this.Withdraw(account2, debit).ConfigureAwait(false);
					this._outputPort.Ok(debit, account2);
				}
				else
				{
					IOutputPort outputPort = this._outputPort;
					if (outputPort != null)
					{
						outputPort.OutOfFunds();
					}
					else
					{
					}
				}
			}
			currentUserId = null;
			account1 = null;
			account2 = null;
		}

		private async Task Withdraw(Account account, Debit debit)
		{
			account.Withdraw(debit);
			ConfiguredTaskAwaitable configuredTaskAwaitable = this._accountRepository.Update(account, debit).ConfigureAwait(false);
			await configuredTaskAwaitable;
			ConfiguredTaskAwaitable<int> configuredTaskAwaitable1 = this._unitOfWork.Save().ConfigureAwait(false);
			await configuredTaskAwaitable1;
		}
	}
}