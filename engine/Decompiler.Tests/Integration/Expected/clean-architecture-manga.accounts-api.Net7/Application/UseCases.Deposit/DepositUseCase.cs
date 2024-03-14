using Application.Services;
using Domain;
using Domain.Credits;
using Domain.ValueObjects;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Application.UseCases.Deposit
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class DepositUseCase : IDepositUseCase
	{
		private readonly IAccountFactory _accountFactory;

		private readonly IAccountRepository _accountRepository;

		private readonly ICurrencyExchange _currencyExchange;

		private readonly IUnitOfWork _unitOfWork;

		private IOutputPort _outputPort;

		public DepositUseCase(IAccountRepository accountRepository, IUnitOfWork unitOfWork, IAccountFactory accountFactory, ICurrencyExchange currencyExchange)
		{
			this._accountRepository = accountRepository;
			this._unitOfWork = unitOfWork;
			this._accountFactory = accountFactory;
			this._currencyExchange = currencyExchange;
			this._outputPort = new DepositPresenter();
		}

		private async Task Deposit(AccountId accountId, Money amount)
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
				ConfiguredTaskAwaitable<Money> configuredTaskAwaitable1 = this._currencyExchange.Convert(amount, account2.get_Currency()).ConfigureAwait(false);
				Money money = await configuredTaskAwaitable1;
				Money money1 = money;
				money = new Money();
				Credit credit = this._accountFactory.NewCredit(account2, money1, DateTime.get_Now());
				await this.Deposit(account2, credit).ConfigureAwait(false);
				this._outputPort.Ok(credit, account2);
			}
			account1 = null;
			account2 = null;
		}

		private async Task Deposit(Account account, Credit credit)
		{
			account.Deposit(credit);
			ConfiguredTaskAwaitable configuredTaskAwaitable = this._accountRepository.Update(account, credit).ConfigureAwait(false);
			await configuredTaskAwaitable;
			ConfiguredTaskAwaitable<int> configuredTaskAwaitable1 = this._unitOfWork.Save().ConfigureAwait(false);
			await configuredTaskAwaitable1;
		}

		public Task Execute(Guid accountId, decimal amount, string currency)
		{
			return this.Deposit(new AccountId(accountId), new Money(amount, new Currency(currency)));
		}

		public void SetOutputPort(IOutputPort outputPort)
		{
			this._outputPort = outputPort;
		}
	}
}