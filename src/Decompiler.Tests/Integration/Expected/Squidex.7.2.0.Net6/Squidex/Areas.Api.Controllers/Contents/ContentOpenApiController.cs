using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag;
using Squidex.Areas.Api.Controllers;
using Squidex.Areas.Api.Controllers.Contents.Generator;
using Squidex.Domain.Apps.Entities;
using Squidex.Domain.Apps.Entities.Schemas;
using Squidex.Infrastructure.Commands;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.Contents
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class ContentOpenApiController : ApiController
	{
		private readonly IAppProvider appProvider;

		private readonly SchemasOpenApiGenerator schemasOpenApiGenerator;

		public ContentOpenApiController(ICommandBus commandBus, IAppProvider appProvider, SchemasOpenApiGenerator schemasOpenApiGenerator) : base(commandBus)
		{
			this.appProvider = appProvider;
			this.schemasOpenApiGenerator = schemasOpenApiGenerator;
		}

		[AllowAnonymous]
		[ApiCosts(0)]
		[HttpGet]
		[Route("content/{app}/docs/")]
		public IActionResult Docs(string app)
		{
			return this.View("Docs", new DocsVM()
			{
				Specification = string.Concat("~/api/content/", app, "/swagger/v1/swagger.json")
			});
		}

		[AllowAnonymous]
		[ApiCosts(0)]
		[HttpGet]
		[Route("content/{app}/docs/flat/")]
		public IActionResult DocsFlat(string app)
		{
			return this.View("Docs", new DocsVM()
			{
				Specification = string.Concat("~/api/content/", app, "/flat/swagger/v1/swagger.json")
			});
		}

		[AllowAnonymous]
		[ApiCosts(0)]
		[HttpGet]
		[Route("content/{app}/flat/swagger/v1/swagger.json")]
		public async Task<IActionResult> GetFlatOpenApi(string app)
		{
			List<ISchemaEntity> schemasAsync = await this.appProvider.GetSchemasAsync(base.get_AppId(), base.get_HttpContext().get_RequestAborted());
			OpenApiDocument openApiDocument = await this.schemasOpenApiGenerator.GenerateAsync(base.get_HttpContext(), base.get_App(), schemasAsync, true);
			return this.Content(openApiDocument.ToJson(), "application/json");
		}

		[AllowAnonymous]
		[ApiCosts(0)]
		[HttpGet]
		[Route("content/{app}/swagger/v1/swagger.json")]
		public async Task<IActionResult> GetOpenApi(string app)
		{
			List<ISchemaEntity> schemasAsync = await this.appProvider.GetSchemasAsync(base.get_AppId(), base.get_HttpContext().get_RequestAborted());
			OpenApiDocument openApiDocument = await this.schemasOpenApiGenerator.GenerateAsync(base.get_HttpContext(), base.get_App(), schemasAsync, false);
			return this.Content(openApiDocument.ToJson(), "application/json");
		}
	}
}