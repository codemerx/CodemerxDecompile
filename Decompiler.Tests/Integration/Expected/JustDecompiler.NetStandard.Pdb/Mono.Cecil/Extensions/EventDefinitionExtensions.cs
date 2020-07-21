using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Mono.Cecil.Extensions
{
	public static class EventDefinitionExtensions
	{
		public static ICollection<ImplementedMember> GetExplicitlyImplementedEvents(this EventDefinition self)
		{
			V_0 = new List<ImplementedMember>();
			if (!self.IsExplicitImplementation())
			{
				return V_0;
			}
			if (self.get_AddMethod() != null)
			{
				V_1 = self.get_AddMethod().GetExplicitlyImplementedMethods().GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_2 = V_1.get_Current();
						V_3 = EventDefinitionExtensions.GetMethodDeclaringEvent(V_2.get_Member() as MethodReference);
						if (V_3 == null)
						{
							continue;
						}
						V_0.Add(new ImplementedMember(V_2.get_DeclaringType(), V_3));
					}
				}
				finally
				{
					if (V_1 != null)
					{
						V_1.Dispose();
					}
				}
				return V_0;
			}
			if (self.get_RemoveMethod() == null)
			{
				return V_0;
			}
			V_1 = self.get_RemoveMethod().GetExplicitlyImplementedMethods().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_4 = V_1.get_Current();
					V_5 = EventDefinitionExtensions.GetMethodDeclaringEvent(V_4.get_Member() as MethodReference);
					if (V_5 == null)
					{
						continue;
					}
					V_0.Add(new ImplementedMember(V_4.get_DeclaringType(), V_5));
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			return V_0;
		}

		public static ICollection<ImplementedMember> GetImplementedEvents(this EventDefinition self)
		{
			V_0 = new List<ImplementedMember>();
			if (self.get_AddMethod() != null)
			{
				V_1 = self.get_AddMethod().GetImplementedMethods().GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_2 = V_1.get_Current();
						V_3 = EventDefinitionExtensions.GetMethodDeclaringEvent(V_2.get_Member() as MethodReference);
						if (V_3 == null)
						{
							continue;
						}
						V_0.Add(new ImplementedMember(V_2.get_DeclaringType(), V_3));
					}
				}
				finally
				{
					if (V_1 != null)
					{
						V_1.Dispose();
					}
				}
				return V_0;
			}
			if (self.get_RemoveMethod() == null)
			{
				return V_0;
			}
			V_1 = self.get_RemoveMethod().GetImplementedMethods().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_4 = V_1.get_Current();
					V_5 = EventDefinitionExtensions.GetMethodDeclaringEvent(V_4.get_Member() as MethodReference);
					if (V_5 == null)
					{
						continue;
					}
					V_0.Add(new ImplementedMember(V_4.get_DeclaringType(), V_5));
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			return V_0;
		}

		private static EventDefinition GetMethodDeclaringEvent(MethodReference method)
		{
			if (method == null)
			{
				return null;
			}
			V_0 = method.get_DeclaringType().Resolve();
			if (V_0 != null)
			{
				V_1 = V_0.get_Events().GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_2 = V_1.get_Current();
						if (V_2.get_AddMethod() == null || !V_2.get_AddMethod().HasSameSignatureWith(method) && V_2.get_RemoveMethod() == null || !V_2.get_RemoveMethod().HasSameSignatureWith(method))
						{
							continue;
						}
						V_3 = V_2;
						goto Label1;
					}
					goto Label0;
				}
				finally
				{
					V_1.Dispose();
				}
			Label1:
				return V_3;
			}
		Label0:
			return null;
		}

		public static ICollection<EventDefinition> GetOverridenAndImplementedEvents(this EventDefinition self)
		{
			V_0 = new List<EventDefinition>();
			V_0.Add(self);
			if (self.get_AddMethod() == null)
			{
				if (self.get_RemoveMethod() != null)
				{
					V_1 = self.get_RemoveMethod().GetOverridenAndImplementedMethods().GetEnumerator();
					try
					{
						while (V_1.MoveNext())
						{
							V_3 = EventDefinitionExtensions.GetMethodDeclaringEvent(V_1.get_Current());
							if (V_3 == null)
							{
								continue;
							}
							V_0.Add(V_3);
						}
					}
					finally
					{
						if (V_1 != null)
						{
							V_1.Dispose();
						}
					}
				}
			}
			else
			{
				V_1 = self.get_AddMethod().GetOverridenAndImplementedMethods().GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_2 = EventDefinitionExtensions.GetMethodDeclaringEvent(V_1.get_Current());
						if (V_2 == null)
						{
							continue;
						}
						V_0.Add(V_2);
					}
				}
				finally
				{
					if (V_1 != null)
					{
						V_1.Dispose();
					}
				}
			}
			return V_0;
		}

		public static bool IsAbstract(this EventDefinition self)
		{
			if (self.get_AddMethod() != null && !self.get_AddMethod().get_IsAbstract())
			{
				return false;
			}
			if (self.get_RemoveMethod() == null)
			{
				return true;
			}
			return self.get_RemoveMethod().get_IsAbstract();
		}

		public static bool IsExplicitImplementation(this EventDefinition self)
		{
			if (self == null)
			{
				return false;
			}
			if (self.get_AddMethod() != null && self.get_AddMethod().get_HasOverrides() && self.get_AddMethod().get_IsPrivate())
			{
				return true;
			}
			if (self.get_RemoveMethod() != null && self.get_RemoveMethod().get_HasOverrides() && self.get_RemoveMethod().get_IsPrivate())
			{
				return true;
			}
			return false;
		}

		public static bool IsExplicitImplementationOf(this EventDefinition self, TypeDefinition interface)
		{
			if (interface == null)
			{
				throw new ArgumentNullException("@interface can not be null.");
			}
			if (!interface.get_IsInterface())
			{
				throw new ArgumentOutOfRangeException("The @interface argument is not an interface definition.");
			}
			if (!self.IsExplicitImplementation())
			{
				return false;
			}
			if (String.op_Equality(self.get_DeclaringType().get_FullName(), interface.get_FullName()))
			{
				return true;
			}
			if (self.get_AddMethod() != null && !self.get_AddMethod().IsExplicitImplementationOf(interface))
			{
				return false;
			}
			if (self.get_RemoveMethod() != null && !self.get_RemoveMethod().IsExplicitImplementationOf(interface))
			{
				return false;
			}
			return true;
		}

		public static bool IsFinal(this EventDefinition self)
		{
			if (self.get_AddMethod() != null && !self.get_AddMethod().get_IsFinal())
			{
				return false;
			}
			if (self.get_RemoveMethod() == null)
			{
				return true;
			}
			return self.get_RemoveMethod().get_IsFinal();
		}

		public static bool IsNewSlot(this EventDefinition self)
		{
			if (self.get_AddMethod() != null && !self.get_AddMethod().get_IsNewSlot())
			{
				return false;
			}
			if (self.get_RemoveMethod() == null)
			{
				return true;
			}
			return self.get_RemoveMethod().get_IsNewSlot();
		}

		public static bool IsStatic(this EventDefinition self)
		{
			if (self.get_AddMethod() != null && !self.get_AddMethod().get_IsStatic())
			{
				return false;
			}
			if (self.get_RemoveMethod() == null)
			{
				return true;
			}
			return self.get_RemoveMethod().get_IsStatic();
		}

		public static bool IsVirtual(this EventDefinition self)
		{
			if (self.get_AddMethod() != null && !self.get_AddMethod().get_IsVirtual())
			{
				return false;
			}
			if (self.get_RemoveMethod() == null)
			{
				return true;
			}
			return self.get_RemoveMethod().get_IsVirtual();
		}
	}
}