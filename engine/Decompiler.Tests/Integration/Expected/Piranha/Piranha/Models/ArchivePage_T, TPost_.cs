using Piranha;
using System;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	[NoCoverage]
	[Obsolete("ArchivePage is obsolete. Please use a regular Page with IsArchive = true")]
	[Serializable]
	public class ArchivePage<T, TPost> : GenericPage<T>, IArchivePage
	where T : ArchivePage<T, TPost>
	where TPost : PostBase
	{
		public PostArchive<TPost> Archive
		{
			get;
			set;
		}

		public ArchivePage()
		{
			base();
			this.set_Archive(new PostArchive<TPost>());
			return;
		}
	}
}