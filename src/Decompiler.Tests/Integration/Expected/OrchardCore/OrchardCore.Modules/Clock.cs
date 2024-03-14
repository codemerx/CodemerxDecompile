using NodaTime;
using NodaTime.TimeZones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace OrchardCore.Modules
{
	public class Clock : IClock
	{
		private static Instant CurrentInstant
		{
			get
			{
				return SystemClock.get_Instance().GetCurrentInstant();
			}
		}

		public DateTime UtcNow
		{
			get
			{
				return Clock.CurrentInstant.ToDateTimeUtc();
			}
		}

		public Clock()
		{
		}

		public DateTimeOffset ConvertToTimeZone(DateTimeOffset dateTimeOffSet, ITimeZone timeZone)
		{
			OffsetDateTime offsetDateTime = OffsetDateTime.FromDateTimeOffset(dateTimeOffSet);
			return offsetDateTime.InZone(((OrchardCore.Modules.TimeZone)timeZone).DateTimeZone).ToDateTimeOffset();
		}

		private ITimeZone CreateTimeZone(DateTimeZone dateTimeZone)
		{
			if (dateTimeZone == null)
			{
				throw new ArgumentException("DateTimeZone");
			}
			ZoneInterval zoneInterval = dateTimeZone.GetZoneInterval(Clock.CurrentInstant);
			return new OrchardCore.Modules.TimeZone(dateTimeZone.get_Id(), zoneInterval.get_StandardOffset(), zoneInterval.get_WallOffset(), dateTimeZone);
		}

		internal static DateTimeZone GetDateTimeZone(string timeZone)
		{
			if (string.IsNullOrEmpty(timeZone) || !Clock.IsValidTimeZone(DateTimeZoneProviders.get_Tzdb(), timeZone))
			{
				return DateTimeZoneProviders.get_Tzdb().GetSystemDefault();
			}
			return DateTimeZoneProviders.get_Tzdb().get_Item(timeZone);
		}

		public ITimeZone GetSystemTimeZone()
		{
			DateTimeZone systemDefault = DateTimeZoneProviders.get_Tzdb().GetSystemDefault();
			if (TzdbDateTimeZoneSource.get_Default().get_CanonicalIdMap().ContainsKey(systemDefault.get_Id()))
			{
				systemDefault = Clock.GetDateTimeZone(TzdbDateTimeZoneSource.get_Default().get_CanonicalIdMap()[systemDefault.get_Id()]);
			}
			return this.CreateTimeZone(systemDefault);
		}

		public ITimeZone GetTimeZone(string timeZoneId)
		{
			if (string.IsNullOrEmpty(timeZoneId))
			{
				return this.GetSystemTimeZone();
			}
			return this.CreateTimeZone(Clock.GetDateTimeZone(timeZoneId));
		}

		public ITimeZone[] GetTimeZones()
		{
			return (
				from location in TzdbDateTimeZoneSource.get_Default().get_ZoneLocations()
				let zoneId = location.get_ZoneId()
				let tz = DateTimeZoneProviders.get_Tzdb().get_Item(zoneId)
				let zoneInterval = tz.GetZoneInterval(Clock.CurrentInstant)
				orderby zoneInterval.get_StandardOffset(), zoneId
				select new OrchardCore.Modules.TimeZone(zoneId, zoneInterval.get_StandardOffset(), zoneInterval.get_WallOffset(), tz)).ToArray<OrchardCore.Modules.TimeZone>();
		}

		private static bool IsValidTimeZone(IDateTimeZoneProvider provider, string timeZoneId)
		{
			return provider.GetZoneOrNull(timeZoneId) != null;
		}
	}
}