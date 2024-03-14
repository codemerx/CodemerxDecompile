using Domain.ValueObjects;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Application.Services
{
	[NullableContext(1)]
	public interface ICurrencyExchange
	{
		Task<Money> Convert(Money originalAmount, Currency destinationCurrency);
	}
}