using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;

namespace OrchardCore.Environment.Cache
{
	public class Signal : ISignal
	{
		private readonly ConcurrentDictionary<string, Signal.ChangeTokenInfo> _changeTokens;

		public Signal()
		{
			base();
			this._changeTokens = new ConcurrentDictionary<string, Signal.ChangeTokenInfo>();
			return;
		}

		public IChangeToken GetToken(string key)
		{
			stackVariable1 = this._changeTokens;
			stackVariable2 = key;
			stackVariable3 = Signal.u003cu003ec.u003cu003e9__2_0;
			if (stackVariable3 == null)
			{
				dummyVar0 = stackVariable3;
				stackVariable3 = new Func<string, Signal.ChangeTokenInfo>(Signal.u003cu003ec.u003cu003e9.u003cGetTokenu003eb__2_0);
				Signal.u003cu003ec.u003cu003e9__2_0 = stackVariable3;
			}
			return stackVariable1.GetOrAdd(stackVariable2, stackVariable3).get_ChangeToken();
		}

		public void SignalToken(string key)
		{
			if (this._changeTokens.TryRemove(key, out V_0))
			{
				V_0.get_TokenSource().Cancel();
			}
			return;
		}

		private struct ChangeTokenInfo
		{
			public IChangeToken ChangeToken
			{
				[IsReadOnly]
				get
				{
					return this.u003cChangeTokenu003ek__BackingField;
				}
			}

			public CancellationTokenSource TokenSource
			{
				[IsReadOnly]
				get
				{
					return this.u003cTokenSourceu003ek__BackingField;
				}
			}

			public ChangeTokenInfo(IChangeToken changeToken, CancellationTokenSource tokenSource)
			{
				this.u003cChangeTokenu003ek__BackingField = changeToken;
				this.u003cTokenSourceu003ek__BackingField = tokenSource;
				return;
			}
		}
	}
}