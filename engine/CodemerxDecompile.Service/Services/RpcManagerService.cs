using Grpc.Core;
using System.Threading.Tasks;

namespace CodemerxDecompile.Service.Services
{
    public class RpcManagerService : RpcManager.RpcManagerBase
    {
        private const string RunningStatus =  "Running";

        public override Task<GetServerStatusResponse> GetServerStatus(GetServerStatusRequest request, ServerCallContext context)
        {
            GetServerStatusResponse response = new GetServerStatusResponse() { Status = RunningStatus };

            return Task.FromResult(response);
        }
    }
}
