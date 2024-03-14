using Squidex.Domain.Apps.Entities;
using Squidex.Infrastructure;
using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Squidex.Areas.Api.Controllers
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class QueryDto
	{
		public DomainId[] Ids
		{
			get;
			set;
		}

		[JsonPropertyName("q")]
		public JsonDocument JsonQuery
		{
			get;
			set;
		}

		public string OData
		{
			get;
			set;
		}

		public DomainId? ParentId
		{
			get;
			set;
		}

		public QueryDto()
		{
		}

		[NullableContext(1)]
		public Q ToQuery()
		{
			Q empty = Q.get_Empty();
			if (this.Ids != null)
			{
				empty = empty.WithIds(this.Ids);
			}
			if (this.OData != null)
			{
				empty = empty.WithODataQuery(this.OData);
			}
			if (this.JsonQuery != null)
			{
				JsonElement rootElement = this.JsonQuery.RootElement;
				empty = empty.WithJsonQuery(rootElement.ToString());
			}
			return empty;
		}
	}
}