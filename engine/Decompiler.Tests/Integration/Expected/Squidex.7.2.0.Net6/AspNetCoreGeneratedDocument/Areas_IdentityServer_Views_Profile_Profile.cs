using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Identity;
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
using Squidex.Areas.IdentityServer.Controllers;
using Squidex.Areas.IdentityServer.Controllers.Profile;
using Squidex.Infrastructure.Translations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace AspNetCoreGeneratedDocument
{
	[CreateNewOnMetadataUpdate]
	[Nullable(new byte[] { 0, 1 })]
	[RazorCompiledItemMetadata("Identifier", "/Areas/IdentityServer/Views/Profile/Profile.cshtml")]
	[RazorSourceChecksum("SHA256", "96902ca7bc9007282c2aa6ba41ece2d109a87d8ca7bd90eaeb5d11ba945ca083", "/Areas/IdentityServer/Views/_ViewImports.cshtml")]
	[RazorSourceChecksum("SHA256", "f69c10b5b1aab911f22074791651cdd1f0154dc25cfb10ea378c274b57f83b5d", "/Areas/IdentityServer/Views/Profile/Profile.cshtml")]
	internal sealed class Areas_IdentityServer_Views_Profile_Profile : RazorPage<ProfileVM>
	{
		private readonly static TagHelperAttribute __tagHelperAttribute_0;

		private readonly static TagHelperAttribute __tagHelperAttribute_1;

		private readonly static TagHelperAttribute __tagHelperAttribute_2;

		private readonly static TagHelperAttribute __tagHelperAttribute_3;

		private readonly static TagHelperAttribute __tagHelperAttribute_4;

		private readonly static TagHelperAttribute __tagHelperAttribute_5;

		private readonly static TagHelperAttribute __tagHelperAttribute_6;

		private readonly static TagHelperAttribute __tagHelperAttribute_7;

		private readonly static TagHelperAttribute __tagHelperAttribute_8;

		private readonly static TagHelperAttribute __tagHelperAttribute_9;

		private readonly static TagHelperAttribute __tagHelperAttribute_10;

		private readonly static TagHelperAttribute __tagHelperAttribute_11;

		private readonly static TagHelperAttribute __tagHelperAttribute_12;

		private readonly static TagHelperAttribute __tagHelperAttribute_13;

		private readonly static TagHelperAttribute __tagHelperAttribute_14;

		private readonly static TagHelperAttribute __tagHelperAttribute_15;

		private readonly static TagHelperAttribute __tagHelperAttribute_16;

		private readonly static TagHelperAttribute __tagHelperAttribute_17;

		private readonly static TagHelperAttribute __tagHelperAttribute_18;

		private readonly static TagHelperAttribute __tagHelperAttribute_19;

		private readonly static TagHelperAttribute __tagHelperAttribute_20;

		private TagHelperExecutionContext __tagHelperExecutionContext;

		private TagHelperRunner __tagHelperRunner = new TagHelperRunner();

		private string __tagHelperStringValueBuffer;

		private TagHelperScopeManager __backed__tagHelperScopeManager;

		private FormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper;

		private RenderAtEndOfFormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper;

		private InputTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper;

		private LabelTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_LabelTagHelper;

		private TagHelperScopeManager __tagHelperScopeManager
		{
			get
			{
				if (this.__backed__tagHelperScopeManager == null)
				{
					this.__backed__tagHelperScopeManager = new TagHelperScopeManager(new Action<HtmlEncoder>(this.StartTagHelperWritingScope), new Func<TagHelperContent>(this.EndTagHelperWritingScope));
				}
				return this.__backed__tagHelperScopeManager;
			}
		}

		[Nullable(1)]
		[RazorInject]
		public IViewComponentHelper Component
		{
			[NullableContext(1)]
			get;
			[NullableContext(1)]
			private set;
		}

		[Nullable(1)]
		[RazorInject]
		public IHtmlHelper<ProfileVM> Html
		{
			[NullableContext(1)]
			get;
			[NullableContext(1)]
			private set;
		}

		[Nullable(1)]
		[RazorInject]
		public IJsonHelper Json
		{
			[NullableContext(1)]
			get;
			[NullableContext(1)]
			private set;
		}

		[Nullable(1)]
		[RazorInject]
		public IModelExpressionProvider ModelExpressionProvider
		{
			[NullableContext(1)]
			get;
			[NullableContext(1)]
			private set;
		}

		[Nullable(1)]
		[RazorInject]
		public IUrlHelper Url
		{
			[NullableContext(1)]
			get;
			[NullableContext(1)]
			private set;
		}

		static Areas_IdentityServer_Views_Profile_Profile()
		{
			Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_0 = new TagHelperAttribute("id", new HtmlString("pictureForm"), 0);
			Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_1 = new TagHelperAttribute("class", new HtmlString("profile-picture-form"), 0);
			Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_2 = new TagHelperAttribute("asp-controller", "Profile", 0);
			Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_3 = new TagHelperAttribute("asp-action", "UploadPicture", 0);
			Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_4 = new TagHelperAttribute("method", "post", 0);
			Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_5 = new TagHelperAttribute("enctype", new HtmlString("multipart/form-data"), 0);
			Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_6 = new TagHelperAttribute("type", "email", 0);
			Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_7 = new TagHelperAttribute("class", new HtmlString("form-control"), 0);
			Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_8 = new TagHelperAttribute("type", "text", 0);
			Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_9 = new TagHelperAttribute("type", "checkbox", 0);
			Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_10 = new TagHelperAttribute("class", new HtmlString("form-check-input"), 0);
			Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_11 = new TagHelperAttribute("class", new HtmlString("form-check-label"), 0);
			Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_12 = new TagHelperAttribute("class", new HtmlString("profile-form profile-section"), 0);
			Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_13 = new TagHelperAttribute("asp-action", "UpdateProfile", 0);
			Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_14 = new TagHelperAttribute("asp-action", "RemoveLogin", 0);
			Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_15 = new TagHelperAttribute("asp-action", "AddLogin", 0);
			Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_16 = new TagHelperAttribute("class", new HtmlString("profile-form"), 0);
			Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_17 = new TagHelperAttribute("asp-action", "ChangePassword", 0);
			Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_18 = new TagHelperAttribute("asp-action", "SetPassword", 0);
			Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_19 = new TagHelperAttribute("asp-action", "GenerateClientSecret", 0);
			Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_20 = new TagHelperAttribute("asp-action", "UpdateProperties", 0);
		}

		public Areas_IdentityServer_Views_Profile_Profile()
		{
		}

		public override async Task ExecuteAsync()
		{
			this.WriteLiteral("\n");
			((dynamic)base.get_ViewBag()).Title = T.Get("users.profile.title", null);
			this.WriteLiteral("\n<h1>");
			this.Write(T.Get("users.profile.headline", null));
			this.WriteLiteral("</h1>\n\n<h2>");
			this.Write(T.Get("users.profile.pii", null));
			this.WriteLiteral("</h2>\n\n");
			if (!string.IsNullOrWhiteSpace(base.get_Model().SuccessMessage))
			{
				this.WriteLiteral("    <div class=\"form-alert form-alert-success\" id=\"success\">\n        ");
				this.Write(base.get_Model().SuccessMessage);
				this.WriteLiteral("\n    </div>\n");
			}
			this.WriteLiteral("\n");
			if (!string.IsNullOrWhiteSpace(base.get_Model().ErrorMessage))
			{
				this.WriteLiteral("    <div class=\"form-alert form-alert-error\">\n        ");
				this.Write(base.get_Model().ErrorMessage);
				this.WriteLiteral("\n    </div>\n");
			}
			this.WriteLiteral("\n<div class=\"row profile-section-sm\">\n    <div class=\"col profile-picture-col\">\n        <img class=\"profile-picture\"");
			this.BeginWriteAttribute("src", " src=\"", 0x3b6, "\"", 0x3ee, 3);
			base.WriteAttributeValue("", 0x3bc, "api/users/{Model!.Id}/picture/?q={", 0x3bc, 34, true);
			base.WriteAttributeValue("", 0x3de, Guid.NewGuid(), 0x3de, 15, false);
			base.WriteAttributeValue("", 0x3ed, "}", 0x3ed, 1, true);
			this.EndWriteAttribute();
			this.WriteLiteral(" />\n    </div>\n    <div class=\"col\">\n        ");
			this.__tagHelperExecutionContext = this.__tagHelperScopeManager.Begin("form", 0, "f69c10b5b1aab911f22074791651cdd1f0154dc25cfb10ea378c274b57f83b5d14724", async () => {
				this.WriteLiteral("\n            <span class=\"btn btn-secondary\" id=\"pictureButton\">\n                <span>");
				this.Write(T.Get("users.profile.uploadPicture", null));
				this.WriteLiteral("</span>\n\n                <input class=\"profile-picture-input\" name=\"file\" type=\"file\" id=\"pictureInput\" />\n            </span>\n        ");
			});
			this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = base.CreateTagHelper<FormTagHelper>();
			this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
			this.__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = base.CreateTagHelper<RenderAtEndOfFormTagHelper>();
			this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
			this.__tagHelperExecutionContext.AddHtmlAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_0);
			this.__tagHelperExecutionContext.AddHtmlAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_1);
			this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.set_Controller((string)Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_2.get_Value());
			this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_2);
			this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.set_Action((string)Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_3.get_Value());
			this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_3);
			this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.set_Method((string)Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_4.get_Value());
			this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_4);
			this.__tagHelperExecutionContext.AddHtmlAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_5);
			await this.__tagHelperRunner.RunAsync(this.__tagHelperExecutionContext);
			if (!this.__tagHelperExecutionContext.get_Output().get_IsContentModified())
			{
				await this.__tagHelperExecutionContext.SetOutputContentAsync();
			}
			this.Write(this.__tagHelperExecutionContext.get_Output());
			this.__tagHelperExecutionContext = this.__tagHelperScopeManager.End();
			this.WriteLiteral("\n    </div>\n</div>\n\n");
			this.__tagHelperExecutionContext = this.__tagHelperScopeManager.Begin("form", 0, "f69c10b5b1aab911f22074791651cdd1f0154dc25cfb10ea378c274b57f83b5d17300", async () => {
				this.WriteLiteral("\n    <div class=\"form-group\">\n        <label for=\"email\">");
				this.Write(T.Get("common.email", null));
				this.WriteLiteral("</label>\n\n");
				this.u003cExecuteAsyncu003eg__RenderValidationu007c31_0("Email");
				this.WriteLiteral("\n        ");
				this.__tagHelperExecutionContext = this.__tagHelperScopeManager.Begin("input", 1, "f69c10b5b1aab911f22074791651cdd1f0154dc25cfb10ea378c274b57f83b5d18098", async () => {
					Areas_IdentityServer_Views_Profile_Profile.u003cu003ec.u003cu003cExecuteAsyncu003eb__31_8u003ed _ = new Areas_IdentityServer_Views_Profile_Profile.u003cu003ec.u003cu003cExecuteAsyncu003eb__31_8u003ed();
					_.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
					_.u003cu003e1__state = -1;
					_.u003cu003et__builder.Start<Areas_IdentityServer_Views_Profile_Profile.u003cu003ec.u003cu003cExecuteAsyncu003eb__31_8u003ed>(ref _);
					return _.u003cu003et__builder.Task;
				});
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper = base.CreateTagHelper<InputTagHelper>();
				this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper);
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.set_InputTypeName((string)Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_6.get_Value());
				this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_6);
				this.__tagHelperExecutionContext.AddHtmlAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_7);
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.set_For(this.ModelExpressionProvider.CreateModelExpression<ProfileVM, string>(base.get_ViewData(), (ProfileVM __model) => __model.Email));
				this.__tagHelperExecutionContext.AddTagHelperAttribute("asp-for", this.__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.get_For(), 0);
				await this.__tagHelperRunner.RunAsync(this.__tagHelperExecutionContext);
				if (!this.__tagHelperExecutionContext.get_Output().get_IsContentModified())
				{
					await this.__tagHelperExecutionContext.SetOutputContentAsync();
				}
				this.Write(this.__tagHelperExecutionContext.get_Output());
				this.__tagHelperExecutionContext = this.__tagHelperScopeManager.End();
				this.WriteLiteral("\n    </div>\n\n    <div class=\"form-group\">\n        <label for=\"displayName\">");
				this.Write(T.Get("common.displayName", null));
				this.WriteLiteral("</label>\n\n");
				this.u003cExecuteAsyncu003eg__RenderValidationu007c31_0("DisplayName");
				this.WriteLiteral("\n        ");
				this.__tagHelperExecutionContext = this.__tagHelperScopeManager.Begin("input", 1, "f69c10b5b1aab911f22074791651cdd1f0154dc25cfb10ea378c274b57f83b5d20422", async () => {
					Areas_IdentityServer_Views_Profile_Profile.u003cu003ec.u003cu003cExecuteAsyncu003eb__31_10u003ed _ = new Areas_IdentityServer_Views_Profile_Profile.u003cu003ec.u003cu003cExecuteAsyncu003eb__31_10u003ed();
					_.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
					_.u003cu003e1__state = -1;
					_.u003cu003et__builder.Start<Areas_IdentityServer_Views_Profile_Profile.u003cu003ec.u003cu003cExecuteAsyncu003eb__31_10u003ed>(ref _);
					return _.u003cu003et__builder.Task;
				});
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper = base.CreateTagHelper<InputTagHelper>();
				this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper);
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.set_InputTypeName((string)Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_8.get_Value());
				this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_8);
				this.__tagHelperExecutionContext.AddHtmlAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_7);
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.set_For(this.ModelExpressionProvider.CreateModelExpression<ProfileVM, string>(base.get_ViewData(), (ProfileVM __model) => __model.DisplayName));
				this.__tagHelperExecutionContext.AddTagHelperAttribute("asp-for", this.__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.get_For(), 0);
				await this.__tagHelperRunner.RunAsync(this.__tagHelperExecutionContext);
				if (!this.__tagHelperExecutionContext.get_Output().get_IsContentModified())
				{
					await this.__tagHelperExecutionContext.SetOutputContentAsync();
				}
				this.Write(this.__tagHelperExecutionContext.get_Output());
				this.__tagHelperExecutionContext = this.__tagHelperScopeManager.End();
				this.WriteLiteral("\n    </div>\n\n    <div class=\"form-group\">\n        <div class=\"form-check\">\n            ");
				this.__tagHelperExecutionContext = this.__tagHelperScopeManager.Begin("input", 1, "f69c10b5b1aab911f22074791651cdd1f0154dc25cfb10ea378c274b57f83b5d22295", async () => {
					Areas_IdentityServer_Views_Profile_Profile.u003cu003ec.u003cu003cExecuteAsyncu003eb__31_12u003ed _ = new Areas_IdentityServer_Views_Profile_Profile.u003cu003ec.u003cu003cExecuteAsyncu003eb__31_12u003ed();
					_.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
					_.u003cu003e1__state = -1;
					_.u003cu003et__builder.Start<Areas_IdentityServer_Views_Profile_Profile.u003cu003ec.u003cu003cExecuteAsyncu003eb__31_12u003ed>(ref _);
					return _.u003cu003et__builder.Task;
				});
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper = base.CreateTagHelper<InputTagHelper>();
				this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper);
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.set_InputTypeName((string)Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_9.get_Value());
				this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_9);
				this.__tagHelperExecutionContext.AddHtmlAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_10);
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.set_For(this.ModelExpressionProvider.CreateModelExpression<ProfileVM, bool>(base.get_ViewData(), (ProfileVM __model) => __model.IsHidden));
				this.__tagHelperExecutionContext.AddTagHelperAttribute("asp-for", this.__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.get_For(), 0);
				await this.__tagHelperRunner.RunAsync(this.__tagHelperExecutionContext);
				if (!this.__tagHelperExecutionContext.get_Output().get_IsContentModified())
				{
					await this.__tagHelperExecutionContext.SetOutputContentAsync();
				}
				this.Write(this.__tagHelperExecutionContext.get_Output());
				this.__tagHelperExecutionContext = this.__tagHelperScopeManager.End();
				this.WriteLiteral("\n\n            ");
				this.__tagHelperExecutionContext = this.__tagHelperScopeManager.Begin("label", 0, "f69c10b5b1aab911f22074791651cdd1f0154dc25cfb10ea378c274b57f83b5d24086", async () => this.Write(T.Get("users.profile.hideProfile", null)));
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_LabelTagHelper = base.CreateTagHelper<LabelTagHelper>();
				this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_LabelTagHelper);
				this.__tagHelperExecutionContext.AddHtmlAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_11);
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_LabelTagHelper.set_For(this.ModelExpressionProvider.CreateModelExpression<ProfileVM, bool>(base.get_ViewData(), (ProfileVM __model) => __model.IsHidden));
				this.__tagHelperExecutionContext.AddTagHelperAttribute("asp-for", this.__Microsoft_AspNetCore_Mvc_TagHelpers_LabelTagHelper.get_For(), 0);
				await this.__tagHelperRunner.RunAsync(this.__tagHelperExecutionContext);
				if (!this.__tagHelperExecutionContext.get_Output().get_IsContentModified())
				{
					await this.__tagHelperExecutionContext.SetOutputContentAsync();
				}
				this.Write(this.__tagHelperExecutionContext.get_Output());
				this.__tagHelperExecutionContext = this.__tagHelperScopeManager.End();
				this.WriteLiteral("\n        </div>\n    </div>\n\n    <button type=\"submit\" class=\"btn btn-primary\">");
				this.Write(T.Get("common.save", null));
				this.WriteLiteral("</button>\n");
			});
			this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = base.CreateTagHelper<FormTagHelper>();
			this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
			this.__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = base.CreateTagHelper<RenderAtEndOfFormTagHelper>();
			this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
			this.__tagHelperExecutionContext.AddHtmlAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_12);
			this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.set_Controller((string)Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_2.get_Value());
			this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_2);
			this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.set_Action((string)Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_13.get_Value());
			this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_13);
			this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.set_Method((string)Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_4.get_Value());
			this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_4);
			await this.__tagHelperRunner.RunAsync(this.__tagHelperExecutionContext);
			if (!this.__tagHelperExecutionContext.get_Output().get_IsContentModified())
			{
				await this.__tagHelperExecutionContext.SetOutputContentAsync();
			}
			this.Write(this.__tagHelperExecutionContext.get_Output());
			this.__tagHelperExecutionContext = this.__tagHelperScopeManager.End();
			this.WriteLiteral("\n\n");
			if (base.get_Model().ExternalProviders.Any<ExternalProvider>())
			{
				this.WriteLiteral("    <hr />\n");
				this.WriteLiteral("    <div class=\"profile-section\">\n        <h2>");
				this.Write(T.Get("users.profile.loginsTitle", null));
				this.WriteLiteral("</h2>\n\n        <table class=\"table table-fixed table-lesspadding\">\n            <colgroup>\n                <col style=\"width: 100px;\" />\n                <col style=\"width: 100%;\" />\n                <col style=\"width: 100px;\" />\n            </colgroup>\n");
				foreach (UserLoginInfo externalLogin in base.get_Model().ExternalLogins)
				{
					this.WriteLiteral("            <tr>\n                    <td>\n                        <span>");
					this.Write(externalLogin.get_LoginProvider());
					this.WriteLiteral("</span>\n                    </td>\n                    <td>\n                        <span class=\"truncate\">");
					this.Write(externalLogin.get_ProviderDisplayName());
					this.WriteLiteral("</span>\n                    </td>\n                    <td class=\"text-right\">\n");
					if (base.get_Model().ExternalLogins.Count > 1 || base.get_Model().HasPassword)
					{
						this.WriteLiteral("                        ");
						this.__tagHelperExecutionContext = this.__tagHelperScopeManager.Begin("form", 0, "f69c10b5b1aab911f22074791651cdd1f0154dc25cfb10ea378c274b57f83b5d29858", async () => {
							this.WriteLiteral("\n                                <input type=\"hidden\"");
							this.BeginWriteAttribute("value", " value=\"", 0xd54, "\"", 0xd70, 1);
							base.WriteAttributeValue("", 0xd5c, externalLogin.get_LoginProvider(), 0xd5c, 20, false);
							this.EndWriteAttribute();
							this.WriteLiteral(" name=\"LoginProvider\" />\n                                <input type=\"hidden\"");
							this.BeginWriteAttribute("value", " value=\"", 0xdbe, "\"", 0xdd8, 1);
							base.WriteAttributeValue("", 0xdc6, externalLogin.get_ProviderKey(), 0xdc6, 18, false);
							this.EndWriteAttribute();
							this.WriteLiteral(" name=\"ProviderKey\" />\n\n                                <button type=\"submit\" class=\"btn btn-text-danger btn-sm\">\n                                    ");
							this.Write(T.Get("common.remove", null));
							this.WriteLiteral("\n                                </button>\n                            ");
						});
						this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = base.CreateTagHelper<FormTagHelper>();
						this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
						this.__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = base.CreateTagHelper<RenderAtEndOfFormTagHelper>();
						this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
						this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.set_Controller((string)Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_2.get_Value());
						this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_2);
						this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.set_Action((string)Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_14.get_Value());
						this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_14);
						this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.set_Method((string)Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_4.get_Value());
						this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_4);
						await this.__tagHelperRunner.RunAsync(this.__tagHelperExecutionContext);
						if (!this.__tagHelperExecutionContext.get_Output().get_IsContentModified())
						{
							await this.__tagHelperExecutionContext.SetOutputContentAsync();
						}
						this.Write(this.__tagHelperExecutionContext.get_Output());
						this.__tagHelperExecutionContext = this.__tagHelperScopeManager.End();
						this.WriteLiteral("\n");
					}
					this.WriteLiteral("                    </td>\n                </tr>\n");
				}
				this.WriteLiteral("        </table>\n\n        ");
				this.__tagHelperExecutionContext = this.__tagHelperScopeManager.Begin("form", 0, "f69c10b5b1aab911f22074791651cdd1f0154dc25cfb10ea378c274b57f83b5d33500", async () => {
					this.WriteLiteral("\n");
					foreach (ExternalProvider externalProvider in 
						from x in base.get_Model().ExternalProviders
						where base.get_Model().ExternalLogins.All<UserLoginInfo>((UserLoginInfo y) => x.AuthenticationScheme != y.get_LoginProvider())
						select x)
					{
						string lowerInvariant = externalProvider.AuthenticationScheme.ToLowerInvariant();
						this.WriteLiteral("            <button");
						this.BeginWriteAttribute("class", " class=\"", 0x1082, "\"", 0x10af, 4);
						base.WriteAttributeValue("", 0x108a, "btn", 0x108a, 3, true);
						base.WriteAttributeValue(" ", 0x108d, "external-button-small", 0x108e, 22, true);
						base.WriteAttributeValue(" ", 0x10a3, "btn-", 0x10a4, 5, true);
						base.WriteAttributeValue("", 0x10a8, lowerInvariant, 0x10a8, 7, false);
						this.EndWriteAttribute();
						this.WriteLiteral(" type=\"submit\" name=\"provider\"");
						this.BeginWriteAttribute("value", " value=\"", 0x10ce, "\"", 0x10f4, 1);
						base.WriteAttributeValue("", 0x10d6, externalProvider.AuthenticationScheme, 0x10d6, 30, false);
						this.EndWriteAttribute();
						this.WriteLiteral(">\n                    <i");
						this.BeginWriteAttribute("class", " class=\"", 0x110d, "\"", 0x112f, 3);
						base.WriteAttributeValue("", 0x1115, "icon-", 0x1115, 5, true);
						base.WriteAttributeValue("", 0x111a, lowerInvariant, 0x111a, 7, false);
						base.WriteAttributeValue(" ", 0x1121, "external-icon", 0x1122, 14, true);
						this.EndWriteAttribute();
						this.WriteLiteral("></i>\n                </button>\n");
					}
					this.WriteLiteral("        ");
				});
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = base.CreateTagHelper<FormTagHelper>();
				this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = base.CreateTagHelper<RenderAtEndOfFormTagHelper>();
				this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.set_Controller((string)Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_2.get_Value());
				this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_2);
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.set_Action((string)Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_15.get_Value());
				this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_15);
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.set_Method((string)Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_4.get_Value());
				this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_4);
				await this.__tagHelperRunner.RunAsync(this.__tagHelperExecutionContext);
				if (!this.__tagHelperExecutionContext.get_Output().get_IsContentModified())
				{
					await this.__tagHelperExecutionContext.SetOutputContentAsync();
				}
				this.Write(this.__tagHelperExecutionContext.get_Output());
				this.__tagHelperExecutionContext = this.__tagHelperScopeManager.End();
				this.WriteLiteral("\n    </div>\n");
			}
			this.WriteLiteral("\n");
			if (base.get_Model().HasPasswordAuth)
			{
				this.WriteLiteral("    <hr />\n");
				this.WriteLiteral("    <div class=\"profile-section\">\n        <h2>");
				this.Write(T.Get("users.profile.passwordTitle", null));
				this.WriteLiteral("</h2>\n\n");
				if (!base.get_Model().HasPassword)
				{
					this.WriteLiteral("        ");
					this.__tagHelperExecutionContext = this.__tagHelperScopeManager.Begin("form", 0, "f69c10b5b1aab911f22074791651cdd1f0154dc25cfb10ea378c274b57f83b5d43160", async () => {
						this.WriteLiteral("\n                <div class=\"form-group\">\n                    <label for=\"password\">");
						this.Write(T.Get("common.password", null));
						this.WriteLiteral("</label>\n\n");
						this.u003cExecuteAsyncu003eg__RenderValidationu007c31_0("Password");
						this.WriteLiteral("\n                    <input type=\"password\" class=\"form-control\" name=\"password\" id=\"password\" />\n                </div>\n\n                <div class=\"form-group\">\n                    <label for=\"passwordConfirm\">");
						this.Write(T.Get("users.profile.confirmPassword", null));
						this.WriteLiteral("</label>\n\n");
						this.u003cExecuteAsyncu003eg__RenderValidationu007c31_0("PasswordConfirm");
						this.WriteLiteral("\n                    <input type=\"password\" class=\"form-control\" name=\"passwordConfirm\" id=\"passwordConfirm\" />\n                </div>\n\n                <div class=\"form-group\">\n                    <button type=\"submit\" class=\"btn btn-primary\">");
						this.Write(T.Get("users.profile.setPassword", null));
						this.WriteLiteral("</button>\n                </div>\n            ");
					});
					this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = base.CreateTagHelper<FormTagHelper>();
					this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
					this.__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = base.CreateTagHelper<RenderAtEndOfFormTagHelper>();
					this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
					this.__tagHelperExecutionContext.AddHtmlAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_16);
					this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.set_Controller((string)Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_2.get_Value());
					this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_2);
					this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.set_Action((string)Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_18.get_Value());
					this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_18);
					this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.set_Method((string)Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_4.get_Value());
					this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_4);
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
					this.WriteLiteral("        ");
					this.__tagHelperExecutionContext = this.__tagHelperScopeManager.Begin("form", 0, "f69c10b5b1aab911f22074791651cdd1f0154dc25cfb10ea378c274b57f83b5d38508", async () => {
						this.WriteLiteral("\n                <div class=\"form-group\">\n                    <label for=\"oldPassword\">");
						this.Write(T.Get("common.oldPassword", null));
						this.WriteLiteral("</label>\n\n");
						this.u003cExecuteAsyncu003eg__RenderValidationu007c31_0("OldPassword");
						this.WriteLiteral("\n                    <input type=\"password\" class=\"form-control\" name=\"oldPassword\" id=\"oldPassword\" />\n                </div>\n\n                <div class=\"form-group\">\n                    <label for=\"password\">");
						this.Write(T.Get("common.password", null));
						this.WriteLiteral("</label>\n\n");
						this.u003cExecuteAsyncu003eg__RenderValidationu007c31_0("Password");
						this.WriteLiteral("\n                    <input type=\"password\" class=\"form-control\" name=\"password\" id=\"password\" />\n                </div>\n\n                <div class=\"form-group\">\n                    <label for=\"passwordConfirm\">");
						this.Write(T.Get("users.profile.confirmPassword", null));
						this.WriteLiteral("</label>\n\n");
						this.u003cExecuteAsyncu003eg__RenderValidationu007c31_0("PasswordConfirm");
						this.WriteLiteral("\n                    <input type=\"password\" class=\"form-control\" name=\"passwordConfirm\" id=\"passwordConfirm\" />\n                </div>\n\n                <div class=\"form-group\">\n                    <button type=\"submit\" class=\"btn btn-primary\">");
						this.Write(T.Get("users.profile.changePassword", null));
						this.WriteLiteral("</button>\n                </div>\n            ");
					});
					this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = base.CreateTagHelper<FormTagHelper>();
					this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
					this.__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = base.CreateTagHelper<RenderAtEndOfFormTagHelper>();
					this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
					this.__tagHelperExecutionContext.AddHtmlAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_16);
					this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.set_Controller((string)Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_2.get_Value());
					this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_2);
					this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.set_Action((string)Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_17.get_Value());
					this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_17);
					this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.set_Method((string)Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_4.get_Value());
					this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_4);
					await this.__tagHelperRunner.RunAsync(this.__tagHelperExecutionContext);
					if (!this.__tagHelperExecutionContext.get_Output().get_IsContentModified())
					{
						await this.__tagHelperExecutionContext.SetOutputContentAsync();
					}
					this.Write(this.__tagHelperExecutionContext.get_Output());
					this.__tagHelperExecutionContext = this.__tagHelperScopeManager.End();
					this.WriteLiteral("\n");
				}
				this.WriteLiteral("    </div>\n");
			}
			this.WriteLiteral("\n<hr />\n\n<div class=\"profile-section\">\n    <h2>");
			this.Write(T.Get("users.profile.clientTitle", null));
			this.WriteLiteral("</h2>\n\n    <small class=\"form-text text-muted mt-2 mb-2\">");
			this.Write(T.Get("users.profile.clientHint", null));
			this.WriteLiteral("</small>\n\n    <div class=\"row g-2 form-group\">\n        <div class=\"col-8\">\n            <label for=\"clientId\">");
			this.Write(T.Get("common.clientId", null));
			this.WriteLiteral("</label>\n\n            <input class=\"form-control\" name=\"clientId\" id=\"clientId\" readonly");
			this.BeginWriteAttribute("value", " value=\"", 0x1c51, "\"", 0x1c63, 1);
			base.WriteAttributeValue("", 0x1c59, base.get_Model().Id, 0x1c59, 10, false);
			this.EndWriteAttribute();
			this.WriteLiteral(" />\n        </div>\n    </div>\n    <div class=\"row g-2 form-group\">\n        <div class=\"col-8\">\n            <label for=\"clientSecret\">");
			this.Write(T.Get("common.clientSecret", null));
			this.WriteLiteral("</label>\n\n            <input class=\"form-control\" name=\"clientSecret\" id=\"clientSecret\" readonly");
			this.BeginWriteAttribute("value", " value=\"", 0x1d66, "\"", 0x1d82, 1);
			base.WriteAttributeValue("", 0x1d6e, base.get_Model().ClientSecret, 0x1d6e, 20, false);
			this.EndWriteAttribute();
			this.WriteLiteral(" />\n        </div>\n        <div class=\"col-4 pl-2\">\n            <label for=\"generate\">&nbsp;</label>\n\n            ");
			this.__tagHelperExecutionContext = this.__tagHelperScopeManager.Begin("form", 0, "f69c10b5b1aab911f22074791651cdd1f0154dc25cfb10ea378c274b57f83b5d49605", async () => {
				this.WriteLiteral("\n                <button type=\"submit\" class=\"btn btn-success btn-block\" id=\"generate\">");
				this.Write(T.Get("users.profile.generateClient", null));
				this.WriteLiteral("</button>\n            ");
			});
			this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = base.CreateTagHelper<FormTagHelper>();
			this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
			this.__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = base.CreateTagHelper<RenderAtEndOfFormTagHelper>();
			this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
			this.__tagHelperExecutionContext.AddHtmlAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_16);
			this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.set_Controller((string)Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_2.get_Value());
			this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_2);
			this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.set_Action((string)Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_19.get_Value());
			this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_19);
			this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.set_Method((string)Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_4.get_Value());
			this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_4);
			await this.__tagHelperRunner.RunAsync(this.__tagHelperExecutionContext);
			if (!this.__tagHelperExecutionContext.get_Output().get_IsContentModified())
			{
				await this.__tagHelperExecutionContext.SetOutputContentAsync();
			}
			this.Write(this.__tagHelperExecutionContext.get_Output());
			this.__tagHelperExecutionContext = this.__tagHelperScopeManager.End();
			this.WriteLiteral("\n        </div>\n    </div>\n</div>\n\n<hr />\n\n<div class=\"profile-section\">\n    <h2>");
			this.Write(T.Get("users.profile.propertiesTitle", null));
			this.WriteLiteral("</h2>\n\n    <small class=\"form-text text-muted mt-2 mb-2\">");
			this.Write(T.Get("users.profile.propertiesHint", null));
			this.WriteLiteral("</small>\n\n    ");
			this.__tagHelperExecutionContext = this.__tagHelperScopeManager.Begin("form", 0, "f69c10b5b1aab911f22074791651cdd1f0154dc25cfb10ea378c274b57f83b5d52516", async () => {
				int num = 0;
				this.WriteLiteral("\n        <div class=\"mb-2\" id=\"properties\">\n");
				for (int i = 0; i < base.get_Model().Properties.Count; i = num + 1)
				{
					this.WriteLiteral("            <div class=\"row g-2 form-group\">\n                    <div class=\"col-5 pr-2\">\n\n");
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(17, 1);
					defaultInterpolatedStringHandler.AppendLiteral("Properties[");
					defaultInterpolatedStringHandler.AppendFormatted<int>(i);
					defaultInterpolatedStringHandler.AppendLiteral("].Name");
					this.u003cExecuteAsyncu003eg__RenderValidationu007c31_0(defaultInterpolatedStringHandler.ToStringAndClear());
					this.WriteLiteral("\n                        ");
					this.__tagHelperExecutionContext = this.__tagHelperScopeManager.Begin("input", 1, "f69c10b5b1aab911f22074791651cdd1f0154dc25cfb10ea378c274b57f83b5d53465", async () => {
						Areas_IdentityServer_Views_Profile_Profile.u003cu003ec.u003cu003cExecuteAsyncu003eb__31_19u003ed _ = new Areas_IdentityServer_Views_Profile_Profile.u003cu003ec.u003cu003cExecuteAsyncu003eb__31_19u003ed();
						_.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
						_.u003cu003e1__state = -1;
						_.u003cu003et__builder.Start<Areas_IdentityServer_Views_Profile_Profile.u003cu003ec.u003cu003cExecuteAsyncu003eb__31_19u003ed>(ref _);
						return _.u003cu003et__builder.Task;
					});
					this.__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper = base.CreateTagHelper<InputTagHelper>();
					this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper);
					this.__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.set_InputTypeName((string)Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_8.get_Value());
					this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_8);
					this.__tagHelperExecutionContext.AddHtmlAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_7);
					this.__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.set_For(this.ModelExpressionProvider.CreateModelExpression<ProfileVM, string>(base.get_ViewData(), (ProfileVM __model) => __model.Properties[i].Name));
					this.__tagHelperExecutionContext.AddTagHelperAttribute("asp-for", this.__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.get_For(), 0);
					await this.__tagHelperRunner.RunAsync(this.__tagHelperExecutionContext);
					if (!this.__tagHelperExecutionContext.get_Output().get_IsContentModified())
					{
						await this.__tagHelperExecutionContext.SetOutputContentAsync();
					}
					this.Write(this.__tagHelperExecutionContext.get_Output());
					this.__tagHelperExecutionContext = this.__tagHelperScopeManager.End();
					this.WriteLiteral("\n                    </div>\n                    <div class=\"col pr-2\">\n\n");
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(18, 1);
					defaultInterpolatedStringHandler.AppendLiteral("Properties[");
					defaultInterpolatedStringHandler.AppendFormatted<int>(i);
					defaultInterpolatedStringHandler.AppendLiteral("].Value");
					this.u003cExecuteAsyncu003eg__RenderValidationu007c31_0(defaultInterpolatedStringHandler.ToStringAndClear());
					this.WriteLiteral("\n                        ");
					this.__tagHelperExecutionContext = this.__tagHelperScopeManager.Begin("input", 1, "f69c10b5b1aab911f22074791651cdd1f0154dc25cfb10ea378c274b57f83b5d55603", async () => {
						Areas_IdentityServer_Views_Profile_Profile.u003cu003ec.u003cu003cExecuteAsyncu003eb__31_21u003ed _ = new Areas_IdentityServer_Views_Profile_Profile.u003cu003ec.u003cu003cExecuteAsyncu003eb__31_21u003ed();
						_.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
						_.u003cu003e1__state = -1;
						_.u003cu003et__builder.Start<Areas_IdentityServer_Views_Profile_Profile.u003cu003ec.u003cu003cExecuteAsyncu003eb__31_21u003ed>(ref _);
						return _.u003cu003et__builder.Task;
					});
					this.__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper = base.CreateTagHelper<InputTagHelper>();
					this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper);
					this.__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.set_InputTypeName((string)Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_8.get_Value());
					this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_8);
					this.__tagHelperExecutionContext.AddHtmlAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_7);
					this.__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.set_For(this.ModelExpressionProvider.CreateModelExpression<ProfileVM, string>(base.get_ViewData(), (ProfileVM __model) => __model.Properties[i].Value));
					this.__tagHelperExecutionContext.AddTagHelperAttribute("asp-for", this.__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.get_For(), 0);
					await this.__tagHelperRunner.RunAsync(this.__tagHelperExecutionContext);
					if (!this.__tagHelperExecutionContext.get_Output().get_IsContentModified())
					{
						await this.__tagHelperExecutionContext.SetOutputContentAsync();
					}
					this.Write(this.__tagHelperExecutionContext.get_Output());
					this.__tagHelperExecutionContext = this.__tagHelperScopeManager.End();
					this.WriteLiteral("\n                    </div>\n                    <div class=\"col-auto\">\n                        <button type=\"button\" class=\"btn btn-text-danger remove-item\">\n                            <i class=\"icon-bin2\"></i>\n                        </button>\n                    </div>\n                </div>\n");
					num = i;
				}
				this.WriteLiteral("        </div>\n\n        <div class=\"form-group\">\n            <button type=\"button\" class=\"btn btn-outline-success\" id=\"propertyAdd\">\n                <i class=\"icon-plus\"></i> ");
				this.Write(T.Get("users.profile.propertyAdd", null));
				this.WriteLiteral("\n            </button>\n        </div>\n\n        <button type=\"submit\" class=\"btn btn-primary\">");
				this.Write(T.Get("common.save", null));
				this.WriteLiteral("</button>\n    ");
			});
			this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = base.CreateTagHelper<FormTagHelper>();
			this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
			this.__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = base.CreateTagHelper<RenderAtEndOfFormTagHelper>();
			this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
			this.__tagHelperExecutionContext.AddHtmlAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_16);
			this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.set_Controller((string)Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_2.get_Value());
			this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_2);
			this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.set_Action((string)Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_20.get_Value());
			this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_20);
			this.__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.set_Method((string)Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_4.get_Value());
			this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views_Profile_Profile.__tagHelperAttribute_4);
			await this.__tagHelperRunner.RunAsync(this.__tagHelperExecutionContext);
			if (!this.__tagHelperExecutionContext.get_Output().get_IsContentModified())
			{
				await this.__tagHelperExecutionContext.SetOutputContentAsync();
			}
			this.Write(this.__tagHelperExecutionContext.get_Output());
			this.__tagHelperExecutionContext = this.__tagHelperScopeManager.End();
			this.WriteLiteral("\n</div>\n\n<script>\n    var propertyPlusButton = document.getElementById('propertyAdd');\n    var propertiesDiv = document.getElementById('properties');\n    var pictureButton = document.getElementById('pictureButton');\n    var pictureInput = document.getElementById('pictureInput');\n    var pictureForm = document.getElementById('pictureForm');\n\n    function updateNames() {\n        for (var i = 0; i < propertiesDiv.children.length; i++) {\n            var child = propertiesDiv.children[i];\n\n            const inputs = child.getElementsByTagName('input');\n            inputs[0].name = `Properties[${i}].Name`;\n            inputs[1].name = `Properties[${i}].Value`;\n        }\n    }\n\n    document.addEventListener('click',\n        function (event) {\n            if (event.target.className.indexOf('remove-item') >= 0) {\n                event.target.parentNode.parentNode.remove();\n\n                updateNames();\n            }\n        });\n\n    pictureButton.addEventListener('click',\n        function () {\n            pictureInp");
			this.WriteLiteral("ut.click();\n        });\n\n    pictureInput.addEventListener('change',\n        function () {\n            pictureForm.submit();\n        });\n\n    propertyPlusButton.addEventListener('click',\n        function () {\n            var template = document.createElement('template');\n\n            template.innerHTML =\n                `<div class=\"row g-2 form-group\">\n                    <div class=\"col-5 pr-2\">\n                        <input class=\"form-control\" />\n                    </div>\n                    <div class=\"col pr-2\">\n                        <input class=\"form-control\" />\n                    </div>\n                    <div class=\"col-auto\">\n                        <button type=\"button\" class=\"btn btn-danger\">\n                            <i class=\"icon-bin\"></i>\n                        </button>\n                    </div>\n                </div>`;\n\n            propertiesDiv.append(template.content.firstChild);\n\n            updateNames();\n        });\n\n    var successMessage = document.getElementById('success')");
			this.WriteLiteral(";\n\n    if (successMessage) {\n        setTimeout(function () {\n            successMessage.remove();\n        }, 5000);\n    }\n</script>\n");
		}
	}
}