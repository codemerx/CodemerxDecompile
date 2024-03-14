using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using Squidex.Config;
using Squidex.Domain.Apps.Core.Apps;
using Squidex.Domain.Apps.Entities;
using Squidex.Domain.Apps.Entities.Apps;
using Squidex.Domain.Users;
using Squidex.Domain.Users.InMemory;
using Squidex.Hosting;
using Squidex.Shared.Identity;
using Squidex.Shared.Users;
using Squidex.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Squidex.Areas.IdentityServer.Config
{
	[Nullable(0)]
	[NullableContext(1)]
	public class DynamicApplicationStore : InMemoryApplicationStore
	{
		private readonly IServiceProvider serviceProvider;

		public DynamicApplicationStore(IServiceProvider serviceProvider) : base(DynamicApplicationStore.CreateStaticClients(serviceProvider))
		{
			this.serviceProvider = serviceProvider;
		}

		private static ImmutableApplication CreateClientFromApp(string id, AppClient appClient)
		{
			OpenIddictApplicationDescriptor openIddictApplicationDescriptor = new OpenIddictApplicationDescriptor();
			openIddictApplicationDescriptor.set_DisplayName(id);
			openIddictApplicationDescriptor.set_ClientId(id);
			openIddictApplicationDescriptor.set_ClientSecret(appClient.get_Secret());
			openIddictApplicationDescriptor.get_Permissions().Add("ept:token");
			openIddictApplicationDescriptor.get_Permissions().Add("gt:client_credentials");
			openIddictApplicationDescriptor.get_Permissions().Add("rst:token");
			openIddictApplicationDescriptor.get_Permissions().Add("scp:email");
			openIddictApplicationDescriptor.get_Permissions().Add("scp:profile");
			openIddictApplicationDescriptor.get_Permissions().Add("scp:roles");
			openIddictApplicationDescriptor.get_Permissions().Add("scp:squidex-api");
			openIddictApplicationDescriptor.get_Permissions().Add("scp:permissions");
			return new ImmutableApplication(id, openIddictApplicationDescriptor);
		}

		private static ImmutableApplication CreateClientFromUser(IUser user, string secret)
		{
			string id = user.get_Id();
			OpenIddictApplicationDescriptor openIddictApplicationDescriptor = new OpenIddictApplicationDescriptor();
			openIddictApplicationDescriptor.set_DisplayName(string.Concat(user.get_Email(), " Client"));
			openIddictApplicationDescriptor.set_ClientId(user.get_Id());
			openIddictApplicationDescriptor.set_ClientSecret(secret);
			openIddictApplicationDescriptor.get_Permissions().Add("ept:token");
			openIddictApplicationDescriptor.get_Permissions().Add("gt:client_credentials");
			openIddictApplicationDescriptor.get_Permissions().Add("rst:token");
			openIddictApplicationDescriptor.get_Permissions().Add("scp:email");
			openIddictApplicationDescriptor.get_Permissions().Add("scp:profile");
			openIddictApplicationDescriptor.get_Permissions().Add("scp:roles");
			openIddictApplicationDescriptor.get_Permissions().Add("scp:squidex-api");
			openIddictApplicationDescriptor.get_Permissions().Add("scp:permissions");
			return new ImmutableApplication(id, openIddictApplicationDescriptor.CopyClaims(user));
		}

		[return: Nullable(new byte[] { 1, 0, 1, 1 })]
		private static IEnumerable<ValueTuple<string, OpenIddictApplicationDescriptor>> CreateStaticClients(IServiceProvider serviceProvider)
		{
			MyIdentityOptions value = ServiceProviderServiceExtensions.GetRequiredService<IOptions<MyIdentityOptions>>(serviceProvider).get_Value();
			IUrlGenerator requiredService = ServiceProviderServiceExtensions.GetRequiredService<IUrlGenerator>(serviceProvider);
			string clientFrontendId = Constants.ClientFrontendId;
			OpenIddictApplicationDescriptor openIddictApplicationDescriptor = new OpenIddictApplicationDescriptor();
			openIddictApplicationDescriptor.set_DisplayName("Frontend Client");
			openIddictApplicationDescriptor.set_ClientId(clientFrontendId);
			openIddictApplicationDescriptor.set_ClientSecret(null);
			openIddictApplicationDescriptor.get_RedirectUris().Add(new Uri(requiredService.BuildUrl("login;", true)));
			openIddictApplicationDescriptor.get_RedirectUris().Add(new Uri(requiredService.BuildUrl("client-callback-silent.html", false)));
			openIddictApplicationDescriptor.get_RedirectUris().Add(new Uri(requiredService.BuildUrl("client-callback-popup.html", false)));
			openIddictApplicationDescriptor.get_PostLogoutRedirectUris().Add(new Uri(requiredService.BuildUrl("logout", false)));
			openIddictApplicationDescriptor.get_Permissions().Add("ept:authorization");
			openIddictApplicationDescriptor.get_Permissions().Add("ept:logout");
			openIddictApplicationDescriptor.get_Permissions().Add("ept:token");
			openIddictApplicationDescriptor.get_Permissions().Add("gt:authorization_code");
			openIddictApplicationDescriptor.get_Permissions().Add("gt:refresh_token");
			openIddictApplicationDescriptor.get_Permissions().Add("rst:code");
			openIddictApplicationDescriptor.get_Permissions().Add("scp:email");
			openIddictApplicationDescriptor.get_Permissions().Add("scp:profile");
			openIddictApplicationDescriptor.get_Permissions().Add("scp:roles");
			openIddictApplicationDescriptor.get_Permissions().Add("scp:squidex-api");
			openIddictApplicationDescriptor.get_Permissions().Add("scp:permissions");
			openIddictApplicationDescriptor.set_Type("public");
			yield return new ValueTuple<string, OpenIddictApplicationDescriptor>(clientFrontendId, openIddictApplicationDescriptor);
			string clientInternalId = Constants.ClientInternalId;
			OpenIddictApplicationDescriptor openIddictApplicationDescriptor1 = new OpenIddictApplicationDescriptor();
			openIddictApplicationDescriptor1.set_DisplayName("Internal Client");
			openIddictApplicationDescriptor1.set_ClientId(clientInternalId);
			openIddictApplicationDescriptor1.set_ClientSecret(Constants.ClientInternalSecret);
			openIddictApplicationDescriptor1.get_RedirectUris().Add(new Uri(requiredService.BuildUrl("/signin-internal", false)));
			openIddictApplicationDescriptor1.get_Permissions().Add("ept:authorization");
			openIddictApplicationDescriptor1.get_Permissions().Add("ept:logout");
			openIddictApplicationDescriptor1.get_Permissions().Add("ept:token");
			openIddictApplicationDescriptor1.get_Permissions().Add("gt:implicit");
			openIddictApplicationDescriptor1.get_Permissions().Add("rst:id_token");
			openIddictApplicationDescriptor1.get_Permissions().Add("rst:id_token token");
			openIddictApplicationDescriptor1.get_Permissions().Add("rst:token");
			openIddictApplicationDescriptor1.get_Permissions().Add("scp:email");
			openIddictApplicationDescriptor1.get_Permissions().Add("scp:profile");
			openIddictApplicationDescriptor1.get_Permissions().Add("scp:roles");
			openIddictApplicationDescriptor1.get_Permissions().Add("scp:squidex-api");
			openIddictApplicationDescriptor1.get_Permissions().Add("scp:permissions");
			openIddictApplicationDescriptor1.set_Type("public");
			yield return new ValueTuple<string, OpenIddictApplicationDescriptor>(clientInternalId, openIddictApplicationDescriptor1);
			if (!value.IsAdminClientConfigured())
			{
				yield break;
			}
			string adminClientId = value.AdminClientId;
			OpenIddictApplicationDescriptor openIddictApplicationDescriptor2 = new OpenIddictApplicationDescriptor();
			openIddictApplicationDescriptor2.set_DisplayName("Admin Client");
			openIddictApplicationDescriptor2.set_ClientId(adminClientId);
			openIddictApplicationDescriptor2.set_ClientSecret(value.AdminClientSecret);
			openIddictApplicationDescriptor2.get_Permissions().Add("ept:token");
			openIddictApplicationDescriptor2.get_Permissions().Add("gt:client_credentials");
			openIddictApplicationDescriptor2.get_Permissions().Add("rst:token");
			openIddictApplicationDescriptor2.get_Permissions().Add("scp:email");
			openIddictApplicationDescriptor2.get_Permissions().Add("scp:profile");
			openIddictApplicationDescriptor2.get_Permissions().Add("scp:roles");
			openIddictApplicationDescriptor2.get_Permissions().Add("scp:squidex-api");
			openIddictApplicationDescriptor2.get_Permissions().Add("scp:permissions");
			yield return new ValueTuple<string, OpenIddictApplicationDescriptor>(adminClientId, openIddictApplicationDescriptor2.SetAdmin());
		}

		[return: Nullable(new byte[] { 0, 2 })]
		public override async ValueTask<ImmutableApplication> FindByClientIdAsync(string identifier, CancellationToken cancellationToken)
		{
			ValueTask<ImmutableApplication> valueTask = this.u003cu003en__1(identifier, cancellationToken);
			return await valueTask ?? await this.GetDynamicAsync(identifier);
		}

		[return: Nullable(new byte[] { 0, 2 })]
		public override async ValueTask<ImmutableApplication> FindByIdAsync(string identifier, CancellationToken cancellationToken)
		{
			ValueTask<ImmutableApplication> valueTask = this.u003cu003en__0(identifier, cancellationToken);
			return await valueTask ?? await this.GetDynamicAsync(identifier);
		}

		[return: Nullable(new byte[] { 1, 2 })]
		private async Task<ImmutableApplication> GetDynamicAsync(string clientId)
		{
			ImmutableApplication immutableApplication;
			CancellationToken cancellationToken;
			object obj;
			AppClient valueOrDefault;
			ImmutableApplication immutableApplication1 = null;
			ValueTuple<string, string> clientParts = Extensions.GetClientParts(clientId);
			string item1 = clientParts.Item1;
			string item2 = clientParts.Item2;
			IAppProvider requiredService = ServiceProviderServiceExtensions.GetRequiredService<IAppProvider>(this.serviceProvider);
			if (!string.IsNullOrWhiteSpace(item1) && !string.IsNullOrWhiteSpace(item2))
			{
				cancellationToken = new CancellationToken();
				IAppEntity appAsync = await requiredService.GetAppAsync(item1, true, cancellationToken);
				if (appAsync != null)
				{
					valueOrDefault = CollectionExtensions.GetValueOrDefault<string, AppClient>(appAsync.get_Clients(), item2);
				}
				else
				{
					valueOrDefault = null;
				}
				AppClient appClient = valueOrDefault;
				if (appClient != null)
				{
					immutableApplication = DynamicApplicationStore.CreateClientFromApp(clientId, appClient);
					item2 = null;
					return immutableApplication;
				}
			}
			AsyncServiceScope asyncServiceScope = ServiceProviderServiceExtensions.CreateAsyncScope(this.serviceProvider);
			object obj1 = null;
			int num = 0;
			try
			{
				IUserService userService = ServiceProviderServiceExtensions.GetRequiredService<IUserService>(asyncServiceScope.get_ServiceProvider());
				string str = clientId;
				cancellationToken = new CancellationToken();
				IUser user = await userService.FindByIdAsync(str, cancellationToken);
				if (user != null)
				{
					string str1 = SquidexClaimsExtensions.ClientSecret(user.get_Claims());
					if (string.IsNullOrWhiteSpace(str1))
					{
						goto Label1;
					}
					else
					{
						immutableApplication1 = DynamicApplicationStore.CreateClientFromUser(user, str1);
					}
				}
				else
				{
					immutableApplication1 = null;
				}
				num = 1;
			}
			catch
			{
				obj = obj2;
				obj1 = obj;
			}
		Label1:
			ValueTask valueTask = asyncServiceScope.DisposeAsync();
			await valueTask;
			obj = obj1;
			if (obj != null)
			{
				Exception exception = obj as Exception;
				if (exception == null)
				{
					throw obj;
				}
				ExceptionDispatchInfo.Capture(exception).Throw();
			}
			if (num != 1)
			{
				obj1 = null;
				immutableApplication1 = null;
				asyncServiceScope = new AsyncServiceScope();
				immutableApplication = null;
			}
			else
			{
				immutableApplication = immutableApplication1;
			}
			item2 = null;
			return immutableApplication;
		}
	}
}