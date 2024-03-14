using Squidex.Areas.Api.Controllers.Backups;
using Squidex.Areas.Api.Controllers.EventConsumers;
using Squidex.Areas.Api.Controllers.Languages;
using Squidex.Areas.Api.Controllers.Ping;
using Squidex.Areas.Api.Controllers.Users;
using Squidex.Web;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Users.Models
{
	public sealed class ResourcesDto : Resource
	{
		public ResourcesDto()
		{
		}

		[NullableContext(1)]
		public static ResourcesDto FromDomain(Resources resources)
		{
			ResourcesDto resourcesDto = new ResourcesDto();
			((Resource)resourcesDto).AddGetLink("ping", resources.Url<PingController>((PingController x) => "GetPing", null), null);
			if (resources.get_CanReadEvents())
			{
				((Resource)resourcesDto).AddGetLink("admin/events", resources.Url<EventConsumersController>((EventConsumersController x) => "GetEventConsumers", null), null);
			}
			if (resources.get_CanRestoreBackup())
			{
				((Resource)resourcesDto).AddGetLink("admin/restore", resources.Url<RestoreController>((RestoreController x) => "GetRestoreJob", null), null);
			}
			if (resources.get_CanReadUsers())
			{
				((Resource)resourcesDto).AddGetLink("admin/users", resources.Url<UserManagementController>((UserManagementController x) => "GetUsers", null), null);
			}
			((Resource)resourcesDto).AddGetLink("languages", resources.Url<LanguagesController>((LanguagesController x) => "GetLanguages", null), null);
			return resourcesDto;
		}
	}
}