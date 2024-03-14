using System;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Messenger.Models.Data
{
	public class MixMessengerUserDevice
	{
		public string ConnectionId
		{
			get;
			set;
		}

		public string DeviceId
		{
			get;
			set;
		}

		public DateTime? EndDate
		{
			get;
			set;
		}

		public DateTime StartDate
		{
			get;
			set;
		}

		public int Status
		{
			get;
			set;
		}

		public string UserId
		{
			get;
			set;
		}

		public MixMessengerUserDevice()
		{
		}
	}
}