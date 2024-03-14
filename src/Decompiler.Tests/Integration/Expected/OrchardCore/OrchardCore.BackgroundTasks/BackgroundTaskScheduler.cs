using NCrontab;
using System;
using System.Runtime.CompilerServices;

namespace OrchardCore.BackgroundTasks
{
	public class BackgroundTaskScheduler
	{
		public string Name
		{
			get;
		}

		public DateTime ReferenceTime
		{
			get;
			set;
		}

		public bool Released
		{
			get;
			set;
		}

		public BackgroundTaskSettings Settings
		{
			get;
			set;
		}

		public BackgroundTaskState State
		{
			get;
			set;
		}

		public string Tenant
		{
			get;
		}

		public bool Updated
		{
			get;
			set;
		}

		public BackgroundTaskScheduler(string tenant, string name, DateTime referenceTime)
		{
			this.Name = name;
			this.Tenant = tenant;
			this.ReferenceTime = referenceTime;
			BackgroundTaskSettings backgroundTaskSetting = new BackgroundTaskSettings();
			backgroundTaskSetting.set_Name(name);
			this.Settings = backgroundTaskSetting;
			BackgroundTaskState backgroundTaskState = new BackgroundTaskState();
			backgroundTaskState.set_Name(name);
			this.State = backgroundTaskState;
		}

		public bool CanRun()
		{
			DateTime nextOccurrence = CrontabSchedule.Parse(this.Settings.get_Schedule()).GetNextOccurrence(this.ReferenceTime);
			if (DateTime.UtcNow >= nextOccurrence)
			{
				if (this.Settings.get_Enable() && !this.Released && this.Updated)
				{
					return true;
				}
				this.ReferenceTime = DateTime.UtcNow;
			}
			return false;
		}

		public void Run()
		{
			BackgroundTaskState state = this.State;
			DateTime utcNow = DateTime.UtcNow;
			DateTime dateTime = utcNow;
			this.ReferenceTime = utcNow;
			state.set_LastStartTime(dateTime);
		}
	}
}