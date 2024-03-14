using NJsonSchema;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using Squidex.Hosting;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Config.OpenApi
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class CommonProcessor : IDocumentProcessor
	{
		private readonly string version;

		private readonly string logoBackground = "#3f83df";

		private readonly string logoUrl;

		private readonly OpenApiExternalDocumentation documentation;

		public CommonProcessor(ExposedValues exposedValues, IUrlGenerator urlGenerator)
		{
			this.logoBackground = "#3f83df";
			OpenApiExternalDocumentation openApiExternalDocumentation = new OpenApiExternalDocumentation();
			openApiExternalDocumentation.set_Url("https://docs.squidex.io");
			this.documentation = openApiExternalDocumentation;
			base();
			this.logoUrl = urlGenerator.BuildUrl("images/logo-white.png", false);
			if (!exposedValues.TryGetValue("version", out this.version) || this.version == null)
			{
				this.version = "1.0";
			}
		}

		public void Process(DocumentProcessorContext context)
		{
			context.get_Document().get_Info().set_Version(this.version);
			OpenApiInfo info = context.get_Document().get_Info();
			Dictionary<string, object> strs = new Dictionary<string, object>();
			strs["x-logo"] = new { url = this.logoUrl, backgroundStyle = string.Empty, backgroundColor = this.logoBackground };
			info.set_ExtensionData(strs);
			context.get_Document().set_ExternalDocumentation(this.documentation);
		}
	}
}