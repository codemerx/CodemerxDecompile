using Piranha;
using System;

namespace Piranha.Models
{
	[NoCoverage]
	[Obsolete("ArchivePage is obsolete. Please use a regular Page with IsArchive = true")]
	[Serializable]
	public class ArchivePage<T> : ArchivePage<T, DynamicPost>
	where T : ArchivePage<T>
	{
		public ArchivePage()
		{
		}
	}
}