using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.IdentityServer.Views
{
	public static class Extensions
	{
		[NullableContext(1)]
		[return: Nullable(2)]
		public static string RootContentUrl(IUrlHelper urlHelper, string contentPath)
		{
			if (string.IsNullOrEmpty(contentPath))
			{
				return null;
			}
			if (contentPath[0] != '~')
			{
				return contentPath;
			}
			string str = contentPath;
			PathString pathString = new PathString(str.Substring(1, str.Length - 1));
			PathString pathBase = urlHelper.get_ActionContext().get_HttpContext().get_Request().get_PathBase();
			if (pathBase.get_Value() != null)
			{
				int num = pathBase.get_Value().LastIndexOf('/');
				if (num >= 0)
				{
					pathBase = pathBase.get_Value().Substring(0, num);
				}
			}
			return pathBase.Add(pathString).get_Value();
		}
	}
}