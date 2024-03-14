using Squidex.Domain.Apps.Core.Contents;
using Squidex.Infrastructure.Collections;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Apps.Models
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class WorkflowTransitionDto
	{
		public string Expression
		{
			get;
			set;
		}

		[Nullable(new byte[] { 2, 1 })]
		public string[] Roles
		{
			[return: Nullable(new byte[] { 2, 1 })]
			get;
			set;
		}

		public WorkflowTransitionDto()
		{
		}

		[NullableContext(1)]
		public static WorkflowTransitionDto FromDomain(WorkflowTransition transition)
		{
			string[] array;
			WorkflowTransitionDto workflowTransitionDto = new WorkflowTransitionDto()
			{
				Expression = transition.get_Expression()
			};
			ReadonlyList<string> roles = transition.get_Roles();
			if (roles != null)
			{
				array = roles.ToArray<string>();
			}
			else
			{
				array = null;
			}
			workflowTransitionDto.Roles = array;
			return workflowTransitionDto;
		}

		[NullableContext(1)]
		public WorkflowTransition ToWorkflowTransition()
		{
			return WorkflowTransition.When(this.Expression, this.Roles);
		}
	}
}