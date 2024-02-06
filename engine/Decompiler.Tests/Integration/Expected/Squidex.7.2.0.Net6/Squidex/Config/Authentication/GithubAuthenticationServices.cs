using AspNet.Security.OAuth.GitHub;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.DependencyInjection;
using Squidex.Config;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Squidex.Config.Authentication
{
	public static class GithubAuthenticationServices
	{
		[NullableContext(1)]
		public static AuthenticationBuilder AddSquidexExternalGithubAuthentication(AuthenticationBuilder authBuilder, MyIdentityOptions identityOptions)
		{
			if (identityOptions.IsGithubAuthConfigured())
			{
				GitHubAuthenticationExtensions.AddGitHub(authBuilder, (GitHubAuthenticationOptions options) => {
					options.set_ClientId(identityOptions.GithubClient);
					options.set_ClientSecret(identityOptions.GithubSecret);
					options.set_Events(new GithubHandler());
					options.get_Scope().Add("user:email");
				});
			}
			return authBuilder;
		}
	}
}