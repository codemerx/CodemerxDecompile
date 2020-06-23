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
			if (self.AddMethod != null)
			{
				foreach (ImplementedMember explicitlyImplementedMethod in self.AddMethod.GetExplicitlyImplementedMethods())
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
			if (self.RemoveMethod == null)
			{
				return implementedMembers;
			}
			foreach (ImplementedMember implementedMember in self.RemoveMethod.GetExplicitlyImplementedMethods())
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
			if (self.AddMethod != null)
			{
				foreach (ImplementedMember implementedMethod in self.AddMethod.GetImplementedMethods())
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
			if (self.RemoveMethod == null)
			{
				return implementedMembers;
			}
			foreach (ImplementedMember implementedMember in self.RemoveMethod.GetImplementedMethods())
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
			TypeDefinition typeDefinition = method.DeclaringType.Resolve();
			if (typeDefinition != null)
			{
				Collection<EventDefinition>.Enumerator enumerator = typeDefinition.Events.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						EventDefinition current = enumerator.Current;
						if ((current.AddMethod == null || !current.AddMethod.HasSameSignatureWith(method)) && (current.RemoveMethod == null || !current.RemoveMethod.HasSameSignatureWith(method)))
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
					((IDisposable)enumerator).Dispose();
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
			if (self.AddMethod != null)
			{
				foreach (MethodDefinition overridenAndImplementedMethod in self.AddMethod.GetOverridenAndImplementedMethods())
				{
					EventDefinition methodDeclaringEvent = EventDefinitionExtensions.GetMethodDeclaringEvent(overridenAndImplementedMethod);
					if (methodDeclaringEvent == null)
					{
						continue;
					}
					eventDefinitions.Add(methodDeclaringEvent);
				}
			}
			else if (self.RemoveMethod != null)
			{
				foreach (MethodDefinition methodDefinition in self.RemoveMethod.GetOverridenAndImplementedMethods())
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
			if (self.AddMethod != null && !self.AddMethod.IsAbstract)
			{
				return false;
			}
			if (self.RemoveMethod == null)
			{
				return true;
			}
			return self.RemoveMethod.IsAbstract;
		}

		public static bool IsExplicitImplementation(this EventDefinition self)
		{
			if (self == null)
			{
				return false;
			}
			if (self.AddMethod != null && self.AddMethod.HasOverrides && self.AddMethod.IsPrivate)
			{
				return true;
			}
			if (self.RemoveMethod != null && self.RemoveMethod.HasOverrides && self.RemoveMethod.IsPrivate)
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
			if (self.AddMethod != null && !self.AddMethod.IsExplicitImplementationOf(@interface))
			{
				return false;
			}
			if (self.RemoveMethod != null && !self.RemoveMethod.IsExplicitImplementationOf(@interface))
			{
				return false;
			}
			return true;
		}

		public static bool IsFinal(this EventDefinition self)
		{
			if (self.AddMethod != null && !self.AddMethod.IsFinal)
			{
				return false;
			}
			if (self.RemoveMethod == null)
			{
				return true;
			}
			return self.RemoveMethod.IsFinal;
		}

		public static bool IsNewSlot(this EventDefinition self)
		{
			if (self.AddMethod != null && !self.AddMethod.IsNewSlot)
			{
				return false;
			}
			if (self.RemoveMethod == null)
			{
				return true;
			}
			return self.RemoveMethod.IsNewSlot;
		}

		public static bool IsStatic(this EventDefinition self)
		{
			if (self.AddMethod != null && !self.AddMethod.IsStatic)
			{
				return false;
			}
			if (self.RemoveMethod == null)
			{
				return true;
			}
			return self.RemoveMethod.IsStatic;
		}

		public static bool IsVirtual(this EventDefinition self)
		{
			if (self.AddMethod != null && !self.AddMethod.IsVirtual)
			{
				return false;
			}
			if (self.RemoveMethod == null)
			{
				return true;
			}
			return self.RemoveMethod.IsVirtual;
		}
	}
}