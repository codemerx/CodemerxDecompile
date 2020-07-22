using System;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	public interface IDynamicContent
	{
		dynamic Regions
		{
			get;
			set;
		}
	}
}