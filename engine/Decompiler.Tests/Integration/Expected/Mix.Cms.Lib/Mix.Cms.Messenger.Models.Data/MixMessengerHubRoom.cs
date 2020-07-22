using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Messenger.Models.Data
{
	public class MixMessengerHubRoom
	{
		public string Avatar
		{
			get;
			set;
		}

		public DateTime CreatedDate
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public string HostId
		{
			get;
			set;
		}

		public Guid Id
		{
			get;
			set;
		}

		public bool IsOpen
		{
			get;
			set;
		}

		public DateTime? LastModified
		{
			get;
			set;
		}

		public ICollection<Mix.Cms.Messenger.Models.Data.MixMessengerMessage> MixMessengerMessage
		{
			get;
			set;
		}

		public ICollection<Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser> MixMessengerNavRoomUser
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public int? TeamId
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public MixMessengerHubRoom()
		{
			base();
			this.set_MixMessengerMessage(new HashSet<Mix.Cms.Messenger.Models.Data.MixMessengerMessage>());
			this.set_MixMessengerNavRoomUser(new HashSet<Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser>());
			return;
		}
	}
}