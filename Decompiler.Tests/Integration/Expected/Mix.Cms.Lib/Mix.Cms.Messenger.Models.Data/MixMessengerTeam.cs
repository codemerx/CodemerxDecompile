using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Messenger.Models.Data
{
	public class MixMessengerTeam
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

		public string HostId
		{
			get;
			set;
		}

		public int Id
		{
			get;
			set;
		}

		public bool? IsOpen
		{
			get;
			set;
		}

		public ICollection<Mix.Cms.Messenger.Models.Data.MixMessengerMessage> MixMessengerMessage
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

		public int Type
		{
			get;
			set;
		}

		public MixMessengerTeam()
		{
			base();
			this.set_MixMessengerMessage(new HashSet<Mix.Cms.Messenger.Models.Data.MixMessengerMessage>());
			this.set_MixMessengerNavTeamUser(new HashSet<Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser>());
			return;
		}
	}
}