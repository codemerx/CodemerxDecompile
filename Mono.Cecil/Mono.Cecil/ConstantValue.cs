using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil.Metadata;

namespace Mono.Cecil.Mono.Cecil
{
	/*Telerik Authorship*/
	public class ConstantValue
	{
		public object Value { get; set; }
		public ElementType Type { get; set; }

		public ConstantValue(object value, ElementType type)
		{
			this.Value = value;
			this.Type = type;
		}

		internal ConstantValue()
		{
		}
	}
}
