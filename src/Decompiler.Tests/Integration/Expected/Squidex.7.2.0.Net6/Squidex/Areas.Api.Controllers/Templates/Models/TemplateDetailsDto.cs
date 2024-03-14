using Squidex.Areas.Api.Controllers.Templates;
using Squidex.Infrastructure.Validation;
using Squidex.Web;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Templates.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public class TemplateDetailsDto : Resource
	{
		[LocalizedRequired]
		public string Details
		{
			get;
			set;
		}

		public TemplateDetailsDto()
		{
		}

		private TemplateDetailsDto CreateLinks(string name, Resources resources)
		{
			var variable = new { name = name };
			base.AddSelfLink(resources.Url<TemplatesController>((TemplatesController c) => "GetTemplate", variable));
			return this;
		}

		public static TemplateDetailsDto FromDomain(string name, string details, Resources resources)
		{
			return (new TemplateDetailsDto()
			{
				Details = details
			}).CreateLinks(name, resources);
		}
	}
}