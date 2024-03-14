using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Application.UseCases.GetAccount
{
	[NullableContext(1)]
	public interface IGetAccountUseCase
	{
		Task Execute(Guid accountId);

		void SetOutputPort(IOutputPort outputPort);
	}
}