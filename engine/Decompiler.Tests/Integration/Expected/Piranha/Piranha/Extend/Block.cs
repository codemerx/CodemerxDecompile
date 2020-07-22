using Piranha.Runtime;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Piranha.Extend
{
	public abstract class Block
	{
		public Guid Id
		{
			get;
			set;
		}

		public string Type
		{
			get;
			set;
		}

		protected Block()
		{
			base();
			return;
		}

		public virtual string GetTitle()
		{
			V_0 = App.get_Blocks().GetByType(this.GetType());
			V_1 = "[Not Implemented]";
			if (!String.IsNullOrEmpty(V_0.get_ListTitleField()))
			{
				V_2 = this.GetType().GetProperty(V_0.get_ListTitleField(), App.get_PropertyBindings());
				if (PropertyInfo.op_Inequality(V_2, null) && System.Type.GetTypeFromHandle(// 
				// Current member / type: System.String Piranha.Extend.Block::GetTitle()
				// Exception in: System.String GetTitle()
				// Specified method is not supported.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com

	}
}