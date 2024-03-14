using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Application.UseCases.OpenAccount
{
	[NullableContext(1)]
	public interface IOpenAccountUseCase
	{
		Task Execute(decimal amount, string currency);

		void SetOutputPort(IOutputPort outputPort);
	}
}