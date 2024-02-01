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
			this._changeTokens = new ConcurrentDictionary<string, Signal.ChangeTokenInfo>();
		}

		public IChangeToken GetToken(string key)
		{
			return this._changeTokens.GetOrAdd(key, (string _) => {
				CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
				return new Signal.ChangeTokenInfo(new CancellationChangeToken(cancellationTokenSource.Token), cancellationTokenSource);
			}).ChangeToken;
		}

		public void SignalToken(string key)
		{
			Signal.ChangeTokenInfo changeTokenInfo;
			if (this._changeTokens.TryRemove(key, out changeTokenInfo))
			{
				changeTokenInfo.TokenSource.Cancel();
			}
		}

		private struct ChangeTokenInfo
		{
			public IChangeToken ChangeToken
			{
				[IsReadOnly]
				get;
			}

			public CancellationTokenSource TokenSource
			{
				[IsReadOnly]
				get;
			}

			public ChangeTokenInfo(IChangeToken changeToken, CancellationTokenSource tokenSource)
			{
				this.ChangeToken = changeToken;
				this.TokenSource = tokenSource;
			}
		}
	}
}