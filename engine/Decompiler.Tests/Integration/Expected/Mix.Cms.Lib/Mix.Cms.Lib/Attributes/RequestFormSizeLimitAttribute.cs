using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple=false, Inherited=true)]
	public class RequestFormSizeLimitAttribute : Attribute, IAuthorizationFilter, IFilterMetadata, IOrderedFilter
	{
		private readonly FormOptions _formOptions;

		public int Order
		{
			get
			{
				return get_Order();
			}
			set
			{
				set_Order(value);
			}
		}

		// <Order>k__BackingField
		private int u003cOrderu003ek__BackingField;

		public int get_Order()
		{
			return this.u003cOrderu003ek__BackingField;
		}

		public void set_Order(int value)
		{
			this.u003cOrderu003ek__BackingField = value;
			return;
		}

		public RequestFormSizeLimitAttribute(int valueCountLimit)
		{
			base();
			stackVariable2 = new FormOptions();
			stackVariable2.set_ValueCountLimit(valueCountLimit);
			stackVariable2.set_KeyLengthLimit(valueCountLimit);
			this._formOptions = stackVariable2;
			return;
		}

		public void OnAuthorization(AuthorizationFilterContext context)
		{
			V_0 = context.get_HttpContext().get_Features();
			V_1 = V_0.Get<IFormFeature>();
			if (V_1 == null || V_1.get_Form() == null)
			{
				V_0.Set<IFormFeature>(new FormFeature(context.get_HttpContext().get_Request(), this._formOptions));
			}
			return;
		}
	}
}