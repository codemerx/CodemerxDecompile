using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JustDecompile.External.JustAssembly
{
	class PropertyMethod : IPropertyMethod
	{
		public PropertyMethodType PropertyMethodType { get; private set; }

		public uint PropertyMethodToken { get; private set; }

		public PropertyMethod(PropertyMethodType propertyMethodType, uint propertyMethodToken)
		{
			this.PropertyMethodType = propertyMethodType;
			this.PropertyMethodToken = propertyMethodToken;
		}
	}
}
