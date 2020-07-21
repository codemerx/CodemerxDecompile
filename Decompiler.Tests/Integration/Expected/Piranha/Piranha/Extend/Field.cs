using System;

namespace Piranha.Extend
{
	public abstract class Field : IField
	{
		protected Field()
		{
			base();
			return;
		}

		public abstract string GetTitle();
	}
}