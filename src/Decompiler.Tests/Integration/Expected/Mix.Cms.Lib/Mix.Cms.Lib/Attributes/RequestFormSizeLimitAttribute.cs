using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
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
				return JustDecompileGenerated_get_Order();
			}
			set
			{
				JustDecompileGenerated_set_Order(value);
			}
		}

		private int JustDecompileGenerated_Order_k__BackingField;

		public int JustDecompileGenerated_get_Order()
		{
			return this.JustDecompileGenerated_Order_k__BackingField;
		}

		public void JustDecompileGenerated_set_Order(int value)
		{
			this.JustDecompileGenerated_Order_k__BackingField = value;
		}

		public RequestFormSizeLimitAttribute(int valueCountLimit)
		{
			FormOptions formOption = new FormOptions();
			formOption.set_ValueCountLimit(valueCountLimit);
			formOption.set_KeyLengthLimit(valueCountLimit);
			this._formOptions = formOption;
		}

		public void OnAuthorization(AuthorizationFilterContext context)
		{
			IFeatureCollection features = context.get_HttpContext().get_Features();
			IFormFeature formFeature = features.Get<IFormFeature>();
			if (formFeature == null || formFeature.get_Form() == null)
			{
				features.Set<IFormFeature>(new FormFeature(context.get_HttpContext().get_Request(), this._formOptions));
			}
		}
	}
}