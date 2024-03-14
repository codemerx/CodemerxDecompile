using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Northwind.Application.Common.Mappings
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			this.ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
		}

		private void ApplyMappingsFromAssembly(Assembly assembly)
		{
			List<Type> list = (
				from t in (IEnumerable<Type>)assembly.GetExportedTypes()
				where ((IEnumerable<Type>)t.GetInterfaces()).Any<Type>((Type i) => (!i.IsGenericType ? false : i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
				select t).ToList<Type>();
			foreach (Type type in list)
			{
				object obj = Activator.CreateInstance(type);
				MethodInfo method = type.GetMethod("Mapping");
				if (method != null)
				{
					method.Invoke(obj, new Object[] { this });
				}
			}
		}
	}
}