using Application.Services;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Application.UseCases.GetAccount
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class GetAccountValidationUseCase : IGetAccountUseCase
	{
		private readonly Notification _notification;

		private readonly IGetAccountUseCase _useCase;

		private IOutputPort _outputPort;

		public GetAccountValidationUseCase(IGetAccountUseCase useCase, Notification notification)
		{
			this._useCase = useCase;
			this._notification = notification;
			this._outputPort = new GetAccountPresenter();
		}

		public async Task Execute(Guid accountId)
		{
			if (accountId == Guid.Empty)
			{
				this._notification.Add("accountId", "AccountId is required.");
			}
			if (!this._notification.IsInvalid)
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