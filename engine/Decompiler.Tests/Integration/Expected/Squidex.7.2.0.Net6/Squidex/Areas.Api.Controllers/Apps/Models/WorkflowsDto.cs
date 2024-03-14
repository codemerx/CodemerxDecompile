using Squidex.Areas.Api.Controllers.Apps;
using Squidex.Domain.Apps.Core.Contents;
using Squidex.Domain.Apps.Entities.Apps;
using Squidex.Domain.Apps.Entities.Contents;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Validation;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.Apps.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class WorkflowsDto : Resource
	{
		[LocalizedRequired]
		public string[] Errors
		{
			get;
			set;
		}

		[LocalizedRequired]
		public WorkflowDto[] Items
		{
			get;
			set;
		}

		public WorkflowsDto()
		{
		}

		private WorkflowsDto CreateLinks(Resources resources)
		{
			var variable = new { app = resources.get_App() };
			base.AddSelfLink(resources.Url<AppWorkflowsController>((AppWorkflowsController x) => "GetWorkflows", variable));
			if (resources.get_CanCreateWorkflow())
			{
				base.AddPostLink("create", resources.Url<AppWorkflowsController>((AppWorkflowsController x) => "PostWorkflow", variable), null);
			}
			return this;
		}

		public static async Task<WorkflowsDto> FromAppAsync(IWorkflowsValidator workflowsValidator, IAppEntity app, Resources resources)
		{
			WorkflowsDto workflowsDto = new WorkflowsDto();
			Workflows workflows = app.get_Workflows();
			workflowsDto.Items = (
				from x in workflows
				select WorkflowDto.FromDomain(x.Key, x.Value) into x
				select x.CreateLinks(resources)).ToArray<WorkflowDto>();
			WorkflowsDto array = workflowsDto;
			IReadOnlyList<string> strs = await workflowsValidator.ValidateAsync(app.get_Id(), app.get_Workflows());
			array.Errors = strs.ToArray<string>();
			WorkflowsDto workflowsDto1 = array.CreateLinks(resources);
			array = null;
			return workflowsDto1;
		}
	}
}