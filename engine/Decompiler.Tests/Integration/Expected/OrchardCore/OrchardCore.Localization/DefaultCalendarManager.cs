using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
			this._calendarSelectors = calendarSelectors;
		}

		public async Task<CalendarName> GetCurrentCalendar()
		{
			DefaultCalendarManager.u003cGetCurrentCalendaru003ed__3 variable = new DefaultCalendarManager.u003cGetCurrentCalendaru003ed__3();
			variable.u003cu003e4__this = this;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<CalendarName>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<DefaultCalendarManager.u003cGetCurrentCalendaru003ed__3>(ref variable);
			return variable.u003cu003et__builder.Task;
		}
	}
}