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
import { Empty } from 'vs/cd/platform/proto/common_pb';
import { ILoggerService, ILogService } from 'vs/platform/log/common/log';
import { IEnvironmentService } from 'vs/platform/environment/common/environment';
import { ILifecycleMainService } from 'vs/platform/lifecycle/electron-main/lifecycleMainService';

const SERVER_PATH = join(__dirname, '..', '..', '..', 'server', 'CodemerxDecompile.Service.dll');
const MAX_RETRIES = 10;
const RUNNING_STATUS = 'Running';
const WAIT_TIME_MILLISECONDS = 200;

export const IGrpcService = createDecorator<IGrpcService>('grpcService');

export interface IGrpcService {
	readonly _serviceBrand: undefined;

	initialize(): Promise<void>;
	getServiceUrl(): Promise<string>;
};

export class GrpcService implements IGrpcService {
	readonly _serviceBrand: undefined;

	private readonly port = 5000;

	constructor(
		@ILoggerService private readonly loggerService: ILoggerService,
		@IEnvironmentService private readonly environmentService: IEnvironmentService,
		@ILifecycleMainService private readonly lifecycleMainService: ILifecycleMainService) { }

	getServiceUrl(): Promise<string> {
		return Promise.resolve(`localhost:${this.port}`);
	}

	async initialize(): Promise<void> {
		const logService: ILogService = this.loggerService.getLogger(this.environmentService.codemerxDecompileLogResource);

		for (let count = 0; count < MAX_RETRIES; count += 1) {
			logService.info(`Starting CodemerxDecompile.Service on port ${this.port}.`);

			const serverProcess = spawn('dotnet', [SERVER_PATH, `--port=${this.port}`]);

			const milliseconds = count * WAIT_TIME_MILLISECONDS;
			logService.info(`Waiting CodemerxDecompile.Service response for ${milliseconds} Milliseconds.`);
			await this.sleep(milliseconds);

			try {
				const status = await this.getServerStatus();

				if (status === RUNNING_STATUS) {
					logService.info(`CodemerxDecompile.Service is listening on port ${this.port}.`);

					this.lifecycleMainService.onWillShutdown(async e => {
						logService.info(`Shutting down CodemerxDecompile.Service.`);
						e.join(this.shutdownServer());
					});

					return;
				} else {
					logService.error(`Failed starting CodemerxDecompile.Service on port "${this.port}". Status returned: ${status}.`);
				}
			} catch (error) {
				logService.error(`Failed starting CodemerxDecompile.Service on port "${this.port}". Error: ${error}.`);
			}

			serverProcess.kill();

			if (count < MAX_RETRIES - 1) {
				logService.info('Retrying.');
			} else {
				logService.info('Quitting.');

				await this.lifecycleMainService.quit();

				return;
			}
		}
	}

	private async getServerStatus(): Promise<string> {
		const client = await this.createGrpcClient();

		const status = await new Promise<string>((resolve, reject) => {
			client.getServerStatus(new Empty(), (error, response) => {
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

	private async shutdownServer(): Promise<void> {
		const client = await this.createGrpcClient();
		await new Promise<void>((resolve, reject) => {
			client.shutdownServer(new Empty(), (err, response) => {
				if (err) {
					reject(`ShutdownServer() failed. Error: ${err}`);
					return;
				}

				resolve();
			});
		});
	}

	private async createGrpcClient(): Promise<RpcManagerClient> {
		const url = await this.getServiceUrl();
		return new RpcManagerClient(url, grpc.credentials.createInsecure());
	}

	private sleep(millisecond: number): Promise<void> {
		return new Promise(resolve => setTimeout(resolve, millisecond));
	}
};
