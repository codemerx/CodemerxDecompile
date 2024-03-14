using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Application.UseCases.Deposit
{
	[NullableContext(1)]
	public interface IDepositUseCase
	{
		Task Execute(Guid accountId, decimal amount, string currency);

		void SetOutputPort(IOutputPort outputPort);
	}
}