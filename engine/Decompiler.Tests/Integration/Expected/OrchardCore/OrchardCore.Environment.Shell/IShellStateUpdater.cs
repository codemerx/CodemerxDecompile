using System.Threading.Tasks;

namespace OrchardCore.Environment.Shell
{
	public interface IShellStateUpdater
	{
		Task ApplyChanges();
	}
}