using Piranha.Services;
using System;

namespace Piranha
{
	public interface IApi : IDisposable
	{
		IAliasService Aliases
		{
			get;
		}

		IArchiveService Archives
		{
			get;
		}

		IMediaService Media
		{
			get;
		}

		IPageService Pages
		{
			get;
		}

		IPageTypeService PageTypes
		{
			get;
		}

		IParamService Params
		{
			get;
		}

		IPostService Posts
		{
			get;
		}

		IPostTypeService PostTypes
		{
			get;
		}

		ISiteService Sites
		{
			get;
		}

		ISiteTypeService SiteTypes
		{
			get;
		}
	}
}