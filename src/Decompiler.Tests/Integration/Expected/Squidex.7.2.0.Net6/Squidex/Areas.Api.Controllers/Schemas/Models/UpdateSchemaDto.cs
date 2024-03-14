using Squidex.Domain.Apps.Core.Schemas;
using Squidex.Domain.Apps.Entities.Schemas.Commands;
using Squidex.Infrastructure.Collections;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Schemas.Models
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class UpdateSchemaDto
	{
		public string ContentEditorUrl
		{
			get;
			set;
		}

		public string ContentSidebarUrl
		{
			get;
			set;
		}

		public string ContentsSidebarUrl
		{
			get;
			set;
		}

		[LocalizedStringLength(0x3e8)]
		public string Hints
		{
			get;
			set;
		}

		[LocalizedStringLength(100)]
		public string Label
		{
			get;
			set;
		}

		[Nullable(new byte[] { 2, 1 })]
		public ReadonlyList<string> Tags
		{
			[return: Nullable(new byte[] { 2, 1 })]
			get;
			set;
		}

		public bool ValidateOnPublish
		{
			get;
			set;
		}

		public UpdateSchemaDto()
		{
		}

		[NullableContext(1)]
		public UpdateSchema ToCommand()
		{
			SchemaProperties schemaProperty = SimpleMapper.Map<UpdateSchemaDto, SchemaProperties>(this, new SchemaProperties());
			UpdateSchema updateSchema = new UpdateSchema();
			updateSchema.set_Properties(schemaProperty);
			return updateSchema;
		}
	}
}