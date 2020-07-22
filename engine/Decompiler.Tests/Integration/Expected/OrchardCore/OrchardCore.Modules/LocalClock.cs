using NodaTime;
using OrchardCore.Localization;
using System;
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
			base();
			this._timeZoneSelectors = timeZoneSelectors;
			this._clock = clock;
			this._calendarManager = calendarManager;
			return;
		}

		public async Task<DateTimeOffset> ConvertToLocalAsync(DateTimeOffset dateTimeOffSet)
		{
			V_0.u003cu003e4__this = this;
			V_0.dateTimeOffSet = dateTimeOffSet;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<DateTimeOffset>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<LocalClock.u003cConvertToLocalAsyncu003ed__9>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<DateTime> ConvertToUtcAsync(DateTime dateTime)
		{
			V_0.u003cu003e4__this = this;
			V_0.dateTime = dateTime;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<DateTime>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<LocalClock.u003cConvertToUtcAsyncu003ed__10>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private async Task<DateTimeOffset> GetLocalNowAsync()
		{
			V_0.u003cu003e4__this = this;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<DateTimeOffset>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<LocalClock.u003cGetLocalNowAsyncu003ed__7>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<ITimeZone> GetLocalTimeZoneAsync()
		{
			V_0.u003cu003e4__this = this;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<ITimeZone>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<LocalClock.u003cGetLocalTimeZoneAsyncu003ed__8>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private async Task<ITimeZone> LoadLocalTimeZoneAsync()
		{
			V_0.u003cu003e4__this = this;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<ITimeZone>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<LocalClock.u003cLoadLocalTimeZoneAsyncu003ed__11>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}