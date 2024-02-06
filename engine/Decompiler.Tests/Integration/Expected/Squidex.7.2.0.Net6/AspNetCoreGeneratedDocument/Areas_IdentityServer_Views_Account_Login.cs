using Microsoft.AspNetCore.Html;
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
using Squidex.Areas.IdentityServer.Controllers;
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
	[RazorCompiledItemMetadata("Identifier", "/Areas/IdentityServer/Views/Account/Login.cshtml")]
	[RazorSourceChecksum("SHA256", "96902ca7bc9007282c2aa6ba41ece2d109a87d8ca7bd90eaeb5d11ba945ca083", "/Areas/IdentityServer/Views/_ViewImports.cshtml")]
	[RazorSourceChecksum("SHA256", "cafef8ff535952c6e109f453f52178fe4bc3cbe9447e9ba77f22faaa39fd9c89", "/Areas/IdentityServer/Views/Account/Login.cshtml")]
	internal sealed class Areas_IdentityServer_Views_Account_Login : RazorPage<LoginVM>
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
		public IHtmlHelper<LoginVM> Html
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

		static Areas_IdentityServer_Views_Account_Login()
		{
			Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_0 = new TagHelperAttribute("class", new HtmlString("btn btn-toggle btn-primary"), 0);
			Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_1 = new TagHelperAttribute("asp-controller", "Account", 0);
			Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_2 = new TagHelperAttribute("asp-action", "Login", 0);
			Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_3 = new TagHelperAttribute("class", new HtmlString("btn btn-toggle"), 0);
			Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_4 = new TagHelperAttribute("asp-action", "Signup", 0);
			Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_5 = new TagHelperAttribute("asp-action", "External", 0);
			Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_6 = new TagHelperAttribute("method", "post", 0);
		}

		public Areas_IdentityServer_Views_Account_Login()
		{
		}

		[NullableContext(0)]
		public override async Task ExecuteAsync()
		{
			string str;
			this.WriteLiteral("\n");
			str = (base.get_Model().IsLogin ? T.Get("common.login", null) : T.Get("common.signup", null));
			string str1 = str;
			((dynamic)base.get_ViewBag()).Title = str1;
			this.WriteLiteral("\n<div class=\"login-container\">\n    <div class=\"container\">\n        <div class=\"row text-center\">\n            <div class=\"btn-group profile-headline\">\n");
			if (!base.get_Model().IsLogin)
			{
				this.WriteLiteral("                ");
				this.__tagHelperExecutionContext = this.__tagHelperScopeManager.Begin("a", 0, "cafef8ff535952c6e109f453f52178fe4bc3cbe9447e9ba77f22faaa39fd9c899716", async () => this.Write(T.Get("common.login", null)));
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = base.CreateTagHelper<AnchorTagHelper>();
				this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
				this.__tagHelperExecutionContext.AddHtmlAttribute(Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_3);
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.set_Controller((string)Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_1.get_Value());
				this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_1);
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.set_Action((string)Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_2.get_Value());
				this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_2);
				if (this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.get_RouteValues() == null)
				{
					throw new InvalidOperationException(base.InvalidTagHelperIndexerAssignment("asp-route-returnurl", "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper", "RouteValues"));
				}
				base.BeginWriteTagHelperAttribute();
				this.WriteLiteral(base.get_Model().ReturnUrl);
				this.__tagHelperStringValueBuffer = base.EndWriteTagHelperAttribute();
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.get_RouteValues()["returnurl"] = this.__tagHelperStringValueBuffer;
				this.__tagHelperExecutionContext.AddTagHelperAttribute("asp-route-returnurl", this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.get_RouteValues()["returnurl"], 0);
				await this.__tagHelperRunner.RunAsync(this.__tagHelperExecutionContext);
				if (!this.__tagHelperExecutionContext.get_Output().get_IsContentModified())
				{
					await this.__tagHelperExecutionContext.SetOutputContentAsync();
				}
				this.Write(this.__tagHelperExecutionContext.get_Output());
				this.__tagHelperExecutionContext = this.__tagHelperScopeManager.End();
				this.WriteLiteral("\n");
			}
			else
			{
				this.WriteLiteral("                ");
				this.__tagHelperExecutionContext = this.__tagHelperScopeManager.Begin("a", 0, "cafef8ff535952c6e109f453f52178fe4bc3cbe9447e9ba77f22faaa39fd9c896911", async () => this.Write(T.Get("common.login", null)));
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = base.CreateTagHelper<AnchorTagHelper>();
				this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
				this.__tagHelperExecutionContext.AddHtmlAttribute(Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_0);
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.set_Controller((string)Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_1.get_Value());
				this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_1);
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.set_Action((string)Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_2.get_Value());
				this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_2);
				if (this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.get_RouteValues() == null)
				{
					throw new InvalidOperationException(base.InvalidTagHelperIndexerAssignment("asp-route-returnurl", "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper", "RouteValues"));
				}
				base.BeginWriteTagHelperAttribute();
				this.WriteLiteral(base.get_Model().ReturnUrl);
				this.__tagHelperStringValueBuffer = base.EndWriteTagHelperAttribute();
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.get_RouteValues()["returnurl"] = this.__tagHelperStringValueBuffer;
				this.__tagHelperExecutionContext.AddTagHelperAttribute("asp-route-returnurl", this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.get_RouteValues()["returnurl"], 0);
				await this.__tagHelperRunner.RunAsync(this.__tagHelperExecutionContext);
				if (!this.__tagHelperExecutionContext.get_Output().get_IsContentModified())
				{
					await this.__tagHelperExecutionContext.SetOutputContentAsync();
				}
				this.Write(this.__tagHelperExecutionContext.get_Output());
				this.__tagHelperExecutionContext = this.__tagHelperScopeManager.End();
				this.WriteLiteral("\n");
			}
			if (base.get_Model().IsLogin)
			{
				this.WriteLiteral("                ");
				this.__tagHelperExecutionContext = this.__tagHelperScopeManager.Begin("a", 0, "cafef8ff535952c6e109f453f52178fe4bc3cbe9447e9ba77f22faaa39fd9c8915489", async () => this.Write(T.Get("common.signup", null)));
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = base.CreateTagHelper<AnchorTagHelper>();
				this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
				this.__tagHelperExecutionContext.AddHtmlAttribute(Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_3);
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.set_Controller((string)Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_1.get_Value());
				this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_1);
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.set_Action((string)Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_4.get_Value());
				this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_4);
				if (this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.get_RouteValues() == null)
				{
					throw new InvalidOperationException(base.InvalidTagHelperIndexerAssignment("asp-route-returnurl", "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper", "RouteValues"));
				}
				base.BeginWriteTagHelperAttribute();
				this.WriteLiteral(base.get_Model().ReturnUrl);
				this.__tagHelperStringValueBuffer = base.EndWriteTagHelperAttribute();
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.get_RouteValues()["returnurl"] = this.__tagHelperStringValueBuffer;
				this.__tagHelperExecutionContext.AddTagHelperAttribute("asp-route-returnurl", this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.get_RouteValues()["returnurl"], 0);
				await this.__tagHelperRunner.RunAsync(this.__tagHelperExecutionContext);
				if (!this.__tagHelperExecutionContext.get_Output().get_IsContentModified())
				{
					await this.__tagHelperExecutionContext.SetOutputContentAsync();
				}
				this.Write(this.__tagHelperExecutionContext.get_Output());
				this.__tagHelperExecutionContext = this.__tagHelperScopeManager.End();
				this.WriteLiteral("\n");
			}
			else
			{
				this.WriteLiteral("                ");
				this.__tagHelperExecutionContext = this.__tagHelperScopeManager.Begin("a", 0, "cafef8ff535952c6e109f453f52178fe4bc3cbe9447e9ba77f22faaa39fd9c8912682", async () => this.Write(T.Get("common.signup", null)));
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = base.CreateTagHelper<AnchorTagHelper>();
				this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
				this.__tagHelperExecutionContext.AddHtmlAttribute(Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_0);
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.set_Controller((string)Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_1.get_Value());
				this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_1);
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.set_Action((string)Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_4.get_Value());
				this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_4);
				if (this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.get_RouteValues() == null)
				{
					throw new InvalidOperationException(base.InvalidTagHelperIndexerAssignment("asp-route-returnurl", "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper", "RouteValues"));
				}
				base.BeginWriteTagHelperAttribute();
				this.WriteLiteral(base.get_Model().ReturnUrl);
				this.__tagHelperStringValueBuffer = base.EndWriteTagHelperAttribute();
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.get_RouteValues()["returnurl"] = this.__tagHelperStringValueBuffer;
				this.__tagHelperExecutionContext.AddTagHelperAttribute("asp-route-returnurl", this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.get_RouteValues()["returnurl"], 0);
				await this.__tagHelperRunner.RunAsync(this.__tagHelperExecutionContext);
				if (!this.__tagHelperExecutionContext.get_Output().get_IsContentModified())
				{
					await this.__tagHelperExecutionContext.SetOutputContentAsync();
				}
				this.Write(this.__tagHelperExecutionContext.get_Output());
				this.__tagHelperExecutionContext = this.__tagHelperScopeManager.End();
				this.WriteLiteral("\n");
			}
			this.WriteLiteral("            </div>\n        </div>\n    </div>\n\n    ");
			this.__tagHelperExecutionContext = this.__tagHelperScopeManager.Begin("form", 0, "cafef8ff535952c6e109f453f52178fe4bc3cbe9447e9ba77f22faaa39fd9c8918303", async () => {
				this.WriteLiteral("\n");
				foreach (ExternalProvider externalProvider in base.get_Model().ExternalProviders)
				{
					string lowerInvariant = externalProvider.AuthenticationScheme.ToLowerInvariant();
					this.WriteLiteral("        <div class=\"form-group\">\n                <button");
					this.BeginWriteAttribute("class", " class=\"", 0x604, "\"", 0x639, 6);
					base.WriteAttributeValue("", 0x60c, "btn", 0x60c, 3, true);
					base.WriteAttributeValue(" ", 0x60f, "external-button", 0x610, 16, true);
					base.WriteAttributeValue(" ", 0x61f, "btn-block", 0x620, 10, true);
					base.WriteAttributeValue(" ", 0x629, "btn", 0x62a, 4, true);
					base.WriteAttributeValue(" ", 0x62d, "btn-", 0x62e, 5, true);
					base.WriteAttributeValue("", 0x632, lowerInvariant, 0x632, 7, false);
					this.EndWriteAttribute();
					this.WriteLiteral(" type=\"submit\" name=\"provider\"");
					this.BeginWriteAttribute("value", " value=\"", 0x658, "\"", 0x67e, 1);
					base.WriteAttributeValue("", 0x660, externalProvider.AuthenticationScheme, 0x660, 30, false);
					this.EndWriteAttribute();
					this.WriteLiteral(">\n                    <i");
					this.BeginWriteAttribute("class", " class=\"", 0x697, "\"", 0x6b9, 3);
					base.WriteAttributeValue("", 0x69f, "icon-", 0x69f, 5, true);
					base.WriteAttributeValue("", 0x6a4, lowerInvariant, 0x6a4, 7, false);
					base.WriteAttributeValue(" ", 0x6ab, "external-icon", 0x6ac, 14, true);
					this.EndWriteAttribute();
					this.WriteLiteral("></i> ");
					this.Write(this.Html.Raw(T.Get("users.login.loginWith", new { action = str1, provider = externalProvider.DisplayName })));
					this.WriteLiteral("\n                </button>\n            </div>\n");
				}
				this.WriteLiteral("    ");
			});
			this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = base.CreateTagHelper<FormTagHelper>();
			this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
			this.__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = base.CreateTagHelper<RenderAtEndOfFormTagHelper>();
			this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
			this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.set_Controller((string)Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_1.get_Value());
			this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_1);
			this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.set_Action((string)Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_5.get_Value());
			this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_5);
			if (this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.get_RouteValues() == null)
			{
				throw new InvalidOperationException(base.InvalidTagHelperIndexerAssignment("asp-route-returnurl", "Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper", "RouteValues"));
			}
			base.BeginWriteTagHelperAttribute();
			this.WriteLiteral(base.get_Model().ReturnUrl);
			this.__tagHelperStringValueBuffer = base.EndWriteTagHelperAttribute();
			this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.get_RouteValues()["returnurl"] = this.__tagHelperStringValueBuffer;
			this.__tagHelperExecutionContext.AddTagHelperAttribute("asp-route-returnurl", this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.get_RouteValues()["returnurl"], 0);
			this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.set_Method((string)Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_6.get_Value());
			this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_6);
			await this.__tagHelperRunner.RunAsync(this.__tagHelperExecutionContext);
			if (!this.__tagHelperExecutionContext.get_Output().get_IsContentModified())
			{
				await this.__tagHelperExecutionContext.SetOutputContentAsync();
			}
			this.Write(this.__tagHelperExecutionContext.get_Output());
			this.__tagHelperExecutionContext = this.__tagHelperScopeManager.End();
			this.WriteLiteral("\n\n");
			if (base.get_Model().HasExternalLogin && base.get_Model().HasPasswordAuth)
			{
				this.WriteLiteral("    <div class=\"profile-separator\">\n            <div class=\"profile-separator-text\">");
				this.Write(T.Get("users.login.separator", null));
				this.WriteLiteral("</div>\n        </div>\n");
			}
			this.WriteLiteral("\n");
			if (base.get_Model().HasPasswordAuth)
			{
				if (!base.get_Model().IsLogin)
				{
					this.WriteLiteral("        <div class=\"profile-password-signup text-center\">");
					this.Write(T.Get("users.login.askAdmin", null));
					this.WriteLiteral("</div>\n");
				}
				else
				{
					if (base.get_Model().IsFailed)
					{
						this.WriteLiteral("            <div class=\"form-alert form-alert-error\">");
						this.Write(T.Get("users.login.error", null));
						this.WriteLiteral("</div>\n");
					}
					this.WriteLiteral("        ");
					this.__tagHelperExecutionContext = this.__tagHelperScopeManager.Begin("form", 0, "cafef8ff535952c6e109f453f52178fe4bc3cbe9447e9ba77f22faaa39fd9c8925189", async () => {
						this.WriteLiteral("\n                <div class=\"form-group\">\n                    <input type=\"email\" class=\"form-control\" name=\"email\" id=\"email\"");
						this.BeginWriteAttribute("placeholder", " placeholder=\"", 0x9dd, "\"", 0xa11, 1);
						base.WriteAttributeValue("", 0x9eb, T.Get("users.login.emailPlaceholder", null), 0x9eb, 38, false);
						this.EndWriteAttribute();
						this.WriteLiteral(" />\n                </div>\n\n                <div class=\"form-group\">\n                    <input type=\"password\" class=\"form-control\" name=\"password\" id=\"password\"");
						this.BeginWriteAttribute("placeholder", " placeholder=\"", 0xab4, "\"", 0xaeb, 1);
						base.WriteAttributeValue("", 0xac2, T.Get("users.login.passwordPlaceholder", null), 0xac2, 41, false);
						this.EndWriteAttribute();
						this.WriteLiteral(" />\n                </div>\n\n                <button type=\"submit\" class=\"btn btn-block btn-primary\">");
						this.Write(str1);
						this.WriteLiteral("</button>\n            ");
					});
					this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = base.CreateTagHelper<FormTagHelper>();
					this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
					this.__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = base.CreateTagHelper<RenderAtEndOfFormTagHelper>();
					this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
					this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.set_Controller((string)Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_1.get_Value());
					this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_1);
					this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.set_Action((string)Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_2.get_Value());
					this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_2);
					if (this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.get_RouteValues() == null)
					{
						throw new InvalidOperationException(base.InvalidTagHelperIndexerAssignment("asp-route-returnurl", "Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper", "RouteValues"));
					}
					base.BeginWriteTagHelperAttribute();
					this.WriteLiteral(base.get_Model().ReturnUrl);
					this.__tagHelperStringValueBuffer = base.EndWriteTagHelperAttribute();
					this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.get_RouteValues()["returnurl"] = this.__tagHelperStringValueBuffer;
					this.__tagHelperExecutionContext.AddTagHelperAttribute("asp-route-returnurl", this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.get_RouteValues()["returnurl"], 0);
					this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.set_Method((string)Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_6.get_Value());
					this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_6);
					await this.__tagHelperRunner.RunAsync(this.__tagHelperExecutionContext);
					if (!this.__tagHelperExecutionContext.get_Output().get_IsContentModified())
					{
						await this.__tagHelperExecutionContext.SetOutputContentAsync();
					}
					this.Write(this.__tagHelperExecutionContext.get_Output());
					this.__tagHelperExecutionContext = this.__tagHelperScopeManager.End();
					this.WriteLiteral("\n");
				}
			}
			this.WriteLiteral("\n");
			if (!base.get_Model().IsLogin)
			{
				this.WriteLiteral("    <p class=\"profile-footer\">\n            ");
				this.Write(T.Get("users.login.noAccountLoginQuestion", null));
				this.WriteLiteral("\n\n            ");
				this.__tagHelperExecutionContext = this.__tagHelperScopeManager.Begin("a", 0, "cafef8ff535952c6e109f453f52178fe4bc3cbe9447e9ba77f22faaa39fd9c8933717", async () => {
					this.WriteLiteral("\n                ");
					this.Write(T.Get("users.login.noAccountLoginAction", null));
					this.WriteLiteral("\n            ");
				});
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = base.CreateTagHelper<AnchorTagHelper>();
				this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.set_Controller((string)Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_1.get_Value());
				this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_1);
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.set_Action((string)Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_2.get_Value());
				this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_2);
				if (this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.get_RouteValues() == null)
				{
					throw new InvalidOperationException(base.InvalidTagHelperIndexerAssignment("asp-route-returnurl", "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper", "RouteValues"));
				}
				base.BeginWriteTagHelperAttribute();
				this.WriteLiteral(base.get_Model().ReturnUrl);
				this.__tagHelperStringValueBuffer = base.EndWriteTagHelperAttribute();
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.get_RouteValues()["returnurl"] = this.__tagHelperStringValueBuffer;
				this.__tagHelperExecutionContext.AddTagHelperAttribute("asp-route-returnurl", this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.get_RouteValues()["returnurl"], 0);
				await this.__tagHelperRunner.RunAsync(this.__tagHelperExecutionContext);
				if (!this.__tagHelperExecutionContext.get_Output().get_IsContentModified())
				{
					await this.__tagHelperExecutionContext.SetOutputContentAsync();
				}
				this.Write(this.__tagHelperExecutionContext.get_Output());
				this.__tagHelperExecutionContext = this.__tagHelperScopeManager.End();
				this.WriteLiteral("\n        </p>\n");
			}
			else
			{
				this.WriteLiteral("    <p class=\"profile-footer\">\n            ");
				this.Write(T.Get("users.login.noAccountSignupQuestion", null));
				this.WriteLiteral("\n\n            ");
				this.__tagHelperExecutionContext = this.__tagHelperScopeManager.Begin("a", 0, "cafef8ff535952c6e109f453f52178fe4bc3cbe9447e9ba77f22faaa39fd9c8930615", async () => {
					this.WriteLiteral("\n                ");
					this.Write(T.Get("users.login.noAccountSignupAction", null));
					this.WriteLiteral("\n            ");
				});
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = base.CreateTagHelper<AnchorTagHelper>();
				this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.set_Controller((string)Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_1.get_Value());
				this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_1);
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.set_Action((string)Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_4.get_Value());
				this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Account_Login.__tagHelperAttribute_4);
				if (this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.get_RouteValues() == null)
				{
					throw new InvalidOperationException(base.InvalidTagHelperIndexerAssignment("asp-route-returnurl", "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper", "RouteValues"));
				}
				base.BeginWriteTagHelperAttribute();
				this.WriteLiteral(base.get_Model().ReturnUrl);
				this.__tagHelperStringValueBuffer = base.EndWriteTagHelperAttribute();
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.get_RouteValues()["returnurl"] = this.__tagHelperStringValueBuffer;
				this.__tagHelperExecutionContext.AddTagHelperAttribute("asp-route-returnurl", this.__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.get_RouteValues()["returnurl"], 0);
				await this.__tagHelperRunner.RunAsync(this.__tagHelperExecutionContext);
				if (!this.__tagHelperExecutionContext.get_Output().get_IsContentModified())
				{
					await this.__tagHelperExecutionContext.SetOutputContentAsync();
				}
				this.Write(this.__tagHelperExecutionContext.get_Output());
				this.__tagHelperExecutionContext = this.__tagHelperScopeManager.End();
				this.WriteLiteral("\n        </p>\n");
			}
			this.WriteLiteral("</div>");
		}
	}
}