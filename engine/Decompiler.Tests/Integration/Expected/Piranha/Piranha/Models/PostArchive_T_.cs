using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	[Serializable]
	public class PostArchive<T>
	where T : PostBase
	{
		public Taxonomy Category
		{
			get;
			set;
		}

		public int CurrentPage
		{
			get;
			set;
		}

		public int? Month
		{
			get;
			set;
		}

		public IList<T> Posts
		{
			get;
			set;
		}

		public Taxonomy Tag
		{
			get;
			set;
		}

		public int TotalPages
		{
			get;
			set;
		}

		public int TotalPosts
		{
			get;
			set;
		}

		public int? Year
		{
			get;
			set;
		}

		public PostArchive()
		{
		}
	}
}