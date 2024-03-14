using Squidex.Domain.Apps.Core.Contents;
using Squidex.Domain.Apps.Entities.Apps.Commands;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Collections;
using Squidex.Infrastructure.Validation;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Apps.Models
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class UpdateWorkflowDto
	{
		[LocalizedRequired]
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

		public UpdateWorkflowDto()
		{
		}

		[NullableContext(1)]
		public UpdateWorkflow ToCommand(DomainId id)
		{
			ReadonlyDictionary<Status, WorkflowStep> readonlyDictionary;
			Status initial = this.Initial;
			Dictionary<Status, WorkflowStepDto> steps = this.Steps;
			if (steps != null)
			{
				readonlyDictionary = ReadonlyDictionary.ToReadonlyDictionary<KeyValuePair<Status, WorkflowStepDto>, Status, WorkflowStep>(steps, (KeyValuePair<Status, WorkflowStepDto> x) => x.Key, (KeyValuePair<Status, WorkflowStepDto> x) => {
					WorkflowStepDto value = x.Value;
					if (value != null)
					{
						return value.ToWorkflowStep();
					}
					return null;
				});
			}
			else
			{
				readonlyDictionary = null;
			}
			Workflow workflow = new Workflow(initial, readonlyDictionary, this.SchemaIds, this.Name);
			UpdateWorkflow updateWorkflow = new UpdateWorkflow();
			updateWorkflow.set_WorkflowId(id);
			updateWorkflow.set_Workflow(workflow);
			return updateWorkflow;
		}
	}
}