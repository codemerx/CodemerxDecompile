using Squidex.Areas.Api.Controllers.Apps;
using Squidex.Domain.Apps.Core.Apps;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Validation;
using Squidex.Web;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Apps.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class ClientDto : Resource
	{
		public bool AllowAnonymous
		{
			get;
			set;
		}

		public long ApiCallsLimit
		{
			get;
			set;
		}

		public long ApiTrafficLimit
		{
			get;
			set;
		}

		[LocalizedRequired]
		public string Id
		{
			get;
			set;
		}

		[LocalizedRequired]
		public string Name
		{
			get;
			set;
		}

		[Nullable(2)]
		public string Role
		{
			[NullableContext(2)]
			get;
			[NullableContext(2)]
			set;
		}

		[LocalizedRequired]
		public string Secret
		{
			get;
			set;
		}

		public ClientDto()
		{
		}

		public ClientDto CreateLinks(Resources resources)
		{
			var variable = new { app = resources.get_App(), id = this.Id };
			if (resources.get_CanUpdateClient())
			{
				base.AddPutLink("update", resources.Url<AppClientsController>((AppClientsController x) => "PutClient", variable), null);
			}
			if (resources.get_CanDeleteClient())
			{
				base.AddDeleteLink("delete", resources.Url<AppClientsController>((AppClientsController x) => "DeleteClient", variable), null);
			}
			return this;
		}

		public static ClientDto FromClient(string id, AppClient client)
		{
			return SimpleMapper.Map<AppClient, ClientDto>(client, new ClientDto()
			{
				Id = id
			});
		}
	}
}