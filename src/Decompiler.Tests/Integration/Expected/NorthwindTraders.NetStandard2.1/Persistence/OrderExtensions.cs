using Northwind.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Northwind.Persistence
{
	internal static class OrderExtensions
	{
		public static Order AddOrderDetails(this Order order, params OrderDetail[] orderDetails)
		{
			OrderDetail[] orderDetailArray = orderDetails;
			for (int i = 0; i < (int)orderDetailArray.Length; i++)
			{
				OrderDetail orderDetail = orderDetailArray[i];
				order.get_OrderDetails().Add(orderDetail);
			}
			return order;
		}
	}
}