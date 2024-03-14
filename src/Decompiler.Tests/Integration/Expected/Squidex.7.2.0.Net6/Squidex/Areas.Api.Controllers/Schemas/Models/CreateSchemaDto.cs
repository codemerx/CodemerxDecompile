using Squidex.Domain.Apps.Core.Schemas;
using Squidex.Domain.Apps.Entities.Schemas.Commands;
using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Schemas.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class CreateSchemaDto : UpsertSchemaDto
	{
		[Obsolete("Use 'type' field now.")]
		public bool IsSingleton
		{
			get
			{
				return this.Type == 1;
			}
			set
			{
				if (value)
				{
					this.Type = 1;
				}
			}
		}

		[LocalizedRegularExpression("^[a-z0-9]+(\\-[a-z0-9]+)*$")]
		[LocalizedRequired]
		public string Name
		{
			get;
			set;
		}

		public SchemaType Type
		{
			get;
			set;
		}

		public CreateSchemaDto()
		{
		}

		public CreateSchema ToCommand()
		{
			return UpsertSchemaDto.ToCommand<CreateSchema, CreateSchemaDto>(this, new CreateSchema());
		}
	}
}