using NodaTime;
using System;
using System.Runtime.CompilerServices;

namespace OrchardCore.Modules
{
	public class TimeZone : ITimeZone
	{
		public NodaTime.DateTimeZone DateTimeZone
		{
			get;
			set;
		}

		public Offset StandardOffset
		{
			get;
			set;
		}

		public string TimeZoneId
		{
			get;
			set;
		}

		public Offset WallOffset
		{
			get;
			set;
		}

		public TimeZone(string timeZoneId, Offset standardOffset, Offset wallOffset, NodaTime.DateTimeZone dateTimeZone)
		{
			base();
			this.set_TimeZoneId(timeZoneId);
			this.set_StandardOffset(standardOffset);
			this.set_WallOffset(wallOffset);
			this.set_DateTimeZone(dateTimeZone);
			return;
		}

		public override string ToString()
		{
			return string.Format("(GMT{0}) {1} ({2:+HH:mm})", this.get_StandardOffset(), this.get_TimeZoneId(), this.get_WallOffset());
		}
	}
}