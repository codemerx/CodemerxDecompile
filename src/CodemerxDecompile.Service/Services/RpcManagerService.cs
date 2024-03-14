//    Copyright CodeMerx 2020
//    This file is part of CodemerxDecompile.

//    CodemerxDecompile is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.

//    CodemerxDecompile is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//    GNU Affero General Public License for more details.

//    You should have received a copy of the GNU Affero General Public License
//    along with CodemerxDecompile.  If not, see<https://www.gnu.org/licenses/>.

using System.Threading.Tasks;

using Grpc.Core;

using CodemerxDecompile.Service.Interfaces;

namespace CodemerxDecompile.Service.Services
{
    public class RpcManagerService : RpcManager.RpcManagerBase
    {
        private const string RunningStatus = "Running";

        private readonly IServiceManager serviceManager;

        public RpcManagerService(IServiceManager serviceManager)
        {
            this.serviceManager = serviceManager;
        }

        public override Task<GetServerStatusResponse> GetServerStatus(Empty request, ServerCallContext context)
        {
            GetServerStatusResponse response = new GetServerStatusResponse() { Status = RunningStatus };

            return Task.FromResult(response);
        }

        public override Task<Empty> ShutdownServer(Empty request, ServerCallContext context)
        {
            this.serviceManager.Shutdown();

            return Task.FromResult(new Empty());
        }
    }
}
