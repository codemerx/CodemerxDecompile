using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Extensions
{
	public static class MixUrlHelperExtensions
	{
		public static string AbsoluteAction(this IUrlHelper url, string actionName, string controllerName, object routeValues = null)
		{
			return UrlHelperExtensions.Action(url, actionName, controllerName, routeValues, url.get_ActionContext().get_HttpContext().get_Request().get_Scheme());
		}

		public static string AbsoluteContent(this IUrlHelper url, string contentPath)
		{
			V_0 = url.get_ActionContext().get_HttpContext().get_Request();
			stackVariable5 = V_0.get_Scheme();
			V_1 = V_0.get_Host();
			return (new Uri(new Uri(string.Concat(stackVariable5, "://", V_1.get_Value())), url.Content(contentPath))).ToString();
		}

		public static string AbsoluteRouteUrl(this IUrlHelper url, string routeName, object routeValues = null)
		{
			return UrlHelperExtensions.RouteUrl(url, routeName, routeValues, url.get_ActionContext().get_HttpContext().get_Request().get_Scheme());
		}
	}
}