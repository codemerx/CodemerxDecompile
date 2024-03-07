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
			CalendarName value;
			if (!this._calendarName.HasValue)
			{
				List<CalendarSelectorResult> calendarSelectorResults = new List<CalendarSelectorResult>();
				foreach (ICalendarSelector _calendarSelector in this._calendarSelectors)
				{
					CalendarSelectorResult calendarAsync = await _calendarSelector.GetCalendarAsync();
					if (calendarAsync == null)
					{
						continue;
					}
					calendarSelectorResults.Add(calendarAsync);
				}
				if (calendarSelectorResults.Count != 0)
				{
					if (calendarSelectorResults.Count > 1)
					{
						List<CalendarSelectorResult> calendarSelectorResults1 = calendarSelectorResults;
						calendarSelectorResults1.Sort((CalendarSelectorResult x, CalendarSelectorResult y) => y.get_Priority().CompareTo(x.get_Priority()));
					}
					this._calendarName = new CalendarName?(await calendarSelectorResults.First<CalendarSelectorResult>().get_CalendarName()());
					value = this._calendarName.Value;
				}
				else
				{
					value = 6;
				}
			}
			else
			{
				value = this._calendarName.Value;
			}
			return value;
		}
	}
}