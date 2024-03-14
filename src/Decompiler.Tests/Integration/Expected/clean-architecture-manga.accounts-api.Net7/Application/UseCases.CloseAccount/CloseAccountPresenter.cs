using Domain;
using System;
using System.Runtime.CompilerServices;

namespace Application.UseCases.CloseAccount
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class CloseAccountPresenter : IOutputPort
	{
		public Domain.Account Account
		{
			get;
			private set;
		}

		public bool HasFundsOutput
		{
			get;
			private set;
		}

		public bool InvalidOutput
		{
			get;
			private set;
		}

		public bool NotFoundOutput
		{
			get;
			private set;
		}

		public CloseAccountPresenter()
		{
		}

		public void HasFunds()
		{
			this.HasFundsOutput = true;
		}

		public void Invalid()
		{
			this.InvalidOutput = true;
		}

		public void NotFound()
		{
			this.NotFoundOutput = true;
		}

		[NullableContext(1)]
		public void Ok(Domain.Account account)
		{
			this.Account = account;
		}
	}
}