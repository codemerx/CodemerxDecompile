using System;
using System.Threading.Tasks;

namespace Piranha
{
	public interface ISecurity
	{
		Task<bool> SignIn(object context, string username, string password);

		Task SignOut(object context);
	}
}