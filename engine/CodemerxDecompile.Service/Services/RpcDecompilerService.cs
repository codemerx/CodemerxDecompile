using System.Threading.Tasks;

using Grpc.Core;

namespace CodemerxDecompile.Service
{
    public class RpcDecompilerService : RpcDecompiler.RpcDecompilerBase
    {
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Test Hello for " + request.Name
            });
        }
    }
}
