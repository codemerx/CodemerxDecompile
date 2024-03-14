using System;
using System.Runtime.CompilerServices;

namespace Northwind.Application.Notifications.Models
{
	public class MessageDto
	{
		public string Body
		{
			get;
			set;
		}

		public string From
		{
			get;
			set;
		}

		public string Subject
		{
			get;
			set;
		}

		public string To
		{
			get;
			set;
		}

		public MessageDto()
		{
		}
	}
}