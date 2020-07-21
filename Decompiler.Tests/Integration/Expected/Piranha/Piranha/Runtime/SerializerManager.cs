using Piranha.Extend;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Piranha.Runtime
{
	public sealed class SerializerManager
	{
		private readonly Dictionary<Type, ISerializer> _serializers;

		public ISerializer this[Type type]
		{
			get
			{
				V_0 = null;
				if (this._serializers.TryGetValue(type, out V_0))
				{
					return V_0;
				}
				return null;
			}
		}

		public SerializerManager()
		{
			base();
			this._serializers = new Dictionary<Type, ISerializer>();
			return;
		}

		public void Register<T>(ISerializer serializer)
		{
			this._serializers.set_Item(Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Piranha.Runtime.SerializerManager::Register(Piranha.Extend.ISerializer)
			// Exception in: System.Void Register(Piranha.Extend.ISerializer)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public void UnRegister<T>()
		{
			dummyVar0 = this._serializers.Remove(Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Piranha.Runtime.SerializerManager::UnRegister()
			// Exception in: System.Void UnRegister()
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

	}
}