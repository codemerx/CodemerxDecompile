using System;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Messenger.Models.Data
{
	public class MixMessengerMessage
	{
		public string Content
		{
			get;
			set;
		}

		public DateTime CreatedDate
		{
			get;
			set;
		}

		public Guid Id
		{
			get;
			set;
		}

		public MixMessengerHubRoom Room
		{
			get;
			set;
		}

		public Guid? RoomId
		{
			get;
			set;
		}

		public MixMessengerTeam Team
		{
			get;
			set;
		}

		public int? TeamId
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

		public MixMessengerMessage()
		{
		}
	}
}