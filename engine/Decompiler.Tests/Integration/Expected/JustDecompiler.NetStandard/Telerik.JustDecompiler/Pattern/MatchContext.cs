using System;
using System.Collections.Generic;
using System.Reflection;

namespace Telerik.JustDecompiler.Pattern
{
	public class MatchContext
	{
		private bool success = true;

		private Dictionary<string, object> datas;

		public object this[string name]
		{
			get
			{
				return this.Store[name];
			}
		}

		private Dictionary<string, object> Store
		{
			get
			{
				if (this.datas == null)
				{
					this.datas = new Dictionary<string, object>();
				}
				return this.datas;
			}
		}

		public bool Success
		{
			get
			{
				return this.success;
			}
			set
			{
				this.success = value;
			}
		}

		public MatchContext()
		{
		}

		public void AddData(MatchData data)
		{
			this.Store[data.Name] = data.Value;
		}

		public bool TryGetData(string name, out object value)
		{
			if (this.datas == null)
			{
				value = null;
				return false;
			}
			return this.Store.TryGetValue(name, out value);
		}
	}
}