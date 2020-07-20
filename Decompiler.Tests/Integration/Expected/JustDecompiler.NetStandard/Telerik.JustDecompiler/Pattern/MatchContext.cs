using System;
using System.Collections.Generic;
using System.Reflection;

namespace Telerik.JustDecompiler.Pattern
{
	public class MatchContext
	{
		private bool success;

		private Dictionary<string, object> datas;

		public object this[string name]
		{
			get
			{
				return this.get_Store().get_Item(name);
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
				return;
			}
		}

		public MatchContext()
		{
			this.success = true;
			base();
			return;
		}

		public void AddData(MatchData data)
		{
			this.get_Store().set_Item(data.Name, data.Value);
			return;
		}

		public bool TryGetData(string name, out object value)
		{
			if (this.datas == null)
			{
				value = null;
				return false;
			}
			return this.get_Store().TryGetValue(name, out value);
		}
	}
}