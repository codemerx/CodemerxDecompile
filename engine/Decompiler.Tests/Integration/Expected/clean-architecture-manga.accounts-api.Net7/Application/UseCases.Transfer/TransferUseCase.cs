using Application.Services;
using Domain;
using Domain.Credits;
using Domain.Debits;
using Domain.ValueObjects;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Application.UseCases.Transfer
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class TransferUseCase : ITransferUseCase
	{
		private readonly IAccountFactory _accountFactory;

		private readonly IAccountRepository _accountRepository;

		private readonly ICurrencyExchange _currencyExchange;

		private readonly IUnitOfWork _unitOfWork;

		[Nullable(2)]
		private IOutputPort _outputPort;

		public TransferUseCase(IAccountRepository accountRepository, IUnitOfWork unitOfWork, IAccountFactory accountFactory, ICurrencyExchange currencyExchange)
		{
			this._accountRepository = accountRepository;
			this._unitOfWork = unitOfWork;
			this._accountFactory = accountFactory;
			this._currencyExchange = currencyExchange;
			this._outputPort = new TransferPresenter();
		}

		private async Task Deposit(Account account, Credit credit)
		{
			account.Deposit(credit);
			ConfiguredTaskAwaitable configuredTaskAwaitable = this._accountRepository.Update(account, credit).ConfigureAwait(false);
			await configuredTaskAwaitable;
			ConfiguredTaskAwaitable<int> configuredTaskAwaitable1 = this._unitOfWork.Save().ConfigureAwait(false);
			await configuredTaskAwaitable1;
		}

		public Task Execute(Guid originAccountId, Guid destinationAccountId, decimal amount, string currency)
		{
			return this.Transfer(new AccountId(originAccountId), new AccountId(destinationAccountId), new Money(amount, new Currency(currency)));
		}

		public void SetOutputPort(IOutputPort outputPort)
		{
			this._outputPort = outputPort;
		}

		private async Task Transfer(AccountId originAccountId, AccountId destinationAccountId, Money transferAmount)
		{
			bool flag;
			Account account = null;
			ConfiguredTaskAwaitable<IAccount> configuredTaskAwaitable = this._accountRepository.GetAccount(originAccountId).ConfigureAwait(false);
			IAccount account1 = await configuredTaskAwaitable;
			IAccount account2 = account1;
			account1 = null;
			configuredTaskAwaitable = this._accountRepository.GetAccount(destinationAccountId).ConfigureAwait(false);
			IAccount account3 = await configuredTaskAwaitable;
			IAccount account4 = account3;
			account3 = null;
			Account account5 = account2 as Account;
			if (account5 == null)
			{
				flag = false;
			}
			else
			{
				account = account4 as Account;
				flag = (object)account != (object)null;
			}
			if (!flag)
			{
				IOutputPort outputPort = this._outputPort;
				if (outputPort != null)
				{
					outputPort.NotFound();
				}
				else
				{
				}
			}
			else
			{
				ConfiguredTaskAwaitable<Money> configuredTaskAwaitable1 = this._currencyExchange.Convert(transferAmount, account5.get_Currency()).ConfigureAwait(false);
				Money money = await configuredTaskAwaitable1;
				Money money1 = money;
				money = new Money();
				Debit debit = this._accountFactory.NewDebit(account5, money1, DateTime.get_Now());
				if (account5.GetCurrentBalance().Subtract(debit.get_Amount()).get_Amount() >= decimal.Zero)
				{
					ConfiguredTaskAwaitable configuredTaskAwaitable2 = this.Withdraw(account5, debit).ConfigureAwait(false);
					await configuredTaskAwaitable2;
					configuredTaskAwaitable1 = this._currencyExchange.Convert(transferAmount, account.get_Currency()).ConfigureAwait(false);
					Money money2 = await configuredTaskAwaitable1;
					Money money3 = money2;
					money2 = new Money();
					Credit credit = this._accountFactory.NewCredit(account, money3, DateTime.get_Now());
					configuredTaskAwaitable2 = this.Deposit(account, credit).ConfigureAwait(false);
					await configuredTaskAwaitable2;
					IOutputPort outputPort1 = this._outputPort;
					if (outputPort1 != null)
					{
						outputPort1.Ok(account5, debit, account, credit);
					}
					else
					{
					}
				}
				else
				{
					IOutputPort outputPort2 = this._outputPort;
					if (outputPort2 != null)
					{
						outputPort2.OutOfFunds();
					}
					else
					{
					}
				}
			}
			account2 = null;
			account4 = null;
			account5 = null;
			account = null;
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