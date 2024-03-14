using Microsoft.AspNetCore.Authorization;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Config.OpenApi
{
	public sealed class ScopesProcessor : IOperationProcessor
	{
		public ScopesProcessor()
		{
		}

		[NullableContext(1)]
		public bool Process(OperationProcessorContext context)
		{
			OpenApiOperation operation = context.get_OperationDescription().get_Operation();
			if (operation.get_Security() == null)
			{
				List<OpenApiSecurityRequirement> openApiSecurityRequirements = new List<OpenApiSecurityRequirement>();
				operation.set_Security(openApiSecurityRequirements);
			}
			ApiPermissionAttribute customAttribute = context.get_MethodInfo().GetCustomAttribute<ApiPermissionAttribute>();
			if (customAttribute == null)
			{
				AuthorizeAttribute[] array = context.get_MethodInfo().GetCustomAttributes<AuthorizeAttribute>(true).Union<AuthorizeAttribute>(context.get_MethodInfo().DeclaringType.GetCustomAttributes<AuthorizeAttribute>(true)).ToArray<AuthorizeAttribute>();
				if (array.Any<AuthorizeAttribute>())
				{
					List<string> list = (
						from a in array
						where a.get_Roles() != null
						select a).SelectMany<AuthorizeAttribute, string>((AuthorizeAttribute a) => a.get_Roles().Split(',', StringSplitOptions.None)).Distinct<string>().ToList<string>();
					ICollection<OpenApiSecurityRequirement> security = context.get_OperationDescription().get_Operation().get_Security();
					OpenApiSecurityRequirement openApiSecurityRequirement = new OpenApiSecurityRequirement();
					openApiSecurityRequirement["squidex-oauth-auth"] = list;
					security.Add(openApiSecurityRequirement);
				}
			}
			else
			{
				ICollection<OpenApiSecurityRequirement> security1 = context.get_OperationDescription().get_Operation().get_Security();
				OpenApiSecurityRequirement permissionIds = new OpenApiSecurityRequirement();
				permissionIds["squidex-oauth-auth"] = customAttribute.get_PermissionIds();
				security1.Add(permissionIds);
			}
			return true;
		}
	}
}