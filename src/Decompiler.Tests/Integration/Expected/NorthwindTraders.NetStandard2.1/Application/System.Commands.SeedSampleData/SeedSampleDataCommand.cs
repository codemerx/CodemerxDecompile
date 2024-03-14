using MediatR;
using System;

namespace Northwind.Application.System.Commands.SeedSampleData
{
	public class SeedSampleDataCommand : IRequest, IRequest<Unit>, IBaseRequest
	{
		public SeedSampleDataCommand()
		{
		}
	}
}