using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Application.UseCases.Transfer
{
	[NullableContext(1)]
	public interface ITransferUseCase
	{
		Task Execute(Guid originAccountId, Guid destinationAccountId, decimal amount, string currency);

		void SetOutputPort(IOutputPort outputPort);
	}
}