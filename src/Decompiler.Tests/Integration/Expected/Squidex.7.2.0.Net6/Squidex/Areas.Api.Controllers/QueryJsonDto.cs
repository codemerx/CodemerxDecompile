using NJsonSchema.Annotations;
using Squidex.Infrastructure.Json.Objects;
using Squidex.Infrastructure.Queries;
using System;

namespace Squidex.Areas.Api.Controllers
{
	[JsonSchemaFlatten]
	public sealed class QueryJsonDto : Query<JsonValue>
	{
		public QueryJsonDto()
		{
		}
	}
}