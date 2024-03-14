using MediatR;
using Northwind.Application.Common.Interfaces;
using Northwind.Application.Notifications.Models;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Northwind.Application.Customers.Commands.CreateCustomer
{
	public class CustomerCreated : INotification
	{
		public string CustomerId
		{
			get;
			set;
		}

		public CustomerCreated()
		{
		}

		public class CustomerCreatedHandler : INotificationHandler<CustomerCreated>
		{
			private readonly INotificationService _notification;

			public CustomerCreatedHandler(INotificationService notification)
			{
				this._notification = notification;
			}

			public async Task Handle(CustomerCreated notification, CancellationToken cancellationToken)
			{
				await this._notification.SendAsync(new MessageDto());
			}
		}
	}
}