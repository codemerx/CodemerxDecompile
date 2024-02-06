using Squidex.Areas.Api.Controllers.Schemas;
using Squidex.Domain.Apps.Entities.Schemas;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Schemas.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class SchemasDto : Resource
	{
		public SchemaDto[] Items
		{
			get;
			set;
		}

		public SchemasDto()
		{
		}

		private SchemasDto CreateLinks(Resources resources)
		{
			var variable = new { app = resources.get_App() };
			base.AddSelfLink(resources.Url<SchemasController>((SchemasController x) => "GetSchemas", variable));
			if (resources.get_CanCreateSchema())
			{
				base.AddPostLink("create", resources.Url<SchemasController>((SchemasController x) => "PostSchema", variable), null);
			}
			return this;
		}

		public static SchemasDto FromDomain(IList<ISchemaEntity> schemas, Resources resources)
		{
			return (new SchemasDto()
			{
				Items = (
					from x in schemas
					select SchemaDto.FromDomain(x, resources)).ToArray<SchemaDto>()
			}).CreateLinks(resources);
		}
	}
}