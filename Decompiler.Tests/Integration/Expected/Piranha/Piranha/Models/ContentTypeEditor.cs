using System;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	[Serializable]
	public sealed class ContentTypeEditor
	{
		public string Component
		{
			get;
			set;
		}

		public string Icon
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public ContentTypeEditor()
		{
			base();
			return;
		}
	}
}