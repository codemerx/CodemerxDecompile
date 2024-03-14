using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;

namespace OrchardCore.Environment.Shell
{
	public class RunningShellTable : IRunningShellTable
	{
		private readonly static char[] HostSeparators;

		private ImmutableDictionary<string, ShellSettings> _shellsByHostAndPrefix = ImmutableDictionary<string, ShellSettings>.Empty.WithComparers(StringComparer.OrdinalIgnoreCase);

		private ShellSettings _default;

		private bool _hasStarMapping;

		static RunningShellTable()
		{
			RunningShellTable.HostSeparators = new char[] { ',', ' ' };
		}

		public RunningShellTable()
		{
		}

		public void Add(ShellSettings settings)
		{
			if ("Default" == settings.get_Name())
			{
				this._default = settings;
			}
			string[] allHostsAndPrefix = this.GetAllHostsAndPrefix(settings);
			Dictionary<string, ShellSettings> strs = new Dictionary<string, ShellSettings>();
			string[] strArrays = allHostsAndPrefix;
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				string str = strArrays[i];
				this._hasStarMapping = (this._hasStarMapping ? true : str.StartsWith('*'));
				strs.TryAdd(str, settings);
			}
			lock (this)
			{
				this._shellsByHostAndPrefix = this._shellsByHostAndPrefix.SetItems(strs);
			}
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
			if (string.IsNullOrWhiteSpace(shellSettings.get_RequestUrlHost()))
			{
				return new string[] { string.Concat("/", shellSettings.get_RequestUrlPrefix()) };
			}
			return (
				from ruh in shellSettings.get_RequestUrlHost().Split(RunningShellTable.HostSeparators, StringSplitOptions.RemoveEmptyEntries)
				select string.Concat(ruh, "/", shellSettings.get_RequestUrlPrefix())).ToArray<string>();
		}

		private string GetHostAndPrefix(StringSegment host, StringSegment path)
		{
			int num = (path.get_Length() > 0 ? path.IndexOf('/', 1) : -1);
			if (num <= -1)
			{
				return string.Concat(host.ToString(), path.get_Value());
			}
			string str = host.ToString();
			StringSegment stringSegment = path.Subsegment(0, num);
			return string.Concat(str, stringSegment.get_Value());
		}

		public ShellSettings Match(HostString host, PathString path, bool fallbackToDefault = true)
		{
			ShellSettings shellSetting;
			string str = host.get_Host();
			if (this.TryMatchInternal(host.get_Value(), str, path.get_Value(), out shellSetting))
			{
				return shellSetting;
			}
			if (this._hasStarMapping && this.TryMatchStarMapping(host.get_Value(), str, path.get_Value(), out shellSetting))
			{
				return shellSetting;
			}
			if (fallbackToDefault && this.DefaultIsCatchAll())
			{
				return this._default;
			}
			if (fallbackToDefault && this.TryMatchInternal("", "", "/", out shellSetting))
			{
				return shellSetting;
			}
			return null;
		}

		public void Remove(ShellSettings settings)
		{
			string[] array = (
				from kv in this._shellsByHostAndPrefix
				where kv.Value.get_Name() == settings.get_Name()
				select kv.Key).ToArray<string>();
			lock (this)
			{
				this._shellsByHostAndPrefix = this._shellsByHostAndPrefix.RemoveRange(array);
			}
			if ((object)this._default == (object)settings)
			{
				this._default = null;
			}
		}

		private bool TryMatchInternal(StringSegment host, StringSegment hostOnly, StringSegment path, out ShellSettings result)
		{
			if (host.get_Length() != 0 && this._shellsByHostAndPrefix.TryGetValue(this.GetHostAndPrefix(host, path), out result) || host.get_Length() != hostOnly.get_Length() && this._shellsByHostAndPrefix.TryGetValue(this.GetHostAndPrefix(hostOnly, path), out result) || host.get_Length() != 0 && this._shellsByHostAndPrefix.TryGetValue(this.GetHostAndPrefix(host, "/"), out result) || host.get_Length() != hostOnly.get_Length() && this._shellsByHostAndPrefix.TryGetValue(this.GetHostAndPrefix(hostOnly, "/"), out result) || this._shellsByHostAndPrefix.TryGetValue(this.GetHostAndPrefix("", path), out result))
			{
				return true;
			}
			result = null;
			return false;
		}

		private bool TryMatchStarMapping(StringSegment host, StringSegment hostOnly, StringSegment path, out ShellSettings result)
		{
			if (this.TryMatchInternal(string.Concat("*.", host.ToString()), string.Concat("*.", hostOnly.ToString()), path, out result))
			{
				return true;
			}
			int num = -1;
			do
			{
				int num1 = host.IndexOf('.', num + 1);
				num = num1;
				if (-1 != num1)
				{
					continue;
				}
				result = null;
				return false;
			}
			while (!this.TryMatchInternal(string.Concat("*", host.Subsegment(num).ToString()), string.Concat("*", hostOnly.Subsegment(num).ToString()), path, out result));
			return true;
		}
	}
}