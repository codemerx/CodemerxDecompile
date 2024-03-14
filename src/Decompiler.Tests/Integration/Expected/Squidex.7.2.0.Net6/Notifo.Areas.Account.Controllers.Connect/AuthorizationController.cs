using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using Squidex.Areas.IdentityServer.Config;
using Squidex.Areas.IdentityServer.Controllers;
using Squidex.Domain.Users;
using Squidex.Infrastructure;
using Squidex.Shared.Users;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Json;
using System.Threading.Tasks;

namespace Notifo.Areas.Account.Controllers.Connect
{
	[Nullable(0)]
	[NullableContext(1)]
	public class AuthorizationController : IdentityServerController
	{
		private readonly IOpenIddictScopeManager scopeManager;

		private readonly IOpenIddictApplicationManager applicationManager;

		private readonly IUserService userService;

		public AuthorizationController(IOpenIddictScopeManager scopeManager, IOpenIddictApplicationManager applicationManager, IUserService userService)
		{
			this.scopeManager = scopeManager;
			this.applicationManager = applicationManager;
			this.userService = userService;
		}

		[HttpGet("connect/authorize")]
		public async Task<IActionResult> Authorize()
		{
			IActionResult actionResult;
			bool isAuthenticated;
			IEnumerable<KeyValuePair<string, StringValues>> keyValuePairs;
			OpenIddictRequest openIddictServerRequest = OpenIddictServerAspNetCoreHelpers.GetOpenIddictServerRequest(base.get_HttpContext());
			if (openIddictServerRequest != null)
			{
				IIdentity identity = base.get_User().Identity;
				if (identity != null)
				{
					isAuthenticated = !identity.IsAuthenticated;
				}
				else
				{
					isAuthenticated = true;
				}
				if (!isAuthenticated)
				{
					IUser async = await this.userService.GetAsync(base.get_User(), base.get_HttpContext().get_RequestAborted());
					if (async != null)
					{
						ClaimsPrincipal claimsPrincipal = await this.CreatePrincipalAsync(openIddictServerRequest, async);
						actionResult = this.SignIn(claimsPrincipal, "OpenIddict.Server.AspNetCore");
					}
					else
					{
						Squidex.Infrastructure.ThrowHelper.InvalidOperationException("The user details cannot be retrieved.");
						actionResult = null;
					}
				}
				else if (!OpenIddictExtensions.HasPrompt(openIddictServerRequest, "none"))
				{
					keyValuePairs = (base.get_Request().get_HasFormContentType() ? base.get_Request().get_Form().ToList<KeyValuePair<string, StringValues>>() : base.get_Request().get_Query().ToList<KeyValuePair<string, StringValues>>());
					QueryString queryString = QueryString.Create(keyValuePairs);
					string pathBase = (base.get_Request().get_PathBase() + base.get_Request().get_Path()) + queryString;
					AuthenticationProperties authenticationProperty = new AuthenticationProperties();
					authenticationProperty.set_RedirectUri(pathBase);
					actionResult = this.Challenge(authenticationProperty);
				}
				else
				{
					Dictionary<string, string> strs = new Dictionary<string, string>();
					strs[".error"] = "login_required";
					strs[".error_description"] = "The user is not logged in.";
					AuthenticationProperties authenticationProperty1 = new AuthenticationProperties(strs);
					string[] strArrays = new string[] { "OpenIddict.Server.AspNetCore" };
					actionResult = this.Forbid(authenticationProperty1, strArrays);
				}
			}
			else
			{
				Squidex.Infrastructure.ThrowHelper.InvalidOperationException("The OpenID Connect request cannot be retrieved.");
				actionResult = null;
			}
			openIddictServerRequest = null;
			return actionResult;
		}

		private async Task<ClaimsPrincipal> CreateApplicationPrincipalAsync(OpenIddictRequest request, object application)
		{
			ClaimsIdentity claimsIdentity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType, "name", "role");
			ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
			if (request.get_ClientId() != null)
			{
				ClaimsIdentity claimsIdentity1 = claimsIdentity;
				string clientId = request.get_ClientId();
				string[] strArrays = new string[] { "access_token", "id_token" };
				OpenIddictExtensions.AddClaim(claimsIdentity1, "sub", clientId, strArrays);
			}
			ValueTask<ImmutableDictionary<string, JsonElement>> propertiesAsync = this.applicationManager.GetPropertiesAsync(application, base.get_HttpContext().get_RequestAborted());
			foreach (Claim claim in ApplicationExtensions.Claims(await propertiesAsync))
			{
				claimsIdentity.AddClaim(claim);
			}
			ClaimsPrincipal claimsPrincipal1 = await this.EnrichPrincipalAsync(claimsPrincipal, request, true);
			claimsIdentity = null;
			claimsPrincipal = null;
			return claimsPrincipal1;
		}

		private async Task<ClaimsPrincipal> CreatePrincipalAsync(OpenIddictRequest request, IUser user)
		{
			ClaimsPrincipal claimsPrincipal = await base.SignInManager.CreateUserPrincipalAsync((IdentityUser)user.get_Identity());
			return await this.EnrichPrincipalAsync(claimsPrincipal, request, false);
		}

		private async Task<ClaimsPrincipal> EnrichPrincipalAsync(ClaimsPrincipal principal, OpenIddictRequest request, bool alwaysDeliverPermissions)
		{
			ImmutableArray<string> scopes = OpenIddictExtensions.GetScopes(request);
			ValueTask<List<string>> listAsync = AsyncEnumerable.ToListAsync<string>(this.scopeManager.ListResourcesAsync(scopes, base.get_HttpContext().get_RequestAborted()), base.get_HttpContext().get_RequestAborted());
			List<string> strs = await listAsync;
			OpenIddictExtensions.SetScopes(principal, scopes);
			OpenIddictExtensions.SetResources(principal, strs);
			foreach (Claim claim in principal.Claims)
			{
				OpenIddictExtensions.SetDestinations(claim, AuthorizationController.GetDestinations(claim, principal, alwaysDeliverPermissions));
			}
			ClaimsPrincipal claimsPrincipal = principal;
			scopes = new ImmutableArray<string>();
			return claimsPrincipal;
		}

		[HttpPost("connect/token")]
		[Produces("application/json", new string[] {  })]
		public async Task<IActionResult> Exchange()
		{
			IActionResult actionResult;
			OpenIddictRequest openIddictServerRequest = OpenIddictServerAspNetCoreHelpers.GetOpenIddictServerRequest(base.get_HttpContext());
			if (openIddictServerRequest == null)
			{
				Squidex.Infrastructure.ThrowHelper.InvalidOperationException("The OpenID Connect request cannot be retrieved.");
				actionResult = null;
			}
			else if (OpenIddictExtensions.IsAuthorizationCodeGrantType(openIddictServerRequest) || OpenIddictExtensions.IsRefreshTokenGrantType(openIddictServerRequest) || OpenIddictExtensions.IsImplicitFlow(openIddictServerRequest))
			{
				ClaimsPrincipal principal = await AuthenticationHttpContextExtensions.AuthenticateAsync(base.get_HttpContext(), "OpenIddict.Server.AspNetCore").get_Principal();
				if (principal != null)
				{
					IUser async = await this.userService.GetAsync(principal, base.get_HttpContext().get_RequestAborted());
					if (async == null)
					{
						Dictionary<string, string> strs = new Dictionary<string, string>();
						strs[".error"] = "invalid_grant";
						strs[".error_description"] = "The token is no longer valid.";
						AuthenticationProperties authenticationProperty = new AuthenticationProperties(strs);
						string[] strArrays = new string[] { "OpenIddict.Server.AspNetCore" };
						actionResult = this.Forbid(authenticationProperty, strArrays);
					}
					else if (await base.SignInManager.CanSignInAsync((IdentityUser)async.get_Identity()))
					{
						foreach (Claim claim in principal.Claims)
						{
							OpenIddictExtensions.SetDestinations(claim, AuthorizationController.GetDestinations(claim, principal, false));
						}
						actionResult = this.SignIn(principal, "OpenIddict.Server.AspNetCore");
					}
					else
					{
						Dictionary<string, string> strs1 = new Dictionary<string, string>();
						strs1[".error"] = "invalid_grant";
						strs1[".error_description"] = "The user is no longer allowed to sign in.";
						AuthenticationProperties authenticationProperty1 = new AuthenticationProperties(strs1);
						string[] strArrays1 = new string[] { "OpenIddict.Server.AspNetCore" };
						actionResult = this.Forbid(authenticationProperty1, strArrays1);
					}
				}
				else
				{
					Squidex.Infrastructure.ThrowHelper.InvalidOperationException("The user details cannot be retrieved.");
					actionResult = null;
				}
			}
			else if (!OpenIddictExtensions.IsClientCredentialsGrantType(openIddictServerRequest))
			{
				Squidex.Infrastructure.ThrowHelper.InvalidOperationException("The specified grant type is not supported.");
				actionResult = null;
			}
			else if (openIddictServerRequest.get_ClientId() != null)
			{
				ValueTask<object> valueTask = this.applicationManager.FindByClientIdAsync(openIddictServerRequest.get_ClientId(), base.get_HttpContext().get_RequestAborted());
				object obj = await valueTask;
				if (obj != null)
				{
					ClaimsPrincipal claimsPrincipal = await this.CreateApplicationPrincipalAsync(openIddictServerRequest, obj);
					actionResult = this.SignIn(claimsPrincipal, "OpenIddict.Server.AspNetCore");
				}
				else
				{
					Squidex.Infrastructure.ThrowHelper.InvalidOperationException("The application details cannot be found in the database.");
					actionResult = null;
				}
			}
			else
			{
				Squidex.Infrastructure.ThrowHelper.InvalidOperationException("The OpenID Connect request cannot be retrieved.");
				actionResult = null;
			}
			openIddictServerRequest = null;
			return actionResult;
		}

		private static IEnumerable<string> GetDestinations(Claim claim, ClaimsPrincipal principal, bool alwaysDeliverPermissions)
		{
			string type = claim.Type;
			if (type == "urn:squidex:name")
			{
				yield return "id_token";
				yield break;
			}
			if (type == "urn:squidex:picture")
			{
				if (OpenIddictExtensions.HasScope(principal, "profile"))
				{
					yield return "id_token";
					yield break;
				}
			}
			else if (type == "urn:squidex:notifo")
			{
				if (OpenIddictExtensions.HasScope(principal, "profile"))
				{
					yield return "id_token";
					yield break;
				}
			}
			else if (type != "urn:squidex:permissions")
			{
				if (type == "name")
				{
					yield return "access_token";
					if (OpenIddictExtensions.HasScope(principal, "profile"))
					{
						yield return "id_token";
					}
					yield break;
				}
				if (type == "email")
				{
					yield return "access_token";
					if (OpenIddictExtensions.HasScope(principal, "email"))
					{
						yield return "id_token";
					}
					yield break;
				}
				if (type == "role")
				{
					yield return "access_token";
					if (OpenIddictExtensions.HasScope(principal, "roles"))
					{
						yield return "id_token";
					}
					yield break;
				}
			}
			else if (OpenIddictExtensions.HasScope(principal, "permissions") | alwaysDeliverPermissions)
			{
				yield return "access_token";
				yield return "id_token";
				yield break;
			}
		}

		[HttpGet("connect/logout")]
		public async Task<IActionResult> Logout()
		{
			await base.SignInManager.SignOutAsync();
			return this.SignOut(new string[] { "OpenIddict.Server.AspNetCore" });
		}
	}
}