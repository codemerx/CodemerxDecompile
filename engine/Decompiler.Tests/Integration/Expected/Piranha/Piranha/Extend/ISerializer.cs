using System;

namespace Piranha.Extend
{
	public interface ISerializer
	{
		object Deserialize(string str);

		string Serialize(object obj);
	}
}