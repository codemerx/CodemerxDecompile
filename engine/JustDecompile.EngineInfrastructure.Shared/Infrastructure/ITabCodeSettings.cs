using System;

namespace JustDecompile.EngineInfrastructure
{
	public interface ITabCodeSettings : IEquatable<ITabCodeSettings>
	{
		bool ShowCodeDocumentation { get; set; }
	}
}
