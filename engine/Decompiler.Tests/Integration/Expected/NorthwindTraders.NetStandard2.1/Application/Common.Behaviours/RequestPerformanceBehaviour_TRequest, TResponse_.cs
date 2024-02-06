using MediatR;
using Microsoft.Extensions.Logging;
using Northwind.Application.Common.Interfaces;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Northwind.Application.Common.Behaviours
{
	public class RequestPerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	{
		private readonly Stopwatch _timer;

		private readonly ILogger<TRequest> _logger;

		private readonly ICurrentUserService _currentUserService;

		public RequestPerformanceBehaviour(ILogger<TRequest> logger, ICurrentUserService currentUserService)
		{
			this._timer = new Stopwatch();
			this._logger = logger;
			this._currentUserService = currentUserService;
		}

		public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
		{
			this._timer.Start();
			TResponse tResponse = await next.Invoke();
			TResponse tResponse1 = tResponse;
			tResponse = default(TResponse);
			this._timer.Stop();
			if (this._timer.ElapsedMilliseconds > (long)0x1f4)
			{
				string name = typeof(TRequest).Name;
				ILogger<TRequest> logger = this._logger;
				Object[] elapsedMilliseconds = new Object[] { name, this._timer.ElapsedMilliseconds, this._currentUserService.UserId, request };
				LoggerExtensions.LogWarning(logger, "Northwind Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@Request}", elapsedMilliseconds);
				name = null;
			}
			TResponse tResponse2 = tResponse1;
			tResponse1 = default(TResponse);
			return tResponse2;
		}
	}
}