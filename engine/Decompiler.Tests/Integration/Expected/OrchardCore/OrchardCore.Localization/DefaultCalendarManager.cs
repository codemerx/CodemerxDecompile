using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace OrchardCore.Localization
{
	public class DefaultCalendarManager : ICalendarManager
	{
		private readonly IEnumerable<ICalendarSelector> _calendarSelectors;

		private CalendarName? _calendarName;

		public DefaultCalendarManager(IEnumerable<ICalendarSelector> calendarSelectors)
		{
			base();
			this._calendarSelectors = calendarSelectors;
			return;
		}

		public async Task<CalendarName> GetCurrentCalendar()
		{
			V_0.u003cu003e4__this = this;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<CalendarName>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<DefaultCalendarManager.u003cGetCurrentCalendaru003ed__3>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}