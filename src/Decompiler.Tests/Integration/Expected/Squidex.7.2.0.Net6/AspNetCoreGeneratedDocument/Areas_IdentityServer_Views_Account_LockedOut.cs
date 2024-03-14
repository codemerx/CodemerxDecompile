using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Hosting;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.CSharp.RuntimeBinder;
using Squidex.Infrastructure.Translations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace AspNetCoreGeneratedDocument
{
	[CreateNewOnMetadataUpdate]
	[Nullable(new byte[] { 0, 1 })]
	[NullableContext(1)]
	[RazorCompiledItemMetadata("Identifier", "/Areas/IdentityServer/Views/Account/LockedOut.cshtml")]
	[RazorSourceChecksum("SHA256", "850361e1dcac9517f2e9ef7585648747adc95307974a3883c63debd53bd786f4", "/Areas/IdentityServer/Views/Account/LockedOut.cshtml")]
	[RazorSourceChecksum("SHA256", "96902ca7bc9007282c2aa6ba41ece2d109a87d8ca7bd90eaeb5d11ba945ca083", "/Areas/IdentityServer/Views/_ViewImports.cshtml")]
	internal sealed class Areas_IdentityServer_Views_Account_LockedOut : RazorPage<object>
	{
		[Nullable(0)]
		private readonly static TagHelperAttribute __tagHelperAttribute_0;

		[Nullable(0)]
		private readonly static TagHelperAttribute __tagHelperAttribute_1;

		[Nullable(0)]
		private TagHelperExecutionContext __tagHelperExecutionContext;

		[Nullable(0)]
		private TagHelperRunner __tagHelperRunner = new TagHelperRunner();

		[Nullable(0)]
		private string __tagHelperStringValueBuffer;

		[Nullable(0)]
		private TagHelperScopeManager __backed__tagHelperScopeManager;

		[Nullable(0)]
		private AnchorTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper;

		[Nullable(0)]
		private TagHelperScopeManager __tagHelperScopeManager
		{
			[NullableContext(0)]
			get
			{
				if (this.__backed__tagHelperScopeManager == null)
				{
					this.__backed__tagHelperScopeManager = new TagHelperScopeManager(new Action<HtmlEncoder>(this.StartTagHelperWritingScope), new Func<TagHelperContent>(this.EndTagHelperWritingScope));
				}
				return this.__backed__tagHelperScopeManager;
			}
		}

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

		static Areas_IdentityServer_Views_Account_LockedOut()
		{
			Areas_IdentityServer_Views_Account_LockedOut.__tagHelperAttribute_0 = new TagHelperAttribute("asp-controller", "Account", 0);
			Areas_IdentityServer_Views_Account_LockedOut.__tagHelperAttribute_1 = new TagHelperAttribute("asp-action", "LogoutRedirect", 0);
		}

		public Areas_IdentityServer_Views_Account_LockedOut()
		{
		}

		[NullableContext(0)]
		public override async Task ExecuteAsync()
		{
			((dynamic)base.get_ViewBag()).Title = T.Get("users.lockedOutTitle", null);
			this.WriteLiteral("\n<img class=\"splash-image\" src=\"squid.svg?title=STOP HERE&text=You%20shall%20not%20pass!\" />\n\n<h1 class=\"splash-h1\">");
			this.Write(T.Get("users.lockedOutTitle", null));
			this.WriteLiteral("</h1>\n\n<p class=\"splash-text\">\n    ");
			this.Write(T.Get("users.lockedOutText", null));
			this.WriteLiteral("\n</p>\n\n<p class=\"splash-text\">\n    ");
			this.__tagHelperExecutionContext = this.__tagHelperScopeManager.Begin("a", 0, "850361e1dcac9517f2e9ef7585648747adc95307974a3883c63debd53bd786f45173", async () => this.Write(T.Get("common.logout", null)));
			this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = base.CreateTagHelper<AnchorTagHelper>();
			this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
			this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.set_Controller((string)Areas_IdentityServer_Views_Account_LockedOut.__tagHelperAttribute_0.get_Value());
			this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Account_LockedOut.__tagHelperAttribute_0);
			this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.set_Action((string)Areas_IdentityServer_Views_Account_LockedOut.__tagHelperAttribute_1.get_Value());
			this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Account_LockedOut.__tagHelperAttribute_1);
			await this.__tagHelperRunner.RunAsync(this.__tagHelperExecutionContext);
			if (!this.__tagHelperExecutionContext.get_Output().get_IsContentModified())
			{
				await this.__tagHelperExecutionContext.SetOutputContentAsync();
			}
			this.Write(this.__tagHelperExecutionContext.get_Output());
			this.__tagHelperExecutionContext = this.__tagHelperScopeManager.End();
			this.WriteLiteral("\n</p>");
		}
	}
}