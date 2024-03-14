using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Squidex.Config;
using Squidex.Domain.Users;
using Squidex.Hosting;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Security;
using Squidex.Shared;
using Squidex.Shared.Identity;
using Squidex.Shared.Users;
using System;
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
	public sealed class CreateAdminInitializer : IInitializable, ISystem
	{
		private readonly IServiceProvider serviceProvider;

		private readonly MyIdentityOptions identityOptions;

		public int Order
		{
			get
			{
				return 0x7fffffff;
			}
		}

		public CreateAdminInitializer(IServiceProvider serviceProvider, IOptions<MyIdentityOptions> identityOptions)
		{
			this.serviceProvider = serviceProvider;
			this.identityOptions = identityOptions.get_Value();
		}

		private PermissionSet CreatePermissions(PermissionSet permissions)
		{
			permissions = permissions.Add("squidex.admin.*");
			foreach (string str in Squidex.Infrastructure.CollectionExtensions.OrEmpty<string>(this.identityOptions.AdminApps))
			{
				permissions = permissions.Add(PermissionIds.ForApp("squidex.apps.{app}.*", str, "*", "*"));
			}
			return permissions;
		}

		public async Task InitializeAsync(CancellationToken ct)
		{
			object obj;
			IdentityModelEventSource.set_ShowPII(this.identityOptions.ShowPII);
			if (this.identityOptions.IsAdminConfigured())
			{
				AsyncServiceScope asyncServiceScope = ServiceProviderServiceExtensions.CreateAsyncScope(this.serviceProvider);
				object obj1 = null;
				try
				{
					IUserService requiredService = ServiceProviderServiceExtensions.GetRequiredService<IUserService>(asyncServiceScope.get_ServiceProvider());
					string adminEmail = this.identityOptions.AdminEmail;
					string adminPassword = this.identityOptions.AdminPassword;
					if (await CreateAdminInitializer.IsEmptyAsync(requiredService) || this.identityOptions.AdminRecreate)
					{
						try
						{
							IUser user = await requiredService.FindByEmailAsync(adminEmail, ct);
							if (user == null)
							{
								PermissionSet permissionSet = this.CreatePermissions(PermissionSet.Empty);
								UserValues userValue = new UserValues();
								userValue.set_Password(adminPassword);
								userValue.set_Permissions(permissionSet);
								userValue.set_DisplayName(adminEmail);
								UserValues userValue1 = userValue;
								await requiredService.CreateAsync(adminEmail, userValue1, false, ct);
							}
							else if (this.identityOptions.AdminRecreate)
							{
								PermissionSet permissionSet1 = this.CreatePermissions(SquidexClaimsExtensions.Permissions(user.get_Claims()));
								UserValues userValue2 = new UserValues();
								userValue2.set_Password(adminPassword);
								userValue2.set_Permissions(permissionSet1);
								UserValues userValue3 = userValue2;
								await requiredService.UpdateAsync(user.get_Id(), userValue3, false, ct);
							}
						}
						catch (Exception exception1)
						{
							Exception exception = exception1;
							ILogger<CreateAdminInitializer> logger = ServiceProviderServiceExtensions.GetRequiredService<ILogger<CreateAdminInitializer>>(this.serviceProvider);
							LoggerExtensions.LogError(logger, exception, "Failed to create administrator.", Array.Empty<object>());
						}
					}
					requiredService = null;
					adminEmail = null;
					adminPassword = null;
				}
				catch
				{
					obj = obj2;
					obj1 = obj;
				}
				await asyncServiceScope.DisposeAsync();
				obj = obj1;
				if (obj != null)
				{
					Exception exception2 = obj as Exception;
					if (exception2 == null)
					{
						throw obj;
					}
					ExceptionDispatchInfo.Capture(exception2).Throw();
				}
				obj1 = null;
				asyncServiceScope = new AsyncServiceScope();
			}
		}

		private static async Task<bool> IsEmptyAsync(IUserService userService)
		{
			IUserService userService1 = userService;
			CancellationToken cancellationToken = new CancellationToken();
			IResultList<IUser> resultList = await userService1.QueryAsync(null, 1, 0, cancellationToken);
			return resultList.get_Total() == (long)0;
		}
	}
}