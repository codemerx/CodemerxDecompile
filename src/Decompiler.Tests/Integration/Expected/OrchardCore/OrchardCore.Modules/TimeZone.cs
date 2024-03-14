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
			this.TimeZoneId = timeZoneId;
			this.StandardOffset = standardOffset;
			this.WallOffset = wallOffset;
			this.DateTimeZone = dateTimeZone;
		}

		public override string ToString()
		{
			return string.Format("(GMT{0}) {1} ({2:+HH:mm})", this.StandardOffset, this.TimeZoneId, this.WallOffset);
		}
	}
}