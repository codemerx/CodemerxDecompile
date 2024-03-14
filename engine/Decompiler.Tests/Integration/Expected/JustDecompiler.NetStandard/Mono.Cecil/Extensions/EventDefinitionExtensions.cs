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
			List<ImplementedMember> implementedMembers = new List<ImplementedMember>();
			if (!self.IsExplicitImplementation())
			{
				return implementedMembers;
			}
			if (self.get_AddMethod() != null)
			{
				foreach (ImplementedMember explicitlyImplementedMethod in self.get_AddMethod().GetExplicitlyImplementedMethods())
				{
					EventDefinition methodDeclaringEvent = EventDefinitionExtensions.GetMethodDeclaringEvent(explicitlyImplementedMethod.Member as MethodReference);
					if (methodDeclaringEvent == null)
					{
						continue;
					}
					implementedMembers.Add(new ImplementedMember(explicitlyImplementedMethod.DeclaringType, methodDeclaringEvent));
				}
				return implementedMembers;
			}
			if (self.get_RemoveMethod() == null)
			{
				return implementedMembers;
			}
			foreach (ImplementedMember implementedMember in self.get_RemoveMethod().GetExplicitlyImplementedMethods())
			{
				EventDefinition eventDefinition = EventDefinitionExtensions.GetMethodDeclaringEvent(implementedMember.Member as MethodReference);
				if (eventDefinition == null)
				{
					continue;
				}
				implementedMembers.Add(new ImplementedMember(implementedMember.DeclaringType, eventDefinition));
			}
			return implementedMembers;
		}

		public static ICollection<ImplementedMember> GetImplementedEvents(this EventDefinition self)
		{
			List<ImplementedMember> implementedMembers = new List<ImplementedMember>();
			if (self.get_AddMethod() != null)
			{
				foreach (ImplementedMember implementedMethod in self.get_AddMethod().GetImplementedMethods())
				{
					EventDefinition methodDeclaringEvent = EventDefinitionExtensions.GetMethodDeclaringEvent(implementedMethod.Member as MethodReference);
					if (methodDeclaringEvent == null)
					{
						continue;
					}
					implementedMembers.Add(new ImplementedMember(implementedMethod.DeclaringType, methodDeclaringEvent));
				}
				return implementedMembers;
			}
			if (self.get_RemoveMethod() == null)
			{
				return implementedMembers;
			}
			foreach (ImplementedMember implementedMember in self.get_RemoveMethod().GetImplementedMethods())
			{
				EventDefinition eventDefinition = EventDefinitionExtensions.GetMethodDeclaringEvent(implementedMember.Member as MethodReference);
				if (eventDefinition == null)
				{
					continue;
				}
				implementedMembers.Add(new ImplementedMember(implementedMember.DeclaringType, eventDefinition));
			}
			return implementedMembers;
		}

		private static EventDefinition GetMethodDeclaringEvent(MethodReference method)
		{
			EventDefinition eventDefinition;
			if (method == null)
			{
				return null;
			}
			TypeDefinition typeDefinition = method.get_DeclaringType().Resolve();
			if (typeDefinition != null)
			{
				Collection<EventDefinition>.Enumerator enumerator = typeDefinition.get_Events().GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						EventDefinition current = enumerator.get_Current();
						if ((current.get_AddMethod() == null || !current.get_AddMethod().HasSameSignatureWith(method)) && (current.get_RemoveMethod() == null || !current.get_RemoveMethod().HasSameSignatureWith(method)))
						{
							continue;
						}
						eventDefinition = current;
						return eventDefinition;
					}
					return null;
				}
				finally
				{
					enumerator.Dispose();
				}
				return eventDefinition;
			}
			return null;
		}

		public static ICollection<EventDefinition> GetOverridenAndImplementedEvents(this EventDefinition self)
		{
			List<EventDefinition> eventDefinitions = new List<EventDefinition>()
			{
				self
			};
			if (self.get_AddMethod() != null)
			{
				foreach (MethodDefinition overridenAndImplementedMethod in self.get_AddMethod().GetOverridenAndImplementedMethods())
				{
					EventDefinition methodDeclaringEvent = EventDefinitionExtensions.GetMethodDeclaringEvent(overridenAndImplementedMethod);
					if (methodDeclaringEvent == null)
					{
						continue;
					}
					eventDefinitions.Add(methodDeclaringEvent);
				}
			}
			else if (self.get_RemoveMethod() != null)
			{
				foreach (MethodDefinition methodDefinition in self.get_RemoveMethod().GetOverridenAndImplementedMethods())
				{
					EventDefinition eventDefinition = EventDefinitionExtensions.GetMethodDeclaringEvent(methodDefinition);
					if (eventDefinition == null)
					{
						continue;
					}
					eventDefinitions.Add(eventDefinition);
				}
			}
			return eventDefinitions;
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

		public static bool IsExplicitImplementationOf(this EventDefinition self, TypeDefinition @interface)
		{
			if (@interface == null)
			{
				throw new ArgumentNullException("@interface can not be null.");
			}
			if (!@interface.get_IsInterface())
			{
				throw new ArgumentOutOfRangeException("The @interface argument is not an interface definition.");
			}
			if (!self.IsExplicitImplementation())
			{
				return false;
			}
			if (self.get_DeclaringType().get_FullName() == @interface.get_FullName())
			{
				return true;
			}
			if (self.get_AddMethod() != null && !self.get_AddMethod().IsExplicitImplementationOf(@interface))
			{
				return false;
			}
			if (self.get_RemoveMethod() != null && !self.get_RemoveMethod().IsExplicitImplementationOf(@interface))
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