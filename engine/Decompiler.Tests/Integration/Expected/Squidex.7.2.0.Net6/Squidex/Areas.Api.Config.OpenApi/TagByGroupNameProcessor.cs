using Microsoft.AspNetCore.Mvc;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Config.OpenApi
{
	public sealed class TagByGroupNameProcessor : IOperationProcessor
	{
		public TagByGroupNameProcessor()
		{
		}

		[NullableContext(1)]
		public bool Process(OperationProcessorContext context)
		{
			string groupName;
			ApiExplorerSettingsAttribute customAttribute = context.get_ControllerType().GetCustomAttribute<ApiExplorerSettingsAttribute>();
			if (customAttribute != null)
			{
				groupName = customAttribute.get_GroupName();
			}
			else
			{
				groupName = null;
			}
			string str = groupName;
			if (string.IsNullOrWhiteSpace(str))
			{
				return false;
			}
			context.get_OperationDescription().get_Operation().set_Tags(new List<string>()
			{
				str
			});
			return true;
		}
	}
}