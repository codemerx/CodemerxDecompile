using NJsonSchema;
using NSwag;
using NSwag.Collections;
using Squidex.Infrastructure;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Contents.Generator
{
	[Nullable(0)]
	[NullableContext(1)]
	internal sealed class OperationsBuilder
	{
		public JsonSchema ContentSchema
		{
			get;
			set;
		}

		public JsonSchema ContentsSchema
		{
			get;
			set;
		}

		public JsonSchema DataSchema
		{
			get;
			set;
		}

		public Builder Parent
		{
			get;
			set;
		}

		public string Path
		{
			get;
			set;
		}

		public string SchemaDisplayName
		{
			get;
			set;
		}

		public string SchemaName
		{
			get;
			set;
		}

		public string SchemaTypeName
		{
			get;
			set;
		}

		public OperationsBuilder()
		{
		}

		public OperationBuilder AddOperation(string method, string path)
		{
			string schemaTypeName = this.SchemaTypeName;
			OpenApiOperation openApiOperation = new OpenApiOperation();
			openApiOperation.set_Tags(new List<string>()
			{
				schemaTypeName
			});
			OpenApiOperation openApiOperation1 = openApiOperation;
			OpenApiPathItem orAddNew = Squidex.Infrastructure.CollectionExtensions.GetOrAddNew<string, OpenApiPathItem>(this.Parent.OpenApiDocument.get_Paths(), string.Concat(this.Path, path));
			orAddNew.set_Item(method, openApiOperation1);
			return new OperationBuilder(this, openApiOperation1);
		}

		public void AddTag(string description)
		{
			OpenApiTag openApiTag = new OpenApiTag();
			openApiTag.set_Name(this.SchemaTypeName);
			openApiTag.set_Description(this.FormatText(description));
			this.Parent.OpenApiDocument.get_Tags().Add(openApiTag);
		}

		[return: Nullable(2)]
		public string FormatText(string text)
		{
			if (text == null)
			{
				return null;
			}
			return text.Replace("[schema]", string.Concat("'", this.SchemaDisplayName, "'"), StringComparison.Ordinal);
		}
	}
}