using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Hosting;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Squidex.Areas.Api.Controllers;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace AspNetCoreGeneratedDocument
{
	[CreateNewOnMetadataUpdate]
	[Nullable(new byte[] { 0, 1 })]
	[NullableContext(1)]
	[RazorCompiledItemMetadata("Identifier", "/Areas/Api/Views/Shared/Docs.cshtml")]
	[RazorSourceChecksum("SHA256", "408543084e6ec112f2adfcdd180390940a17504ab38008ec549f2f74bfcecd11", "/Areas/Api/Views/Shared/Docs.cshtml")]
	internal sealed class Areas_Api_Views_Shared_Docs : RazorPage<DocsVM>
	{
		[Nullable(0)]
		private readonly static TagHelperAttribute __tagHelperAttribute_0;

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
		public IHtmlHelper<DocsVM> Html
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

		static Areas_Api_Views_Shared_Docs()
		{
			Areas_Api_Views_Shared_Docs.__tagHelperAttribute_0 = new TagHelperAttribute("href", new HtmlString("~/"), 0);
		}

		public Areas_Api_Views_Shared_Docs()
		{
		}

		[NullableContext(0)]
		public override async Task ExecuteAsync()
		{
			this.WriteLiteral("\n");
			base.set_Layout(null);
			this.WriteLiteral("\n<!DOCTYPE html>\n<html>\n");
			this.__tagHelperExecutionContext = this.__tagHelperScopeManager.Begin("head", 0, "408543084e6ec112f2adfcdd180390940a17504ab38008ec549f2f74bfcecd113739", async () => {
				this.WriteLiteral("\n    <title>API Docs</title>\n    \n    ");
				this.__tagHelperExecutionContext = this.__tagHelperScopeManager.Begin("base", 2, "408543084e6ec112f2adfcdd180390940a17504ab38008ec549f2f74bfcecd114056", async () => {
					Areas_Api_Views_Shared_Docs.u003cu003ec.u003cu003cExecuteAsyncu003eb__10_2u003ed _ = new Areas_Api_Views_Shared_Docs.u003cu003ec.u003cu003cExecuteAsyncu003eb__10_2u003ed();
					_.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
					_.u003cu003e1__state = -1;
					_.u003cu003et__builder.Start<Areas_Api_Views_Shared_Docs.u003cu003ec.u003cu003cExecuteAsyncu003eb__10_2u003ed>(ref _);
					return _.u003cu003et__builder.Task;
				});
				this.__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = base.CreateTagHelper<UrlResolutionTagHelper>();
				this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
				this.__tagHelperExecutionContext.AddHtmlAttribute(Areas_Api_Views_Shared_Docs.__tagHelperAttribute_0);
				await this.__tagHelperRunner.RunAsync(this.__tagHelperExecutionContext);
				if (!this.__tagHelperExecutionContext.get_Output().get_IsContentModified())
				{
					await this.__tagHelperExecutionContext.SetOutputContentAsync();
				}
				this.Write(this.__tagHelperExecutionContext.get_Output());
				this.__tagHelperExecutionContext = this.__tagHelperScopeManager.End();
				this.WriteLiteral("\n\n    <meta charset=\"utf-8\" />\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">\n\n    <style>\n        body { margin: 0; padding: 0; }\n\n        .menu-content {\n            position: fixed !important;\n        }\n\n        .api-content {\n            margin-left: 260px;\n        }\n\n        ");
				this.WriteLiteral("@media print, screen and (max-width: 50rem) {\n            .api-content {\n                margin-left: 0;\n            }\n        }\n    </style>\n");
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
			this.__tagHelperExecutionContext = this.__tagHelperScopeManager.Begin("body", 0, "408543084e6ec112f2adfcdd180390940a17504ab38008ec549f2f74bfcecd116340", async () => {
				this.WriteLiteral("\n    <div id=\"redoc-container\"></div>\n\n    <script src=\"https://cdn.jsdelivr.net/npm/redoc@2.0.0-rc.57/bundles/redoc.standalone.js\"></script>\n    <script>\n        Redoc.init('");
				this.Write(this.Url.Content(base.get_Model().Specification));
				this.WriteLiteral("', {\n            theme: {\n                colors: {\n                    primary: {\n                        main: '#3f83df'\n                    }\n                }\n            },\n            nativeScrollbars: true\n        }, document.getElementById('redoc-container'))\n    </script>\n");
			});
			this.__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper = base.CreateTagHelper<BodyTagHelper>();
			this.__tagHelperExecutionContext.Add(this.__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper);
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