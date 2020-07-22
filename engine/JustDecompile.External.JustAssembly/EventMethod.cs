using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JustDecompile.External.JustAssembly
{
	class EventMethod : IEventMethod
	{
		public EventMethodType EventMethodType { get; private set; }

		public uint EventMethodToken { get; private set; }

		public EventMethod(EventMethodType eventMethodType, uint eventMethodToken)
		{
			this.EventMethodType = eventMethodType;
			this.EventMethodToken = eventMethodToken;
		}
	}
}
