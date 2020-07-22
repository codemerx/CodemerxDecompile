using System;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	[Serializable]
	public class MediaStructureItem : StructureItem<MediaStructure, MediaStructureItem>
	{
		public DateTime Created
		{
			get;
			set;
		}

		public int FolderCount
		{
			get;
			set;
		}

		public int MediaCount
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public MediaStructureItem()
		{
			base();
			this.set_Items(new MediaStructure());
			return;
		}
	}
}