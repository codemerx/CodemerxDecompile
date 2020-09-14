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

import { createDecorator } from 'vs/platform/instantiation/common/instantiation';
import { join } from 'path';
import { spawn } from 'child_process';
import * as grpc from "@grpc/grpc-js";
import { RpcManagerClient } from 'vs/cd/platform/proto/manager_grpc_pb';
import { GetServerStatusRequest } from 'vs/cd/platform/proto/manager_pb';

export const IGrpcMainService = createDecorator<IGrpcMainService>('grpcService');

export interface IGrpcMainService {
	readonly _serviceBrand: undefined;

	initialize(): void;
	getServiceUrl(): Promise<string>;
};

export class GrpcMainService implements IGrpcMainService {
	readonly _serviceBrand: undefined;

	private readonly port = 5000;

	getServiceUrl(): Promise<string> {
		return Promise.resolve(`localhost:${this.port}`);
	}

	initialize(): Promise<void> {
		return new Promise((resolve, reject) => {
			const serverPath = join(__dirname, '..', '..', '..', 'server', 'CodemerxDecompile.Service.dll');

			spawn('dotnet', [serverPath, `--port=${this.port}`]);

			this.getServiceUrl().then(url => {
				const statusService = new RpcManagerClient(url, grpc.credentials.createInsecure());
				statusService.getServerStatus(new GetServerStatusRequest(), (error, response) => {
					if (!error) {
						console.log(response?.getStatus());
						resolve();
					}
					else {
						reject(error);
					}
				});
			})
		});
	}
};
