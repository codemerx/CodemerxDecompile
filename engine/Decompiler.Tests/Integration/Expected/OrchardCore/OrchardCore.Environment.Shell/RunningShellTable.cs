using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace OrchardCore.Environment.Shell
{
	public class RunningShellTable : IRunningShellTable
	{
		private readonly static char[] HostSeparators;

		private ImmutableDictionary<string, ShellSettings> _shellsByHostAndPrefix;

		private ShellSettings _default;

		private bool _hasStarMapping;

		static RunningShellTable()
		{
			stackVariable1 = new char[2];
			stackVariable1[0] = ',';
			stackVariable1[1] = ' ';
			RunningShellTable.HostSeparators = stackVariable1;
			return;
		}

		public RunningShellTable()
		{
			this._shellsByHostAndPrefix = ImmutableDictionary<string, ShellSettings>.Empty.WithComparers(StringComparer.get_OrdinalIgnoreCase());
			base();
			return;
		}

		public void Add(ShellSettings settings)
		{
			if (string.op_Equality("Default", settings.get_Name()))
			{
				this._default = settings;
			}
			stackVariable6 = this.GetAllHostsAndPrefix(settings);
			V_0 = new Dictionary<string, ShellSettings>();
			V_1 = stackVariable6;
			V_2 = 0;
			while (V_2 < (int)V_1.Length)
			{
				V_3 = V_1[V_2];
				if (this._hasStarMapping)
				{
					stackVariable19 = true;
				}
				else
				{
					stackVariable19 = V_3.StartsWith('*');
				}
				this._hasStarMapping = stackVariable19;
				dummyVar0 = V_0.TryAdd(V_3, settings);
				V_2 = V_2 + 1;
			}
			V_4 = this;
			V_5 = false;
			try
			{
				Monitor.Enter(V_4, ref V_5);
				this._shellsByHostAndPrefix = this._shellsByHostAndPrefix.SetItems(V_0);
			}
			finally
			{
				if (V_5)
				{
					Monitor.Exit(V_4);
				}
			}
			return;
		}

		private bool DefaultIsCatchAll()
		{
			if (this._default == null || !string.IsNullOrEmpty(this._default.get_RequestUrlHost()))
			{
				return false;
			}
			return string.IsNullOrEmpty(this._default.get_RequestUrlPrefix());
		}

		private string[] GetAllHostsAndPrefix(ShellSettings shellSettings)
		{
			V_0 = new RunningShellTable.u003cu003ec__DisplayClass10_0();
			V_0.shellSettings = shellSettings;
			if (string.IsNullOrWhiteSpace(V_0.shellSettings.get_RequestUrlHost()))
			{
				stackVariable19 = new string[1];
				stackVariable19[0] = string.Concat("/", V_0.shellSettings.get_RequestUrlPrefix());
				return stackVariable19;
			}
			return V_0.shellSettings.get_RequestUrlHost().Split(RunningShellTable.HostSeparators, 1).Select<string, string>(new Func<string, string>(V_0.u003cGetAllHostsAndPrefixu003eb__0)).ToArray<string>();
		}

		private string GetHostAndPrefix(StringSegment host, StringSegment path)
		{
			if (path.get_Length() > 0)
			{
				stackVariable6 = path.IndexOf('/', 1);
			}
			else
			{
				stackVariable6 = -1;
			}
			V_0 = stackVariable6;
			if (V_0 <= -1)
			{
				return string.Concat(host.ToString(), path.get_Value());
			}
			stackVariable15 = host.ToString();
			V_1 = path.Subsegment(0, V_0);
			return string.Concat(stackVariable15, V_1.get_Value());
		}

		public ShellSettings Match(HostString host, PathString path, bool fallbackToDefault = true)
		{
			V_0 = host.get_Host();
			if (this.TryMatchInternal(StringSegment.op_Implicit(host.get_Value()), StringSegment.op_Implicit(V_0), StringSegment.op_Implicit(path.get_Value()), out V_1))
			{
				return V_1;
			}
			if (this._hasStarMapping && this.TryMatchStarMapping(StringSegment.op_Implicit(host.get_Value()), StringSegment.op_Implicit(V_0), StringSegment.op_Implicit(path.get_Value()), out V_1))
			{
				return V_1;
			}
			if (fallbackToDefault && this.DefaultIsCatchAll())
			{
				return this._default;
			}
			if (fallbackToDefault && this.TryMatchInternal(StringSegment.op_Implicit(""), StringSegment.op_Implicit(""), StringSegment.op_Implicit("/"), out V_1))
			{
				return V_1;
			}
			return null;
		}

		public void Remove(ShellSettings settings)
		{
			V_0 = new RunningShellTable.u003cu003ec__DisplayClass5_0();
			V_0.settings = settings;
			stackVariable8 = this._shellsByHostAndPrefix.Where<KeyValuePair<string, ShellSettings>>(new Func<KeyValuePair<string, ShellSettings>, bool>(V_0.u003cRemoveu003eb__0));
			stackVariable9 = RunningShellTable.u003cu003ec.u003cu003e9__5_1;
			if (stackVariable9 == null)
			{
				dummyVar0 = stackVariable9;
				stackVariable9 = new Func<KeyValuePair<string, ShellSettings>, string>(RunningShellTable.u003cu003ec.u003cu003e9.u003cRemoveu003eb__5_1);
				RunningShellTable.u003cu003ec.u003cu003e9__5_1 = stackVariable9;
			}
			V_1 = stackVariable8.Select<KeyValuePair<string, ShellSettings>, string>(stackVariable9).ToArray<string>();
			V_2 = this;
			V_3 = false;
			try
			{
				Monitor.Enter(V_2, ref V_3);
				this._shellsByHostAndPrefix = this._shellsByHostAndPrefix.RemoveRange(V_1);
			}
			finally
			{
				if (V_3)
				{
					Monitor.Exit(V_2);
				}
			}
			if ((object)this._default == (object)V_0.settings)
			{
				this._default = null;
			}
			return;
		}

		private bool TryMatchInternal(StringSegment host, StringSegment hostOnly, StringSegment path, out ShellSettings result)
		{
			if (host.get_Length() != 0 && this._shellsByHostAndPrefix.TryGetValue(this.GetHostAndPrefix(host, path), out result) || host.get_Length() != hostOnly.get_Length() && this._shellsByHostAndPrefix.TryGetValue(this.GetHostAndPrefix(hostOnly, path), out result) || host.get_Length() != 0 && this._shellsByHostAndPrefix.TryGetValue(this.GetHostAndPrefix(host, StringSegment.op_Implicit("/")), out result) || host.get_Length() != hostOnly.get_Length() && this._shellsByHostAndPrefix.TryGetValue(this.GetHostAndPrefix(hostOnly, StringSegment.op_Implicit("/")), out result) || this._shellsByHostAndPrefix.TryGetValue(this.GetHostAndPrefix(StringSegment.op_Implicit(""), path), out result))
			{
				return true;
			}
			result = null;
			return false;
		}

		private bool TryMatchStarMapping(StringSegment host, StringSegment hostOnly, StringSegment path, out ShellSettings result)
		{
			if (this.TryMatchInternal(StringSegment.op_Implicit(string.Concat("*.", host.ToString())), StringSegment.op_Implicit(string.Concat("*.", hostOnly.ToString())), path, out result))
			{
				return true;
			}
			V_0 = -1;
			do
			{
				stackVariable21 = host.IndexOf('.', V_0 + 1);
				V_0 = stackVariable21;
				if (-1 != stackVariable21)
				{
					continue;
				}
				result = null;
				return false;
			}
			while (!this.TryMatchInternal(StringSegment.op_Implicit(string.Concat("*", host.Subsegment(V_0).ToString())), StringSegment.op_Implicit(string.Concat("*", hostOnly.Subsegment(V_0).ToString())), path, out result));
			return true;
		}
	}
}