using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib.Models.Account;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels.Account
{
	public class RefreshTokenViewModel : ViewModelBase<MixCmsAccountContext, RefreshTokens, RefreshTokenViewModel>
	{
		[JsonProperty("clientId")]
		public string ClientId
		{
			get;
			set;
		}

		[JsonProperty("email")]
		public string Email
		{
			get;
			set;
		}

		[JsonProperty("expiresUtc")]
		public DateTime ExpiresUtc
		{
			get;
			set;
		}

		[JsonProperty("id")]
		public string Id
		{
			get;
			set;
		}

		[JsonProperty("issuedUtc")]
		public DateTime IssuedUtc
		{
			get;
			set;
		}

		public RefreshTokenViewModel()
		{
			base();
			return;
		}

		public RefreshTokenViewModel(RefreshTokens model, MixCmsAccountContext _context = null, IDbContextTransaction _transaction = null)
		{
			base(model, _context, _transaction);
			return;
		}
	}
}