using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Messenger.Models.Data
{
	public class MixMessengerUser
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

		public string FacebookId
		{
			get;
			set;
		}

		public string Id
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

		public ICollection<Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser> MixMessengerNavTeamUser
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public string Status
		{
			get;
			set;
		}

		public MixMessengerUser()
		{
			this.MixMessengerMessage = new HashSet<Mix.Cms.Messenger.Models.Data.MixMessengerMessage>();
			this.MixMessengerNavRoomUser = new HashSet<Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser>();
			this.MixMessengerNavTeamUser = new HashSet<Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser>();
		}
	}
}