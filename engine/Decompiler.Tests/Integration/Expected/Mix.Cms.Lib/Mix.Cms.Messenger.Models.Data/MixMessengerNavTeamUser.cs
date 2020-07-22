using System;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Messenger.Models.Data
{
	public class MixMessengerNavTeamUser
	{
		public DateTime JoinedDate
		{
			get;
			set;
		}

		public DateTime? LastModified
		{
			get;
			set;
		}

		public string Status
		{
			get;
			set;
		}

		public MixMessengerTeam Team
		{
			get;
			set;
		}

		public int TeamId
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

		public MixMessengerNavTeamUser()
		{
			base();
			return;
		}
	}
}