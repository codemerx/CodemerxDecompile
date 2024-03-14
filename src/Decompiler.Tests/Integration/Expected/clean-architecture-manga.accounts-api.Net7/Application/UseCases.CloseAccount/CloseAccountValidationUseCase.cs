using Application.Services;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Application.UseCases.CloseAccount
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class CloseAccountValidationUseCase : ICloseAccountUseCase
	{
		private readonly Notification _notification;

		private readonly ICloseAccountUseCase _useCase;

		private IOutputPort _outputPort;

		public CloseAccountValidationUseCase(ICloseAccountUseCase useCase, Notification notification)
		{
			this._useCase = useCase;
			this._notification = notification;
			this._outputPort = new CloseAccountPresenter();
		}

		public async Task Execute(Guid accountId)
		{
			if (accountId == Guid.Empty)
			{
				this._notification.Add("accountId", "AccountId is required.");
			}
			if (this._notification.IsValid)
			{
				await this._useCase.Execute(accountId).ConfigureAwait(false);
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