using Squidex.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.IdentityServer.Controllers.Profile
{
	[Nullable(0)]
	[NullableContext(1)]
	public class ChangePropertiesModel
	{
		public List<UserProperty> Properties
		{
			get;
			set;
		}

		public ChangePropertiesModel()
		{
		}

		public UserValues ToValues()
		{
			object list;
			List<UserProperty> properties = this.Properties;
			if (properties != null)
			{
				list = (
					from x in properties
					select x.ToTuple()).ToList<ValueTuple<string, string>>();
			}
			else
			{
				list = null;
			}
			if (list == null)
			{
				list = new List<ValueTuple<string, string>>();
			}
			List<ValueTuple<string, string>> valueTuples = (List<ValueTuple<string, string>>)list;
			UserValues userValue = new UserValues();
			userValue.set_Properties(valueTuples);
			return userValue;
		}
	}
}