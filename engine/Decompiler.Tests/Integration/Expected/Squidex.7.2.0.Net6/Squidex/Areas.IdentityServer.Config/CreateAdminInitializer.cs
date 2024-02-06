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
			CreateAdminInitializer.u003cInitializeAsyncu003ed__5 variable = new CreateAdminInitializer.u003cInitializeAsyncu003ed__5();
			variable.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			variable.u003cu003e4__this = this;
			variable.ct = ct;
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<CreateAdminInitializer.u003cInitializeAsyncu003ed__5>(ref variable);
			return variable.u003cu003et__builder.Task;
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