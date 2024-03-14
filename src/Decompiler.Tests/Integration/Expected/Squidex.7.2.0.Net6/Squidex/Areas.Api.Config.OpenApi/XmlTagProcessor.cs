using Microsoft.AspNetCore.Mvc;
using Namotion.Reflection;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Config.OpenApi
{
	public sealed class XmlTagProcessor : IDocumentProcessor
	{
		public XmlTagProcessor()
		{
		}

		[NullableContext(1)]
		public void Process(DocumentProcessorContext context)
		{
			try
			{
				foreach (Type controllerType in context.get_ControllerTypes())
				{
					ApiExplorerSettingsAttribute customAttribute = controllerType.GetCustomAttribute<ApiExplorerSettingsAttribute>();
					if (customAttribute == null)
					{
						continue;
					}
					OpenApiTag openApiTag = context.get_Document().get_Tags().FirstOrDefault<OpenApiTag>((OpenApiTag x) => x.get_Name() == customAttribute.get_GroupName());
					if (openApiTag == null)
					{
						continue;
					}
					string xmlDocsSummary = XmlDocsExtensions.GetXmlDocsSummary(controllerType, null);
					if (xmlDocsSummary == null)
					{
						continue;
					}
					OpenApiTag openApiTag1 = openApiTag;
					if (openApiTag1.get_Description() == null)
					{
						string empty = string.Empty;
						openApiTag1.set_Description(empty);
					}
					if (openApiTag.get_Description().Contains(xmlDocsSummary, StringComparison.Ordinal))
					{
						continue;
					}
					OpenApiTag openApiTag2 = openApiTag;
					openApiTag2.set_Description(string.Concat(openApiTag2.get_Description(), "\n\n", xmlDocsSummary));
				}
			}
			finally
			{
				XmlDocs.ClearCache();
			}
		}
	}
}