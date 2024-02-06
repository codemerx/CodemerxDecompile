using NodaTime;
using Squidex.Areas.Api.Controllers.Apps;
using Squidex.Areas.Api.Controllers.Assets;
using Squidex.Areas.Api.Controllers.Backups;
using Squidex.Areas.Api.Controllers.Ping;
using Squidex.Areas.Api.Controllers.Plans;
using Squidex.Areas.Api.Controllers.Rules;
using Squidex.Areas.Api.Controllers.Schemas;
using Squidex.Domain.Apps.Core.Apps;
using Squidex.Domain.Apps.Entities;
using Squidex.Domain.Apps.Entities.Apps;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Json.Objects;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Security;
using Squidex.Infrastructure.Validation;
using Squidex.Shared;
using Squidex.Shared.Identity;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace Squidex.Areas.Api.Controllers.Apps.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class AppDto : Resource
	{
		[Obsolete("Use 'roleProperties' field now.")]
		public bool CanAccessApi
		{
			get;
			set;
		}

		public bool CanAccessContent
		{
			get;
			set;
		}

		public Instant Created
		{
			get;
			set;
		}

		[Nullable(2)]
		public string Description
		{
			[NullableContext(2)]
			get;
			[NullableContext(2)]
			set;
		}

		public DomainId Id
		{
			get;
			set;
		}

		[Nullable(2)]
		public string Label
		{
			[NullableContext(2)]
			get;
			[NullableContext(2)]
			set;
		}

		public Instant LastModified
		{
			get;
			set;
		}

		[LocalizedRegularExpression("^[a-z0-9]+(\\-[a-z0-9]+)*$")]
		[LocalizedRequired]
		public string Name
		{
			get;
			set;
		}

		public IEnumerable<string> Permissions { get; set; } = Array.Empty<string>();

		[Nullable(2)]
		public string RoleName
		{
			[NullableContext(2)]
			get;
			[NullableContext(2)]
			set;
		}

		[LocalizedRequired]
		public JsonObject RoleProperties
		{
			get;
			set;
		}

		public DomainId? TeamId
		{
			get;
			set;
		}

		public long Version
		{
			get;
			set;
		}

		public AppDto()
		{
		}

		private AppDto CreateLinks(IAppEntity app, Resources resources, PermissionSet permissions, bool isContributor)
		{
			var variable = new { app = this.Name };
			base.AddGetLink("ping", resources.Url<PingController>((PingController x) => "GetAppPing", variable), null);
			if (app.get_Image() != null)
			{
				base.AddGetLink("image", resources.Url<AppImageController>((AppImageController x) => "GetImage", variable), null);
			}
			if (isContributor)
			{
				base.AddDeleteLink("leave", resources.Url<AppContributorsController>((AppContributorsController x) => "DeleteMyself", variable), null);
			}
			if (resources.IsAllowed("squidex.apps.{app}.delete", this.Name, "*", "*", permissions))
			{
				base.AddDeleteLink("delete", resources.Url<AppsController>((AppsController x) => "DeleteApp", variable), null);
			}
			if (resources.IsAllowed("squidex.apps.{app}.transfer", this.Name, "*", "*", permissions))
			{
				base.AddPutLink("transfer", resources.Url<AppsController>((AppsController x) => "PutAppTeam", variable), null);
			}
			if (resources.IsAllowed("squidex.apps.{app}.update", this.Name, "*", "*", permissions))
			{
				base.AddPutLink("update", resources.Url<AppsController>((AppsController x) => "PutApp", variable), null);
			}
			if (resources.IsAllowed("squidex.apps.{app}.assets.read", this.Name, "*", "*", permissions))
			{
				base.AddGetLink("assets", resources.Url<AssetsController>((AssetsController x) => "GetAssets", variable), null);
			}
			if (resources.IsAllowed("squidex.apps.{app}.backups.read", this.Name, "*", "*", permissions))
			{
				base.AddGetLink("backups", resources.Url<BackupsController>((BackupsController x) => "GetBackups", variable), null);
			}
			if (resources.IsAllowed("squidex.apps.{app}.clients.read", this.Name, "*", "*", permissions))
			{
				base.AddGetLink("clients", resources.Url<AppClientsController>((AppClientsController x) => "GetClients", variable), null);
			}
			if (resources.IsAllowed("squidex.apps.{app}.contributors.read", this.Name, "*", "*", permissions))
			{
				base.AddGetLink("contributors", resources.Url<AppContributorsController>((AppContributorsController x) => "GetContributors", variable), null);
			}
			if (resources.IsAllowed("squidex.apps.{app}.languages.read", this.Name, "*", "*", permissions))
			{
				base.AddGetLink("languages", resources.Url<AppLanguagesController>((AppLanguagesController x) => "GetLanguages", variable), null);
			}
			if (resources.IsAllowed("squidex.apps.{app}.plans.read", this.Name, "*", "*", permissions))
			{
				base.AddGetLink("plans", resources.Url<AppPlansController>((AppPlansController x) => "GetPlans", variable), null);
			}
			if (resources.IsAllowed("squidex.apps.{app}.roles.read", this.Name, "*", "*", permissions))
			{
				base.AddGetLink("roles", resources.Url<AppRolesController>((AppRolesController x) => "GetRoles", variable), null);
			}
			if (resources.IsAllowed("squidex.apps.{app}.rules.read", this.Name, "*", "*", permissions))
			{
				base.AddGetLink("rules", resources.Url<RulesController>((RulesController x) => "GetRules", variable), null);
			}
			if (resources.IsAllowed("squidex.apps.{app}.schemas.read", this.Name, "*", "*", permissions))
			{
				base.AddGetLink("schemas", resources.Url<SchemasController>((SchemasController x) => "GetSchemas", variable), null);
			}
			if (resources.IsAllowed("squidex.apps.{app}.workflows.read", this.Name, "*", "*", permissions))
			{
				base.AddGetLink("workflows", resources.Url<AppWorkflowsController>((AppWorkflowsController x) => "GetWorkflows", variable), null);
			}
			if (resources.IsAllowed("squidex.apps.{app}.schemas.create", this.Name, "*", "*", permissions))
			{
				base.AddPostLink("schemas/create", resources.Url<SchemasController>((SchemasController x) => "PostSchema", variable), null);
			}
			if (resources.IsAllowed("squidex.apps.{app}.assets.create", this.Name, "*", "*", permissions))
			{
				base.AddPostLink("assets/create", resources.Url<SchemasController>((SchemasController x) => "PostSchema", variable), null);
			}
			if (resources.IsAllowed("squidex.apps.{app}.image", this.Name, "*", "*", permissions))
			{
				base.AddPostLink("image/upload", resources.Url<AppsController>((AppsController x) => "UploadImage", variable), null);
			}
			if (resources.IsAllowed("squidex.apps.{app}.image", this.Name, "*", "*", permissions))
			{
				base.AddDeleteLink("image/delete", resources.Url<AppsController>((AppsController x) => "DeleteImage", variable), null);
			}
			if (resources.IsAllowed("squidex.apps.{app}.asset-scripts.update", this.Name, "*", "*", permissions))
			{
				base.AddDeleteLink("assets/scripts", resources.Url<AppAssetsController>((AppAssetsController x) => "GetAssetScripts", variable), null);
			}
			base.AddGetLink("settings", resources.Url<AppSettingsController>((AppSettingsController x) => "GetSettings", variable), null);
			return this;
		}

		public static AppDto FromDomain(IAppEntity app, string userId, bool isFrontend, Resources resources)
		{
			Role role = null;
			AppDto name = SimpleMapper.Map<IAppEntity, AppDto>(app, new AppDto());
			PermissionSet empty = PermissionSet.Empty;
			bool flag = false;
			if (AppExtensions.TryGetContributorRole(app, userId, isFrontend, ref role))
			{
				flag = true;
				name.RoleName = role.get_Name();
				name.RoleProperties = role.get_Properties();
				name.Permissions = empty.ToIds();
				empty = role.get_Permissions();
			}
			else if (!AppExtensions.TryGetClientRole(app, userId, isFrontend, ref role))
			{
				name.RoleProperties = new JsonObject();
			}
			else
			{
				name.RoleName = role.get_Name();
				name.RoleProperties = role.get_Properties();
				name.Permissions = empty.ToIds();
				empty = role.get_Permissions();
			}
			foreach (ValueTuple<string, string> uIProperty in SquidexClaimsExtensions.GetUIProperties(resources.get_Context().get_UserPrincipal().Claims, app.get_Name()))
			{
				string item1 = uIProperty.Item1;
				string item2 = uIProperty.Item2;
				name.RoleProperties[item1] = JsonValue.Create(item2);
			}
			if (resources.Includes(PermissionIds.ForApp("squidex.apps.{app}.contents.{schema}", app.get_Name(), "*", "*"), empty))
			{
				name.CanAccessContent = true;
			}
			return name.CreateLinks(app, resources, empty, flag);
		}
	}
}