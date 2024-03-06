using System.Threading.Tasks;

namespace CodemerxDecompile.Services;

public interface IAutoUpdateService
{
    Task CheckForNewerVersionAsync();
}
