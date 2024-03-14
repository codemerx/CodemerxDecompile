using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
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
	[RazorCompiledItemMetadata("Identifier", "/Areas/IdentityServer/Views/_Layout.cshtml")]
	[RazorSourceChecksum("SHA256", "85fc3c0474348f253e633bddac705b95a29ecf682dc381e0e011137390411ec6", "/Areas/IdentityServer/Views/_Layout.cshtml")]
	[RazorSourceChecksum("SHA256", "96902ca7bc9007282c2aa6ba41ece2d109a87d8ca7bd90eaeb5d11ba945ca083", "/Areas/IdentityServer/Views/_ViewImports.cshtml")]
	internal sealed class Areas_IdentityServer_Views__Layout : RazorPage<object>
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
		private TagHelperExecutionContext __tagHelperExecutionContext;

		[Nullable(0)]
		private TagHelperRunner __tagHelperRunner = new TagHelperRunner();

		[Nullable(0)]
		private string __tagHelperStringValueBuffer;

		[Nullable(0)]
		private TagHelperScopeManager __backed__tagHelperScopeManager;

		[Nullable(0)]
		private HeadTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper;

		[Nullable(0)]
		private UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;

		[Nullable(0)]
		private LinkTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_LinkTagHelper;

		[Nullable(0)]
		private EnvironmentTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_EnvironmentTagHelper;

		[Nullable(0)]
		private BodyTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper;

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

		static Areas_IdentityServer_Views__Layout()
		{
			Areas_IdentityServer_Views__Layout.__tagHelperAttribute_0 = new TagHelperAttribute("href", new HtmlString("~/"), 0);
			Areas_IdentityServer_Views__Layout.__tagHelperAttribute_1 = new TagHelperAttribute("rel", new HtmlString("stylesheet"), 0);
			Areas_IdentityServer_Views__Layout.__tagHelperAttribute_2 = new TagHelperAttribute("href", "styles.css", 0);
			Areas_IdentityServer_Views__Layout.__tagHelperAttribute_3 = new TagHelperAttribute("include", "Development", 0);
			Areas_IdentityServer_Views__Layout.__tagHelperAttribute_4 = new TagHelperAttribute("class", new HtmlString("profile"), 0);
		}

		public Areas_IdentityServer_Views__Layout()
		{
		}

		[NullableContext(0)]
		public override async Task ExecuteAsync()
		{
			this.WriteLiteral("<!DOCTYPE html>\n<html>\n");
			this.__tagHelperExecutionContext = this.__tagHelperScopeManager.Begin("head", 0, "85fc3c0474348f253e633bddac705b95a29ecf682dc381e0e011137390411ec65877", async () => {
				this.WriteLiteral("\n    ");
				this.__tagHelperExecutionContext = this.__tagHelperScopeManager.Begin("base", 2, "85fc3c0474348f253e633bddac705b95a29ecf682dc381e0e011137390411ec66159", async () => {
					Areas_IdentityServer_Views__Layout.u003cu003ec.u003cu003cExecuteAsyncu003eb__16_2u003ed _ = new Areas_IdentityServer_Views__Layout.u003cu003ec.u003cu003cExecuteAsyncu003eb__16_2u003ed();
					_.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
					_.u003cu003e1__state = -1;
					_.u003cu003et__builder.Start<Areas_IdentityServer_Views__Layout.u003cu003ec.u003cu003cExecuteAsyncu003eb__16_2u003ed>(ref _);
					return _.u003cu003et__builder.Task;
				});
				this.__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = base.CreateTagHelper<UrlResolutionTagHelper>();
				this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
				this.__tagHelperExecutionContext.AddHtmlAttribute(Areas_IdentityServer_Views__Layout.__tagHelperAttribute_0);
				await this.__tagHelperRunner.RunAsync(this.__tagHelperExecutionContext);
				if (!this.__tagHelperExecutionContext.get_Output().get_IsContentModified())
				{
					await this.__tagHelperExecutionContext.SetOutputContentAsync();
				}
				this.Write(this.__tagHelperExecutionContext.get_Output());
				this.__tagHelperExecutionContext = this.__tagHelperScopeManager.End();
				this.WriteLiteral("\n\n    <meta charset=\"UTF-8\">\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">\n\n    <title>");
				this.Write(((dynamic)base.get_ViewBag()).Title);
				this.WriteLiteral(" - ");
				this.Write(T.Get("common.product", null));
				this.WriteLiteral("</title>\n\n    ");
				this.__tagHelperExecutionContext = this.__tagHelperScopeManager.Begin("link", 1, "85fc3c0474348f253e633bddac705b95a29ecf682dc381e0e011137390411ec67813", async () => {
					Areas_IdentityServer_Views__Layout.u003cu003ec.u003cu003cExecuteAsyncu003eb__16_3u003ed _ = new Areas_IdentityServer_Views__Layout.u003cu003ec.u003cu003cExecuteAsyncu003eb__16_3u003ed();
					_.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
					_.u003cu003e1__state = -1;
					_.u003cu003et__builder.Start<Areas_IdentityServer_Views__Layout.u003cu003ec.u003cu003cExecuteAsyncu003eb__16_3u003ed>(ref _);
					return _.u003cu003et__builder.Task;
				});
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_LinkTagHelper = base.CreateTagHelper<LinkTagHelper>();
				this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_LinkTagHelper);
				this.__tagHelperExecutionContext.AddHtmlAttribute(Areas_IdentityServer_Views__Layout.__tagHelperAttribute_1);
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_LinkTagHelper.set_AppendVersion(new bool?(true));
				this.__tagHelperExecutionContext.AddTagHelperAttribute("asp-append-version", this.__Microsoft_AspNetCore_Mvc_TagHelpers_LinkTagHelper.get_AppendVersion(), 0);
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_LinkTagHelper.set_Href((string)Areas_IdentityServer_Views__Layout.__tagHelperAttribute_2.get_Value());
				this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views__Layout.__tagHelperAttribute_2);
				await this.__tagHelperRunner.RunAsync(this.__tagHelperExecutionContext);
				if (!this.__tagHelperExecutionContext.get_Output().get_IsContentModified())
				{
					await this.__tagHelperExecutionContext.SetOutputContentAsync();
				}
				this.Write(this.__tagHelperExecutionContext.get_Output());
				this.__tagHelperExecutionContext = this.__tagHelperScopeManager.End();
				this.WriteLiteral("\n    \n");
				if (base.IsSectionDefined("header"))
				{
					this.Write(await base.RenderSectionAsync("header"));
				}
				this.WriteLiteral("\n    ");
				this.__tagHelperExecutionContext = this.__tagHelperScopeManager.Begin("environment", 0, "85fc3c0474348f253e633bddac705b95a29ecf682dc381e0e011137390411ec610106", async () => this.WriteLiteral("\n        <script type=\"text/javascript\" src=\"runtime.js\"></script>\n        <script type=\"text/javascript\" src=\"polyfills.js\"></script>\n    "));
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_EnvironmentTagHelper = base.CreateTagHelper<EnvironmentTagHelper>();
				this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_TagHelpers_EnvironmentTagHelper);
				this.__Microsoft_AspNetCore_Mvc_TagHelpers_EnvironmentTagHelper.set_Include((string)Areas_IdentityServer_Views__Layout.__tagHelperAttribute_3.get_Value());
				this.__tagHelperExecutionContext.AddTagHelperAttribute(Areas_IdentityServer_Views__Layout.__tagHelperAttribute_3);
				await this.__tagHelperRunner.RunAsync(this.__tagHelperExecutionContext);
				if (!this.__tagHelperExecutionContext.get_Output().get_IsContentModified())
				{
					await this.__tagHelperExecutionContext.SetOutputContentAsync();
				}
				this.Write(this.__tagHelperExecutionContext.get_Output());
				this.__tagHelperExecutionContext = this.__tagHelperScopeManager.End();
				this.WriteLiteral("\n");
			});
			this.__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper = base.CreateTagHelper<HeadTagHelper>();
			this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper);
			await this.__tagHelperRunner.RunAsync(this.__tagHelperExecutionContext);
			if (!this.__tagHelperExecutionContext.get_Output().get_IsContentModified())
			{
				await this.__tagHelperExecutionContext.SetOutputContentAsync();
			}
			this.Write(this.__tagHelperExecutionContext.get_Output());
			this.__tagHelperExecutionContext = this.__tagHelperScopeManager.End();
			this.WriteLiteral("\n");
			this.__tagHelperExecutionContext = this.__tagHelperScopeManager.Begin("body", 0, "85fc3c0474348f253e633bddac705b95a29ecf682dc381e0e011137390411ec612201", async () => {
				this.WriteLiteral("\n    <div class=\"profile-container\">\n        <img class=\"profile-logo\"");
				this.BeginWriteAttribute("alt", " alt=\"", 0x29d, "\"", 0x2bb, 1);
				base.WriteAttributeValue("", 0x2a3, T.Get("common.product", null), 0x2a3, 24, false);
				this.EndWriteAttribute();
				this.BeginWriteAttribute("title", " title=\"", 0x2bc, "\"", 0x2dc, 1);
				base.WriteAttributeValue("", 0x2c4, T.Get("common.product", null), 0x2c4, 24, false);
				this.EndWriteAttribute();
				this.WriteLiteral(" src=\"images/logo.svg\" />\n\n        <div class=\"profile-card card\">\n            <div class=\"profile-card-body card-body\">\n                ");
				this.Write(this.RenderBody());
				this.WriteLiteral("\n            </div>\n        </div>\n\n        <div class=\"profile-footer text-center mt-4 mb-2\">\n            <small class=\"text-muted\">\n                ");
				this.Write(T.Get("setup.madeBy", null));
				this.WriteLiteral("<br />");
				this.Write(T.Get("setup.madeByCopyright", null));
				this.WriteLiteral("\n            </small>\n        </div>\n    </div>\n");
			});
			this.__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper = base.CreateTagHelper<BodyTagHelper>();
			this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper);
			this.__tagHelperExecutionContext.AddHtmlAttribute(Areas_IdentityServer_Views__Layout.__tagHelperAttribute_4);
			await this.__tagHelperRunner.RunAsync(this.__tagHelperExecutionContext);
			if (!this.__tagHelperExecutionContext.get_Output().get_IsContentModified())
			{
				await this.__tagHelperExecutionContext.SetOutputContentAsync();
			}
			this.Write(this.__tagHelperExecutionContext.get_Output());
			this.__tagHelperExecutionContext = this.__tagHelperScopeManager.End();
			this.WriteLiteral("\n</html>");
		}
	}
}