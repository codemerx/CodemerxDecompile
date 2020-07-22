using System;
using System.Runtime.CompilerServices;

namespace OrchardCore.BackgroundTasks
{
	public class BackgroundTaskScheduler
	{
		public string Name
		{
			get
			{
				return this.u003cNameu003ek__BackingField;
			}
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
			get
			{
				return this.u003cTenantu003ek__BackingField;
			}
		}

		public bool Updated
		{
			get;
			set;
		}

		public BackgroundTaskScheduler(string tenant, string name, DateTime referenceTime)
		{
			base();
			this.u003cNameu003ek__BackingField = name;
			this.u003cTenantu003ek__BackingField = tenant;
			this.set_ReferenceTime(referenceTime);
			stackVariable8 = new BackgroundTaskSettings();
			stackVariable8.set_Name(name);
			this.set_Settings(stackVariable8);
			stackVariable11 = new BackgroundTaskState();
			stackVariable11.set_Name(name);
			this.set_State(stackVariable11);
			return;
		}

		public bool CanRun()
		{
			V_0 = CrontabSchedule.Parse(this.get_Settings().get_Schedule()).GetNextOccurrence(this.get_ReferenceTime());
			if (DateTime.op_GreaterThanOrEqual(DateTime.get_UtcNow(), V_0))
			{
				if (this.get_Settings().get_Enable() && !this.get_Released() && this.get_Updated())
				{
					return true;
				}
				this.set_ReferenceTime(DateTime.get_UtcNow());
			}
			return false;
		}

		public void Run()
		{
			stackVariable1 = this.get_State();
			stackVariable3 = DateTime.get_UtcNow();
			V_0 = stackVariable3;
			this.set_ReferenceTime(stackVariable3);
			stackVariable1.set_LastStartTime(V_0);
			return;
		}
	}
}