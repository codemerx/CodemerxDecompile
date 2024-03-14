using MediatR;
using Northwind.Application.Common.Interfaces;
using Northwind.Persistence;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Northwind.Application.System.Commands.SeedSampleData
{
	public class SeedSampleDataCommandHandler : IRequestHandler<SeedSampleDataCommand>, IRequestHandler<SeedSampleDataCommand, Unit>
	{
		private readonly INorthwindDbContext _context;

		private readonly IUserManager _userManager;

		public SeedSampleDataCommandHandler(INorthwindDbContext context, IUserManager userManager)
		{
			this._context = context;
			this._userManager = userManager;
		}

		public async Task<Unit> Handle(SeedSampleDataCommand request, CancellationToken cancellationToken)
		{
			SampleDataSeeder sampleDataSeeder = new SampleDataSeeder(this._context, this._userManager);
			await sampleDataSeeder.SeedAllAsync(cancellationToken);
			Unit value = Unit.Value;
			sampleDataSeeder = null;
			return value;
		}
	}
}