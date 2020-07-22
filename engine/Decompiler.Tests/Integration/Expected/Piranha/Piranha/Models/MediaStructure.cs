using System;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	[Serializable]
	public class MediaStructure : Structure<MediaStructure, MediaStructureItem>
	{
		public int MediaCount
		{
			get;
			set;
		}

		public int TotalCount
		{
			get;
			set;
		}

		public MediaStructure()
		{
			base();
			return;
		}
	}
}