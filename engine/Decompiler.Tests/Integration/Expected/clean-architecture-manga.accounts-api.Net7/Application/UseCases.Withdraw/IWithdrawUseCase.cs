using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Application.UseCases.Withdraw
{
	[NullableContext(1)]
	public interface IWithdrawUseCase
	{
		Task Execute(Guid accountId, decimal amount, string currency);

		void SetOutputPort(IOutputPort outputPort);
	}
}