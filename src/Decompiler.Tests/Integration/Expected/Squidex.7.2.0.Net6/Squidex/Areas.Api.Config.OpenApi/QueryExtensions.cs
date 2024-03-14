using NJsonSchema;
using NSwag;
using Squidex.Domain.Apps.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Config.OpenApi
{
	public static class QueryExtensions
	{
		[NullableContext(1)]
		public static void AddQuery(OpenApiOperation operation, bool supportSearch)
		{
			QueryExtensions.u003cu003ec__DisplayClass0_0 variable = new QueryExtensions.u003cu003ec__DisplayClass0_0();
			variable.operation = operation;
			JsonSchema jsonSchema = new JsonSchema();
			jsonSchema.set_Type(64);
			JsonSchema jsonSchema1 = jsonSchema;
			JsonSchema jsonSchema2 = new JsonSchema();
			jsonSchema2.set_Type(16);
			JsonSchema jsonSchema3 = jsonSchema2;
			if (supportSearch)
			{
				OpenApiParameter openApiParameter = new OpenApiParameter();
				openApiParameter.set_Schema(jsonSchema1);
				openApiParameter.set_Name("$search");
				openApiParameter.set_Description(FieldDescriptions.get_QuerySkip());
				QueryExtensions.u003cAddQueryu003eg__AddParameterQueryu007c0_0(openApiParameter, ref variable);
			}
			OpenApiParameter openApiParameter1 = new OpenApiParameter();
			openApiParameter1.set_Schema(jsonSchema3);
			openApiParameter1.set_Name("$top");
			openApiParameter1.set_Description(FieldDescriptions.get_QueryTop());
			QueryExtensions.u003cAddQueryu003eg__AddParameterQueryu007c0_0(openApiParameter1, ref variable);
			OpenApiParameter openApiParameter2 = new OpenApiParameter();
			openApiParameter2.set_Schema(jsonSchema3);
			openApiParameter2.set_Name("$skip");
			openApiParameter2.set_Description(FieldDescriptions.get_QuerySkip());
			QueryExtensions.u003cAddQueryu003eg__AddParameterQueryu007c0_0(openApiParameter2, ref variable);
			OpenApiParameter openApiParameter3 = new OpenApiParameter();
			openApiParameter3.set_Schema(jsonSchema1);
			openApiParameter3.set_Name("$orderby");
			openApiParameter3.set_Description(FieldDescriptions.get_QueryOrderBy());
			QueryExtensions.u003cAddQueryu003eg__AddParameterQueryu007c0_0(openApiParameter3, ref variable);
			OpenApiParameter openApiParameter4 = new OpenApiParameter();
			openApiParameter4.set_Schema(jsonSchema1);
			openApiParameter4.set_Name("$filter");
			openApiParameter4.set_Description(FieldDescriptions.get_QueryFilter());
			QueryExtensions.u003cAddQueryu003eg__AddParameterQueryu007c0_0(openApiParameter4, ref variable);
			OpenApiParameter openApiParameter5 = new OpenApiParameter();
			openApiParameter5.set_Schema(jsonSchema1);
			openApiParameter5.set_Name("q");
			openApiParameter5.set_Description(FieldDescriptions.get_QueryQ());
			QueryExtensions.u003cAddQueryu003eg__AddParameterQueryu007c0_0(openApiParameter5, ref variable);
			OpenApiParameter openApiParameter6 = new OpenApiParameter();
			openApiParameter6.set_Schema(jsonSchema1);
			openApiParameter6.set_Name("ids");
			openApiParameter6.set_Description(FieldDescriptions.get_QueryIds());
			QueryExtensions.u003cAddQueryu003eg__AddParameterQueryu007c0_0(openApiParameter6, ref variable);
		}
	}
}