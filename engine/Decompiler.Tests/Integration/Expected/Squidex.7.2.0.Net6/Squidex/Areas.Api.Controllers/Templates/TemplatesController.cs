using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Squidex.Areas.Api.Controllers.Templates.Models;
using Squidex.Domain.Apps.Entities.Apps.Templates;
using Squidex.Infrastructure.Commands;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.Templates
{
	[ApiExplorerSettings(GroupName="Templates")]
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class TemplatesController : ApiController
	{
		private readonly TemplatesClient templatesClient;

		public TemplatesController(ICommandBus commandBus, TemplatesClient templatesClient) : base(commandBus)
		{
			this.templatesClient = templatesClient;
		}

		[ApiPermission(new string[] {  })]
		[HttpGet]
		[ProducesResponseType(typeof(TemplateDetailsDto), 200)]
		[Route("templates/{name}")]
		public async Task<IActionResult> GetTemplate(string name)
		{
			IActionResult actionResult;
			string detailAsync = await this.templatesClient.GetDetailAsync(name, base.get_HttpContext().get_RequestAborted());
			if (detailAsync != null)
			{
				TemplateDetailsDto templateDetailsDto = TemplateDetailsDto.FromDomain(name, detailAsync, base.get_Resources());
				actionResult = this.Ok(templateDetailsDto);
			}
			else
			{
				actionResult = this.NotFound();
			}
			return actionResult;
		}

		[ApiPermission(new string[] {  })]
		[HttpGet]
		[ProducesResponseType(typeof(TemplatesDto), 200)]
		[Route("templates/")]
		public async Task<IActionResult> GetTemplates()
		{
			List<Template> templatesAsync = await this.templatesClient.GetTemplatesAsync(base.get_HttpContext().get_RequestAborted());
			TemplatesDto templatesDto = TemplatesDto.FromDomain(templatesAsync, base.get_Resources());
			return this.Ok(templatesDto);
		}
	}
}