using OpenIddict.Abstractions;
using Squidex.Infrastructure.Security;
using Squidex.Shared.Users;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text.Json;

namespace Squidex.Areas.IdentityServer.Config
{
	[Nullable(0)]
	[NullableContext(1)]
	public static class ApplicationExtensions
	{
		public static IEnumerable<Claim> Claims(IReadOnlyDictionary<string, JsonElement> properties)
		{
			// 
			// Current member / type: System.Collections.Generic.IEnumerable`1<System.Security.Claims.Claim> Squidex.Areas.IdentityServer.Config.ApplicationExtensions::Claims(System.Collections.Generic.IReadOnlyDictionary`2<System.String,System.Text.Json.JsonElement>)
			// Exception in: System.Collections.Generic.IEnumerable<System.Security.Claims.Claim> Claims(System.Collections.Generic.IReadOnlyDictionary<System.String,System.Text.Json.JsonElement>)
			// Invalid state value
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		public static OpenIddictApplicationDescriptor CopyClaims(this OpenIddictApplicationDescriptor application, IUser claims)
		{
			foreach (IGrouping<string, Claim> strs in 
				from x in claims.get_Claims()
				group x by x.Type)
			{
				application.get_Properties()[strs.Key] = ApplicationExtensions.CreateParameter(
					from x in strs
					select x.Value);
			}
			return application;
		}

		private static JsonElement CreateParameter(IEnumerable<string> values)
		{
			return (JsonElement)(new OpenIddictParameter?(new OpenIddictParameter(values.ToArray<string>())));
		}

		public static OpenIddictApplicationDescriptor SetAdmin(this OpenIddictApplicationDescriptor application)
		{
			application.get_Properties()["urn:squidex:permissions"] = ApplicationExtensions.CreateParameter(Enumerable.Repeat<string>("squidex.*", 1));
			return application;
		}
	}
}