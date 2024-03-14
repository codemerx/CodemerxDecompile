using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Northwind.Application.Common.Interfaces;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Northwind.Application.Common.Behaviours
{
	public class RequestLogger<TRequest> : IRequestPreProcessor<TRequest>
	{
		private readonly ILogger _logger;

		private readonly ICurrentUserService _currentUserService;

		public RequestLogger(ILogger<TRequest> logger, ICurrentUserService currentUserService)
		{
			this._logger = logger;
			this._currentUserService = currentUserService;
		}

		public Task Process(TRequest request, CancellationToken cancellationToken)
		{
			string name = typeof(TRequest).Name;
			LoggerExtensions.LogInformation(this._logger, "Northwind Request: {Name} {@UserId} {@Request}", new Object[] { name, this._currentUserService.UserId, request });
			return Task.CompletedTask;
		}
	}
}