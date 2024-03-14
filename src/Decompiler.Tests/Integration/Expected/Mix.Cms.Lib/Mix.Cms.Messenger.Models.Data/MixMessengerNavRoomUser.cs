using System;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Messenger.Models.Data
{
	public class MixMessengerNavRoomUser
	{
		public DateTime JoinedDate
		{
			get;
			set;
		}

		public MixMessengerHubRoom Room
		{
			get;
			set;
		}

		public Guid RoomId
		{
			get;
			set;
		}

		public MixMessengerUser User
		{
			get;
			set;
		}

		public string UserId
		{
			get;
			set;
		}

		public MixMessengerNavRoomUser()
		{
		}
	}
}