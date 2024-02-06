using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Hosting;
using Microsoft.CSharp.RuntimeBinder;
using Squidex.Areas.IdentityServer.Controllers.Error;
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
	[RazorCompiledItemMetadata("Identifier", "/Areas/IdentityServer/Views/Error/Error.cshtml")]
	[RazorSourceChecksum("SHA256", "96902ca7bc9007282c2aa6ba41ece2d109a87d8ca7bd90eaeb5d11ba945ca083", "/Areas/IdentityServer/Views/_ViewImports.cshtml")]
	[RazorSourceChecksum("SHA256", "f116f699fe33474e020169276c636dee3fd9e7bf83a76bce19fa4e161de20fdc", "/Areas/IdentityServer/Views/Error/Error.cshtml")]
	internal sealed class Areas_IdentityServer_Views_Error_Error : RazorPage<ErrorVM>
	{
		[RazorInject]
		public IViewComponentHelper Component
		{
			get;
			private set;
		}

		[RazorInject]
		public IHtmlHelper<ErrorVM> Html
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

		public Areas_IdentityServer_Views_Error_Error()
		{
		}

		[NullableContext(0)]
		public override async Task ExecuteAsync()
		{
			this.WriteLiteral("\n");
			((dynamic)base.get_ViewBag()).Title = T.Get("users.error.title", null);
			this.WriteLiteral("\n<img class=\"splash-image\" src=\"squid.svg?title=OH%20DAMN&text=I%20am%20sorry%2C%20that%20something%20went%20wrong\" />\n\n<h1 class=\"splash-h1\">");
			this.Write(T.Get("users.error.headline", null));
			this.WriteLiteral("</h1>\n\n<p class=\"splash-text\">\n");
			if (base.get_Model().ErrorMessage == null)
			{
				this.WriteLiteral("    <span>");
				this.Write(T.Get("users.error.text", null));
				this.WriteLiteral("</span>\n");
			}
			else
			{
				this.Write(base.get_Model().ErrorMessage);
			}
			this.WriteLiteral("</p>");
		}
	}
}