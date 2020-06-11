using System;
using System.Collections.Generic;

namespace Mono.Cecil.Extensions
{
    public static class EventDefinitionExtensions
    {
        // check invoke method as well?
        public static bool IsFinal(this EventDefinition self)
        {
            return (self.AddMethod == null || self.AddMethod.IsFinal) && (self.RemoveMethod == null || self.RemoveMethod.IsFinal);
        }

        public static bool IsNewSlot(this EventDefinition self)
        {
            return (self.AddMethod == null || self.AddMethod.IsNewSlot) && (self.RemoveMethod == null || self.RemoveMethod.IsNewSlot);
        }

        public static bool IsAbstract(this EventDefinition self)
        {
            return (self.AddMethod == null || self.AddMethod.IsAbstract) && (self.RemoveMethod == null || self.RemoveMethod.IsAbstract);
        }

        public static bool IsVirtual(this EventDefinition self)
        {
            return (self.AddMethod == null || self.AddMethod.IsVirtual) && (self.RemoveMethod == null || self.RemoveMethod.IsVirtual);
        }

        public static bool IsStatic(this EventDefinition self)
        {
            return (self.AddMethod == null || self.AddMethod.IsStatic) && (self.RemoveMethod == null || self.RemoveMethod.IsStatic);
        }

        public static bool IsExplicitImplementation(this EventDefinition self)
        {
            if (self == null)
            {
                return false;
            }
            if (self.AddMethod != null)
            {
                if (self.AddMethod.HasOverrides && self.AddMethod.IsPrivate)
                {
                    return true;
                }
            }
            if (self.RemoveMethod != null)
            {
                if (self.RemoveMethod.HasOverrides && self.RemoveMethod.IsPrivate)
                {
                    return true;
                }
            }
            return false;
        }

		public static bool IsExplicitImplementationOf(this EventDefinition self, TypeDefinition @interface)
		{
			if (@interface == null)
			{
				throw new ArgumentNullException("@interface can not be null.");
			}

			if (!@interface.IsInterface)
			{
				throw new ArgumentOutOfRangeException("The @interface argument is not an interface definition.");
			}

			if (!self.IsExplicitImplementation())
			{
				return false;
			}
			
			if (self.DeclaringType.FullName == @interface.FullName)
			{
				return true;
			}

			if (self.AddMethod != null)
			{
				if (!self.AddMethod.IsExplicitImplementationOf(@interface))
				{
					return false;
				}
			}

			if (self.RemoveMethod != null)
			{
				if (!self.RemoveMethod.IsExplicitImplementationOf(@interface))
				{
					return false;
				}
			}

			return true;
		}

		private static EventDefinition GetMethodDeclaringEvent(MethodReference method)
		{
			if (method == null)
			{
				return null;
			}

			TypeDefinition type = method.DeclaringType.Resolve();
			if (type != null)
			{
				foreach (EventDefinition @event in type.Events)
				{
					if ((@event.AddMethod != null && @event.AddMethod.HasSameSignatureWith(method)) ||
						(@event.RemoveMethod != null && @event.RemoveMethod.HasSameSignatureWith(method)))
					{
						return @event;
					}
				}
			}

			return null;
		}

		public static ICollection<EventDefinition> GetOverridenAndImplementedEvents(this EventDefinition self)
		{
			List<EventDefinition> result = new List<EventDefinition>();

			result.Add(self);

			if (self.AddMethod != null)
			{
				ICollection<MethodDefinition> implementedAddMethods = self.AddMethod.GetOverridenAndImplementedMethods();
				foreach (MethodDefinition implementedAddMethod in implementedAddMethods)
				{
					EventDefinition implementedEvent = GetMethodDeclaringEvent(implementedAddMethod);

					if (implementedEvent != null)
					{
						result.Add(implementedEvent);
					}
				}
			}
			else if (self.RemoveMethod != null)
			{
				ICollection<MethodDefinition> implementedRemoveMethods = self.RemoveMethod.GetOverridenAndImplementedMethods();
				foreach (MethodDefinition implementedRemoveMethod in implementedRemoveMethods)
				{
					EventDefinition implementedEvent = GetMethodDeclaringEvent(implementedRemoveMethod);

					if (implementedEvent != null)
					{
						result.Add(implementedEvent);
					}
				}
			}

			return result;
		}

		public static ICollection<ImplementedMember> GetImplementedEvents(this EventDefinition self)
		{
			List<ImplementedMember> result = new List<ImplementedMember>();

			if (self.AddMethod != null)
			{
				ICollection<ImplementedMember> implementedAddMethods = self.AddMethod.GetImplementedMethods();
				foreach (ImplementedMember implementedAddMethod in implementedAddMethods)
				{
					EventDefinition implementedEvent = GetMethodDeclaringEvent(implementedAddMethod.Member as MethodReference);
					if (implementedEvent != null)
					{
						result.Add(new ImplementedMember(implementedAddMethod.DeclaringType, implementedEvent));
					}
				}

				return result;
			}

			if (self.RemoveMethod != null)
			{
				ICollection<ImplementedMember> implementedRemoveMethods = self.RemoveMethod.GetImplementedMethods();
				foreach (ImplementedMember implementedRemoveMethod in implementedRemoveMethods)
				{
					EventDefinition implementedEvent = GetMethodDeclaringEvent(implementedRemoveMethod.Member as MethodReference);
					if (implementedEvent != null)
					{
						result.Add(new ImplementedMember(implementedRemoveMethod.DeclaringType, implementedEvent));
					}
				}

				return result;
			}

			return result;
		}

		public static ICollection<ImplementedMember> GetExplicitlyImplementedEvents(this EventDefinition self)
		{
			List<ImplementedMember> result = new List<ImplementedMember>();

			if (!self.IsExplicitImplementation())
			{
				return result;
			}

			if (self.AddMethod != null)
			{
				ICollection<ImplementedMember> explicitlyImplementedAddMethods = self.AddMethod.GetExplicitlyImplementedMethods();
				foreach (ImplementedMember explicitlyImplementedAddMethod in explicitlyImplementedAddMethods)
				{
					EventDefinition explicitlyImplementedEvent = GetMethodDeclaringEvent(explicitlyImplementedAddMethod.Member as MethodReference);
					if (explicitlyImplementedEvent != null)
					{
						result.Add(new ImplementedMember(explicitlyImplementedAddMethod.DeclaringType, explicitlyImplementedEvent));
					}
				}

				return result;
			}

			if (self.RemoveMethod != null)
			{
				ICollection<ImplementedMember> explicitlyImplementedRemoveMethods = self.RemoveMethod.GetExplicitlyImplementedMethods();
				foreach (ImplementedMember explicitlyImplementedRemoveMethod in explicitlyImplementedRemoveMethods)
				{
					EventDefinition explicitlyImplementedEvent = GetMethodDeclaringEvent(explicitlyImplementedRemoveMethod.Member as MethodReference);
					if (explicitlyImplementedEvent != null)
					{
						result.Add(new ImplementedMember(explicitlyImplementedRemoveMethod.DeclaringType, explicitlyImplementedEvent));
					}
				}

				return result;
			}

			return result;
		}
    }
}
