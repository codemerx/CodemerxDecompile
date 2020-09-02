import { createDecorator } from 'vs/platform/instantiation/common/instantiation';
import { join } from 'path';
import { spawn } from 'child_process';

export const IGrpcMainService = createDecorator<IGrpcMainService>('grpcService');

interface IGrpcMainService {
	readonly _serviceBrand: undefined;

	initialize(): void;
	getServiceUrl() : Promise<string>;
};

export class GrpcMainService implements IGrpcMainService {
	readonly _serviceBrand: undefined;

	private readonly port = 5000;

	getServiceUrl(): Promise<string> {
		return Promise.resolve(`http://localhost:${this.port}/`);
	}

	initialize(): Promise<void> {
		return new Promise((resolve, reject) => {
			const serverPath = join(__dirname, '..', '..', '..', 'server', 'CodemerxDecompile.Service.exe');

			spawn(serverPath, [`--port=${this.port}`]);

			resolve();
		});
	}
};
