using System;

namespace Piranha.Models
{
	public interface IMeta
	{
		string MetaDescription
		{
			get;
			set;
		}

		string MetaKeywords
		{
			get;
			set;
		}

		string Title
		{
			get;
			set;
		}
	}
}