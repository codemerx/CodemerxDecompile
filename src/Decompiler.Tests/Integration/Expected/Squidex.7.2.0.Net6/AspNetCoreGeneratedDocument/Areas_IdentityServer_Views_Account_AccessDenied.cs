using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Hosting;
using Microsoft.CSharp.RuntimeBinder;
using Squidex.Infrastructure.Translations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AspNetCoreGeneratedDocument
{
	[CreateNewOnMetadataUpdate]
	[Nullable(new byte[] { 0, 1 })]
	[NullableContext(1)]
	[RazorCompiledItemMetadata("Identifier", "/Areas/IdentityServer/Views/Account/AccessDenied.cshtml")]
	[RazorSourceChecksum("SHA256", "96902ca7bc9007282c2aa6ba41ece2d109a87d8ca7bd90eaeb5d11ba945ca083", "/Areas/IdentityServer/Views/_ViewImports.cshtml")]
	[RazorSourceChecksum("SHA256", "bb25fc20f1079b707b80bdc72bb08f73b0b83d7b092a80b4c4a382f163c1f20b", "/Areas/IdentityServer/Views/Account/AccessDenied.cshtml")]
	internal sealed class Areas_IdentityServer_Views_Account_AccessDenied : RazorPage<object>
	{
		[RazorInject]
		public IViewComponentHelper Component
		{
			get;
			private set;
		}

		[RazorInject]
		public IHtmlHelper<dynamic> Html
		{
			get;
			private set;
		}

		[RazorInject]
		public IJsonHelper Json
		{
			get;
			private set;
		}

		[RazorInject]
		public IModelExpressionProvider ModelExpressionProvider
		{
			get;
			private set;
		}

		[RazorInject]
		public IUrlHelper Url
		{
			get;
			private set;
		}

		public Areas_IdentityServer_Views_Account_AccessDenied()
		{
		}

		[NullableContext(0)]
		public override async Task ExecuteAsync()
		{
			((dynamic)base.get_ViewBag()).Title = T.Get("users.accessDenied.title", null);
			this.WriteLiteral("\n<img class=\"splash-image\" src=\"squid.svg?title=STOP%20HERE&text=You%20shall%20not%20pass!\" />\n\n<h1 class=\"splash-h1\">");
			this.Write(T.Get("users.accessDenied.title", null));
			this.WriteLiteral("</h1>\n\n<p class=\"splash-text\">\n    ");
			this.Write(T.Get("users.accessDenied.text", null));
			this.WriteLiteral("\n</p>\n\n<p class=\"splash-text\">\n    <a href=\"account/logout-redirect\">");
			this.Write(T.Get("common.logout", null));
			this.WriteLiteral("</a>\n</p>");
		}
	}
}