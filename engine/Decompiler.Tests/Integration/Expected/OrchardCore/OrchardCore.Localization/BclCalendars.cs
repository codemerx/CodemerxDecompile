using NodaTime;
using System;
using System.Globalization;

namespace OrchardCore.Localization
{
	internal static class BclCalendars
	{
		public readonly static Calendar Hebrew;

		public readonly static Calendar Hijri;

		public readonly static Calendar Gregorian;

		public readonly static Calendar Julian;

		public readonly static Calendar Persian;

		public readonly static Calendar UmAlQura;

		static BclCalendars()
		{
			BclCalendars.Hebrew = new HebrewCalendar();
			BclCalendars.Hijri = new HijriCalendar();
			BclCalendars.Gregorian = new GregorianCalendar();
			BclCalendars.Julian = new JulianCalendar();
			BclCalendars.Persian = new PersianCalendar();
			BclCalendars.UmAlQura = new UmAlQuraCalendar();
		}

		public static CalendarSystem ConvertToCalendarSystem(Calendar calendar)
		{
			if (calendar == null)
			{
				throw new ArgumentNullException("calendar");
			}
			Type type = calendar.GetType();
			if (type == typeof(GregorianCalendar))
			{
				return CalendarSystem.get_Iso();
			}
			if (type == typeof(HebrewCalendar))
			{
				return CalendarSystem.get_HebrewCivil();
			}
			if (type == typeof(HijriCalendar))
			{
				return CalendarSystem.get_IslamicBcl();
			}
			if (type == typeof(JulianCalendar))
			{
				return CalendarSystem.get_Julian();
			}
			if (type == typeof(PersianCalendar))
			{
				if (!calendar.IsLeapYear(1))
				{
					return CalendarSystem.get_PersianAstronomical();
				}
				return CalendarSystem.get_PersianSimple();
			}
			if (type == typeof(UmAlQuraCalendar))
			{
				return CalendarSystem.get_UmAlQura();
			}
			return null;
		}

		public static CalendarSystem GetCalendarByName(CalendarName calendarName)
		{
			switch (calendarName)
			{
				case 0:
				{
					return CalendarSystem.get_HebrewCivil();
				}
				case 1:
				{
					return CalendarSystem.get_IslamicBcl();
				}
				case 2:
				{
					return CalendarSystem.get_Iso();
				}
				case 3:
				{
					return CalendarSystem.get_Julian();
				}
				case 4:
				{
					if (!CultureInfo.CurrentUICulture.Calendar.IsLeapYear(1))
					{
						return CalendarSystem.get_PersianAstronomical();
					}
					return CalendarSystem.get_PersianSimple();
				}
				case 5:
				{
					return CalendarSystem.get_UmAlQura();
				}
			}
			throw new NotSupportedException("The calendar is not supported.");
		}

		public static CalendarName GetCalendarName(Calendar calendar)
		{
			if (calendar == null)
			{
				throw new ArgumentNullException("calendar");
			}
			Type type = calendar.GetType();
			if (type == typeof(GregorianCalendar))
			{
				return 2;
			}
			if (type == typeof(HebrewCalendar))
			{
				return 0;
			}
			if (type == typeof(HijriCalendar))
			{
				return 1;
			}
			if (type == typeof(JulianCalendar))
			{
				return 3;
			}
			if (type == typeof(PersianCalendar))
			{
				return 4;
			}
			if (type == typeof(UmAlQuraCalendar))
			{
				return 5;
			}
			return 6;
		}
	}
}