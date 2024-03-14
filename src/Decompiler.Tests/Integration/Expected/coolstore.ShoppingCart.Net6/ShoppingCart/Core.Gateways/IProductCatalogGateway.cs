using ShoppingCart.Core.Dtos;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingCart.Core.Gateways
{
	public interface IProductCatalogGateway
	{
		[return: Nullable(new byte[] { 1, 2 })]
		Task<ProductDto> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = null);
	}
}