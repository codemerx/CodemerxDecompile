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
	[RazorCompiledItemMetadata("Identifier", "/Areas/IdentityServer/Views/Account/LogoutCompleted.cshtml")]
	[RazorSourceChecksum("SHA256", "96902ca7bc9007282c2aa6ba41ece2d109a87d8ca7bd90eaeb5d11ba945ca083", "/Areas/IdentityServer/Views/_ViewImports.cshtml")]
	[RazorSourceChecksum("SHA256", "e85d30d28609c6bee84b1f6ca63068ddc1e2ca0a3a346ca9b7121d12eac49626", "/Areas/IdentityServer/Views/Account/LogoutCompleted.cshtml")]
	internal sealed class Areas_IdentityServer_Views_Account_LogoutCompleted : RazorPage<object>
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

		public Areas_IdentityServer_Views_Account_LogoutCompleted()
		{
		}

		[NullableContext(0)]
		public override async Task ExecuteAsync()
		{
			((dynamic)base.get_ViewBag()).Title = T.Get("users.logout.title", null);
			this.WriteLiteral("\n<img class=\"splash-image\" src=\"squid.svg?title=BYE%20BYE&text=Hope%20to%20see%20you%20again%20soon!&face=happy\" />\n\n<h1 class=\"splash-h1\">");
			this.Write(T.Get("users.logout.headline", null));
			this.WriteLiteral("</h1>\n\n<p class=\"splash-text\">\n    ");
			this.Write(T.Get("users.logout.text", null));
			this.WriteLiteral("\n</p>\n");
		}
	}
}