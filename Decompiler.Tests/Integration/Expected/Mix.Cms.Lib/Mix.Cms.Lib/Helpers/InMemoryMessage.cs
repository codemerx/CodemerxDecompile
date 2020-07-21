using Microsoft.OData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Helpers
{
	public class InMemoryMessage : IODataRequestMessage, IODataResponseMessage, IContainerProvider, IDisposable
	{
		private readonly Dictionary<string, string> headers;

		public IServiceProvider Container
		{
			get
			{
				return get_Container();
			}
			set
			{
				set_Container(value);
			}
		}

		// <Container>k__BackingField
		private IServiceProvider u003cContaineru003ek__BackingField;

		public IServiceProvider get_Container()
		{
			return this.u003cContaineru003ek__BackingField;
		}

		public void set_Container(IServiceProvider value)
		{
			this.u003cContaineru003ek__BackingField = value;
			return;
		}

		public Action DisposeAction
		{
			get;
			set;
		}

		public IEnumerable<KeyValuePair<string, string>> Headers
		{
			get
			{
				return this.headers;
			}
		}

		public string Method
		{
			get;
			set;
		}

		public int StatusCode
		{
			get;
			set;
		}

		public System.IO.Stream Stream
		{
			get;
			set;
		}

		public Uri Url
		{
			get;
			set;
		}

		public InMemoryMessage()
		{
			base();
			this.headers = new Dictionary<string, string>();
			return;
		}

		public string GetHeader(string headerName)
		{
			if (!this.headers.TryGetValue(headerName, ref V_0))
			{
				return null;
			}
			return V_0;
		}

		public System.IO.Stream GetStream()
		{
			return this.get_Stream();
		}

		System.IO.Stream Microsoft.OData.IODataResponseMessage.GetStream()
		{
			return this.get_Stream();
		}

		public void SetHeader(string headerName, string headerValue)
		{
			this.headers.set_Item(headerName, headerValue);
			return;
		}

		void System.IDisposable.Dispose()
		{
			if (this.get_DisposeAction() != null)
			{
				this.get_DisposeAction().Invoke();
			}
			return;
		}
	}
}