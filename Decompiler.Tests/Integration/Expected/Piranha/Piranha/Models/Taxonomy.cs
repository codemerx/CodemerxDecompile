using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	[Serializable]
	public class Taxonomy
	{
		public Guid Id
		{
			get;
			set;
		}

		[StringLength(128)]
		public string Slug
		{
			get;
			set;
		}

		[StringLength(128)]
		public string Title
		{
			get;
			set;
		}

		public TaxonomyType Type
		{
			get;
			set;
		}

		public Taxonomy()
		{
			base();
			return;
		}

		public static implicit operator Taxonomy(string str)
		{
			stackVariable0 = new Taxonomy();
			stackVariable0.set_Title(str);
			return stackVariable0;
		}
	}
}