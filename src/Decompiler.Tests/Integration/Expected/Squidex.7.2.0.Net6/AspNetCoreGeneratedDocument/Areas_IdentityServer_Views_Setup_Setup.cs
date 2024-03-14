using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Hosting;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.CSharp.RuntimeBinder;
using Squidex.Areas.IdentityServer.Controllers.Setup;
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
	[RazorCompiledItemMetadata("Identifier", "/Areas/IdentityServer/Views/Setup/Setup.cshtml")]
	[RazorSourceChecksum("SHA256", "0f4ee9f6a43b12bfcddbd3d48c62db1f269b675fae955996bedfc02202d262a9", "/Areas/IdentityServer/Views/Setup/Setup.cshtml")]
	[RazorSourceChecksum("SHA256", "96902ca7bc9007282c2aa6ba41ece2d109a87d8ca7bd90eaeb5d11ba945ca083", "/Areas/IdentityServer/Views/_ViewImports.cshtml")]
	internal sealed class Areas_IdentityServer_Views_Setup_Setup : RazorPage<SetupVM>
	{
		[Nullable(0)]
		private readonly static TagHelperAttribute __tagHelperAttribute_0;

		[Nullable(0)]
		private readonly static TagHelperAttribute __tagHelperAttribute_1;

		[Nullable(0)]
		private readonly static TagHelperAttribute __tagHelperAttribute_2;

		[Nullable(0)]
		private readonly static TagHelperAttribute __tagHelperAttribute_3;

		[Nullable(0)]
		private readonly static TagHelperAttribute __tagHelperAttribute_4;

		[Nullable(0)]
		private readonly static TagHelperAttribute __tagHelperAttribute_5;

		[Nullable(0)]
		private readonly static TagHelperAttribute __tagHelperAttribute_6;

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
		private FormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper;

		[Nullable(0)]
		private RenderAtEndOfFormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper;

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
		public IHtmlHelper<SetupVM> Html
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

		static Areas_IdentityServer_Views_Setup_Setup()
		{
			Areas_IdentityServer_Views_Setup_Setup.__tagHelperAttribute_0 = new TagHelperAttribute("class", new HtmlString("btn btn-primary force-white"), 0);
			Areas_IdentityServer_Views_Setup_Setup.__tagHelperAttribute_1 = new TagHelperAttribute("asp-controller", "Account", 0);
			Areas_IdentityServer_Views_Setup_Setup.__tagHelperAttribute_2 = new TagHelperAttribute("asp-action", "Login", 0);
			Areas_IdentityServer_Views_Setup_Setup.__tagHelperAttribute_3 = new TagHelperAttribute("class", new HtmlString("profile-form"), 0);
			Areas_IdentityServer_Views_Setup_Setup.__tagHelperAttribute_4 = new TagHelperAttribute("asp-controller", "Setup", 0);
			Areas_IdentityServer_Views_Setup_Setup.__tagHelperAttribute_5 = new TagHelperAttribute("asp-action", "Setup", 0);
			Areas_IdentityServer_Views_Setup_Setup.__tagHelperAttribute_6 = new TagHelperAttribute("method", "post", 0);
		}

		public Areas_IdentityServer_Views_Setup_Setup()
		{
		}

		[NullableContext(0)]
		public override async Task ExecuteAsync()
		{
			this.WriteLiteral("\n");
			((dynamic)base.get_ViewBag()).Title = T.Get("setup.title", null);
			this.WriteLiteral("\n<h1>");
			this.Write(T.Get("setup.headline", null));
			this.WriteLiteral("</h1>\n\n<img style=\"height: 250px\" class=\"mt-2 mb-2\" src=\"squid.svg?title=Welcome&text=Welcome%20to%20the%20Installation%20Process&face=happy\" />\n\n<div class=\"mt-2 mb-2\">\n    <small class=\"form-text text-muted\">");
			this.Write(T.Get("setup.hint", null));
			this.WriteLiteral("</small>\n</div>\n\n<div class=\"profile-section\">\n    <h2>");
			this.Write(T.Get("setup.rules.headline", null));
			this.WriteLiteral("</h2>\n\n");
			if (!base.get_Model().IsValidHttps)
			{
				this.u003cExecuteAsyncu003eg__RenderRuleAsCriticalu007c16_2(T.Get("setup.ruleHttps.failure", null));
			}
			else
			{
				this.u003cExecuteAsyncu003eg__RenderRuleAsSuccessu007c16_1(T.Get("setup.ruleHttps.success", null));
			}
			this.WriteLiteral("\n");
			if (base.get_Model().BaseUrlConfigured != base.get_Model().BaseUrlCurrent)
			{
				this.u003cExecuteAsyncu003eg__RenderRuleAsCriticalu007c16_2(T.Get("setup.ruleUrl.failure", new { actual = base.get_Model().BaseUrlCurrent, configured = base.get_Model().BaseUrlConfigured }));
			}
			else
			{
				this.u003cExecuteAsyncu003eg__RenderRuleAsSuccessu007c16_1(T.Get("setup.ruleUrl.success", null));
			}
			this.WriteLiteral("\n");
			if (!base.get_Model().EverybodyCanCreateApps)
			{
				this.u003cExecuteAsyncu003eg__RenderRuleAsWarningu007c16_3(T.Get("setup.ruleAppCreation.warningAll", null));
			}
			else
			{
				this.u003cExecuteAsyncu003eg__RenderRuleAsWarningu007c16_3(T.Get("setup.ruleAppCreation.warningAdmins", null));
			}
			this.WriteLiteral("\n");
			if (!base.get_Model().EverybodyCanCreateTeams)
			{
				this.u003cExecuteAsyncu003eg__RenderRuleAsWarningu007c16_3(T.Get("setup.ruleTeamCreation.warningAll", null));
			}
			else
			{
				this.u003cExecuteAsyncu003eg__RenderRuleAsWarningu007c16_3(T.Get("setup.ruleTeamCreation.warningAdmins", null));
			}
			this.WriteLiteral("\n");
			if (base.get_Model().IsAssetStoreFtp)
			{
				this.u003cExecuteAsyncu003eg__RenderRuleAsWarningu007c16_3(T.Get("setup.ruleFtp.warning", null));
			}
			this.WriteLiteral("\n");
			if (base.get_Model().IsAssetStoreFile)
			{
				this.u003cExecuteAsyncu003eg__RenderRuleAsWarningu007c16_3(T.Get("setup.ruleFolder.warning", null));
			}
			this.WriteLiteral("</div>\n\n<hr />\n\n<div class=\"profile-section\">\n    <h2 class=\"mb-3\">");
			this.Write(T.Get("setup.createUser.headline", null));
			this.WriteLiteral("</h2>\n\n");
			if (base.get_Model().HasExternalLogin)
			{
				this.WriteLiteral("    <div>\n            <small class=\"form-text text-muted mt-2 mb-2\">");
				this.Write(T.Get("setup.createUser.loginHint", null));
				this.WriteLiteral("</small>\n\n            <div class=\"mt-3\">\n                ");
				this.__tagHelperExecutionContext = this.__tagHelperScopeManager.Begin("a", 0, "0f4ee9f6a43b12bfcddbd3d48c62db1f269b675fae955996bedfc02202d262a914105", async () => {
					this.WriteLiteral("\n                    ");
					this.Write(T.Get("setup.createUser.loginLink", null));
					this.WriteLiteral("\n                ");
				});
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = base.CreateTagHelper<AnchorTagHelper>();
				this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
				this.__tagHelperExecutionContext.AddHtmlAttribute(Areas_IdentityServer_Views_Setup_Setup.__tagHelperAttribute_0);
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.set_Controller((string)Areas_IdentityServer_Views_Setup_Setup.__tagHelperAttribute_1.get_Value());
				this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Setup_Setup.__tagHelperAttribute_1);
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.set_Action((string)Areas_IdentityServer_Views_Setup_Setup.__tagHelperAttribute_2.get_Value());
				this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Setup_Setup.__tagHelperAttribute_2);
				await this.__tagHelperRunner.RunAsync(this.__tagHelperExecutionContext);
				if (!this.__tagHelperExecutionContext.get_Output().get_IsContentModified())
				{
					await this.__tagHelperExecutionContext.SetOutputContentAsync();
				}
				this.Write(this.__tagHelperExecutionContext.get_Output());
				this.__tagHelperExecutionContext = this.__tagHelperScopeManager.End();
				this.WriteLiteral("\n            </div>\n        </div>\n");
			}
			this.WriteLiteral("\n");
			if (base.get_Model().HasExternalLogin && base.get_Model().HasPasswordAuth)
			{
				this.WriteLiteral("    <div class=\"profile-separator\">\n            <div class=\"profile-separator-text\">");
				this.Write(T.Get("setup.createUser.separator", null));
				this.WriteLiteral("</div>\n        </div>\n");
			}
			this.WriteLiteral("\n");
			if (base.get_Model().HasPasswordAuth)
			{
				this.WriteLiteral("    <h3>");
				this.Write(T.Get("setup.createUser.headlineCreate", null));
				this.WriteLiteral("</h3>\n");
				if (!string.IsNullOrWhiteSpace(base.get_Model().ErrorMessage))
				{
					this.WriteLiteral("        <div class=\"form-alert form-alert-error\">\n                ");
					this.Write(base.get_Model().ErrorMessage);
					this.WriteLiteral("\n            </div>\n");
				}
				this.WriteLiteral("    ");
				this.__tagHelperExecutionContext = this.__tagHelperScopeManager.Begin("form", 0, "0f4ee9f6a43b12bfcddbd3d48c62db1f269b675fae955996bedfc02202d262a917944", async () => {
					this.WriteLiteral("\n            <div class=\"form-group\">\n                <label for=\"email\">");
					this.Write(T.Get("common.email", null));
					this.WriteLiteral("</label>\n\n");
					this.u003cExecuteAsyncu003eg__RenderValidationu007c16_0("Email");
					this.WriteLiteral("\n                <input type=\"text\" class=\"form-control\" name=\"email\" id=\"email\" />\n            </div>\n\n            <div class=\"form-group\">\n                <label for=\"password\">");
					this.Write(T.Get("common.password", null));
					this.WriteLiteral("</label>\n\n");
					this.u003cExecuteAsyncu003eg__RenderValidationu007c16_0("Password");
					this.WriteLiteral("\n                <input type=\"password\" class=\"form-control\" name=\"password\" id=\"password\" />\n            </div>\n\n            <div class=\"form-group\">\n                <label for=\"passwordConfirm\">");
					this.Write(T.Get("setup.createUser.confirmPassword", null));
					this.WriteLiteral("</label>\n\n");
					this.u003cExecuteAsyncu003eg__RenderValidationu007c16_0("PasswordConfirm");
					this.WriteLiteral("\n                <input type=\"password\" class=\"form-control\" name=\"passwordConfirm\" id=\"passwordConfirm\" />\n            </div>\n\n            <div class=\"form-group mb-0\">\n                <button type=\"submit\" class=\"btn btn-success\">");
					this.Write(T.Get("setup.createUser.button", null));
					this.WriteLiteral("</button>\n            </div>\n        ");
				});
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = base.CreateTagHelper<FormTagHelper>();
				this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = base.CreateTagHelper<RenderAtEndOfFormTagHelper>();
				this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
				this.__tagHelperExecutionContext.AddHtmlAttribute(Areas_IdentityServer_Views_Setup_Setup.__tagHelperAttribute_3);
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.set_Controller((string)Areas_IdentityServer_Views_Setup_Setup.__tagHelperAttribute_4.get_Value());
				this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Setup_Setup.__tagHelperAttribute_4);
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.set_Action((string)Areas_IdentityServer_Views_Setup_Setup.__tagHelperAttribute_5.get_Value());
				this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Setup_Setup.__tagHelperAttribute_5);
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.set_Method((string)Areas_IdentityServer_Views_Setup_Setup.__tagHelperAttribute_6.get_Value());
				this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Setup_Setup.__tagHelperAttribute_6);
				await this.__tagHelperRunner.RunAsync(this.__tagHelperExecutionContext);
				if (!this.__tagHelperExecutionContext.get_Output().get_IsContentModified())
				{
					await this.__tagHelperExecutionContext.SetOutputContentAsync();
				}
				this.Write(this.__tagHelperExecutionContext.get_Output());
				this.__tagHelperExecutionContext = this.__tagHelperScopeManager.End();
				this.WriteLiteral("\n");
			}
			this.WriteLiteral("\n");
			if (!base.get_Model().HasExternalLogin && !base.get_Model().HasPasswordAuth)
			{
				this.WriteLiteral("    <div>\n            ");
				this.Write(T.Get("setup.createUser.failure", null));
				this.WriteLiteral("\n        </div>\n");
			}
			this.WriteLiteral("</div>");
		}
	}
}