using Squidex.Domain.Apps.Entities.Schemas.Commands;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Schemas.Models
{
	public sealed class SynchronizeSchemaDto : UpsertSchemaDto
	{
		public bool NoFieldDeletion
		{
			get;
			set;
		}

		public bool NoFieldRecreation
		{
			get;
			set;
		}

		public SynchronizeSchemaDto()
		{
		}

		[NullableContext(1)]
		public SynchronizeSchema ToCommand()
		{
			return UpsertSchemaDto.ToCommand<SynchronizeSchema, SynchronizeSchemaDto>(this, new SynchronizeSchema());
		}
	}
}