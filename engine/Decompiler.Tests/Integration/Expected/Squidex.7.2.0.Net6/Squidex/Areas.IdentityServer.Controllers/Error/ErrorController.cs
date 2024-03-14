using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using Squidex.Areas.IdentityServer.Controllers;
using Squidex.Infrastructure;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.IdentityServer.Controllers.Error
{
	public sealed class ErrorController : IdentityServerController
	{
		public ErrorController()
		{
		}

		[NullableContext(1)]
		[Route("error/")]
		public async Task<IActionResult> Error([Nullable(2)] string errorId = null)
		{
			string errorDescription;
			string error;
			Exception exception;
			object innerException;
			await base.SignInManager.SignOutAsync();
			ErrorVM errorVM = new ErrorVM();
			OpenIddictResponse openIddictServerResponse = OpenIddictServerAspNetCoreHelpers.GetOpenIddictServerResponse(base.get_HttpContext());
			ErrorVM errorVM1 = errorVM;
			if (openIddictServerResponse != null)
			{
				errorDescription = openIddictServerResponse.get_ErrorDescription();
			}
			else
			{
				errorDescription = null;
			}
			errorVM1.ErrorMessage = errorDescription;
			ErrorVM errorVM2 = errorVM;
			if (openIddictServerResponse != null)
			{
				error = openIddictServerResponse.get_Error();
			}
			else
			{
				error = null;
			}
			errorVM2.ErrorCode = error;
			if (string.IsNullOrWhiteSpace(errorVM.ErrorMessage))
			{
				IExceptionHandlerFeature exceptionHandlerFeature = base.get_HttpContext().get_Features().Get<IExceptionHandlerFeature>();
				if (exceptionHandlerFeature != null)
				{
					exception = exceptionHandlerFeature.get_Error();
				}
				else
				{
					exception = null;
				}
				Exception exception1 = exception;
				DomainException domainException = exception1 as DomainException;
				if (domainException == null)
				{
					if (exception1 != null)
					{
						innerException = exception1.InnerException;
					}
					else
					{
						innerException = null;
					}
					DomainException domainException1 = innerException as DomainException;
					if (domainException1 != null)
					{
						errorVM.ErrorMessage = domainException1.Message;
					}
				}
				else
				{
					errorVM.ErrorMessage = domainException.Message;
				}
			}
			return this.View("Error", errorVM);
		}
	}
}