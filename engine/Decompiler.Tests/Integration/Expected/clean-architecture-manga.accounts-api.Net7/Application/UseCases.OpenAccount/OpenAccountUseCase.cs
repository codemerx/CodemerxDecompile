using Application.Services;
using Domain;
using Domain.Credits;
using Domain.ValueObjects;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Application.UseCases.OpenAccount
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class OpenAccountUseCase : IOpenAccountUseCase
	{
		private readonly IAccountFactory _accountFactory;

		private readonly IAccountRepository _accountRepository;

		private readonly IUnitOfWork _unitOfWork;

		private readonly IUserService _userService;

		private IOutputPort _outputPort;

		public OpenAccountUseCase(IAccountRepository accountRepository, IUnitOfWork unitOfWork, IUserService userService, IAccountFactory accountFactory)
		{
			this._accountRepository = accountRepository;
			this._unitOfWork = unitOfWork;
			this._userService = userService;
			this._accountFactory = accountFactory;
			this._outputPort = new OpenAccountPresenter();
		}

		private async Task Deposit(Account account, Credit credit)
		{
			account.Deposit(credit);
			ConfiguredTaskAwaitable configuredTaskAwaitable = this._accountRepository.Add(account, credit).ConfigureAwait(false);
			await configuredTaskAwaitable;
			ConfiguredTaskAwaitable<int> configuredTaskAwaitable1 = this._unitOfWork.Save().ConfigureAwait(false);
			await configuredTaskAwaitable1;
		}

		public Task Execute(decimal amount, string currency)
		{
			return this.OpenAccount(new Money(amount, new Currency(currency)));
		}

		private async Task OpenAccount(Money amountToDeposit)
		{
			string currentUserId = this._userService.GetCurrentUserId();
			Account account = this._accountFactory.NewAccount(currentUserId, amountToDeposit.get_Currency());
			Credit credit = this._accountFactory.NewCredit(account, amountToDeposit, DateTime.get_Now());
			await this.Deposit(account, credit).ConfigureAwait(false);
			IOutputPort outputPort = this._outputPort;
			if (outputPort != null)
			{
				outputPort.Ok(account);
			}
			else
			{
			}
			currentUserId = null;
			account = null;
			credit = null;
		}

		public void SetOutputPort(IOutputPort outputPort)
		{
			this._outputPort = outputPort;
		}
	}
}