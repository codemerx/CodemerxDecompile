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
using Squidex.Areas.IdentityServer.Controllers.Account;
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
	[RazorCompiledItemMetadata("Identifier", "/Areas/IdentityServer/Views/Account/Consent.cshtml")]
	[RazorSourceChecksum("SHA256", "56a8b90a6a33b793f65e32ec9ba50362d1110ecbf71e872b620245d1ad5abe98", "/Areas/IdentityServer/Views/Account/Consent.cshtml")]
	[RazorSourceChecksum("SHA256", "96902ca7bc9007282c2aa6ba41ece2d109a87d8ca7bd90eaeb5d11ba945ca083", "/Areas/IdentityServer/Views/_ViewImports.cshtml")]
	internal sealed class Areas_IdentityServer_Views_Account_Consent : RazorPage<ConsentVM>
	{
		[Nullable(0)]
		private readonly static TagHelperAttribute __tagHelperAttribute_0;

		[Nullable(0)]
		private readonly static TagHelperAttribute __tagHelperAttribute_1;

		[Nullable(0)]
		private readonly static TagHelperAttribute __tagHelperAttribute_2;

		[Nullable(0)]
		private TagHelperExecutionContext __tagHelperExecutionContext;

		[Nullable(0)]
		private TagHelperRunner __tagHelperRunner = new TagHelperRunner();

		[Nullable(0)]
		private string __tagHelperStringValueBuffer;

		[Nullable(0)]
		private TagHelperScopeManager __backed__tagHelperScopeManager;

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
		public IHtmlHelper<ConsentVM> Html
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

		static Areas_IdentityServer_Views_Account_Consent()
		{
			Areas_IdentityServer_Views_Account_Consent.__tagHelperAttribute_0 = new TagHelperAttribute("asp-controller", "Account", 0);
			Areas_IdentityServer_Views_Account_Consent.__tagHelperAttribute_1 = new TagHelperAttribute("asp-action", "Consent", 0);
			Areas_IdentityServer_Views_Account_Consent.__tagHelperAttribute_2 = new TagHelperAttribute("method", "post", 0);
		}

		public Areas_IdentityServer_Views_Account_Consent()
		{
		}

		public string ErrorClass(string error)
		{
			bool validationState;
			ModelStateEntry item = base.get_ViewData().get_ModelState().get_Item(error);
			if (item != null)
			{
				validationState = item.get_ValidationState() == 1;
			}
			else
			{
				validationState = false;
			}
			if (!validationState)
			{
				return "";
			}
			return "border-danger";
		}

		[NullableContext(0)]
		public override async Task ExecuteAsync()
		{
			this.WriteLiteral("\n");
			((dynamic)base.get_ViewBag()).Title = T.Get("users.consent.title", null);
			this.WriteLiteral("\n");
			this.WriteLiteral("\n");
			this.__tagHelperExecutionContext = this.__tagHelperScopeManager.Begin("form", 0, "56a8b90a6a33b793f65e32ec9ba50362d1110ecbf71e872b620245d1ad5abe985055", async () => {
				this.WriteLiteral("\n    <h2>");
				this.Write(T.Get("users.consent.headline", null));
				this.WriteLiteral("</h2>\n        \n    <label for=\"consentToAutomatedEmails\">\n        <div");
				this.BeginWriteAttribute("class", " class=\"", 0x224, "\"", 0x265, 3);
				base.WriteAttributeValue("", 0x22c, "card", 0x22c, 4, true);
				base.WriteAttributeValue(" ", 0x230, "card-consent", 0x231, 13, true);
				base.WriteAttributeValue(" ", 0x23d, this.ErrorClass("ConsentToAutomatedEmails"), 0x23e, 39, false);
				this.EndWriteAttribute();
				this.WriteLiteral(">\n            <div class=\"card-body\">\n                <h4 class=\"card-title\">");
				this.Write(T.Get("users.consent.emailHeadline", null));
				this.WriteLiteral("</h4>\n\n                <div class=\"card-text row\">\n                    <div class=\"col col-auto\">\n                        <input type=\"checkbox\" id=\"consentToAutomatedEmails\" name=\"consentToAutomatedEmails\" value=\"True\" />\n                    </div>\n                    <div class=\"col\">\n                        ");
				this.Write(this.Html.Raw(T.Get("users.consent.emailText", null)));
				this.WriteLiteral("\n                    </div>\n                </div>\n            </div>\n        </div>\n    </label>\n\n    <label for=\"consentToCookies\">\n        <div");
				this.BeginWriteAttribute("class", " class=\"", 0x4cd, "\"", 0x506, 3);
				base.WriteAttributeValue("", 0x4d5, "card", 0x4d5, 4, true);
				base.WriteAttributeValue(" ", 0x4d9, "card-consent", 0x4da, 13, true);
				base.WriteAttributeValue(" ", 0x4e6, this.ErrorClass("ConsentToCookies"), 0x4e7, 31, false);
				this.EndWriteAttribute();
				this.WriteLiteral(">\n            <div class=\"card-body\">\n                <h4 class=\"card-title\">");
				this.Write(T.Get("users.consent.cookiesHeadline", null));
				this.WriteLiteral("</h4>\n\n                <div class=\"card-text row\">\n                    <div class=\"col col-auto\">\n                        <input type=\"checkbox\" id=\"consentToCookies\" name=\"consentToCookies\" value=\"True\" />\n                    </div>\n                    <div class=\"col\">\n                        ");
				this.Write(this.Html.Raw(T.Get("users.consent.cookiesText", new { privacyUrl = base.get_Model().PrivacyUrl })));
				this.WriteLiteral("\n                    </div>\n                </div>\n            </div>\n        </div>\n    </label>\n\n    <label for=\"consentToPersonalInformation\">\n        <div");
				this.BeginWriteAttribute("class", " class=\"", 0x796, "\"", 0x7db, 3);
				base.WriteAttributeValue("", 0x79e, "card", 0x79e, 4, true);
				base.WriteAttributeValue(" ", 0x7a2, "card-consent", 0x7a3, 13, true);
				base.WriteAttributeValue(" ", 0x7af, this.ErrorClass("ConsentToPersonalInformation"), 0x7b0, 43, false);
				this.EndWriteAttribute();
				this.WriteLiteral(">\n            <div class=\"card-body\">\n                <h4 class=\"card-title\">");
				this.Write(T.Get("users.consent.piiHeadline", null));
				this.WriteLiteral("</h4>\n\n                <div class=\"card-text row\">\n                    <div class=\"col col-auto\">\n                        <input type=\"checkbox\" id=\"consentToPersonalInformation\" name=\"consentToPersonalInformation\" value=\"True\" />\n                    </div>\n                    <div class=\"col\">\n                        ");
				this.Write(this.Html.Raw(T.Get("users.consent.piiText", null)));
				this.WriteLiteral("\n                    </div>\n                </div>\n            </div>\n        </div>\n    </label>\n\n    <div class=\"profile-section-sm text-right\">\n        <button type=\"submit\" class=\"btn btn-success\">");
				this.Write(T.Get("users.consent.agree", null));
				this.WriteLiteral("</button>\n    </div>\n");
			});
			this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = base.CreateTagHelper<FormTagHelper>();
			this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
			this.__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = base.CreateTagHelper<RenderAtEndOfFormTagHelper>();
			this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
			this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.set_Controller((string)Areas_IdentityServer_Views_Account_Consent.__tagHelperAttribute_0.get_Value());
			this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Account_Consent.__tagHelperAttribute_0);
			this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.set_Action((string)Areas_IdentityServer_Views_Account_Consent.__tagHelperAttribute_1.get_Value());
			this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Account_Consent.__tagHelperAttribute_1);
			if (this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.get_RouteValues() == null)
			{
				throw new InvalidOperationException(base.InvalidTagHelperIndexerAssignment("asp-route-returnurl", "Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper", "RouteValues"));
			}
			base.BeginWriteTagHelperAttribute();
			this.WriteLiteral(base.get_Model().ReturnUrl);
			this.__tagHelperStringValueBuffer = base.EndWriteTagHelperAttribute();
			this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.get_RouteValues()["returnurl"] = this.__tagHelperStringValueBuffer;
			this.__tagHelperExecutionContext.AddTagHelperAttribute("asp-route-returnurl", this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.get_RouteValues()["returnurl"], 0);
			this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.set_Method((string)Areas_IdentityServer_Views_Account_Consent.__tagHelperAttribute_2.get_Value());
			this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Account_Consent.__tagHelperAttribute_2);
			await this.__tagHelperRunner.RunAsync(this.__tagHelperExecutionContext);
			if (!this.__tagHelperExecutionContext.get_Output().get_IsContentModified())
			{
				await this.__tagHelperExecutionContext.SetOutputContentAsync();
			}
			this.Write(this.__tagHelperExecutionContext.get_Output());
			this.__tagHelperExecutionContext = this.__tagHelperScopeManager.End();
		}
	}
}