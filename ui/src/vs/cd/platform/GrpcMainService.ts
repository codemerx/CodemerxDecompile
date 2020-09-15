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

const SERVER_PATH = join(__dirname, '..', '..', '..', 'server', 'CodemerxDecompile.Service.exe');
const MAX_RETRIES = 5;
const RUNNING_STATUS = 'Running';

export const IGrpcMainService = createDecorator<IGrpcMainService>('grpcService');

export interface IGrpcMainService {
	readonly _serviceBrand: undefined;

	initialize(): void;
	getServiceUrl(): Promise<string>;
};

export class GrpcMainService implements IGrpcMainService {
	readonly _serviceBrand: undefined;

	private readonly port = 5000;

	private async getServerStatus(): Promise<string> {
		const url = await this.getServiceUrl();
		const statusService = new RpcManagerClient(url, grpc.credentials.createInsecure());

		const status = await new Promise<string>((resolve, reject) => {
			statusService.getServerStatus(new GetServerStatusRequest(), (error, response) => {
				if (!error) {
					resolve(response?.getStatus());
				}
				else {
					reject(error);
				}
			});
		});

		return status;
	}

	getServiceUrl(): Promise<string> {
		return Promise.resolve(`localhost:${this.port}`);
	}

	async initialize(): Promise<void> {
		console.info('Starting CodemerxDecompile.Service...');

		let count = 1;
		while (count <= MAX_RETRIES) {
			spawn('dotnet', [SERVER_PATH, `--port=${this.port}`]);

			try {
				const status = await this.getServerStatus();

				if (status == RUNNING_STATUS) {
					console.info(`CodemerxDecompile.Service is listening on port ${this.port}.`);

					return;
				}
			} catch (error) {
				if (count < MAX_RETRIES) {
					console.error(`Failed starting CodemerxDecompile.Service on port "${this.port}" with reason: ${error}. Retrying attempt.`);
				} else {
					throw error;
				}
			}

			count++;
		}
	}
};
