using Application.Services;
using Domain.ValueObjects;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Application.UseCases.Transfer
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class TransferValidationUseCase : ITransferUseCase
	{
		private readonly Notification _notification;

		private readonly ITransferUseCase _useCase;

		private IOutputPort _outputPort;

		public TransferValidationUseCase(ITransferUseCase useCase, Notification notification)
		{
			this._useCase = useCase;
			this._notification = notification;
			this._outputPort = new TransferPresenter();
		}

		public async Task Execute(Guid originAccountId, Guid destinationAccountId, decimal amount, string currency)
		{
			bool flag;
			if (originAccountId == Guid.Empty)
			{
				this._notification.Add("originAccountId", "AccountId is required.");
			}
			if (destinationAccountId == Guid.Empty)
			{
				this._notification.Add("destinationAccountId", "AccountId is required.");
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
				ConfiguredTaskAwaitable configuredTaskAwaitable = this._useCase.Execute(originAccountId, destinationAccountId, amount, currency).ConfigureAwait(false);
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