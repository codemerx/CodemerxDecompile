using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Application.UseCases.CloseAccount
{
	[NullableContext(1)]
	public interface ICloseAccountUseCase
	{
		Task Execute(Guid accountId);

		void SetOutputPort(IOutputPort outputPort);
	}
}