using Application.Services;
using Domain.ValueObjects;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Application.UseCases.Deposit
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class DepositValidationUseCase : IDepositUseCase
	{
		private readonly Notification _notification;

		private readonly IDepositUseCase _useCase;

		private IOutputPort _outputPort;

		public DepositValidationUseCase(IDepositUseCase useCase, Notification notification)
		{
			this._useCase = useCase;
			this._notification = notification;
			this._outputPort = new DepositPresenter();
		}

		public async Task Execute(Guid accountId, decimal amount, string currency)
		{
			bool flag;
			if (accountId == Guid.Empty)
			{
				this._notification.Add("accountId", "AccountId is required.");
			}
			flag = (!(currency != Currency.Dollar.get_Code()) || !(currency != Currency.Euro.get_Code()) || !(currency != Currency.BritishPound.get_Code()) || !(currency != Currency.Canadian.get_Code()) || !(currency != Currency.Real.get_Code()) ? false : currency != Currency.Krona.get_Code());
			if (flag)
			{
				this._notification.Add("currency", "Currency is required.");
			}
			if (amount <= decimal.Zero)
			{
				this._notification.Add("amount", "Amount should be positive.");
			}
			if (!this._notification.IsInvalid)
			{
				ConfiguredTaskAwaitable configuredTaskAwaitable = this._useCase.Execute(accountId, amount, currency).ConfigureAwait(false);
				await configuredTaskAwaitable;
			}
			else
			{
				this._outputPort.Invalid();
			}
		}

		public void SetOutputPort(IOutputPort outputPort)
		{
			this._outputPort = outputPort;
			this._useCase.SetOutputPort(outputPort);
		}
	}
}