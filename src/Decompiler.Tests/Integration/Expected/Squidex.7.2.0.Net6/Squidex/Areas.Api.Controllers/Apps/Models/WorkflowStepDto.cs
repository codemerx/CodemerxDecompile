using Squidex.Domain.Apps.Core.Contents;
using Squidex.Infrastructure.Collections;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Apps.Models
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class WorkflowStepDto
	{
		public string Color
		{
			get;
			set;
		}

		public bool NoUpdate
		{
			get;
			set;
		}

		public string NoUpdateExpression
		{
			get;
			set;
		}

		[Nullable(new byte[] { 2, 1 })]
		public string[] NoUpdateRoles
		{
			[return: Nullable(new byte[] { 2, 1 })]
			get;
			set;
		}

		[LocalizedRequired]
		[Nullable(1)]
		public Dictionary<Status, WorkflowTransitionDto> Transitions
		{
			[NullableContext(1)]
			get;
			[NullableContext(1)]
			set;
		}

		public bool Validate
		{
			get;
			set;
		}

		public WorkflowStepDto()
		{
		}

		[NullableContext(1)]
		public static WorkflowStepDto FromDomain(WorkflowStep step)
		{
			string[] array;
			WorkflowStepDto expression = SimpleMapper.Map<WorkflowStep, WorkflowStepDto>(step, new WorkflowStepDto()
			{
				Transitions = step.get_Transitions().ToDictionary<KeyValuePair<Status, WorkflowTransition>, Status, WorkflowTransitionDto>((KeyValuePair<Status, WorkflowTransition> y) => y.Key, (KeyValuePair<Status, WorkflowTransition> y) => WorkflowTransitionDto.FromDomain(y.Value))
			});
			if (step.get_NoUpdate() != null)
			{
				expression.NoUpdate = true;
				expression.NoUpdateExpression = step.get_NoUpdate().get_Expression();
				WorkflowStepDto workflowStepDto = expression;
				ReadonlyList<string> roles = step.get_NoUpdate().get_Roles();
				if (roles != null)
				{
					array = roles.ToArray<string>();
				}
				else
				{
					array = null;
				}
				workflowStepDto.NoUpdateRoles = array;
			}
			return expression;
		}

		[NullableContext(1)]
		public WorkflowStep ToWorkflowStep()
		{
			ReadonlyDictionary<Status, WorkflowTransition> readonlyDictionary;
			Squidex.Domain.Apps.Core.Contents.NoUpdate noUpdate;
			Dictionary<Status, WorkflowTransitionDto> transitions = this.Transitions;
			if (transitions != null)
			{
				readonlyDictionary = ReadonlyDictionary.ToReadonlyDictionary<KeyValuePair<Status, WorkflowTransitionDto>, Status, WorkflowTransition>(transitions, (KeyValuePair<Status, WorkflowTransitionDto> y) => y.Key, (KeyValuePair<Status, WorkflowTransitionDto> y) => {
					WorkflowTransitionDto value = y.Value;
					if (value != null)
					{
						return value.ToWorkflowTransition();
					}
					return null;
				});
			}
			else
			{
				readonlyDictionary = null;
			}
			string color = this.Color;
			if (this.NoUpdate)
			{
				noUpdate = Squidex.Domain.Apps.Core.Contents.NoUpdate.When(this.NoUpdateExpression, this.NoUpdateRoles);
			}
			else
			{
				noUpdate = null;
			}
			return new WorkflowStep(readonlyDictionary, color, noUpdate, this.Validate);
		}
	}
}