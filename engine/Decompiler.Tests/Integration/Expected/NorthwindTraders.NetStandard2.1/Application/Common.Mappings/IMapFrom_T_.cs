using AutoMapper;
using System;

namespace Northwind.Application.Common.Mappings
{
	public interface IMapFrom<T>
	{
		void Mapping(Profile profile)
		{
			profile.CreateMap(typeof(T), this.GetType());
		}
	}
}