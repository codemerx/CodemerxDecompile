using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Application.Services
{
	[NullableContext(1)]
	public interface IUnitOfWork
	{
		Task<int> Save();
	}
}