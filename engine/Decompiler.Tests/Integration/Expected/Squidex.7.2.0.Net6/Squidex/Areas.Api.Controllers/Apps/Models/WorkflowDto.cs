using Squidex.Areas.Api.Controllers.Apps;
using Squidex.Domain.Apps.Core.Contents;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Collections;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Validation;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Apps.Models
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class WorkflowDto : Resource
	{
		public DomainId Id
		{
			get;
			set;
		}

		public Status Initial
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public ReadonlyList<DomainId> SchemaIds
		{
			get;
			set;
		}

		[LocalizedRequired]
		[Nullable(1)]
		public Dictionary<Status, WorkflowStepDto> Steps
		{
			[NullableContext(1)]
			get;
			[NullableContext(1)]
			set;
		}

		public WorkflowDto()
		{
		}

		[NullableContext(1)]
		public WorkflowDto CreateLinks(Resources resources)
		{
			var variable = new { app = resources.get_App(), id = this.Id };
			if (resources.get_CanUpdateWorkflow())
			{
				base.AddPutLink("update", resources.Url<AppWorkflowsController>((AppWorkflowsController x) => "PutWorkflow", variable), null);
			}
			if (resources.get_CanDeleteWorkflow())
			{
				base.AddDeleteLink("delete", resources.Url<AppWorkflowsController>((AppWorkflowsController x) => "DeleteWorkflow", variable), null);
			}
			return this;
		}

		[NullableContext(1)]
		public static WorkflowDto FromDomain(DomainId id, Workflow workflow)
		{
			return SimpleMapper.Map<Workflow, WorkflowDto>(workflow, new WorkflowDto()
			{
				Steps = workflow.get_Steps().ToDictionary<KeyValuePair<Status, WorkflowStep>, Status, WorkflowStepDto>((KeyValuePair<Status, WorkflowStep> x) => x.Key, (KeyValuePair<Status, WorkflowStep> x) => WorkflowStepDto.FromDomain(x.Value)),
				Id = id
			});
		}
	}
}