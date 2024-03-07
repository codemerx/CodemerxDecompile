using NodaTime;
using OrchardCore.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace OrchardCore.Modules
{
	public class LocalClock : ILocalClock
	{
		private readonly IEnumerable<ITimeZoneSelector> _timeZoneSelectors;

		private readonly IClock _clock;

		private readonly ICalendarManager _calendarManager;

		private ITimeZone _timeZone;

		public Task<DateTimeOffset> LocalNowAsync
		{
			get
			{
				return this.GetLocalNowAsync();
			}
		}

		public LocalClock(IEnumerable<ITimeZoneSelector> timeZoneSelectors, IClock clock, ICalendarManager calendarManager)
		{
			this._timeZoneSelectors = timeZoneSelectors;
			this._clock = clock;
			this._calendarManager = calendarManager;
		}

		public async Task<DateTimeOffset> ConvertToLocalAsync(DateTimeOffset dateTimeOffSet)
		{
			DateTimeZone dateTimeZone = ((OrchardCore.Modules.TimeZone)await this.GetLocalTimeZoneAsync()).DateTimeZone;
			OffsetDateTime offsetDateTime = OffsetDateTime.FromDateTimeOffset(dateTimeOffSet);
			CalendarSystem calendarByName = BclCalendars.GetCalendarByName(await this._calendarManager.GetCurrentCalendar());
			ZonedDateTime zonedDateTime = offsetDateTime.InZone(dateTimeZone);
			zonedDateTime = zonedDateTime.WithCalendar(calendarByName);
			return zonedDateTime.ToDateTimeOffset();
		}

		public async Task<DateTime> ConvertToUtcAsync(DateTime dateTime)
		{
			DateTimeZone dateTimeZone = ((OrchardCore.Modules.TimeZone)await this.GetLocalTimeZoneAsync()).DateTimeZone;
			LocalDateTime localDateTime = LocalDateTime.FromDateTime(dateTime);
			return dateTimeZone.AtStrictly(localDateTime).ToDateTimeUtc();
		}

		private async Task<DateTimeOffset> GetLocalNowAsync()
		{
			IClock clock = this._clock;
			return clock.ConvertToTimeZone(this._clock.get_UtcNow(), await this.GetLocalTimeZoneAsync());
		}

		public async Task<ITimeZone> GetLocalTimeZoneAsync()
		{
			if (this._timeZone == null)
			{
				this._timeZone = await this.LoadLocalTimeZoneAsync();
			}
			return this._timeZone;
		}

		private async Task<ITimeZone> LoadLocalTimeZoneAsync()
		{
			ITimeZone timeZone;
			List<TimeZoneSelectorResult> timeZoneSelectorResults = new List<TimeZoneSelectorResult>();
			foreach (ITimeZoneSelector _timeZoneSelector in this._timeZoneSelectors)
			{
				TimeZoneSelectorResult timeZoneAsync = await _timeZoneSelector.GetTimeZoneAsync();
				if (timeZoneAsync == null)
				{
					continue;
				}
				timeZoneSelectorResults.Add(timeZoneAsync);
			}
			if (timeZoneSelectorResults.Count != 0)
			{
				if (timeZoneSelectorResults.Count > 1)
				{
					List<TimeZoneSelectorResult> timeZoneSelectorResults1 = timeZoneSelectorResults;
					timeZoneSelectorResults1.Sort((TimeZoneSelectorResult x, TimeZoneSelectorResult y) => y.get_Priority().CompareTo(x.get_Priority()));
				}
				foreach (TimeZoneSelectorResult timeZoneSelectorResult in timeZoneSelectorResults)
				{
					string timeZoneId = await timeZoneSelectorResult.get_TimeZoneId()();
					if (string.IsNullOrEmpty(timeZoneId))
					{
						continue;
					}
					timeZone = this._clock.GetTimeZone(timeZoneId);
					return timeZone;
				}
				timeZone = this._clock.GetSystemTimeZone();
			}
			else
			{
				timeZone = this._clock.GetSystemTimeZone();
			}
			return timeZone;
		}
	}
}