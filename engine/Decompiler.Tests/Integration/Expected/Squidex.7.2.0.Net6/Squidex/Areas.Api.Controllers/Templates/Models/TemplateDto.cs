using Squidex.Areas.Api.Controllers.Templates;
using Squidex.Domain.Apps.Entities.Apps.Templates;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Validation;
using Squidex.Web;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Templates.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class TemplateDto : Resource
	{
		[LocalizedRequired]
		public string Description
		{
			get;
			set;
		}

		public bool IsStarter
		{
			get;
			set;
		}

		[LocalizedRequired]
		public string Name
		{
			get;
			set;
		}

		[LocalizedRequired]
		public string Title
		{
			get;
			set;
		}

		public TemplateDto()
		{
		}

		private TemplateDto CreateLinks(Resources resources)
		{
			var variable = new { name = this.Name };
			base.AddSelfLink(resources.Url<TemplatesController>((TemplatesController c) => "GetTemplate", variable));
			return this;
		}

		public static TemplateDto FromDomain(Template template, Resources resources)
		{
			return SimpleMapper.Map<Template, TemplateDto>(template, new TemplateDto()).CreateLinks(resources);
		}
	}
}