using Squidex.Areas.Api.Controllers.Templates;
using Squidex.Domain.Apps.Entities.Apps.Templates;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Templates.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class TemplatesDto : Resource
	{
		public TemplateDto[] Items
		{
			get;
			set;
		}

		public TemplatesDto()
		{
		}

		private TemplatesDto CreateLinks(Resources resources)
		{
			base.AddSelfLink(resources.Url<TemplatesController>((TemplatesController c) => "GetTemplates", null));
			return this;
		}

		public static TemplatesDto FromDomain(IEnumerable<Template> items, Resources resources)
		{
			return (new TemplatesDto()
			{
				Items = (
					from x in items
					select TemplateDto.FromDomain(x, resources)).ToArray<TemplateDto>()
			}).CreateLinks(resources);
		}
	}
}