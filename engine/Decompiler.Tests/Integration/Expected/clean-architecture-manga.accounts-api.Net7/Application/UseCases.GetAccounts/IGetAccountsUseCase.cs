using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Application.UseCases.GetAccounts
{
	[NullableContext(1)]
	public interface IGetAccountsUseCase
	{
		Task Execute();

		void SetOutputPort(IOutputPort outputPort);
	}
}