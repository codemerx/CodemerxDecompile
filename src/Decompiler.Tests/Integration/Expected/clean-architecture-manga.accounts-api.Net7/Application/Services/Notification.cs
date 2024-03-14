using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Application.Services
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class Notification
	{
		private readonly IDictionary<string, IList<string>> _errorMessages = new Dictionary<string, IList<string>>();

		public bool IsInvalid
		{
			get
			{
				return this._errorMessages.get_Count() > 0;
			}
		}

		public bool IsValid
		{
			get
			{
				return this._errorMessages.get_Count() == 0;
			}
		}

		public IDictionary<string, string[]> ModelState
		{
			get
			{
				IDictionary<string, IList<string>> dictionary = this._errorMessages;
				Func<KeyValuePair<string, IList<string>>, string> u003cu003e9_20 = Notification.u003cu003ec.u003cu003e9__2_0;
				if (u003cu003e9_20 == null)
				{
					u003cu003e9_20 = new Func<KeyValuePair<string, IList<string>>, string>(Notification.u003cu003ec.u003cu003e9, (KeyValuePair<string, IList<string>> item) => item.get_Key());
					Notification.u003cu003ec.u003cu003e9__2_0 = u003cu003e9_20;
				}
				Func<KeyValuePair<string, IList<string>>, string[]> u003cu003e9_21 = Notification.u003cu003ec.u003cu003e9__2_1;
				if (u003cu003e9_21 == null)
				{
					u003cu003e9_21 = new Func<KeyValuePair<string, IList<string>>, string[]>(Notification.u003cu003ec.u003cu003e9, (KeyValuePair<string, IList<string>> item) => Enumerable.ToArray<string>(item.get_Value()));
					Notification.u003cu003ec.u003cu003e9__2_1 = u003cu003e9_21;
				}
				return Enumerable.ToDictionary<KeyValuePair<string, IList<string>>, string, string[]>(dictionary, u003cu003e9_20, u003cu003e9_21);
			}
		}

		public Notification()
		{
		}

		public void Add(string key, string message)
		{
			if (!this._errorMessages.ContainsKey(key))
			{
				this._errorMessages.set_Item(key, new List<string>());
			}
			this._errorMessages.get_Item(key).Add(message);
		}
	}
}