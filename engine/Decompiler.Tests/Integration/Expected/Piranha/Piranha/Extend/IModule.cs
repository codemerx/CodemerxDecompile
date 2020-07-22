using System;

namespace Piranha.Extend
{
	public interface IModule
	{
		string Author
		{
			get;
		}

		string Description
		{
			get;
		}

		string IconUrl
		{
			get;
		}

		string Name
		{
			get;
		}

		string PackageUrl
		{
			get;
		}

		string Version
		{
			get;
		}

		void Init();
	}
}