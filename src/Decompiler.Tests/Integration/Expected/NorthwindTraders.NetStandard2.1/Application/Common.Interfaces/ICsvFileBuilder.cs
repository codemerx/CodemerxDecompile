using Northwind.Application.Products.Queries.GetProductsFile;
using System;
using System.Collections.Generic;

namespace Northwind.Application.Common.Interfaces
{
	public interface ICsvFileBuilder
	{
		byte[] BuildProductsFile(IEnumerable<ProductRecordDto> records);
	}
}