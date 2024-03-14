using NSwag;
using NSwag.Generation.Processors.Security;
using Squidex.Hosting;
using Squidex.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Config.OpenApi
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class SecurityProcessor : SecurityDefinitionAppender
	{
		public SecurityProcessor(IUrlGenerator urlGenerator) : base("squidex-oauth-auth", Enumerable.Empty<string>(), SecurityProcessor.CreateOAuthSchema(urlGenerator))
		{
		}

		private static OpenApiSecurityScheme CreateOAuthSchema(IUrlGenerator urlGenerator)
		{
			OpenApiSecurityScheme openApiSecurityScheme = new OpenApiSecurityScheme();
			openApiSecurityScheme.set_Type(3);
			OpenApiSecurityScheme openApiSecurityScheme1 = openApiSecurityScheme;
			string str = urlGenerator.BuildUrl("//identity-server/connect/token", false);
			openApiSecurityScheme1.set_TokenUrl(str);
			SecurityProcessor.SetupDescription(openApiSecurityScheme1, str);
			SecurityProcessor.SetupFlow(openApiSecurityScheme1);
			SecurityProcessor.SetupScopes(openApiSecurityScheme1);
			return openApiSecurityScheme1;
		}

		private static void SetupDescription(OpenApiSecurityScheme securityScheme, string tokenUrl)
		{
			string str = Resources.OpenApiSecurity.Replace("<TOKEN_URL>", tokenUrl, StringComparison.Ordinal);
			securityScheme.set_Description(str);
		}

		private static void SetupFlow(OpenApiSecurityScheme security)
		{
			security.set_Flow(3);
		}

		private static void SetupScopes(OpenApiSecurityScheme security)
		{
			Dictionary<string, string> strs = new Dictionary<string, string>();
			strs["squidex-api"] = "Read and write access to the API";
			security.set_Scopes(strs);
		}
	}
}