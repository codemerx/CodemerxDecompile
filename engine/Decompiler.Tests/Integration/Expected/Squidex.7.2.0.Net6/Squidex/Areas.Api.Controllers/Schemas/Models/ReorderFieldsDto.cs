using Squidex.Domain.Apps.Entities.Schemas.Commands;
using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Schemas.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class ReorderFieldsDto
	{
		[LocalizedRequired]
		public long[] FieldIds
		{
			get;
			set;
		}

		public ReorderFieldsDto()
		{
		}

		public ReorderFields ToCommand(long? parentId = null)
		{
			ReorderFields reorderField = new ReorderFields();
			reorderField.set_ParentFieldId(parentId);
			reorderField.set_FieldIds(this.FieldIds);
			return reorderField;
		}
	}
}