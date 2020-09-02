import { IMainProcessService } from 'vs/platform/ipc/electron-sandbox/mainProcessService';
import { createChannelSender } from 'vs/base/parts/ipc/common/ipc';
import { createDecorator } from 'vs/platform/instantiation/common/instantiation';

export const IGrpcService = createDecorator<IGrpcService>('grpcService');

export interface IGrpcService {
	readonly _serviceBrand: undefined;

	getServiceUrl(): Promise<string>;
}

export class GrpcService {
	readonly _serviceBrand: undefined;

	constructor(@IMainProcessService mainProcessService: IMainProcessService) {
		return createChannelSender<IGrpcService>(mainProcessService.getChannel('grpc'));
	}
}
