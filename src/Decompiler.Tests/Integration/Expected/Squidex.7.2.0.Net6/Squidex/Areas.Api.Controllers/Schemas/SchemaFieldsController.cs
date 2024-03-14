using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Squidex.Areas.Api.Controllers.Schemas.Models;
using Squidex.Domain.Apps.Entities.Schemas;
using Squidex.Domain.Apps.Entities.Schemas.Commands;
using Squidex.Infrastructure.Commands;
using Squidex.Web;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.Schemas
{
	[ApiExplorerSettings(GroupName="Schemas")]
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class SchemaFieldsController : ApiController
	{
		public SchemaFieldsController(ICommandBus commandBus) : base(commandBus)
		{
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.schemas.{schema}.update" })]
		[HttpDelete]
		[ProducesResponseType(typeof(SchemaDto), 200)]
		[Route("apps/{app}/schemas/{schema}/fields/{id:long}/")]
		public async Task<IActionResult> DeleteField(string app, string schema, long id)
		{
			DeleteField deleteField = new DeleteField();
			deleteField.set_FieldId(id);
			return this.Ok(await this.InvokeCommandAsync(deleteField));
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.schemas.{schema}.update" })]
		[HttpDelete]
		[ProducesResponseType(typeof(SchemaDto), 200)]
		[Route("apps/{app}/schemas/{schema}/fields/{parentId:long}/nested/{id:long}/")]
		public async Task<IActionResult> DeleteNestedField(string app, string schema, long parentId, long id)
		{
			DeleteField deleteField = new DeleteField();
			deleteField.set_ParentFieldId(new long?(parentId));
			deleteField.set_FieldId(id);
			return this.Ok(await this.InvokeCommandAsync(deleteField));
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.schemas.{schema}.update" })]
		[HttpPut]
		[ProducesResponseType(typeof(SchemaDto), 200)]
		[Route("apps/{app}/schemas/{schema}/fields/{id:long}/disable/")]
		public async Task<IActionResult> DisableField(string app, string schema, long id)
		{
			DisableField disableField = new DisableField();
			disableField.set_FieldId(id);
			return this.Ok(await this.InvokeCommandAsync(disableField));
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.schemas.{schema}.update" })]
		[HttpPut]
		[ProducesResponseType(typeof(SchemaDto), 200)]
		[Route("apps/{app}/schemas/{schema}/fields/{parentId:long}/nested/{id:long}/disable/")]
		public async Task<IActionResult> DisableNestedField(string app, string schema, long parentId, long id)
		{
			DisableField disableField = new DisableField();
			disableField.set_ParentFieldId(new long?(parentId));
			disableField.set_FieldId(id);
			return this.Ok(await this.InvokeCommandAsync(disableField));
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.schemas.{schema}.update" })]
		[HttpPut]
		[ProducesResponseType(typeof(SchemaDto), 200)]
		[Route("apps/{app}/schemas/{schema}/fields/{id:long}/enable/")]
		public async Task<IActionResult> EnableField(string app, string schema, long id)
		{
			EnableField enableField = new EnableField();
			enableField.set_FieldId(id);
			return this.Ok(await this.InvokeCommandAsync(enableField));
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.schemas.{schema}.update" })]
		[HttpPut]
		[ProducesResponseType(typeof(SchemaDto), 200)]
		[Route("apps/{app}/schemas/{schema}/fields/{parentId:long}/nested/{id:long}/enable/")]
		public async Task<IActionResult> EnableNestedField(string app, string schema, long parentId, long id)
		{
			EnableField enableField = new EnableField();
			enableField.set_ParentFieldId(new long?(parentId));
			enableField.set_FieldId(id);
			return this.Ok(await this.InvokeCommandAsync(enableField));
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.schemas.{schema}.update" })]
		[HttpPut]
		[ProducesResponseType(typeof(SchemaDto), 200)]
		[Route("apps/{app}/schemas/{schema}/fields/{id:long}/hide/")]
		public async Task<IActionResult> HideField(string app, string schema, long id)
		{
			HideField hideField = new HideField();
			hideField.set_FieldId(id);
			return this.Ok(await this.InvokeCommandAsync(hideField));
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.schemas.{schema}.update" })]
		[HttpPut]
		[ProducesResponseType(typeof(SchemaDto), 200)]
		[Route("apps/{app}/schemas/{schema}/fields/{parentId:long}/nested/{id:long}/hide/")]
		public async Task<IActionResult> HideNestedField(string app, string schema, long parentId, long id)
		{
			HideField hideField = new HideField();
			hideField.set_ParentFieldId(new long?(parentId));
			hideField.set_FieldId(id);
			return this.Ok(await this.InvokeCommandAsync(hideField));
		}

		private async Task<SchemaDto> InvokeCommandAsync(ICommand command)
		{
			ISchemaEntity schemaEntity = await base.get_CommandBus().PublishAsync(command, base.get_HttpContext().get_RequestAborted()).Result<ISchemaEntity>();
			return SchemaDto.FromDomain(schemaEntity, base.get_Resources());
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.schemas.{schema}.update" })]
		[HttpPut]
		[ProducesResponseType(typeof(SchemaDto), 200)]
		[Route("apps/{app}/schemas/{schema}/fields/{id:long}/lock/")]
		public async Task<IActionResult> LockField(string app, string schema, long id)
		{
			LockField lockField = new LockField();
			lockField.set_FieldId(id);
			return this.Ok(await this.InvokeCommandAsync(lockField));
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.schemas.{schema}.update" })]
		[HttpPut]
		[ProducesResponseType(typeof(SchemaDto), 200)]
		[Route("apps/{app}/schemas/{schema}/fields/{parentId:long}/nested/{id:long}/lock/")]
		public async Task<IActionResult> LockNestedField(string app, string schema, long parentId, long id)
		{
			LockField lockField = new LockField();
			lockField.set_ParentFieldId(new long?(parentId));
			lockField.set_FieldId(id);
			return this.Ok(await this.InvokeCommandAsync(lockField));
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.schemas.{schema}.update" })]
		[HttpPost]
		[ProducesResponseType(typeof(SchemaDto), 201)]
		[Route("apps/{app}/schemas/{schema}/fields/")]
		public async Task<IActionResult> PostField(string app, string schema, [FromBody] AddFieldDto request)
		{
			AddField command = request.ToCommand(null);
			SchemaDto schemaDto = await this.InvokeCommandAsync(command);
			IActionResult actionResult = this.CreatedAtAction("GetSchema", "Schemas", new { app = app, schema = schema }, schemaDto);
			return actionResult;
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.schemas.{schema}.update" })]
		[HttpPost]
		[ProducesResponseType(typeof(SchemaDto), 201)]
		[Route("apps/{app}/schemas/{schema}/fields/{parentId:long}/nested/")]
		public async Task<IActionResult> PostNestedField(string app, string schema, long parentId, [FromBody] AddFieldDto request)
		{
			AddField command = request.ToCommand(new long?(parentId));
			SchemaDto schemaDto = await this.InvokeCommandAsync(command);
			IActionResult actionResult = this.CreatedAtAction("GetSchema", "Schemas", new { app = app, schema = schema }, schemaDto);
			return actionResult;
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.schemas.{schema}.update" })]
		[HttpPut]
		[ProducesResponseType(typeof(SchemaDto), 200)]
		[Route("apps/{app}/schemas/{schema}/fields/{id:long}/")]
		public async Task<IActionResult> PutField(string app, string schema, long id, [FromBody] UpdateFieldDto request)
		{
			UpdateField command = request.ToCommand(id, null);
			return this.Ok(await this.InvokeCommandAsync(command));
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.schemas.{schema}.update" })]
		[HttpPut]
		[ProducesResponseType(typeof(SchemaDto), 200)]
		[Route("apps/{app}/schemas/{schema}/fields/{parentId:long}/nested/{id:long}/")]
		public async Task<IActionResult> PutNestedField(string app, string schema, long parentId, long id, [FromBody] UpdateFieldDto request)
		{
			UpdateField command = request.ToCommand(id, new long?(parentId));
			return this.Ok(await this.InvokeCommandAsync(command));
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.schemas.{schema}.update" })]
		[HttpPut]
		[ProducesResponseType(typeof(SchemaDto), 200)]
		[Route("apps/{app}/schemas/{schema}/fields/{parentId:long}/nested/ordering/")]
		public async Task<IActionResult> PutNestedFieldOrdering(string app, string schema, long parentId, [FromBody] ReorderFieldsDto request)
		{
			ReorderFields command = request.ToCommand(new long?(parentId));
			return this.Ok(await this.InvokeCommandAsync(command));
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.schemas.{schema}.update" })]
		[HttpPut]
		[ProducesResponseType(typeof(SchemaDto), 200)]
		[Route("apps/{app}/schemas/{schema}/fields/ordering/")]
		public async Task<IActionResult> PutSchemaFieldOrdering(string app, string schema, [FromBody] ReorderFieldsDto request)
		{
			ReorderFields command = request.ToCommand(null);
			return this.Ok(await this.InvokeCommandAsync(command));
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.schemas.{schema}.update" })]
		[HttpPut]
		[ProducesResponseType(typeof(SchemaDto), 200)]
		[Route("apps/{app}/schemas/{schema}/fields/ui/")]
		public async Task<IActionResult> PutSchemaUIFields(string app, string schema, [FromBody] ConfigureUIFieldsDto request)
		{
			ConfigureUIFields command = request.ToCommand();
			return this.Ok(await this.InvokeCommandAsync(command));
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.schemas.{schema}.update" })]
		[HttpPut]
		[ProducesResponseType(typeof(SchemaDto), 200)]
		[Route("apps/{app}/schemas/{schema}/fields/{id:long}/show/")]
		public async Task<IActionResult> ShowField(string app, string schema, long id)
		{
			ShowField showField = new ShowField();
			showField.set_FieldId(id);
			return this.Ok(await this.InvokeCommandAsync(showField));
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.schemas.{schema}.update" })]
		[HttpPut]
		[ProducesResponseType(typeof(SchemaDto), 200)]
		[Route("apps/{app}/schemas/{schema}/fields/{parentId:long}/nested/{id:long}/show/")]
		public async Task<IActionResult> ShowNestedField(string app, string schema, long parentId, long id)
		{
			ShowField showField = new ShowField();
			showField.set_ParentFieldId(new long?(parentId));
			showField.set_FieldId(id);
			return this.Ok(await this.InvokeCommandAsync(showField));
		}
	}
}