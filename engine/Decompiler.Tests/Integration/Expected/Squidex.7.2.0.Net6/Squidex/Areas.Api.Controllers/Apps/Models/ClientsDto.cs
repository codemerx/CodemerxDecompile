using Squidex.Areas.Api.Controllers.Apps;
using Squidex.Domain.Apps.Core.Apps;
using Squidex.Domain.Apps.Entities.Apps;
using Squidex.Infrastructure.Validation;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Apps.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class ClientsDto : Resource
	{
		[LocalizedRequired]
		public ClientDto[] Items
		{
			get;
			set;
		}

		public ClientsDto()
		{
		}

		private ClientsDto CreateLinks(Resources resources)
		{
			var variable = new { app = resources.get_App() };
			base.AddSelfLink(resources.Url<AppClientsController>((AppClientsController x) => "GetClients", variable));
			if (resources.get_CanCreateClient())
			{
				base.AddPostLink("create", resources.Url<AppClientsController>((AppClientsController x) => "PostClient", variable), null);
			}
			return this;
		}

		public static ClientsDto FromApp(IAppEntity app, Resources resources)
		{
			return (new ClientsDto()
			{
				Items = (
					from x in app.get_Clients()
					select ClientDto.FromClient(x.Key, x.Value) into x
					select x.CreateLinks(resources)).ToArray<ClientDto>()
			}).CreateLinks(resources);
		}
	}
}